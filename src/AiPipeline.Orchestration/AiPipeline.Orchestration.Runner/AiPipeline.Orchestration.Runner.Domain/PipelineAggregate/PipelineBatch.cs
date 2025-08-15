using TireOcr.Shared.Result;

namespace AiPipeline.Orchestration.Runner.Domain.PipelineAggregate;

public class PipelineBatch
{
    private static readonly PipelineBatchValidator Validator = new();
    public Guid Id { get; }
    public Guid UserId { get; }

    private readonly List<Pipeline> _pipelines;
    public IReadOnlyCollection<Pipeline> Pipelines => _pipelines.AsReadOnly();

    public PipelineBatch(Guid userId, Guid? id = null)
    {
        Id = id ?? Guid.NewGuid();
        UserId = userId;
        _pipelines = [];
    }

    public Result Validate()
    {
        var validationResult = Validator.Validate(this);
        if (!validationResult.IsValid)
            return Result.Failure(validationResult.Errors
                .Select(x => new Failure(422, $"{x.PropertyName}: {x.ErrorMessage}"))
                .ToArray()
            );

        var pipelineValidationFailures = Pipelines
            .Select(p => p.Validate())
            .SelectMany(r => r.Failures)
            .ToArray();

        if (!pipelineValidationFailures.Any())
            return Result.Failure(pipelineValidationFailures);

        return Result.Success();
    }

    public Result AddPipeline(Pipeline pipeline)
    {
        var alreadyContained = Pipelines.Any(p => p.Id == pipeline.Id);
        if (alreadyContained)
            return Result.Conflict($"Pipeline batch already contains a pipeline with id: {pipeline.Id}");


        _pipelines.Add(pipeline);
        return Result.Success();
    }

    public bool RemovePipeline(Pipeline pipeline)
    {
        return _pipelines.Remove(pipeline);
    }
}