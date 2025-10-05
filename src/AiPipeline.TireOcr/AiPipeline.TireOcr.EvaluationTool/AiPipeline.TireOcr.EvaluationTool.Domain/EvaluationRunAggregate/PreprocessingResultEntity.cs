using AiPipeline.TireOcr.EvaluationTool.Domain.Common;

namespace AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate;

public class PreprocessingResultEntity: TimestampedEntity
{
    public Guid Id { get; }
    public ImageValueObject PreprocessingResult { get; }
    public long DurationMs { get; }

    public PreprocessingResultEntity(ImageValueObject preprocessingResult, long durationMs, Guid? id = null)
    {
        Id = id ?? Guid.NewGuid();
        PreprocessingResult = preprocessingResult;
        DurationMs = durationMs;
    }
}