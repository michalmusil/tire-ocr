using AiPipeline.Orchestration.Shared.All.Contracts.Commands.RunPipelineStep;
using AiPipeline.Orchestration.Shared.All.Contracts.Events.PipelineCompletion;
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
            Result: result
        );
        await bus.PublishAsync(stepCompletionMessage);

        var nextStepProcedureIdentifier = processedStep.NextSteps.FirstOrDefault();
        var entirePipelineIsCompleted = nextStepProcedureIdentifier is null;

        if (entirePipelineIsCompleted)
        {
            var pipelineCompletionMessage = new PipelineCompleted(
                PipelineId: processedStep.PipelineId,
                CompletedAt: completionDateTime,
                FinalResult: result
            );
            await bus.PublishAsync(pipelineCompletionMessage);
        }

        return Result.Success();
    }
}