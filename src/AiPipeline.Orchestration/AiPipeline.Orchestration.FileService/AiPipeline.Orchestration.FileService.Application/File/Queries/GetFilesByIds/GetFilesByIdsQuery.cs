using AiPipeline.Orchestration.FileService.Application.File.Dtos;
using TireOcr.Shared.UseCase;

namespace AiPipeline.Orchestration.FileService.Application.File.Queries.GetFilesByIds;

public record GetFilesByIdsQuery(IEnumerable<Guid> FileIds, Guid UserId, bool FailIfNotAllFound)
    : IQuery<IEnumerable<GetFileDto>>;