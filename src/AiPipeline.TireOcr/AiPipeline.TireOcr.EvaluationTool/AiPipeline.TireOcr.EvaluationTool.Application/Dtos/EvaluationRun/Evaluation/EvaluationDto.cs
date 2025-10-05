using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate.Evaluation;

namespace AiPipeline.TireOcr.EvaluationTool.Application.Dtos.EvaluationRun.Evaluation;

public record EvaluationDto(
    TireCodeDto ExpectedTireCode,
    int TotalDistance,
    int FullMatchParameterCount,
    decimal EstimatedAccuracy,
    ParameterEvaluationDto? VehicleClassEvaluation,
    ParameterEvaluationDto? WidthEvaluation,
    ParameterEvaluationDto? DiameterEvaluation,
    ParameterEvaluationDto? AspectRatioEvaluation,
    ParameterEvaluationDto? ConstructionEvaluation,
    ParameterEvaluationDto? LoadRangeEvaluation,
    ParameterEvaluationDto? LoadIndexEvaluation,
    ParameterEvaluationDto? LoadIndex2Evaluation,
    ParameterEvaluationDto? SpeedRatingEvaluation
)
{
    public static EvaluationDto FromDomain(EvaluationEntity domain) => new EvaluationDto(
        ExpectedTireCode: TireCodeDto.FromDomain(domain.ExpectedTireCode),
        TotalDistance: domain.TotalDistance,
        FullMatchParameterCount: domain.FullMatchParameterCount,
        EstimatedAccuracy: domain.EstimatedAccuracy,
        VehicleClassEvaluation: domain.VehicleClassEvaluation is not null
            ? ParameterEvaluationDto.FromDomain(domain.VehicleClassEvaluation)
            : null,
        WidthEvaluation: domain.WidthEvaluation is not null
            ? ParameterEvaluationDto.FromDomain(domain.WidthEvaluation)
            : null,
        DiameterEvaluation: domain.DiameterEvaluation is not null
            ? ParameterEvaluationDto.FromDomain(domain.DiameterEvaluation)
            : null,
        AspectRatioEvaluation: domain.AspectRatioEvaluation is not null
            ? ParameterEvaluationDto.FromDomain(domain.AspectRatioEvaluation)
            : null,
        ConstructionEvaluation: domain.ConstructionEvaluation is not null
            ? ParameterEvaluationDto.FromDomain(domain.ConstructionEvaluation)
            : null,
        LoadRangeEvaluation: domain.LoadRangeEvaluation is not null
            ? ParameterEvaluationDto.FromDomain(domain.LoadRangeEvaluation)
            : null,
        LoadIndexEvaluation: domain.LoadIndexEvaluation is not null
            ? ParameterEvaluationDto.FromDomain(domain.LoadIndexEvaluation)
            : null,
        LoadIndex2Evaluation: domain.LoadIndex2Evaluation is not null
            ? ParameterEvaluationDto.FromDomain(domain.LoadIndex2Evaluation)
            : null,
        SpeedRatingEvaluation: domain.SpeedRatingEvaluation is not null
            ? ParameterEvaluationDto.FromDomain(domain.SpeedRatingEvaluation)
            : null
    );
}