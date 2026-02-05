namespace TireOcr.Preprocessing.WebApi.Contracts.ExtractRoi;

public record ExtractRoiResponse(
    string FileName,
    string ContentType,
    string Base64ImageData,
    long DurationMs
);