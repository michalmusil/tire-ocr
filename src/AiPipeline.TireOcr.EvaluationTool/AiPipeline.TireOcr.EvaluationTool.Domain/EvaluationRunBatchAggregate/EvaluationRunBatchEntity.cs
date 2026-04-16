using AiPipeline.TireOcr.EvaluationTool.Domain.Common;
using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate;
using TireOcr.Shared.Result;

namespace AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunBatchAggregate;

public class EvaluationRunBatchEntity : TimestampedEntity
{
    public Guid Id { get; }
    public string Title { get; private set; }
    public string? Description { get; private set; }

    public DateTime? StartedAt => EvaluationRuns.Min(r => r.StartedAt);

    public DateTime? FinishedAt =>
        EvaluationRuns.All(r => r.HasFinished) ? EvaluationRuns.Max(e => e.FinishedAt) : null;

    public List<EvaluationRunEntity> _evaluationRuns { get; }
    public IReadOnlyCollection<EvaluationRunEntity> EvaluationRuns => _evaluationRuns.AsReadOnly();

    private EvaluationRunBatchEntity()
    {
    }

    public EvaluationRunBatchEntity(List<EvaluationRunEntity> evaluationRuns, string? title, string? description = null,
        Guid? id = null)
    {
        Id = id ?? Guid.NewGuid();
        Title = title ?? Id.ToString();
        Description = description;
        _evaluationRuns = evaluationRuns;
    }

    public void AddEvaluationRuns(params EvaluationRunEntity[] evaluationRuns)
    {
        var newEvaluationRuns = evaluationRuns.Where(r => _evaluationRuns.All(existing => existing.Id != r.Id));
        _evaluationRuns.AddRange(newEvaluationRuns);
        SetUpdated();
    }

    public Result SetTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title) || title.Trim().Length <= 2)
            return Result.Invalid("Batch title must be at least 3 characters long");

        Title = title;
        return Result.Success();
    }

    public Result SetDescription(string? description)
    {
        Description = description;
        return Result.Success();
    }
}