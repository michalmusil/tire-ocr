using TireOcr.Preprocessing.Application.Dtos;
using TireOcr.Shared.UseCase;

namespace TireOcr.Preprocessing.Application.Commands.ExtractImageSlices;

/// <summary>
/// Unwraps the tire and slices the unwrapped image vertically.
/// This command first uses a model to detect a tire in the image (if none is found, command fails) and unwrap the tire part into a
/// wide strip. Then this strip is sliced horizontally into multiple images (specified by NumberOfSlices) with overlap.
/// These slices are then composed into a vertical stack.
/// </summary>
/// <param name="ImageData">Binary data of the tire image</param>
/// <param name="ImageName">Original name of the tire image</param>
/// <param name="OriginalContentType">Content type of the tire image</param>
/// <param name="NumberOfSlices">How many times should the strip be sliced horizontally</param>
public record ExtractImageSlicesCommand(
    byte[] ImageData,
    string ImageName,
    string OriginalContentType,
    int NumberOfSlices)
    : ICommand<PreprocessedImageDto>;