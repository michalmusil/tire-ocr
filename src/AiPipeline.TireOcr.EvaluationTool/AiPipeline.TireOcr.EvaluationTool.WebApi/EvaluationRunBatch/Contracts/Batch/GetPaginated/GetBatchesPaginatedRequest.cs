namespace AiPipeline.TireOcr.EvaluationTool.WebApi.EvaluationRunBatch.Contracts.Batch.GetPaginated;

public record GetBatchesPaginatedRequest(
    int PageNumber,
    int PageSize,
    string? SearchTerm = null
);