using AiPipeline.TireOcr.EvaluationTool.Domain.Common;

namespace AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate;

public class PostprocessingResultEntity : TimestampedEntity
{
    public Guid Id { get; }
    public TireCodeValueObject TireCode { get; }
    public long DurationMs { get; }

    public PostprocessingResultEntity(TireCodeValueObject tireCode, long durationMs, Guid? id = null)
    {
        Id = id ?? Guid.NewGuid();
        TireCode = tireCode;
        DurationMs = durationMs;
    }
}