using TireOcr.Preprocessing.Application.Dtos;
using TireOcr.Shared.UseCase;

namespace TireOcr.Preprocessing.Application.Queries.GetPreprocessedImage;

public record GetPreprocessedImageQuery(byte[] ImageData, string ImageName): IQuery<PreprocessedImageDto>;