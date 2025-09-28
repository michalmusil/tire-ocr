namespace AiPipeline.TireOcr.EvaluationTool.Infrastructure.Dtos.RemoteDbMatchingProcessor;

public record TireDbEntryResponseDto(
    int Width,
    decimal Diameter,
    int Profile,
    string Construction,
    int? LoadIndex,
    string? SpeedIndex,
    string LoadIndexSpeedIndex
);