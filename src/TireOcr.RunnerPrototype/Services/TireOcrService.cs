using System.Diagnostics;
using TireOcr.RunnerPrototype.Clients;
using TireOcr.RunnerPrototype.Dtos;
using TireOcr.RunnerPrototype.Models;
using TireOcr.Shared.Result;

namespace TireOcr.RunnerPrototype.Services;

public class TireOcrService : ITireOcrService
{
    private readonly PreprocessingClient _preprocessingClient;
    private readonly OcrClient _ocrClient;
    private readonly PostprocessingClient _postprocessingClient;
    private readonly ILogger<TireOcrService> _logger;

    public TireOcrService(PreprocessingClient preprocessingClient, OcrClient ocrClient, ILogger<TireOcrService> logger,
        PostprocessingClient postprocessingClient)
    {
        _preprocessingClient = preprocessingClient;
        _ocrClient = ocrClient;
        _logger = logger;
        _postprocessingClient = postprocessingClient;
    }

    public async Task<DataResult<TireOcrResult>> RunSingleOcrPipelineAsync(
        Image image,
        TireCodeDetectorType detectorType)
    {
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

        var tireCode = ocrResult.Item2.Data!;

        var postprocessingResult = await PerformTimeMeasuredTask("Postprocessing",
            () => _postprocessingClient.PostprocessTireCode(tireCode));
        if (postprocessingResult.Item2.IsFailure)
            return DataResult<TireOcrResult>.Failure(ocrResult.Item2.Failures);
        
        var postprocessedTireCode = postprocessingResult.Item2.Data!;

        List<RunStat> runTrace =
        [
            preprocessingResult.Item1,
            ocrResult.Item1,
            postprocessingResult.Item1
        ];

        var finalResult = new TireOcrResult(postprocessedTireCode, detectorType, runTrace);
        return DataResult<TireOcrResult>.Success(finalResult);
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