using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate;
using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate.DbMatch;
using AiPipeline.TireOcr.EvaluationTool.Domain.StepTypes;
using TireOcr.Shared.Result;

namespace AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRun.Services.Processors;

public interface IDbMatchingProcessor
{
    public Task<DataResult<DbMatchingResultEntity>> Process(OcrResultEntity ocrResult,
        TireCodeValueObject postprocessingResult, DbMatchingType dbMatchingType);
}