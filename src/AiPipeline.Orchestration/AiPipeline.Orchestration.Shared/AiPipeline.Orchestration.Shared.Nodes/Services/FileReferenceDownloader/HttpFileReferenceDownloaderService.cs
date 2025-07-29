using AiPipeline.Orchestration.Shared.All.Contracts.Commands.RunPipelineStep;
using AiPipeline.Orchestration.Shared.Nodes.Exceptions;
using Microsoft.Extensions.Logging;
using TireOcr.Shared.Result;

namespace AiPipeline.Orchestration.Shared.Nodes.Services.FileReferenceDownloader;

public class HttpFileReferenceDownloaderService : IFileReferenceDownloaderService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<HttpFileReferenceDownloaderService> _logger;

    public HttpFileReferenceDownloaderService(HttpClient httpClient, ILogger<HttpFileReferenceDownloaderService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<DataResult<Stream>> DownloadFileReferenceDataAsync(FileReference reference)
    {
        try
        {
            var res = await _httpClient.GetAsync($"/api/v1/Files/{reference.Id}/Download");
            if (!res.IsSuccessStatusCode)
            {
                var errorContent = await res.Content.ReadAsStringAsync();
                throw new HttpRequestExceptionWithContent(res.StatusCode, content: errorContent);
            }

            var imageStream = await res.Content.ReadAsStreamAsync();
            return DataResult<Stream>.Success(imageStream);
        }
        catch (HttpRequestExceptionWithContent ex)
        {
            var statusCode = ex.StatusCode;
            ex.TryGetContentJsonProperty("detail", out var content);
            var failureMessage =
                $"{nameof(HttpFileReferenceDownloaderService)}: {content ?? $"Error while downloading file '{reference.Id}'"}";

            _logger.LogError(ex, failureMessage);
            return DataResult<Stream>.Failure(new Failure((int)statusCode!, failureMessage));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Unexpected error while downloading file reference '{@reference}'");
            return DataResult<Stream>.Failure(new Failure(500,
                $"{nameof(HttpFileReferenceDownloaderService)}: Unexpected error occurred while downloading file '{reference.Id}'"));
        }
    }
}