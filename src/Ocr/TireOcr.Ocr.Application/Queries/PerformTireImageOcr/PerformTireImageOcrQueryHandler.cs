using TireOcr.Ocr.Application.Dtos;
using TireOcr.Ocr.Application.Services;
using TireOcr.Ocr.Domain.ImageEntity;
using TireOcr.Shared.Result;
using TireOcr.Shared.UseCase;

namespace TireOcr.Ocr.Application.Queries.PerformTireImageOcr;

public class PerformTireImageOcrQueryHandler : IQueryHandler<PerformTireImageOcrQuery, OcrResultDto>
{
    private readonly ITireCodeOcrService _tireCodeOcrService;

    public PerformTireImageOcrQueryHandler(ITireCodeOcrService tireCodeOcrService)
    {
        _tireCodeOcrService = tireCodeOcrService;
    }

    public async Task<DataResult<OcrResultDto>> Handle(PerformTireImageOcrQuery request,
        CancellationToken cancellationToken)
    {
        var image = new Image(request.ImageData, request.ImageName, request.ImageContentType);

        return await _tireCodeOcrService.DetectAsync(request.DetectorType, image);
    }
}