using AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRunBatch.Dtos;
using TireOcr.Shared.UseCase;

namespace AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRunBatch.Commands.UpdateEvaluationBatch;

public record UpdateEvaluationBatchCommand(
    Guid BatchId,
    string? BatchTitle
) : ICommand<EvaluationRunBatchFullDto>;