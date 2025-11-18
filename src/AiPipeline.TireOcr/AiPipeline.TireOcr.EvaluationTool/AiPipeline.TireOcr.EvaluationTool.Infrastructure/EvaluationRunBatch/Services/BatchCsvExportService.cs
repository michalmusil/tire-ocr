using System.Globalization;
using System.Text;
using AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRunBatch.Services;
using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunBatchAggregate;
using AiPipeline.TireOcr.EvaluationTool.Infrastructure.EvaluationRunBatch.Dtos;
using CsvHelper;
using TireOcr.Shared.Result;

namespace AiPipeline.TireOcr.EvaluationTool.Infrastructure.EvaluationRunBatch.Services;

public class BatchCsvExportService : IBatchCsvExportService
{
    public async Task<DataResult<byte[]>> ExportBatch(EvaluationRunBatchEntity evaluationRunBatch)
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
}