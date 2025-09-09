using System.ComponentModel.DataAnnotations;

namespace TireOcr.Postprocessing.WebApi.Contracts.PerformPostprocessing;

public record PerformPostprocessingRequest(
    [Required] string RawTireCode
);