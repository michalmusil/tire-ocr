using TireOcr.Preprocessing.Application.Dtos;
using TireOcr.Shared.UseCase;

namespace TireOcr.Preprocessing.Application.Commands.ResizeImage;

public record ResizeImageCommand(
    byte[] ImageData,
    string ImageName,
    string OriginalContentType,
    int MaxImageSideDimension) : ICommand<PreprocessedImageDto>;