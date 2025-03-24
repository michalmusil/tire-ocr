using System.ComponentModel.DataAnnotations;

namespace TireOcr.Ocr.WebApi.Contracts.PerformOcr;

public record PerformOcrResponse(
    [Required] string DetectedCode
);