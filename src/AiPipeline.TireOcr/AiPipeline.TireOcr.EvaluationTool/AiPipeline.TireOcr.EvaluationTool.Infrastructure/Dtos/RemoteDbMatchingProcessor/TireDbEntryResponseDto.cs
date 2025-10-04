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
    public TireCodeValueObject ToDomain() => new(
        rawCode: $"{Width}/{Profile}{Construction}{Diameter}{LoadIndex}{GetLoadIndex2String()}{SpeedIndex}",
        vehicleClass: null,
        width: Width,
        diameter: Diameter,
        aspectRatio: Profile,
        construction: Construction,
        loadIndex: LoadIndex,
        loadIndex2: LoadIndex2,
        speedRating: SpeedIndex
    );

    private string GetLoadIndex2String() => LoadIndex2 is null ? "" : $"/{LoadIndex2}";
}