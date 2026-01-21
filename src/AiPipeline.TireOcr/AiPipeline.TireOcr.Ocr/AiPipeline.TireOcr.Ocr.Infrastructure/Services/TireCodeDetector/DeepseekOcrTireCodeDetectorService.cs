using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using TireOcr.Ocr.Application.Dtos;
using TireOcr.Ocr.Application.Repositories;
using TireOcr.Ocr.Application.Services;
using TireOcr.Ocr.Domain.ImageEntity;
using TireOcr.Ocr.Infrastructure.Dtos.DeepseekOcrResponse;
using TireOcr.Ocr.Infrastructure.Services.ImageUtils;
using TireOcr.Shared.Result;

namespace TireOcr.Ocr.Infrastructure.Services.TireCodeDetector;

public class DeepseekOcrTireCodeDetectorService : ITireCodeDetectorService
{
    private readonly HttpClient _httpClient;
    private readonly IImageConvertorService _imageConvertorService;
    private readonly IConfiguration _configuration;
    private readonly IPromptRepository _promptRepository;

    public DeepseekOcrTireCodeDetectorService(HttpClient httpClient, IImageConvertorService imageConvertorService,
        IConfiguration configuration, IPromptRepository promptRepository)
    {
        _httpClient = httpClient;
        _imageConvertorService = imageConvertorService;
        _configuration = configuration;
        _promptRepository = promptRepository;
    }

    public async Task<DataResult<OcrResultDto>> DetectAsync(Image image)
    {
        try
        {
            var endpointUri = GetPromptEndpointUri();
            if (endpointUri is null)
                return DataResult<OcrResultDto>.Failure(new Failure(500,
                    "Failed to retrieve Deepseek OCR endpoint configuration"));

            var apiKey = GetApiKey();
            if (apiKey is null)
                return DataResult<OcrResultDto>.Failure(new Failure(500,
                    "Failed to retrieve Deepseek OCR api key"));

            using var prompt = await GetPromptJsonBody(image);
            using var request = new HttpRequestMessage(HttpMethod.Post, endpointUri);
            request.Content = prompt;
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
            using var response = await _httpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            var responseDto = JsonConvert.DeserializeObject<DeepseekOcrResponseDto>(responseContent);

            var foundTireCode = responseDto.Output.Choices
                .Select(c => c.Message.Content)
                .FirstOrDefault(c => !string.IsNullOrEmpty(c) && c.Contains('/'));

            if (foundTireCode is null)
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
            var failure = new Failure(500, "Failed to perform Ocr via Deepseek OCR Tire Code Detector");
            return DataResult<OcrResultDto>.Failure(failure);
        }
    }

    private string? GetPromptEndpointUri()
    {
        try
        {
            return _configuration.GetValue<string>("OcrEndpoints:DeepseekOcr");
        }
        catch
        {
            return null;
        }
    }

    private string? GetApiKey()
    {
        try
        {
            return _configuration.GetValue<string>("ApiKeys:RunPod");
        }
        catch
        {
            return null;
        }
    }

    private async Task<StringContent> GetPromptJsonBody(Image image)
    {
        var prompt = await _promptRepository.GetSpecializedDeepseekOcrPromptAsync();
        var base64Image = _imageConvertorService.ConvertToBase64(image);
        var payload = new
        {
            input = new
            {
                messages = new[]
                {
                    new
                    {
                        prompt = prompt,
                        multi_modal_data = new
                        {
                            image = base64Image
                        }
                    }
                }
            }
        };
        var jsonPayload = JsonConvert.SerializeObject(payload);
        return new StringContent(jsonPayload, Encoding.UTF8, "application/json");
    }
}