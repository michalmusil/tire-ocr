using AiPipeline.TireOcr.Shared.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TireOcr.Ocr.Application.Repositories;
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
    private readonly IPromptRepository _promptRepository;
    private readonly ILogger<OpenAiGptTireCodeDetectorService> _openaiLogger;

    public TireCodeDetectorResolverService(IImageConvertorService imageConvertorService, HttpClient httpClient,
        IConfiguration configuration, IPromptRepository promptRepository,
        ILogger<OpenAiGptTireCodeDetectorService> openaiLogger)
    {
        _imageConvertorService = imageConvertorService;
        _httpClient = httpClient;
        _configuration = configuration;
        _promptRepository = promptRepository;
        _openaiLogger = openaiLogger;
    }

    public DataResult<ITireCodeDetectorService> Resolve(TireCodeDetectorType detectorType)
    {
        ITireCodeDetectorService? detector = detectorType switch
        {
            TireCodeDetectorType.GoogleGemini => new GoogleGeminiTireCodeDetectorService(
                _httpClient, _imageConvertorService, _configuration, _promptRepository),
            TireCodeDetectorType.MistralPixtral => new MistralPixtralTireCodeDetectorService(_httpClient,
                _imageConvertorService, _configuration, _promptRepository),
            TireCodeDetectorType.OpenAiGpt => new OpenAiGptTireCodeDetectorService(_configuration, _promptRepository,
                _openaiLogger),
            TireCodeDetectorType.GoogleCloudVision => new GoogleCloudVisionTireCodeDetectorService(_configuration),
            TireCodeDetectorType.AzureAiVision => new AzureAiVisionTireCodeDetectorService(_configuration),
            TireCodeDetectorType.QwenVl => new RunPodVllmApiTireCodeDetectorService(
                _configuration,
                _promptRepository,
                _configuration.GetValue<string>("RunPodModelNames:QwenVl")!,
                _configuration.GetValue<string>("OcrEndpoints:QwenVl")!
            ),
            TireCodeDetectorType.InternVl => new OpenRouterApiTireCodeDetectorService(
                _configuration,
                _promptRepository,
                _configuration.GetValue<string>("OpenRouterModelNames:InternVl")!
            ),
            TireCodeDetectorType.DeepseekOcr => new DeepseekOcrTireCodeDetectorService(
                _httpClient, _imageConvertorService, _configuration, _promptRepository),
            _ => null
        };

        if (detector is null)
            return DataResult<ITireCodeDetectorService>.Failure(
                new Failure(500, $"Detector type {detectorType} not supported"));

        return DataResult<ITireCodeDetectorService>.Success(detector);
    }
}