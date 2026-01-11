namespace AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRunBatch.Dtos.BatchEvaluation;

public record BatchEvaluationDto(
    BatchEvaluationCountsDto Counts,
    BatchEvaluationDistancesDto Distances,
    decimal AverageCer
);