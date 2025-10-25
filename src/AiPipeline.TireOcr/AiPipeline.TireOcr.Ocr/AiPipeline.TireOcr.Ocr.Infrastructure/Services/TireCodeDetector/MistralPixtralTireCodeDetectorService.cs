using System.Text;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using TireOcr.Ocr.Application.Dtos;
using TireOcr.Ocr.Application.Repositories;
using TireOcr.Ocr.Application.Services;
using TireOcr.Ocr.Domain.ImageEntity;
using TireOcr.Ocr.Infrastructure.Dtos.MistralResponse;
using TireOcr.Ocr.Infrastructure.Services.ImageUtils;
using TireOcr.Shared.Result;

namespace TireOcr.Ocr.Infrastructure.Services.TireCodeDetector;

public class MistralPixtralTireCodeDetectorService : ITireCodeDetectorService
{
    private readonly HttpClient _httpClient;
    private readonly IImageConvertorService _imageConvertorService;
    private readonly IConfiguration _configuration;
    private readonly IPromptRepository _promptRepository;

    public MistralPixtralTireCodeDetectorService(HttpClient httpClient, IImageConvertorService imageConvertorService,
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
            var apiKey = GetAuthorizationHeader();
            if (endpointUri is null || apiKey is null)
                return DataResult<OcrResultDto>.Failure(new Failure(500,
                    "Failed to retrieve Mistral endpoint configuration"));

            using var requestBody = await GetPromptJsonBody(image);
            _httpClient.DefaultRequestHeaders.Add("Authorization", apiKey);
            using var response = await _httpClient.PostAsync(endpointUri, requestBody);
            _httpClient.DefaultRequestHeaders.Remove("Authorization");

            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            var responseDto = JsonConvert.DeserializeObject<MistralResponseDto>(responseContent);

            var foundTireCode = responseDto!.Choices
                .Select(c => c.Message)
                .FirstOrDefault(p => !string.IsNullOrEmpty(p.Content) && p.Content.Contains('/'))
                ?.Content;

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
                new OcrRequestBillingDto(
                    responseDto.Usage.PromptTokens,
                    responseDto.Usage.CompletionTokens,
                    BillingUnitType.Token
                )
            );
            return DataResult<OcrResultDto>.Success(result);
        }
        catch (Exception e)
        {
            var failure = new Failure(500, "Failed to perform Ocr via Mistral Pixtral Tire Code Detector");
            return DataResult<OcrResultDto>.Failure(failure);
        }
    }

    private string? GetPromptEndpointUri()
    {
        try
        {
            return _configuration.GetValue<string>("OcrEndpoints:Mistral");
        }
        catch
        {
            return null;
        }
    }

    private string? GetAuthorizationHeader()
    {
        try
        {
            var apiKey = _configuration.GetValue<string>("ApiKeys:Mistral");
            return $"Bearer {apiKey}";
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
            model = "pixtral-large-latest",
            messages = new[]
            {
                new
                {
                    role = "user",
                    content = new object[]
                    {
                        new
                        {
                            type = "text",
                            text = prompt
                        },
                        new
                        {
                            type = "image_url",
                            image_url = $"data:image/jpeg;base64,{base64Image}"
                        }
                    }
                }
            },
            temperature = 0.2
        };
        var jsonPayload = JsonConvert.SerializeObject(payload);
        return new StringContent(jsonPayload, Encoding.UTF8, "application/json");
    }
}