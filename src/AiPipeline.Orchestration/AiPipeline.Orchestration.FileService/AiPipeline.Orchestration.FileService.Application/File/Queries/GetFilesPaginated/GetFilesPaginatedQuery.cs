using AiPipeline.Orchestration.FileService.Application.File.Dtos;
using AiPipeline.Orchestration.FileService.Domain.FileAggregate;
using TireOcr.Shared.Pagination;
using TireOcr.Shared.UseCase;

namespace AiPipeline.Orchestration.FileService.Application.File.Queries.GetFilesPaginated;

public record GetFilesPaginatedQuery(PaginationParams Pagination, FileStorageScope? ScopeFilter = null)
    : IQuery<PaginatedCollection<GetFileDto>>;