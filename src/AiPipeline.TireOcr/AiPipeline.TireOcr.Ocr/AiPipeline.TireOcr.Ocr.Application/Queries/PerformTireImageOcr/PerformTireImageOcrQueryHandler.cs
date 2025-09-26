using TireOcr.Ocr.Application.Dtos;
using TireOcr.Ocr.Application.Services;
using TireOcr.Ocr.Domain.ImageEntity;
using TireOcr.Shared.Result;
using TireOcr.Shared.UseCase;

namespace TireOcr.Ocr.Application.Queries.PerformTireImageOcr;

public class PerformTireImageOcrQueryHandler : IQueryHandler<PerformTireImageOcrQuery, OcrWithBillingDto>
{
    private readonly ITireCodeOcrService _tireCodeOcrService;
    private readonly ICostEstimationService _costEstimationService;

    public PerformTireImageOcrQueryHandler(ITireCodeOcrService tireCodeOcrService,
        ICostEstimationService costEstimationService)
    {
        _tireCodeOcrService = tireCodeOcrService;
        _costEstimationService = costEstimationService;
    }

    public async Task<DataResult<OcrWithBillingDto>> Handle(PerformTireImageOcrQuery request,
        CancellationToken cancellationToken)
    {
        var image = new Image(request.ImageData, request.ImageName, request.ImageContentType);

        var ocrResult = await _tireCodeOcrService.DetectAsync(
            detectorType: request.DetectorType,
            image: image,
            resizeOptions: request.ResizeToMaxSide.HasValue
                ? new ResizeImageToMaxSideOptions(request.ResizeToMaxSide.Value)
                : null
        );
        if (ocrResult.IsFailure)
            return DataResult<OcrWithBillingDto>.Failure(ocrResult.Failures);

        var ocrResultData = ocrResult.Data!;
        var result = new OcrWithBillingDto(
            DetectedCode: ocrResultData.DetectedTireCode,
            DetectedManufacturer: ocrResultData.DetectedManufacturer,
            EstimatedCosts: null
        );

        if (request.IncludeCostEstimation && ocrResultData.Billing is not null)
        {
            var costEstimationResult = await _costEstimationService.GetEstimatedCostsAsync(
                request.DetectorType,
                ocrResultData.Billing
            );
            result = result with { EstimatedCosts = costEstimationResult.Data };
        }

        return DataResult<OcrWithBillingDto>.Success(result);
    }
}