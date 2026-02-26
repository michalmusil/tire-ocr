using System.Text;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using TireOcr.Ocr.Application.Dtos;
using TireOcr.Ocr.Application.Repositories;
using TireOcr.Ocr.Application.Services;
using TireOcr.Ocr.Domain.ImageEntity;
using TireOcr.Ocr.Infrastructure.Dtos.GoogleGeminiResponse;
using TireOcr.Ocr.Infrastructure.Services.ImageUtils;
using TireOcr.Shared.Result;

namespace TireOcr.Ocr.Infrastructure.Services.TireCodeDetector;

public class GoogleGeminiTireCodeDetectorService : ITireCodeDetectorService
{
    private const int Seed = 42;
    private readonly HttpClient _httpClient;
    private readonly IImageConvertorService _imageConvertorService;
    private readonly IConfiguration _configuration;
    private readonly IPromptRepository _promptRepository;

    public GoogleGeminiTireCodeDetectorService(HttpClient httpClient, IImageConvertorService imageConvertorService,
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
                    "Failed to retrieve Google Gemini endpoint configuration"));

            using var prompt = await GetPromptJsonBody(image);
            using var response = await _httpClient.PostAsync(endpointUri, prompt);

            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            var responseDto = JsonConvert.DeserializeObject<GoogleGeminiResponseDto>(responseContent);

            var foundTireCode = responseDto?.ContentCandidates
                .SelectMany(c => c.Content.Parts)
                .FirstOrDefault(p => !string.IsNullOrEmpty(p.Text) && p.Text.Contains('/'))
                ?.Text;

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
                    responseDto!.UsageMetadata.PromptTokenCount,
                    responseDto.UsageMetadata.CandidatesTokenCount,
                    BillingUnitType.Token
                )
            );
            return DataResult<OcrResultDto>.Success(result);
        }
        catch (Exception e)
        {
            var failure = new Failure(500, "Failed to perform Ocr via Google Gemini Tire Code Detector");
            return DataResult<OcrResultDto>.Failure(failure);
        }
    }

    private string? GetPromptEndpointUri()
    {
        try
        {
            var endpoint = _configuration.GetValue<string>("OcrEndpoints:Gemini");
            var key = _configuration.GetValue<string>("ApiKeys:Gemini");

            return $"{endpoint}?key={key}";
        }
        catch
        {
            return null;
        }
    }

    private async Task<StringContent> GetPromptJsonBody(Image image)
    {
        var prompt = await _promptRepository.GetMainPromptAsync(useRandomPrefix: true);
        var base64Image = _imageConvertorService.ConvertToBase64(image);
        var payload = new
        {
            systemInstruction = new
            {
                role = "system",
                parts = new object[]
                {
                    new { text = prompt },
                }
            },
            contents = new[]
            {
                new
                {
                    role = "user",
                    parts = new object[]
                    {
                        new
                        {
                            inline_data = new
                            {
                                mime_type = image.ContentType,
                                data = base64Image
                            }
                        }
                    }
                }
            },
            generationConfig = new
            {
                temperature = 0.0,
                seed = Seed,
                mediaResolution = "MEDIA_RESOLUTION_HIGH"
            }
        };
        var jsonPayload = JsonConvert.SerializeObject(payload);
        return new StringContent(jsonPayload, Encoding.UTF8, "application/json");
    }
}