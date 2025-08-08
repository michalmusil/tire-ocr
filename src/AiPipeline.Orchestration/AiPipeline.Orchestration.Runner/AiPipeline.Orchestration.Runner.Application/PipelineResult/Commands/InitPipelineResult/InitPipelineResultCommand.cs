using AiPipeline.Orchestration.Runner.Application.PipelineResult.Dtos;
using TireOcr.Shared.UseCase;

namespace AiPipeline.Orchestration.Runner.Application.PipelineResult.Commands.InitPipelineResult;

public record InitPipelineResultCommand(Guid PipelineId, Guid UserId) : ICommand<GetPipelineResultDto>;