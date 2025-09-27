namespace AiPipeline.TireOcr.EvaluationTool.Application.Dtos.EvaluationRun.DbMatching;

public record TireCodeMatchDto(
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