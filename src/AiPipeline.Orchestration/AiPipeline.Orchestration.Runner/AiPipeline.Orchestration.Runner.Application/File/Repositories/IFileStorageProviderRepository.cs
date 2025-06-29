namespace AiPipeline.Orchestration.Runner.Application.File.Repositories;

public interface IFileStorageProviderRepository
{
    public Task<bool> UploadFileAsync(Stream fileStream, string path, string contentType);
    public Task<Stream?> DownloadFileAsync(string path);
    public Task<bool> RemoveFileAsync(string path);
    public string GetProviderName();
}