using AiPipeline.Orchestration.Shared;
using AiPipeline.Orchestration.Shared.Contracts.Events.NodeAdvertisement;
using AiPipeline.Orchestration.Shared.Contracts.Schema.Converters;
using AiPipeline.TireOcr.Ocr.Messaging.Producers;
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
                .AutoProvision()
                .DeclareExchange(MessagingConstants.AdvertisementsExchangeName, exc =>
                {
                    exc.ExchangeType = ExchangeType.Fanout;
                    exc.BindQueue(MessagingConstants.AdvertisementsQueueName);
                });

            opt.PublishMessage<NodeAdvertised>()
                .ToRabbitExchange(MessagingConstants.AdvertisementsExchangeName);

            opt.ListenToRabbitQueue(MessagingConstants.TireOcrOcrQueueName);

            opt.UseSystemTextJsonForSerialization(stj => { stj.Converters.Add(new ApElementConverter()); });
            opt.Services.AddResourceSetupOnStartup();
            opt.Services.AddHostedService<NodeAdvertisementProducer>();
        });
    }
}