namespace AiPipeline.TireOcr.TasyDbMatcher.Application.Dtos;

public record RawTireParamsDatabaseEntryDto(
    string ProductSizeWidth,
    string ProductSizeDiameter,
    string ProductSizeProfile,
    string ProductConstruction,
    string? ProductLi,
    string? ProductSi,
    string? ProductLisi
);