namespace AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRunBatch.Dtos.BatchEvaluation;

public record IncalculableInputsDto(
    decimal? FixedExpenditurePer1000Requests,
    bool AddVariableExpenditure
);