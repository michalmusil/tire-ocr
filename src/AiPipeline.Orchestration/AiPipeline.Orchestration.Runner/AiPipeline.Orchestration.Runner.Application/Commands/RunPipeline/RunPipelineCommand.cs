using AiPipeline.Orchestration.Runner.Application.Dtos.Pipeline;
using AiPipeline.Orchestration.Runner.Application.Dtos.Pipeline.Run;
using TireOcr.Shared.UseCase;

namespace AiPipeline.Orchestration.Runner.Application.Commands.RunPipeline;

public record RunPipelineCommand(RunPipelineDto Dto) : ICommand<PipelineDto>;