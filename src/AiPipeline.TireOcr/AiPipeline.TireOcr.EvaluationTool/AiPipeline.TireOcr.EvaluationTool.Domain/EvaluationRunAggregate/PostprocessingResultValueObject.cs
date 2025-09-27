using TireOcr.Shared.Domain;

namespace AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate;

public class PostprocessingResultValueObject : ValueObject
{
    public required TireCodeValueObject TireCode { get; init; }
    public long DurationMs { get; init; }


    protected override IEnumerable<object?> GetEqualityComponents() => [TireCode, DurationMs];
}