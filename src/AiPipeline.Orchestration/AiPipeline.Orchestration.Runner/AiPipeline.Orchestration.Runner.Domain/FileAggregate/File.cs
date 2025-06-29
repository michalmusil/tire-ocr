namespace AiPipeline.Orchestration.Runner.Domain.FileAggregate;

public class File
{
    public Guid Id { get; }
    public FileStorageScope FileStorageScope { get; }
    public string StorageProvider { get; }
    public string Path { get; }
    public string ContentType { get; }

    public File(FileStorageScope fileStorageScope, string storageProvider, string path, string contentType,
        Guid? id = null)
    {
        Id = id ?? Guid.NewGuid();
        FileStorageScope = fileStorageScope;
        StorageProvider = storageProvider;
        Path = path;
        ContentType = contentType;
    }
}