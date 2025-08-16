using AiPipeline.Orchestration.Runner.Application.Pipeline.Services;
using AiPipeline.Orchestration.Runner.Domain.PipelineAggregate;
using AiPipeline.Orchestration.Shared.All.Contracts.Commands.RunPipelineStep;
using Microsoft.Extensions.Logging;
using TireOcr.Shared.Result;
using Wolverine;

namespace AiPipeline.Orchestration.Runner.Infrastructure.Pipeline.Services;

public class PipelineRabbitMqPublisherService : IPipelinePublisherService
{
    private readonly IMessageBus _messageBus;
    private readonly ILogger<PipelineRabbitMqPublisherService> _logger;

    public PipelineRabbitMqPublisherService(IMessageBus messageBus, ILogger<PipelineRabbitMqPublisherService> logger)
    {
        _messageBus = messageBus;
        _logger = logger;
    }

    public async Task<Result> PublishAsync(Domain.PipelineAggregate.Pipeline pipeline)
    {
        var commandResult = MapPipelineToRunPipelineStep(pipeline);
        if (commandResult.IsFailure)
            return Result.Failure(commandResult.Failures);

        var command = commandResult.Data!;
        try
        {
            await _messageBus.PublishAsync(command);
            return Result.Success();
        }
        catch (Exception ex)
        {
            var customMessage = $"Unexpected error while publishing pipeline '{pipeline.Id}'";
            _logger.LogError(ex, $"{customMessage}: {ex.Message}");
            return Result.Failure(new Failure(500, customMessage));
        }
    }

    public async Task<Result> PublishBatchAsync(PipelineBatch batch)
    {
        var commandResults = batch.Pipelines
            .Select(MapPipelineToRunPipelineStep)
            .ToList();

        var commandFailures = commandResults
            .SelectMany(result => result.Failures)
            .ToArray();
        if (commandFailures.Any())
            return Result.Failure(commandFailures);

        var commands = commandResults
            .Select(cr => cr.Data!)
            .ToList();

        var publishFutures = commands
            .Select(async c =>
            {
                try
                {
                    await _messageBus.PublishAsync(c);
                    return Result.Success();
                }
                catch (Exception ex)
                {
                    var customMessage = $"Failed to publish pipeline '{c.PipelineId}'";
                    _logger.LogError(ex, $"{customMessage}: {ex.Message}");
                    return Result.Failure(new Failure(500, c.PipelineId.ToString()));
                }
            })
            .ToList();

        var publishResults = await Task.WhenAll(publishFutures);
        var publishFailures = publishResults
            .SelectMany(result => result.Failures)
            .ToArray();

        if (publishFailures.Any())
        {
            var failedPipelinesIds = string.Join(',', publishFailures.Select(f => f.Message));
            var customMessage =
                $"Failed to publish all pipelines of batch '{batch.Id}'. Failed pipeline ids: {failedPipelinesIds}";
            _logger.LogError(customMessage);
            return Result.Failure(new Failure(500, customMessage));
        }

        return Result.Success();
    }

    private DataResult<RunPipelineStep> MapPipelineToRunPipelineStep(Domain.PipelineAggregate.Pipeline pipeline)
    {
        var firstStep = pipeline.Steps.FirstOrDefault();

        if (firstStep is null)
            return DataResult<RunPipelineStep>.Invalid("Pipeline must contain at least one step to be published.");
        var currentProcedureIdentifier = new ProcedureIdentifier(firstStep.NodeId, firstStep.NodeProcedureId);

        var nextSteps = pipeline.Steps.Skip(1).ToList();
        var nextProcedureIdentifiers = nextSteps
            .Select(s => new ProcedureIdentifier(s.NodeId, s.NodeProcedureId))
            .ToList();

        var fileReferences = pipeline.Files
            .Select(f => new FileReference(f.Id, f.StorageProvider, f.Path, f.ContentType))
            .ToList();

        var command = new RunPipelineStep(
            PipelineId: pipeline.Id,
            UserId: pipeline.UserId,
            CurrentStep: currentProcedureIdentifier,
            CurrentStepInput: pipeline.Input,
            NextSteps: nextProcedureIdentifiers,
            FileReferences: fileReferences
        );

        return DataResult<RunPipelineStep>.Success(command);
    }
}