namespace AiPipeline.TireOcr.EvaluationTool.Infrastructure.Dtos.RemotePreprocessingProcessor;

public record TirePreprocessingResponseDto(
    string FileName,
    string ContentType,
    string Base64ImageData,
    long DurationMs
);