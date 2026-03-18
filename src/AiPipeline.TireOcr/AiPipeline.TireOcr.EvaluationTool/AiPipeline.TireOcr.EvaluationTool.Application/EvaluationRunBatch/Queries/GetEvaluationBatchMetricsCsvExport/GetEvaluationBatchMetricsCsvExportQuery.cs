using TireOcr.Shared.UseCase;

namespace AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRunBatch.Queries.GetEvaluationBatchMetricsCsvExport;

public record GetEvaluationBatchMetricsExportQuery(
    Guid BatchId,
    Guid? OtherBatchId,
    decimal? FixedExpenditure,
    bool AddVariableExpenditure,
    bool AverageMetricsWithOtherBatch) : IQuery<byte[]>;