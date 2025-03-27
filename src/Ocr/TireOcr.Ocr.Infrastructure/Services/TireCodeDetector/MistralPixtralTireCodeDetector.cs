using System.Text;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using TireOcr.Ocr.Application.Dtos;
using TireOcr.Ocr.Application.Services;
using TireOcr.Ocr.Domain.ImageEntity;
using TireOcr.Ocr.Infrastructure.Dtos.MistralResponse;
using TireOcr.Ocr.Infrastructure.Services.ImageUtils;
using TireOcr.Shared.Result;

namespace TireOcr.Ocr.Infrastructure.Services.TireCodeDetector;

public class MistralPixtralTireCodeDetector : ITireCodeDetector
{
    private readonly HttpClient _httpClient;
    private readonly IImageUtils _imageUtils;
    private readonly IConfiguration _configuration;

    private readonly string _prompt =
        "In the following image, there should be a picture of a portion of a car tire. On this picture, there should be embossed tire code. The format is for example: \"185/75R1482S\" or \"P215/55ZR1895V\". Keep in mind that the \"/\" character has to be in the output. Please read the tire code from the image and return only the detected code string itself (for example just \"210/60ZR15\"). If you can't detect any tire code in the photo for whatever reason, just answer with letter \"N\" and nothing else.";


    public MistralPixtralTireCodeDetector(HttpClient httpClient, IImageUtils imageUtils, IConfiguration configuration)
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
            
            var foundTireCode = responseDto.Choices
                .Select(c => c.Message)
                .FirstOrDefault(p => !string.IsNullOrEmpty(p.Content) && p.Content.Contains('/'))
                ?.Content;
            
            if (foundTireCode is null)
                return DataResult<OcrResultDto>.NotFound("No tire code detected");

            var result = new OcrResultDto(foundTireCode);
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
        var base64Image = _imageUtils.ConvertToBase64(image);
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
                            text = _prompt
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