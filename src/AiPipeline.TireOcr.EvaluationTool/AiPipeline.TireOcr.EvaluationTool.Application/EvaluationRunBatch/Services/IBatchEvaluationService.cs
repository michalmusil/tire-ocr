using AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRunBatch.Dtos.BatchEvaluation;
using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunBatchAggregate;
using TireOcr.Shared.Result;

namespace AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRunBatch.Services;

public interface IBatchEvaluationService
{
    public Task<DataResult<BatchEvaluationDto>> EvaluateBatch(EvaluationRunBatchEntity batch,
        IncalculableInputsDto? inputs = null);

    /// <summary>
    /// Evaluates the batch AND uses the other relative batches to average its metrics and calculate inference
    /// stability with the related batches. 
    /// </summary>
    /// <param name="batch">The main evaluated batch</param>
    /// <param name="relatedBatches">The related batches for inference stability calculation and metrics averaging</param>
    /// <param name="inputs">Additional inputs for batch evaluation</param>
    /// <returns></returns>
    public Task<DataResult<BatchEvaluationDto>> EvaluateBatchWithRelatedBatches(EvaluationRunBatchEntity batch,
        List<EvaluationRunBatchEntity> relatedBatches, IncalculableInputsDto? inputs);
}