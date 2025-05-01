using System.Text;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using TireOcr.Ocr.Application.Dtos;
using TireOcr.Ocr.Application.Services;
using TireOcr.Ocr.Domain.ImageEntity;
using TireOcr.Ocr.Infrastructure.Dtos.GoogleGeminiResponse;
using TireOcr.Ocr.Infrastructure.Services.ImageUtils;
using TireOcr.Shared.Result;

namespace TireOcr.Ocr.Infrastructure.Services.TireCodeDetector;

public class GoogleGeminiTireCodeDetector : ITireCodeDetector
{
    private readonly HttpClient _httpClient;
    private readonly IImageUtils _imageUtils;
    private readonly IConfiguration _configuration;

    private readonly string _prompt =
        "In the following image, there should be a picture of a portion of a car tire. On this picture, there should be embossed tire code. The format is for example: \"185/75R1482S\" or \"P215/55ZR1895V\". Keep in mind that the \"/\" character has to be in the output. Please read the tire code from the image and return only the detected code string itself (for example just \"210/60ZR15\"). If you can't detect any tire code in the photo for whatever reason, just answer with letter \"N\" and nothing else.";


    public GoogleGeminiTireCodeDetector(HttpClient httpClient, IImageUtils imageUtils, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _imageUtils = imageUtils;
        _configuration = configuration;
    }

    public async Task<DataResult<OcrResultDto>> DetectAsync(Image image)
    {
        try
        {
            var endpointUri = GetPromptEndpointUri();
            if (endpointUri is null)
                return DataResult<OcrResultDto>.Failure(new Failure(500,
                    "Failed to retrieve Google Gemini endpoint configuration"));

            using var prompt = GetPromptJsonBody(image);
            using var response = await _httpClient.PostAsync(endpointUri, prompt);

            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            var responseDto = JsonConvert.DeserializeObject<GoogleGeminiResponseDto>(responseContent);

            var foundTireCode = responseDto.ContentCandidates
                .SelectMany(c => c.Content.Parts)
                .FirstOrDefault(p => !string.IsNullOrEmpty(p.Text) && p.Text.Contains('/'))
                ?.Text;

            if (foundTireCode is null)
                return DataResult<OcrResultDto>.NotFound("No tire code detected");

            var result = new OcrResultDto(
                foundTireCode,
                new OcrRequestBillingDto(responseDto.UsageMetadata.TotalTokenCount, BillingUnit.Token)
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

    private StringContent GetPromptJsonBody(Image image)
    {
        var base64Image = _imageUtils.ConvertToBase64(image);
        var payload = new
        {
            contents = new[]
            {
                new
                {
                    parts = new object[]
                    {
                        new { text = _prompt },
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
            }
        };
        var jsonPayload = JsonConvert.SerializeObject(payload);
        return new StringContent(jsonPayload, Encoding.UTF8, "application/json");
    }
}