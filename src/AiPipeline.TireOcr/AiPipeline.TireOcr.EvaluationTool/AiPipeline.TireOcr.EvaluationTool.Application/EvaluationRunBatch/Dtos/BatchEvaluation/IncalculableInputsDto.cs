using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunBatchAggregate;

namespace AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRunBatch.Dtos.BatchEvaluation;

public record IncalculableInputsDto(
    decimal? AnnualFixedCostUsd,
    EvaluationRunBatchEntity? InferenceStabilityRelative
);