using AiPipeline.TireOcr.TasyDbMatcher.Application.Dtos;
using AiPipeline.TireOcr.TasyDbMatcher.Application.Repositories;
using AiPipeline.TireOcr.TasyDbMatcher.Application.Services;
using TireOcr.Shared.Result;
using TireOcr.Shared.UseCase;

namespace AiPipeline.TireOcr.TasyDbMatcher.Application.Queries.GetTasyDbEntriesForTireCode;

public class GetTasyDbEntriesForTireCodeQueryHandler : IQueryHandler<GetTasyDbEntriesForTireCodeQuery,
    List<TireDbMatchDto>>
{
    private readonly ITireParamsDbRepository _tireParamsRepository;
    private readonly ITireCodeDbMatchingService _matchingService;

    public GetTasyDbEntriesForTireCodeQueryHandler(ITireParamsDbRepository tireParamsRepository,
        ITireCodeDbMatchingService matchingService)
    {
        _tireParamsRepository = tireParamsRepository;
        _matchingService = matchingService;
    }

    public async Task<DataResult<List<TireDbMatchDto>>> Handle(
        GetTasyDbEntriesForTireCodeQuery request,
        CancellationToken cancellationToken)
    {
        var existingTireEntriesResult = await _tireParamsRepository.GetAllTireParamEntries();
        if (existingTireEntriesResult.IsFailure)
            return DataResult<List<TireDbMatchDto>>.Success([]); // TODO: Remove when endpoint is fixed
            // return DataResult<List<TireDbMatchDto>>.Failure(existingTireEntriesResult.Failures);

        var matchingEntries = await _matchingService.GetOrderedMatchingEntriesForCode(
            tireCode: request.DetectedCode,
            entriesToMatch: existingTireEntriesResult.Data!,
            limit: request.MaxEntries ?? 30
        );

        if (!matchingEntries.Any())
            return DataResult<List<TireDbMatchDto>>.Success([]);

        return DataResult<List<TireDbMatchDto>>.Success(matchingEntries);
    }
}