using Microsoft.Extensions.Logging;
using TireOcr.Preprocessing.Application.Dtos;
using TireOcr.Preprocessing.Application.Facades;
using TireOcr.Preprocessing.Application.Services;
using TireOcr.Preprocessing.Domain.ImageEntity;
using TireOcr.Shared.Extensions;
using TireOcr.Shared.Result;
using TireOcr.Shared.UseCase;

namespace TireOcr.Preprocessing.Application.Commands.ExtractTireCodeRoi;

public class ExtractTireCodeRoiCommandHandler : ICommandHandler<ExtractTireCodeRoiCommand, PreprocessedImageDto>
{
    private readonly IImageManipulationService _imageManipulationService;
    private readonly ITireDetectionService _tireDetectionService;
    private readonly ITextDetectionFacade _textDetectionFacade;
    private readonly IContentTypeResolverService _contentTypeResolverService;
    private readonly ITireSidewallExtractionService _tireSidewallExtractionService;
    private readonly ILogger<ExtractTireCodeRoiCommandHandler> _logger;

    public ExtractTireCodeRoiCommandHandler(
        IImageManipulationService imageManipulationService,
        ITireDetectionService tireDetectionService,
        ITextDetectionFacade textDetectionFacade,
        IContentTypeResolverService contentTypeResolverService,
        ITireSidewallExtractionService tireSidewallExtractionService,
        ILogger<ExtractTireCodeRoiCommandHandler> logger
    )
    {
        _imageManipulationService = imageManipulationService;
        _tireDetectionService = tireDetectionService;
        _textDetectionFacade = textDetectionFacade;
        _contentTypeResolverService = contentTypeResolverService;
        _tireSidewallExtractionService = tireSidewallExtractionService;
        _logger = logger;
    }

    public async Task<DataResult<PreprocessedImageDto>> Handle(
        ExtractTireCodeRoiCommand request,
        CancellationToken cancellationToken
    )
    {
        var preprocessingResult = await PerformanceUtils.PerformTimeMeasuredTask(
            runTask: () => PerformPreprocessing(request)
        );
        var timeTaken = preprocessingResult.Item1;
        var preprocessedImageResult = preprocessingResult.Item2;
        if (preprocessedImageResult.IsFailure)
            return DataResult<PreprocessedImageDto>.Failure(preprocessingResult.Item2.Failures);

        var preprocessedImage = preprocessedImageResult.Data!;
        var resultDto = new PreprocessedImageDto(
            preprocessedImage.Name,
            preprocessedImage.Data,
            request.OriginalContentType,
            DurationMs: (long)timeTaken.TotalMilliseconds
        );

        return DataResult<PreprocessedImageDto>.Success(resultDto);
    }

    private async Task<DataResult<Image>> PerformPreprocessing(ExtractTireCodeRoiCommand request)
    {
        var contentTypeSupported = _contentTypeResolverService.IsContentTypeSupported(request.OriginalContentType);
        if (!contentTypeSupported)
            return DataResult<Image>.Invalid(
                $"Content type {request.OriginalContentType} is not supported");

        var originalSize = _imageManipulationService.GetRawImageSize(request.ImageData);
        var image = new Image(request.ImageData, request.ImageName, originalSize);

        // Apply global adjustments to reduce image size and improve contrast
        var processedImage = _imageManipulationService.ResizeToMaxSideSize(image, 2048);
        processedImage = _imageManipulationService.ApplyClahe(
            processedImage,
            windowSize: new ImageSize(10, 10)
        );

        // Attempt to detect tire circle (only successful for photos containing the whole tire)
        var detectedTireResult = await _tireDetectionService.DetectTireRimCircle(processedImage);
        if (detectedTireResult.IsFailure)
        {
            var failure = detectedTireResult.PrimaryFailure!;
            switch (failure.Code)
            {
                // NotFound or Invalid result means that the photo doesn't contain the entire tire. Full photo with global adjustments used as fallback in this case. 
                case 404 or 422:
                    _logger.LogWarning(
                        $"Rim detection failed for '{request.ImageName}'.\nReason:'{failure.Message}'\nReturning fallback image version."
                    );
                    return DataResult<Image>.Success(processedImage);
                default:
                    _logger.LogError(
                        $"Rim detected failed fatally for '{request.ImageName}'.\nReason:'{failure.Message}'\nPreprocessing failed."
                    );
                    return DataResult<Image>.Failure(failure);
            }
        }

        var fallbackImage = processedImage;
        var detectedTire = detectedTireResult.Data!;

        // Unwrapping only the tire sidewall portion of the image into a long strip and appending overlap manually from left side to right 
        processedImage = await _tireSidewallExtractionService
            .ExtractSidewallStripAroundRimCircle(processedImage, detectedTire.RimCircle);
        processedImage = _imageManipulationService.CopyAndAppendImagePortionFromLeft(processedImage, 0.17);

        // Performing the roi extraction
        var roiExtractionResult = request.EnhanceCharacters
            ? await _textDetectionFacade.ExtractTireCodeRoiAndEnhanceCharacters(processedImage)
            : await _textDetectionFacade.ExtractTireCodeRoi(processedImage);

        return roiExtractionResult.Map(
            onSuccess: res => DataResult<Image>.Success(res.BestImage),
            onFailure: failures =>
            {
                _logger.LogError(
                    $"Roi extraction failed for '{request.ImageName}'.\nReason:'{failures.FirstOrDefault()?.Message ?? ""}'\nReturning fallback image version.");
                return DataResult<Image>.Success(fallbackImage);
            }
        );
    }
}