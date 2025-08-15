using AiPipeline.Orchestration.Runner.Application.Pipeline.Dtos;
using AiPipeline.Orchestration.Runner.Application.Pipeline.Dtos.Run;
using TireOcr.Shared.UseCase;

namespace AiPipeline.Orchestration.Runner.Application.Pipeline.Commands.RunPipelineBatch;

public record RunPipelineBatchCommand(RunPipelineBatchDto RunPipelineBatchDto) : ICommand<GetPipelineBatchDto>;