using AiPipeline.Orchestration.Runner.Domain.NodeTypeAggregate;
using TireOcr.Shared.Pagination;

namespace AiPipeline.Orchestration.Runner.Application.Repositories;

public interface INodeTypeRepository
{
    public Task<PaginatedCollection<NodeType>> GetNodeTypesPaginatedAsync(
        PaginationParams pagination
    );

    public Task<IEnumerable<NodeType>> GetNodeTypesByIdsAsync(params string[] ids);

    public Task<NodeType?> GetNodeTypeById(string nodeId);

    public Task Add(NodeType nodeType);
    public Task Remove(NodeType nodeType);
}