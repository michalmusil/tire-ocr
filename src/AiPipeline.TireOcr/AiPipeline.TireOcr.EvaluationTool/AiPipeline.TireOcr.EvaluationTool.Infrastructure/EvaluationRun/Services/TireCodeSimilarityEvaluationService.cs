using System.Globalization;
using AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRun.Services;
using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate;
using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate.Evaluation;
using AiPipeline.TireOcr.EvaluationTool.Infrastructure.Common.Extensions;
using Fastenshtein;

namespace AiPipeline.TireOcr.EvaluationTool.Infrastructure.EvaluationRun.Services;

public class TireCodeSimilarityEvaluationService : ITireCodeSimilarityEvaluationService
{
    public async Task<EvaluationEntity> EvaluateTireCodeSimilarity(TireCodeValueObject expectedTireCode,
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

        return new EvaluationEntity
        (
            evaluationRunId: Guid.Empty,
            expectedTireCode: expectedTireCode,
            totalDistance: totalDistance,
            fullMatchParameterCount: fullMatchParameterCount,
            estimatedAccuracy: estimatedAccuracy,
            vehicleClassEvaluation: vehicleClassEvaluation,
            widthEvaluation: widthEvaluation,
            diameterEvaluation: diameterEvaluation,
            aspectRatioEvaluation: aspectRatioEvaluation,
            constructionEvaluation: constructionEvaluation,
            loadRangeEvaluation: loadRangeEvaluation,
            loadIndexEvaluation: loadIndexEvaluation,
            loadIndex2Evaluation: loadIndex2Evaluation,
            speedRatingEvaluation: speedRatingEvaluation
        );
    }

    private ParameterEvaluationValueObject? GetParameterMatch(string? extracted, string? groundTruth)
    {
        if (extracted is null && groundTruth is null)
            return null;
        var levenshtein = new Levenshtein(extracted ?? "");
        var distance = levenshtein.DistanceFrom(groundTruth ?? "");
        var estimatedAccuracy = GetAccuracyForLevenshteinDistance(distance, extracted ?? "", groundTruth ?? "");
        // var cer = groundTruth is null ? 1.0m : (decimal)distance / (decimal)groundTruth.Length;

        return new ParameterEvaluationValueObject
        {
            EstimatedAccuracy = estimatedAccuracy,
            Distance = distance,
            // Cer = cer
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