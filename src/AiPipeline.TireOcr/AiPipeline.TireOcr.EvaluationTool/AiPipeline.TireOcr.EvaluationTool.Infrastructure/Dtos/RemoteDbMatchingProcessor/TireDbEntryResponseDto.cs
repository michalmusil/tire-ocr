using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate;

namespace AiPipeline.TireOcr.EvaluationTool.Infrastructure.Dtos.RemoteDbMatchingProcessor;

public record TireDbEntryResponseDto(
    decimal Width,
    decimal Diameter,
    decimal Profile,
    string Construction,
    int? LoadIndex,
    int? LoadIndex2,
    string? SpeedIndex,
    string LoadIndexSpeedIndex
)
{
    public TireCodeValueObject ToDomain() => new()
    {
        RawCode = $"{Width}/{Profile}{Construction}{Diameter}{LoadIndex}{GetLoadIndex2String()}{SpeedIndex}",
        VehicleClass = null,
        Width = Width,
        Diameter = Diameter,
        AspectRatio = Profile,
        Construction = Construction,
        LoadIndex = LoadIndex,
        LoadIndex2 = LoadIndex2,
        SpeedRating = SpeedIndex,
    };

    private string GetLoadIndex2String() => LoadIndex2 is null ? "" : $"/{LoadIndex2}";
}