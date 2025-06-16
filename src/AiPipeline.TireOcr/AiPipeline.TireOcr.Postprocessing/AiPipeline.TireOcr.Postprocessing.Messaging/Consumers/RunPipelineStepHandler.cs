using AiPipeline.Orchestration.Shared.Contracts.Commands.RunPipelineStep;

namespace AiPipeline.TireOcr.Postprocessing.Messaging.Consumers;

public class RunPipelineStepHandler
{
    private readonly ILogger<RunPipelineStepHandler> _logger;

    public RunPipelineStepHandler(ILogger<RunPipelineStepHandler> logger)
    {
        _logger = logger;
    }

    public async Task HandleAsync(RunPipelineStep message)
    {
        _logger.LogInformation($"Run pipeline message consumed: {@message}");
    }
}