using System.Net;
using System.Net.Http.Headers;
using TireOcr.RunnerPrototype.Models;
using TireOcr.Shared.Result;

namespace TireOcr.RunnerPrototype.Clients;

public class PreprocessingClient
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

            var res = await _httpClient.PostAsync("/Preprocess", content);
            res.EnsureSuccessStatusCode();
            var imageData = await res.Content.ReadAsByteArrayAsync();
            return DataResult<Image>.Success(new Image(imageData, image.FileName, image.ContentType));
        }
        catch (HttpRequestException ex)
        {
            return ex.StatusCode switch
            {
                HttpStatusCode.NotFound => DataResult<Image>.NotFound("No tire code was detected during Preprocessing"),
                _ => DataResult<Image>.Failure(DefaultFailure)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while calling Preprocess service.");
            return DataResult<Image>.Failure(DefaultFailure);
        }
    }
}