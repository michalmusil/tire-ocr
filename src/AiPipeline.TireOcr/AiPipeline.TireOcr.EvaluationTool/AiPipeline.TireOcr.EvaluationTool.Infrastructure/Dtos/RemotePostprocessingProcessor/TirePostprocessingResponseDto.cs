using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate;

namespace AiPipeline.TireOcr.EvaluationTool.Infrastructure.Dtos.RemotePostprocessingProcessor;

public record TirePostprocessingResponseDto(
    string RawCode,
    string PostprocessedTireCode,
    string? VehicleClass,
    decimal? Width,
    decimal? AspectRatio,
    string? Construction,
    decimal? Diameter,
    char? LoadRange,
    int? LoadIndex,
    int? LoadIndex2,
    string? SpeedRating,
    long DurationMs
)
{
    public PostprocessingResultValueObject ToDomain()
    {
        return new PostprocessingResultValueObject
        {
            TireCode = new TireCodeValueObject
            (
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
            ),
            DurationMs = DurationMs
        };
    }
}