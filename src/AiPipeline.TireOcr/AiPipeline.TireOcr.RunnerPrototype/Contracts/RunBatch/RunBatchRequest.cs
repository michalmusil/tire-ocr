using System.ComponentModel.DataAnnotations;
using TireOcr.RunnerPrototype.Models;

namespace TireOcr.RunnerPrototype.Contracts.RunBatch;

public record RunBatchRequest(
    [Required] TireCodeDetectorType DetectorType,
    [Required] [Range(1, 10)] int BatchSize,
    [Required] [MinLength(1)] IEnumerable<string> ImageUrls
);