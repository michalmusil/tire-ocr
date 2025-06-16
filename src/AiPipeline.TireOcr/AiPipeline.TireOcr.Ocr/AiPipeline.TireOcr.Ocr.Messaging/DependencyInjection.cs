using AiPipeline.Orchestration.Shared;
using AiPipeline.Orchestration.Shared.Contracts.Events.NodeAdvertisement;
using AiPipeline.Orchestration.Shared.Contracts.Schema.Converters;
using AiPipeline.Orchestration.Shared.Producers;
using AiPipeline.TireOcr.Ocr.Messaging.Constants;
using JasperFx.Resources;
using Wolverine;
using Wolverine.RabbitMQ;

namespace AiPipeline.TireOcr.Ocr.Messaging;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services, IHostBuilder hostBuilder)
    {
        AddMessaging(hostBuilder);
        return services;
    }

    private static void AddMessaging(IHostBuilder hostBuilder)
    {
        hostBuilder.UseWolverine(opt =>
        {
            opt.UseRabbitMqUsingNamedConnection("rabbitmq")
                .AutoProvision();

            opt.PublishMessage<NodeAdvertised>()
                .ToRabbitExchange(MessagingConstants.AdvertisementsExchangeName);

            opt.ListenToRabbitQueue(MessagingConstants.TireOcrOcrQueueName);

            opt.UseSystemTextJsonForSerialization(stj => { stj.Converters.Add(new ApElementConverter()); });
            opt.Services.AddResourceSetupOnStartup();
            opt.Services.AddHostedService(provider =>
                new NodeAdvertisementProducerService(provider, NodeMessagingConstants.NodeAdvertisement));
        });
    }
}