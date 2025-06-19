using AiPipeline.Orchestration.Shared.Constants;
using AiPipeline.Orchestration.Shared.Contracts.Commands.RunPipelineStep;
using AiPipeline.Orchestration.Shared.Contracts.Events.NodeAdvertisement;
using AiPipeline.Orchestration.Shared.Contracts.Schema.Converters;
using JasperFx.Resources;
using Wolverine;
using Wolverine.RabbitMQ;
using Wolverine.RabbitMQ.Internal;

namespace AiPipeline.Orchestration.Shared.Extensions;

public static class WolverineExtension
{
    public static RabbitMqTransportExpression DeclareExchanges(this RabbitMqTransportExpression expression)
    {
        return expression
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
    }

    public static WolverineOptions ConfigureMessagePublishing(this WolverineOptions options)
    {
        options.PublishMessagesToRabbitMqExchange<RunPipelineStep>(MessagingConstants.RunPipelineExchangeName,
            src => $"{MessagingConstants.RunPipelineExchangeName}.{src.CurrentStep.NodeId}");
        options.PublishMessage<NodeAdvertised>()
            .ToRabbitExchange(MessagingConstants.AdvertisementsExchangeName);

        return options;
    }

    public static WolverineOptions ApplyCustomConfiguration(this WolverineOptions options)
    {
        options.UseSystemTextJsonForSerialization(stj => { stj.Converters.Add(new ApElementConverter()); });
        options.Services.AddResourceSetupOnStartup();
        return options;
    }
}