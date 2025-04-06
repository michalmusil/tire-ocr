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
    private readonly IContentTypeResolver _contentTypeResolver;
    private ILogger<GetPreprocessedImageQueryHandler> _logger;

    public GetPreprocessedImageQueryHandler(IImageManipulationService imageManipulationService,
        ITireDetectionService tireDetectionService, ITextDetectionFacade textDetectionFacade,
        IContentTypeResolver contentTypeResolver, ILogger<GetPreprocessedImageQueryHandler> logger)
    {
        _imageManipulationService = imageManipulationService;
        _tireDetectionService = tireDetectionService;
        _textDetectionFacade = textDetectionFacade;
        _contentTypeResolver = contentTypeResolver;
        _logger = logger;
    }

    public async Task<DataResult<PreprocessedImageDto>> Handle(
        GetPreprocessedImageQuery request,
        CancellationToken cancellationToken
    )
    {
        var contentTypeSupported = _contentTypeResolver.IsContentTypeSupported(request.OriginalContentType);
        if (!contentTypeSupported)
            return DataResult<PreprocessedImageDto>.Invalid(
                $"Content type {request.OriginalContentType} is not supported");

        var originalSize = _imageManipulationService.GetRawImageSize(request.ImageData);
        var image = new Image(request.ImageData, request.ImageName, originalSize);

        double tireDetectionSeconds;
        Image imageToProcess;
        try
        {
            var resized = _imageManipulationService.ResizeToMaxSideSize(image, 2048);
            var withClahe = _imageManipulationService.ApplyClahe(
                resized,
                windowSize: new ImageSize(10, 10)
            );

            var detectedTireResult = await _tireDetectionService.DetectTireRimCircle(withClahe);
            if (detectedTireResult.IsFailure)
                return DataResult<PreprocessedImageDto>.Failure(detectedTireResult.Failures);

            var detectedTire = detectedTireResult.Data!;
            tireDetectionSeconds = detectedTire.TimeTaken.TotalSeconds;
            var unwrapped = GetUnwrappedTireStrip(withClahe, detectedTire.RimCircle);
            var extended = _imageManipulationService.CopyAndAppendImagePortionFromLeft(unwrapped, 0.17);
            imageToProcess = extended;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return DataResult<PreprocessedImageDto>.Failure(
                new Failure(500, "Fundamental preprocessing steps failed for image")
            );
        }

        // return DataResult<PreprocessedImageDto>.Success(new PreprocessedImageDto(imageToProcess.Name,
        //     imageToProcess.Data, request.OriginalContentType));

        var textArea = await _textDetectionFacade.GetTextAreaFromImageAsync(imageToProcess);

        return textArea.Map(
            onFailure: failures => DataResult<PreprocessedImageDto>.Failure(failures),
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
}