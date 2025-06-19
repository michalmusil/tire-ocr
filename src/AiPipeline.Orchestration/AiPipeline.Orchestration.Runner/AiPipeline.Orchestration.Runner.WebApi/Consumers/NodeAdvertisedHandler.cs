using AiPipeline.Orchestration.Runner.Application.NodeType.Repositories;
using AiPipeline.Orchestration.Runner.Domain.NodeTypeAggregate;
using AiPipeline.Orchestration.Shared.Contracts.Events.NodeAdvertisement;

namespace AiPipeline.Orchestration.Runner.WebApi.Consumers;

public class NodeAdvertisedHandler
{
    private readonly INodeTypeRepository _nodeTypeRepository;
    private readonly ILogger<NodeAdvertisedHandler> _logger;

    public NodeAdvertisedHandler(INodeTypeRepository nodeTypeRepository, ILogger<NodeAdvertisedHandler> logger)
    {
        _nodeTypeRepository = nodeTypeRepository;
        _logger = logger;
    }


    public async Task HandleAsync(NodeAdvertised message)
    {
        var nodeType = new NodeType(
            id: message.NodeId,
            availableProcedures: message.Procedures
                .Select(pd => new NodeProcedure(pd.Id, pd.SchemaVersion, pd.Input, pd.Output))
        );
        await _nodeTypeRepository.Put(nodeType);
        _logger.LogInformation($"Message consumed: {@message}");
    }
}