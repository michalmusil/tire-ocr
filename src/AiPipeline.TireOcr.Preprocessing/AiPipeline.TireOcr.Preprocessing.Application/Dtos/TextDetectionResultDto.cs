using TireOcr.Preprocessing.Domain.Common;
using TireOcr.Preprocessing.Domain.ImageEntity;

namespace TireOcr.Preprocessing.Application.Dtos;

public record TextDetectionResultDto(
    Image BestImage,
    Dictionary<StringInImage, int> DetectedStringsWithLevenshteinDistance,
    TimeSpan TimeTaken
);