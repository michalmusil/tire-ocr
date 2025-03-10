using TireOcr.Preprocessing.Application.Dtos;
using TireOcr.Preprocessing.Domain.Common;
using TireOcr.Preprocessing.Domain.ImageEntity;
using TireOcr.Shared.Result;

namespace TireOcr.Preprocessing.Application.Services;

public interface ITireDetectionService
{
    public Task<DataResult<TireDetectionResult>> DetectTireRimCircle(Image image);
}