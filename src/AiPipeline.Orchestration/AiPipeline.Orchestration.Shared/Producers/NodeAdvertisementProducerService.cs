using AiPipeline.Orchestration.Shared.Contracts.Events.NodeAdvertisement;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Wolverine;

namespace AiPipeline.Orchestration.Shared.Producers;

public class NodeAdvertisementProducerService : IHostedService
{
    private readonly int _periodSeconds;
    private readonly IServiceProvider _serviceProvider;
    private readonly NodeAdvertised _advertisement;


    public NodeAdvertisementProducerService(IServiceProvider serviceProvider, NodeAdvertised advertisement,
        int periodSeconds = 30)
    {
        _serviceProvider = serviceProvider;
        _advertisement = advertisement;
        _periodSeconds = periodSeconds;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await using var scope = _serviceProvider.CreateAsyncScope();
        var bus = scope.ServiceProvider.GetRequiredService<IMessageBus>();

        while (!cancellationToken.IsCancellationRequested)
        {
            await bus.PublishAsync(_advertisement);

            await Task.Delay(_periodSeconds * 1000, cancellationToken);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}