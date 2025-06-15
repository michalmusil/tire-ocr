
using AiPipeline.Orchestration.Shared.Contracts.Events.NodeAdvertisement;

namespace AiPipeline.Orchestration.Runner.WebApi.Consumers;

public class NodeAdvertisedHandler
{
    private static readonly List<NodeAdvertised> _advertisements = [];
    private readonly ILogger<NodeAdvertisedHandler> _logger;

    public NodeAdvertisedHandler(ILogger<NodeAdvertisedHandler> logger)
    {
        _logger = logger;
    }


    public void Handle(NodeAdvertised message)
    {
        _advertisements.Add(message);
        _logger.LogInformation($"Message consumed: {@message}");
    }
}