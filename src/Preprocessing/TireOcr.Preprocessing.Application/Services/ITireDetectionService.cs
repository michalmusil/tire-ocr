using TireOcr.Preprocessing.Domain.Common;
using TireOcr.Preprocessing.Domain.ImageEntity;
using TireOcr.Shared.Result;

namespace TireOcr.Preprocessing.Application.Services;

public interface ITireDetectionService
{
    public Task<DataResult<CircleInImage>> DetectTireRimCircle(Image image);
}