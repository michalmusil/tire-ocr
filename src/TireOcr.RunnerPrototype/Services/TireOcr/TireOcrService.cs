using System.Diagnostics;
using TireOcr.RunnerPrototype.Clients.ImageDownload;
using TireOcr.RunnerPrototype.Clients.Ocr;
using TireOcr.RunnerPrototype.Clients.Postprocessing;
using TireOcr.RunnerPrototype.Clients.Preprocessing;
using TireOcr.RunnerPrototype.Dtos;
using TireOcr.RunnerPrototype.Dtos.Batch;
using TireOcr.RunnerPrototype.Extensions;
using TireOcr.RunnerPrototype.Models;
using TireOcr.RunnerPrototype.Services.CostEstimation;
using TireOcr.Shared.Result;

namespace TireOcr.RunnerPrototype.Services.TireOcr;

public class TireOcrService : ITireOcrService
{
    private const int MaxBatchSize = 10;

    private readonly IPreprocessingClient _preprocessingClient;
    private readonly IOcrClient _ocrClient;
    private readonly IPostprocessingClient _postprocessingClient;
    private readonly ICostEstimationService _costEstimationService;
    private readonly IImageDownloadClient _imageDownloadClient;
    private readonly ILogger<TireOcrService> _logger;

    public TireOcrService(IPreprocessingClient preprocessingClient, IOcrClient ocrClient,
        ILogger<TireOcrService> logger, IPostprocessingClient postprocessingClient,
        ICostEstimationService costEstimationService, IImageDownloadClient imageDownloadClient)
    {
        _preprocessingClient = preprocessingClient;
        _ocrClient = ocrClient;
        _logger = logger;
        _postprocessingClient = postprocessingClient;
        _costEstimationService = costEstimationService;
        _imageDownloadClient = imageDownloadClient;
    }

    public async Task<DataResult<TireOcrResult>> RunSingleOcrPipelineAsync(
        Image image,
        TireCodeDetectorType detectorType)
    {
        var totalStopwatch = new Stopwatch();
        totalStopwatch.Start();

        var preprocessingResult = await PerformTimeMeasuredTask("Preprocessing",
            () => _preprocessingClient.PreprocessImage(image)
        );
        if (preprocessingResult.Item2.IsFailure)
            return DataResult<TireOcrResult>.Failure(preprocessingResult.Item2.Failures);

        var preprocessedImage = preprocessingResult.Item2.Data!;

        var ocrResult = await PerformTimeMeasuredTask("Ocr",
            () => _ocrClient.PerformTireCodeOcrOnImage(preprocessedImage, detectorType)
        );
        if (ocrResult.Item2.IsFailure)
            return DataResult<TireOcrResult>.Failure(ocrResult.Item2.Failures);

        var ocrResultData = ocrResult.Item2.Data!;

        var postprocessingResult = await PerformTimeMeasuredTask("Postprocessing",
            () => _postprocessingClient.PostprocessTireCode(ocrResultData.DetectedCode));
        if (postprocessingResult.Item2.IsFailure)
            return DataResult<TireOcrResult>.Failure(ocrResult.Item2.Failures);

        var postprocessedTireCode = postprocessingResult.Item2.Data!;

        var estimatedCostsResult = ocrResultData.Billing is null
            ? null
            : await _costEstimationService.GetEstimatedCostsAsync(detectorType, ocrResultData.Billing);


        List<RunStat> runTrace =
        [
            preprocessingResult.Item1,
            ocrResult.Item1,
            postprocessingResult.Item1
        ];

        totalStopwatch.Stop();

        var finalResult = new TireOcrResult(
            image.FileName,
            postprocessedTireCode,
            detectorType,
            estimatedCostsResult?.Data,
            totalStopwatch.Elapsed.TotalMilliseconds,
            runTrace
        );
        return DataResult<TireOcrResult>.Success(finalResult);
    }

    public async Task<DataResult<TireOcrBatchResult>> RunOcrPipelineBatchAsync(
        IEnumerable<string> imageUrls,
        int batchSize,
        TireCodeDetectorType detectorType
    )
    {
        var urls = imageUrls.ToList();
        if (batchSize > MaxBatchSize)
            return DataResult<TireOcrBatchResult>.Invalid($"Batch size exceeds maximum of {MaxBatchSize}");

        var totalStopwatch = new Stopwatch();
        totalStopwatch.Start();

        var successfullyProcessed = new List<TireOcrResult>();
        var failed = new List<TireOcrFailure>();

        var batches = urls.ToList().GetSubLists(batchSize);
        foreach (var batch in batches)
        {
            var results = (await ProcessOcrPipelineBatch(detectorType, batch))
                .ToList();
            var successful = results
                .Where(r => r.ImageWithOcr.IsSuccess)
                .Select(r => r.ImageWithOcr.Data!)
                .Where(r => r.Result.IsSuccess)
                .Select(r => r.Result.Data!);
            successfullyProcessed.AddRange(successful);

            var failedDueToDownload = results
                .Where(r => r.ImageWithOcr.IsFailure)
                .Select(r => new TireOcrFailure(
                    r.ImageUrl,
                    r.ImageWithOcr.PrimaryFailure?.Message ?? "Failed to download image."
                ));

            var failedDueToOcr = results
                .Where(r => r.ImageWithOcr.IsSuccess)
                .Select(r => r.ImageWithOcr.Data!)
                .Where(r => r.Result.IsFailure)
                .Select(r => new TireOcrFailure(
                    r.Image.FileName,
                    r.Result.PrimaryFailure?.Message ?? "Ocr pipeline failed"
                ));
            failed.AddRange(failedDueToDownload);
            failed.AddRange(failedDueToOcr);
        }

        var totalEstimatedCosts = successfullyProcessed
            .Select(r => r.EstimatedCosts?.EstimatedCost ?? 0)
            .Sum();
        var estimatedCostsCurrency = successfullyProcessed
            .FirstOrDefault(r => r.EstimatedCosts is not null)
            ?.EstimatedCosts?.EstimatedCostCurrency;

        totalStopwatch.Stop();
        var result = new TireOcrBatchResult(
            Summary: new BatchSummaryDto(
                totalEstimatedCosts,
                estimatedCostsCurrency ?? "",
                totalStopwatch.Elapsed.TotalMilliseconds,
                new PipelineCompletionSuccessRateDto(
                    urls.Count,
                    successfullyProcessed.Count(),
                    failed.Count(),
                    double.Round((double)successfullyProcessed.Count / (double)urls.Count, 2)
                )
            ),
            SuccessfulResults: successfullyProcessed,
            Failures: failed
        );

        return DataResult<TireOcrBatchResult>.Success(result);
    }

    private async Task<IEnumerable<BatchFileResult>> ProcessOcrPipelineBatch(
        TireCodeDetectorType detectorType,
        IEnumerable<string> imageUrls
    )
    {
        var imageDownloadResults = (await _imageDownloadClient.DownloadImageBatch(imageUrls))
            .ToList();
        var notDownloadedImages = imageDownloadResults
            .Where(r => r.Result.IsFailure);
        var downloadedImages = imageDownloadResults
            .Where(r => r.Result.IsSuccess)
            .Select(r => new { Url = r.ImageUrl, Image = r.Result.Data! });


        var pipelineTasks = downloadedImages
            .Select(async res =>
            {
                var result = await RunSingleOcrPipelineAsync(res.Image, detectorType);
                return new { Url = res.Url, Image = res.Image, Result = result };
            });

        var processedResults = (await Task.WhenAll(pipelineTasks))
            .Select(x => new BatchFileResult(
                x.Url,
                DataResult<ImageWithOcrResult>.Success(new ImageWithOcrResult(x.Image, x.Result))
            ));
        var failedResults = notDownloadedImages
            .Select(x => new BatchFileResult(
                x.ImageUrl,
                DataResult<ImageWithOcrResult>.Failure(x.Result.Failures))
            );

        return [..processedResults, ..failedResults];
    }

    private async Task<(RunStat, T)> PerformTimeMeasuredTask<T>(string taskName, Func<Task<T>> performTask)
    {
        var stopWatch = new Stopwatch();
        stopWatch.Start();
        var result = await performTask();
        stopWatch.Stop();

        var timeTaken = stopWatch.Elapsed.TotalMilliseconds;
        return (new RunStat(taskName, timeTaken), result);
    }

    private record BatchFileResult(string ImageUrl, DataResult<ImageWithOcrResult> ImageWithOcr);

    private record ImageWithOcrResult(Image Image, DataResult<TireOcrResult> Result);
}