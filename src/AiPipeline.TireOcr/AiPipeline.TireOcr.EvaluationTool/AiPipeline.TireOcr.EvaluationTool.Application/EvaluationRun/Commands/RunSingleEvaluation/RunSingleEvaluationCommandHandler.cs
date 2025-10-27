using AiPipeline.TireOcr.EvaluationTool.Application.Common.DataAccess;
using AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRun.Dtos;
using AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRun.Dtos.EvaluationRun;
using AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRun.Facades;
using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate;
using TireOcr.Shared.Result;
using TireOcr.Shared.UseCase;

namespace AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRun.Commands.RunSingleEvaluation;

public class RunSingleEvaluationCommandHandler : ICommandHandler<RunSingleEvaluationCommand, EvaluationRunDto>
{
    private readonly IRunFacade _runFacade;
    private readonly IUnitOfWork _unitOfWork;

    public RunSingleEvaluationCommandHandler(IRunFacade runFacade, IUnitOfWork unitOfWork)
    {
        _runFacade = runFacade;
        _unitOfWork = unitOfWork;
    }

    public async Task<DataResult<EvaluationRunDto>> Handle(RunSingleEvaluationCommand request,
        CancellationToken cancellationToken)
    {
        var inputDetails = new RunEntityInputDetailsDto(
            Id: request.RunId,
            Title: request.RunTitle
        );

        var expectedTireCodeResult = request.ExpectedTireCodeLabel is null
            ? null
            : TireCodeValueObject.FromLabelString(request.ExpectedTireCodeLabel);

        var result = request.InputImage is null
            ? await _runFacade.RunSingleEvaluationAsync(
                imageUrl: request.InputImageUrl!,
                runConfig: request.RunConfig,
                expectedTireCode: expectedTireCodeResult?.Data,
                runEntityInputDetailsDto: inputDetails
            )
            : await _runFacade.RunSingleEvaluationAsync(
                image: request.InputImage,
                runConfig: request.RunConfig,
                expectedTireCode: expectedTireCodeResult?.Data,
                runEntityInputDetailsDto: inputDetails
            );

        if (result.IsFailure)
            return DataResult<EvaluationRunDto>.Failure(result.Failures);

        var evaluationRun = result.Data!;
        await _unitOfWork.EvaluationRunRepository.Add(evaluationRun);
        await _unitOfWork.SaveChangesAsync();

        var dto = EvaluationRunDto.FromDomain(result.Data!);
        return DataResult<EvaluationRunDto>.Success(dto);
    }
}