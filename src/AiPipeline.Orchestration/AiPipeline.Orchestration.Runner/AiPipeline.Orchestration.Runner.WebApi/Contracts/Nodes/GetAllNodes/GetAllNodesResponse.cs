using AiPipeline.Orchestration.Runner.Application.Dtos.Node;
using TireOcr.Shared.Pagination;

namespace AiPipeline.Orchestration.Runner.WebApi.Contracts.Nodes.GetAllNodes;

public record GetAllNodesResponse(IEnumerable<NodeDto> Items, Pagination Pagination);