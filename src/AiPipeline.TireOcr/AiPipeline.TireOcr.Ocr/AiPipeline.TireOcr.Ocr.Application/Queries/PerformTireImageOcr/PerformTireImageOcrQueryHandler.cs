using TireOcr.Ocr.Application.Dtos;
using TireOcr.Ocr.Application.Services;
using TireOcr.Ocr.Domain.ImageEntity;
using TireOcr.Shared.Extensions;
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

    private record OcrResultWithoutDuration(
        string DetectedCode,
        string? DetectedManufacturer,
        EstimatedCostsDto? EstimatedCosts
    );

    public async Task<DataResult<OcrWithBillingDto>> Handle(PerformTireImageOcrQuery request,
        CancellationToken cancellationToken)
    {
        var result = await PerformanceUtils.PerformTimeMeasuredTask(
            runTask: () => PerformOcr(request)
        );
        var timeTaken = result.Item1;
        var actualResult = result.Item2;

        return actualResult.Map(
            onSuccess: res =>
            {
                var withDuration = new OcrWithBillingDto(
                    DetectedCode: res.DetectedCode,
                    DetectedManufacturer: res.DetectedManufacturer,
                    EstimatedCosts: res.EstimatedCosts,
                    DurationMs: (long)timeTaken.TotalMilliseconds
                );
                return DataResult<OcrWithBillingDto>.Success(withDuration);
            },
            onFailure: DataResult<OcrWithBillingDto>.Failure
        );
    }

    private async Task<DataResult<OcrResultWithoutDuration>> PerformOcr(PerformTireImageOcrQuery request)
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
            return DataResult<OcrResultWithoutDuration>.Failure(ocrResult.Failures);

        var ocrResultData = ocrResult.Data!;
        var result = new OcrResultWithoutDuration(
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

        return DataResult<OcrResultWithoutDuration>.Success(result);
    }
}