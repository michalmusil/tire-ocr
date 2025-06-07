using TireOcr.Postprocessing.Application.Dtos;
using TireOcr.Postprocessing.Application.Services;
using TireOcr.Shared.Result;
using TireOcr.Shared.UseCase;

namespace TireOcr.Postprocessing.Application.Queries.TireCodePostprocessing;

public class TireCodePostprocessingQueryHandler : IQueryHandler<TireCodePostprocessingQuery, ProcessedTireCodeResultDto>
{
    private readonly ICodeFeatureExtractionService _codeFeatureExtractionService;

    public TireCodePostprocessingQueryHandler(ICodeFeatureExtractionService codeFeatureExtractionService)
    {
        _codeFeatureExtractionService = codeFeatureExtractionService;
    }

    public async Task<DataResult<ProcessedTireCodeResultDto>> Handle(TireCodePostprocessingQuery request,
        CancellationToken cancellationToken)
    {
        var tireCodesResult = _codeFeatureExtractionService.ExtractTireCodes(request.RawTireCode);
        if (tireCodesResult.IsFailure)
            return DataResult<ProcessedTireCodeResultDto>.Failure(tireCodesResult.Failures);

        var potentialTireCodes = tireCodesResult.Data!;
        var bestTireCodeResult = _codeFeatureExtractionService.PickBestMatchingTireCode(potentialTireCodes);
        if (bestTireCodeResult.IsFailure)
            return DataResult<ProcessedTireCodeResultDto>.Failure(bestTireCodeResult.Failures);

        var bestTireCode = bestTireCodeResult.Data!;
        var result = new ProcessedTireCodeResultDto
        (
            RawCode: request.RawTireCode,
            PostprocessedTireCode: bestTireCode.GetProcessedCode(),
            VehicleClass: bestTireCode.VehicleClass,
            Width: bestTireCode.Width,
            AspectRatio: bestTireCode.AspectRatio,
            Construction: bestTireCode.Construction,
            Diameter: bestTireCode.Diameter,
            LoadIndex: bestTireCode.LoadIndex,
            SpeedRating: bestTireCode.SpeedRating
        );

        return DataResult<ProcessedTireCodeResultDto>.Success(result);
    }
}