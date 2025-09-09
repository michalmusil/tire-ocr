using AiPipeline.Orchestration.Shared.All.Contracts.Commands.RunPipelineStep;
using AiPipeline.Orchestration.Shared.All.Contracts.Events.StepCompletion;
using AiPipeline.Orchestration.Shared.All.Contracts.Schema;
using TireOcr.Shared.Result;
using Wolverine;

namespace AiPipeline.Orchestration.Shared.Nodes.Procedures.ProcedureCompletion;

public class DefaultProcedureCompletionStrategy : IProcedureCompletionStrategy
{
    public async Task<Result> Execute(IMessageBus bus, RunPipelineStep processedStep, IApElement? result)
    {
        var completionDateTime = DateTime.UtcNow;

        var stepCompletionMessage = new StepCompleted(
            PipelineId: processedStep.PipelineId,
            ProcedureIdentifier: processedStep.CurrentStep,
            CompletedAt: completionDateTime,
            Result: result,
            NextSteps: processedStep.NextSteps
        );
        await bus.PublishAsync(stepCompletionMessage);

        return Result.Success();
    }
}