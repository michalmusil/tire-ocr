using AiPipeline.TireOcr.EvaluationTool.Application.Common.DataAccess;
using AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRunBatch.Dtos.BatchEvaluation;
using AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRunBatch.Services;
using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunBatchAggregate;
using TireOcr.Shared.Result;
using TireOcr.Shared.UseCase;

namespace AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRunBatch.Queries.GetEvaluationBatchMetricsCsvExport;

public class GetEvaluationBatchMetricsCsvExportQueryHandler
    : IQueryHandler<GetEvaluationBatchMetricsExportQuery, byte[]>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBatchCsvExportService _batchCsvExportService;
    private readonly IBatchEvaluationService _batchEvaluationService;

    public GetEvaluationBatchMetricsCsvExportQueryHandler(IUnitOfWork unitOfWork,
        IBatchCsvExportService batchCsvExportService, IBatchEvaluationService batchEvaluationService)
    {
        _unitOfWork = unitOfWork;
        _batchCsvExportService = batchCsvExportService;
        _batchEvaluationService = batchEvaluationService;
    }

    public async Task<DataResult<byte[]>> Handle(GetEvaluationBatchMetricsExportQuery request,
        CancellationToken cancellationToken)
    {
        var batch = await FetchBatchWithRuns(request.BatchId);
        if (batch is null)
            return DataResult<byte[]>.NotFound($"Batch with id '{request.BatchId}' not found");

        var otherBatch = request.OtherBatchId is { } otherId
            ? await FetchBatchWithRuns(otherId)
            : null;

        var hasIncalculableInputs = otherBatch is not null
                                    || request.ExpectedAnnualInferences is not null
                                    || request.AnnualFixedCost is not null;
        var incalculableInputs = hasIncalculableInputs
            ? new IncalculableInputsDto(
                AnnualFixedCostUsd: request.AnnualFixedCost,
                ExpectedAnnualInferences: request.ExpectedAnnualInferences,
                InferenceStabilityRelative: otherBatch)
            : null;

        var evaluationResult = await _batchEvaluationService.EvaluateBatch(batch, incalculableInputs);
        if (evaluationResult.IsFailure)
            return DataResult<byte[]>.Failure(evaluationResult.Failures);
        var evaluation = evaluationResult.Data!;

        if (otherBatch is not null && request.AverageMetricsWithOtherBatch)
        {
            var averagedMetricsEvaluationResult =
                await AverageEvaluationMetricsWithOtherBatch(evaluation, incalculableInputs, otherBatch);
            if (averagedMetricsEvaluationResult.IsFailure)
                return DataResult<byte[]>.Failure(averagedMetricsEvaluationResult.Failures);

            evaluation = averagedMetricsEvaluationResult.Data!;
        }

        var csvContent = await _batchCsvExportService.ExportBatchMetrics(evaluation);
        return csvContent.Map(
            onFailure: DataResult<byte[]>.Failure,
            onSuccess: DataResult<byte[]>.Success
        );
    }

    private async Task<EvaluationRunBatchEntity?> FetchBatchWithRuns(Guid batchId)
    {
        var batch = await _unitOfWork.EvaluationRunBatchRepository.GetEvaluationRunBatchByIdAsync(id: batchId);
        if (batch is null)
            return null;

        var runs = await _unitOfWork.EvaluationRunRepository.GetEvaluationRunsByBatchIdAsync(batch.Id);
        batch = new EvaluationRunBatchEntity(
            id: batch.Id,
            title: batch.Title,
            description: batch.Description,
            evaluationRuns: runs.ToList()
        );

        return batch;
    }

    private async Task<DataResult<BatchEvaluationDto>> AverageEvaluationMetricsWithOtherBatch(
        BatchEvaluationDto originalEvaluation,
        IncalculableInputsDto? originalIncalculableInputs,
        EvaluationRunBatchEntity otherBatch)
    {
        var otherBatchEvaluationResult = await _batchEvaluationService.EvaluateBatch(otherBatch);
        if (otherBatchEvaluationResult.IsFailure)
            return otherBatchEvaluationResult;
        var otherEvaluation = otherBatchEvaluationResult.Data!;

        var averageInferenceCost =
            (originalEvaluation.Metrics.AverageInferenceCost + otherEvaluation.Metrics.AverageInferenceCost) / 2;
        var estimatedAnnualCost = 0m;
        if (originalIncalculableInputs?.AnnualFixedCostUsd is { } fixedCostUsd)
            estimatedAnnualCost += fixedCostUsd;
        if (originalIncalculableInputs?.ExpectedAnnualInferences is { } expectedInferenceCount)
            estimatedAnnualCost += expectedInferenceCount * averageInferenceCost;

        var originalMetrics = originalEvaluation.Metrics;
        var otherMetrics = otherEvaluation.Metrics;
        var averagedEvaluation = originalEvaluation with
        {
            Metrics = new BatchEvaluationMetricsDto(
                ParameterSuccessRate: (originalMetrics.ParameterSuccessRate + otherMetrics.ParameterSuccessRate) / 2,
                FalsePositiveRate: (originalMetrics.FalsePositiveRate + otherMetrics.FalsePositiveRate) / 2,
                AverageCer: (originalMetrics.AverageCer + otherMetrics.AverageCer) / 2,
                AverageInferenceCost: averageInferenceCost,
                AverageLatencyMs: (originalMetrics.AverageLatencyMs + otherMetrics.AverageLatencyMs) / 2,
                EstimatedAnnualCostUsd: estimatedAnnualCost,
                InferenceStability: originalMetrics.InferenceStability
            )
        };
        return DataResult<BatchEvaluationDto>.Success(averagedEvaluation);
    }
}