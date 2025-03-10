using System.Diagnostics;
using TireOcr.Preprocessing.Application.Dtos;
using TireOcr.Preprocessing.Application.Facades;
using TireOcr.Preprocessing.Application.Services;
using TireOcr.Preprocessing.Domain.ImageEntity;
using TireOcr.Shared.Result;

namespace TireOcr.Preprocessing.Infrastructure.Facades;

public class TextDetectionFacade : ITextDetectionFacade
{
    private readonly IImageSlicer _imageSlicer;
    private readonly IImageTextApproximator _imageTextApproximator;
    private readonly ITextDetectionService _textDetectionService;

    public TextDetectionFacade(IImageSlicer imageSlicer, IImageTextApproximator imageTextApproximator,
        ITextDetectionService textDetectionService)
    {
        _imageSlicer = imageSlicer;
        _imageTextApproximator = imageTextApproximator;
        _textDetectionService = textDetectionService;
    }

    public async Task<DataResult<TextDetectionResult>> GetTextAreaFromImageAsync(Image image)
    {
        var stopWatch = new Stopwatch();
        stopWatch.Start();

        var sliceSize = new ImageSize(
            image.Size.Height,
            (int)(image.Size.Width * 0.2)
        );
        var slicesResult = await _imageSlicer.SliceImage(image, sliceSize, 0.3, 0);
        if (slicesResult.IsFailure)
            return DataResult<TextDetectionResult>.Failure(slicesResult.Failures);

        var slices = slicesResult.Data!;
        var results = await Task.WhenAll(slices.Select(ProcessSingleImageSliceAsync));

        var successfulResults = results
            .Where(r => r.IsSuccess)
            .Select(r => r.Data!)
            .ToList();

        if (!successfulResults.Any())
            return DataResult<TextDetectionResult>.NotFound("No text detected in the image");

        var bestResult = successfulResults
            .MinBy(r => r.DetectedStrings.Values.ToArray().Min());
        if (bestResult is null)
            return DataResult<TextDetectionResult>.Failure(new Failure(500,
                "Failed to determine image with best text match"));

        stopWatch.Stop();
        var timeTaken = stopWatch.Elapsed;

        var result = new TextDetectionResult(bestResult.Image, bestResult.DetectedStrings, timeTaken);
        return DataResult<TextDetectionResult>.Success(result);
    }

    private async Task<DataResult<ImageWithDetectedText>> ProcessSingleImageSliceAsync(Image slice)
    {
        var detectedCharactersResult = await _textDetectionService.DetectTextInImage(slice);
        if (detectedCharactersResult.IsFailure)
            return DataResult<ImageWithDetectedText>.Failure(detectedCharactersResult.Failures);

        var detectedCharacters = detectedCharactersResult.Data!;
        var approximatedStringsResult = _imageTextApproximator
            .ApproximateStringsFromCharacters(detectedCharacters);

        if (approximatedStringsResult.IsFailure)
            return DataResult<ImageWithDetectedText>.Failure(approximatedStringsResult.Failures);
        var approximatedStrings = approximatedStringsResult.Data!;

        var scoresResult = _imageTextApproximator.GetTireCodeLevenshteinDistanceOfStrings(approximatedStrings);
        if (scoresResult.IsFailure)
            return DataResult<ImageWithDetectedText>.Failure(scoresResult.Failures);

        var scores = scoresResult.Data!;
        var result = new ImageWithDetectedText(slice, scores);
        return DataResult<ImageWithDetectedText>.Success(result);
    }

    private record ImageWithDetectedText(Image Image, Dictionary<string, int> DetectedStrings);
}