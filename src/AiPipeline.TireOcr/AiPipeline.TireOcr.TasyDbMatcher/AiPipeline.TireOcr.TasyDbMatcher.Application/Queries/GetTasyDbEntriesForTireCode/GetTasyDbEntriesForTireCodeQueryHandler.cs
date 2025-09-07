using AiPipeline.TireOcr.TasyDbMatcher.Application.Dtos;
using TireOcr.Shared.Result;
using TireOcr.Shared.UseCase;

namespace AiPipeline.TireOcr.TasyDbMatcher.Application.Queries.GetTasyDbEntriesForTireCode;

public class GetTasyDbEntriesForTireCodeQueryHandler: IQueryHandler<GetTasyDbEntriesForTireCodeQuery, List<ProcessedTireParamsDatabaseEntryDto>>
{
    public Task<DataResult<List<ProcessedTireParamsDatabaseEntryDto>>> Handle(GetTasyDbEntriesForTireCodeQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}