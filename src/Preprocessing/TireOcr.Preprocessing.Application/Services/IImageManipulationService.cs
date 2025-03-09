using TireOcr.Preprocessing.Domain.Common;
using TireOcr.Preprocessing.Domain.ImageEntity;

namespace TireOcr.Preprocessing.Application.Services;

public interface IImageManipulationService
{
    public ImageSize GetRawImageSize(byte[] rawImage);
    public Image ResizeToMaxSideSize(Image image, int maxSide);
    public Image ApplyGrayscale(Image image);
    public Image ApplyClahe(Image image, double clipLimit = 40.0, ImageSize? windowSize = null);
    public Image UnwrapRingIntoRectangle(Image image, ImageCoordinate center, double innerRadius, double outerRadius);
}