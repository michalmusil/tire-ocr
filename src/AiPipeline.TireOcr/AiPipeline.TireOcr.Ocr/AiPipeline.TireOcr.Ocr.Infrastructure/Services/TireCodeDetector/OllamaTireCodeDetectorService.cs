using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using TireOcr.Ocr.Application.Dtos;
using TireOcr.Ocr.Application.Repositories;
using TireOcr.Ocr.Application.Services;
using TireOcr.Ocr.Domain.ImageEntity;
using TireOcr.Ocr.Infrastructure.Dtos.OllamaResponse;
using TireOcr.Ocr.Infrastructure.Services.ImageUtils;
using TireOcr.Shared.Result;

namespace TireOcr.Ocr.Infrastructure.Services.TireCodeDetector;

public class OllamaTireCodeDetectorService : ITireCodeDetectorService
{
    private readonly HttpClient _httpClient;
    private readonly IImageConvertorService _imageConvertorService;
    private readonly IConfiguration _configuration;
    private readonly IPromptRepository _promptRepository;
    private readonly string _modelName;

    public OllamaTireCodeDetectorService(HttpClient httpClient, IImageConvertorService imageConvertorService,
        IConfiguration configuration, IPromptRepository promptRepository, string modelName)
    {
        _httpClient = httpClient;
        _imageConvertorService = imageConvertorService;
        _configuration = configuration;
        _promptRepository = promptRepository;
        _modelName = modelName;
    }


    public async Task<DataResult<OcrResultDto>> DetectAsync(Image image)
    {
        try
        {
            var endpointUri = GetPromptEndpointUri();
            if (endpointUri is null)
                return DataResult<OcrResultDto>.Failure(new Failure(500,
                    "Failed to retrieve Ollama endpoint configuration"));

            using var prompt = await GetPromptJsonBody(image);
            using var response = await _httpClient.PostAsync(endpointUri, prompt);

            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            var responseDto = JsonSerializer.Deserialize<OllamaResponseDto>(responseContent);

            var foundTireCode = responseDto?.Message.Content;

            if (string.IsNullOrEmpty(foundTireCode) || !foundTireCode.Contains('/'))
                return DataResult<OcrResultDto>.NotFound("No tire code detected");

            string? foundManufacturer = null;
            var indexOfManufacturerSplit = foundTireCode.IndexOf('|');
            var manufacturerFound = indexOfManufacturerSplit > 0;
            if (manufacturerFound)
            {
                foundManufacturer = foundTireCode.Substring(0, indexOfManufacturerSplit);
                foundTireCode = foundTireCode.Substring(indexOfManufacturerSplit + 1);
            }


            var result = new OcrResultDto(
                DetectedTireCode: foundTireCode,
                DetectedManufacturer: foundManufacturer,
                Billing: null
            );
            return DataResult<OcrResultDto>.Success(result);
        }
        catch (Exception e)
        {
            var failure = new Failure(500, $"Failed to perform Ocr via Ollama model '{_modelName}' Tire Code Detector");
            return DataResult<OcrResultDto>.Failure(failure);
        }
    }

    private string? GetPromptEndpointUri()
    {
        try
        {
            return _configuration.GetValue<string>("OcrEndpoints:Ollama");
        }
        catch
        {
            return null;
        }
    }

    private async Task<StringContent> GetPromptJsonBody(Image image)
    {
        var prompt = await _promptRepository.GetMainPromptAsync();
        var base64Image = _imageConvertorService.ConvertToBase64(image);
        var payload = new
        {
            model = _modelName,
            messages = new[]
            {
                new { role = "user", content = prompt, images = new[] { base64Image } },
            },
            stream = false,
            think = false,
        };
        var jsonPayload = JsonSerializer.Serialize(payload);
        return new StringContent(jsonPayload, Encoding.UTF8, "application/json");
    }
}