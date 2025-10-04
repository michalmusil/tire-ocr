using TireOcr.Shared.Domain;

namespace AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate.DbMatch;

public class TireDbMatchValueObject : ValueObject
{
    public required TireCodeValueObject TireCode { get; init; }
    public required int TotalRequiredCharEdits { get; init; }
    public required decimal EstimatedAccuracy { get; init; }
    public int WidthEditDistance { get; init; }
    public int DiameterEditDistance { get; init; }
    public int ProfileEditDistance { get; init; }
    public int? ConstructionEditDistance { get; init; }
    public int LoadIndexEditDistance { get; init; }
    public int? LoadIndex2EditDistance { get; init; }
    public int SpeedIndexEditDistance { get; init; }

    protected override IEnumerable<object?> GetEqualityComponents() =>
    [
        TireCode, TotalRequiredCharEdits, EstimatedAccuracy, WidthEditDistance, DiameterEditDistance,
        ProfileEditDistance, ConstructionEditDistance, LoadIndexEditDistance, LoadIndex2EditDistance,
        SpeedIndexEditDistance
    ];
}