using TireOcr.Preprocessing.Application.Dtos;
using TireOcr.Shared.UseCase;

namespace TireOcr.Preprocessing.Application.Queries.GetResizedImage;

public record GetResizedImageQuery(byte[] ImageData, string ImageName, string OriginalContentType, int MaxImageSideDimension): IQuery<PreprocessedImageDto>;