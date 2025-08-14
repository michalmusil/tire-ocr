using AiPipeline.Orchestration.Runner.Application.PipelineResult.Dtos;
using TireOcr.Shared.Pagination;
using TireOcr.Shared.UseCase;

namespace AiPipeline.Orchestration.Runner.Application.PipelineResult.Queries.GetPipelineResults;

public record GetPipelineResultsQuery(PaginationParams Pagination, Guid UserId) : IQuery<PaginatedCollection<GetPipelineResultDto>>;