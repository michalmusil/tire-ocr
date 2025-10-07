using AiPipeline.TireOcr.EvaluationTool.Application.Dtos;
using TireOcr.Shared.Pagination;
using TireOcr.Shared.UseCase;

namespace AiPipeline.TireOcr.EvaluationTool.Application.Queries.GetEvaluationRunBatchesPaginated;

public record GetEvaluationRunBatchesPaginatedQuery(PaginationParams Pagination)
    : IQuery<PaginatedCollection<EvaluationRunBatchLightDto>>;