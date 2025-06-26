namespace AiPipeline.Orchestration.Runner.WebApi.Contracts.PipelineResults.GetAllResults;

public record GetAllResultsRequest(
    int PageNumber,
    int PageSize
);