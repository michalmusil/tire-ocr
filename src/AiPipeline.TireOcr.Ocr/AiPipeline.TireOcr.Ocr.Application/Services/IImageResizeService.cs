using TireOcr.Ocr.Application.Dtos;
using TireOcr.Ocr.Domain.ImageEntity;

namespace TireOcr.Ocr.Application.Services;

public interface IImageResizeService
{
    public Task<Image> Resize(Image image, ResizeImageOptions resizeOptions);
    public Task<Image> ResizeToRespectMaxSize(Image image, ResizeImageToMaxSideOptions resizeOptions);
}