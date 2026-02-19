namespace TireOcr.Preprocessing.Application.Options;

public class ImageProcessingOptions
{
    public int MaxInputImageSize { get; set; }
    public int MaxOutputImageSize { get; set; }
    public double TireOuterRadiusRatio { get; set; }
    public double TireInnerRadiusRatio { get; set; }
    public double TireStripProlongWidthRatio { get; set; }
    public double NormSliceWidthPortion { get; set; }
    public double NormSliceOverlapRatio { get; set; }
    public double AdditiveSliceOverlapRatio { get; set; }
}