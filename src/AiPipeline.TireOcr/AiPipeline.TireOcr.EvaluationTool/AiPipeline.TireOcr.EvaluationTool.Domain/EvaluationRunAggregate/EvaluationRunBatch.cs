namespace AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate;

public class EvaluationRunBatch
{
    public Guid Id { get; }
    
    public DateTime? StartedAt => EvaluationRuns.Min(r => r.StartedAt);
    public DateTime? FinishedAt =>
        EvaluationRuns.All(r => r.HasFinished) ? EvaluationRuns.Max(e => e.FinishedAt) : null;

    public List<EvaluationRun> _evaluationRuns { get; }
    public IReadOnlyCollection<EvaluationRun> EvaluationRuns => _evaluationRuns.AsReadOnly();

    private EvaluationRunBatch()
    {
    }

    public EvaluationRunBatch(List<EvaluationRun> evaluationRuns, Guid? id = null)
    {
        Id = id ?? Guid.NewGuid();
        _evaluationRuns = evaluationRuns;
    }

    public void AddEvaluationRuns(params EvaluationRun[] evaluationRuns)
    {
        var newEvaluationRuns = evaluationRuns.Where(r => _evaluationRuns.All(existing => existing.Id != r.Id));
        _evaluationRuns.AddRange(newEvaluationRuns);
    }
}