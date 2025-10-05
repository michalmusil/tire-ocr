using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate;

namespace AiPipeline.TireOcr.EvaluationTool.Application.Dtos;

public record PreprocessingProcessorResult(
    Guid EvaluationRunId,
    ImageDto Image,
    long DurationMs
)
{
    public PreprocessingResultEntity ToDomain() => new(
        evaluationRunId: EvaluationRunId,
        preprocessingResult: Image.ToDomain(),
        durationMs: DurationMs
    );
}