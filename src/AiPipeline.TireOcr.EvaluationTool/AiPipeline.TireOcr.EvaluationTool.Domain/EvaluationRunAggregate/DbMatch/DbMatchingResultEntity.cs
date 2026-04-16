using AiPipeline.TireOcr.EvaluationTool.Domain.Common;

namespace AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate.DbMatch;

public class DbMatchingResultEntity : TimestampedEntity
{
    public Guid Id { get; }
    public Guid RunId { get; private set; }
    public List<TireDbMatchValueObject> Matches { get; }
    public string? ManufacturerMatch { get; }
    public long DurationMs { get; }

    private DbMatchingResultEntity()
    {
    }

    public DbMatchingResultEntity(Guid evaluationRunId, List<TireDbMatchValueObject> matches, string? manufacturerMatch,
        long durationMs,
        Guid? id = null) : base()
    {
        Id = id ?? Guid.NewGuid();
        RunId = evaluationRunId;
        Matches = matches;
        ManufacturerMatch = manufacturerMatch;
        DurationMs = durationMs;
    }
    
    public void SetEvaluationRunId(Guid evaluationRunId)
    {
        RunId = evaluationRunId;
        SetUpdated();
    }
}