using AiPipeline.TireOcr.EvaluationTool.Application.Common.Dtos;
using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate;

namespace AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRun.Dtos;

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