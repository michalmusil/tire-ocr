namespace TireOcr.Preprocessing.WebApi.Contracts.ResizeToMaxSide;

public record ResizeToMaxSideResponse(
    string FileName,
    string ContentType,
    string Base64ImageData,
    long DurationMs
);