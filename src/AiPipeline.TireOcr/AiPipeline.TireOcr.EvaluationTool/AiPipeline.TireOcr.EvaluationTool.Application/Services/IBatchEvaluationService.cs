using AiPipeline.TireOcr.EvaluationTool.Application.Dtos.BatchEvaluation;
using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunBatchAggregate;
using TireOcr.Shared.Result;

namespace AiPipeline.TireOcr.EvaluationTool.Application.Services;

public interface IBatchEvaluationService
{
    public Task<DataResult<BatchEvaluationDto>> EvaluateBatch(EvaluationRunBatchEntity batch);
}