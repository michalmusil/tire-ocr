using AiPipeline.Orchestration.Runner.Application.NodeType.Repositories;
using AiPipeline.Orchestration.Runner.Application.Pipeline.Builders;
using AiPipeline.Orchestration.Runner.Application.Pipeline.Dtos.Run;
using AiPipeline.Orchestration.Runner.Domain.NodeTypeAggregate;
using AiPipeline.Orchestration.Runner.Domain.PipelineAggregate;
using AiPipeline.Orchestration.Shared.Contracts.Schema;
using AiPipeline.Orchestration.Shared.Contracts.Schema.Properties;
using TireOcr.Shared.Result;

namespace AiPipeline.Orchestration.Runner.Infrastructure.Pipeline.Builders;

public class PipelineBuilder : IPipelineBuilder
{
    private readonly INodeTypeRepository _nodeTypeRepository;

    private IApElement? _pipelineInput = null;
    private readonly List<RunPipelineStepDto> _steps = [];
    private readonly List<PipelineInputFileDto> _inputFiles = [];

    public IApElement? PipelineInput => _pipelineInput;
    public IReadOnlyCollection<RunPipelineStepDto> Steps => _steps.AsReadOnly();
    public IReadOnlyCollection<PipelineInputFileDto> Files => _inputFiles.AsReadOnly();

    private static Failure NoInputProvidedFailure =>
        new Failure(422, "Pipeline input must be provided before pipeline can be built.");

    public PipelineBuilder(INodeTypeRepository nodeTypeRepository)
    {
        _nodeTypeRepository = nodeTypeRepository;
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

    public void AddFile(PipelineInputFileDto file)
    {
        _inputFiles.Add(file);
    }

    public void AddFiles(IEnumerable<PipelineInputFileDto> files)
    {
        _inputFiles.AddRange(files);
    }

    public bool RemoveFile(PipelineInputFileDto file)
    {
        return _inputFiles.Remove(file);
    }

    public async Task<DataResult<Domain.PipelineAggregate.Pipeline>> BuildAsync()
    {
        if (_pipelineInput is null)
            return DataResult<Domain.PipelineAggregate.Pipeline>.Failure(NoInputProvidedFailure);

        var nodes = await GetNodeTypesForAllStepsAsync();
        List<PipelineStep> pipelineSteps = [];
        for (var i = 0; i < _steps.Count; i++)
        {
            var requestedStep = _steps[i];
            var node = nodes.FirstOrDefault(n => n.Id == requestedStep.NodeId);
            if (node is null)
                return DataResult<Domain.PipelineAggregate.Pipeline>.NotFound(
                    $"Node type {requestedStep.NodeId} not found");

            var procedure = node.AvailableProcedures.FirstOrDefault(p => p.Id == requestedStep.ProcedureId);
            if (procedure is null)
                return DataResult<Domain.PipelineAggregate.Pipeline>.NotFound(
                    $"Procedure {requestedStep.ProcedureId} not found on node {node.Id}");

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

        var pipeline = new Domain.PipelineAggregate.Pipeline(steps: pipelineSteps);
        var validationResult = pipeline.Validate();
        return validationResult.Map(
            onSuccess: () => DataResult<Domain.PipelineAggregate.Pipeline>.Success(pipeline),
            onFailure: DataResult<Domain.PipelineAggregate.Pipeline>.Failure
        );
    }

    private async Task<List<Domain.NodeTypeAggregate.NodeType>> GetNodeTypesForAllStepsAsync()
    {
        var nodeIds = _steps.Select(s => s.NodeId).ToArray();
        return (await _nodeTypeRepository.GetNodeTypesByIdsAsync(nodeIds))
            .ToList();
    }

    private Result ValidateInputAgainstProcedure(NodeProcedure procedure)
    {
        if (_pipelineInput is null)
            return Result.Failure(NoInputProvidedFailure);


        var schemaIsValid = _pipelineInput.HasCompatibleSchemaWith(procedure.InputSchema);
        if (!schemaIsValid)
            return Result.Invalid($"Pipeline input doesn't match the '{procedure.Id}' procedure input schema.");

        var schemaFileInputs = procedure.InputSchema.GetAllDescendantsOfType<ApFile>();
        var providedFileNames = _inputFiles.Select(f => f.FileName).ToArray();
        var nonProvidedFileNames = schemaFileInputs
            .Where(sfi => !providedFileNames.Contains(sfi.FileName))
            .Select(sfi => sfi.FileName)
            .ToList();

        if (nonProvidedFileNames.Any())
            return Result.Invalid(
                $"Pipeline input contains files that have not been provided. Not provided files: {string.Join(',', nonProvidedFileNames)}");

        return Result.Success();
    }
}