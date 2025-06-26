using AiPipeline.Orchestration.Shared.Contracts.Events.StepCompletion;

namespace AiPipeline.Orchestration.Runner.WebApi.Consumers;

public class PipelineStepCompletedHandler
{
    private readonly ILogger<PipelineStepCompletedHandler> _logger;

    public PipelineStepCompletedHandler(ILogger<PipelineStepCompletedHandler> logger)
    {
        _logger = logger;
    }

    public async Task HandleAsync(StepCompleted message)
    {
        _logger.LogInformation($"Pipeline step completed: {message}");
    }
}