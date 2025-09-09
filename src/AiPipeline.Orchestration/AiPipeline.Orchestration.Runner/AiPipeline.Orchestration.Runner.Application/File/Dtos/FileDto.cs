using AiPipeline.Orchestration.Runner.Domain.FileAggregate;

namespace AiPipeline.Orchestration.Runner.Application.File.Dtos;

public record FileDto(
    Guid Id,
    Guid UserId,
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
            UserId: domain.UserId,
            FileStorageScope: domain.FileStorageScope,
            ContentType: domain.ContentType,
            StorageProvider: domain.StorageProvider,
            Path: domain.Path
        );
    }
}