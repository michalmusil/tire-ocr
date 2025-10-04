using TireOcr.Shared.Domain;

namespace AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate.DbMatch;

public class DbMatchingResultValueObject : ValueObject
{
    public required List<TireDbMatchValueObject> Matches { get; init; }
    public string? ManufacturerMatch { get; init; }
    public required long DurationMs { get; init; }

    protected override IEnumerable<object?> GetEqualityComponents() =>
        [Matches.ToArray(), ManufacturerMatch, DurationMs];
}