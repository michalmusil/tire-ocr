using TireOcr.Preprocessing.Application.Dtos;
using TireOcr.Preprocessing.Application.Services;
using TireOcr.Preprocessing.Domain.ImageEntity;
using TireOcr.Shared.Result;
using TireOcr.Shared.UseCase;

namespace TireOcr.Preprocessing.Application.Queries.GetPreprocessedImage;

public class GetPreprocessedImageQueryHandler : IQueryHandler<GetPreprocessedImageQuery, PreprocessedImageDto>
{
    private readonly IImageManipulationService _imageManipulationService;

    public GetPreprocessedImageQueryHandler(IImageManipulationService imageManipulationService)
    {
        _imageManipulationService = imageManipulationService;
    }

    public async Task<DataResult<PreprocessedImageDto>> Handle(
        GetPreprocessedImageQuery request,
        CancellationToken cancellationToken
    )
    {
        var image = new Image(request.ImageData, request.ImageName, new ImageSize(0,0));
        var withClahe = _imageManipulationService.ApplyClahe(image);

        var dto = new PreprocessedImageDto(withClahe.Name, withClahe.Data, "image/jpeg");
        return DataResult<PreprocessedImageDto>.Success(dto);
    }
}