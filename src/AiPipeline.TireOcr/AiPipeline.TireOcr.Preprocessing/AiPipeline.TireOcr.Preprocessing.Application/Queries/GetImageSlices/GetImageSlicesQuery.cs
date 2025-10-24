using TireOcr.Preprocessing.Application.Dtos;
using TireOcr.Shared.UseCase;

namespace TireOcr.Preprocessing.Application.Queries.GetImageSlices;

public record GetImageSlicesQuery(byte[] ImageData, string ImageName, string OriginalContentType, int NumberOfSlices)
    : IQuery<ImageSlicesResultDto>;