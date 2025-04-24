using TireOcr.Postprocessing.Application.Dtos;
using TireOcr.Postprocessing.Application.Services;
using TireOcr.Shared.Result;
using TireOcr.Shared.UseCase;

namespace TireOcr.Postprocessing.Application.Queries.TireCodePostprocessing;

public class TireCodePostprocessingQueryHandler : IQueryHandler<TireCodePostprocessingQuery, ProcessedTireCodeResult>
{
    private readonly ICodeFeatureExtractionService _codeFeatureExtractionService;

    public TireCodePostprocessingQueryHandler(ICodeFeatureExtractionService codeFeatureExtractionService)
    {
        _codeFeatureExtractionService = codeFeatureExtractionService;
    }

    public async Task<DataResult<ProcessedTireCodeResult>> Handle(TireCodePostprocessingQuery request,
        CancellationToken cancellationToken)
    {
        var tireCodesResult = _codeFeatureExtractionService.ExtractTireCodes(request.RawTireCode);
        if (tireCodesResult.IsFailure)
            return DataResult<ProcessedTireCodeResult>.Failure(tireCodesResult.Failures);

        var potentialTireCodes = tireCodesResult.Data!;
        var bestTireCodeResult = _codeFeatureExtractionService.PickBestMatchingTireCode(potentialTireCodes);
        if (bestTireCodeResult.IsFailure)
            return DataResult<ProcessedTireCodeResult>.Failure(bestTireCodeResult.Failures);

        var bestTireCode = bestTireCodeResult.Data!;
        var result = new ProcessedTireCodeResult
        (
            RawCode: bestTireCode.RawCode,
            PostprocessedTireCode: bestTireCode.GetProcessedCode(),
            VehicleClass: bestTireCode.VehicleClass,
            Width: bestTireCode.Width,
            AspectRatio: bestTireCode.AspectRatio,
            Construction: bestTireCode.Construction,
            LoadRangeAndIndex: bestTireCode.LoadRangeAndIndex,
            SpeedRating: bestTireCode.SpeedRating
        );

        return DataResult<ProcessedTireCodeResult>.Success(result);
    }
}