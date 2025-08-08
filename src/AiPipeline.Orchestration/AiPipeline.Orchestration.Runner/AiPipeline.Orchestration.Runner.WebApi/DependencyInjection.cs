using System.Text.Json.Serialization;
using AiPipeline.Orchestration.Shared.All.Constants;
using AiPipeline.Orchestration.Shared.All.Contracts.Schema.Converters;
using AiPipeline.Orchestration.Shared.All.Extensions;
using Asp.Versioning;
using Wolverine;
using Wolverine.RabbitMQ;

namespace AiPipeline.Orchestration.Runner.WebApi;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services, IHostBuilder hostBuilder)
    {
        AddControllersConfig(services);
        AddRoutesConfig(services);
        AddSwagger(services);
        AddMessaging(hostBuilder);
        return services;
    }

    private static void AddControllersConfig(IServiceCollection serviceCollection)
    {
        serviceCollection
            .AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters
                    .Add(new JsonStringEnumConverter());
                options.JsonSerializerOptions.Converters.Add(new ApElementConverter());
            });
    }

    private static void AddRoutesConfig(IServiceCollection serviceCollection)
    {
        serviceCollection.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.ApiVersionReader = new UrlSegmentApiVersionReader();
        }).AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'V";
            options.SubstituteApiVersionInUrl = true;
        });
    }

    private static void AddSwagger(IServiceCollection serviceCollection)
    {
        serviceCollection.AddEndpointsApiExplorer();
        serviceCollection.AddSwaggerGen();
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

            opt.ListenToRabbitQueue(MessagingConstants.AdvertisementsQueueName);
            opt.ListenToRabbitQueue(MessagingConstants.CompletedPipelineStepsQueueName);
            opt.ListenToRabbitQueue(MessagingConstants.FailedPipelinesQueueName);
        });
    }
}