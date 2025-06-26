using AiPipeline.Orchestration.Runner.Application.PipelineResult.Dtos;
using TireOcr.Shared.UseCase;

namespace AiPipeline.Orchestration.Runner.Application.PipelineResult.Queries.GetResultOfPipeline;

public record GetResultOfPipelineQuery(Guid PipelineId) : IQuery<GetPipelineResultDto>;