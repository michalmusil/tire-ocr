using TireOcr.Ocr.Domain.ImageEntity;

namespace TireOcr.Ocr.Infrastructure.Services.ImageUtils;

public interface IImageConvertorService
{
    public string ConvertToBase64(Image image);
}