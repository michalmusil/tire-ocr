using AiPipeline.Orchestration.Runner.Application.Pipeline.Services;
using AiPipeline.Orchestration.Shared.Contracts.Commands.RunPipelineStep;
using AiPipeline.Orchestration.Shared.Contracts.Schema;
using TireOcr.Shared.Result;
using Wolverine;

namespace AiPipeline.Orchestration.Runner.Infrastructure.Pipeline.Services;

public class PipelineRabbitMqPublisherService : IPipelinePublisherService
{
    private readonly IMessageBus _messageBus;

    public PipelineRabbitMqPublisherService(IMessageBus messageBus)
    {
        _messageBus = messageBus;
    }

    public async Task<Result> PublishPipeline(Domain.PipelineAggregate.Pipeline pipeline, IApElement input)
    {
        var firstStep = pipeline.Steps.FirstOrDefault();

        if (firstStep is null)
            return Result.Invalid("Pipeline must contain at least one step to be published.");
        var currentProcedureIdentifier = new ProcedureIdentifier(firstStep.NodeId, firstStep.NodeProcedureId);

        var nextSteps = pipeline.Steps.Skip(1).ToList();
        var nextProcedureIdentifiers = nextSteps
            .Select(s => new ProcedureIdentifier(s.NodeId, s.NodeProcedureId))
            .ToList();

        var fileReferences = pipeline.Files
            .Select(f => new FileReference(f.Id, f.StorageProvider, f.Path, f.ContentType))
            .ToList();

        var command = new RunPipelineStep(
            PipelineId: pipeline.Id,
            CurrentStep: currentProcedureIdentifier,
            CurrentStepInput: input,
            NextSteps: nextProcedureIdentifiers,
            FileReferences: fileReferences
        );

        try
        {
            await _messageBus.PublishAsync(command);
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(new Failure(500, ex.Message));
        }
    }
}