using TireOcr.Shared.Domain;

namespace AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate.DbMatch;

public class DbMatchingResultValueObject : ValueObject
{
    public required List<TireDbMatch> Matches { get; init; }
    public string? ManufacturerMatch { get; init; }
    protected override IEnumerable<object?> GetEqualityComponents() => [Matches.ToArray(), ManufacturerMatch];
}