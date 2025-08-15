using AiPipeline.Orchestration.Runner.Application.PipelineResult.Dtos;
using AiPipeline.Orchestration.Shared.All.Contracts.Schema;
using TireOcr.Shared.UseCase;

namespace AiPipeline.Orchestration.Runner.Application.PipelineResult.Commands.InitPipelineResult;

public record InitPipelineResultCommand(Guid PipelineId, Guid UserId, IApElement Input)
    : ICommand<GetPipelineResultDto>;