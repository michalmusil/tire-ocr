using AiPipeline.Orchestration.Runner.Application.Common.DataAccess;
using AiPipeline.Orchestration.Runner.Application.File.Repositories;
using AiPipeline.Orchestration.Runner.Application.Pipeline.Dtos.Run;
using AiPipeline.Orchestration.Runner.Domain.FileAggregate;
using AiPipeline.Orchestration.Runner.Domain.NodeTypeAggregate;
using AiPipeline.Orchestration.Runner.Domain.PipelineAggregate;
using AiPipeline.Orchestration.Shared.All.Contracts.Schema;
using AiPipeline.Orchestration.Shared.All.Contracts.Schema.Properties;
using TireOcr.Shared.Result;

namespace AiPipeline.Orchestration.Runner.Infrastructure.Pipeline.Builders;

public abstract class PipelineBuilderBase
{
    protected readonly Guid OwnerId;
    protected readonly List<RunPipelineStepDto> PipelineSteps;
    protected readonly List<FileValueObject> Files;

    protected readonly IUnitOfWork UnitOfWork;
    protected readonly IFileRepository FileRepository;

    protected PipelineBuilderBase(Guid ownerId, IUnitOfWork unitOfWork, IFileRepository fileRepository)
    {
        OwnerId = ownerId;
        UnitOfWork = unitOfWork;
        FileRepository = fileRepository;

        PipelineSteps = new();
        Files = new();
    }

    protected abstract List<ApFile> GetInputApFiles();

    protected async Task<Result> ValidateOwnerAsync()
    {
        var existingOwner = await UnitOfWork.UserRepository.GetByIdAsync(OwnerId);
        if (existingOwner is null)
            return Result.NotFound($"Pipeline owner with id '{OwnerId}' does not exist");
        return Result.Success();
    }

    protected async Task<Result> LoadInputFilesAsync()
    {
        var allApFilesOfInputResult = GetInputApFiles();

        var fileIds = allApFilesOfInputResult
            .Select(x => x.Id)
            .ToArray();

        var filesResult = await FileRepository.GetFilesByIdsAsync(userId: OwnerId, fileIds: fileIds);
        if (filesResult.IsFailure)
            return Result.Failure(filesResult.Failures);
        Files.Clear();
        Files.AddRange(filesResult.Data!);
        return Result.Success();
    }

    protected Result ValidateInputFilesExistAsync()
    {
        var inputApFiles = GetInputApFiles();
        var fileIds = inputApFiles
            .Select(x => x.Id)
            .Distinct()
            .ToArray();

        var notLoadedFileIds = fileIds
            .Except(Files.Select(x => x.Id))
            .ToArray();

        if (notLoadedFileIds.Any())
            return Result.NotFound(
                $"Some files of input were not found. Not found file ids: {string.Join(", ", notLoadedFileIds)}"
            );

        return Result.Success();
    }

    protected async Task<List<Domain.NodeTypeAggregate.NodeType>> GetNodeTypesForAllStepsAsync()
    {
        var nodeIds = PipelineSteps.Select(s => s.NodeId).ToArray();
        return (await UnitOfWork.NodeTypeRepository.GetNodeTypesByIdsAsync(nodeIds))
            .ToList();
    }

    protected DataResult<Domain.PipelineAggregate.Pipeline> BuildPipeline(
        IApElement input,
        List<Domain.NodeTypeAggregate.NodeType> nodes
    )
    {
        List<PipelineStep> pipelineSteps = [];
        for (var i = 0; i < PipelineSteps.Count; i++)
        {
            var requestedStep = PipelineSteps[i];
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
                var inputValidationResult = ValidateInputAgainstProcedure(procedure, input);
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
            userId: OwnerId,
            input: input,
            steps: pipelineSteps,
            pipelineFiles: Files
        );
        var validationResult = pipeline.Validate();
        return validationResult.Map(
            onSuccess: () => DataResult<Domain.PipelineAggregate.Pipeline>.Success(pipeline),
            onFailure: DataResult<Domain.PipelineAggregate.Pipeline>.Failure
        );
    }

    protected Result ValidateInputAgainstProcedure(NodeProcedure procedure, IApElement input)
    {
        var schemaIsValid = input.HasCompatibleSchemaWith(procedure.InputSchema);
        if (!schemaIsValid)
            return Result.Invalid($"Pipeline input doesn't match the '{procedure.Id}' procedure input schema.");

        return Result.Success();
    }
}