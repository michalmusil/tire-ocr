using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunBatchAggregate;
using TireOcr.Shared.Result;

namespace AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRunBatch.Services;

public interface IBatchCsvExportService
{
    public Task<DataResult<byte[]>> ExportBatch(EvaluationRunBatchEntity evaluationRunBatch);
}