using TireOcr.Shared.Domain;

namespace AiPipeline.Orchestration.Runner.Domain.FileAggregate;

public class FileValueObject : ValueObject
{
    public required Guid Id { get; init; }
    public required FileStorageScope FileStorageScope { get; init; }
    public required string StorageProvider { get; init; }
    public required string Path { get; init; }
    public required string ContentType { get; init; }

    protected override IEnumerable<object?> GetEqualityComponents() =>
        [Id, FileStorageScope, StorageProvider, Path, ContentType, ContentType];
}