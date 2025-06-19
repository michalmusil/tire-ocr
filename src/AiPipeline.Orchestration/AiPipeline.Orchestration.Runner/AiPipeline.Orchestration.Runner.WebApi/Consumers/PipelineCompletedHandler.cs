using AiPipeline.Orchestration.Shared.Contracts.Events.PipelineCompletion;

namespace AiPipeline.Orchestration.Runner.WebApi.Consumers;

public class PipelineCompletedHandler
{
    private readonly ILogger<PipelineCompletedHandler> _logger;

    public PipelineCompletedHandler(ILogger<PipelineCompletedHandler> logger)
    {
        _logger = logger;
    }

    public async Task HandleAsync(PipelineCompleted message)
    {
        _logger.LogInformation($"Pipeline completed: {message}");
    }
}