using AiPipeline.TireOcr.EvaluationTool.Application.Dtos.BatchEvaluation;
using AiPipeline.TireOcr.EvaluationTool.Application.Services;
using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate.Evaluation;
using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate.RunFailure;
using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunBatchAggregate;
using TireOcr.Shared.Result;

namespace AiPipeline.TireOcr.EvaluationTool.Infrastructure.Services;

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

        foreach (var run in batch.EvaluationRuns)
        {
            if (run.RunFailure is not null)
            {
                switch (run.RunFailure.Reason)
                {
                    case EvaluationRunFailureReason.Preprocessing:
                        failedPreprocessingCount++;
                        break;
                    case EvaluationRunFailureReason.Ocr:
                        failedOcrCount++;
                        break;
                    case EvaluationRunFailureReason.Postprocessing:
                        failedPostprocessingCount++;
                        break;
                    case EvaluationRunFailureReason.Unexpected:
                        failedUnexpectedCount++;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                continue;
            }

            // Successful runs with no evaluation are skipped
            if (run.Evaluation is null)
                continue;

            var evaluation = run.Evaluation!;

            var isFullyCorrect = evaluation.TotalDistance == 0;
            var areMainParametersCorrect =
                evaluation.WidthEvaluation?.Distance == 0 &&
                evaluation.DiameterEvaluation?.Distance == 0 &&
                evaluation.AspectRatioEvaluation?.Distance == 0 &&
                evaluation.ConstructionEvaluation?.Distance == 0;

            if (isFullyCorrect)
                fullyCorrectResultCount++;
            else if (areMainParametersCorrect)
                correctMainParameterCount++;

            totalDistances.Add(evaluation.TotalDistance);
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
            Counts: new BatchEvaluationCountsDto(
                TotalCount: totalRuns,
                FullyCorrectCount: fullyCorrectResultCount,
                CorrectMainParametersCount: correctMainParameterCount,
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