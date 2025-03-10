using TireOcr.Preprocessing.Application.Dtos;
using TireOcr.Preprocessing.Application.Facades;
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
    private readonly ITextDetectionFacade _textDetectionFacade;

    public GetPreprocessedImageQueryHandler(IImageManipulationService imageManipulationService,
        ITireDetectionService tireDetectionService, ITextDetectionFacade textDetectionFacade)
    {
        _imageManipulationService = imageManipulationService;
        _tireDetectionService = tireDetectionService;
        _textDetectionFacade = textDetectionFacade;
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

        var textArea = await _textDetectionFacade.GetTextAreaFromImageAsync(unwrapped);
        return textArea.Map(
            onFailure: failures => DataResult<PreprocessedImageDto>.Failure(failures),
            onSuccess: res =>
            {
                var resultDto = new PreprocessedImageDto(res.BestImage.Name, res.BestImage.Data, "image/jpeg");
                return DataResult<PreprocessedImageDto>.Success(resultDto);
            }
        );
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
}