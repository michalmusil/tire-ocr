using AiPipeline.TireOcr.TasyDbMatcher.Application.Dtos;
using AiPipeline.TireOcr.TasyDbMatcher.Application.Repositories;
using AiPipeline.TireOcr.TasyDbMatcher.Application.Services;
using TireOcr.Shared.Result;

namespace AiPipeline.TireOcr.TasyDbMatcher.Infrastructure.Services;

public class ManufacturerDbMatchingService : IManufacturerDbMatchingService
{
    private readonly ISupportedManufacturersRepository _repository;

    public ManufacturerDbMatchingService(ISupportedManufacturersRepository repository)
    {
        _repository = repository;
    }

    public async Task<DataResult<SupportedManufacturerDto>> GetSupportedManufacturerFromRawManufacturer(
        string rawTireManufacturer)
    {
        try
        {
            var allSupportedManufacturers = await _repository.GetSupportedManufacturers();
            var matchingManufacturer = allSupportedManufacturers
                .FirstOrDefault(m => m.Name.Trim().Equals(rawTireManufacturer.Trim(), StringComparison.OrdinalIgnoreCase));

            if (matchingManufacturer is null)
                return DataResult<SupportedManufacturerDto>.NotFound(
                    $"No supported manufacturer matches {rawTireManufacturer}");

            return DataResult<SupportedManufacturerDto>.Success(matchingManufacturer);
        }
        catch
        {
            return DataResult<SupportedManufacturerDto>.Failure(new Failure(500,
                "Failed to retrieve supported manufacturer"));
        }
    }
}