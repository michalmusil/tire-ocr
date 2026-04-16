using AiPipeline.TireOcr.EvaluationTool.Domain.Common;

namespace AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate;

public class PreprocessingResultEntity : TimestampedEntity
{
    public Guid Id { get; }
    public Guid RunId { get; private set; }
    public ImageValueObject PreprocessingResult { get; }
    public long DurationMs { get; }

    private PreprocessingResultEntity()
    {
    }

    public PreprocessingResultEntity(Guid evaluationRunId, ImageValueObject preprocessingResult, long durationMs,
        Guid? id = null)
    {
        Id = id ?? Guid.NewGuid();
        RunId = evaluationRunId;
        PreprocessingResult = preprocessingResult;
        DurationMs = durationMs;
    }

    public void SetEvaluationRunId(Guid evaluationRunId)
    {
        RunId = evaluationRunId;
        SetUpdated();
    }
}