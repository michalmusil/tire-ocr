using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate.DbMatch;

namespace AiPipeline.TireOcr.EvaluationTool.Infrastructure.Dtos.RemoteDbMatchingProcessor;

public record TireDbMatchResponseDto(
    TireDbEntryResponseDto TireEntry,
    decimal EstimatedAccuracy,
    int TotalRequiredCharEdits,
    int MatchedMainParameterCount,
    ParameterMatchResponseDto WidthMatch,
    ParameterMatchResponseDto DiameterMatch,
    ParameterMatchResponseDto ProfileMatch,
    ParameterMatchResponseDto? ConstructionMatch,
    ParameterMatchResponseDto LoadIndexMatch,
    ParameterMatchResponseDto? LoadIndex2Match,
    ParameterMatchResponseDto SpeedIndexMatch
)
{
    public TireDbMatchValueObject ToDomain() => new()
    {
        TireCode = TireEntry.ToDomain(),
        EstimatedAccuracy = EstimatedAccuracy,
        TotalRequiredCharEdits = TotalRequiredCharEdits,
        WidthEditDistance = WidthMatch.RequiredCharEdits,
        DiameterEditDistance = DiameterMatch.RequiredCharEdits,
        ProfileEditDistance = ProfileMatch.RequiredCharEdits,
        ConstructionEditDistance = ConstructionMatch?.RequiredCharEdits,
        LoadIndexEditDistance = LoadIndexMatch.RequiredCharEdits,
        LoadIndex2EditDistance = LoadIndex2Match?.RequiredCharEdits,
        SpeedIndexEditDistance = SpeedIndexMatch.RequiredCharEdits,
    };
}