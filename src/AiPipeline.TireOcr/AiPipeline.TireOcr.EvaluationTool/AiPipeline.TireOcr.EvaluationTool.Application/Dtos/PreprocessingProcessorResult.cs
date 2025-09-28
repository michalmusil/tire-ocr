namespace AiPipeline.TireOcr.EvaluationTool.Application.Dtos;

public record PreprocessingProcessorResult(
    ImageDto Image,
    long DurationMs
);