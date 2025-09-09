using AiPipeline.Orchestration.Runner.Application.Common.DataAccess;
using AiPipeline.Orchestration.Runner.Application.File.Repositories;
using AiPipeline.Orchestration.Runner.Application.Pipeline.Builders;
using AiPipeline.Orchestration.Runner.Application.Pipeline.Dtos.Run;
using AiPipeline.Orchestration.Runner.Domain.PipelineAggregate;
using AiPipeline.Orchestration.Shared.All.Contracts.Schema;
using AiPipeline.Orchestration.Shared.All.Contracts.Schema.Properties;
using TireOcr.Shared.Result;

namespace AiPipeline.Orchestration.Runner.Infrastructure.Pipeline.Builders;

public class PipelineBatchBuilder : PipelineBuilderBase, IPipelineBatchBuilder
{
    private readonly List<IApElement> _inputs;

    public PipelineBatchBuilder(Guid userId, IUnitOfWork unitOfWork, IFileRepository fileRepository)
        : base(userId, unitOfWork, fileRepository)
    {
        _inputs = new();
    }

    public IReadOnlyCollection<IApElement> PipelineInputs => _inputs.AsReadOnly();
    public IReadOnlyCollection<RunPipelineStepDto> Steps => PipelineSteps.AsReadOnly();

    public void AddInput(IApElement input)
    {
        _inputs.Add(input);
    }

    public void AddInputs(params IApElement[] inputs)
    {
        _inputs.AddRange(inputs);
    }

    public bool RemoveInput(IApElement input)
    {
        return _inputs.Remove(input);
    }

    public void AddStep(RunPipelineStepDto step)
    {
        PipelineSteps.Add(step);
    }

    public void AddSteps(params RunPipelineStepDto[] steps)
    {
        PipelineSteps.AddRange(steps);
    }

    public bool RemoveStep(RunPipelineStepDto step)
    {
        return PipelineSteps.Remove(step);
    }

    public async Task<DataResult<PipelineBatch>> ValidateAndBuildAsync()
    {
        if (_inputs.Count == 0)
            return DataResult<PipelineBatch>.Invalid("At least one input in PipelineBatch is required.");

        if (PipelineSteps.Count == 0)
            return DataResult<PipelineBatch>.Invalid("At least one pipeline step is required.");

        var ownerValidationResult = await ValidateOwnerAsync();
        if (ownerValidationResult.IsFailure)
            return DataResult<PipelineBatch>.Failure(ownerValidationResult.Failures);

        var loadFilesResult = await LoadInputFilesAsync();
        if (loadFilesResult.IsFailure)
            return DataResult<PipelineBatch>.Failure(loadFilesResult.Failures);

        var inputFilesValidationResult = ValidateInputFilesExistAsync();
        if (inputFilesValidationResult.IsFailure)
            return DataResult<PipelineBatch>.Failure(inputFilesValidationResult.Failures);

        var nodes = await GetNodeTypesForAllStepsAsync();
        var pipelineResults = _inputs
            .Select(i => BuildPipeline(i, nodes))
            .ToList();

        var pipelineResultFailures = pipelineResults
            .SelectMany(pr => pr.Failures)
            .ToArray();
        if (pipelineResultFailures.Any())
            return DataResult<PipelineBatch>.Failure(pipelineResultFailures);
        var pipelines = pipelineResults
            .Select(pr => pr.Data!)
            .ToArray();
        
        var pipelineBatch = new PipelineBatch(OwnerId);
        var addPipelinesResult = pipelineBatch.AddPipelines(pipelines);
        if (addPipelinesResult.IsFailure)
            return DataResult<PipelineBatch>.Failure(addPipelinesResult.Failures);

        var batchValidationResult = pipelineBatch.Validate();
        if (batchValidationResult.IsFailure)
            return DataResult<PipelineBatch>.Failure(batchValidationResult.Failures);
        
        return DataResult<PipelineBatch>.Success(pipelineBatch);
    }

    protected override List<ApFile> GetInputApFiles()
    {
        var inputItselfApFiles = _inputs
            .Where(i => i is ApFile)
            .Cast<ApFile>();
        var descendantApFiles = _inputs
            .SelectMany(i => i.GetAllDescendantsOfType<ApFile>());

        var apFiles = new List<ApFile>();
        apFiles.AddRange(inputItselfApFiles);
        apFiles.AddRange(descendantApFiles);

        var uniqueApFiles = apFiles
            .DistinctBy(f => f.Id)
            .ToList();

        return uniqueApFiles;
    }
}