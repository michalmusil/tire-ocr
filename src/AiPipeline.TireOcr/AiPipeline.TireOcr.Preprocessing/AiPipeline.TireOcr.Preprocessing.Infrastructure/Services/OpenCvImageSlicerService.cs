using OpenCvSharp;
using TireOcr.Preprocessing.Application.Services;
using TireOcr.Preprocessing.Domain.ImageEntity;
using TireOcr.Preprocessing.Infrastructure.Extensions;
using TireOcr.Shared.Result;

namespace TireOcr.Preprocessing.Infrastructure.Services;

public class OpenCvImageSlicerService : IImageSlicerService
{
    public async Task<DataResult<IEnumerable<Image>>> SliceImageNormalOverlap(
        Image image,
        ImageSize sliceSize,
        double xOverlapRatio,
        double yOverlapRatio
    )
    {
        try
        {
            using var inputImage = image.ToCv2();
            var height = inputImage.Height;
            var width = inputImage.Width;

            var realSliceWidth = sliceSize.Width;
            var realSliceHeight = sliceSize.Height;

            var overlapWidth = xOverlapRatio * realSliceWidth;
            var overlapHeight = yOverlapRatio * realSliceHeight;

            var logicalSliceWidth = realSliceWidth - overlapWidth;
            var logicalSliceHeight = realSliceHeight - overlapHeight;

            var startingXs = GenerateRange(0, width, logicalSliceWidth);
            var startingYs = GenerateRange(0, height, logicalSliceHeight);

            var slices = new List<Mat>();
            foreach (var y in startingYs)
            {
                foreach (var x in startingXs)
                {
                    var xmin = (int)x;
                    var ymin = (int)y;
                    var xmax = (int)Math.Min(x + realSliceWidth, width);
                    var ymax = (int)Math.Min(y + realSliceHeight, height);

                    var rect = new Rect(xmin, ymin, xmax - xmin, ymax - ymin);
                    slices.Add(new Mat(inputImage, rect));
                }
            }

            var resultImages = new List<Image>();
            for (var i = 0; i < slices.Count; i++)
            {
                var slice = slices[i];
                var imageName = $"{i + 1}_{image.Name}";
                resultImages.Add(slice.ToDomain(imageName));

                slice.Dispose();
            }

            return DataResult<IEnumerable<Image>>.Success(resultImages);
        }
        catch (Exception ex)
        {
            return DataResult<IEnumerable<Image>>.Failure(new Failure(500, "Failed to slice image."));
        }
    }

    public async Task<DataResult<IEnumerable<Image>>> SliceImageAdditiveOverlap(Image image, ImageSize sliceSize,
        double xOverlapRatio, double yOverlapRatio)
    {
        try
        {
            using var inputImage = image.ToCv2();
            var height = inputImage.Height;
            var width = inputImage.Width;

            var realSliceWidth = sliceSize.Width;
            var realSliceHeight = sliceSize.Height;

            var overlapWidth = xOverlapRatio * realSliceWidth;
            var overlapHeight = yOverlapRatio * realSliceHeight;

            var startingXs = GenerateRange(0, width, realSliceWidth);
            var startingYs = GenerateRange(0, height, realSliceHeight);

            var slices = new List<Mat>();
            foreach (var y in startingYs)
            {
                foreach (var x in startingXs)
                {
                    var xmin = (int)Math.Max(0, x - overlapWidth);
                    var ymin = (int)Math.Max(0, y - overlapHeight);
                    var xmax = (int)Math.Min(x + realSliceWidth + overlapWidth, width);
                    var ymax = (int)Math.Min(y + realSliceHeight + overlapHeight, height);

                    var rect = new Rect(xmin, ymin, xmax - xmin, ymax - ymin);
                    slices.Add(new Mat(inputImage, rect));
                }
            }

            var resultImages = new List<Image>();
            for (var i = 0; i < slices.Count; i++)
            {
                var slice = slices[i];
                var imageName = $"{i + 1}_{image.Name}";
                resultImages.Add(slice.ToDomain(imageName));

                slice.Dispose();
            }

            return DataResult<IEnumerable<Image>>.Success(resultImages);
        }
        catch (Exception ex)
        {
            return DataResult<IEnumerable<Image>>.Failure(new Failure(500, "Failed to slice image."));
        }
    }

    private static List<double> GenerateRange(double start, double end, double step)
    {
        var range = new List<double>();
        for (double i = start; i < end; i += step)
        {
            range.Add(i);
        }

        return range;
    }
}