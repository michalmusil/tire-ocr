using TireOcr.Shared.UseCase;

namespace AiPipeline.Orchestration.Runner.Application.PipelineResultBatch.Commands.MarkPipelineResultBatchCompleted;

public record MarkPipelineResultBatchCompletedCommand(Guid BatchResultId, DateTime? CompletedAt): ICommand;