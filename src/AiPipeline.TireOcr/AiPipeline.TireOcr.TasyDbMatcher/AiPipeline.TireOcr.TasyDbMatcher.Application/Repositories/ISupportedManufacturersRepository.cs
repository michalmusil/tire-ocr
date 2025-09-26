using AiPipeline.TireOcr.TasyDbMatcher.Application.Dtos;

namespace AiPipeline.TireOcr.TasyDbMatcher.Application.Repositories;

public interface ISupportedManufacturersRepository
{
    public Task<IEnumerable<SupportedManufacturerDto>> GetSupportedManufacturers();
}