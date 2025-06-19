using AiPipeline.Orchestration.Shared.Constants;
using AiPipeline.Orchestration.Shared.Extensions;
using AiPipeline.Orchestration.Shared.Producers;
using AiPipeline.TireOcr.Ocr.Messaging.Constants;
using Wolverine;
using Wolverine.RabbitMQ;

namespace AiPipeline.TireOcr.Ocr.Messaging;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services, IHostBuilder hostBuilder)
    {
        AddMessaging(hostBuilder);
        services.AddProcedureRoutingFromAssemblies(typeof(DependencyInjection).Assembly);
        return services;
    }

    private static void AddMessaging(IHostBuilder hostBuilder)
    {
        hostBuilder.UseWolverine(opt =>
        {
            opt.UseRabbitMqUsingNamedConnection("rabbitmq")
                .DeclareExchanges()
                .AutoProvision();

            opt.ConfigureMessagePublishing();
            opt.ApplyCustomConfiguration();

            opt.ListenToRabbitQueue(MessagingConstants.TireOcrOcrQueueName);

            opt.Services.AddHostedService(provider =>
                new NodeAdvertisementProducerService(provider, NodeMessagingConstants.NodeAdvertisement));
        });
    }
}