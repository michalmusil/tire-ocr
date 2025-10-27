namespace AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRunBatch.Dtos.BatchEvaluation;

public record BatchEvaluationDistancesDto(
    double AverageDistance,
    double AverageVehicleClassDistance,
    double AverageWidthDistance,
    double AverageDiameterDistance,
    double AverageAspectRatioDistance,
    double AverageConstructionDistance,
    double AverageLoadRangeDistance,
    double AverageLoadIndexDistance,
    double AverageLoadIndex2Distance,
    double AverageSpeedRatingDistance
);