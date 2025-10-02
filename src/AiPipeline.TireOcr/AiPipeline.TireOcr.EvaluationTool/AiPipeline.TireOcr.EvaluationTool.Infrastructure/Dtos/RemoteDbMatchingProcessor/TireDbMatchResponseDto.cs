namespace AiPipeline.TireOcr.EvaluationTool.Infrastructure.Dtos.RemoteDbMatchingProcessor;

public record TireDbMatchResponseDto(
    TireDbEntryResponseDto TireEntry,
    int RequiredCharEdits,
    int MatchedMainParameterCount,
    decimal EstimatedAccuracy
);