using AiPipeline.Orchestration.Shared.All.Constants;
using AiPipeline.Orchestration.Shared.All.Contracts.Commands.RunPipelineStep;
using AiPipeline.Orchestration.Shared.All.Contracts.Events.NodeAdvertisement;
using AiPipeline.Orchestration.Shared.All.Contracts.Events.PipelineFailure;
using AiPipeline.Orchestration.Shared.All.Contracts.Events.StepCompletion;
using AiPipeline.Orchestration.Shared.All.Contracts.Schema.Converters;
using JasperFx.Resources;
using Wolverine;
using Wolverine.RabbitMQ;
using Wolverine.RabbitMQ.Internal;

namespace AiPipeline.Orchestration.Runner.WebApi.Extensions;

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
            .DeclareExchange(MessagingConstants.FailedPipelinesExchangeName, exc =>
            {
                exc.ExchangeType = ExchangeType.Fanout;
                exc.BindQueue(MessagingConstants.FailedPipelinesQueueName);
            })
            .DeclareExchange(MessagingConstants.RunPipelineExchangeName, exc =>
            {
                exc.ExchangeType = ExchangeType.Topic;
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