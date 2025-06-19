using AiPipeline.Orchestration.Shared.Contracts.Commands.RunPipelineStep;
using AiPipeline.Orchestration.Shared.Procedures;

namespace AiPipeline.TireOcr.Ocr.Messaging.Consumers;

public class RunPipelineStepHandler
{
    private readonly IProcedureRouter _procedureRouter;
    private readonly ILogger<RunPipelineStepHandler> _logger;

    public RunPipelineStepHandler(ILogger<RunPipelineStepHandler> logger, IProcedureRouter procedureRouter)
    {
        _logger = logger;
        _procedureRouter = procedureRouter;
    }

    public async Task HandleAsync(RunPipelineStep message)
    {
        _logger.LogInformation($"Run pipeline message consumed: {@message}");
    }
}