using AiPipeline.Orchestration.Runner.Application.PipelineResult.Dtos;

namespace AiPipeline.Orchestration.Runner.WebApi.Contracts.PipelineResults.GetResultOfPipeline;

public record GetResultOfPipelineResponse(
    GetPipelineResultDto Result
);