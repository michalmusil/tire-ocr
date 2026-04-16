using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using OpenCvSharp;
using TireOcr.Preprocessing.Application.Services;
using TireOcr.Preprocessing.Domain.ImageEntity;
using TireOcr.Preprocessing.Infrastructure.Extensions;
using TireOcr.Preprocessing.Infrastructure.Models;
using TireOcr.Preprocessing.Infrastructure.Services.ModelResolver;
using TireOcr.Shared.Result;

namespace TireOcr.Preprocessing.Infrastructure.Services;

public class CharacterEnhancementService : ICharacterEnhancementService
{
    private const int TargetWidth = 512;
    private const int TargetHeight = 128;

    private readonly IMlModelResolverService _modelResolverService;

    public CharacterEnhancementService(IMlModelResolverService modelResolverService)
    {
        _modelResolverService = modelResolverService;
    }

    public async Task<DataResult<Image>> EnhanceCharactersAsync(Image image)
    {
        var modelResult = await _modelResolverService.Resolve<ICharacterEnhancementService>();
        if (modelResult.IsFailure)
            return DataResult<Image>.Failure(
                new Failure(500, modelResult.PrimaryFailure?.Message ?? "Failed to resolve ML model")
            );

        var model = modelResult.Data!;
        using var src = Cv2.ImDecode(image.Data, ImreadModes.Grayscale);
        var originalSize = src.Size();

        using var letterboxedInput = LetterboxResizeGray(src, TargetWidth, TargetHeight);

        using var session = new InferenceSession(model.GetMainFilePath());
        var inputName = session.InputMetadata.Keys.First();
        var tensor = ExtractAndNormalize(letterboxedInput.Image, TargetWidth, TargetHeight);
        var inputs = new List<NamedOnnxValue> { NamedOnnxValue.CreateFromTensor(inputName, tensor) };

        using var results = session.Run(inputs);
        var output = results[0].AsTensor<float>();

        using var mask = ExtractMaskFromModelOutput(output, TargetHeight, TargetWidth, letterboxedInput);
        using var resizedMask = new Mat();
        Cv2.Resize(mask, resizedMask, originalSize, 0, 0, InterpolationFlags.Nearest);
        using var appliedMask = ApplyBlendedEdgeEnhancement(resizedMask, src);

        var resultData = appliedMask.ToBytes(".png");
        var resultImage = new Image(
            resultData,
            image.Name,
            image.Size
        );
        return DataResult<Image>.Success(resultImage);
    }

    private LetterboxedImage LetterboxResizeGray(Mat image, int targetWidth, int targetHeight)
    {
        var scale = Math.Min((double)targetWidth / image.Width, (double)targetHeight / image.Height);
        var newWidth = (int)(image.Width * scale);
        var newHeight = (int)(image.Height * scale);

        using var resized = new Mat();
        Cv2.Resize(image, resized, new Size(newWidth, newHeight), 0, 0, InterpolationFlags.Area);

        var canvas = new Mat(new Size(targetWidth, targetHeight), MatType.CV_8UC1, Scalar.Black);
        var offsetX = (targetWidth - newWidth) / 2;
        var offsetY = (targetHeight - newHeight) / 2;

        var roi = new Mat(canvas, new Rect(offsetX, offsetY, newWidth, newHeight));
        resized.CopyTo(roi);

        return new LetterboxedImage(
            Image: canvas,
            ImageUpscaledWidth: newWidth,
            ImageUpscaledHeight: newHeight,
            OffsetX: offsetX,
            OffsetY: offsetY
        );
    }

    private DenseTensor<float> ExtractAndNormalize(Mat mat, int targetWidth, int targetHeight)
    {
        var tensor = new DenseTensor<float>(new[] { 1, 1, targetHeight, targetWidth });
        mat.ConvertTo(mat, MatType.CV_32F, 1.0 / 255.0);

        for (var y = 0; y < targetHeight; y++)
        {
            for (var x = 0; x < targetWidth; x++)
            {
                tensor[0, 0, y, x] = mat.At<float>(y, x);
            }
        }

        return tensor;
    }

    private Mat ExtractMaskFromModelOutput(Tensor<float> output, int targetHeight, int targetWidth,
        LetterboxedImage letterboxedImage)
    {
        using var fullMask = new Mat(targetHeight, targetWidth, MatType.CV_32F);
        for (var y = 0; y < targetHeight; y++)
        {
            for (var x = 0; x < targetWidth; x++)
            {
                fullMask.Set(y, x, output[0, 0, y, x]);
            }
        }

        using var binary = new Mat();
        Cv2.Threshold(fullMask, binary, 0.01, 255, ThresholdTypes.Binary);
        binary.ConvertTo(binary, MatType.CV_8U);

        var cropRect = new Rect(
            letterboxedImage.OffsetX,
            letterboxedImage.OffsetY,
            letterboxedImage.ImageUpscaledWidth,
            letterboxedImage.ImageUpscaledHeight
        );
        return new Mat(binary, cropRect).Clone();
    }

    private Mat ApplyMaskToOriginalImage(Mat mask, Mat originalImage, double maskAlpha)
    {
        using var colorSrc = new Mat();
        Cv2.CvtColor(originalImage, colorSrc, ColorConversionCodes.GRAY2BGR);
        using var greenLayer = new Mat(colorSrc.Size(), colorSrc.Type(), new Scalar(0, 255, 0));
        using var overlay = new Mat();
        Cv2.AddWeighted(colorSrc, 1 - maskAlpha, greenLayer, maskAlpha, 0, overlay);
        var applied = colorSrc.Clone();
        overlay.CopyTo(applied, mask);
        return applied;
    }

    private Mat ApplyBlendedEdgeEnhancement(Mat mask, Mat originalImage)
    {
        // Get edges using Sobel
        using var blurred = new Mat();
        Cv2.GaussianBlur(originalImage, blurred, new Size(3, 3), 0);

        using var gradX = new Mat();
        using var gradY = new Mat();
        Cv2.Sobel(blurred, gradX, MatType.CV_16S, 1, 0, ksize: 3);
        Cv2.Sobel(blurred, gradY, MatType.CV_16S, 0, 1, ksize: 3);

        using var absX = new Mat();
        using var absY = new Mat();
        Cv2.ConvertScaleAbs(gradX, absX);
        Cv2.ConvertScaleAbs(gradY, absY);

        using var edges = new Mat();
        Cv2.AddWeighted(absX, 0.8, absY, 0.8, 0, edges);

        // Blur the mask edges
        using var softMask = new Mat();
        Cv2.GaussianBlur(mask, softMask, new Size(15, 15), 0);
        softMask.ConvertTo(softMask, MatType.CV_32F, 1.0 / 255.0);

        // Add detected edges to original image
        using var sharpened = new Mat();
        Cv2.AddWeighted(originalImage, 0.2, edges, 0.8, 0, sharpened);

        // Apply highlighted edges only in the area of mask (sharpened * mask + original * (1 - Mask))
        var result = new Mat(originalImage.Size(), originalImage.Type());
        Cv2.Multiply(sharpened, softMask, sharpened, 1.0, MatType.CV_8U);
        Cv2.Multiply(originalImage, Scalar.All(1.0) - softMask, originalImage, 1.0, MatType.CV_8U);
        Cv2.Add(sharpened, originalImage, result);

        return result;
    }
}