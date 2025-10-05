using AiPipeline.TireOcr.EvaluationTool.Application.Dtos.EvaluationRun;

namespace AiPipeline.TireOcr.EvaluationTool.WebApi.Contracts.Run.RunBatchForm;

public record RunBatchFormResponse(
    EvaluationRunBatchDto Result
);