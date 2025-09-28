using AiPipeline.TireOcr.EvaluationTool.Application.Dtos.EvaluationRun;
using AiPipeline.TireOcr.EvaluationTool.Application.Facades;
using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate;
using TireOcr.Shared.Result;
using TireOcr.Shared.UseCase;

namespace AiPipeline.TireOcr.EvaluationTool.Application.Commands.RunSingleEvaluation;

public class RunSingleEvaluationCommandHandler : ICommandHandler<RunSingleEvaluationCommand, EvaluationRunDto>
{
    private readonly IRunFacade _runFacade;

    public RunSingleEvaluationCommandHandler(IRunFacade runFacade)
    {
        _runFacade = runFacade;
    }

    public async Task<DataResult<EvaluationRunDto>> Handle(RunSingleEvaluationCommand request,
        CancellationToken cancellationToken)
    {
        var result = request.InputImage is null
            ? await _runFacade.RunSingleEvaluationAsync(request.InputImageUrl!, request.RunConfig,
                request.ExpectedTireCode?.ToDomain())
            : await _runFacade.RunSingleEvaluationAsync(request.InputImage, request.RunConfig,
                request.ExpectedTireCode?.ToDomain());

        if (result.IsFailure)
            return DataResult<EvaluationRunDto>.Failure(result.Failures);

        var dto = EvaluationRunDto.FromDomain(result.Data!);
        return DataResult<EvaluationRunDto>.Success(dto);
    }
}