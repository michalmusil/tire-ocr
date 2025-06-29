namespace AiPipeline.Orchestration.Runner.Application.File.Repositories;

public interface IFileStorageProviderRepository
{
    Task<bool> UploadFileAsync(Stream fileStream, string path, string contentType);
    Task<Stream?> DownloadFileAsync(string path);
    Task<bool> RemoveFileAsync(string path);
}