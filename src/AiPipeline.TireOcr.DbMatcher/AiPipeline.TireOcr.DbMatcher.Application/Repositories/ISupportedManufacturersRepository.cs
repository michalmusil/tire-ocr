using AiPipeline.TireOcr.DbMatcher.Application.Dtos;

namespace AiPipeline.TireOcr.DbMatcher.Application.Repositories;

public interface ISupportedManufacturersRepository
{
    public Task<IEnumerable<SupportedManufacturerDto>> GetSupportedManufacturers();
}