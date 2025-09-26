using TireOcr.Ocr.Application.Dtos;
using TireOcr.Ocr.Application.Services;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace TireOcr.Ocr.Infrastructure.Services;

public class ImageResizeService : IImageResizeService
{
    public async Task<Domain.ImageEntity.Image> Resize(Domain.ImageEntity.Image image, ResizeImageOptions resizeOptions)
    {
        using var inputStream = new MemoryStream(image.Data);
        using var inputImage = await Image.LoadAsync(inputStream);
        using var outputStream = new MemoryStream();

        inputImage.Mutate(x => x.Resize(resizeOptions.Width, resizeOptions.Height));
        await inputImage.SaveAsPngAsync(outputStream);

        var resizedImage = new Domain.ImageEntity.Image(outputStream.ToArray(), image.Name, image.ContentType);
        return resizedImage;
    }

    public async Task<Domain.ImageEntity.Image> ResizeToRespectMaxSize(Domain.ImageEntity.Image image,
        ResizeImageToMaxSideOptions resizeOptions)
    {
        using var inputStream = new MemoryStream(image.Data);
        using var inputImage = await Image.LoadAsync(inputStream);

        var maxSide = Math.Max(inputImage.Width, inputImage.Height);
        if (maxSide <= resizeOptions.MaxSide)
            return image;

        var newWidth = 0;
        var newHeight = 0;
        if (inputImage.Width >= inputImage.Height)
            newWidth = resizeOptions.MaxSide;
        else
            newHeight = resizeOptions.MaxSide;

        using var outputStream = new MemoryStream();
        // If one of width or height is set to 0, ImageSharp automatically preserves aspect ratio
        inputImage.Mutate(x => x.Resize(newWidth, newHeight));
        await inputImage.SaveAsJpegAsync(outputStream);
        var resizedImage = new Domain.ImageEntity.Image(outputStream.ToArray(), image.Name, image.ContentType);
        return resizedImage;
    }
}