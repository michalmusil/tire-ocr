using TireOcr.Shared.Domain;

namespace AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate.Evaluation;

public class ParameterEvaluationValueObject : ValueObject
{
    public required int Distance { get; init; }
    public required decimal EstimatedAccuracy { get; init; }
    protected override IEnumerable<object?> GetEqualityComponents() => [Distance, EstimatedAccuracy];
}