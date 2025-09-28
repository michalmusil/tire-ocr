namespace AiPipeline.TireOcr.EvaluationTool.Infrastructure.Dtos.RemoteDbMatchingProcessor;

public record TireDbMatchResponseDto(
    TireDbEntryResponseDto TireEntryResponse,
    int RequiredCharEdits,
    decimal EstimatedAccuracy
);