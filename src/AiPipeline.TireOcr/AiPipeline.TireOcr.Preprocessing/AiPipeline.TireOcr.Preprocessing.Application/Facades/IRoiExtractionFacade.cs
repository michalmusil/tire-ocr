using TireOcr.Preprocessing.Application.Dtos;
using TireOcr.Preprocessing.Domain.ImageEntity;
using TireOcr.Shared.Result;

namespace TireOcr.Preprocessing.Application.Facades;

public interface IRoiExtractionFacade
{
    public Task<DataResult<TextDetectionResultDto>> ExtractSliceContainingTireCode(Image image);
    public Task<DataResult<TextDetectionResultDto>> ExtractSliceContainingTireCodeAndEnhanceCharacters(Image image);
}