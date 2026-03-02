using TireOcr.Preprocessing.Application.Dtos;
using TireOcr.Shared.UseCase;

namespace TireOcr.Preprocessing.Application.Commands.ExtractAbsoluteRoiPosition;

public record ExtractAbsoluteRoiPositionCommand(byte[] ImageData, string ImageName, string OriginalContentType)
    : ICommand<PreprocessedImageDto>;