using System.ComponentModel.DataAnnotations;
using TireOcr.Ocr.Domain;

namespace TireOcr.Ocr.WebApi.Contracts.PerformOcr;

public record PerformOcrRequest(
    [Required] TireCodeDetectorType DetectorType,
    [Required] IFormFile Image
);