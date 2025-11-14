using TireOcr.Preprocessing.Application.Services;
using TireOcr.Preprocessing.Domain.Common;
using TireOcr.Preprocessing.Domain.ImageEntity;

namespace TireOcr.Preprocessing.Infrastructure.Services;

public class TireSidewallExtractionService : ITireSidewallExtractionService
{
    private const double OuterSidewallRimRadiusRatio = 1.3;
    private const double InnerSidewallRimRadiusRatio = 0.9;

    private readonly IImageManipulationService _imageManipulationService;

    public TireSidewallExtractionService(IImageManipulationService imageManipulationService)
    {
        _imageManipulationService = imageManipulationService;
    }

    public Task<Image> ExtractSidewallStripAroundRimCircle(Image image, CircleInImage rimCircle) => Task.FromResult(
        _imageManipulationService.UnwrapRingIntoRectangle(image,
            rimCircle.Center,
            rimCircle.Radius * InnerSidewallRimRadiusRatio,
            rimCircle.Radius * OuterSidewallRimRadiusRatio
        )
    );
}