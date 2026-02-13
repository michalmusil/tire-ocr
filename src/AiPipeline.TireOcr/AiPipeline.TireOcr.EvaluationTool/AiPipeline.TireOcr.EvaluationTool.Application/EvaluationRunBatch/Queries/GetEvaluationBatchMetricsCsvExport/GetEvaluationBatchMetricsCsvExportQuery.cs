using TireOcr.Shared.UseCase;

namespace AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRunBatch.Queries.GetEvaluationBatchMetricsCsvExport;

public record GetEvaluationBatchMetricsExportQuery(
    Guid BatchId,
    Guid? OtherBatchId,
    int? ExpectedAnnualInferences,
    decimal? AnnualFixedCost) : IQuery<byte[]>;