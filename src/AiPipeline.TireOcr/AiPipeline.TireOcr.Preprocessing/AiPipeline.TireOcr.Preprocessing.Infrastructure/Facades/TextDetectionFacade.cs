using System.Diagnostics;
using TireOcr.Preprocessing.Application.Dtos;
using TireOcr.Preprocessing.Application.Facades;
using TireOcr.Preprocessing.Application.Services;
using TireOcr.Preprocessing.Domain.Common;
using TireOcr.Preprocessing.Domain.ImageEntity;
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

    public async Task<DataResult<TextDetectionResultDto>> GetTextAreaFromImageAsync(Image image)
    {
        var stopWatch = new Stopwatch();
        stopWatch.Start();

        var sliceSize = new ImageSize(
            image.Size.Height,
            (int)(image.Size.Width * 0.17)
        );
        var slicesResult = await _imageSlicerService.SliceImage(image, sliceSize, 0.3, 0);
        if (slicesResult.IsFailure)
            return DataResult<TextDetectionResultDto>.Failure(slicesResult.Failures);

        var slices = slicesResult.Data!;
        var results = await Task.WhenAll(slices.Select(ProcessSingleImageSliceAsync));

        var successfulResults = results
            .Where(r => r.IsSuccess)
            .Select(r => r.Data!)
            .ToList();

        if (!successfulResults.Any())
            return DataResult<TextDetectionResultDto>.NotFound("No text detected in the image");

        var bestResult = successfulResults
            .MinBy(r => r.DetectedStrings.Values.ToArray().Min());
        if (bestResult is null)
            return DataResult<TextDetectionResultDto>.Failure(new Failure(500,
                "Failed to determine image with best text match"));

        var bestResultPreprocessed = await PerformFurtherImagePreprocessing(bestResult);

        stopWatch.Stop();
        var timeTaken = stopWatch.Elapsed;

        var result = new TextDetectionResultDto(bestResultPreprocessed.Image, bestResultPreprocessed.DetectedStrings,
            timeTaken);
        // var result = new TextDetectionResultDto(bestResult.Image, bestResult.DetectedStrings,
        //     timeTaken);
        return DataResult<TextDetectionResultDto>.Success(result);
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

    private async Task<ImageWithDetectedTexts> PerformFurtherImagePreprocessing(
        ImageWithDetectedTexts imageWithDetectedTexts)
    {
        var boundingBoxes = imageWithDetectedTexts.DetectedStrings.Keys
            .Where(s => s.Characters.Count > 0)
            .Select(s => _imageManipulationService.GetBoundingBoxForTireCodeString(imageWithDetectedTexts.Image, s));
        var onlyBoundingBoxAreas =
            _imageManipulationService.ApplyMaskToEverythingExceptBoundingBoxes(imageWithDetectedTexts.Image,
                boundingBoxes);

        var finalImage = _imageManipulationService.ApplyClahe(onlyBoundingBoxAreas);
        finalImage = _imageManipulationService.ApplyBilateralFilter(finalImage, d: 7, sigmaColor: 50, sigmaSpace: 70);
        // finalImage = _imageManipulationService.ApplyClahe(finalImage, clipLimit: 60);
        // finalImage = _imageManipulationService.ApplyClahe(finalImage);
        // finalImage = _imageManipulationService.ApplyGausianBlur(finalImage, kernelWidth: 3, kernelHeight: 3);
        // finalImage = _imageManipulationService.ApplyAdaptiveTreshold(finalImage, value: 255, blockSize: 13);
        // finalImage = _imageManipulationService.ApplySharpening(finalImage, strength: 5f);
        // finalImage = _imageManipulationService.ApplyEdgeDetection(finalImage, treshold1: 150, treshold2: 220);
        finalImage = _imageManipulationService.ApplyBitwiseNot(finalImage);

        return new ImageWithDetectedTexts(
            finalImage,
            imageWithDetectedTexts.DetectedStrings
        );
    }

    private record ImageWithDetectedTexts(Image Image, Dictionary<StringInImage, int> DetectedStrings);
}