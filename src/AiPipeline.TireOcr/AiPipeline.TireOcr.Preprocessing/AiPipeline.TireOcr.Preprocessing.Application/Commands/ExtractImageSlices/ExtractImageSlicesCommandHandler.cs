using Microsoft.Extensions.Logging;
using TireOcr.Preprocessing.Application.Dtos;
using TireOcr.Preprocessing.Application.Services;
using TireOcr.Preprocessing.Domain.ImageEntity;
using TireOcr.Shared.Extensions;
using TireOcr.Shared.Result;
using TireOcr.Shared.UseCase;

namespace TireOcr.Preprocessing.Application.Commands.ExtractImageSlices;

public class ExtractImageSlicesCommandHandler : ICommandHandler<ExtractImageSlicesCommand, PreprocessedImageDto>
{
    private readonly IImageManipulationService _imageManipulationService;
    private readonly ITireDetectionService _tireDetectionService;
    private readonly IContentTypeResolverService _contentTypeResolverService;
    private readonly IImageSlicerService _imageSlicerService;
    private readonly ITireSidewallExtractionService _tireSidewallExtractionService;
    private readonly ILogger<ExtractImageSlicesCommandHandler> _logger;

    public ExtractImageSlicesCommandHandler(
        IImageManipulationService imageManipulationService,
        ITireDetectionService tireDetectionService,
        IContentTypeResolverService contentTypeResolverService,
        IImageSlicerService imageSlicerService,
        ITireSidewallExtractionService tireSidewallExtractionService,
        ILogger<ExtractImageSlicesCommandHandler> logger
    )
    {
        _imageManipulationService = imageManipulationService;
        _tireDetectionService = tireDetectionService;
        _contentTypeResolverService = contentTypeResolverService;
        _imageSlicerService = imageSlicerService;
        _tireSidewallExtractionService = tireSidewallExtractionService;
        _logger = logger;
    }

    public async Task<DataResult<PreprocessedImageDto>> Handle(
        ExtractImageSlicesCommand request,
        CancellationToken cancellationToken
    )
    {
        var preprocessingResult = await PerformanceUtils.PerformTimeMeasuredTask(
            runTask: () => PerformSlicingAndComposition(request)
        );
        var timeTaken = preprocessingResult.Item1;
        var slicedCompositionResult = preprocessingResult.Item2;
        if (slicedCompositionResult.IsFailure)
            return DataResult<PreprocessedImageDto>.Failure(slicedCompositionResult.Failures);

        var composedImage = slicedCompositionResult.Data!;

        var result = new PreprocessedImageDto(
            Name: composedImage.Name,
            ContentType: request.OriginalContentType,
            ImageData: composedImage.Data,
            DurationMs: (long)timeTaken.TotalMilliseconds
        );

        return DataResult<PreprocessedImageDto>.Success(result);
    }

    private async Task<DataResult<Image>> PerformSlicingAndComposition(ExtractImageSlicesCommand request)
    {
        var contentTypeSupported = _contentTypeResolverService.IsContentTypeSupported(request.OriginalContentType);
        if (!contentTypeSupported)
            return DataResult<Image>.Invalid(
                $"Content type {request.OriginalContentType} is not supported");

        var originalSize = _imageManipulationService.GetRawImageSize(request.ImageData);
        var processedImage = new Image(request.ImageData, request.ImageName, originalSize);

        // Reduce image size
        processedImage = _imageManipulationService
            .ResizeToMaxSideSize(processedImage, 2048);

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
        var detectedTire = detectedTireResult.Data!;

        // Unwrapping only the tire sidewall portion of the image into a long strip 
        processedImage = await _tireSidewallExtractionService
            .ExtractSidewallStripAroundRimCircle(processedImage, detectedTire.RimCircle);
        processedImage = _imageManipulationService.CopyAndAppendImagePortionFromLeft(processedImage, 0.17);

        // Slicing the strip horizontally to get more acceptable aspect ratio - overlap in slices included
        var sliceWidth = (decimal)processedImage.Size.Width / (decimal)request.NumberOfSlices;
        var sliceSize = new ImageSize(
            height: processedImage.Size.Height,
            width: (int)Math.Ceiling(sliceWidth)
        );
        var slicesResult = await _imageSlicerService.SliceImageAdditiveOverlap(
            image: processedImage,
            sliceSize: sliceSize,
            xOverlapRatio: 0.15,
            yOverlapRatio: 0
        );
        if (slicesResult.IsFailure)
            return DataResult<Image>.Failure(slicesResult.Failures);
        var slices = slicesResult.Data!.ToList();
        if (!slices.Any())
            return DataResult<Image>.Failure(new Failure(500, "Failed to generate image slices."));

        // Stacking slices on top of each other vertically
        var stackedImage = _imageManipulationService.StackImagesVertically(slices);
        if (stackedImage == null)
            return DataResult<Image>.Failure(
                new Failure(500, "Failed to compose generated slices vertically.")
            );

        // Applying more processing to improve contrast
        var finalImage = _imageManipulationService.ApplyClahe(stackedImage);
        finalImage = _imageManipulationService.ApplyBilateralFilter(finalImage, d: 5, sigmaColor: 40, sigmaSpace: 40);
        finalImage = _imageManipulationService.ApplyBitwiseNot(finalImage);

        // If requested, extract only the image edges
        if (request.ExtractEdges)
            finalImage = _imageManipulationService.ApplySobelEdgeDetection(finalImage, preBlur: false);


        return DataResult<Image>.Success(finalImage);
    }
}