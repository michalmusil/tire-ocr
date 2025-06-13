using AiPipeline.Orchestration.Runner.Application.Dtos.Node;
using TireOcr.Shared.Pagination;
using TireOcr.Shared.UseCase;

namespace AiPipeline.Orchestration.Runner.Application.Queries.GetAvailableNodes;

public record GetAvailableNodesQuery(PaginationParams Pagination) : IQuery<PaginatedCollection<NodeDto>>;