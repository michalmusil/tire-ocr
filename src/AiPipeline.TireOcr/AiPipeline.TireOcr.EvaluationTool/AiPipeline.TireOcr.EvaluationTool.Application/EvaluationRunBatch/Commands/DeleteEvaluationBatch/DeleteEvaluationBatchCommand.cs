using AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRun.Dtos.EvaluationRun;
using AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRunBatch.Dtos;
using TireOcr.Shared.UseCase;

namespace AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRunBatch.Commands.DeleteEvaluationBatch;

public record DeleteEvaluationBatchCommand(
    Guid BatchId
) : ICommand;