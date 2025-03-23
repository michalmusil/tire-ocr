using TireOcr.Ocr.Domain.ImageEntity;

namespace TireOcr.Ocr.Infrastructure.Services.ImageUtils;

public interface IImageUtils
{
    public string ConvertToBase64(Image image);
}