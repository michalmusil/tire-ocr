using AiPipeline.Orchestration.Runner.Application.Common.DataAccess;
using AiPipeline.Orchestration.Runner.Application.Pipeline.Dtos;
using TireOcr.Shared.Result;
using TireOcr.Shared.UseCase;

namespace AiPipeline.Orchestration.Runner.Application.PipelineResultBatch.Commands.InitPipelineResultBatch;

public class PipelineResultBatchCommandHandler : ICommandHandler<InitPipelineResultBatchCommand, GetPipelineBatchDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public PipelineResultBatchCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<DataResult<GetPipelineBatchDto>> Handle(
        InitPipelineResultBatchCommand request,
        CancellationToken cancellationToken
    )
    {
        var batch = request.Batch;
        var validationResult = batch.Validate();
        if (validationResult.IsFailure)
            return DataResult<GetPipelineBatchDto>.Failure(validationResult.Failures);

        var newBatchResult = new Domain.PipelineResultBatchAggregate.PipelineResultBatch(batch.UserId, batch.Id);
        var newPipelineResults = batch.Pipelines
            .Select(p =>
                new Domain.PipelineResultAggregate.PipelineResult(
                    pipelineId: p.Id,
                    batchId: batch.Id,
                    initialInput: p.Input,
                    userId: p.UserId
                )
            )
            .ToList();

        var pipelineResultValidationFailures = newPipelineResults
            .Select(pr => pr.Validate())
            .SelectMany(vr => vr.Failures)
            .ToArray();
        if (pipelineResultValidationFailures.Any())
            return DataResult<GetPipelineBatchDto>.Failure(pipelineResultValidationFailures);

        await _unitOfWork.PipelineResultBatchRepository.Add(newBatchResult);
        await _unitOfWork.PipelineResultRepository.AddRange(newPipelineResults);
        await _unitOfWork.SaveChangesAsync();

        var dto = GetPipelineBatchDto.FromDomain(batch);
        return DataResult<GetPipelineBatchDto>.Success(dto);
    }
}