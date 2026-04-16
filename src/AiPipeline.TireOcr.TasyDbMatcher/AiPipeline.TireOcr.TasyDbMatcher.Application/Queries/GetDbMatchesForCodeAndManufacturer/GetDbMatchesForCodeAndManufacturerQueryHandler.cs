using AiPipeline.TireOcr.TasyDbMatcher.Application.Dtos;
using AiPipeline.TireOcr.TasyDbMatcher.Application.Services;
using TireOcr.Shared.Extensions;
using TireOcr.Shared.Result;
using TireOcr.Shared.UseCase;

namespace AiPipeline.TireOcr.TasyDbMatcher.Application.Queries.GetDbMatchesForCodeAndManufacturer;

public class GetDbMatchesForCodeAndManufacturerQueryHandler : IQueryHandler<GetDbMatchesForCodeAndManufacturerQuery,
    DbMatchingResultDto>
{
    private readonly ITireCodeDbMatchingService _tireMatchingService;
    private readonly IManufacturerDbMatchingService _manufacturerMatchingService;

    public GetDbMatchesForCodeAndManufacturerQueryHandler(ITireCodeDbMatchingService tireMatchingService,
        IManufacturerDbMatchingService manufacturerMatchingService)
    {
        _tireMatchingService = tireMatchingService;
        _manufacturerMatchingService = manufacturerMatchingService;
    }

    private record MatchedResults(
        List<TireDbMatchDto> TireDbMatches,
        string? ManufacturerDbMatch
    );

    public async Task<DataResult<DbMatchingResultDto>> Handle(
        GetDbMatchesForCodeAndManufacturerQuery request,
        CancellationToken cancellationToken)
    {
        var result = await PerformanceUtils.PerformTimeMeasuredTask(
            runTask: () => PerformMatching(request)
        );
        var timeTaken = result.Item1;
        var dbMatches = result.Item2;


        return DataResult<DbMatchingResultDto>.Success(new DbMatchingResultDto(
            TireDbMatches: dbMatches.TireDbMatches,
            ManufacturerDbMatch: dbMatches.ManufacturerDbMatch,
            DurationMs: (long)timeTaken.TotalMilliseconds
        ));
    }

    private async Task<MatchedResults> PerformMatching(GetDbMatchesForCodeAndManufacturerQuery request)
    {
        var matchingEntries = await _tireMatchingService.GetOrderedMatchingEntriesForCode(
            tireCode: request.DetectedCode,
            limit: request.MaxEntries ?? 30
        );

        DataResult<SupportedManufacturerDto> matchingManufacturerResult = request.DetectedManufacturer is null
            ? DataResult<SupportedManufacturerDto>.NotFound("No raw manufacturer provided")
            : await _manufacturerMatchingService.GetSupportedManufacturerFromRawManufacturer(
                request.DetectedManufacturer);
        var manufacturerDbMatch = matchingManufacturerResult.Map<string?>(
            onSuccess: m => m.Name,
            onFailure: _ => null
        );

        return new MatchedResults(matchingEntries, manufacturerDbMatch);
    }
}