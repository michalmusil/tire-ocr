using AiPipeline.TireOcr.EvaluationTool.Application.Dtos;

namespace AiPipeline.TireOcr.EvaluationTool.WebApi.Contracts.Batch.RunBatchForm;

public record RunBatchFormResponse(
    EvaluationRunBatchFullDto Result
);