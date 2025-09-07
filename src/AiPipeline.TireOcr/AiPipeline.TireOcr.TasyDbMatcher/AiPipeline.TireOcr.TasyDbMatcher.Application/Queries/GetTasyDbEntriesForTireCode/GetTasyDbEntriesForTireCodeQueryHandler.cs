using AiPipeline.TireOcr.TasyDbMatcher.Application.Dtos;
using AiPipeline.TireOcr.TasyDbMatcher.Application.Repositories;
using AiPipeline.TireOcr.TasyDbMatcher.Application.Services;
using TireOcr.Shared.Result;
using TireOcr.Shared.UseCase;

namespace AiPipeline.TireOcr.TasyDbMatcher.Application.Queries.GetTasyDbEntriesForTireCode;

public class GetTasyDbEntriesForTireCodeQueryHandler : IQueryHandler<GetTasyDbEntriesForTireCodeQuery,
    List<TireDbMatch>>
{
    private readonly ITireParamsDbRepository _tireParamsRepository;
    private readonly ITireCodeDbMatchingService _matchingService;

    public GetTasyDbEntriesForTireCodeQueryHandler(ITireParamsDbRepository tireParamsRepository,
        ITireCodeDbMatchingService matchingService)
    {
        _tireParamsRepository = tireParamsRepository;
        _matchingService = matchingService;
    }

    public async Task<DataResult<List<TireDbMatch>>> Handle(
        GetTasyDbEntriesForTireCodeQuery request,
        CancellationToken cancellationToken)
    {
        var existingTireEntriesResult = await _tireParamsRepository.GetAllTireParamEntries();
        if (existingTireEntriesResult.IsFailure)
            return DataResult<List<TireDbMatch>>.Failure(existingTireEntriesResult.Failures);

        var matchingEntries = await _matchingService.GetOrderedMatchingEntriesForCode(
            tireCode: request.DetectedCode,
            entriesToMatch: existingTireEntriesResult.Data!,
            limit: request.MaxEntries ?? 30
        );

        return DataResult<List<TireDbMatch>>.Success(matchingEntries);
    }
}