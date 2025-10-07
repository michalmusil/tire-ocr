namespace AiPipeline.TireOcr.EvaluationTool.WebApi.Contracts.Batch.GetPaginated;

public record GetBatchesPaginatedRequest(
    int PageNumber,
    int PageSize
);