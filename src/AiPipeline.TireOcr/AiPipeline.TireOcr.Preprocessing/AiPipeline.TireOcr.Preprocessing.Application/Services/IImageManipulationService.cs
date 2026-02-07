using TireOcr.Preprocessing.Domain.Common;
using TireOcr.Preprocessing.Domain.ImageEntity;

namespace TireOcr.Preprocessing.Application.Services;

public interface IImageManipulationService
{
    public ImageSize GetRawImageSize(byte[] rawImage);
    public Image ResizeToMaxSideSize(Image image, int maxSide);
    public Image ApplyBitwiseNot(Image image);
    public Image ApplyGrayscale(Image image);
    public Image ApplyClahe(Image image, double clipLimit = 40.0, ImageSize? windowSize = null);
    public Image ApplyNormalization(Image image, int minValue = 0, int maxValue = 255);
    public Image ApplyBilateralFilter(Image image, int d, double sigmaColor, double sigmaSpace);
    public Image AddImagesWeighted(Image image1, double alpha, Image image2, double beta);

    public Image ApplyGausianBlur(Image image, int kernelWidth = 5, int kernelHeight = 5, int sigmaX = 0,
        int sigmaY = 0);

    public Image ApplyCannyEdgeDetection(Image image, double treshold1 = 100, double treshold2 = 200);
    public Image ApplySobelEdgeDetection(Image image, bool preBlur);
    public Image ApplySharpening(Image image, float strength = 5);

    public Image ApplyAdaptiveTreshold(Image image, int value = 255, int blockSize = 11, double c = 0);
    public Image UnwrapRingIntoRectangle(Image image, ImageCoordinate center, double innerRadius, double outerRadius);
    public Image CopyAndAppendImagePortionFromLeft(Image image, double appendPortionWidthRatio);
    public BoundingBox GetBoundingBoxForTireCodeString(Image image, StringInImage stringInImage);
    public Image ApplyMaskToEverythingExceptBoundingBoxes(Image image, IEnumerable<BoundingBox> boundingBoxes);
    public Image? StackImagesVertically(List<Image> images);
}