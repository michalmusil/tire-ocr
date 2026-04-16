namespace AiPipeline.TireOcr.DbMatcher.Application.Dtos;

public record ParameterMatchDto(
    int RequiredCharEdits,
    decimal EstimatedAccuracy
)
{
    public bool MatchesExactly => RequiredCharEdits == 0;
}