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
    string? LoadIndex,
    string? SpeedRating
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
                LoadIndex = LoadIndex,
                SpeedRating = SpeedRating
            },
            DurationMs = 0 // TODO: add in remote postprocessing service
        };
    }
}