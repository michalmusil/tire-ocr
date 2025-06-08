using AiPipeline.Orchestration.Contracts.Events.NodeAdvertisement;
using AiPipeline.TireOcr.Ocr.Messaging.Constants;
using MassTransit;

namespace AiPipeline.TireOcr.Ocr.Messaging.Producers;

public class NodeAdvertisementProducer : BackgroundService
{
    private const int PeriodSeconds = 30;
    private readonly IBus _bus;

    public NodeAdvertisementProducer(IBus bus)
    {
        _bus = bus;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await _bus.Publish(new NodeAdvertised
            {
                NodeName = MessagingConstants.NodeName,
                Procedures = MessagingConstants.AvailableProcedures
            }, stoppingToken);

            await Task.Delay(PeriodSeconds * 1000, stoppingToken);
        }
    }
} 