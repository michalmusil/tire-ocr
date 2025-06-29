using AiPipeline.Orchestration.Runner.Domain.FileAggregate;

namespace AiPipeline.Orchestration.Runner.Application.File.Repositories;

public interface IFileStorageProviderRepository
{
    public Task<bool> UploadFileAsync(Stream fileStream, string contentType, FileStorageScope scope, string fileName,
        string? prefix = null);

    public Task<Stream?> DownloadFileAsync(FileStorageScope scope, string fileName, string? prefix = null);
    public Task<bool> RemoveFileAsync(FileStorageScope scope, string fileName, string? prefix = null);
    public string GetProviderName();
}