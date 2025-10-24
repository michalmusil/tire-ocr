namespace TireOcr.Preprocessing.WebApi.Contracts.ExtractSlices;

public record ExtractSlicesResponse(
    IEnumerable<SliceDto> Slices,
    long DurationMs
);