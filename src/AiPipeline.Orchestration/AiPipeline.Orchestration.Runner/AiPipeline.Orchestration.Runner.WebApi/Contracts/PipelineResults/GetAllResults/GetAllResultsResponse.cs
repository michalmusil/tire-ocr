using AiPipeline.Orchestration.Runner.Application.PipelineResult.Dtos;
using TireOcr.Shared.Pagination;

namespace AiPipeline.Orchestration.Runner.WebApi.Contracts.PipelineResults.GetAllResults;

public record GetAllResultsResponse(IEnumerable<GetPipelineResultDto> Items, Pagination Pagination);