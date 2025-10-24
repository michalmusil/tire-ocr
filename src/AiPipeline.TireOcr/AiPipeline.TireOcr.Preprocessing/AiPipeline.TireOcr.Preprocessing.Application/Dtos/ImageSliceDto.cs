namespace TireOcr.Preprocessing.Application.Dtos;

public record ImageSliceDto(string Name, byte[] ImageData, string ContentType);