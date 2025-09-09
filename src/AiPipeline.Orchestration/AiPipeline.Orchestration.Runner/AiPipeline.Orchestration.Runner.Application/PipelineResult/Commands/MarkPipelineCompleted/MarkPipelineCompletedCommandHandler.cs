using AiPipeline.Orchestration.Runner.Application.Common.DataAccess;
using AiPipeline.Orchestration.Runner.Application.Pipeline.Services;
using AiPipeline.Orchestration.Runner.Application.PipelineResult.Dtos;
using AiPipeline.Orchestration.Runner.Application.PipelineResultBatch.Commands.MarkPipelineResultBatchCompleted;
using MediatR;
using Microsoft.Extensions.Logging;
using TireOcr.Shared.Result;
using TireOcr.Shared.UseCase;

namespace AiPipeline.Orchestration.Runner.Application.PipelineResult.Commands.MarkPipelineCompleted;

public class MarkPipelineCompletedCommandHandler : ICommandHandler<MarkPipelineCompletedCommand, GetPipelineResultDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _mediator;
    private readonly IPipelineResultSubscriberService _pipelineResultSubscriberService;
    private readonly ILogger<MarkPipelineCompletedCommandHandler> _logger;

    public MarkPipelineCompletedCommandHandler(IUnitOfWork unitOfWork, IMediator mediator,
        IPipelineResultSubscriberService pipelineResultSubscriberService,
        ILogger<MarkPipelineCompletedCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mediator = mediator;
        _pipelineResultSubscriberService = pipelineResultSubscriberService;
        _logger = logger;
    }

    public async Task<DataResult<GetPipelineResultDto>> Handle(
        MarkPipelineCompletedCommand request,
        CancellationToken cancellationToken
    )
    {
        var existingResult = await _unitOfWork
            .PipelineResultRepository
            .GetPipelineResultByPipelineIdAsync(request.PipelineId);
        if (existingResult is null)
            return DataResult<GetPipelineResultDto>.NotFound($"Result for pipeline {request.PipelineId} not found");

        var completedAt = request.CompletedAt ?? DateTime.UtcNow;
        existingResult.MarkAsFinished(completedAt);

        var validationResult = existingResult.Validate();
        if (validationResult.IsFailure)
            return DataResult<GetPipelineResultDto>.Failure(validationResult.Failures);

        await _unitOfWork.SaveChangesAsync();
        await _pipelineResultSubscriberService.CompleteWithPipelineResultAsync(existingResult);
        
        await EnsurePipelineBatchCompletesAsync(existingResult, completedAt);

        var dto = GetPipelineResultDto.FromDomain(existingResult);
        return DataResult<GetPipelineResultDto>.Success(dto);
    }

    private async Task EnsurePipelineBatchCompletesAsync(
        Domain.PipelineResultAggregate.PipelineResult pipelineResult,
        DateTime completedAt
    )
    {
        if (pipelineResult.BatchId is null)
            return;

        Guid batchId = pipelineResult.BatchId.Value;
        var allPipelineResultsOfBatch = (await _unitOfWork.PipelineResultRepository
                .GetPipelineResultsByBatchIdAsync(batchId))
            .ToList();

        var allResultsCount = allPipelineResultsOfBatch.Count;
        var completedResultsCount = allPipelineResultsOfBatch
            .Count(pr => pr.Succeeded || pr.Failed);

        var batchIsCompleted = allResultsCount == completedResultsCount;
        if (!batchIsCompleted)
            return;

        var markBatchCompletedCommand = new MarkPipelineResultBatchCompletedCommand(batchId, completedAt);
        var batchCompletionResult = await _mediator.Send(markBatchCompletedCommand);
        if (batchCompletionResult.IsFailure)
            _logger.LogCritical(
                $"Failed to mark pipeline batch '{batchId}' as completed after last pipeline '{pipelineResult.Id}' successfully finished"
            );
    }
}