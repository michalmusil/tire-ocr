namespace AiPipeline.TireOcr.EvaluationTool.Infrastructure.EvaluationRun.Dtos.RemoteDbMatchingProcessor;

public record ParameterMatchResponseDto(
    int RequiredCharEdits,
    decimal EstimatedAccuracy
);