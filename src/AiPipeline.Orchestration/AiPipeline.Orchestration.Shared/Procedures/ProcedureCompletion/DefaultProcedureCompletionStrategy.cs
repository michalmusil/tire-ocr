using AiPipeline.Orchestration.Shared.Contracts.Commands.RunPipelineStep;
using AiPipeline.Orchestration.Shared.Contracts.Events.PipelineCompletion;
using AiPipeline.Orchestration.Shared.Contracts.Events.StepCompletion;
using AiPipeline.Orchestration.Shared.Contracts.Schema;
using TireOcr.Shared.Result;
using Wolverine;

namespace AiPipeline.Orchestration.Shared.Procedures.ProcedureCompletion;

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