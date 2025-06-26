using AiPipeline.Orchestration.Shared.Constants;
using AiPipeline.Orchestration.Shared.Contracts.Commands.RunPipelineStep;
using AiPipeline.Orchestration.Shared.Contracts.Events.NodeAdvertisement;
using AiPipeline.Orchestration.Shared.Contracts.Events.PipelineCompletion;
using AiPipeline.Orchestration.Shared.Contracts.Events.PipelineFailure;
using AiPipeline.Orchestration.Shared.Contracts.Events.StepCompletion;
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
            .DeclareExchange(MessagingConstants.CompletedPipelineStepsExchangeName, exc =>
            {
                exc.ExchangeType = ExchangeType.Fanout;
                exc.BindQueue(MessagingConstants.CompletedPipelineStepsQueueName);
            })
            .DeclareExchange(MessagingConstants.CompletedPipelinesExchangeName, exc =>
            {
                exc.ExchangeType = ExchangeType.Fanout;
                exc.BindQueue(MessagingConstants.CompletedPipelinesQueueName);
            })
            .DeclareExchange(MessagingConstants.FailedPipelinesExchangeName, exc =>
            {
                exc.ExchangeType = ExchangeType.Fanout;
                exc.BindQueue(MessagingConstants.FailedPipelinesQueueName);
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

        options.PublishMessage<StepCompleted>()
            .ToRabbitExchange(MessagingConstants.CompletedPipelineStepsExchangeName);

        options.PublishMessage<PipelineCompleted>()
            .ToRabbitExchange(MessagingConstants.CompletedPipelinesExchangeName);

        options.PublishMessage<PipelineFailed>()
            .ToRabbitExchange(MessagingConstants.FailedPipelinesExchangeName);

        return options;
    }

    public static WolverineOptions ApplyCustomConfiguration(this WolverineOptions options)
    {
        options.UseSystemTextJsonForSerialization(stj => { stj.Converters.Add(new ApElementConverter()); });
        options.Services.AddResourceSetupOnStartup();
        return options;
    }
}