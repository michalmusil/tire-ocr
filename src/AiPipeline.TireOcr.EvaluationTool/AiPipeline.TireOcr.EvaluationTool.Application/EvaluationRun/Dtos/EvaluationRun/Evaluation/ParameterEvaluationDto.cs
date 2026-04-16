using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate.Evaluation;

namespace AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRun.Dtos.EvaluationRun.Evaluation;

public record ParameterEvaluationDto(
    int Distance,
    decimal EstimatedAccuracy
    // decimal Cer
)
{
    public static ParameterEvaluationDto FromDomain(ParameterEvaluationValueObject domain) =>
        new(domain.Distance, domain.EstimatedAccuracy);
}