using AiPipeline.Orchestration.Runner.Application.Common.DataAccess;
using AiPipeline.Orchestration.Runner.Application.File.Repositories;
using AiPipeline.Orchestration.Runner.Application.Pipeline.Dtos.Run;
using AiPipeline.Orchestration.Runner.Domain.FileAggregate;
using AiPipeline.Orchestration.Runner.Domain.NodeTypeAggregate;
using AiPipeline.Orchestration.Runner.Domain.PipelineAggregate;
using AiPipeline.Orchestration.Shared.All.Contracts.Schema;
using AiPipeline.Orchestration.Shared.All.Contracts.Schema.Properties;
using AiPipeline.Orchestration.Shared.All.Contracts.Schema.Selectors;
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
            var schemaValidationResult = ValidateStepSchemas(
                forProcedure: procedure,
                currentStep: requestedStep,
                pipelineInput: input,
                previousStep: isFirstStep ? null : pipelineSteps[i - 1]
            );
            if (schemaValidationResult.IsFailure)
                return DataResult<Domain.PipelineAggregate.Pipeline>.Failure(schemaValidationResult.Failures);

            pipelineSteps.Add(new PipelineStep(
                nodeId: node.Id,
                nodeProcedureId: procedure.Id,
                schemaVersion: procedure.SchemaVersion,
                order: i + 1,
                outputValueSelector: requestedStep.OutputValueSelector,
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

    private Result ValidateStepSchemas(NodeProcedure forProcedure, IApElement pipelineInput,
        RunPipelineStepDto currentStep, PipelineStep? previousStep)
    {
        var isFirstStep = previousStep is null;
        if (isFirstStep)
        {
            var inputValidationResult = ValidateInputAgainstProcedure(
                procedure: forProcedure,
                input: pipelineInput,
                outputValueSelector: null,
                previousProcedureId: null
            );
            if (inputValidationResult.IsFailure)
                return inputValidationResult;
        }
        else
        {
            var schemaValidationResult = ValidateInputAgainstProcedure(
                procedure: forProcedure,
                input: previousStep!.OutputSchema,
                outputValueSelector: previousStep.OutputValueSelector,
                previousProcedureId: previousStep.NodeProcedureId
            );
            if (schemaValidationResult.IsFailure)
                return schemaValidationResult;
        }

        return Result.Success();
    }

    private Result ValidateInputAgainstProcedure(NodeProcedure procedure, IApElement input, string? outputValueSelector,
        string? previousProcedureId)
    {
        var incomingSchema = input;
        if (outputValueSelector is not null)
        {
            var selectorResult = ChildElementSelector.FromString(outputValueSelector);
            if (selectorResult.IsFailure)
                return selectorResult;
            var selector = selectorResult.Data!;
            var selectedSchemaResult = selector.Select(input);
            if (selectedSchemaResult.IsFailure)
                return selectedSchemaResult;

            incomingSchema = selectedSchemaResult.Data!;
        }

        var acceptingSchema = procedure.InputSchema;

        var schemaIsValid = incomingSchema.HasCompatibleSchemaWith(acceptingSchema);
        if (!schemaIsValid)
        {
            var incomingIdentifier = previousProcedureId is null
                ? "Pipeline input"
                : $"Procedure '{previousProcedureId}' output schema";
            var incomingIdentifierAppendix =
                outputValueSelector is null ? "" : $" with value selector: '{outputValueSelector}'";
            return Result.Invalid(
                $"{incomingIdentifier}{incomingIdentifierAppendix} doesn't match the '{procedure.Id}' procedure input schema."
            );
        }

        return Result.Success();
    }
}