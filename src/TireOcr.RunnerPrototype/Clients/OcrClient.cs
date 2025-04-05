using System.Net;
using System.Net.Http.Headers;
using TireOcr.RunnerPrototype.Dtos;
using TireOcr.RunnerPrototype.Models;
using TireOcr.Shared.Result;

namespace TireOcr.RunnerPrototype.Clients;

public class OcrClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<OcrClient> _logger;

    private Failure DefaultFailure => new Failure(500, "Failed to perform Ocr on preprocessed image.");

    public OcrClient(HttpClient httpClient, ILogger<OcrClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<DataResult<string>> PerformTireCodeOcrOnImage(Image image, TireCodeDetectorType detectorType)
    {
        try
        {
            using var content = new MultipartFormDataContent();
            content.Add(new ByteArrayContent(image.Data)
            {
                Headers =
                {
                    ContentType = MediaTypeHeaderValue.Parse(image.ContentType),
                    ContentDisposition = new ContentDispositionHeaderValue("form-data")
                    {
                        Name = "Image",
                        FileName = image.FileName
                    }
                }
            });
            content.Add(new StringContent(detectorType.ToString()), "DetectorType");

            var res = await _httpClient.PostAsync("/api/v1/Ocr", content);
            res.EnsureSuccessStatusCode();
            var ocrResult = await res.Content.ReadFromJsonAsync<OcrServiceResultDto>();

            return DataResult<string>.Success(ocrResult!.DetectedCode);
        }
        catch (HttpRequestException ex)
        {
            return ex.StatusCode switch
            {
                HttpStatusCode.NotFound => DataResult<string>.NotFound("No tire code was detected during Ocr"),
                _ => DataResult<string>.Failure(DefaultFailure)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while calling Ocr service.");
            return DataResult<string>.Failure(DefaultFailure);
        }
    }
}