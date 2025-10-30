using AiPipeline.TireOcr.EvaluationTool.Application.Common.DataAccess;
using TireOcr.Shared.Result;
using TireOcr.Shared.UseCase;

namespace AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRun.Commands.DeleteEvaluationRun;

public class DeleteEvaluationRunCommandHandler : ICommandHandler<DeleteEvaluationRunCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteEvaluationRunCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteEvaluationRunCommand request,
        CancellationToken cancellationToken)
    {
        var foundRun = await _unitOfWork.EvaluationRunRepository.GetEvaluationRunByIdAsync(request.RunId);
        if (foundRun is null)
            return Result.NotFound($"Run with id {request.RunId} does not exist.");

        await _unitOfWork.EvaluationRunRepository.Remove(foundRun);
        await _unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}