using System.Diagnostics;
using TireOcr.RunnerPrototype.Clients;
using TireOcr.RunnerPrototype.Dtos;
using TireOcr.RunnerPrototype.Models;
using TireOcr.RunnerPrototype.Services.CostEstimation;
using TireOcr.Shared.Result;

namespace TireOcr.RunnerPrototype.Services.TireOcr;

public class TireOcrService : ITireOcrService
{
    private readonly PreprocessingClient _preprocessingClient;
    private readonly OcrClient _ocrClient;
    private readonly PostprocessingClient _postprocessingClient;
    private readonly ICostEstimationService _costEstimationService;
    private readonly ILogger<TireOcrService> _logger;

    public TireOcrService(PreprocessingClient preprocessingClient, OcrClient ocrClient, ILogger<TireOcrService> logger,
        PostprocessingClient postprocessingClient, ICostEstimationService costEstimationService)
    {
        _preprocessingClient = preprocessingClient;
        _ocrClient = ocrClient;
        _logger = logger;
        _postprocessingClient = postprocessingClient;
        _costEstimationService = costEstimationService;
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

    public async Task<DataResult<TireOcrBatchResult>> RunOcrPipelineBatchAsync(IEnumerable<Image> images,
        TireCodeDetectorType detectorType)
    {
        var totalStopwatch = new Stopwatch();
        totalStopwatch.Start();
        var tasks = images
            .Select(async image =>
            {
                var result = await RunSingleOcrPipelineAsync(image, detectorType);
                return (image, result);
            });

        var intermediateResults = await Task.WhenAll(tasks);
        var successfulResults = intermediateResults
            .Where(x => x.result.IsSuccess)
            .Select(x => x.result.Data!)
            .ToList();
        var failingResults = intermediateResults
            .Where(x => x.result.IsFailure)
            .Select(x => new TireOcrFailure(
                x.image.FileName,
                x.result.PrimaryFailure?.Message ?? "Unknown error occurred")
            )
            .ToList();

        var totalEstimatedCosts = successfulResults
            .Select(r => r.EstimatedCosts?.EstimatedCost ?? 0)
            .Sum();
        var estimatedCostsCurrency = successfulResults
            .FirstOrDefault(r => r.EstimatedCosts is not null)
            ?.EstimatedCosts?.EstimatedCostCurrency;

        totalStopwatch.Stop();
        var result = new TireOcrBatchResult(
            Summary: new BatchSummaryDto(
                totalEstimatedCosts,
                estimatedCostsCurrency ?? "",
                totalStopwatch.Elapsed.TotalMilliseconds
            ),
            SuccessfulResults: successfulResults,
            Failures: failingResults
        );

        return DataResult<TireOcrBatchResult>.Success(result);
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
}