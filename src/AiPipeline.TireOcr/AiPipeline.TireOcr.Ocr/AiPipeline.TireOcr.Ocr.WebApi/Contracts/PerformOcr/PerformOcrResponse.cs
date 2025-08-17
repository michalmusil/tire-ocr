using TireOcr.Ocr.Application.Dtos;

namespace TireOcr.Ocr.WebApi.Contracts.PerformOcr;

public record PerformOcrResponse(
    string DetectedCode,
    EstimatedCostsDto? EstimatedCosts
);