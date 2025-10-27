namespace AiPipeline.TireOcr.EvaluationTool.WebApi.EvaluationRun.Contracts.Run.GetPaginated;

public record GetRunsPaginatedRequest(
    int PageNumber,
    int PageSize
);