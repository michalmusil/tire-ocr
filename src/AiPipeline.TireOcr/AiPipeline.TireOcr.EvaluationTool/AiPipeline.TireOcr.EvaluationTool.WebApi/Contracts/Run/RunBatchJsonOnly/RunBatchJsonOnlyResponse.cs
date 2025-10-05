using AiPipeline.TireOcr.EvaluationTool.Application.Dtos.EvaluationRun;

namespace AiPipeline.TireOcr.EvaluationTool.WebApi.Contracts.Run.RunBatchJsonOnly;

public record RunBatchJsonOnlyResponse(
    EvaluationRunBatchDto Result
);