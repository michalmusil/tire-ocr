using TireOcr.Shared.Result;

namespace AiPipeline.TireOcr.EvaluationTool.Application.Services;

public interface ICsvParserService
{
    public Task<DataResult<Dictionary<string, string?>>> ParseImageUrlLabelPairs(Stream csvStream);
}