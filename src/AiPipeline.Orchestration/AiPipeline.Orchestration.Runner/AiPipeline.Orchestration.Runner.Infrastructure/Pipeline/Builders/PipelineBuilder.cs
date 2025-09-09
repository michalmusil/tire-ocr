using AiPipeline.Orchestration.Runner.Application.Common.DataAccess;
using AiPipeline.Orchestration.Runner.Application.File.Repositories;
using AiPipeline.Orchestration.Runner.Application.Pipeline.Builders;
using AiPipeline.Orchestration.Runner.Application.Pipeline.Dtos.Run;
using AiPipeline.Orchestration.Shared.All.Contracts.Schema;
using AiPipeline.Orchestration.Shared.All.Contracts.Schema.Properties;
using TireOcr.Shared.Result;

namespace AiPipeline.Orchestration.Runner.Infrastructure.Pipeline.Builders;

public class PipelineBuilder : PipelineBuilderBase, IPipelineBuilder
{
    private IApElement? _pipelineInput;
    public IApElement? PipelineInput => _pipelineInput;
    public IReadOnlyCollection<RunPipelineStepDto> Steps => PipelineSteps.AsReadOnly();

    private static Failure NoInputProvidedFailure =>
        new Failure(422, "Pipeline input must be provided before pipeline can be built.");

    public PipelineBuilder(Guid userId, IUnitOfWork unitOfWork, IFileRepository fileRepository)
        : base(userId, unitOfWork, fileRepository)
    {
        _pipelineInput = null;
    }


    public void SetPipelineInput(IApElement input)
    {
        _pipelineInput = input;
    }

    public void AddStep(RunPipelineStepDto step)
    {
        PipelineSteps.Add(step);
    }

    public void AddSteps(List<RunPipelineStepDto> steps)
    {
        PipelineSteps.AddRange(steps);
    }

    public bool RemoveStep(RunPipelineStepDto step)
    {
        return PipelineSteps.Remove(step);
    }

    public async Task<DataResult<Domain.PipelineAggregate.Pipeline>> ValidateAndBuildAsync()
    {
        if (_pipelineInput is null)
            return DataResult<Domain.PipelineAggregate.Pipeline>.Failure(NoInputProvidedFailure);

        if (PipelineSteps.Count == 0)
            return DataResult<Domain.PipelineAggregate.Pipeline>.Invalid("At least one pipeline step is required.");

        var ownerValidationResult = await ValidateOwnerAsync();
        if (ownerValidationResult.IsFailure)
            return DataResult<Domain.PipelineAggregate.Pipeline>.Failure(ownerValidationResult.Failures);

        var loadFilesResult = await LoadInputFilesAsync();
        if (loadFilesResult.IsFailure)
            return DataResult<Domain.PipelineAggregate.Pipeline>.Failure(loadFilesResult.Failures);

        var inputFilesValidationResult = ValidateInputFilesExistAsync();
        if (inputFilesValidationResult.IsFailure)
            return DataResult<Domain.PipelineAggregate.Pipeline>.Failure(inputFilesValidationResult.Failures);

        var nodes = await GetNodeTypesForAllStepsAsync();
        var pipelineResult = BuildPipeline(_pipelineInput, nodes);

        return pipelineResult;
    }

    protected override List<ApFile> GetInputApFiles()
    {
        if (_pipelineInput is null)
            return [];

        var apFiles = _pipelineInput.GetAllDescendantsOfType<ApFile>();
        if (_pipelineInput is ApFile inputApFile)
            apFiles.Add(inputApFile);

        return apFiles;
    }
}