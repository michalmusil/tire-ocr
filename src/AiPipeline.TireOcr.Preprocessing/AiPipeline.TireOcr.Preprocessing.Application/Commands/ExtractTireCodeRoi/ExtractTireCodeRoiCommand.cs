using TireOcr.Preprocessing.Application.Dtos;
using TireOcr.Shared.UseCase;

namespace TireOcr.Preprocessing.Application.Commands.ExtractTireCodeRoi;

public record ExtractTireCodeRoiCommand(byte[] ImageData, string ImageName, string OriginalContentType, bool EnhanceCharacters)
    : ICommand<PreprocessedImageDto>;