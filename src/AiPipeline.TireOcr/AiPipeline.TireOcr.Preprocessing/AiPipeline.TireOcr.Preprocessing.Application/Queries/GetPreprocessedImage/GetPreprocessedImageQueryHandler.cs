using Microsoft.Extensions.Logging;
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
    private readonly IContentTypeResolverService _contentTypeResolverService;
    private ILogger<GetPreprocessedImageQueryHandler> _logger;

    public GetPreprocessedImageQueryHandler(IImageManipulationService imageManipulationService,
        ITireDetectionService tireDetectionService, ITextDetectionFacade textDetectionFacade,
        IContentTypeResolverService contentTypeResolverService, ILogger<GetPreprocessedImageQueryHandler> logger)
    {
        _imageManipulationService = imageManipulationService;
        _tireDetectionService = tireDetectionService;
        _textDetectionFacade = textDetectionFacade;
        _contentTypeResolverService = contentTypeResolverService;
        _logger = logger;
    }

    public async Task<DataResult<PreprocessedImageDto>> Handle(
        GetPreprocessedImageQuery request,
        CancellationToken cancellationToken
    )
    {
        var contentTypeSupported = _contentTypeResolverService.IsContentTypeSupported(request.OriginalContentType);
        if (!contentTypeSupported)
            return DataResult<PreprocessedImageDto>.Invalid(
                $"Content type {request.OriginalContentType} is not supported");

        var originalSize = _imageManipulationService.GetRawImageSize(request.ImageData);
        var image = new Image(request.ImageData, request.ImageName, originalSize);

        double tireDetectionSeconds = 0;
        Image? fallbackImage = null;
        Image? unwrappedImage = null;
        try
        {
            var resized = _imageManipulationService.ResizeToMaxSideSize(image, 2048);
            var withClahe = _imageManipulationService.ApplyClahe(
                resized,
                windowSize: new ImageSize(10, 10)
            );
            fallbackImage = withClahe;

            var detectedTireResult = await _tireDetectionService.DetectTireRimCircle(withClahe);
            if (detectedTireResult.IsFailure)
                // return DataResult<PreprocessedImageDto>.Failure(detectedTireResult.Failures);
                throw new Exception("Failed to detect tire rim");

            var detectedTire = detectedTireResult.Data!;
            tireDetectionSeconds = detectedTire.TimeTaken.TotalSeconds;
            var unwrapped = GetUnwrappedTireStrip(withClahe, detectedTire.RimCircle);
            var extended = _imageManipulationService.CopyAndAppendImagePortionFromLeft(unwrapped, 0.17);
            unwrappedImage = extended;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Exception during preprocessing: {ex.StackTrace}");
        }

        if (unwrappedImage is null)
            return HandleUnsuccessfulPreprocessing(request.OriginalContentType, fallbackImage);

        var textArea = await _textDetectionFacade.GetTextAreaFromImageAsync(unwrappedImage);
        return textArea.Map(
            onFailure: failures => HandleUnsuccessfulPreprocessing(
                request.OriginalContentType,
                fallbackImage,
                failures
            ),
            onSuccess: res =>
            {
                var textDetectionSeconds = res.TimeTaken.TotalSeconds;
                _logger.LogInformation(
                    $"Preprocessing completed:\nTire detection: {tireDetectionSeconds}s\nText detection: {textDetectionSeconds}s");

                var resultDto = new PreprocessedImageDto(
                    res.BestImage.Name,
                    res.BestImage.Data,
                    request.OriginalContentType
                );
                return DataResult<PreprocessedImageDto>.Success(resultDto);
            }
        );
    }

    private Image GetUnwrappedTireStrip(Image image, CircleInImage rimCircle)
    {
        var outerTireRadius = rimCircle.Radius * 1.3;
        var innerTireRadius = rimCircle.Radius * 0.9;
        return _imageManipulationService.UnwrapRingIntoRectangle(image,
            rimCircle.Center,
            innerTireRadius,
            outerTireRadius
        );
    }

    private DataResult<PreprocessedImageDto> HandleUnsuccessfulPreprocessing(string contentType, Image? image = null,
        Failure[]? failures = null)
    {
        if (image == null)
        {
            _logger.LogError("Preprocessing failed with no image to return");
            return DataResult<PreprocessedImageDto>.Failure(failures ??
            [
                new Failure(500, "Fatal failure during preprocessing")
            ]);
        }

        var resultDto = new PreprocessedImageDto(
            image.Name,
            image.Data,
            contentType
        );
        return DataResult<PreprocessedImageDto>.Success(resultDto);
    }
}