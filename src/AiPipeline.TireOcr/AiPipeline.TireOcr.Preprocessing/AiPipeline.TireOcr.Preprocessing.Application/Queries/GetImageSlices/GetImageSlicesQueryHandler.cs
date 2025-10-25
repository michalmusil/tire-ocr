using Microsoft.Extensions.Logging;
using TireOcr.Preprocessing.Application.Dtos;
using TireOcr.Preprocessing.Application.Services;
using TireOcr.Preprocessing.Domain.Common;
using TireOcr.Preprocessing.Domain.ImageEntity;
using TireOcr.Shared.Extensions;
using TireOcr.Shared.Result;
using TireOcr.Shared.UseCase;

namespace TireOcr.Preprocessing.Application.Queries.GetImageSlices;

public class GetImageSlicesQueryHandler : IQueryHandler<GetImageSlicesQuery, PreprocessedImageDto>
{
    private readonly IImageManipulationService _imageManipulationService;
    private readonly ITireDetectionService _tireDetectionService;
    private readonly IContentTypeResolverService _contentTypeResolverService;
    private readonly IImageSlicerService _imageSlicerService;
    private readonly ILogger<GetImageSlicesQueryHandler> _logger;

    public GetImageSlicesQueryHandler(IImageManipulationService imageManipulationService,
        ITireDetectionService tireDetectionService, IContentTypeResolverService contentTypeResolverService,
        IImageSlicerService imageSlicerService, ILogger<GetImageSlicesQueryHandler> logger)
    {
        _imageManipulationService = imageManipulationService;
        _tireDetectionService = tireDetectionService;
        _contentTypeResolverService = contentTypeResolverService;
        _imageSlicerService = imageSlicerService;
        _logger = logger;
    }

    public async Task<DataResult<PreprocessedImageDto>> Handle(
        GetImageSlicesQuery request,
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

    private async Task<DataResult<Image>> PerformSlicingAndComposition(GetImageSlicesQuery request)
    {
        var contentTypeSupported = _contentTypeResolverService.IsContentTypeSupported(request.OriginalContentType);
        if (!contentTypeSupported)
            return DataResult<Image>.Invalid(
                $"Content type {request.OriginalContentType} is not supported");

        var originalSize = _imageManipulationService.GetRawImageSize(request.ImageData);
        var image = new Image(request.ImageData, request.ImageName, originalSize);

        var resized = _imageManipulationService.ResizeToMaxSideSize(image, 2048);
        var withClahe = _imageManipulationService.ApplyClahe(
            resized,
            windowSize: new ImageSize(10, 10)
        );

        var detectedTireResult = await _tireDetectionService.DetectTireRimCircle(withClahe);
        if (detectedTireResult.IsFailure)
            return DataResult<Image>.Failure(detectedTireResult.Failures);

        var detectedTire = detectedTireResult.Data!;
        var unwrapped = GetUnwrappedTireStrip(withClahe, detectedTire.RimCircle);
        var unwrappedExtended = _imageManipulationService.CopyAndAppendImagePortionFromLeft(unwrapped, 0.17);

        var sliceSize = new ImageSize(
            height: unwrappedExtended.Size.Height,
            width: (int)Math.Ceiling((decimal)unwrappedExtended.Size.Width / (decimal)request.NumberOfSlices)
        );

        var slicesResult = await _imageSlicerService.SliceImageAdditiveOverlap(
            image: unwrappedExtended,
            sliceSize: sliceSize,
            xOverlapRatio: 0.15,
            yOverlapRatio: 0
        );
        if (slicesResult.IsFailure)
            return DataResult<Image>.Failure(slicesResult.Failures);

        var slices = slicesResult.Data!.ToList();
        if (!slices.Any())
            return DataResult<Image>.Failure(new Failure(500, "Failed to generate image slices."));

        var stackedImage = _imageManipulationService.StackImagesVertically(slices);
        if (stackedImage == null)
            return DataResult<Image>.Failure(
                new Failure(500, "Failed to compose generated slices vertically.")
            );

        var finalImage = _imageManipulationService.ApplyClahe(stackedImage);
        finalImage = _imageManipulationService.ApplyBilateralFilter(finalImage, d: 5, sigmaColor: 40, sigmaSpace: 40);
        finalImage = _imageManipulationService.ApplyBitwiseNot(finalImage);


        return DataResult<Image>.Success(finalImage);
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