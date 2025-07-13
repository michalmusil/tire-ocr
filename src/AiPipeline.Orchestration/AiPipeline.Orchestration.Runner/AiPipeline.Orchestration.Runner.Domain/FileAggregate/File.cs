using AiPipeline.Orchestration.Runner.Domain.Common;

namespace AiPipeline.Orchestration.Runner.Domain.FileAggregate;

public class File: TimestampedEntity
{
    public Guid Id { get; }
    public FileStorageScope FileStorageScope { get; }
    public string StorageProvider { get; }
    public string Path { get; }
    public string ContentType { get; }

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
    }
}