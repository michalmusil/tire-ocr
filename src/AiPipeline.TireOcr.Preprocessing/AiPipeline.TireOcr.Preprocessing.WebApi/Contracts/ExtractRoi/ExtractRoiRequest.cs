using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TireOcr.Preprocessing.WebApi.Contracts.ExtractRoi;

public record ExtractRoiRequest(
    [Required] IFormFile Image,
    [DefaultValue(false)] bool EnhanceCharacters
);