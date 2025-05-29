using TireOcr.Preprocessing.Domain.Common;

namespace TireOcr.Preprocessing.Application.Dtos;

public record TireDetectionResultDto(CircleInImage RimCircle, TimeSpan TimeTaken);