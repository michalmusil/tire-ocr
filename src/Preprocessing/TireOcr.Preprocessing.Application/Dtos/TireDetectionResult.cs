using TireOcr.Preprocessing.Domain.Common;

namespace TireOcr.Preprocessing.Application.Dtos;

public record TireDetectionResult(CircleInImage RimCircle, TimeSpan TimeTaken);