using Microsoft.Extensions.Configuration;
using TireOcr.Ocr.Application.Services;
using TireOcr.Ocr.Domain;
using TireOcr.Ocr.Infrastructure.Services.ImageUtils;
using TireOcr.Ocr.Infrastructure.Services.TireCodeDetector;
using TireOcr.Shared.Result;

namespace TireOcr.Ocr.Infrastructure.Services.TireCodeDetectorResolver;

public class TireCodeDetectorResolver : ITireCodeDetectorResolver
{
    private readonly IImageUtils _imageUtils;
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public TireCodeDetectorResolver(IImageUtils imageUtils, HttpClient httpClient, IConfiguration configuration)
    {
        _imageUtils = imageUtils;
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public DataResult<ITireCodeDetector> Resolve(TireCodeDetectorType detectorType)
    {
        var detector = detectorType switch
        {
            TireCodeDetectorType.GoogleGemini => new GoogleGeminiTireCodeDetector(
                _httpClient, _imageUtils, _configuration),
            _ => null
        };

        if (detector == null)
            return DataResult<ITireCodeDetector>.Failure(
                new Failure(500, $"Detector type {detectorType} not supported"));

        return DataResult<ITireCodeDetector>.Success(detector);
    }
}