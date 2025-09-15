using TireOcr.Preprocessing.Application.Dtos;
using TireOcr.Shared.UseCase;

namespace TireOcr.Preprocessing.Application.Queries.GetTireCodeRoi;

public record GetTireCodeRoiQuery(byte[] ImageData, string ImageName, string OriginalContentType, bool RemoveBackground)
    : IQuery<PreprocessedImageDto>;