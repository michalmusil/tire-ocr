using AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRunBatch.Dtos.BatchEvaluation;
using AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRunBatch.Services;
using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate;
using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate.Evaluation;
using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunBatchAggregate;
using TireOcr.Shared.Result;

namespace AiPipeline.TireOcr.EvaluationTool.Infrastructure.EvaluationRunBatch.Services;

public class BatchEvaluationService : IBatchEvaluationService
{
    public async Task<DataResult<BatchEvaluationDto>> EvaluateBatch(EvaluationRunBatchEntity batch)
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
        List<decimal> allInferenceCosts = [];
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
                continue;

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
            allInferenceCosts.Add(stats.InferenceCost);
            allParameterSuccessRates.Add(stats.ParameterSuccessRate);
            if (stats.TotalDurationWithoutTraffic is not null)
                allDurationsWithoutTraffic.Add(stats.TotalDurationWithoutTraffic.Value);
        }

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
            Statistics: new BatchEvaluationStatisticsDto(
                ParameterSuccessRate: allParameterSuccessRates.Average(),
                FalsePositiveRate: (decimal)falsePositiveCount / (decimal)batch.EvaluationRuns.Count,
                AverageCer: allCers.Average(),
                AverageInferenceCost: allInferenceCosts.Average(),
                AverageLatencyMs: allDurationsWithoutTraffic.Average()
            )
        );

        return DataResult<BatchEvaluationDto>.Success(batchEvaluation);
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

    private RunStatCalculation? CalculateStatsOfEvaluationRun(EvaluationRunEntity run)
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


        return new RunStatCalculation(
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

    private record RunStatCalculation(
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