namespace AiPipeline.Orchestration.Runner.WebApi.Contracts.Nodes.GetAllNodes;

public record GetAllNodesRequest(
    int PageNumber,
    int PageSize
);