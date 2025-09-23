namespace TireOcr.Ocr.Application.Dtos;

public record OcrResultDto(string DetectedTireCode, string? DetectedManufacturer, OcrRequestBillingDto? Billing);