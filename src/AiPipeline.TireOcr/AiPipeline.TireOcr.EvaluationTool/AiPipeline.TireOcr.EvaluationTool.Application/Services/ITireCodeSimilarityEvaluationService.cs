using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate;
using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate.Evaluation;

namespace AiPipeline.TireOcr.EvaluationTool.Application.Services;

public interface ITireCodeSimilarityEvaluationService
{
    public Task<EvaluationEntity> EvaluateTireCodeSimilarity(TireCodeValueObject expectedTireCode,
        TireCodeValueObject actualTireCode);
}