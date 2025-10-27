using AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRun.Dtos.EvaluationRun;
using AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRunBatch.Dtos;
using TireOcr.Shared.UseCase;

namespace AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRunBatch.Commands.RunEvaluationBatch;

public record RunEvaluationBatchCommand(
    Dictionary<string, string?> ImageUrlsWithExpectedTireCodeLabels,
    RunConfigDto RunConfig,
    int ProcessingBatchSize = 5,
    Guid? BatchId = null,
    string? BatchTitle = null
) : ICommand<EvaluationRunBatchFullDto>;