namespace AiPipeline.TireOcr.EvaluationTool.Infrastructure.Dtos.RemoteDbMatchingProcessor;

public record ParameterMatchResponseDto(
    int RequiredCharEdits,
    decimal EstimatedAccuracy
);