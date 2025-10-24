namespace TireOcr.Preprocessing.WebApi.Contracts.ExtractSlices;

public record SliceDto(
    string FileName,
    string ContentType,
    string Base64ImageData
);