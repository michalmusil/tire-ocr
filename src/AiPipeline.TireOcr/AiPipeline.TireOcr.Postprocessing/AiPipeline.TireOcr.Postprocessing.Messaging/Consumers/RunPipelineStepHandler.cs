using AiPipeline.Orchestration.Shared.Contracts.Commands.RunPipelineStep;
using AiPipeline.Orchestration.Shared.Procedures;

namespace AiPipeline.TireOcr.Postprocessing.Messaging.Consumers;

public class RunPipelineStepHandler
{
    private readonly IProcedureRouter _procedureRouter;
    private readonly ILogger<RunPipelineStepHandler> _logger;

    public RunPipelineStepHandler(IProcedureRouter procedureRouter, ILogger<RunPipelineStepHandler> logger)
    {
        _procedureRouter = procedureRouter;
        _logger = logger;
    }

    public async Task HandleAsync(RunPipelineStep message)
    {
        _logger.LogInformation($"Run pipeline message consumed: {@message}");
    }
}