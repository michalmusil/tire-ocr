using AiPipeline.Orchestration.Runner.Application.PipelineResult.Dtos;
using TireOcr.Shared.UseCase;

namespace AiPipeline.Orchestration.Runner.Application.PipelineResult.Commands.MarkPipelineCompleted;

public record MarkPipelineCompletedCommand(Guid PipelineId, DateTime? CompletedAt) : ICommand<GetPipelineResultDto>;