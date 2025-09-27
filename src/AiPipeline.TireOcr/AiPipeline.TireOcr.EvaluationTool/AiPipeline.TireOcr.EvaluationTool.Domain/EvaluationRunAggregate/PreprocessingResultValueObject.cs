using TireOcr.Shared.Domain;

namespace AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate;

public class PreprocessingResultValueObject : ValueObject
{
    public required ImageValueObject PreprocessingResult { get; init; }
    public long DurationMs { get; init; }

    protected override IEnumerable<object?> GetEqualityComponents() => [PreprocessingResult, DurationMs];
}