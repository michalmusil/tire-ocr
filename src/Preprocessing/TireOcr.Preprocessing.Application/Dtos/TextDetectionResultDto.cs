using TireOcr.Preprocessing.Domain.ImageEntity;

namespace TireOcr.Preprocessing.Application.Dtos;

public record TextDetectionResultDto(
    Image BestImage,
    Dictionary<string, int> DetectedStringsWithLevenshteinDistance,
    TimeSpan TimeTaken
);