using AiPipeline.TireOcr.TasyDbMatcher.Application.Dtos;
using TireOcr.Shared.Result;

namespace AiPipeline.TireOcr.TasyDbMatcher.Application.Services;

public interface IManufacturerDbMatchingService
{
    public Task<DataResult<SupportedManufacturerDto>> GetSupportedManufacturerFromRawManufacturer(
        string rawTireManufacturer);
}