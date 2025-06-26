using AiPipeline.Orchestration.Runner.Application.NodeType.Repositories;
using AiPipeline.Orchestration.Runner.Domain.NodeTypeAggregate;
using AiPipeline.Orchestration.Shared.Contracts.Schema.Properties;
using TireOcr.Shared.Pagination;

namespace AiPipeline.Orchestration.Runner.Infrastructure.NodeType.Repositories;

public class NodeTypeRepositoryFake : INodeTypeRepository
{
    private static List<Domain.NodeTypeAggregate.NodeType> _nodeTypes = [];

    public Task<PaginatedCollection<Domain.NodeTypeAggregate.NodeType>> GetNodeTypesPaginatedAsync(
        PaginationParams pagination)
    {
        return Task.FromResult(
            new PaginatedCollection<Domain.NodeTypeAggregate.NodeType>(
                _nodeTypes,
                _nodeTypes.Count,
                pagination.PageNumber,
                pagination.PageSize)
        );
    }

    public Task<IEnumerable<Domain.NodeTypeAggregate.NodeType>> GetNodeTypesByIdsAsync(params string[] ids)
    {
        return Task.FromResult(_nodeTypes.AsEnumerable());
    }

    public Task<Domain.NodeTypeAggregate.NodeType?> GetNodeTypeByIdAsync(string nodeId)
    {
        return Task.FromResult(_nodeTypes.FirstOrDefault());
    }

    public Task Add(Domain.NodeTypeAggregate.NodeType nodeType)
    {
        _nodeTypes.Add(nodeType);
        return Task.CompletedTask;
    }

    public Task Put(Domain.NodeTypeAggregate.NodeType nodeType)
    {
        var alreadyExistingNodeType = _nodeTypes.FirstOrDefault(x => x.Id == nodeType.Id);
        if (alreadyExistingNodeType is null)
        {
            _nodeTypes.Add(nodeType);
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
        _nodeTypes.Remove(nodeType);
        return Task.CompletedTask;
    }
}