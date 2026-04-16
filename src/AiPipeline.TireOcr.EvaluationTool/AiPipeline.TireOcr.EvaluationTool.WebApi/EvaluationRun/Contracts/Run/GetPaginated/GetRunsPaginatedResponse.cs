using AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRun.Dtos.EvaluationRun;
using TireOcr.Shared.Pagination;

namespace AiPipeline.TireOcr.EvaluationTool.WebApi.EvaluationRun.Contracts.Run.GetPaginated;

public record GetRunsPaginatedResponse(IEnumerable<EvaluationRunDto> Items, Pagination Pagination);