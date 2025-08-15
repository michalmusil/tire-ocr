using AiPipeline.Orchestration.Runner.Application.Pipeline.Services;
using AiPipeline.Orchestration.Shared.All.Contracts.Commands.RunPipelineStep;
using AiPipeline.Orchestration.Shared.All.Contracts.Schema;
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

    public async Task<Result> PublishAsync(Domain.PipelineAggregate.Pipeline pipeline, IApElement input)
    {
        var firstStep = pipeline.Steps.FirstOrDefault();

        if (firstStep is null)
            return Result.Invalid("Pipeline must contain at least one step to be published.");
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
            CurrentStepInput: input,
            NextSteps: nextProcedureIdentifiers,
            FileReferences: fileReferences
        );

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
}