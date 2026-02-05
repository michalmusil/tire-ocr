using TireOcr.Preprocessing.Application.Dtos;
using TireOcr.Preprocessing.Application.Services;
using TireOcr.Preprocessing.Domain.ImageEntity;
using TireOcr.Shared.Extensions;
using TireOcr.Shared.Result;
using TireOcr.Shared.UseCase;

namespace TireOcr.Preprocessing.Application.Commands.ResizeImage;

public class ResizeImageCommandHandler : ICommandHandler<ResizeImageCommand, PreprocessedImageDto>
{
    private readonly IImageManipulationService _imageManipulationService;
    private readonly IContentTypeResolverService _contentTypeResolverService;

    public ResizeImageCommandHandler(IImageManipulationService imageManipulationService,
        IContentTypeResolverService contentTypeResolverService)
    {
        _imageManipulationService = imageManipulationService;
        _contentTypeResolverService = contentTypeResolverService;
    }

    public async Task<DataResult<PreprocessedImageDto>> Handle(
        ResizeImageCommand request,
        CancellationToken cancellationToken
    )
    {
        var resizeResult = await PerformanceUtils.PerformTimeMeasuredTask(
            runTask: () => PerformResize(request)
        );
        var timeTaken = resizeResult.Item1;
        var imageResult = resizeResult.Item2;

        return imageResult.Map(
            onSuccess: img =>
            {
                var resultDto = new PreprocessedImageDto(
                    img.Name,
                    img.Data,
                    request.OriginalContentType,
                    (long)timeTaken.TotalMilliseconds
                );

                return DataResult<PreprocessedImageDto>.Success(resultDto);
            },
            onFailure: DataResult<PreprocessedImageDto>.Failure
        );
    }

    private async Task<DataResult<Image>> PerformResize(ResizeImageCommand request)
    {
        var contentTypeSupported = _contentTypeResolverService.IsContentTypeSupported(request.OriginalContentType);
        if (!contentTypeSupported)
            return DataResult<Image>.Invalid(
                $"Content type {request.OriginalContentType} is not supported");

        var originalSize = _imageManipulationService.GetRawImageSize(request.ImageData);
        var image = new Image(request.ImageData, request.ImageName, originalSize);
        try
        {
            var resized = _imageManipulationService.ResizeToMaxSideSize(image, request.MaxImageSideDimension);
            return DataResult<Image>.Success(resized);
        }
        catch
        {
            return DataResult<Image>.Failure(new Failure(500, "Failed to resize image"));
        }
    }
}