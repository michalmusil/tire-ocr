using Microsoft.Extensions.Options;
using TireOcr.Preprocessing.Application.Options;
using TireOcr.Preprocessing.Application.Services;
using TireOcr.Preprocessing.Domain.Common;
using TireOcr.Preprocessing.Domain.ImageEntity;

namespace TireOcr.Preprocessing.Infrastructure.Services;

public class TireSidewallExtractionService : ITireSidewallExtractionService
{
    private readonly IImageManipulationService _imageManipulationService;
    private readonly ImageProcessingOptions _imageProcessingOptions;

    public TireSidewallExtractionService(IImageManipulationService imageManipulationService,
        IOptions<ImageProcessingOptions> imageProcessingOptions)
    {
        _imageManipulationService = imageManipulationService;
        _imageProcessingOptions = imageProcessingOptions.Value;
    }

    public Task<Image> ExtractSidewallStripAroundRimCircle(Image image, CircleInImage rimCircle) => Task.FromResult(
        _imageManipulationService.UnwrapRingIntoRectangle(image,
            rimCircle.Center,
            rimCircle.Radius * _imageProcessingOptions.TireInnerRadiusRatio,
            rimCircle.Radius * _imageProcessingOptions.TireOuterRadiusRatio
        )
    );
}