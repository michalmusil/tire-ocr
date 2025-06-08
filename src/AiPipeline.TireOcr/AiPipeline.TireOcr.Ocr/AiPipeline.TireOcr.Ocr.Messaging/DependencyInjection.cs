using AiPipeline.TireOcr.Ocr.Messaging.Producers;
using MassTransit;

namespace AiPipeline.TireOcr.Ocr.Messaging;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        AddMessaging(services);
        AddBackgroundServices(services);
        return services;
    }

    private static void AddMessaging(IServiceCollection serviceCollection)
    {
        serviceCollection.AddMassTransit(busConfigurator =>
        {
            busConfigurator.SetKebabCaseEndpointNameFormatter();

            busConfigurator.UsingRabbitMq((busContext, busFactoryConfigurator) =>
            {
                var configuration = busContext.GetRequiredService<IConfiguration>();
                var host = configuration.GetConnectionString("rabbitmq");
                busFactoryConfigurator.Host(host);
                busFactoryConfigurator.ConfigureEndpoints(busContext);
            });
        });
    }

    private static void AddBackgroundServices(IServiceCollection serviceCollection)
    {
        serviceCollection.AddHostedService<NodeAdvertisementProducer>();
    }
}