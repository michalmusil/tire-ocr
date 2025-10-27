using System.Globalization;
using AiPipeline.TireOcr.EvaluationTool.Application.Common.Services;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using TireOcr.Shared.Result;

namespace AiPipeline.TireOcr.EvaluationTool.Infrastructure.Common.Services;

public class CsvParserService : ICsvParserService
{
    private readonly CsvConfiguration _csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
        { HasHeaderRecord = false, };

    private record ImageUrlLabelPair
    {
        [Index(0)] public required string ImageUrl { get; init; }
        [Index(1)] public string? Label { get; init; }
    }

    public async Task<DataResult<Dictionary<string, string?>>> ParseImageUrlLabelPairs(Stream csvStream)
    {
        Dictionary<string, string?> pairs = new();
        try
        {
            await using var originalStream = csvStream;
            using var originalStreamReader = new StreamReader(originalStream);
            using var csvReader = new CsvReader(originalStreamReader, _csvConfig);
            var records = csvReader.GetRecordsAsync<ImageUrlLabelPair>();
            await foreach (var record in records)
            {
                var label = record.Label?.Length == 0 ? null : record.Label;
                pairs.Add(record.ImageUrl, label);
            }

            return DataResult<Dictionary<string, string?>>.Success(pairs);
        }
        catch
        {
            return DataResult<Dictionary<string, string?>>.Invalid(
                "Failed to parse csv with image urls and labels. Ensure that your CSV is in the correct format, meaning no header and one image URL(mandatory) and one label(optional) per line.");
        }
    }
}