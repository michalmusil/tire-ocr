using AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRunBatch.Dtos.BatchEvaluation;
using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunBatchAggregate;
using TireOcr.Shared.Result;

namespace AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRunBatch.Services;

public interface IBatchEvaluationService
{
    public Task<DataResult<BatchEvaluationDto>> EvaluateBatch(EvaluationRunBatchEntity batch);
}