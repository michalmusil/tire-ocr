using System.ComponentModel.DataAnnotations;
using AiPipeline.TireOcr.Shared.Models;

namespace TireOcr.Ocr.WebApi.Contracts.PerformOcr;

public record PerformOcrRequest(
    [Required] TireCodeDetectorType DetectorType,
    [Required] IFormFile Image
);