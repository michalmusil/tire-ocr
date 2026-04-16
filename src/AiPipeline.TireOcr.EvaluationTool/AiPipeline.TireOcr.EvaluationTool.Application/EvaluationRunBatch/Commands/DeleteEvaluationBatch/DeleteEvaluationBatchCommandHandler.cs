using AiPipeline.TireOcr.EvaluationTool.Application.Common.DataAccess;
using AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRun.Dtos;
using AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRun.Facades;
using AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRunBatch.Dtos;
using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate;
using TireOcr.Shared.Result;
using TireOcr.Shared.UseCase;

namespace AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRunBatch.Commands.DeleteEvaluationBatch;

public class DeleteEvaluationBatchCommandHandler : ICommandHandler<DeleteEvaluationBatchCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteEvaluationBatchCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteEvaluationBatchCommand request,
        CancellationToken cancellationToken)
    {
        var foundBatch = await _unitOfWork.EvaluationRunBatchRepository.GetEvaluationRunBatchByIdAsync(request.BatchId);
        if (foundBatch is null)
            return Result.NotFound($"Batch with id {request.BatchId} does not exist.");

        await _unitOfWork.EvaluationRunBatchRepository.Remove(foundBatch);
        await _unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}