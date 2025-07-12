using TireOcr.Shared.Pagination;
using TireOcr.Shared.Persistence;

namespace AiPipeline.Orchestration.Runner.Application.NodeType.Repositories;

public interface INodeTypeRepository: IRepository
{
    public Task<PaginatedCollection<Domain.NodeTypeAggregate.NodeType>> GetNodeTypesPaginatedAsync(
        PaginationParams pagination
    );

    public Task<IEnumerable<Domain.NodeTypeAggregate.NodeType>> GetNodeTypesByIdsAsync(params string[] ids);

    public Task<Domain.NodeTypeAggregate.NodeType?> GetNodeTypeByIdAsync(string id);

    public Task Add(Domain.NodeTypeAggregate.NodeType nodeType);
    public Task Put(Domain.NodeTypeAggregate.NodeType nodeType);
    public Task Remove(Domain.NodeTypeAggregate.NodeType nodeType);
}