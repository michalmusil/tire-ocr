using System.Collections.Concurrent;
using AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRunBatch.Dtos.BatchEvaluation;
using AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRunBatch.Services;
using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate;
using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunBatchAggregate;
using TireOcr.Shared.Result;

namespace AiPipeline.TireOcr.EvaluationTool.Infrastructure.EvaluationRunBatch.Services;

public class BatchEvaluationService : IBatchEvaluationService
{
    public async Task<DataResult<BatchEvaluationDto>> EvaluateBatch(EvaluationRunBatchEntity batch,
        IncalculableInputsDto? inputs = null)
    {
        if (batch.EvaluationRuns.Count < 1)
            return DataResult<BatchEvaluationDto>.Invalid(
                $"Batch {batch.Id} has no evaluation runs and can't be evaluated."
            );
        var totalRuns = batch.EvaluationRuns.Count;

        var fullyCorrectResultCount = 0;
        var correctMainParameterCount = 0;
        var insufficientExtractionCount = 0;
        var falsePositiveCount = 0;
        var failedPreprocessingCount = 0;
        var failedOcrCount = 0;
        var failedPostprocessingCount = 0;
        var failedUnexpectedCount = 0;

        List<int> totalDistances = [];
        List<int> vehicleClassDistances = [];
        List<int> widthDistances = [];
        List<int> diameterDistances = [];
        List<int> aspectRatioDistances = [];
        List<int> constructionDistances = [];
        List<int> loadRangeDistances = [];
        List<int> loadIndexDistances = [];
        List<int> loadIndex2Distances = [];
        List<int> speedRatingDistances = [];
        List<decimal> allCers = [];
        List<decimal> allParameterSuccessRates = [];
        List<decimal> allMeasuredInferenceCosts = [];
        List<decimal> allDurationsWithoutTraffic = [];

        foreach (var run in batch.EvaluationRuns)
        {
            switch (run.GetEvaluationResultCategory())
            {
                case EvaluationResultCategory.NoEvaluation:
                    break;
                case EvaluationResultCategory.FullyCorrect:
                    fullyCorrectResultCount++;
                    break;
                case EvaluationResultCategory.CorrectInMainParameters:
                    correctMainParameterCount++;
                    break;
                case EvaluationResultCategory.NoCodeDetectedPreprocessing:
                    failedPreprocessingCount++;
                    break;
                case EvaluationResultCategory.NoCodeDetectedOcr:
                    failedOcrCount++;
                    break;
                case EvaluationResultCategory.NoCodeDetectedPostprocessing:
                    failedPostprocessingCount++;
                    break;
                case EvaluationResultCategory.NoCodeDetectedUnexpected:
                    failedUnexpectedCount++;
                    break;
                case EvaluationResultCategory.InsufficientExtraction:
                    insufficientExtractionCount++;
                    break;
                case EvaluationResultCategory.FalsePositive:
                    falsePositiveCount++;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(
                        $"Encountered unexpected result category during batch evaluation of run {run.Id}"
                    );
            }

            var stats = CalculateStatsOfEvaluationRun(run);
            if (stats is null)
            {
                // Corrective measure: PSR includes non-successful inferences
                if (run.GetEvaluationResultCategory() is not EvaluationResultCategory.NoEvaluation)
                    allParameterSuccessRates.Add(0);
                continue;
            }

            totalDistances.Add(stats.TotalDistance);
            AddDistanceIfNotNull(stats.VehicleClassDistance, vehicleClassDistances);
            AddDistanceIfNotNull(stats.WidthDistance, widthDistances);
            AddDistanceIfNotNull(stats.DiameterDistance, diameterDistances);
            AddDistanceIfNotNull(stats.AspectRatioDistance, aspectRatioDistances);
            AddDistanceIfNotNull(stats.ConstructionDistance, constructionDistances);
            AddDistanceIfNotNull(stats.LoadRangeDistance, loadRangeDistances);
            AddDistanceIfNotNull(stats.LoadIndexDistance, loadIndexDistances);
            AddDistanceIfNotNull(stats.LoadIndex2Distance, loadIndex2Distances);
            AddDistanceIfNotNull(stats.SpeedRatingDistance, speedRatingDistances);

            allCers.Add(stats.Cer);
            allMeasuredInferenceCosts.Add(stats.InferenceCost);
            allParameterSuccessRates.Add(stats.ParameterSuccessRate);
            if (stats.TotalDurationWithoutTraffic is not null)
                allDurationsWithoutTraffic.Add(stats.TotalDurationWithoutTraffic.Value);
        }

        var estimatedExpenditurePer1000Requests = CalculateAverageExpenditurePer1000Requests(
            fixedCostsPer1000Requests: inputs?.FixedExpenditurePer1000Requests,
            measuredVariableCosts: allMeasuredInferenceCosts,
            includeVariableExpenditure: inputs?.AddVariableExpenditure ?? false
        );

        var batchEvaluation = new BatchEvaluationDto(
            Counts: new BatchEvaluationCountsDto(
                TotalCount: totalRuns,
                FullyCorrectCount: fullyCorrectResultCount,
                CorrectMainParametersCount: correctMainParameterCount,
                InsufficientExtractionCount: insufficientExtractionCount,
                FalsePositiveCount: falsePositiveCount,
                FailedPreprocessingCount: failedPreprocessingCount,
                FailedOcrCount: failedOcrCount,
                FailedPostprocessingCount: failedPostprocessingCount,
                FailedUnexpectedCount: failedUnexpectedCount
            ),
            Distances: new BatchEvaluationDistancesDto(
                AverageDistance: GetAverageDistance(totalDistances),
                AverageVehicleClassDistance: GetAverageDistance(vehicleClassDistances),
                AverageWidthDistance: GetAverageDistance(widthDistances),
                AverageDiameterDistance: GetAverageDistance(diameterDistances),
                AverageAspectRatioDistance: GetAverageDistance(aspectRatioDistances),
                AverageConstructionDistance: GetAverageDistance(constructionDistances),
                AverageLoadRangeDistance: GetAverageDistance(loadRangeDistances),
                AverageLoadIndexDistance: GetAverageDistance(loadIndexDistances),
                AverageLoadIndex2Distance: GetAverageDistance(loadIndex2Distances),
                AverageSpeedRatingDistance: GetAverageDistance(speedRatingDistances)
            ),
            Metrics: new BatchEvaluationMetricsDto(
                ParameterSuccessRate: SafeAverage(allParameterSuccessRates),
                FalsePositiveRate: (decimal)falsePositiveCount / (decimal)batch.EvaluationRuns.Count,
                AverageCer: SafeAverage(allCers),
                AverageVariableInferenceExpenditure: SafeAverage(allMeasuredInferenceCosts),
                TailLatencyMs: Percentile(allDurationsWithoutTraffic, 0.9m),
                InferenceStability: null,
                NormalizedInferenceExpenditure: estimatedExpenditurePer1000Requests
            )
        );

        return DataResult<BatchEvaluationDto>.Success(batchEvaluation);
    }

    public async Task<DataResult<BatchEvaluationDto>> EvaluateBatchWithRelatedBatches(EvaluationRunBatchEntity batch,
        List<EvaluationRunBatchEntity> relatedBatches, IncalculableInputsDto? inputs)
    {
        var mainBatchResult = await EvaluateBatch(batch, inputs);
        if (mainBatchResult.IsFailure)
            return mainBatchResult;

        var relatedBatchesResults = await Task.WhenAll(relatedBatches
            .Select(rb => EvaluateBatch(rb, inputs))
        );
        if (relatedBatchesResults.Any(r => r.IsFailure))
            return relatedBatchesResults.First();

        var mainBatchData = mainBatchResult.Data!;
        var relatedBatchData = relatedBatchesResults
            .Select(r => r.Data!.Metrics);

        List<EvaluationRunBatchEntity> allEvaluationRuns = [batch];
        allEvaluationRuns.AddRange(relatedBatches);
        var inferenceStability = await CalculateInferenceStabilityAsync(allEvaluationRuns);

        List<BatchEvaluationMetricsDto> allEvaluationMetrics = [mainBatchData.Metrics];
        allEvaluationMetrics.AddRange(relatedBatchData);

        return DataResult<BatchEvaluationDto>.Success(mainBatchData with
        {
            Metrics = new BatchEvaluationMetricsDto(
                InferenceStability: inferenceStability,
                ParameterSuccessRate: allEvaluationMetrics.Average(m => m.ParameterSuccessRate),
                FalsePositiveRate: allEvaluationMetrics.Average(m => m.FalsePositiveRate),
                AverageCer: allEvaluationMetrics.Average(m => m.AverageCer),
                TailLatencyMs: allEvaluationMetrics.Average(m => m.TailLatencyMs),
                AverageVariableInferenceExpenditure: allEvaluationMetrics.Average(m =>
                    m.AverageVariableInferenceExpenditure),
                NormalizedInferenceExpenditure: allEvaluationMetrics
                    .Where(m => m.NormalizedInferenceExpenditure is not null)
                    .Average(m => m.NormalizedInferenceExpenditure)
            )
        });
    }

    private void AddDistanceIfNotNull(int? distance, List<int> distances)
    {
        if (distance is not null)
            distances.Add(distance.Value);
    }

    private double GetAverageDistance(List<int> distances)
    {
        if (distances.Count == 0)
            return 0;

        return distances.Average();
    }

    private async Task<decimal> CalculateInferenceStabilityAsync(List<EvaluationRunBatchEntity> batches)
    {
        var referenceBatch = batches.First();
        var otherBatches = batches.Skip(1);
        var resultsDict = referenceBatch.EvaluationRuns
            .ToDictionary(e => e.InputImage.FileName, e => new
            {
                Categories = new List<EvaluationResultCategory> { e.GetEvaluationResultCategory() },
                RawCodes = new List<string?> { e.PostprocessingResult?.TireCode.RawCode.ToLower() }
            });
        foreach (var batch in otherBatches)
        {
            foreach (var er in batch.EvaluationRuns)
            {
                var exists = resultsDict.TryGetValue(er.InputImage.FileName, out var result);
                if (!exists)
                    continue;
                result!.Categories.Add(er.GetEvaluationResultCategory());
                result.RawCodes.Add(er.PostprocessingResult?.TireCode.RawCode.ToLower());
            }
        }

        var scores = new List<decimal>();
        foreach (var result in resultsDict.Values)
        {
            var imagePresentInAllBatches = result.Categories.Count == batches.Count;
            var allCategoriesMatch = result.Categories.Distinct().Count() == 1;
            var allCodesMatch = result.RawCodes.Distinct().Count() == 1;

            var isStable = imagePresentInAllBatches && allCategoriesMatch && allCodesMatch;
            scores.Add(isStable ? 1 : 0);
        }

        return SafeAverage(scores);
    }

    private decimal? CalculateAverageExpenditurePer1000Requests(decimal? fixedCostsPer1000Requests,
        List<decimal> measuredVariableCosts, bool includeVariableExpenditure)
    {
        if (!includeVariableExpenditure)
            return fixedCostsPer1000Requests;

        var averageVariableCostPer1000 = SafeAverage(measuredVariableCosts) * 1000;
        return averageVariableCostPer1000 + fixedCostsPer1000Requests;
    }

    private SuccessDependentStats? CalculateStatsOfEvaluationRun(EvaluationRunEntity run)
    {
        if (run.Evaluation is null || run.PostprocessingResult is null)
            return null;

        var category = run.GetEvaluationResultCategory();
        var inferenceCost = run.OcrResult?.EstimatedCost ?? 0;
        var parameterSuccessRate = 0m;
        if (category is EvaluationResultCategory.CorrectInMainParameters or EvaluationResultCategory.FullyCorrect)
        {
            var totalParametersInGt = run.Evaluation.ExpectedTireCode.GetNonNullParameterCount();
            var totalCorrectlyRecognizedParameters = run.PostprocessingResult.TireCode.GetNonNullParameterCount();
            parameterSuccessRate = (decimal)totalCorrectlyRecognizedParameters / (decimal)totalParametersInGt;
        }

        long? durationWithoutTrafic = null;
        if (category is EvaluationResultCategory.CorrectInMainParameters or EvaluationResultCategory.FullyCorrect
            or EvaluationResultCategory.FalsePositive or EvaluationResultCategory.InsufficientExtraction)
            durationWithoutTrafic = 0
                                    + (run.PreprocessingResult?.DurationMs ?? 0L)
                                    + (run.OcrResult?.DurationMs ?? 0L)
                                    + (run.PostprocessingResult?.DurationMs ?? 0L);


        return new SuccessDependentStats(
            TotalDistance: run.Evaluation.TotalDistance,
            VehicleClassDistance: run.Evaluation.VehicleClassEvaluation?.Distance,
            WidthDistance: run.Evaluation.WidthEvaluation?.Distance,
            DiameterDistance: run.Evaluation.DiameterEvaluation?.Distance,
            AspectRatioDistance: run.Evaluation.AspectRatioEvaluation?.Distance,
            ConstructionDistance: run.Evaluation.ConstructionEvaluation?.Distance,
            LoadRangeDistance: run.Evaluation.LoadRangeEvaluation?.Distance,
            LoadIndexDistance: run.Evaluation.LoadIndexEvaluation?.Distance,
            LoadIndex2Distance: run.Evaluation.LoadIndex2Evaluation?.Distance,
            SpeedRatingDistance: run.Evaluation.SpeedRatingEvaluation?.Distance,
            Cer: run.Evaluation.Cer,
            InferenceCost: inferenceCost,
            ParameterSuccessRate: parameterSuccessRate,
            TotalDurationWithoutTraffic: durationWithoutTrafic
        );
    }

    private decimal SafeAverage(IEnumerable<decimal> elements)
    {
        var list = elements.ToList();
        return list.Count == 0 ? 0 : list.Average();
    }

    // See: https://en.wikipedia.org/wiki/Percentile
    private decimal Percentile(List<decimal> elements, decimal percentile)
    {
        if (elements.Count == 0)
            throw new ArgumentException("Percentile input collection cannot be empty.");
        if (percentile is <= 0 or >= 1)
            throw new ArgumentException("Percentile must be < 0 and < 1.");

        var sortedElements = elements.OrderBy(x => x).ToArray();
        var length = sortedElements.Length;

        var indexUnbound = (length - 1) * percentile;
        var lowerIndex = (int)Math.Floor(indexUnbound);
        var upperIndex = (int)Math.Ceiling(indexUnbound);

        if (lowerIndex == upperIndex)
            return sortedElements[lowerIndex];

        var fraction = indexUnbound - lowerIndex;
        return sortedElements[lowerIndex] + (sortedElements[upperIndex] - sortedElements[lowerIndex]) * fraction;
    }

    private record SuccessDependentStats(
        int TotalDistance,
        int? VehicleClassDistance,
        int? WidthDistance,
        int? DiameterDistance,
        int? AspectRatioDistance,
        int? ConstructionDistance,
        int? LoadRangeDistance,
        int? LoadIndexDistance,
        int? LoadIndex2Distance,
        int? SpeedRatingDistance,
        decimal Cer,
        decimal InferenceCost,
        decimal ParameterSuccessRate,
        long? TotalDurationWithoutTraffic
    );
}