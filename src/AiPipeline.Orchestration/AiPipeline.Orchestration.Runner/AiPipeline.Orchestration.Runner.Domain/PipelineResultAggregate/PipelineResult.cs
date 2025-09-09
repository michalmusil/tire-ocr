using AiPipeline.Orchestration.Runner.Domain.Common;
using AiPipeline.Orchestration.Shared.All.Contracts.Schema;
using TireOcr.Shared.Result;

namespace AiPipeline.Orchestration.Runner.Domain.PipelineResultAggregate;

public class PipelineResult : TimestampedEntity
{
    private static readonly PipelineResultValidator Validator = new PipelineResultValidator();
    public Guid Id { get; }
    public Guid PipelineId { get; }
    public Guid UserId { get; }
    public Guid? BatchId { get; }
    public IApElement? InitialInput { get; }
    public DateTime? FinishedAt { get; private set; }
    public readonly List<PipelineStepResult> _stepResults;

    public IReadOnlyCollection<PipelineStepResult> StepResults => _stepResults
        .OrderBy(s => s.Order)
        .ToList()
        .AsReadOnly();

    public bool Succeeded => FinishedAt is not null && StepResults.All(psr => psr.WasSuccessful);
    public bool Failed => FinishedAt is not null && StepResults.Any(psr => psr.FailureReason is not null);

    private PipelineResult()
    {
    }

    public PipelineResult(Guid pipelineId, Guid userId, Guid? batchId, IApElement? initialInput, Guid? id = null)
    {
        Id = id ?? Guid.NewGuid();
        PipelineId = pipelineId;
        UserId = userId;
        BatchId = batchId;
        InitialInput = initialInput;
        FinishedAt = null;
        _stepResults = new List<PipelineStepResult>();
    }

    public Result Validate()
    {
        var validationResult = Validator.Validate(this);
        if (validationResult.IsValid)
            return Result.Success();

        return Result.Failure(validationResult.Errors
            .Select(x => new Failure(422, $"{x.PropertyName}: {x.ErrorMessage}"))
            .ToArray()
        );
    }

    public Result AddStepResult(PipelineStepResult stepResult)
    {
        var existingStepResult = _stepResults.FirstOrDefault(s => s.Id == stepResult.Id);
        if (existingStepResult is not null)
            return Result.Conflict($"Step result '{stepResult.Id}' is already stored in result '{Id}'");
        _stepResults.Add(stepResult);
        SetUpdated();
        return Result.Success();
    }

    public void MarkAsFinished(DateTime? finishedAt = null)
    {
        SetUpdated();
        var inUtc = finishedAt?.ToUniversalTime() ?? DateTime.UtcNow;
        FinishedAt = inUtc;
    }
}