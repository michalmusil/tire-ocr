using AiPipeline.Orchestration.Runner.Domain.FileAggregate;
using TireOcr.Shared.Result;

namespace AiPipeline.Orchestration.Runner.Domain.PipelineAggregate;

public class Pipeline
{
    private static readonly PipelineValidator Validator = new PipelineValidator();

    public Guid Id { get; }
    public Guid UserId { get; }
    private readonly List<PipelineStep> _steps;
    private readonly List<FileValueObject> _pipelineFiles;
    public IReadOnlyCollection<PipelineStep> Steps => _steps.AsReadOnly();
    public IReadOnlyCollection<FileValueObject> Files => _pipelineFiles.AsReadOnly();

    public Pipeline(Guid userId, Guid? id = null, List<PipelineStep>? steps = null,
        List<FileValueObject>? pipelineFiles = null)
    {
        UserId = userId;
        Id = id ?? Guid.NewGuid();
        _steps = steps ?? new List<PipelineStep>();
        _pipelineFiles = pipelineFiles ?? new List<FileValueObject>();
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

    public Result AddStep(PipelineStep step)
    {
        var alreadyContained = _steps.Any(s => s.Id == step.Id);
        if (alreadyContained)
            return Result.Conflict($"Pipeline already contains a step with id: {step.Id}");


        _steps.Add(step);
        return Result.Success();
    }

    public bool RemoveStep(PipelineStep step)
    {
        return _steps.Remove(step);
    }

    public Result AddFile(FileValueObject file)
    {
        var alreadyContained = _pipelineFiles.Any(s => s.Id == file.Id);
        if (alreadyContained)
            return Result.Conflict($"Pipeline already contains a file with id: {file.Id}");

        return Result.Success();
    }

    public bool RemoveFile(FileValueObject file)
    {
        return _pipelineFiles.Remove(file);
    }
}