using AiPipeline.Orchestration.Contracts.Events.NodeAdvertisement;
using AiPipeline.Orchestration.Contracts.Schema.Converters;
using AiPipeline.TireOcr.Ocr.Messaging.Constants;
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
            opt.ListenToRabbitQueue(MessagingConstants.NodeQueueName);

            opt.PublishMessage<NodeAdvertised>()
                .ToRabbitExchange(MessagingConstants.AdvertisementsExchangeName, exc =>
                {
                    exc.ExchangeType = ExchangeType.Fanout;
                    exc.BindQueue(MessagingConstants.AdvertisementsQueueName);
                });

            opt.UseRabbitMqUsingNamedConnection("rabbitmq").AutoProvision();

            opt.UseSystemTextJsonForSerialization(stj => { stj.Converters.Add(new ApElementConverter()); });

            opt.Services.AddResourceSetupOnStartup();
            opt.Services.AddHostedService<NodeAdvertisementProducer>();
        });
    }
}