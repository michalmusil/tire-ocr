using TireOcr.Shared.Domain;

namespace AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate.Evaluation;

public class EvaluationValueObject : ValueObject
{
    public required TireCodeValueObject ExpectedTireCode { get; init; }
    public required int TotalDistance { get; init; }
    public required int FullMatchParameterCount { get; init; }
    public required decimal EstimatedAccuracy { get; init; }
    public required ParameterEvaluationValueObject? VehicleClassEvaluation { get; init; }
    public required ParameterEvaluationValueObject? WidthEvaluation { get; init; }
    public required ParameterEvaluationValueObject? DiameterEvaluation { get; init; }
    public required ParameterEvaluationValueObject? AspectRatioEvaluation { get; init; }
    public required ParameterEvaluationValueObject? ConstructionEvaluation { get; init; }
    public required ParameterEvaluationValueObject? LoadRangeEvaluation { get; init; }
    public required ParameterEvaluationValueObject? LoadIndexEvaluation { get; init; }
    public required ParameterEvaluationValueObject? LoadIndex2Evaluation { get; init; }
    public required ParameterEvaluationValueObject? SpeedRatingEvaluation { get; init; }

    protected override IEnumerable<object?> GetEqualityComponents() =>
    [
        ExpectedTireCode, TotalDistance, FullMatchParameterCount, EstimatedAccuracy, VehicleClassEvaluation,
        WidthEvaluation, DiameterEvaluation, AspectRatioEvaluation, ConstructionEvaluation, LoadRangeEvaluation,
        LoadIndexEvaluation, LoadIndex2Evaluation, SpeedRatingEvaluation
    ];
}