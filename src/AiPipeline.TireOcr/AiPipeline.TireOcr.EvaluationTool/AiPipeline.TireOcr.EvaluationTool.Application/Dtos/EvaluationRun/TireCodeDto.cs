using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate;

namespace AiPipeline.TireOcr.EvaluationTool.Application.Dtos.EvaluationRun;

public record TireCodeDto(
    string RawCode,
    string ProcessedCode,
    string? VehicleClass,
    decimal? Width,
    decimal? AspectRatio,
    string? Construction,
    decimal? Diameter,
    string? LoadIndex,
    string? SpeedRating
)
{
    public static TireCodeDto FromDomain(TireCodeValueObject domain) => new(
        RawCode: domain.RawCode,
        ProcessedCode: domain.ToString(),
        VehicleClass: domain.VehicleClass,
        Width: domain.Width,
        AspectRatio: domain.AspectRatio,
        Construction: domain.Construction,
        Diameter: domain.Diameter,
        LoadIndex: domain.LoadIndex,
        SpeedRating: domain.SpeedRating
    );
}