using System.ComponentModel.DataAnnotations;

namespace TireOcr.Preprocessing.WebApi.Contracts.ExtractSlicesComposition;

public record ExtractSlicesCompositionRequest(
    [Required] IFormFile Image,
    [Required] int NumberOfSlices
);