using AiPipeline.Orchestration.Runner.Domain.NodeTypeEntity;
using TireOcr.Shared.Pagination;

namespace AiPipeline.Orchestration.Runner.Application.Repositories;

public interface INodeTypeRepository
{
    public Task<PaginatedCollection<NodeType>> GetMessagesPaginatedAsync(
        PaginationParams pagination
    );
    
    public Task Add(NodeType nodeType);
    public Task Remove(NodeType nodeType);
}