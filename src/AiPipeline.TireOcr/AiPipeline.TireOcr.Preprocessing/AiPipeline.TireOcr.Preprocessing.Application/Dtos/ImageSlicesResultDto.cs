namespace TireOcr.Preprocessing.Application.Dtos;

public record ImageSlicesResultDto(
    IEnumerable<ImageSliceDto> Slices,
    long DurationMs
);