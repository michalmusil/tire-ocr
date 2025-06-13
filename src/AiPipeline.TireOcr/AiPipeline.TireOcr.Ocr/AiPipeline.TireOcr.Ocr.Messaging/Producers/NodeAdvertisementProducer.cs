using AiPipeline.Orchestration.Contracts.Events.NodeAdvertisement;
using AiPipeline.TireOcr.Ocr.Messaging.Constants;
using Wolverine;

namespace AiPipeline.TireOcr.Ocr.Messaging.Producers;

public class NodeAdvertisementProducer : IHostedService
{
    private const int PeriodSeconds = 30;
    private readonly IServiceProvider _serviceProvider;


    public NodeAdvertisementProducer(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await using var scope = _serviceProvider.CreateAsyncScope();
        var bus = scope.ServiceProvider.GetRequiredService<IMessageBus>();

        while (!cancellationToken.IsCancellationRequested)
        {
            await bus.PublishAsync(new NodeAdvertised
            {
                NodeName = MessagingConstants.NodeQueueName,
                Procedures = MessagingConstants.AvailableProcedures
            });

            await Task.Delay(PeriodSeconds * 1000, cancellationToken);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}