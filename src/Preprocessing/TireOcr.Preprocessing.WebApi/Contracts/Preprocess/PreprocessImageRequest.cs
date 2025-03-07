using System.ComponentModel.DataAnnotations;

namespace TireOcr.Preprocessing.WebApi.Contracts.Preprocess;

public record PreprocessImageRequest(
    [Required] 
    IFormFile Image
);