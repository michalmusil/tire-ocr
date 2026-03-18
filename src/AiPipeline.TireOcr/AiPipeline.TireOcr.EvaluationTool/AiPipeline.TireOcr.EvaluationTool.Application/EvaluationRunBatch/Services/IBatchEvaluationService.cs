using AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRunBatch.Dtos.BatchEvaluation;
using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunBatchAggregate;
using TireOcr.Shared.Result;

namespace AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRunBatch.Services;

public interface IBatchEvaluationService
{
    public Task<DataResult<BatchEvaluationDto>> EvaluateBatch(EvaluationRunBatchEntity batch,
        IncalculableInputsDto? inputs = null);

    /// <summary>
    /// Evaluates the batch AND uses the other relative batch to average its metrics and calculate inference
    /// stability with the related batch. 
    /// </summary>
    /// <param name="batch">The main evaluated batch</param>
    /// <param name="relatedBatch">The related batch for inference stability calculation and metrics averaging</param>
    /// <param name="inputs">Additional inputs for batch evaluation</param>
    /// <returns></returns>
    public Task<DataResult<BatchEvaluationDto>> EvaluateBatchWithRelatedBatch(EvaluationRunBatchEntity batch,
        EvaluationRunBatchEntity relatedBatch, IncalculableInputsDto? inputs);
}