using TireOcr.Preprocessing.Application.Dtos;
using TireOcr.Preprocessing.Application.Services;
using TireOcr.Preprocessing.Domain.ImageEntity;
using TireOcr.Shared.Result;
using TireOcr.Shared.UseCase;

namespace TireOcr.Preprocessing.Application.Queries.GetPreprocessedImage;

public class GetPreprocessedImageQueryHandler : IQueryHandler<GetPreprocessedImageQuery, PreprocessedImageDto>
{
    private readonly IImageManipulationService _imageManipulationService;
    private readonly ITireDetectionService _tireDetectionService;

    public GetPreprocessedImageQueryHandler(IImageManipulationService imageManipulationService,
        ITireDetectionService tireDetectionService)
    {
        _imageManipulationService = imageManipulationService;
        _tireDetectionService = tireDetectionService;
    }

    public async Task<DataResult<PreprocessedImageDto>> Handle(
        GetPreprocessedImageQuery request,
        CancellationToken cancellationToken
    )
    {
        var originalSize = _imageManipulationService.GetRawImageSize(request.ImageData);
        var image = new Image(request.ImageData, request.ImageName, originalSize);
        
        var resized = _imageManipulationService.ResizeToMaxSideSize(image, 2048);
        var withClahe = _imageManipulationService.ApplyClahe(resized);

        var circlesResult = await _tireDetectionService.DetectTireRimCircle(withClahe);
        if (circlesResult.IsFailure)
            return DataResult<PreprocessedImageDto>.Failure(circlesResult.Failures);
        
        var dto = new PreprocessedImageDto(withClahe.Name, withClahe.Data, "image/jpeg");
        return DataResult<PreprocessedImageDto>.Success(dto);
    }
}