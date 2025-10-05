using AiPipeline.TireOcr.EvaluationTool.Domain.Common;

namespace AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate.DbMatch;

public class DbMatchingResultEntity: TimestampedEntity
{
    public Guid Id { get; }
    public List<TireDbMatchValueObject> Matches { get; }
    public string? ManufacturerMatch { get; }
    public long DurationMs { get; }

    public DbMatchingResultEntity(List<TireDbMatchValueObject> matches, string? manufacturerMatch, long durationMs,
        Guid? id = null): base()
    {
        Id = id ?? Guid.NewGuid();
        Matches = matches;
        ManufacturerMatch = manufacturerMatch;
        DurationMs = durationMs;
    }
}