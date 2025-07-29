using System.Net.Http.Headers;
using System.Net.Http.Json;
using AiPipeline.Orchestration.Shared.All.Contracts.Commands.RunPipelineStep;
using AiPipeline.Orchestration.Shared.Nodes.Dtos.GetFileUploadResult;
using AiPipeline.Orchestration.Shared.Nodes.Exceptions;
using JasperFx.Core;
using Microsoft.Extensions.Logging;
using TireOcr.Shared.Result;

namespace AiPipeline.Orchestration.Shared.Nodes.Services.FileReferenceUploader;

public class HttpFileReferenceUploaderService : IFileReferenceUploaderService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<HttpFileReferenceUploaderService> _logger;

    public HttpFileReferenceUploaderService(HttpClient httpClient, ILogger<HttpFileReferenceUploaderService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<DataResult<FileReference>> UploadFileDataAsync(Guid fileId, Stream fileStream, string contentType,
        string fileName, bool storePermanently)
    {
        try
        {
            using var content = new MultipartFormDataContent();
            var fileData = await fileStream.ReadAllBytesAsync();
            content.Add(new ByteArrayContent(fileData)
            {
                Headers =
                {
                    ContentType = MediaTypeHeaderValue.Parse(contentType),
                    ContentDisposition = new ContentDispositionHeaderValue("form-data")
                    {
                        Name = "File",
                        FileName = fileName
                    }
                }
            });
            content.Add(new StringContent(fileId.ToString()), "Id");

            var res = await _httpClient.PostAsync("/api/v1/Files/Upload", content);
            if (!res.IsSuccessStatusCode)
            {
                var errorContent = await res.Content.ReadAsStringAsync();
                throw new HttpRequestExceptionWithContent(res.StatusCode, content: errorContent);
            }

            var postprocessResult = (await res.Content.ReadFromJsonAsync<GetFileUploadResultDto>())?.File;
            if (postprocessResult is null || postprocessResult.Id == Guid.Empty || postprocessResult.Path is null)
                throw new Exception($"Encountered unexpected response format while uploading file {fileId}");

            var fileReference = new FileReference(
                Id: postprocessResult.Id,
                StorageProvider: postprocessResult.StorageProvider,
                ContentType: postprocessResult.ContentType,
                Path: postprocessResult.Path
            );
            return DataResult<FileReference>.Success(fileReference);
        }
        catch (HttpRequestExceptionWithContent ex)
        {
            var statusCode = ex.StatusCode;
            ex.TryGetContentJsonProperty("detail", out var content);
            var failureMessage =
                $"{nameof(HttpFileReferenceUploaderService)}: {content ?? $"Error while uploading file '{fileId}'"}";

            _logger.LogError(ex, failureMessage);
            return DataResult<FileReference>.Failure(new Failure((int)statusCode!, failureMessage));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Unexpected error while downloading file reference '{fileId}'");
            return DataResult<FileReference>.Failure(
                new Failure(500,
                    $"{nameof(HttpFileReferenceUploaderService)}: Unexpected error while uploading file '{fileId}'")
            );
        }
    }
}