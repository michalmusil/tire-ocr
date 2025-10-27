using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate;

namespace AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRun.Dtos.EvaluationRun;

public record TireCodeDto(
    string RawCode,
    string ProcessedCode,
    string? VehicleClass,
    decimal? Width,
    decimal? AspectRatio,
    string? Construction,
    decimal? Diameter,
    char? LoadRange,
    int? LoadIndex,
    int? LoadIndex2,
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
        LoadRange: domain.LoadRange,
        LoadIndex: domain.LoadIndex,
        LoadIndex2: domain.LoadIndex2,
        SpeedRating: domain.SpeedRating
    );

    public TireCodeValueObject ToDomain() => new(
        rawCode: RawCode,
        vehicleClass: VehicleClass,
        width: Width,
        aspectRatio: AspectRatio,
        construction: Construction,
        diameter: Diameter,
        loadRange: LoadRange,
        loadIndex: LoadIndex,
        loadIndex2: LoadIndex2,
        speedRating: SpeedRating
    );
}