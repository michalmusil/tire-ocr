using AiPipeline.TireOcr.TasyDbMatcher.Application.Dtos;
using AiPipeline.TireOcr.TasyDbMatcher.Application.Services;
using TireOcr.Shared.Result;
using TireOcr.Shared.UseCase;

namespace AiPipeline.TireOcr.TasyDbMatcher.Application.Queries.GetDbMatchesForCodeAndManufacturer;

public class GetDbMatchesForCodeAndManufacturerQueryHandler : IQueryHandler<GetDbMatchesForCodeAndManufacturerQuery,
    DbMatchingResultDto>
{
    private readonly ITireCodeDbMatchingService _tireMatchingService;
    private readonly IManufacturerDbMatchingService _manufacturerMatchingService;

    private readonly DbMatchingResultDto _fallbackResult = new(
        TireDbMatches: [],
        ManufacturerDbMatch: null
    );

    public GetDbMatchesForCodeAndManufacturerQueryHandler(ITireCodeDbMatchingService tireMatchingService,
        IManufacturerDbMatchingService manufacturerMatchingService)
    {
        _tireMatchingService = tireMatchingService;
        _manufacturerMatchingService = manufacturerMatchingService;
    }

    public async Task<DataResult<DbMatchingResultDto>> Handle(
        GetDbMatchesForCodeAndManufacturerQuery request,
        CancellationToken cancellationToken)
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

        return DataResult<DbMatchingResultDto>.Success(new DbMatchingResultDto(
            TireDbMatches: matchingEntries,
            ManufacturerDbMatch: manufacturerDbMatch
        ));
    }
}