using Microsoft.Extensions.Logging;
using TireOcr.Preprocessing.Application.Dtos;
using TireOcr.Preprocessing.Application.Services;
using TireOcr.Preprocessing.Domain.Common;
using TireOcr.Preprocessing.Domain.ImageEntity;
using TireOcr.Shared.Extensions;
using TireOcr.Shared.Result;
using TireOcr.Shared.UseCase;

namespace TireOcr.Preprocessing.Application.Queries.GetImageSlices;

public class GetImageSlicesQueryHandler : IQueryHandler<GetImageSlicesQuery, ImageSlicesResultDto>
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

    public async Task<DataResult<ImageSlicesResultDto>> Handle(
        GetImageSlicesQuery request,
        CancellationToken cancellationToken
    )
    {
        var preprocessingResult = await PerformanceUtils.PerformTimeMeasuredTask(
            runTask: () => PerformSlicing(request)
        );
        var timeTaken = preprocessingResult.Item1;
        var slicesResult = preprocessingResult.Item2;
        if (slicesResult.IsFailure)
            return DataResult<ImageSlicesResultDto>.Failure(preprocessingResult.Item2.Failures);

        var slices = slicesResult.Data!;
        var resultDto = new ImageSlicesResultDto(
            Slices: slices.Select(s =>
                new ImageSliceDto(
                    Name: s.Name,
                    ImageData: s.Data,
                    ContentType: request.OriginalContentType
                )),
            DurationMs: (long)timeTaken.TotalMilliseconds
        );

        return DataResult<ImageSlicesResultDto>.Success(resultDto);
    }

    private async Task<DataResult<IEnumerable<Image>>> PerformSlicing(GetImageSlicesQuery request)
    {
        var contentTypeSupported = _contentTypeResolverService.IsContentTypeSupported(request.OriginalContentType);
        if (!contentTypeSupported)
            return DataResult<IEnumerable<Image>>.Invalid(
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
            return DataResult<IEnumerable<Image>>.Failure(detectedTireResult.Failures);

        var detectedTire = detectedTireResult.Data!;
        var unwrapped = GetUnwrappedTireStrip(withClahe, detectedTire.RimCircle);
        var unwrappedExtended = _imageManipulationService.CopyAndAppendImagePortionFromLeft(unwrapped, 0.17);

        var sliceSize = new ImageSize(
            height: unwrappedExtended.Size.Height,
            width: unwrappedExtended.Size.Width / request.NumberOfSlices
        );

        var slices = await _imageSlicerService.SliceImage(
            image: unwrappedExtended,
            sliceSize: sliceSize,
            xOverlapRatio: 0,
            yOverlapRatio: 0
        );
        return slices;
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