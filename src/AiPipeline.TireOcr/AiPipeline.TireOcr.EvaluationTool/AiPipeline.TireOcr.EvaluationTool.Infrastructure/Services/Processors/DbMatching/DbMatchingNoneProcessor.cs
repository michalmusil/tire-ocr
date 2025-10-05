using AiPipeline.TireOcr.EvaluationTool.Application.Services.Processors;
using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate;
using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate.DbMatch;
using AiPipeline.TireOcr.EvaluationTool.Domain.StepTypes;
using TireOcr.Shared.Result;

namespace AiPipeline.TireOcr.EvaluationTool.Infrastructure.Services.Processors.DbMatching;

public class DbMatchingNoneProcessor : IDbMatchingProcessor
{
    public Task<DataResult<DbMatchingResultEntity>> Process(OcrResultEntity ocrResult,
        TireCodeValueObject postprocessingResult, DbMatchingType dbMatchingType) =>
        Task.FromResult(
            DataResult<DbMatchingResultEntity>.Failure(new Failure(204, "Db matching was not requested")));
}