namespace TireOcr.Preprocessing.Application.Dtos;

public record PreprocessedImageDto(string Name, byte[] ImageData, string ContentType);