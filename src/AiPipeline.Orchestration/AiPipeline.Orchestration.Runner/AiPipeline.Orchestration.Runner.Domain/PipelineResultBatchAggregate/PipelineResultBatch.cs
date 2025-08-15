using AiPipeline.Orchestration.Runner.Domain.Common;

namespace AiPipeline.Orchestration.Runner.Domain.PipelineResultBatchAggregate;

public class PipelineResultBatch : TimestampedEntity
{
    public Guid Id { get; }
    public Guid UserId { get; }
    public DateTime? FinishedAt { get; private set; }

    private PipelineResultBatch()
    {
    }

    public PipelineResultBatch(Guid userId, Guid? id = null)
    {
        Id = id ?? Guid.NewGuid();
        UserId = userId;
        FinishedAt = null;
    }

    public void MarkAsFinished(DateTime? finishedAt = null)
    {
        SetUpdated();
        var inUtc = finishedAt?.ToUniversalTime() ?? DateTime.UtcNow;
        FinishedAt = inUtc;
    }
}