using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate;

namespace AiPipeline.TireOcr.EvaluationTool.Application.Dtos;

public record PreprocessingProcessorResult(
    ImageDto Image,
    long DurationMs
)
{
    public PreprocessingResultEntity ToDomain() => new(
        preprocessingResult: Image.ToDomain(),
        durationMs: DurationMs
    );
}