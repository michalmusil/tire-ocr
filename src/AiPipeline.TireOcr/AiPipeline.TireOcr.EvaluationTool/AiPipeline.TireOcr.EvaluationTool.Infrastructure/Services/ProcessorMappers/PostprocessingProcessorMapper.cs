using AiPipeline.TireOcr.EvaluationTool.Application.Services;
using AiPipeline.TireOcr.EvaluationTool.Application.Services.Processors;
using AiPipeline.TireOcr.EvaluationTool.Domain.StepTypes;
using AiPipeline.TireOcr.EvaluationTool.Infrastructure.Services.Processors.Postprocessing;
using TireOcr.Shared.Result;

namespace AiPipeline.TireOcr.EvaluationTool.Infrastructure.Services.ProcessorMappers;

public class PostprocessingProcessorMapper : IEnumToObjectMapper<PostprocessingType, IPostprocessingProcessor>
{
    private readonly PostprocessingRemoteServiceProcessor _postprocessingRemoteServiceProcessor;

    public PostprocessingProcessorMapper(PostprocessingRemoteServiceProcessor postprocessingRemoteServiceProcessor)
    {
        _postprocessingRemoteServiceProcessor = postprocessingRemoteServiceProcessor;
    }

    public DataResult<IPostprocessingProcessor> Map(PostprocessingType input)
    {
        IPostprocessingProcessor? processor = input switch
        {
            PostprocessingType.SimpleExtractValues => _postprocessingRemoteServiceProcessor,
            _ => null
        };

        if (processor == null)
            return DataResult<IPostprocessingProcessor>.Invalid($"Invalid postprocessing type {input}");

        return DataResult<IPostprocessingProcessor>.Success(processor);
    }
}