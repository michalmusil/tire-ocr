using TireOcr.Preprocessing.Domain.ImageEntity;
using TireOcr.Shared.Result;

namespace TireOcr.Preprocessing.Application.Services;

public interface IImageSlicerService
{
    /// <summary>
    /// Splits an image into slices based on specified slice size and overlap.
    /// The image is sliced horizontally if the sliceSize width is less than image width and vertically if sliceSize
    /// height is less than image height. Each slice has the dimension of the specified sliceSize, or less (the last
    /// slice usually has to be smaller, unless the sliceSize dimension is an equal division of the image dimension).
    ///
    /// The slices also overlap each other according to the specified xOverlapRatio and yOverlapRatio. Each ratio
    /// is between 0 and 1 and specifies a percentage of the sliceSize dimension in which individual slices should
    /// overlap (in a given axis). 
    /// </summary>
    public Task<DataResult<IEnumerable<Image>>> SliceImageNormalOverlap(
        Image image,
        ImageSize sliceSize,
        double xOverlapRatio,
        double yOverlapRatio
    );

    /// <summary>
    /// Splits an image into slices based on specified slice size and overlap.
    /// The image is sliced horizontally if the sliceSize width is less than image width and vertically if sliceSize
    /// height is less than image height. Each slice has the dimension of the specified sliceSize, or less (the last
    /// slice usually has to be smaller, unless the sliceSize dimension is an equal division of the image dimension).
    ///
    /// In this method as opposed to the 'SliceImage', the overlap is added to the slices sides - meaning if I for example
    /// provide x overlap of 0.2 and y overlap of 0.1, each slice will be enlarged by 20% of its width on left and right
    /// and 10% of its height on top and bottom. The overlap is not added in case it would overflow the original image
    /// dimension. 
    /// </summary>
    public Task<DataResult<IEnumerable<Image>>> SliceImageAdditiveOverlap(
        Image image,
        ImageSize sliceSize,
        double xOverlapRatio,
        double yOverlapRatio
    );
}