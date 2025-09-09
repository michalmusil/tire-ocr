using System.Text;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using TireOcr.Ocr.Application.Dtos;
using TireOcr.Ocr.Application.Services;
using TireOcr.Ocr.Domain.ImageEntity;
using TireOcr.Ocr.Infrastructure.Constants;
using TireOcr.Ocr.Infrastructure.Dtos.MistralResponse;
using TireOcr.Ocr.Infrastructure.Services.ImageUtils;
using TireOcr.Shared.Result;

namespace TireOcr.Ocr.Infrastructure.Services.TireCodeDetector;

public class MistralPixtralTireCodeDetectorService : ITireCodeDetectorService
{
    private readonly HttpClient _httpClient;
    private readonly IImageConvertorService _imageConvertorService;
    private readonly IConfiguration _configuration;

    public MistralPixtralTireCodeDetectorService(HttpClient httpClient, IImageConvertorService imageConvertorService, IConfiguration configuration)
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
            var apiKey = GetAuthorizationHeader();
            if (endpointUri is null || apiKey is null)
                return DataResult<OcrResultDto>.Failure(new Failure(500,
                    "Failed to retrieve Mistral endpoint configuration"));

            using var prompt = GetPromptJsonBody(image);
            _httpClient.DefaultRequestHeaders.Add("Authorization", apiKey);
            using var response = await _httpClient.PostAsync(endpointUri, prompt);
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

            var result = new OcrResultDto(
                foundTireCode,
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


    private StringContent GetPromptJsonBody(Image image)
    {
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
                            text = ModelPrompts.TireCodeOcrPrompt
                        },
                        new
                        {
                            type = "image_url",
                            image_url = $"data:image/jpeg;base64,{base64Image}"
                        }
                    }
                }
            }
        };
        var jsonPayload = JsonConvert.SerializeObject(payload);
        return new StringContent(jsonPayload, Encoding.UTF8, "application/json");
    }
}