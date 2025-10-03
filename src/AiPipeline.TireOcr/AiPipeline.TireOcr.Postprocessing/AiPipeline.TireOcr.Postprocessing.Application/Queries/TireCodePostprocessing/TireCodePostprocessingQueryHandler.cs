using TireOcr.Postprocessing.Application.Dtos;
using TireOcr.Postprocessing.Application.Services;
using TireOcr.Postprocessing.Domain.TireCodeEntity;
using TireOcr.Shared.Extensions;
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
        var result = await PerformanceUtils.PerformTimeMeasuredTask(
            runTask: () => PerformPostprocessing(request)
        );
        var timeTaken = result.Item1;
        var postprocessingResult = result.Item2;

        if (postprocessingResult.IsFailure)
            return DataResult<ProcessedTireCodeResultDto>.Failure(postprocessingResult.Failures);

        var bestTireCode = postprocessingResult.Data!;
        var dto = new ProcessedTireCodeResultDto
        (
            RawCode: request.RawTireCode,
            PostprocessedTireCode: bestTireCode.GetProcessedCode(),
            VehicleClass: bestTireCode.VehicleClass,
            Width: bestTireCode.Width,
            AspectRatio: bestTireCode.AspectRatio,
            Construction: bestTireCode.Construction,
            Diameter: bestTireCode.Diameter,
            LoadRange: bestTireCode.LoadRange,
            LoadIndex: bestTireCode.LoadIndex,
            LoadIndex2: bestTireCode.LoadIndex2,
            SpeedRating: bestTireCode.SpeedRating,
            DurationMs: (long)timeTaken.TotalMilliseconds
        );

        return DataResult<ProcessedTireCodeResultDto>.Success(dto);
    }

    private async Task<DataResult<TireCode>> PerformPostprocessing(TireCodePostprocessingQuery request)
    {
        var tireCodesResult = _codeFeatureExtractionService.ExtractTireCodes(request.RawTireCode);
        if (tireCodesResult.IsFailure)
            return DataResult<TireCode>.Failure(tireCodesResult.Failures);

        var potentialTireCodes = tireCodesResult.Data!;
        var bestTireCodeResult = _codeFeatureExtractionService.PickBestMatchingTireCode(potentialTireCodes);
        if (bestTireCodeResult.IsFailure)
            return DataResult<TireCode>.Failure(bestTireCodeResult.Failures);

        var bestTireCode = bestTireCodeResult.Data!;
        return DataResult<TireCode>.Success(bestTireCode);
    }
}