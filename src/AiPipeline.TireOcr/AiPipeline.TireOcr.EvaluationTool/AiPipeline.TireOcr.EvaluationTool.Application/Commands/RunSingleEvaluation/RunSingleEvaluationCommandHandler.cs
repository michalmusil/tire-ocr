using AiPipeline.TireOcr.EvaluationTool.Application.Dtos;
using AiPipeline.TireOcr.EvaluationTool.Application.Dtos.EvaluationRun;
using AiPipeline.TireOcr.EvaluationTool.Application.Facades;
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
        var inputDetails = new RunEntityInputDetailsDto(
            Id: request.RunId,
            Title: request.RunTitle
        );

        var result = request.InputImage is null
            ? await _runFacade.RunSingleEvaluationAsync(
                imageUrl: request.InputImageUrl!,
                runConfig: request.RunConfig,
                expectedTireCode: request.ExpectedTireCode?.ToDomain(),
                runEntityInputDetailsDto: inputDetails
            )
            : await _runFacade.RunSingleEvaluationAsync(
                image: request.InputImage,
                runConfig: request.RunConfig,
                expectedTireCode: request.ExpectedTireCode?.ToDomain(),
                runEntityInputDetailsDto: inputDetails
            );

        if (result.IsFailure)
            return DataResult<EvaluationRunDto>.Failure(result.Failures);

        var dto = EvaluationRunDto.FromDomain(result.Data!);
        return DataResult<EvaluationRunDto>.Success(dto);
    }
}