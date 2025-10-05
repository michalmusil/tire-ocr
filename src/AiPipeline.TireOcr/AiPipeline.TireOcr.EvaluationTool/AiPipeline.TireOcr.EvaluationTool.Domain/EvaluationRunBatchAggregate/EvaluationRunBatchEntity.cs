using AiPipeline.TireOcr.EvaluationTool.Domain.Common;
using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate;

namespace AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunBatchAggregate;

public class EvaluationRunBatchEntity: TimestampedEntity
{
    public Guid Id { get; }
    public string Title { get; }

    public DateTime? StartedAt => EvaluationRuns.Min(r => r.StartedAt);

    public DateTime? FinishedAt =>
        EvaluationRuns.All(r => r.HasFinished) ? EvaluationRuns.Max(e => e.FinishedAt) : null;

    public List<EvaluationRunEntity> _evaluationRuns { get; }
    public IReadOnlyCollection<EvaluationRunEntity> EvaluationRuns => _evaluationRuns.AsReadOnly();

    private EvaluationRunBatchEntity()
    {
    }

    public EvaluationRunBatchEntity(List<EvaluationRunEntity> evaluationRuns, string? title, Guid? id = null)
    {
        Id = id ?? Guid.NewGuid();
        Title = title ?? Id.ToString();
        _evaluationRuns = evaluationRuns;
    }

    public void AddEvaluationRuns(params EvaluationRunEntity[] evaluationRuns)
    {
        var newEvaluationRuns = evaluationRuns.Where(r => _evaluationRuns.All(existing => existing.Id != r.Id));
        _evaluationRuns.AddRange(newEvaluationRuns);
        SetUpdated();
    }
}