using AiPipeline.Orchestration.Shared.Contracts.Commands.RunPipelineStep;
using AiPipeline.Orchestration.Shared.Contracts.Events.PipelineFailure;
using TireOcr.Shared.Result;
using Wolverine;

namespace AiPipeline.Orchestration.Shared.Procedures.PipelineFailure;

public class DefaultPipelineFailureStrategy : IPipelineFailureStrategy
{
    public async Task<Result> Execute(IMessageBus bus, RunPipelineStep failedStep, Failure failure,
        Exception? exception)
    {
        var failureMessage = new PipelineFailed(
            PipelineId: failedStep.PipelineId,
            ProcedureIdentifier: failedStep.CurrentStep,
            FailedAt: DateTime.UtcNow,
            FailureCode: failure.Code,
            FailureReason: failure.Message,
            ExceptionMessage: exception is null
                ? null
                : $"{exception.Message} | Stack trace: {exception.StackTrace ?? "N/A"}"
        );

        await bus.PublishAsync(failureMessage);
        return Result.Success();
    }
}