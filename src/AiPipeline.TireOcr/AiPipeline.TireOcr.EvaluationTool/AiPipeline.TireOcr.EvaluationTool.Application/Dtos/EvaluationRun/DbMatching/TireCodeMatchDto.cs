using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate.DbMatch;

namespace AiPipeline.TireOcr.EvaluationTool.Application.Dtos.EvaluationRun.DbMatching;

public record TireCodeMatchDto(
    TireCodeDto TireCode,
    int TotalRequiredCharEdits,
    decimal EstimatedAccuracy,
    int WidthEditDistance,
    int DiameterEditDistance,
    int ProfileEditDistance,
    int? ConstructionEditDistance,
    int LoadIndexEditDistance,
    int? LoadIndex2EditDistance,
    int SpeedIndexEditDistance
)
{
    public static TireCodeMatchDto FromDomain(TireDbMatchValueObject domain) => new TireCodeMatchDto(
        TireCode: TireCodeDto.FromDomain(domain.TireCode),
        TotalRequiredCharEdits: domain.TotalRequiredCharEdits,
        EstimatedAccuracy: domain.EstimatedAccuracy,
        WidthEditDistance: domain.WidthEditDistance,
        DiameterEditDistance: domain.DiameterEditDistance,
        ProfileEditDistance: domain.ProfileEditDistance,
        ConstructionEditDistance: domain.ConstructionEditDistance,
        LoadIndexEditDistance: domain.LoadIndexEditDistance,
        LoadIndex2EditDistance: domain.LoadIndex2EditDistance,
        SpeedIndexEditDistance: domain.SpeedIndexEditDistance
    );
}