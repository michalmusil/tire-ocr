using TireOcr.Shared.UseCase;

namespace AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRun.Commands.DeleteEvaluationRun;

public record DeleteEvaluationRunCommand(
    Guid RunId
) : ICommand;