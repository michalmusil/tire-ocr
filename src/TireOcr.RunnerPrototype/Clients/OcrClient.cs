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

    public async Task<DataResult<OcrServiceResultDto>> PerformTireCodeOcrOnImage(Image image,
        TireCodeDetectorType detectorType)
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

            return DataResult<OcrServiceResultDto>.Success(ocrResult!);
        }
        catch (HttpRequestException ex)
        {
            return ex.StatusCode switch
            {
                HttpStatusCode.NotFound => DataResult<OcrServiceResultDto>.NotFound(
                    "No tire code was detected during Ocr"
                ),
                _ => DataResult<OcrServiceResultDto>.Failure(DefaultFailure)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while calling Ocr service.");
            return DataResult<OcrServiceResultDto>.Failure(DefaultFailure);
        }
    }
}