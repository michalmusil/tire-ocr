using AiPipeline.Orchestration.Runner.Application.Common.DataAccess;
using TireOcr.Shared.Result;
using TireOcr.Shared.UseCase;

namespace AiPipeline.Orchestration.Runner.Application.PipelineResultBatch.Commands.MarkPipelineResultBatchCompleted;

public class MarkPipelineResultBatchCompletedCommandHandler : ICommandHandler<MarkPipelineResultBatchCompletedCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public MarkPipelineResultBatchCompletedCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(MarkPipelineResultBatchCompletedCommand request,
        CancellationToken cancellationToken)
    {
        var foundResultBatch =
            await _unitOfWork.PipelineResultBatchRepository.GetResultBatchByIdAsync(request.BatchResultId);
        if (foundResultBatch is null)
            return Result.NotFound($"Pipeline result batch with id '{request.BatchResultId}' not found");

        foundResultBatch.MarkAsFinished(request.CompletedAt);
        await _unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}