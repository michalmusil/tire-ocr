using AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRun.Dtos.EvaluationRun;
using TireOcr.Shared.UseCase;

namespace AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRun.Commands.UpdateEvaluationRun;

public record UpdateEvaluationRunCommand(
    Guid RunId,
    string? RunTitle
) : ICommand<EvaluationRunDto>;