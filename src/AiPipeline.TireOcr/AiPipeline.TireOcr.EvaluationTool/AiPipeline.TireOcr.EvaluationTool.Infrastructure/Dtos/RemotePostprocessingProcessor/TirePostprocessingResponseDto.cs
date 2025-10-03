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
            {
                RawCode = RawCode,
                VehicleClass = VehicleClass,
                Width = Width,
                AspectRatio = AspectRatio,
                Construction = Construction,
                Diameter = Diameter,
                LoadRange = LoadRange,
                LoadIndex = LoadIndex,
                LoadIndex2 = LoadIndex2,
                
                SpeedRating = SpeedRating
            },
            DurationMs = DurationMs
        };
    }
}