using AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRunBatch.Dtos;
using TireOcr.Shared.Pagination;
using TireOcr.Shared.UseCase;

namespace AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRunBatch.Queries.GetEvaluationRunBatchesPaginated;

public record GetEvaluationRunBatchesPaginatedQuery(PaginationParams Pagination, string? SearchTerm = null)
    : IQuery<PaginatedCollection<EvaluationRunBatchLightDto>>;