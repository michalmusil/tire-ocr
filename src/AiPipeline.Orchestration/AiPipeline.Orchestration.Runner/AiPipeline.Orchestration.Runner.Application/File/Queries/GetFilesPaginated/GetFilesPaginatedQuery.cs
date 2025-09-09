using AiPipeline.Orchestration.Runner.Application.File.Dtos;
using AiPipeline.Orchestration.Runner.Domain.FileAggregate;
using TireOcr.Shared.Pagination;
using TireOcr.Shared.UseCase;

namespace AiPipeline.Orchestration.Runner.Application.File.Queries.GetFilesPaginated;

public record GetFilesPaginatedQuery(PaginationParams Pagination, Guid UserId, FileStorageScope? ScopeFilter = null)
    : IQuery<PaginatedCollection<FileDto>>;