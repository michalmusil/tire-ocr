namespace TireOcr.Preprocessing.WebApi.Contracts.ExtractSlices;

public record ExtractSlicesResponse(
    string FileName,
    string ContentType,
    string Base64ImageData,
    long DurationMs
);