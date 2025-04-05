using TireOcr.RunnerPrototype.Clients;
using TireOcr.RunnerPrototype.Dtos;
using TireOcr.RunnerPrototype.Models;
using TireOcr.Shared.Result;

namespace TireOcr.RunnerPrototype.Services;

public class TireOcrService : ITireOcrService
{
    private readonly PreprocessingClient _preprocessingClient;
    private readonly OcrClient _ocrClient;
    private readonly ILogger<TireOcrService> _logger;

    public TireOcrService(PreprocessingClient preprocessingClient, OcrClient ocrClient, ILogger<TireOcrService> logger)
    {
        _preprocessingClient = preprocessingClient;
        _ocrClient = ocrClient;
        _logger = logger;
    }

    public async Task<DataResult<TireOcrResult>> RunSingleOcrPipelineAsync(Image image, TireCodeDetectorType detectorType)
    {
        throw new NotImplementedException();
    }
}