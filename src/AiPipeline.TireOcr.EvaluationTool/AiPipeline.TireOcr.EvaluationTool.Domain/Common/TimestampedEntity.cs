namespace AiPipeline.TireOcr.EvaluationTool.Domain.Common;

public abstract class TimestampedEntity
{
    public DateTime CreatedAt { get; protected set; }
    public DateTime UpdatedAt { get; private set; }

    protected TimestampedEntity()
    {
        var now = DateTime.UtcNow;
        CreatedAt = now;
        UpdatedAt = now;
    }

    protected TimestampedEntity(DateTime createdAt, DateTime updatedAt)
    {
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    protected void SetUpdated()
    {
        UpdatedAt = DateTime.UtcNow;
    }
}