using TireOcr.Shared.UseCase;

namespace AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRunBatch.Queries.GetEvaluationBatchRawCsvExport;

public record GetEvaluationBatchRawCsvExportQuery(Guid Id) : IQuery<byte[]>;