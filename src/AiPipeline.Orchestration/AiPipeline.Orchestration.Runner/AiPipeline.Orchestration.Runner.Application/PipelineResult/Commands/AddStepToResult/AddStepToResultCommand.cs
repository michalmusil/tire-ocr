using AiPipeline.Orchestration.Runner.Application.PipelineResult.Dtos;
using TireOcr.Shared.UseCase;

namespace AiPipeline.Orchestration.Runner.Application.PipelineResult.Commands.AddStepToResult;

public record AddStepToResultCommand(Guid PipelineId, CreatePipelineStepDto Dto) : ICommand<GetPipelineResultDto>;