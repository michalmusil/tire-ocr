using System.ComponentModel.DataAnnotations;
using TireOcr.RunnerPrototype.Models;

namespace TireOcr.RunnerPrototype.Contracts.RunSingle;

public record RunSingleRequest(
    [Required] TireCodeDetectorType DetectorType,
    [Required] IFormFile Image
);