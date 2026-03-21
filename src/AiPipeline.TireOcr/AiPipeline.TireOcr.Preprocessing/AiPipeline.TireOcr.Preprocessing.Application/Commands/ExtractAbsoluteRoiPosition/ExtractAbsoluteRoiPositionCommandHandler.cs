using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TireOcr.Preprocessing.Application.Dtos;
using TireOcr.Preprocessing.Application.Facades;
using TireOcr.Preprocessing.Application.Options;
using TireOcr.Preprocessing.Application.Services;
using TireOcr.Preprocessing.Domain.ImageEntity;
using TireOcr.Shared.Extensions;
using TireOcr.Shared.Result;
using TireOcr.Shared.UseCase;

namespace TireOcr.Preprocessing.Application.Commands.ExtractAbsoluteRoiPosition;

public class ExtractAbsoluteRoiPositionCommandHandler
    : ICommandHandler<ExtractAbsoluteRoiPositionCommand, PreprocessedImageDto>
{
    private readonly IImageManipulationService _imageManipulationService;
    private readonly ITireDetectionService _tireDetectionService;
    private readonly IRoiExtractionFacade _roiExtractionFacade;
    private readonly IContentTypeResolverService _contentTypeResolverService;
    private readonly ITireSidewallExtractionService _tireSidewallExtractionService;
    private readonly ImageProcessingOptions _imageProcessingOptions;
    private readonly ILogger<ExtractAbsoluteRoiPositionCommandHandler> _logger;

    public ExtractAbsoluteRoiPositionCommandHandler(
        IImageManipulationService imageManipulationService,
        ITireDetectionService tireDetectionService,
        IRoiExtractionFacade roiExtractionFacade,
        IContentTypeResolverService contentTypeResolverService,
        ITireSidewallExtractionService tireSidewallExtractionService,
        IOptions<ImageProcessingOptions> imageProcessingOptions,
        ILogger<ExtractAbsoluteRoiPositionCommandHandler> logger)
    {
        _imageManipulationService = imageManipulationService;
        _tireDetectionService = tireDetectionService;
        _roiExtractionFacade = roiExtractionFacade;
        _contentTypeResolverService = contentTypeResolverService;
        _tireSidewallExtractionService = tireSidewallExtractionService;
        _imageProcessingOptions = imageProcessingOptions.Value;
        _logger = logger;
    }

    public async Task<DataResult<PreprocessedImageDto>> Handle(
        ExtractAbsoluteRoiPositionCommand request,
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

    private async Task<DataResult<Image>> PerformPreprocessing(ExtractAbsoluteRoiPositionCommand request)
    {
        var contentTypeSupported = _contentTypeResolverService.IsContentTypeSupported(request.OriginalContentType);
        if (!contentTypeSupported)
            return DataResult<Image>.Invalid(
                $"Content type {request.OriginalContentType} is not supported");

        var originalSize = _imageManipulationService.GetRawImageSize(request.ImageData);
        var image = new Image(request.ImageData, request.ImageName, originalSize);

        // Prevent enormous images
        var processedImage = _imageManipulationService
            .ResizeToMaxSideSize(image, _imageProcessingOptions.MaxInputImageSize);

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
                    var fallbackImage = ApplyGlobalImageAdjustments(processedImage, true);
                    return DataResult<Image>.Success(fallbackImage);
                default:
                    _logger.LogError(
                        $"Rim detected failed fatally for '{request.ImageName}'.\nReason:'{failure.Message}'\nPreprocessing failed."
                    );
                    return DataResult<Image>.Failure(failure);
            }
        }

        // Unwrapping only the tire sidewall portion of the image into a long strip and appending overlap manually from left side to right 
        var detectedTire = detectedTireResult.Data!;
        var sliceImage = await _tireSidewallExtractionService
            .ExtractSidewallStripAroundRimCircle(processedImage, detectedTire.RimCircle);
        sliceImage = _imageManipulationService.CopyAndAppendImagePortionFromLeft(
            sliceImage,
            _imageProcessingOptions.TireStripProlongWidthRatio
        );

        // Applying more processing to improve contrast
        sliceImage = ApplyGlobalImageAdjustments(sliceImage, false);

        // Performing the roi extraction
        var roiExtractionResult = await _roiExtractionFacade.ExtractAbsoluteTireCodeRoi(sliceImage);

        return roiExtractionResult.Map(
            onSuccess: res =>
            {
                var finalImage = _imageManipulationService
                    .ResizeToMaxSideSize(res.BestImage, _imageProcessingOptions.MaxOutputImageSize);
                return DataResult<Image>.Success(finalImage);
            },
            onFailure: failures =>
            {
                _logger.LogError(
                    $"Absolute ROI extraction failed for '{request.ImageName}'.\nReason:'{failures.FirstOrDefault()?.Message ?? ""}'\nReturning fallback image version.");
                var fallbackImage = ApplyGlobalImageAdjustments(processedImage, true);
                return DataResult<Image>.Success(fallbackImage);
            }
        );
    }

    private Image ApplyGlobalImageAdjustments(Image processedImage, bool resizeToMaxOutputSize)
    {
        // Applying more processing to improve contrast
        var adjustedImage = _imageManipulationService.ApplyClahe(processedImage);
        adjustedImage =
            _imageManipulationService.ApplyBilateralFilter(adjustedImage, d: 5, sigmaColor: 40, sigmaSpace: 40);
        adjustedImage = _imageManipulationService.ApplyBitwiseNot(adjustedImage);

        // Reduce image size
        if (resizeToMaxOutputSize)
            adjustedImage = _imageManipulationService
                .ResizeToMaxSideSize(adjustedImage, _imageProcessingOptions.MaxOutputImageSize);

        return adjustedImage;
    }
}