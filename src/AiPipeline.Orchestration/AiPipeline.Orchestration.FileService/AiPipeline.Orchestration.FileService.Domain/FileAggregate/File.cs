namespace AiPipeline.Orchestration.FileService.Domain.FileAggregate;

public class File
{
    public Guid Id { get; }
    public FileStorageScope FileStorageScope { get; }
    public string StorageProvider { get; }
    public string Path { get; }
    public string ContentType { get; }
    public DateTime CreatedAt { get; protected set; }
    public DateTime UpdatedAt { get; private set; }

    private File()
    {
    }

    public File(Guid id, FileStorageScope fileStorageScope, string storageProvider, string path, string contentType)
    {
        Id = id;
        FileStorageScope = fileStorageScope;
        StorageProvider = storageProvider;
        Path = path;
        ContentType = contentType;
        
        var now = DateTime.UtcNow;
        CreatedAt = now;
        UpdatedAt = now;
    }
    
    private void SetUpdated()
    {
        UpdatedAt = DateTime.UtcNow;
    }
}