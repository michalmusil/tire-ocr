namespace AiPipeline.TireOcr.TasyDbMatcher.Application.Dtos;

public record RawTireParamsDatabaseEntryDto(
    string ProductSizeWidth,
    string ProductSizeDiameter,
    string ProductSizeProfile,
    string? ProductConstruction,
    string? ProductLi,
    string? ProductLi2,
    string? ProductSi,
    string? ProductSi2,
    string? ProductLisi
);