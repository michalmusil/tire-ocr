using AiPipeline.Orchestration.Runner.Application.PipelineResult.Commands.AddStepToResult;
using AiPipeline.Orchestration.Runner.Application.PipelineResult.Dtos;
using AiPipeline.Orchestration.Shared.All.Contracts.Events.StepCompletion;
using MediatR;
using TireOcr.Shared.Result;

namespace AiPipeline.Orchestration.Runner.WebApi.Consumers;

public class PipelineStepCompletedHandler
{
    private readonly IMediator _mediator;
    private readonly ILogger<PipelineStepCompletedHandler> _logger;

    public PipelineStepCompletedHandler(ILogger<PipelineStepCompletedHandler> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    public async Task HandleAsync(StepCompleted message)
    {
        _logger.LogInformation(
            $"Pipeline {message.PipelineId} step completed: {message.ProcedureIdentifier.NodeId} - {message.ProcedureIdentifier.ProcedureId}"
        );
        var dto = CreatePipelineStepDto.SuccessfulStep(
            nodeId: message.ProcedureIdentifier.NodeId,
            nodeProcedureId: message.ProcedureIdentifier.ProcedureId,
            finishedAt: message.CompletedAt,
            result: message.Result
        );
        var result = await _mediator.Send(new AddStepToResultCommand(message.PipelineId, dto));
        if (result.IsFailure)
        {
            _logger.LogCritical(
                $"Failed to persist completed pipeline {message.PipelineId} step: {result.PrimaryFailure!.Code} - {result.PrimaryFailure.Message}"
            );
            var failure = result.PrimaryFailure ?? new Failure(500, "Failed to persist completed pipeline step");
            failure.ThrowAsException();
        }
    }
}