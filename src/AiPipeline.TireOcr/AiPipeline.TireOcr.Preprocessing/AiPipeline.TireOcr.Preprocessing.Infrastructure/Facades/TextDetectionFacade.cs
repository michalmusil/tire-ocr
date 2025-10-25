using System.Diagnostics;
using TireOcr.Preprocessing.Application.Dtos;
using TireOcr.Preprocessing.Application.Facades;
using TireOcr.Preprocessing.Application.Services;
using TireOcr.Preprocessing.Domain.Common;
using TireOcr.Preprocessing.Domain.ImageEntity;
using TireOcr.Shared.Extensions;
using TireOcr.Shared.Result;

namespace TireOcr.Preprocessing.Infrastructure.Facades;

public class TextDetectionFacade : ITextDetectionFacade
{
    private readonly IImageSlicerService _imageSlicerService;
    private readonly IImageTextApproximatorService _imageTextApproximatorService;
    private readonly ITextDetectionService _textDetectionService;
    private readonly IImageManipulationService _imageManipulationService;

    public TextDetectionFacade(IImageSlicerService imageSlicerService,
        IImageTextApproximatorService imageTextApproximatorService,
        ITextDetectionService textDetectionService, IImageManipulationService imageManipulationService)
    {
        _imageSlicerService = imageSlicerService;
        _imageTextApproximatorService = imageTextApproximatorService;
        _textDetectionService = textDetectionService;
        _imageManipulationService = imageManipulationService;
    }

    public async Task<DataResult<TextDetectionResultDto>> ExtractTireCodeRoi(Image image)
    {
        var taskResult = await PerformanceUtils
            .PerformTimeMeasuredTask(() => GetRoiOfImage(image));

        var timeTaken = taskResult.Item1;
        var resultImage = taskResult.Item2;

        if (resultImage.IsFailure)
            return DataResult<TextDetectionResultDto>.Failure(resultImage.Failures);

        var result = new TextDetectionResultDto(
            BestImage: resultImage.Data!.Image,
            DetectedStringsWithLevenshteinDistance: resultImage.Data!.DetectedStrings,
            TimeTaken: timeTaken
        );
        return DataResult<TextDetectionResultDto>.Success(result);
    }

    public async Task<DataResult<TextDetectionResultDto>> ExtractTireCodeRoiAndRemoveBg(Image image)
    {
        var taskResult = await PerformanceUtils
            .PerformTimeMeasuredTask(async () =>
            {
                var roi = await GetRoiOfImage(image);
                if (roi.IsFailure)
                    return DataResult<ImageWithDetectedTexts>.Failure(roi.Failures);

                var roiImage = roi.Data!;
                var boundingBoxes = roiImage.DetectedStrings.Keys
                    .Where(s => s.Characters.Count > 0)
                    .Select(s => _imageManipulationService.GetBoundingBoxForTireCodeString(roiImage.Image, s));
                var roiImageWithoutBg =
                    _imageManipulationService.ApplyMaskToEverythingExceptBoundingBoxes(roiImage.Image, boundingBoxes);

                var finalImage = _imageManipulationService.ApplyClahe(roiImageWithoutBg);
                finalImage =
                    _imageManipulationService.ApplyBilateralFilter(finalImage, d: 5, sigmaColor: 40, sigmaSpace: 40);
                finalImage = _imageManipulationService.ApplyBitwiseNot(finalImage);

                return DataResult<ImageWithDetectedTexts>.Success(roiImage with { Image = finalImage });
            });

        var timeTaken = taskResult.Item1;
        var resultImage = taskResult.Item2;

        if (resultImage.IsFailure)
            return DataResult<TextDetectionResultDto>.Failure(resultImage.Failures);

        var result = new TextDetectionResultDto(
            BestImage: resultImage.Data!.Image,
            DetectedStringsWithLevenshteinDistance: resultImage.Data!.DetectedStrings,
            TimeTaken: timeTaken
        );
        return DataResult<TextDetectionResultDto>.Success(result);
    }

    private async Task<DataResult<ImageWithDetectedTexts>> GetRoiOfImage(Image image)
    {
        var sliceSize = new ImageSize(
            image.Size.Height,
            (int)(image.Size.Width * 0.17)
        );
        var slicesResult = await _imageSlicerService.SliceImageNormalOverlap(image, sliceSize, 0.3, 0);
        if (slicesResult.IsFailure)
            return DataResult<ImageWithDetectedTexts>.Failure(slicesResult.Failures);

        var slices = slicesResult.Data!;
        var results = await Task.WhenAll(slices.Select(ProcessSingleImageSliceAsync));

        var successfulResults = results
            .Where(r => r.IsSuccess)
            .Select(r => r.Data!)
            .ToList();

        if (!successfulResults.Any())
            return DataResult<ImageWithDetectedTexts>.NotFound("No text detected in the image");

        var bestResult = successfulResults
            .MinBy(r => r.DetectedStrings.Values.ToArray().Min());
        if (bestResult is null)
            return DataResult<ImageWithDetectedTexts>.Failure(new Failure(500,
                "Failed to determine image with best text match"));

        return DataResult<ImageWithDetectedTexts>.Success(bestResult);
    }

    private async Task<DataResult<ImageWithDetectedTexts>> ProcessSingleImageSliceAsync(Image slice)
    {
        var detectedCharactersResult = await _textDetectionService.DetectTextInImage(slice);
        if (detectedCharactersResult.IsFailure)
            return DataResult<ImageWithDetectedTexts>.Failure(detectedCharactersResult.Failures);

        var detectedCharacters = detectedCharactersResult.Data!;
        var approximatedStringsResult = _imageTextApproximatorService
            .ApproximateStringsFromCharacters(detectedCharacters);

        if (approximatedStringsResult.IsFailure)
            return DataResult<ImageWithDetectedTexts>.Failure(approximatedStringsResult.Failures);
        var approximatedStrings = approximatedStringsResult.Data!;

        var scoresResult = _imageTextApproximatorService.GetTireCodeLevenshteinDistanceOfStrings(approximatedStrings);
        if (scoresResult.IsFailure)
            return DataResult<ImageWithDetectedTexts>.Failure(scoresResult.Failures);

        var scores = scoresResult.Data!;
        var result = new ImageWithDetectedTexts(slice, scores);
        return DataResult<ImageWithDetectedTexts>.Success(result);
    }

    private record ImageWithDetectedTexts(Image Image, Dictionary<StringInImage, int> DetectedStrings);
}