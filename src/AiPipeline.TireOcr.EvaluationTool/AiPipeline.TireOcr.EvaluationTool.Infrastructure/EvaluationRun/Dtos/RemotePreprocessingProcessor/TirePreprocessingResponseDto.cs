namespace AiPipeline.TireOcr.EvaluationTool.Infrastructure.EvaluationRun.Dtos.RemotePreprocessingProcessor;

public record TirePreprocessingResponseDto(
    string FileName,
    string ContentType,
    string Base64ImageData,
    long DurationMs
);