namespace TireOcr.Preprocessing.WebApi.Contracts.Extract;

public record ExtractResponse(
    string FileName,
    string ContentType,
    string Base64ImageData,
    long DurationMs
);