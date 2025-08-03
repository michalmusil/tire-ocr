using AiPipeline.Orchestration.FileService.Domain.FileAggregate;

namespace AiPipeline.Orchestration.FileService.Application.File.Repositories;

public interface IFileStorageProviderRepository
{
    public Task<bool> UploadFileAsync(Stream fileStream, string contentType, FileStorageScope scope, string fileName,
        string? prefix = null);

    public Task<Stream?> DownloadFileAsync(FileStorageScope scope, string fileName, string? prefix = null);
    public Task<bool> RemoveFileAsync(FileStorageScope scope, string fileName, string? prefix = null);
    public string GetProviderName();
    public string GetFullFilePathFor(FileStorageScope scope, string fileName, string? prefix = null);
}