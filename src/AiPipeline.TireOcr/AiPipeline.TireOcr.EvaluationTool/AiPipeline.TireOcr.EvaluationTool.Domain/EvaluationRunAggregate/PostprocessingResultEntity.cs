using AiPipeline.TireOcr.EvaluationTool.Domain.Common;

namespace AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate;

public class PostprocessingResultEntity : TimestampedEntity
{
    public Guid Id { get; }
    public Guid RunId { get; private set; }
    public TireCodeValueObject TireCode { get; }
    public long DurationMs { get; }

    private PostprocessingResultEntity()
    {
    }

    public PostprocessingResultEntity(Guid evaluationRunId, TireCodeValueObject tireCode, long durationMs,
        Guid? id = null)
    {
        Id = id ?? Guid.NewGuid();
        RunId = evaluationRunId;
        TireCode = tireCode;
        DurationMs = durationMs;
    }

    public void SetEvaluationRunId(Guid evaluationRunId)
    {
        RunId = evaluationRunId;
        SetUpdated();
    }
}