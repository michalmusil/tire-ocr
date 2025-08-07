using AiPipeline.Orchestration.Runner.Domain.FileAggregate;

namespace AiPipeline.Orchestration.Runner.Application.File.Dtos;

public record FileDto(
    Guid Id,
    FileStorageScope FileStorageScope,
    string ContentType,
    string StorageProvider,
    string Path
)
{
    public static FileDto FromDomain(FileValueObject domain)
    {
        return new FileDto(
            Id: domain.Id,
            FileStorageScope: domain.FileStorageScope,
            ContentType: domain.ContentType,
            StorageProvider: domain.StorageProvider,
            Path: domain.Path
        );
    }
}