using AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRunBatch.Dtos.BatchEvaluation;
using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunBatchAggregate;
using TireOcr.Shared.Result;

namespace AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRunBatch.Services;

public interface IBatchCsvExportService
{
    public Task<DataResult<byte[]>> ExportRawBatch(EvaluationRunBatchEntity evaluationRunBatch);
    public Task<DataResult<byte[]>> ExportBatchMetrics(BatchEvaluationDto batchEvaluationDto);
}