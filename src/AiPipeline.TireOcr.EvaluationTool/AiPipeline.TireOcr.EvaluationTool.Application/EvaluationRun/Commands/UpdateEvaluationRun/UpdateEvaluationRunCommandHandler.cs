using AiPipeline.TireOcr.EvaluationTool.Application.Common.DataAccess;
using AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRun.Dtos.EvaluationRun;
using TireOcr.Shared.Result;
using TireOcr.Shared.UseCase;

namespace AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRun.Commands.UpdateEvaluationRun;

public class UpdateEvaluationRunCommandHandler : ICommandHandler<UpdateEvaluationRunCommand, EvaluationRunDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateEvaluationRunCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<DataResult<EvaluationRunDto>> Handle(UpdateEvaluationRunCommand request,
        CancellationToken cancellationToken)
    {
        var foundRun = await _unitOfWork.EvaluationRunRepository.GetEvaluationRunByIdAsync(request.RunId);
        if (foundRun is null)
            return DataResult<EvaluationRunDto>.NotFound($"Run with id {request.RunId} does not exist.");

        var updateResults = new List<Result>();

        // Performing actual updates
        if (request.RunTitle is not null)
            updateResults.Add(foundRun.SetTitle(request.RunTitle));
        updateResults.Add(foundRun.SetDescription(request.RunDescription));

        var failures = updateResults
            .SelectMany(r => r.Failures)
            .ToArray();

        if (failures.Any())
            return DataResult<EvaluationRunDto>.Failure(failures.ToArray());

        await _unitOfWork.SaveChangesAsync();
        var dto = EvaluationRunDto.FromDomain(foundRun);
        return DataResult<EvaluationRunDto>.Success(dto);
    }
}