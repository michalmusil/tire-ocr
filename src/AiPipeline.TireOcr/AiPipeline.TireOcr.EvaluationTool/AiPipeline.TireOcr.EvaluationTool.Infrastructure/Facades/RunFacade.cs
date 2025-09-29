using AiPipeline.TireOcr.EvaluationTool.Application.Dtos;
using AiPipeline.TireOcr.EvaluationTool.Application.Dtos.EvaluationRun;
using AiPipeline.TireOcr.EvaluationTool.Application.Facades;
using AiPipeline.TireOcr.EvaluationTool.Application.Services;
using AiPipeline.TireOcr.EvaluationTool.Application.Services.Processors;
using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate;
using AiPipeline.TireOcr.EvaluationTool.Domain.StepTypes;
using AiPipeline.TireOcr.EvaluationTool.Infrastructure.Extensions;
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

    public async Task<DataResult<EvaluationRun>> RunSingleEvaluationAsync(ImageDto image, RunConfigDto runConfig,
        TireCodeValueObject? expectedTireCode, RunEntityInputDetailsDto? runEntityInputDetailsDto)
    {
        var preprocessingProcessorResult = _preprocessingMapper.Map(runConfig.PreprocessingType);
        var ocrProcessorResult = _ocrMapper.Map(runConfig.OcrType);
        var postprocessingProcessorResult = _postprocessingMapper.Map(runConfig.PostprocessingType);
        var dbMatchingProcessorResult = _dbMatchingMapper.Map(runConfig.DbMatchingType);

        var processorMappingFailures = preprocessingProcessorResult.Failures
            .Concat(ocrProcessorResult.Failures)
            .Concat(postprocessingProcessorResult.Failures)
            .Concat(dbMatchingProcessorResult.Failures)
            .ToArray();
        if (processorMappingFailures.Any())
            return DataResult<EvaluationRun>.Failure(processorMappingFailures);

        var evaluationRun = new EvaluationRun(
            inputImage: image.ToDomain(),
            title: runEntityInputDetailsDto?.Title,
            id: runEntityInputDetailsDto?.Id,
            expectedPostprocessingResult: expectedTireCode,
            preprocessingType: runConfig.PreprocessingType,
            ocrType: runConfig.OcrType,
            postprocessingType: runConfig.PostprocessingType,
            dbMatchingType: runConfig.DbMatchingType
        );

        var preprocessingResult = await preprocessingProcessorResult.Data!.Process(
            image: image,
            preprocessingType: runConfig.PreprocessingType
        );
        if (preprocessingResult.IsSuccess)
            evaluationRun.SetPreprocessingResult(new PreprocessingResultValueObject
            {
                PreprocessingResult = preprocessingResult.Data!.Image.ToDomain(),
                DurationMs = preprocessingResult.Data!.DurationMs
            });
        else
        {
            evaluationRun.SetFailure(
                EvaluationRunFailure.PreprocessingFailure(preprocessingResult.PrimaryFailure!)
            );
            return DataResult<EvaluationRun>.Success(evaluationRun);
        }

        var ocrResult = await ocrProcessorResult.Data!.Process(
            image: preprocessingResult.Data!.Image,
            ocrType: runConfig.OcrType
        );
        if (ocrResult.IsSuccess)
            evaluationRun.SetOcrResult(ocrResult.Data!);
        else
        {
            evaluationRun.SetFailure(
                EvaluationRunFailure.OcrFailure(ocrResult.PrimaryFailure!)
            );
            return DataResult<EvaluationRun>.Success(evaluationRun);
        }

        var postprocessingResult = await postprocessingProcessorResult.Data!.Process(
            ocrResult: ocrResult.Data!,
            postprocessingType: runConfig.PostprocessingType
        );
        if (postprocessingResult.IsSuccess)
            evaluationRun.SetPostprocessingResult(postprocessingResult.Data!);
        else
        {
            evaluationRun.SetFailure(
                EvaluationRunFailure.PostprocessingFailure(postprocessingResult.PrimaryFailure!)
            );
            return DataResult<EvaluationRun>.Success(evaluationRun);
        }

        var dbMatchingResult = await dbMatchingProcessorResult.Data!.Process(
            ocrResult: ocrResult.Data!,
            postprocessingResult: postprocessingResult.Data!.TireCode,
            dbMatchingType: runConfig.DbMatchingType
        );
        if (dbMatchingResult.IsSuccess)
            evaluationRun.SetDbMatchingResult(dbMatchingResult.Data!);
        else
            evaluationRun.SetFinishedAt(DateTime.UtcNow);

        return DataResult<EvaluationRun>.Success(evaluationRun);
    }

    public async Task<DataResult<EvaluationRun>> RunSingleEvaluationAsync(string imageUrl, RunConfigDto runConfig,
        TireCodeValueObject? expectedTireCode, RunEntityInputDetailsDto? runEntityInputDetailsDto)
    {
        var imageResult = await _imageDownloadService.DownloadImageAsync(imageUrl);
        if (imageResult.IsFailure)
            return DataResult<EvaluationRun>.Failure(imageResult.Failures);

        return await RunSingleEvaluationAsync(
            image: imageResult.Data!,
            runConfig: runConfig,
            expectedTireCode: expectedTireCode,
            runEntityInputDetailsDto: runEntityInputDetailsDto
        );
    }

    public async Task<DataResult<EvaluationRunBatch>> RunEvaluationBatchAsync(
        Dictionary<string, TireCodeValueObject?> imageUrls,
        int batchSize,
        RunConfigDto runConfig,
        RunEntityInputDetailsDto? runEntityInputDetailsDto
    )
    {
        var result = new EvaluationRunBatch(
            evaluationRuns: [],
            title: runEntityInputDetailsDto?.Title,
            id: runEntityInputDetailsDto?.Id
        );

        var processedBatches = imageUrls.ToList().GetSubLists(batchSize);
        foreach (var batch in processedBatches)
        {
            var batchUrls = batch.ToDictionary();
            var processedBatch = await ProcessOcrPipelineBatch(
                imageUrls: batchUrls,
                runConfig: runConfig
            );

            result.AddEvaluationRuns(processedBatch.ToArray());
        }

        return DataResult<EvaluationRunBatch>.Success(result);
    }

    private async Task<IEnumerable<EvaluationRun>> ProcessOcrPipelineBatch(
        Dictionary<string, TireCodeValueObject?> imageUrls,
        RunConfigDto runConfig
    )
    {
        var downloadedImages = (await _imageDownloadService
                .DownloadImageBatchAsync(imageUrls.Keys))
            .Where(x => x.Value.IsSuccess)
            .ToDictionary(x => x.Key, x => x.Value.Data!);

        var results = new List<EvaluationRun>();
        var batchProcessingTasks = imageUrls
            .Select(async x =>
            {
                var imageUrl = x.Key;
                var expectedTireCode = x.Value;
                var successfullyDownloaded = downloadedImages.TryGetValue(imageUrl, out var downloadedImage);

                var fallbackEvaluationRun = new EvaluationRun(
                    title: null,
                    inputImage: downloadedImage?.ToDomain() ?? new()
                    {
                        FileName = imageUrl,
                        ContentType = string.Empty,
                    },
                    preprocessingType: runConfig.PreprocessingType,
                    ocrType: runConfig.OcrType,
                    postprocessingType: runConfig.PostprocessingType,
                    dbMatchingType: runConfig.DbMatchingType,
                    expectedPostprocessingResult: expectedTireCode
                );

                if (!successfullyDownloaded)
                {
                    fallbackEvaluationRun.SetFailure(
                        EvaluationRunFailure.UnexpectedFailure(
                            new Failure(500, $"Failed to download image '{imageUrl}'")
                        )
                    );
                    results.Add(fallbackEvaluationRun);
                }

                var result = await RunSingleEvaluationAsync(
                    image: downloadedImage!,
                    runConfig: runConfig,
                    expectedTireCode: expectedTireCode,
                    runEntityInputDetailsDto: null
                );

                results.Add(
                    result.Map(
                        onSuccess: r => r,
                        onFailure: unexpectedFailures =>
                        {
                            fallbackEvaluationRun.SetFailure(
                                EvaluationRunFailure.UnexpectedFailure(unexpectedFailures.First())
                            );
                            return fallbackEvaluationRun;
                        }
                    )
                );
            });

        await Task.WhenAll(batchProcessingTasks);
        return results;
    }
}