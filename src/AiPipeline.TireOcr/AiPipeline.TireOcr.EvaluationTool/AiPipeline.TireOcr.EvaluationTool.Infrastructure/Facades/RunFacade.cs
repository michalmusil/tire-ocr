using AiPipeline.TireOcr.EvaluationTool.Application.Dtos;
using AiPipeline.TireOcr.EvaluationTool.Application.Dtos.EvaluationRun;
using AiPipeline.TireOcr.EvaluationTool.Application.Facades;
using AiPipeline.TireOcr.EvaluationTool.Application.Services;
using AiPipeline.TireOcr.EvaluationTool.Application.Services.Processors;
using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate;
using AiPipeline.TireOcr.EvaluationTool.Domain.StepTypes;
using TireOcr.Shared.Result;

namespace AiPipeline.TireOcr.EvaluationTool.Infrastructure.Facades;

public class RunFacade : IRunFacade
{
    private readonly IImageDownloadService _imageDownloadService;

    private readonly IEnumToObjectMapper<PreprocessingType, IPreprocessingProcessor> _preprocessingMapper;
    private readonly IEnumToObjectMapper<OcrType, IOcrProcessor> _ocrMapper;
    private readonly IEnumToObjectMapper<PostprocessingType, IPostprocessingProcessor> _postprocessingMapper;
    private readonly IEnumToObjectMapper<DbMatchingType, IDbMatchingProcessor> _dbMatchingMapper;

    public RunFacade(IImageDownloadService imageDownloadService,
        IEnumToObjectMapper<PreprocessingType, IPreprocessingProcessor> preprocessingMapper,
        IEnumToObjectMapper<OcrType, IOcrProcessor> ocrMapper,
        IEnumToObjectMapper<PostprocessingType, IPostprocessingProcessor> postprocessingMapper,
        IEnumToObjectMapper<DbMatchingType, IDbMatchingProcessor> dbMatchingMapper)
    {
        _imageDownloadService = imageDownloadService;
        _preprocessingMapper = preprocessingMapper;
        _ocrMapper = ocrMapper;
        _postprocessingMapper = postprocessingMapper;
        _dbMatchingMapper = dbMatchingMapper;
    }

    public Task<DataResult<EvaluationRun>> RunSingleEvaluationAsync(ImageDto image, RunConfigDto runConfig,
        TireCodeValueObject? expectedTireCode)
    {
        throw new NotImplementedException();
    }

    public Task<DataResult<EvaluationRun>> RunSingleEvaluationAsync(string imageUrl, RunConfigDto runConfig,
        TireCodeValueObject? expectedTireCode)
    {
        throw new NotImplementedException();
    }

    public Task<DataResult<EvaluationRunBatch>> RunEvaluationBatchAsync(
        Dictionary<string, TireCodeValueObject?> imageUrls, int batchSize, RunConfigDto runConfig)
    {
        throw new NotImplementedException();
    }
}