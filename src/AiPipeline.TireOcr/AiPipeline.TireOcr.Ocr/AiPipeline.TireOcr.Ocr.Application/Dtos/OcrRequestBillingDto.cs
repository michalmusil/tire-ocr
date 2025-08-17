namespace TireOcr.Ocr.Application.Dtos;

public record OcrRequestBillingDto(decimal InputAmount, decimal OutputAmount, BillingUnitType UnitType);