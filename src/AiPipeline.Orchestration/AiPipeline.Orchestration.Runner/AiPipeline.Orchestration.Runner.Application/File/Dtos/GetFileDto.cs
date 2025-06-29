using AiPipeline.Orchestration.Runner.Domain.FileAggregate;

namespace AiPipeline.Orchestration.Runner.Application.File.Dtos;

public record GetFileDto(
    Guid Id,
    FileStorageScope FileStorageScope,
    string ContentType
)
{
    public static GetFileDto FromDomain(Domain.FileAggregate.File domain)
    {
        return new GetFileDto(
            Id: domain.Id,
            FileStorageScope: domain.FileStorageScope,
            ContentType: domain.ContentType
        );
    }
}