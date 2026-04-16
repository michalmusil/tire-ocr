using TireOcr.Shared.UseCase;

namespace AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRunBatch.Queries.GetEvaluationBatchMetricsCsvExport;

public record GetEvaluationBatchMetricsExportQuery(
    Guid BatchId,
    IEnumerable<Guid>? OtherBatchIds,
    decimal? FixedExpenditure,
    bool AddVariableExpenditure,
    bool AverageMetricsWithOtherBatch) : IQuery<byte[]>;