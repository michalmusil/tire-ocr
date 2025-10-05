using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate;
using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate.Evaluation;

namespace AiPipeline.TireOcr.EvaluationTool.Application.Services;

public interface ITireCodeSimilarityEvaluationService
{
    public Task<EvaluationValueObject> EvaluateTireCodeSimilarity(TireCodeValueObject expectedTireCode,
        TireCodeValueObject actualTireCode);
}