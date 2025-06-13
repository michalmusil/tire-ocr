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
                    "{\"Type\":\"ApObject\",\"Properties\":{\"FullName\":{\"Type\":\"ApString\",\"Value\":\"John Doe\"},\"Age\":{\"Type\":\"ApInt\",\"Value\":30},\"CreditCard\":{\"Type\":\"ApObject\",\"Properties\":{\"Number\":{\"Type\":\"ApString\",\"Value\":\"12344567890/1111\"},\"OwnerName\":{\"Type\":\"ApString\",\"Value\":\"John Doe\"},\"ActiveAt\":{\"Type\":\"ApList\",\"Items\":[{\"Type\":\"ApDateTime\",\"Value\":\"2025-06-13T15:13:12.880364Z\"},{\"Type\":\"ApDateTime\",\"Value\":\"2025-06-13T15:13:12.880428Z\"}]}},\"NonRequiredProperties\":[]}},\"NonRequiredProperties\":[\"CreditCard\"]}",
                    "{\"Type\":\"ApObject\",\"Properties\":{\"FullName\":{\"Type\":\"ApString\",\"Value\":\"John Doe\"},\"Age\":{\"Type\":\"ApInt\",\"Value\":30},\"CreditCard\":{\"Type\":\"ApObject\",\"Properties\":{\"Number\":{\"Type\":\"ApString\",\"Value\":\"12344567890/1111\"},\"OwnerName\":{\"Type\":\"ApString\",\"Value\":\"John Doe\"},\"ActiveAt\":{\"Type\":\"ApList\",\"Items\":[{\"Type\":\"ApDateTime\",\"Value\":\"2025-06-13T15:13:12.880364Z\"},{\"Type\":\"ApDateTime\",\"Value\":\"2025-06-13T15:13:12.880428Z\"}]}},\"NonRequiredProperties\":[]}},\"NonRequiredProperties\":[\"CreditCard\"]}")
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

    public Task<NodeType?> GetNodeTypeById(string nodeId)
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