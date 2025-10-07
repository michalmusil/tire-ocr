namespace AiPipeline.TireOcr.EvaluationTool.Application.Dtos.BatchEvaluation;

public record BatchEvaluationDto(
    BatchEvaluationCountsDto Counts,
    BatchEvaluationDistancesDto Distances
);