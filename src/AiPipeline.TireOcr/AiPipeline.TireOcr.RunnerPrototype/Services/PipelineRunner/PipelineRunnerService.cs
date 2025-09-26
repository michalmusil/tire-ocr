using System.Diagnostics;
using TireOcr.RunnerPrototype.Clients.DbMatching;
using TireOcr.RunnerPrototype.Clients.ImageDownload;
using TireOcr.RunnerPrototype.Clients.Ocr;
using TireOcr.RunnerPrototype.Clients.Postprocessing;
using TireOcr.RunnerPrototype.Clients.Preprocessing;
using TireOcr.RunnerPrototype.Dtos;
using TireOcr.RunnerPrototype.Dtos.Batch;
using TireOcr.RunnerPrototype.Extensions;
using TireOcr.RunnerPrototype.Models;
using TireOcr.Shared.Result;

namespace TireOcr.RunnerPrototype.Services.PipelineRunner;

public class PipelineRunnerService : IPipelineRunnerService
{
    private const int MaxBatchSize = 10;

    private readonly IPreprocessingClient _preprocessingClient;
    private readonly IOcrClient _ocrClient;
    private readonly IPostprocessingClient _postprocessingClient;
    private readonly IDbMatchingClient _dbMatchingClient;
    private readonly IImageDownloadClient _imageDownloadClient;
    private readonly ILogger<PipelineRunnerService> _logger;

    public PipelineRunnerService(IPreprocessingClient preprocessingClient, IOcrClient ocrClient,
        ILogger<PipelineRunnerService> logger, IPostprocessingClient postprocessingClient,
        IImageDownloadClient imageDownloadClient, IDbMatchingClient dbMatchingClient)
    {
        _preprocessingClient = preprocessingClient;
        _ocrClient = ocrClient;
        _logger = logger;
        _postprocessingClient = postprocessingClient;
        _imageDownloadClient = imageDownloadClient;
        _dbMatchingClient = dbMatchingClient;
    }

    public async Task<DataResult<TireOcrResultDto>> RunSingleOcrPipelineAsync(
        Image image,
        TireCodeDetectorType detectorType)
    {
        var totalStopwatch = new Stopwatch();
        totalStopwatch.Start();

        // var preprocessingResult = await PerformTimeMeasuredTask("Preprocessing",
        //     () => _preprocessingClient.PreprocessImage(image)
        // );
        // if (preprocessingResult.Item2.IsFailure)
        //     return DataResult<TireOcrResultDto>.Failure(preprocessingResult.Item2.Failures);
        //
        // var preprocessedImage = preprocessingResult.Item2.Data!;

        var ocrResult = await PerformTimeMeasuredTask("Ocr",
            () => _ocrClient.PerformTireCodeOcrOnImage(image, detectorType)
        );
        if (ocrResult.Item2.IsFailure)
            return DataResult<TireOcrResultDto>.Failure(ocrResult.Item2.Failures);

        var ocrResultData = ocrResult.Item2.Data!;

        var postprocessingResult = await PerformTimeMeasuredTask("Postprocessing",
            () => _postprocessingClient.PostprocessTireCode(ocrResultData.DetectedCode)
        );
        if (postprocessingResult.Item2.IsFailure)
            return DataResult<TireOcrResultDto>.Failure(postprocessingResult.Item2.Failures);

        var postprocessedTireCode = postprocessingResult.Item2.Data!;

        var dbMatchingResult = await PerformTimeMeasuredTask("DbMatching",
            () => _dbMatchingClient.GetDbMatchesForTireCode(postprocessedTireCode, ocrResultData.DetectedManufacturer));
        if (dbMatchingResult.Item2.IsFailure)
            return DataResult<TireOcrResultDto>.Failure(dbMatchingResult.Item2.Failures);

        var dbMatches = dbMatchingResult.Item2.Data!;

        List<RunStatDto> runTrace =
        [
            // preprocessingResult.Item1,
            ocrResult.Item1,
            postprocessingResult.Item1,
            dbMatchingResult.Item1
        ];

        totalStopwatch.Stop();

        var finalResult = new TireOcrResultDto(
            ImageFileName: image.FileName,
            DetectorType: detectorType,
            OcrResult: ocrResultData,
            PostprocessingResult: postprocessedTireCode,
            TasyDbMatchesResult: dbMatches,
            TotalDurationMs: totalStopwatch.Elapsed.TotalMilliseconds,
            RunTrace: runTrace
        );
        return DataResult<TireOcrResultDto>.Success(finalResult);
    }

    public async Task<DataResult<TireOcrBatchResultDto>> RunOcrPipelineBatchAsync(
        IEnumerable<string> imageUrls,
        int batchSize,
        TireCodeDetectorType detectorType
    )
    {
        var urls = imageUrls.ToList();
        if (batchSize > MaxBatchSize)
            return DataResult<TireOcrBatchResultDto>.Invalid($"Batch size exceeds maximum of {MaxBatchSize}");

        var totalStopwatch = new Stopwatch();
        totalStopwatch.Start();

        var successfullyProcessed = new List<TireOcrResultDto>();
        var failed = new List<TireOcrFailureDto>();

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
                .Select(r => new TireOcrFailureDto(
                    r.ImageUrl,
                    r.ImageWithOcr.PrimaryFailure?.Message ?? "Failed to download image."
                ));

            var failedDueToOcr = results
                .Where(r => r.ImageWithOcr.IsSuccess)
                .Select(r => r.ImageWithOcr.Data!)
                .Where(r => r.Result.IsFailure)
                .Select(r => new TireOcrFailureDto(
                    r.Image.FileName,
                    r.Result.PrimaryFailure?.Message ?? "Ocr pipeline failed"
                ));
            failed.AddRange(failedDueToDownload);
            failed.AddRange(failedDueToOcr);
        }

        var totalEstimatedCosts = successfullyProcessed
            .Select(r => r.OcrResult.EstimatedCosts?.EstimatedCost ?? 0)
            .Sum();
        var estimatedCostsCurrency = successfullyProcessed
            .FirstOrDefault(r => r.OcrResult.EstimatedCosts is not null)
            ?.OcrResult.EstimatedCosts?.EstimatedCostCurrency;

        totalStopwatch.Stop();
        var result = new TireOcrBatchResultDto(
            Summary: new BatchSummaryDto(
                totalEstimatedCosts,
                estimatedCostsCurrency ?? "",
                totalStopwatch.Elapsed.TotalMilliseconds,
                new PipelineCompletionSuccessRateDto(
                    urls.Count,
                    successfullyProcessed.Count,
                    failed.Count,
                    double.Round((double)successfullyProcessed.Count / (double)urls.Count, 2)
                )
            ),
            SuccessfulResults: successfullyProcessed,
            Failures: failed
        );

        return DataResult<TireOcrBatchResultDto>.Success(result);
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

    private async Task<(RunStatDto, T)> PerformTimeMeasuredTask<T>(string taskName, Func<Task<T>> performTask)
    {
        var stopWatch = new Stopwatch();
        stopWatch.Start();
        var result = await performTask();
        stopWatch.Stop();

        var timeTaken = stopWatch.Elapsed.TotalMilliseconds;
        return (new RunStatDto(taskName, timeTaken), result);
    }

    private record BatchFileResult(string ImageUrl, DataResult<ImageWithOcrResult> ImageWithOcr);

    private record ImageWithOcrResult(Image Image, DataResult<TireOcrResultDto> Result);
}