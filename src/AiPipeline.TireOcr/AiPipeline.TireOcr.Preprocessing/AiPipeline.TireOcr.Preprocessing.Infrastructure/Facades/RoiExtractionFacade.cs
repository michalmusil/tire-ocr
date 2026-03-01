using Microsoft.Extensions.Options;
using TireOcr.Preprocessing.Application.Dtos;
using TireOcr.Preprocessing.Application.Facades;
using TireOcr.Preprocessing.Application.Options;
using TireOcr.Preprocessing.Application.Services;
using TireOcr.Preprocessing.Domain.Common;
using TireOcr.Preprocessing.Domain.ImageEntity;
using TireOcr.Shared.Extensions;
using TireOcr.Shared.Result;

namespace TireOcr.Preprocessing.Infrastructure.Facades;

public class RoiExtractionFacade : IRoiExtractionFacade
{
    private readonly IImageSlicerService _imageSlicerService;
    private readonly IImageTextApproximatorService _imageTextApproximatorService;
    private readonly ITextDetectionService _textDetectionService;
    private readonly ICharacterEnhancementService _characterEnhancementService;
    private readonly ImageProcessingOptions _imageProcessingOptions;

    public RoiExtractionFacade(IImageSlicerService imageSlicerService,
        IImageTextApproximatorService imageTextApproximatorService,
        ITextDetectionService textDetectionService, ICharacterEnhancementService characterEnhancementService,
        IOptions<ImageProcessingOptions> imageProcessingOptions)
    {
        _imageSlicerService = imageSlicerService;
        _imageTextApproximatorService = imageTextApproximatorService;
        _textDetectionService = textDetectionService;
        _characterEnhancementService = characterEnhancementService;
        _imageProcessingOptions = imageProcessingOptions.Value;
    }

    public async Task<DataResult<TextDetectionResultDto>> ExtractSliceContainingTireCode(Image image)
    {
        var taskResult = await PerformanceUtils
            .PerformTimeMeasuredTask(async () =>
            {
                var slicesWithTextResult = await GetSlicesWithDetectedTexts(image);
                if (slicesWithTextResult.IsFailure)
                    return DataResult<ImageWithDetectedTexts>.Failure(slicesWithTextResult.Failures);

                var slicesWithDetectedTexts = slicesWithTextResult.Data!.ToList();
                if (slicesWithDetectedTexts.Count < 1)
                    return DataResult<ImageWithDetectedTexts>.NotFound("No text detected in the image");

                var bestResult = slicesWithDetectedTexts
                    .MinBy(r => r.DetectedStrings.Values.ToArray().Min());

                return DataResult<ImageWithDetectedTexts>.Success(bestResult!);
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

    public async Task<DataResult<TextDetectionResultDto>> ExtractSliceContainingTireCodeAndEnhanceCharacters(Image image)
    {
        var taskResult = await PerformanceUtils
            .PerformTimeMeasuredTask(async () =>
            {
                var slicesWithTextResult = await GetSlicesWithDetectedTexts(image);
                if (slicesWithTextResult.IsFailure)
                    return DataResult<ImageWithDetectedTexts>.Failure(slicesWithTextResult.Failures);

                var slicesWithDetectedTexts = slicesWithTextResult.Data!.ToList();
                if (slicesWithDetectedTexts.Count < 1)
                    return DataResult<ImageWithDetectedTexts>.NotFound("No text detected in the image");

                var bestResult = slicesWithDetectedTexts
                    .MinBy(r => r.DetectedStrings.Values.ToArray().Min())!;

                var enhancementResult = await _characterEnhancementService.EnhanceCharactersAsync(bestResult.Image);
                if (enhancementResult.IsFailure)
                    return DataResult<ImageWithDetectedTexts>.Failure(enhancementResult.Failures);

                return DataResult<ImageWithDetectedTexts>.Success(bestResult with { Image = enhancementResult.Data! });
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

    private async Task<DataResult<IEnumerable<ImageWithDetectedTexts>>> GetSlicesWithDetectedTexts(Image image)
    {
        var sliceSize = new ImageSize(
            image.Size.Height,
            (int)(image.Size.Width * _imageProcessingOptions.NormSliceWidthPortion)
        );
        var slicesResult =
            await _imageSlicerService.SliceImageNormalOverlap(
                image,
                sliceSize,
                _imageProcessingOptions.NormSliceOverlapRatio,
                0
            );
        if (slicesResult.IsFailure)
            return DataResult<IEnumerable<ImageWithDetectedTexts>>.Failure(slicesResult.Failures);

        var slices = slicesResult.Data!;
        var results = await Task.WhenAll(slices.Select(ProcessSingleImageSliceAsync));

        var successfulResults = results
            .Where(r => r.IsSuccess)
            .Select(r => r.Data!)
            .ToList();

        return DataResult<IEnumerable<ImageWithDetectedTexts>>.Success(successfulResults);
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