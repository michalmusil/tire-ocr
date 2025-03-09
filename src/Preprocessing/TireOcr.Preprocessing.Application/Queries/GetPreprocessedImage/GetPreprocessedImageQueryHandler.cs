using TireOcr.Preprocessing.Application.Dtos;
using TireOcr.Preprocessing.Application.Services;
using TireOcr.Preprocessing.Domain.Common;
using TireOcr.Preprocessing.Domain.ImageEntity;
using TireOcr.Shared.Result;
using TireOcr.Shared.UseCase;

namespace TireOcr.Preprocessing.Application.Queries.GetPreprocessedImage;

public class GetPreprocessedImageQueryHandler : IQueryHandler<GetPreprocessedImageQuery, PreprocessedImageDto>
{
    private readonly IImageManipulationService _imageManipulationService;
    private readonly ITireDetectionService _tireDetectionService;
    private readonly IImageSlicer _imageSlicer;

    public GetPreprocessedImageQueryHandler(IImageManipulationService imageManipulationService,
        ITireDetectionService tireDetectionService, IImageSlicer imageSlicer)
    {
        _imageManipulationService = imageManipulationService;
        _tireDetectionService = tireDetectionService;
        _imageSlicer = imageSlicer;
    }

    public async Task<DataResult<PreprocessedImageDto>> Handle(
        GetPreprocessedImageQuery request,
        CancellationToken cancellationToken
    )
    {
        var originalSize = _imageManipulationService.GetRawImageSize(request.ImageData);
        var image = new Image(request.ImageData, request.ImageName, originalSize);

        var resized = _imageManipulationService.ResizeToMaxSideSize(image, 2048);
        var withClahe = _imageManipulationService.ApplyClahe(
            resized,
            windowSize: new ImageSize(10, 10)
        );

        var circlesResult = await _tireDetectionService.DetectTireRimCircle(withClahe);
        if (circlesResult.IsFailure)
            return DataResult<PreprocessedImageDto>.Failure(circlesResult.Failures);

        var unwrapped = GetUnwrappedTireStrip(withClahe, circlesResult.Data!);
        var slicesResult = await SliceTireStrip(unwrapped);
        if (slicesResult.IsFailure)
            return DataResult<PreprocessedImageDto>.Failure(slicesResult.Failures);

        var slices = slicesResult.Data!.ToList();


        var dto = new PreprocessedImageDto(slices[0].Name, slices[0].Data, "image/jpeg");
        return DataResult<PreprocessedImageDto>.Success(dto);
    }

    private Image GetUnwrappedTireStrip(Image image, CircleInImage rimCircle)
    {
        var outerTireRadius = rimCircle.Radius * 1.25;
        var innerTireRadius = rimCircle.Radius * 0.9;
        return _imageManipulationService.UnwrapRingIntoRectangle(image,
            rimCircle.Center,
            innerTireRadius,
            outerTireRadius
        );
    }

    private async Task<DataResult<IEnumerable<Image>>> SliceTireStrip(Image image)
    {
        var sliceSize = new ImageSize(
            image.Size.Height,
            (int)(image.Size.Width * 0.2)
        );

        return await _imageSlicer.SliceImage(image, sliceSize, 0.3, 0);
    }
}