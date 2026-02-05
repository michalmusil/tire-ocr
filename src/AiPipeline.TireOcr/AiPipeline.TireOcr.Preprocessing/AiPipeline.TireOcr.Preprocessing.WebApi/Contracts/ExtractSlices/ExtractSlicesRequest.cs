using System.ComponentModel.DataAnnotations;

namespace TireOcr.Preprocessing.WebApi.Contracts.ExtractSlices;

public record ExtractSlicesRequest(
    [Required] IFormFile Image,
    [Required] int NumberOfSlices
);