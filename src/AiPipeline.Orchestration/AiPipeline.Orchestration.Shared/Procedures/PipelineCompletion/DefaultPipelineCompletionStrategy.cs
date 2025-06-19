using AiPipeline.Orchestration.Shared.Contracts.Commands.RunPipelineStep;
using AiPipeline.Orchestration.Shared.Contracts.Events.PipelineCompletion;
using AiPipeline.Orchestration.Shared.Contracts.Schema;
using TireOcr.Shared.Result;
using Wolverine;

namespace AiPipeline.Orchestration.Shared.Procedures.PipelineCompletion;

public class DefaultPipelineCompletionStrategy : IPipelineCompletionStrategy
{
    public async Task<Result> Execute(IMessageBus bus, RunPipelineStep processedStep, IApElement? result)
    {
        var completionMessage = new PipelineCompleted(
            PipelineId: processedStep.PipelineId,
            CompletedAt: DateTime.UtcNow,
            FinalResult: result
        );
        await bus.PublishAsync(completionMessage);
        return Result.Success();
    }
}