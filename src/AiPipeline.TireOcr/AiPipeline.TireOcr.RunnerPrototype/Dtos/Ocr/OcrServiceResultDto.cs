using System.ComponentModel.DataAnnotations;

namespace TireOcr.RunnerPrototype.Dtos.Ocr;

public record OcrServiceResultDto(
    [Required] string DetectedCode,
    EstimatedCostsDto? EstimatedCosts
);