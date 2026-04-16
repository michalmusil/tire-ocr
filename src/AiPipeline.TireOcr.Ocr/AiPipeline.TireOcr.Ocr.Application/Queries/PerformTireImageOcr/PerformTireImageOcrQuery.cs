using AiPipeline.TireOcr.Shared.Models;
using TireOcr.Ocr.Application.Dtos;
using TireOcr.Shared.UseCase;

namespace TireOcr.Ocr.Application.Queries.PerformTireImageOcr;

public record PerformTireImageOcrQuery(
    TireCodeDetectorType DetectorType,
    byte[] ImageData,
    string ImageName,
    string ImageContentType,
    bool IncludeCostEstimation,
    int? ResizeToMaxSide
) : IQuery<OcrWithBillingDto>;