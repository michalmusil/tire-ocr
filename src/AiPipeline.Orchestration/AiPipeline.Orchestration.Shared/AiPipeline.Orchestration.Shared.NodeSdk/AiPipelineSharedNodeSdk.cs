using System.Reflection;
using AiPipeline.Orchestration.Shared.All.Constants;
using AiPipeline.Orchestration.Shared.NodeSdk.Extensions;
using AiPipeline.Orchestration.Shared.NodeSdk.Producers;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Wolverine;
using Wolverine.RabbitMQ;

namespace AiPipeline.Orchestration.Shared.NodeSdk;

public static class AiPipelineSharedNodeSdk
{
    public static async Task<WebApplication> CreateNodeApplication(
        string nodeId,
        Func<WebApplicationBuilder, string> provideRabbitMqConnectionString,
        Func<WebApplicationBuilder, Uri>? provideGrpcServerUri = null,
        Func<WebApplicationBuilder, Task>? configureBuilder = null,
        params Assembly[] assemblies
    )
    {
        var builder = WebApplication.CreateBuilder();

        builder.UseWolverine(opt =>
        {
            opt.ConfigureMessagePublishing();
            opt.ApplyCustomConfiguration();
            opt.ListenToRabbitQueue(nodeId);

            var rabbitMqConnectionString = provideRabbitMqConnectionString(builder);
            opt.UseRabbitMq(rabbitMqConnectionString)
                .AutoProvision()
                .DeclareExchange(MessagingConstants.RunPipelineExchangeName, exc =>
                {
                    exc.ExchangeType = ExchangeType.Topic;
                    exc.BindTopic(
                            $"{MessagingConstants.RunPipelineExchangeName}.{nodeId}")
                        .ToQueue(nodeId);
                });

            opt.Services.AddHostedService(provider =>
                new NodeAdvertisementProducerService(
                    serviceProvider: provider,
                    nodeId: nodeId,
                    assemblies: assemblies
                )
            );
        });

        builder.Services.AddProcedureRoutingFromAssemblies(assemblies);
        
        if (provideGrpcServerUri is not null)
            builder.Services.AddFileManipulation(provideGrpcServerUri(builder));

        if (configureBuilder is not null)
            await configureBuilder(builder);

        return builder.Build();
    }
}