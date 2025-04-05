using System.ComponentModel.DataAnnotations;

namespace TireOcr.RunnerPrototype.Dtos;

public record OcrServiceResultDto(
    [Required] string DetectedCode
);