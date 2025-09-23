namespace TireOcr.Ocr.Application.Dtos;

public record OcrWithBillingDto(
    string DetectedCode,
    string? DetectedManufacturer,
    EstimatedCostsDto? EstimatedCosts
);