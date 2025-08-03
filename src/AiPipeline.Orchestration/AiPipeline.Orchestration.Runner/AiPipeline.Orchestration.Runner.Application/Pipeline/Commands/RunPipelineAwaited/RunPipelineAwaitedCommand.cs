using AiPipeline.Orchestration.Runner.Application.Pipeline.Dtos.Run;
using AiPipeline.Orchestration.Runner.Application.PipelineResult.Dtos;
using TireOcr.Shared.UseCase;

namespace AiPipeline.Orchestration.Runner.Application.Pipeline.Commands.RunPipelineAwaited;

public record RunPipelineAwaitedCommand(RunPipelineDto Dto, TimeSpan Timeout)
    : ICommand<GetPipelineResultDto>;