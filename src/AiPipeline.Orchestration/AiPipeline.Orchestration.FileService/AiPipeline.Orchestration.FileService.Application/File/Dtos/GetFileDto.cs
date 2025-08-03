using AiPipeline.Orchestration.FileService.Domain.FileAggregate;

namespace AiPipeline.Orchestration.FileService.Application.File.Dtos;

public record GetFileDto(
    Guid Id,
    FileStorageScope FileStorageScope,
    string ContentType,
    string StorageProvider,
    string Path
)
{
    public static GetFileDto FromDomain(Domain.FileAggregate.File domain)
    {
        return new GetFileDto(
            Id: domain.Id,
            FileStorageScope: domain.FileStorageScope,
            ContentType: domain.ContentType,
            StorageProvider: domain.StorageProvider,
            Path: domain.Path
        );
    }
}