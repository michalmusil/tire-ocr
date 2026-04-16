using AiPipeline.TireOcr.DbMatcher.Application.Dtos;
using TireOcr.Shared.Result;

namespace AiPipeline.TireOcr.DbMatcher.Application.Repositories;

public interface ITireParamsDbRepository
{
    public Task<DataResult<IEnumerable<ProcessedTireParamsDatabaseEntryDto>>> GetAllTireParamEntries();
}