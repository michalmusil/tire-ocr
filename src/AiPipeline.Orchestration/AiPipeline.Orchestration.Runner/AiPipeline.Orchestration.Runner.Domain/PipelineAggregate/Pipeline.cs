using TireOcr.Shared.Result;

namespace AiPipeline.Orchestration.Runner.Domain.PipelineAggregate;

public class Pipeline
{
    public Guid Id { get; }
    private readonly List<PipelineStep> _steps;
    public IReadOnlyCollection<PipelineStep> Steps => _steps.AsReadOnly();

    public Pipeline(Guid? id = null, List<PipelineStep>? steps = null)
    {
        Id = id ?? Guid.NewGuid();
        _steps = steps ?? new List<PipelineStep>();
    }

    public Result AddStep(PipelineStep step)
    {
        var alreadyContained = _steps.Any(s => s.Id == step.Id);
        if (alreadyContained)
        {
            return Result.Conflict($"Pipeline already contains a step with id: {step.Id}");
        }
        _steps.Add(step);
        return Result.Success();
    }
    
    public bool RemoveStep(PipelineStep step)
    {
        return _steps.Remove(step);
    }
}