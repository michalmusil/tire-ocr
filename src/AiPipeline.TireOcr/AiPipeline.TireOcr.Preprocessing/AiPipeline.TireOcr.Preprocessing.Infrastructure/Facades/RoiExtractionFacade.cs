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
    private readonly IImageManipulationService _imageManipulationService;
    private readonly ImageProcessingOptions _imageProcessingOptions;

    public RoiExtractionFacade(IImageSlicerService imageSlicerService,
        IImageTextApproximatorService imageTextApproximatorService,
        ITextDetectionService textDetectionService, ICharacterEnhancementService characterEnhancementService,
        IImageManipulationService imageManipulationService, IOptions<ImageProcessingOptions> imageProcessingOptions)
    {
        _imageSlicerService = imageSlicerService;
        _imageTextApproximatorService = imageTextApproximatorService;
        _textDetectionService = textDetectionService;
        _characterEnhancementService = characterEnhancementService;
        _imageManipulationService = imageManipulationService;
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
            BestImage: resultImage.Data!.ImageWithOffset.Image,
            DetectedStringsWithLevenshteinDistance: resultImage.Data!.DetectedStrings,
            TimeTaken: timeTaken
        );
        return DataResult<TextDetectionResultDto>.Success(result);
    }

    public async Task<DataResult<TextDetectionResultDto>>
        ExtractSliceContainingTireCodeAndEnhanceCharacters(Image image)
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

                var enhancementResult =
                    await _characterEnhancementService.EnhanceCharactersAsync(bestResult.ImageWithOffset.Image);
                if (enhancementResult.IsFailure)
                    return DataResult<ImageWithDetectedTexts>.Failure(enhancementResult.Failures);

                return DataResult<ImageWithDetectedTexts>.Success(bestResult with
                {
                    ImageWithOffset = bestResult.ImageWithOffset with
                    {
                        Image = enhancementResult.Data!
                    }
                });
            });

        var timeTaken = taskResult.Item1;
        var resultImage = taskResult.Item2;

        if (resultImage.IsFailure)
            return DataResult<TextDetectionResultDto>.Failure(resultImage.Failures);

        var result = new TextDetectionResultDto(
            BestImage: resultImage.Data!.ImageWithOffset.Image,
            DetectedStringsWithLevenshteinDistance: resultImage.Data!.DetectedStrings,
            TimeTaken: timeTaken
        );
        return DataResult<TextDetectionResultDto>.Success(result);
    }

    public async Task<DataResult<TextDetectionResultDto>> ExtractAbsoluteTireCodeRoi(Image image)
    {
        var taskResult = await PerformanceUtils
            .PerformTimeMeasuredTask(async () =>
            {
                var slicesWithTextResult = await GetSlicesWithDetectedTexts(image);
                if (slicesWithTextResult.IsFailure)
                    return DataResult<ImageWithDetectedText>.Failure(slicesWithTextResult.Failures);

                var slicesWithDetectedTexts = slicesWithTextResult.Data!.ToList();
                if (slicesWithDetectedTexts.Count < 1)
                    return DataResult<ImageWithDetectedText>.NotFound("No text detected in the image");

                var bestSlice = slicesWithDetectedTexts
                    .MinBy(r => r.DetectedStrings.Values.ToArray().Min())!;
                var bestStringPair = bestSlice.DetectedStrings
                    .ToList()
                    .MinBy(x => x.Value);
                var bestString = bestStringPair.Key;

                var stringBbox = GetStringBoundingBox(bestString);
                if (stringBbox is null)
                    return DataResult<ImageWithDetectedText>.Failure(new Failure(500,
                        "Unexpected error during roi extraction"));

                var offset = bestSlice.ImageWithOffset.TopLeftCornerOffset;
                var absoluteStringCenter = new ImageCoordinate(
                    x: offset.X + (stringBbox.TopLeft.X + stringBbox.BottomRight.X) / 2,
                    y: offset.Y + (stringBbox.TopLeft.Y + stringBbox.BottomRight.Y) / 2
                );

                var roiWidth = (int)Math.Ceiling(image.Size.Width * _imageProcessingOptions.AbsoluteRoiWidthRatio);
                // var roiHeight = (int)Math.Ceiling(
                //     (stringBbox.BottomRight.Y - stringBbox.TopLeft.Y) * _imageProcessingOptions.AbsoluteRoiHeightRatio
                // );

                var roiTopLeftCoordinate = new ImageCoordinate(
                    x: Math.Max(0, absoluteStringCenter.X - roiWidth / 2),
                    // y: Math.Max(0, absoluteStringCenter.Y - roiHeight / 2)
                    y: 0
                );
                var roiBottomRightCoordinate = new ImageCoordinate(
                    x: Math.Min(roiTopLeftCoordinate.X + roiWidth, image.Size.Width),
                    // y: Math.Min(roiTopLeftCoordinate.Y + roiHeight, image.Size.Height)
                    y: image.Size.Height
                );

                var roi = _imageManipulationService.GetBboxInImage(
                    image: image,
                    bbox: new BoundingBox { TopLeft = roiTopLeftCoordinate, BottomRight = roiBottomRightCoordinate }
                );

                var result = new ImageWithDetectedText(
                    ImageWithOffset: new ImageWithOffset(roiTopLeftCoordinate, roi),
                    DetectedString: bestString,
                    LevenshteinDistance: bestStringPair.Value
                );


                return DataResult<ImageWithDetectedText>.Success(result);
            });

        var timeTaken = taskResult.Item1;
        var imageResult = taskResult.Item2;

        if (imageResult.IsFailure)
            return DataResult<TextDetectionResultDto>.Failure(imageResult.Failures);

        var finalImage = imageResult.Data!;

        var result = new TextDetectionResultDto(
            BestImage: finalImage.ImageWithOffset.Image,
            DetectedStringsWithLevenshteinDistance: new Dictionary<StringInImage, int>
                { { finalImage.DetectedString, finalImage.LevenshteinDistance } },
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

    private async Task<DataResult<ImageWithDetectedTexts>> ProcessSingleImageSliceAsync(ImageWithOffset slice)
    {
        var detectedCharactersResult = await _textDetectionService.DetectTextInImage(slice.Image);
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

    private BoundingBox? GetStringBoundingBox(StringInImage stringInImage)
    {
        var characters = stringInImage.Characters;
        if (characters.Count < 1)
            return null;

        var leftMostCoordinate = characters
            .MinBy(c => c.TopLeftCoordinate.X)!.TopLeftCoordinate.X;
        var rightMostCoordinate = characters
            .MaxBy(c => c.BottomRightCoordinate.X)!.BottomRightCoordinate.X;

        var topMostCoordinate = characters
            .MinBy(c => c.TopLeftCoordinate.Y)!.TopLeftCoordinate.Y;
        var bottomMostCoordinate = characters
            .MaxBy(c => c.BottomRightCoordinate.Y)!.BottomRightCoordinate.Y;

        return new BoundingBox
        {
            TopLeft = new ImageCoordinate(leftMostCoordinate, topMostCoordinate),
            BottomRight = new ImageCoordinate(rightMostCoordinate, bottomMostCoordinate),
        };
    }

    private record ImageWithDetectedTexts(
        ImageWithOffset ImageWithOffset,
        Dictionary<StringInImage, int> DetectedStrings);

    private record ImageWithDetectedText(
        ImageWithOffset ImageWithOffset,
        StringInImage DetectedString,
        int LevenshteinDistance
    );
}