using AiPipeline.TireOcr.DbMatcher.Application.Dtos;
using TireOcr.Shared.Result;

namespace AiPipeline.TireOcr.DbMatcher.Application.Services;

public interface IManufacturerDbMatchingService
{
    public Task<DataResult<SupportedManufacturerDto>> GetSupportedManufacturerFromRawManufacturer(
        string rawTireManufacturer);
}