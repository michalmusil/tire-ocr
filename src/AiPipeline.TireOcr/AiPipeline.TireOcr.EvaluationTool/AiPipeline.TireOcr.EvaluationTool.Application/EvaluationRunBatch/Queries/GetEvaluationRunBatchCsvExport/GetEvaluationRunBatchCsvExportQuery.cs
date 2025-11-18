using TireOcr.Shared.UseCase;

namespace AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRunBatch.Queries.GetEvaluationRunBatchCsvExport;

public record GetEvaluationRunBatchCsvExportQuery(Guid Id) : IQuery<byte[]>;