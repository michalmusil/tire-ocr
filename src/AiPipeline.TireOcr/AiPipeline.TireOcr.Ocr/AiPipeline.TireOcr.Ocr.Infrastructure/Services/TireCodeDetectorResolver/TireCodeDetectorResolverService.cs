using AiPipeline.TireOcr.Shared.Models;
using Microsoft.Extensions.Configuration;
using TireOcr.Ocr.Application.Services;
using TireOcr.Ocr.Infrastructure.Services.ImageUtils;
using TireOcr.Ocr.Infrastructure.Services.TireCodeDetector;
using TireOcr.Shared.Result;

namespace TireOcr.Ocr.Infrastructure.Services.TireCodeDetectorResolver;

public class TireCodeDetectorResolverService : ITireCodeDetectorResolverService
{
    private readonly IImageConvertorService _imageConvertorService;
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public TireCodeDetectorResolverService(IImageConvertorService imageConvertorService, HttpClient httpClient,
        IConfiguration configuration)
    {
        _imageConvertorService = imageConvertorService;
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public DataResult<ITireCodeDetectorService> Resolve(TireCodeDetectorType detectorType)
    {
        ITireCodeDetectorService? detector = detectorType switch
        {
            TireCodeDetectorType.GoogleGemini => new GoogleGeminiTireCodeDetectorService(
                _httpClient, _imageConvertorService, _configuration),
            TireCodeDetectorType.MistralPixtral => new MistralPixtralTireCodeDetectorService(_httpClient,
                _imageConvertorService,
                _configuration),
            TireCodeDetectorType.OpenAiGpt => new OpenAiGptTireCodeDetectorService(_configuration),
            TireCodeDetectorType.GoogleCloudVision => new GoogleCloudVisionTireCodeDetectorService(_configuration),
            TireCodeDetectorType.AzureAiVision => new AzureAiVisionTireCodeDetectorService(_configuration),
            TireCodeDetectorType.Ollama => new OllamaTireCodeDetectorService(_httpClient, _imageConvertorService,
                _configuration),
            TireCodeDetectorType.LmStudio => new LmStudioTireCodeDetectorService(_configuration),
            _ => null
        };

        if (detector is null)
            return DataResult<ITireCodeDetectorService>.Failure(
                new Failure(500, $"Detector type {detectorType} not supported"));

        return DataResult<ITireCodeDetectorService>.Success(detector);
    }
}