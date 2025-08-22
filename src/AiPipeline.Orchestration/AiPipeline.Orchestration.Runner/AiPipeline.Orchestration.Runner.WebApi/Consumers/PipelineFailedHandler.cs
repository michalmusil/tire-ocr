using AiPipeline.Orchestration.Runner.Application.PipelineResult.Commands.AddStepToResult;
using AiPipeline.Orchestration.Runner.Application.PipelineResult.Commands.MarkPipelineCompleted;
using AiPipeline.Orchestration.Runner.Application.PipelineResult.Dtos;
using AiPipeline.Orchestration.Runner.Domain.PipelineResultAggregate;
using AiPipeline.Orchestration.Shared.All.Contracts.Events.PipelineFailure;
using MediatR;
using TireOcr.Shared.Result;

namespace AiPipeline.Orchestration.Runner.WebApi.Consumers;

public class PipelineFailedHandler
{
    private readonly IMediator _mediator;
    private readonly ILogger<PipelineFailedHandler> _logger;

    public PipelineFailedHandler(ILogger<PipelineFailedHandler> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    public async Task HandleAsync(PipelineFailed message)
    {
        _logger.LogInformation(
            $"Pipeline {message.PipelineId} failed: {message.FailureCode} - {message.FailureReason}"
        );
        var dto = CreatePipelineStepDto.FailedStep(
            nodeId: message.ProcedureIdentifier.NodeId,
            nodeProcedureId: message.ProcedureIdentifier.ProcedureId,
            finishedAt: message.FailedAt,
            order: message.ProcedureIdentifier.OrderInPipeline,
            failureReason: new PipelineFailureReason(message.FailureCode, message.FailureReason,
                message.ExceptionMessage),
            outputValueSelector: message.ProcedureIdentifier.OutputValueSelector
        );
        var saveStepResult = await _mediator.Send(new AddStepToResultCommand(message.PipelineId, dto));
        if (saveStepResult.IsFailure)
        {
            _logger.LogCritical(
                $"Failed to persist failed pipeline {message.PipelineId} step: {saveStepResult.PrimaryFailure!.Code} - {saveStepResult.PrimaryFailure.Message}"
            );
            var failure = saveStepResult.PrimaryFailure ?? new Failure(500, "Failed to persist failed pipeline step");
            failure.ThrowAsException();
        }

        var result = await _mediator.Send(new MarkPipelineCompletedCommand(message.PipelineId, message.FailedAt));
        if (result.IsFailure)
        {
            _logger.LogCritical(
                $"Failed to persist marking pipeline {message.PipelineId} as completed after failing step: {result.PrimaryFailure!.Code} - {result.PrimaryFailure.Message}"
            );
            var failure = result.PrimaryFailure ??
                          new Failure(500, "Failed mark pipeline as completed after failing step");
            failure.ThrowAsException();
        }
    }
}