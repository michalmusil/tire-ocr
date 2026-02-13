using System.Globalization;
using System.Text;
using AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRunBatch.Dtos.BatchEvaluation;
using AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRunBatch.Services;
using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunBatchAggregate;
using AiPipeline.TireOcr.EvaluationTool.Infrastructure.EvaluationRunBatch.Dtos;
using CsvHelper;
using TireOcr.Shared.Result;

namespace AiPipeline.TireOcr.EvaluationTool.Infrastructure.EvaluationRunBatch.Services;

public class BatchCsvExportService : IBatchCsvExportService
{
    public async Task<DataResult<byte[]>> ExportRawBatch(EvaluationRunBatchEntity evaluationRunBatch)
    {
        using var outputStream = new MemoryStream();
        await using var streamWriter = new StreamWriter(outputStream, Encoding.UTF8);
        await using var csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture);

        var runRecords = evaluationRunBatch.EvaluationRuns
            .Select(EvaluationRunCsvLineDto.FromDomain);
        await csvWriter.WriteRecordsAsync(runRecords);
        await streamWriter.FlushAsync();

        return DataResult<byte[]>.Success(outputStream.ToArray());
    }

    public async Task<DataResult<byte[]>> ExportBatchMetrics(BatchEvaluationDto batchEvaluationDto)
    {
        using var outputStream = new MemoryStream();
        await using var streamWriter = new StreamWriter(outputStream, Encoding.UTF8);
        await using var csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture);

        var metrics = batchEvaluationDto.Metrics;
        var records = new List<EvaluationMetricCsvLineDto>
        {
            new("Parameter Success Rate [ratio]", "PSR", metrics.ParameterSuccessRate),
            new("Average Latency [ms]", "AL", metrics.AverageLatencyMs),
            new("False Positive Rate [ratio]", "FPR", metrics.FalsePositiveRate),
            new("Character Error Rate [ratio]", "CER", metrics.AverageCer)
        };
        if (metrics.InferenceStability is { } inferenceStability)
            records.Add(new EvaluationMetricCsvLineDto("InferenceStability [ratio]", "IS", inferenceStability));
        if (metrics.EstimatedAnnualCostUsd is { } estimatedAnnualCostUsd)
            records.Add(new EvaluationMetricCsvLineDto("Estimated Annual Cost [USD]", "EAC", estimatedAnnualCostUsd));

        await csvWriter.WriteRecordsAsync(records);
        await streamWriter.FlushAsync();

        return DataResult<byte[]>.Success(outputStream.ToArray());
    }
}