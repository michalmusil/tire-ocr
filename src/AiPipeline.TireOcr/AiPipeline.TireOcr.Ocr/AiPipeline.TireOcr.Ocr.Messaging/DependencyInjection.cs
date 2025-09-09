using AiPipeline.Orchestration.Shared.All.Constants;
using AiPipeline.Orchestration.Shared.All.Extensions;
using AiPipeline.Orchestration.Shared.Nodes.Extensions;
using AiPipeline.Orchestration.Shared.Nodes.Producers;
using Wolverine;
using Wolverine.RabbitMQ;

namespace AiPipeline.TireOcr.Ocr.Messaging;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services, IHostBuilder hostBuilder,
        IConfiguration configuration)
    {
        AddMessaging(hostBuilder);
        services.AddFileManipulation(configuration);
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

            opt.ListenToRabbitQueue(MessagingConstants.TireOcrOcrId);

            opt.Services.AddHostedService(provider =>
                new NodeAdvertisementProducerService(
                    serviceProvider: provider,
                    nodeId: MessagingConstants.TireOcrOcrId,
                    assemblies: typeof(DependencyInjection).Assembly
                )
            );
        });
    }
}