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

        int fullyCorrectResultCount = 0;
        int correctMainParameterCount = 0;
        int insufficientExtractionCount = 0;
        int falsePositiveCount = 0;
        int failedPreprocessingCount = 0;
        int failedOcrCount = 0;
        int failedPostprocessingCount = 0;
        int failedUnexpectedCount = 0;

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
                    throw new ArgumentOutOfRangeException();
            }

            // Successful runs with no evaluation are skipped
            var evaluation = run.Evaluation;
            if (evaluation is null)
                continue;


            totalDistances.Add(evaluation.TotalDistance);
            allCers.Add(evaluation.Cer);
            AddParameterEvaluationDistance(evaluation.VehicleClassEvaluation, vehicleClassDistances);
            AddParameterEvaluationDistance(evaluation.WidthEvaluation, widthDistances);
            AddParameterEvaluationDistance(evaluation.DiameterEvaluation, diameterDistances);
            AddParameterEvaluationDistance(evaluation.AspectRatioEvaluation, aspectRatioDistances);
            AddParameterEvaluationDistance(evaluation.ConstructionEvaluation, constructionDistances);
            AddParameterEvaluationDistance(evaluation.LoadRangeEvaluation, loadRangeDistances);
            AddParameterEvaluationDistance(evaluation.LoadIndexEvaluation, loadIndexDistances);
            AddParameterEvaluationDistance(evaluation.LoadIndex2Evaluation, loadIndex2Distances);
            AddParameterEvaluationDistance(evaluation.SpeedRatingEvaluation, speedRatingDistances);
        }

        var batchEvaluation = new BatchEvaluationDto(
            AverageCer: allCers.Count == 0 ? 0 : allCers.Average(),
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
            )
        );

        return DataResult<BatchEvaluationDto>.Success(batchEvaluation);
    }

    private void AddParameterEvaluationDistance(ParameterEvaluationValueObject? evaluation, List<int> distances)
    {
        if (evaluation is not null)
            distances.Add(evaluation.Distance);
    }

    private double GetAverageDistance(List<int> distances)
    {
        if (distances.Count == 0)
            return 0;

        return distances.Average();
    }
}