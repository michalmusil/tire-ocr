using System.Text;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using TireOcr.Ocr.Application.Dtos;
using TireOcr.Ocr.Application.Services;
using TireOcr.Ocr.Domain.ImageEntity;
using TireOcr.Ocr.Infrastructure.Constants;
using TireOcr.Ocr.Infrastructure.Dtos.OllamaResponse;
using TireOcr.Ocr.Infrastructure.Services.ImageUtils;
using TireOcr.Shared.Result;

namespace TireOcr.Ocr.Infrastructure.Services.TireCodeDetector;

public class OllamaTireCodeDetectorService : ITireCodeDetectorService
{
    private const string Model = "llama3.2-vision:11b";
    private readonly HttpClient _httpClient;
    private readonly IImageConvertorService _imageConvertorService;
    private readonly IConfiguration _configuration;

    public OllamaTireCodeDetectorService(HttpClient httpClient, IImageConvertorService imageConvertorService,
        IConfiguration configuration)
    {
        _httpClient = httpClient;
        _imageConvertorService = imageConvertorService;
        _configuration = configuration;
    }

    public async Task<DataResult<OcrResultDto>> DetectAsync(Image image)
    {
        try
        {
            var endpointUri = GetPromptEndpointUri();
            if (endpointUri is null)
                return DataResult<OcrResultDto>.Failure(new Failure(500,
                    "Failed to retrieve Ollama endpoint configuration"));

            using var prompt = GetPromptJsonBody(image);
            using var response = await _httpClient.PostAsync(endpointUri, prompt);

            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            var responseDto = JsonConvert.DeserializeObject<OllamaResponseDto>(responseContent);

            var foundTireCode = responseDto?.Response;

            if (foundTireCode is null)
                return DataResult<OcrResultDto>.NotFound("No tire code detected");

            var result = new OcrResultDto(
                foundTireCode,
                new OcrRequestBillingDto(0, 1, BillingUnitType.Transaction)
            );
            return DataResult<OcrResultDto>.Success(result);
        }
        catch (Exception e)
        {
            var failure = new Failure(500, "Failed to perform Ocr via Ollama Tire Code Detector");
            return DataResult<OcrResultDto>.Failure(failure);
        }
    }

    private string? GetPromptEndpointUri()
    {
        try
        {
            var endpoint = _configuration.GetValue<string>("OcrEndpoints:Ollama");
            return $"{endpoint}";
        }
        catch
        {
            return null;
        }
    }

    private StringContent GetPromptJsonBody(Image image)
    {
        var base64Image = _imageConvertorService.ConvertToBase64(image);
        var payload = new
        {
            model = Model,
            prompt = ModelPrompts.TireCodeOcrPrompt,
            stream = false,
            images = new[] { base64Image }
        };
        var jsonPayload = JsonConvert.SerializeObject(payload);
        return new StringContent(jsonPayload, Encoding.UTF8, "application/json");
    }
}