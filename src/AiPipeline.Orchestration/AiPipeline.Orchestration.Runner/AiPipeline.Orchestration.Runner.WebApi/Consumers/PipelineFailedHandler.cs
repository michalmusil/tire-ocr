using AiPipeline.Orchestration.Shared.Contracts.Events.PipelineFailure;

namespace AiPipeline.Orchestration.Runner.WebApi.Consumers;

public class PipelineFailedHandler
{
    private readonly ILogger<PipelineFailedHandler> _logger;

    public PipelineFailedHandler(ILogger<PipelineFailedHandler> logger)
    {
        _logger = logger;
    }

    public async Task HandleAsync(PipelineFailed message)
    {
        _logger.LogInformation($"Pipeline failed: {message}");
    }
}