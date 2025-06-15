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
        var secondStep = pipeline.Steps.Skip(1).FirstOrDefault();

        if (firstStep is null)
            return Result.Invalid("Pipeline must contain at least one step to be published.");

        var currentProcedureIdentifier = new ProcedureIdentifier(firstStep.NodeId, firstStep.NodeProcedureId);
        var secondProcedureIdentifier = secondStep is null
            ? null
            : new ProcedureIdentifier(secondStep.NodeId, secondStep.NodeProcedureId);

        var command = new RunPipelineStep(
            CurrentStep: currentProcedureIdentifier,
            NextStep: secondProcedureIdentifier,
            CurrentStepInput: input
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