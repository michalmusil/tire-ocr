namespace TireOcr.RunnerPrototype.Dtos.Preprocessing;

public record PreprocessingResponseDto(
    string FileName,
    string ContentType,
    string Base64ImageData
);