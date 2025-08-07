using AiPipeline.Orchestration.Runner.Application.Common.DataAccess;
using AiPipeline.Orchestration.Runner.Application.File.Repositories;
using AiPipeline.Orchestration.Runner.Application.Pipeline.Builders;
using AiPipeline.Orchestration.Runner.Application.Pipeline.Dtos.Run;
using AiPipeline.Orchestration.Runner.Domain.FileAggregate;
using AiPipeline.Orchestration.Runner.Domain.NodeTypeAggregate;
using AiPipeline.Orchestration.Runner.Domain.PipelineAggregate;
using AiPipeline.Orchestration.Shared.All.Contracts.Schema;
using AiPipeline.Orchestration.Shared.All.Contracts.Schema.Properties;
using TireOcr.Shared.Result;

namespace AiPipeline.Orchestration.Runner.Infrastructure.Pipeline.Builders;

public class PipelineBuilder : IPipelineBuilder
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileRepository _fileRepository;

    private IApElement? _pipelineInput;
    private readonly List<RunPipelineStepDto> _steps;
    private readonly List<FileValueObject> _files;

    public IApElement? PipelineInput => _pipelineInput;
    public IReadOnlyCollection<RunPipelineStepDto> Steps => _steps.AsReadOnly();

    private static Failure NoInputProvidedFailure =>
        new Failure(422, "Pipeline input must be provided before pipeline can be built.");

    public PipelineBuilder(IUnitOfWork unitOfWork, IFileRepository fileRepository)
    {
        _unitOfWork = unitOfWork;
        _fileRepository = fileRepository;
        _pipelineInput = null;
        _steps = new List<RunPipelineStepDto>();
        _files = new List<FileValueObject>();
    }


    public void SetPipelineInput(IApElement input)
    {
        _pipelineInput = input;
    }

    public void AddStep(RunPipelineStepDto step)
    {
        _steps.Add(step);
    }

    public void AddSteps(List<RunPipelineStepDto> steps)
    {
        _steps.AddRange(steps);
    }

    public bool RemoveStep(RunPipelineStepDto step)
    {
        return _steps.Remove(step);
    }

    public async Task<DataResult<Domain.PipelineAggregate.Pipeline>> BuildAsync()
    {
        if (_pipelineInput is null)
            return DataResult<Domain.PipelineAggregate.Pipeline>.Failure(NoInputProvidedFailure);

        var loadFilesResult = await LoadInputFilesAsync();
        if (loadFilesResult.IsFailure)
            return DataResult<Domain.PipelineAggregate.Pipeline>.Failure(loadFilesResult.Failures);

        var inputFilesValidationResult = await ValidateInputFilesExistAsync();
        if (inputFilesValidationResult.IsFailure)
            return DataResult<Domain.PipelineAggregate.Pipeline>.Failure(inputFilesValidationResult.Failures);

        var nodes = await GetNodeTypesForAllStepsAsync();
        List<PipelineStep> pipelineSteps = [];
        for (var i = 0; i < _steps.Count; i++)
        {
            var requestedStep = _steps[i];
            var node = nodes.FirstOrDefault(n => n.Id == requestedStep.NodeId);
            if (node is null)
                return DataResult<Domain.PipelineAggregate.Pipeline>.NotFound(
                    $"Node type '{requestedStep.NodeId}' not found");

            var procedure = node.AvailableProcedures.FirstOrDefault(p => p.Id == requestedStep.ProcedureId);
            if (procedure is null)
                return DataResult<Domain.PipelineAggregate.Pipeline>.NotFound(
                    $"Procedure '{requestedStep.ProcedureId}' not found on node {node.Id}");

            var isFirstStep = i == 0;
            if (isFirstStep)
            {
                var inputValidationResult = ValidateInputAgainstProcedure(procedure);
                if (inputValidationResult.IsFailure)
                    return DataResult<Domain.PipelineAggregate.Pipeline>.Failure(inputValidationResult.Failures);
            }


            pipelineSteps.Add(new PipelineStep(
                nodeId: node.Id,
                nodeProcedureId: procedure.Id,
                schemaVersion: procedure.SchemaVersion,
                inputSchema: procedure.InputSchema,
                outputSchema: procedure.OutputSchema
            ));
        }

        var pipeline = new Domain.PipelineAggregate.Pipeline(
            steps: pipelineSteps,
            pipelineFiles: _files
        );
        var validationResult = pipeline.Validate();
        return validationResult.Map(
            onSuccess: () => DataResult<Domain.PipelineAggregate.Pipeline>.Success(pipeline),
            onFailure: DataResult<Domain.PipelineAggregate.Pipeline>.Failure
        );
    }

    private async Task<List<Domain.NodeTypeAggregate.NodeType>> GetNodeTypesForAllStepsAsync()
    {
        var nodeIds = _steps.Select(s => s.NodeId).ToArray();
        return (await _unitOfWork.NodeTypeEntityRepository.GetNodeTypesByIdsAsync(nodeIds))
            .ToList();
    }

    private Result ValidateInputAgainstProcedure(NodeProcedure procedure)
    {
        if (_pipelineInput is null)
            return Result.Failure(NoInputProvidedFailure);


        var schemaIsValid = _pipelineInput.HasCompatibleSchemaWith(procedure.InputSchema);
        if (!schemaIsValid)
            return Result.Invalid($"Pipeline input doesn't match the '{procedure.Id}' procedure input schema.");

        return Result.Success();
    }

    private async Task<Result> LoadInputFilesAsync()
    {
        var allApFilesOfInputResult = GetAllApFilesOfInput();
        return await allApFilesOfInputResult.MapAsync(
            onFailure: failures => Task.FromResult(Result.Failure(failures)),
            onSuccess: async apFiles =>
            {
                var fileIds = apFiles
                    .Select(x => x.Id)
                    .Distinct()
                    .ToArray();

                var foundFiles = (await _fileRepository.GetFilesByIdsAsync(fileIds))
                    .ToList();
                _files.Clear();
                _files.AddRange(foundFiles);
                return Result.Success();
            }
        );
    }

    private async Task<Result> ValidateInputFilesExistAsync()
    {
        var allApFilesOfInputResult = GetAllApFilesOfInput();
        return await allApFilesOfInputResult.MapAsync(
            onFailure: failures => Task.FromResult(Result.Failure(failures)),
            onSuccess: apFiles =>
            {
                var fileIds = apFiles
                    .Select(x => x.Id)
                    .Distinct()
                    .ToArray();

                var notLoadedFileIds = fileIds
                    .Except(_files.Select(x => x.Id))
                    .ToArray();

                if (notLoadedFileIds.Any())
                    return Task.FromResult(
                        Result.NotFound(
                            $"Some files from the pipeline input were not found. Not found file ids: {string.Join(", ", notLoadedFileIds)}"
                        )
                    );

                return Task.FromResult(Result.Success());
            }
        );
    }

    private DataResult<List<ApFile>> GetAllApFilesOfInput()
    {
        if (_pipelineInput is null)
            return DataResult<List<ApFile>>.Failure(NoInputProvidedFailure);

        var apFiles = _pipelineInput.GetAllDescendantsOfType<ApFile>();
        if (_pipelineInput is ApFile inputApFile)
            apFiles.Add(inputApFile);

        return DataResult<List<ApFile>>.Success(apFiles);
    }
}