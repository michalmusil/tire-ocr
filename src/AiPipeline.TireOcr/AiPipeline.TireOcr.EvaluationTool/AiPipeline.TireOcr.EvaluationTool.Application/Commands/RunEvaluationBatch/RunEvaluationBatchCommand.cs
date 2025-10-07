using AiPipeline.TireOcr.EvaluationTool.Application.Dtos;
using AiPipeline.TireOcr.EvaluationTool.Application.Dtos.EvaluationRun;
using TireOcr.Shared.UseCase;

namespace AiPipeline.TireOcr.EvaluationTool.Application.Commands.RunEvaluationBatch;

public record RunEvaluationBatchCommand(
    Dictionary<string, string?> ImageUrlsWithExpectedTireCodeLabels,
    RunConfigDto RunConfig,
    int ProcessingBatchSize = 5,
    Guid? BatchId = null,
    string? BatchTitle = null
) : ICommand<EvaluationRunBatchFullDto>;