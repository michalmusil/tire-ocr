using System.ComponentModel.DataAnnotations;
using TireOcr.RunnerPrototype.Models;

namespace TireOcr.RunnerPrototype.Contracts.RunBatch;

public record RunBatchRequest(
    [Required] TireCodeDetectorType DetectorType,
    [Required] IEnumerable<IFormFile> Images
);