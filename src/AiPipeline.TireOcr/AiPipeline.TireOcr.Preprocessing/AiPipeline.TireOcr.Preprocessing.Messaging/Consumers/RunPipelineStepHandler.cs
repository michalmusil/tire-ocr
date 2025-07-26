using AiPipeline.Orchestration.Shared.All.Contracts.Commands.RunPipelineStep;
using AiPipeline.Orchestration.Shared.Nodes.Procedures.Routing;

namespace AiPipeline.TireOcr.Preprocessing.Messaging.Consumers;

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
        await _procedureRouter.ProcessPipelineStep(message);
    }
}