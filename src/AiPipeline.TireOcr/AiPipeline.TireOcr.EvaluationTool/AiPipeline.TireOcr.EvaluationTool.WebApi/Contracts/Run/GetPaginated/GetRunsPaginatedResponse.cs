using AiPipeline.TireOcr.EvaluationTool.Application.Dtos.EvaluationRun;
using TireOcr.Shared.Pagination;

namespace AiPipeline.TireOcr.EvaluationTool.WebApi.Contracts.Run.GetPaginated;

public record GetRunsPaginatedResponse(IEnumerable<EvaluationRunDto> Items, Pagination Pagination);