namespace AiPipeline.TireOcr.EvaluationTool.WebApi.Contracts.Run.GetPaginated;

public record GetRunsPaginatedRequest(
    int PageNumber,
    int PageSize
);