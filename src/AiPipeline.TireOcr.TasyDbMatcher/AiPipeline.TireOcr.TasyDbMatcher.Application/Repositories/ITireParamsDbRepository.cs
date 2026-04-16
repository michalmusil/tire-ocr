using AiPipeline.TireOcr.TasyDbMatcher.Application.Dtos;
using TireOcr.Shared.Result;

namespace AiPipeline.TireOcr.TasyDbMatcher.Application.Repositories;

public interface ITireParamsDbRepository
{
    public Task<DataResult<IEnumerable<ProcessedTireParamsDatabaseEntryDto>>> GetAllTireParamEntries();
}