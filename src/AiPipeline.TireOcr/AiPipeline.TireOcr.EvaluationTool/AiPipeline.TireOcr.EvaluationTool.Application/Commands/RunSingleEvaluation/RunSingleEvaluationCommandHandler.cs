using AiPipeline.TireOcr.EvaluationTool.Application.Dtos.EvaluationRun;
using TireOcr.Shared.Result;
using TireOcr.Shared.UseCase;

namespace AiPipeline.TireOcr.EvaluationTool.Application.Commands.RunSingleEvaluation;

public class RunSingleEvaluationCommandHandler : ICommandHandler<RunSingleEvaluationCommand, EvaluationRunDto>
{
    public Task<DataResult<EvaluationRunDto>> Handle(RunSingleEvaluationCommand request,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}