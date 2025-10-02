using System.Net.Http.Headers;
using TireOcr.RunnerPrototype.Dtos.Preprocessing;
using TireOcr.RunnerPrototype.Models;
using TireOcr.Shared.Exceptions;
using TireOcr.Shared.Result;

namespace TireOcr.RunnerPrototype.Clients.Preprocessing;

public class PreprocessingClient : IPreprocessingClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<PreprocessingClient> _logger;

    private Failure DefaultFailure => new Failure(500, "Failed to preprocess image.");

    public PreprocessingClient(HttpClient httpClient, ILogger<PreprocessingClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<DataResult<Image>> PreprocessImage(Image image)
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

            var res = await _httpClient.PostAsync("/api/v1/Preprocess", content);
            if (!res.IsSuccessStatusCode)
            {
                var errorContent = await res.Content.ReadAsStringAsync();
                throw new HttpRequestExceptionWithContent(res.StatusCode, content: errorContent);
            }

            var responseContent = await res.Content.ReadFromJsonAsync<PreprocessingResponseDto>();
            var imageData = Convert.FromBase64String(responseContent!.Base64ImageData);
            return DataResult<Image>.Success(
                new Image(imageData, responseContent.FileName, responseContent.ContentType)
            );
        }
        catch (HttpRequestExceptionWithContent ex)
        {
            var statusCode = ex.StatusCode;
            ex.TryGetContentJsonProperty("detail", out var content);
            var failureMessage = $"Preprocessing: {content ?? "Failed to detect a tire code"}";

            _logger.LogError(ex, failureMessage);
            return DataResult<Image>.Failure(new Failure((int)statusCode!, failureMessage));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while calling Preprocess service.");
            return DataResult<Image>.Failure(DefaultFailure);
        }
    }
}