using AiPipeline.TireOcr.EvaluationTool.Application.Common.Services;
using AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRun.Services.Processors;
using AiPipeline.TireOcr.EvaluationTool.Domain.StepTypes;
using AiPipeline.TireOcr.EvaluationTool.Infrastructure.EvaluationRun.Services.Processors.DbMatching;
using TireOcr.Shared.Result;

namespace AiPipeline.TireOcr.EvaluationTool.Infrastructure.EvaluationRun.Services.ProcessorMappers;

public class DbMatchingProcessorMapper : IEnumToObjectMapper<DbMatchingType, IDbMatchingProcessor>
{
    private readonly DbMatchingNoneProcessor _noneProcessor;
    private readonly DbMatchingRemoteProcessor _remoteProcessor;

    public DbMatchingProcessorMapper(DbMatchingNoneProcessor noneProcessor,
        DbMatchingRemoteProcessor remoteProcessor)
    {
        _noneProcessor = noneProcessor;
        _remoteProcessor = remoteProcessor;
    }

    public DataResult<IDbMatchingProcessor> Map(DbMatchingType input)
    {
        IDbMatchingProcessor? processor = input switch
        {
            DbMatchingType.None => _noneProcessor,
            DbMatchingType.TireCodeAndManufacturer => _remoteProcessor,
            _ => null
        };

        if (processor == null)
            return DataResult<IDbMatchingProcessor>.Invalid($"Invalid DbMatching type {input}");

        return DataResult<IDbMatchingProcessor>.Success(processor);
    }
}