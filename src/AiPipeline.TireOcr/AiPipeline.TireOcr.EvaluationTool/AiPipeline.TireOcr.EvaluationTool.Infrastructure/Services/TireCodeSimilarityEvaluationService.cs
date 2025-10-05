using System.Globalization;
using AiPipeline.TireOcr.EvaluationTool.Application.Services;
using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate;
using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate.Evaluation;
using AiPipeline.TireOcr.EvaluationTool.Infrastructure.Extensions;
using Fastenshtein;

namespace AiPipeline.TireOcr.EvaluationTool.Infrastructure.Services;

public class TireCodeSimilarityEvaluationService : ITireCodeSimilarityEvaluationService
{
    public async Task<EvaluationValueObject> EvaluateTireCodeSimilarity(TireCodeValueObject expectedTireCode,
        TireCodeValueObject actualTireCode)
    {
        var parameterEvaluations = new List<ParameterEvaluationValueObject>();

        var vehicleClassEvaluation = GetParameterMatch(actualTireCode.VehicleClass, expectedTireCode.VehicleClass);
        parameterEvaluations.AddIfNotNull(vehicleClassEvaluation);

        var widthEvaluation = GetParameterMatch(
            actualTireCode.Width?.ToString(CultureInfo.InvariantCulture),
            expectedTireCode.Width?.ToString(CultureInfo.InvariantCulture));
        parameterEvaluations.AddIfNotNull(widthEvaluation);

        var diameterEvaluation = GetParameterMatch(
            actualTireCode.Diameter?.ToString(CultureInfo.InvariantCulture),
            expectedTireCode.Diameter?.ToString(CultureInfo.InvariantCulture));
        parameterEvaluations.AddIfNotNull(diameterEvaluation);

        var aspectRatioEvaluation = GetParameterMatch(
            actualTireCode.AspectRatio?.ToString(CultureInfo.InvariantCulture),
            expectedTireCode.AspectRatio?.ToString(CultureInfo.InvariantCulture));
        parameterEvaluations.AddIfNotNull(aspectRatioEvaluation);

        var constructionEvaluation = GetParameterMatch(actualTireCode.Construction, expectedTireCode.Construction);
        parameterEvaluations.AddIfNotNull(constructionEvaluation);

        var loadRangeEvaluation = GetParameterMatch(
            actualTireCode.LoadRange?.ToString(CultureInfo.InvariantCulture),
            expectedTireCode.LoadRange?.ToString(CultureInfo.InvariantCulture));
        parameterEvaluations.AddIfNotNull(loadRangeEvaluation);

        var loadIndexEvaluation = GetParameterMatch(
            actualTireCode.LoadIndex?.ToString(CultureInfo.InvariantCulture),
            expectedTireCode.LoadIndex?.ToString(CultureInfo.InvariantCulture));
        parameterEvaluations.AddIfNotNull(loadIndexEvaluation);

        var loadIndex2Evaluation = GetParameterMatch(
            actualTireCode.LoadIndex2?.ToString(CultureInfo.InvariantCulture),
            expectedTireCode.LoadIndex2?.ToString(CultureInfo.InvariantCulture));
        parameterEvaluations.AddIfNotNull(loadIndex2Evaluation);

        var speedRatingEvaluation = GetParameterMatch(actualTireCode.SpeedRating, expectedTireCode.SpeedRating);
        parameterEvaluations.AddIfNotNull(speedRatingEvaluation);

        var actualTireCodeString = actualTireCode.ToString();
        var expectedTireCodeString = expectedTireCode.ToString();

        var totalDistance = parameterEvaluations.Sum(parameterEvaluation => parameterEvaluation.Distance);
        var estimatedAccuracy =
            GetAccuracyForLevenshteinDistance(totalDistance, actualTireCodeString, expectedTireCodeString);
        var fullMatchParameterCount = parameterEvaluations
            .Count(m => m.Distance == 0);

        return new EvaluationValueObject
        {
            ExpectedTireCode = expectedTireCode,
            TotalDistance = totalDistance,
            FullMatchParameterCount = fullMatchParameterCount,
            EstimatedAccuracy = estimatedAccuracy,
            VehicleClassEvaluation = vehicleClassEvaluation,
            WidthEvaluation = widthEvaluation,
            DiameterEvaluation = diameterEvaluation,
            AspectRatioEvaluation = aspectRatioEvaluation,
            ConstructionEvaluation = constructionEvaluation,
            LoadRangeEvaluation = loadRangeEvaluation,
            LoadIndexEvaluation = loadIndexEvaluation,
            LoadIndex2Evaluation = loadIndex2Evaluation,
            SpeedRatingEvaluation = speedRatingEvaluation
        };
    }

    private ParameterEvaluationValueObject? GetParameterMatch(string? parameter1, string? parameter2)
    {
        if (parameter1 is null && parameter2 is null)
            return null;
        var levenshtein = new Levenshtein(parameter1 ?? "");
        var distance = levenshtein.DistanceFrom(parameter2 ?? "");
        var estimatedAccuracy = GetAccuracyForLevenshteinDistance(distance, parameter1 ?? "", parameter2 ?? "");

        return new ParameterEvaluationValueObject
        {
            EstimatedAccuracy = estimatedAccuracy,
            Distance = distance
        };
    }

    private decimal GetAccuracyForLevenshteinDistance(int distance, string string1, string string2)
    {
        var stringLength = Math.Max((decimal)string1.Length, (decimal)string2.Length);
        if (stringLength == 0)
            return 0;
        return 1 - distance / stringLength;
    }
}