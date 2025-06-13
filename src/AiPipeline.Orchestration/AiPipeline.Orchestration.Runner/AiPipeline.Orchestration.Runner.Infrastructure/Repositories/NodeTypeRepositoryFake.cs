using AiPipeline.Orchestration.Contracts.Schema.Properties;
using AiPipeline.Orchestration.Runner.Application.Repositories;
using AiPipeline.Orchestration.Runner.Domain.NodeTypeAggregate;
using TireOcr.Shared.Pagination;

namespace AiPipeline.Orchestration.Runner.Infrastructure.Repositories;

public class NodeTypeRepositoryFake : INodeTypeRepository
{
    private static List<NodeType> _nodeTypes =
    [
        new NodeType("fake-node-type", [
                new NodeProcedure(
                    "fake-node-procedure",
                    1,
                    new ApBool(false),
                    new ApString(""))
            ]
        )
    ];

    public Task<PaginatedCollection<NodeType>> GetNodeTypesPaginatedAsync(PaginationParams pagination)
    {
        return Task.FromResult(
            new PaginatedCollection<NodeType>(
                _nodeTypes,
                _nodeTypes.Count,
                pagination.PageNumber,
                pagination.PageSize)
        );
    }

    public Task<IEnumerable<NodeType>> GetNodeTypesByIdsAsync(params string[] ids)
    {
        return Task.FromResult(_nodeTypes.AsEnumerable());
    }

    public Task<NodeType?> GetNodeTypeByIdAsync(string nodeId)
    {
        return Task.FromResult(_nodeTypes.FirstOrDefault());
    }

    public Task Add(NodeType nodeType)
    {
        _nodeTypes.Add(nodeType);
        return Task.CompletedTask;
    }

    public Task Remove(NodeType nodeType)
    {
        _nodeTypes.Remove(nodeType);
        return Task.CompletedTask;
    }
}