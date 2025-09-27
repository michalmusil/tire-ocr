using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate;
using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate.DbMatch;
using AiPipeline.TireOcr.EvaluationTool.Domain.StepTypes;
using TireOcr.Shared.Result;

namespace AiPipeline.TireOcr.EvaluationTool.Application.Services.Processors;

public interface IDbMatchingProcessor
{
    public Task<DataResult<DbMatchingResultValueObject>> Process(OcrResultValueObject ocrResult,
        TireCodeValueObject postprocessingResult, DbMatchingType dbMatchingType);
}