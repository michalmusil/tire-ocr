using AiPipeline.Orchestration.Runner.Application.PipelineResult.Commands.MarkPipelineCompleted;
using AiPipeline.Orchestration.Shared.Contracts.Events.PipelineCompletion;
using MediatR;
using TireOcr.Shared.Result;

namespace AiPipeline.Orchestration.Runner.WebApi.Consumers;

public class PipelineCompletedHandler
{
    private readonly IMediator _mediator;
    private readonly ILogger<PipelineCompletedHandler> _logger;

    public PipelineCompletedHandler(ILogger<PipelineCompletedHandler> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    public async Task HandleAsync(PipelineCompleted message)
    {
        _logger.LogInformation($"Pipeline {message.PipelineId} completed");
        var result = await _mediator.Send(new MarkPipelineCompletedCommand(message.PipelineId, message.CompletedAt));
        if (result.IsFailure)
        {
            _logger.LogCritical(
                $"Failed to persist marking pipeline {message.PipelineId} as completed: {result.PrimaryFailure!.Code} - {result.PrimaryFailure.Message}"
            );
            var failure = result.PrimaryFailure ?? new Failure(500, "Failed to mark pipeline as completed");
            failure.ThrowAsException();
        }
    }
}