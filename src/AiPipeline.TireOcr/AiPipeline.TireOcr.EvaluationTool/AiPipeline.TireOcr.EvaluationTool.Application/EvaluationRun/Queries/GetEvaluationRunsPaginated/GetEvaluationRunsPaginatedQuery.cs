using AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRun.Dtos.EvaluationRun;
using TireOcr.Shared.Pagination;
using TireOcr.Shared.UseCase;

namespace AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRun.Queries.GetEvaluationRunsPaginated;

public record GetEvaluationRunsPaginatedQuery(PaginationParams Pagination)
    : IQuery<PaginatedCollection<EvaluationRunDto>>;