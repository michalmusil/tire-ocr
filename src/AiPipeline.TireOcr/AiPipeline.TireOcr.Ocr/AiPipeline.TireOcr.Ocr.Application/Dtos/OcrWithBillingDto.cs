namespace TireOcr.Ocr.Application.Dtos;

public record OcrWithBillingDto(
    string DetectedCode,
    EstimatedCostsDto? EstimatedCosts
);