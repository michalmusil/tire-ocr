using System.ComponentModel.DataAnnotations;

namespace TireOcr.Preprocessing.WebApi.Contracts.ResizeToMaxSide;

public record ResizeToMaxSideRequest(
    [Required] IFormFile Image,
    [Required] int MaxSidePixels
);