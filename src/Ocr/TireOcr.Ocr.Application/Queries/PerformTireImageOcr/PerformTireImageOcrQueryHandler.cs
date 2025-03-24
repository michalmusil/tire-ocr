using TireOcr.Ocr.Application.Dtos;
using TireOcr.Ocr.Application.Services;
using TireOcr.Ocr.Domain.ImageEntity;
using TireOcr.Shared.Result;
using TireOcr.Shared.UseCase;

namespace TireOcr.Ocr.Application.Queries.PerformTireImageOcr;

public class PerformTireImageOcrQueryHandler : IQueryHandler<PerformTireImageOcrQuery, OcrResultDto>
{
    private readonly ITireCodeDetectionService _tireCodeDetectionService;

    public PerformTireImageOcrQueryHandler(ITireCodeDetectionService tireCodeDetectionService)
    {
        _tireCodeDetectionService = tireCodeDetectionService;
    }

    public async Task<DataResult<OcrResultDto>> Handle(PerformTireImageOcrQuery request,
        CancellationToken cancellationToken)
    {
        var image = new Image(request.ImageData, request.ImageName, request.ImageContentType);

        return await _tireCodeDetectionService.DetectAsync(request.DetectorType, image);
    }
}