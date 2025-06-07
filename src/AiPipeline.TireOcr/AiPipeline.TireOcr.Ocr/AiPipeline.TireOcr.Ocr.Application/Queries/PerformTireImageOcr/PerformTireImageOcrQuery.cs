using TireOcr.Ocr.Application.Dtos;
using TireOcr.Ocr.Domain;
using TireOcr.Shared.UseCase;

namespace TireOcr.Ocr.Application.Queries.PerformTireImageOcr;

public record PerformTireImageOcrQuery(
    TireCodeDetectorType DetectorType,
    byte[] ImageData,
    string ImageName,
    string ImageContentType) : IQuery<OcrResultDto>;