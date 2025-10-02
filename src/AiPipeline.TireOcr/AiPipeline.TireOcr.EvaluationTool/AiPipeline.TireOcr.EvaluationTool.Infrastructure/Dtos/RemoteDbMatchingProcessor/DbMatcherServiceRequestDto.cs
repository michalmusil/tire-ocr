using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate;

namespace AiPipeline.TireOcr.EvaluationTool.Infrastructure.Dtos.RemoteDbMatchingProcessor;

public record DbMatcherServiceRequestDto(
    string RawCode,
    string PostprocessedTireCode,
    string? VehicleClass,
    decimal? Width,
    decimal? AspectRatio,
    string? Construction,
    decimal? Diameter,
    string? LoadIndex,
    string? SpeedRating
)
{
    public static DbMatcherServiceRequestDto FromDomain(TireCodeValueObject domain) => new DbMatcherServiceRequestDto(
        RawCode: domain.RawCode,
        PostprocessedTireCode: domain.ToString(),
        VehicleClass: domain.VehicleClass,
        Width: domain.Width,
        AspectRatio: domain.AspectRatio,
        Construction: domain.Construction,
        Diameter: domain.Diameter,
        LoadIndex: domain.LoadIndex,
        SpeedRating: domain.SpeedRating
    );
}