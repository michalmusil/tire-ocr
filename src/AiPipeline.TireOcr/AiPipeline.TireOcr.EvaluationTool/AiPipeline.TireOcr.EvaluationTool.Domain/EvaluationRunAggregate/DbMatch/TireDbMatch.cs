namespace AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate.DbMatch;

public record TireDbMatch(
    int RequiredCharEdits,
    decimal EstimatedAccuracy,
    int Width,
    decimal Diameter,
    int Profile,
    string Construction,
    string LoadIndexSpeedIndex,
    int? LoadIndex,
    string? SpeedIndex
);