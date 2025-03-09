using TireOcr.Preprocessing.Domain.ImageEntity;
using TireOcr.Shared.Result;

namespace TireOcr.Preprocessing.Application.Services;

public interface IImageSlicer
{
    public Task<DataResult<IEnumerable<Image>>> SliceImage(
        Image image,
        ImageSize sliceSize,
        double xOverlapRatio,
        double yOverlapRatio
    );
}