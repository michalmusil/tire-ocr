using AiPipeline.Orchestration.Runner.Application.NodeType.Repositories;
using TireOcr.Shared.Pagination;

namespace AiPipeline.Orchestration.Runner.Infrastructure.NodeType.Repositories;

public class NodeTypeEntityRepositoryFake : INodeTypeEntityRepository
{
    private static readonly List<Domain.NodeTypeAggregate.NodeType> NodeTypes = [];

    public Task<PaginatedCollection<Domain.NodeTypeAggregate.NodeType>> GetNodeTypesPaginatedAsync(
        PaginationParams pagination)
    {
        return Task.FromResult(
            new PaginatedCollection<Domain.NodeTypeAggregate.NodeType>(
                NodeTypes,
                NodeTypes.Count,
                pagination.PageNumber,
                pagination.PageSize)
        );
    }

    public Task<IEnumerable<Domain.NodeTypeAggregate.NodeType>> GetNodeTypesByIdsAsync(params string[] ids)
    {
        return Task.FromResult(NodeTypes.Where(nt => ids.Contains(nt.Id)));
    }

    public Task<Domain.NodeTypeAggregate.NodeType?> GetNodeTypeByIdAsync(string id)
    {
        return Task.FromResult(NodeTypes.FirstOrDefault(nt => nt.Id == id));
    }

    public Task Add(Domain.NodeTypeAggregate.NodeType nodeType)
    {
        NodeTypes.Add(nodeType);
        return Task.CompletedTask;
    }

    public Task Put(Domain.NodeTypeAggregate.NodeType nodeType)
    {
        var alreadyExistingNodeType = NodeTypes.FirstOrDefault(x => x.Id == nodeType.Id);
        if (alreadyExistingNodeType is null)
        {
            NodeTypes.Add(nodeType);
            return Task.CompletedTask;
        }

        alreadyExistingNodeType.ClearProcedures();
        foreach (var nodeTypeAvailableProcedure in nodeType.AvailableProcedures)
        {
            alreadyExistingNodeType.AddProcedure(nodeTypeAvailableProcedure);
        }

        return Task.CompletedTask;
    }

    public Task Remove(Domain.NodeTypeAggregate.NodeType nodeType)
    {
        NodeTypes.Remove(nodeType);
        return Task.CompletedTask;
    }

    public Task<int> SaveChangesAsync()
    {
        return Task.FromResult(1);
    }
}