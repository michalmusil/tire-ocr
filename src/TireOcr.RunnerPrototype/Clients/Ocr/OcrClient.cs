using System.Net.Http.Headers;
using TireOcr.RunnerPrototype.Dtos;
using TireOcr.RunnerPrototype.Exceptions;
using TireOcr.RunnerPrototype.Models;
using TireOcr.Shared.Result;

namespace TireOcr.RunnerPrototype.Clients.Ocr;

public class OcrClient : IOcrClient
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
            if (!res.IsSuccessStatusCode)
            {
                var errorContent = await res.Content.ReadAsStringAsync();
                throw new HttpRequestExceptionWithContent(res.StatusCode, content: errorContent);
            }
            
            var ocrResult = await res.Content.ReadFromJsonAsync<OcrServiceResultDto>();

            return DataResult<OcrServiceResultDto>.Success(ocrResult!);
        }
        catch (HttpRequestExceptionWithContent ex)
        {
            var statusCode = ex.StatusCode;
            ex.TryGetContentJsonProperty("detail", out var content);
            var failureMessage = $"Ocr: {content ?? "No tire code was detected during Ocr"}";

            _logger.LogError(ex, failureMessage);
            return DataResult<OcrServiceResultDto>.Failure(new Failure((int)statusCode!, failureMessage));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while calling Ocr service.");
            return DataResult<OcrServiceResultDto>.Failure(DefaultFailure);
        }
    }
}