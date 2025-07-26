using AiPipeline.Orchestration.Shared.All.Constants;
using AiPipeline.Orchestration.Shared.All.Extensions;
using AiPipeline.Orchestration.Shared.Nodes.Extensions;
using AiPipeline.Orchestration.Shared.Nodes.Producers;
using Wolverine;
using Wolverine.RabbitMQ;

namespace AiPipeline.TireOcr.Postprocessing.Messaging;

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

            opt.ListenToRabbitQueue(MessagingConstants.TireOcrPostprocessingId);

            opt.Services.AddHostedService(provider =>
                new NodeAdvertisementProducerService(
                    serviceProvider: provider,
                    nodeId: MessagingConstants.TireOcrPostprocessingId,
                    assemblies: typeof(DependencyInjection).Assembly
                )
            );
        });
    }
}