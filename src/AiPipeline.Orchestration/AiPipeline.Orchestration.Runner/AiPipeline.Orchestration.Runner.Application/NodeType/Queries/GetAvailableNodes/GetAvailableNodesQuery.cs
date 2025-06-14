using AiPipeline.Orchestration.Runner.Application.NodeType.Dtos;
using TireOcr.Shared.Pagination;
using TireOcr.Shared.UseCase;

namespace AiPipeline.Orchestration.Runner.Application.NodeType.Queries.GetAvailableNodes;

public record GetAvailableNodesQuery(PaginationParams Pagination) : IQuery<PaginatedCollection<NodeDto>>;