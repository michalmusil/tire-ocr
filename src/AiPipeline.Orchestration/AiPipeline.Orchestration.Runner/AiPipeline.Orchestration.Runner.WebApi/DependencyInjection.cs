using System.Text.Json.Serialization;
using AiPipeline.Orchestration.Shared;
using AiPipeline.Orchestration.Shared.Contracts.Commands.RunPipelineStep;
using AiPipeline.Orchestration.Shared.Contracts.Schema.Converters;
using Asp.Versioning;
using JasperFx.Resources;
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
                .AutoProvision()
                .DeclareExchange(MessagingConstants.AdvertisementsExchangeName, exc =>
                {
                    exc.ExchangeType = ExchangeType.Fanout;
                    exc.BindQueue(MessagingConstants.AdvertisementsQueueName);
                })
                .DeclareExchange(MessagingConstants.RunPipelineExchangeName, exc =>
                {
                    exc.ExchangeType = ExchangeType.Topic;

                    exc.BindTopic(
                            $"{MessagingConstants.RunPipelineExchangeName}.{MessagingConstants.TireOcrPreprocessingQueueName}")
                        .ToQueue(MessagingConstants.TireOcrPreprocessingQueueName);
                    exc.BindTopic(
                            $"{MessagingConstants.RunPipelineExchangeName}.{MessagingConstants.TireOcrOcrQueueName}")
                        .ToQueue(MessagingConstants.TireOcrOcrQueueName);
                    exc.BindTopic(
                            $"{MessagingConstants.RunPipelineExchangeName}.{MessagingConstants.TireOcrPostprocessingQueueName}")
                        .ToQueue(MessagingConstants.TireOcrPostprocessingQueueName);
                });

            opt.ListenToRabbitQueue(MessagingConstants.AdvertisementsQueueName);
            opt.PublishMessagesToRabbitMqExchange<RunPipelineStep>(MessagingConstants.RunPipelineExchangeName,
                src => $"{MessagingConstants.RunPipelineExchangeName}.{src.CurrentStep.NodeId}");

            opt.UseSystemTextJsonForSerialization(stj => { stj.Converters.Add(new ApElementConverter()); });
            opt.Services.AddResourceSetupOnStartup();
        });
    }
}