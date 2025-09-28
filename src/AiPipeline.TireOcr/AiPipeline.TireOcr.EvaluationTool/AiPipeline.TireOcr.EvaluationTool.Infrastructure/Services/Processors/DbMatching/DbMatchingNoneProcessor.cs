using AiPipeline.TireOcr.EvaluationTool.Application.Services.Processors;
using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate;
using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate.DbMatch;
using AiPipeline.TireOcr.EvaluationTool.Domain.StepTypes;
using TireOcr.Shared.Result;

namespace AiPipeline.TireOcr.EvaluationTool.Infrastructure.Services.Processors.DbMatching;

public class DbMatchingNoneProcessor : IDbMatchingProcessor
{
    public Task<DataResult<DbMatchingResultValueObject>> Process(OcrResultValueObject ocrResult,
        TireCodeValueObject postprocessingResult, DbMatchingType dbMatchingType) =>
        Task.FromResult(
            DataResult<DbMatchingResultValueObject>.Success(new DbMatchingResultValueObject
            {
                Matches = [],
                ManufacturerMatch = null,
                DurationMs = 0
            })
        );
}