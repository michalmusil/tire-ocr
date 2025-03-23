using TireOcr.Ocr.Domain.ImageEntity;

namespace TireOcr.Ocr.Infrastructure.Services.ImageUtils;

public class ImageUtils : IImageUtils
{
    public string ConvertToBase64(Image image)
    {
        return Convert.ToBase64String(image.Data);
    }
}