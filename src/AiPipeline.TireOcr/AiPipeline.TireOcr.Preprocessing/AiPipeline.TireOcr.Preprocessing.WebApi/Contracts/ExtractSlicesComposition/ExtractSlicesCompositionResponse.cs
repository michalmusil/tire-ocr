namespace TireOcr.Preprocessing.WebApi.Contracts.ExtractSlicesComposition;

public record ExtractSlicesCompositionResponse(
    string FileName,
    string ContentType,
    string Base64ImageData,
    long DurationMs
);