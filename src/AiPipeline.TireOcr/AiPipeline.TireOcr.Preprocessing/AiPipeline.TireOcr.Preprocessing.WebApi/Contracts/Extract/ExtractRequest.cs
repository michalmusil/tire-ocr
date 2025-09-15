using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TireOcr.Preprocessing.WebApi.Contracts.Extract;

public record ExtractRequest(
    [Required] IFormFile Image,
    [DefaultValue(false)] bool RemoveBackground
);