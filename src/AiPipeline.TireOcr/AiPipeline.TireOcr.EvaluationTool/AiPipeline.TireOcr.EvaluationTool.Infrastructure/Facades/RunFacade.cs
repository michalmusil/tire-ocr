using AiPipeline.TireOcr.EvaluationTool.Application.Dtos;
using AiPipeline.TireOcr.EvaluationTool.Application.Dtos.EvaluationRun;
using AiPipeline.TireOcr.EvaluationTool.Application.Facades;
using AiPipeline.TireOcr.EvaluationTool.Application.Services;
using AiPipeline.TireOcr.EvaluationTool.Application.Services.Processors;
using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate;
using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate.RunFailure;
using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunBatchAggregate;
using AiPipeline.TireOcr.EvaluationTool.Domain.StepTypes;
using AiPipeline.TireOcr.EvaluationTool.Infrastructure.Extensions;
using Microsoft.Extensions.Logging;
using TireOcr.Shared.Result;

namespace AiPipeline.TireOcr.EvaluationTool.Infrastructure.Facades;

public class RunFacade : IRunFacade
{
    private readonly IImageDownloadService _imageDownloadService;
    private readonly ITireCodeSimilarityEvaluationService _evaluationService;

    private readonly IEnumToObjectMapper<PreprocessingType, IPreprocessingProcessor> _preprocessingMapper;
    private readonly IEnumToObjectMapper<OcrType, IOcrProcessor> _ocrMapper;
    private readonly IEnumToObjectMapper<PostprocessingType, IPostprocessingProcessor> _postprocessingMapper;
    private readonly IEnumToObjectMapper<DbMatchingType, IDbMatchingProcessor> _dbMatchingMapper;
    private readonly ILogger<RunFacade> _logger;

    public RunFacade(IImageDownloadService imageDownloadService,
        IEnumToObjectMapper<PreprocessingType, IPreprocessingProcessor> preprocessingMapper,
        IEnumToObjectMapper<OcrType, IOcrProcessor> ocrMapper,
        IEnumToObjectMapper<PostprocessingType, IPostprocessingProcessor> postprocessingMapper,
        IEnumToObjectMapper<DbMatchingType, IDbMatchingProcessor> dbMatchingMapper,
        ITireCodeSimilarityEvaluationService evaluationService, ILogger<RunFacade> logger)
    {
        _imageDownloadService = imageDownloadService;
        _preprocessingMapper = preprocessingMapper;
        _ocrMapper = ocrMapper;
        _postprocessingMapper = postprocessingMapper;
        _dbMatchingMapper = dbMatchingMapper;
        _evaluationService = evaluationService;
        _logger = logger;
    }

    public async Task<DataResult<EvaluationRunEntity>> RunSingleEvaluationAsync(ImageDto image, RunConfigDto runConfig,
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
            return DataResult<EvaluationRunEntity>.Failure(processorMappingFailures);

        var evaluationRun = new EvaluationRunEntity(
            inputImage: image.ToDomain(),
            title: runEntityInputDetailsDto?.Title,
            id: runEntityInputDetailsDto?.Id,
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
        {
            var domainResult = preprocessingResult.Data!.ToDomain();
            domainResult.SetEvaluationRunId(evaluationRun.Id);
            evaluationRun.SetPreprocessingResult(domainResult);
        }
        else
        {
            evaluationRun.SetFailure(
                EvaluationRunFailureValueObject.PreprocessingFailure(preprocessingResult.PrimaryFailure!)
            );
            return DataResult<EvaluationRunEntity>.Success(evaluationRun);
        }

        var ocrResult = await ocrProcessorResult.Data!.Process(
            image: preprocessingResult.Data!.Image,
            ocrType: runConfig.OcrType
        );
        if (ocrResult.IsSuccess)
        {
            var domainResult = ocrResult.Data!;
            domainResult.SetEvaluationRunId(evaluationRun.Id);
            evaluationRun.SetOcrResult(domainResult);
        }
        else
        {
            evaluationRun.SetFailure(
                EvaluationRunFailureValueObject.OcrFailure(ocrResult.PrimaryFailure!)
            );
            return DataResult<EvaluationRunEntity>.Success(evaluationRun);
        }

        var postprocessingResult = await postprocessingProcessorResult.Data!.Process(
            ocrResult: ocrResult.Data!,
            postprocessingType: runConfig.PostprocessingType
        );
        if (postprocessingResult.IsSuccess)
        {
            var domainResult = postprocessingResult.Data!;
            domainResult.SetEvaluationRunId(evaluationRun.Id);
            evaluationRun.SetPostprocessingResult(domainResult);
            if (expectedTireCode is not null)
            {
                var evaluation = await _evaluationService.EvaluateTireCodeSimilarity(
                    expectedTireCode: expectedTireCode,
                    actualTireCode: domainResult.TireCode
                );
                evaluation.SetEvaluationRunId(evaluationRun.Id);
                evaluationRun.SetEvaluation(evaluation);
            }
        }
        else
        {
            evaluationRun.SetFailure(
                EvaluationRunFailureValueObject.PostprocessingFailure(postprocessingResult.PrimaryFailure!)
            );
            return DataResult<EvaluationRunEntity>.Success(evaluationRun);
        }

        var dbMatchingResult = await dbMatchingProcessorResult.Data!.Process(
            ocrResult: ocrResult.Data!,
            postprocessingResult: postprocessingResult.Data!.TireCode,
            dbMatchingType: runConfig.DbMatchingType
        );
        if (dbMatchingResult.IsSuccess)
        {
            var domainResult = dbMatchingResult.Data!;
            domainResult.SetEvaluationRunId(evaluationRun.Id);
            evaluationRun.SetDbMatchingResult(domainResult);
        }
        else
            evaluationRun.SetFinishedAt(DateTime.UtcNow);

        return DataResult<EvaluationRunEntity>.Success(evaluationRun);
    }

    public async Task<DataResult<EvaluationRunEntity>> RunSingleEvaluationAsync(string imageUrl, RunConfigDto runConfig,
        TireCodeValueObject? expectedTireCode, RunEntityInputDetailsDto? runEntityInputDetailsDto)
    {
        var imageResult = await _imageDownloadService.DownloadImageAsync(imageUrl);
        if (imageResult.IsFailure)
            return DataResult<EvaluationRunEntity>.Failure(imageResult.Failures);

        return await RunSingleEvaluationAsync(
            image: imageResult.Data!,
            runConfig: runConfig,
            expectedTireCode: expectedTireCode,
            runEntityInputDetailsDto: runEntityInputDetailsDto
        );
    }

    public async Task<DataResult<EvaluationRunBatchEntity>> RunEvaluationBatchAsync(
        Dictionary<string, TireCodeValueObject?> imageUrls,
        int batchSize,
        RunConfigDto runConfig,
        RunEntityInputDetailsDto? runEntityInputDetailsDto
    )
    {
        var result = new EvaluationRunBatchEntity(
            evaluationRuns: [],
            title: runEntityInputDetailsDto?.Title,
            id: runEntityInputDetailsDto?.Id
        );

        var completedCount = 0;
        var processedBatches = imageUrls.ToList().GetSubLists(batchSize);
        foreach (var batch in processedBatches)
        {
            var batchUrls = batch.ToDictionary();
            var processedBatch = await ProcessOcrPipelineBatch(
                imageUrls: batchUrls,
                runConfig: runConfig,
                batchId: result.Id
            );
            completedCount += batch.Count;
            _logger.LogInformation($"BATCH COMPLETION: {completedCount}/{batchUrls.Count}");

            result.AddEvaluationRuns(processedBatch.ToArray());
        }

        return DataResult<EvaluationRunBatchEntity>.Success(result);
    }

    private async Task<IEnumerable<EvaluationRunEntity>> ProcessOcrPipelineBatch(
        Dictionary<string, TireCodeValueObject?> imageUrls,
        RunConfigDto runConfig, Guid batchId
    )
    {
        var downloadedImages = (await _imageDownloadService
                .DownloadImageBatchAsync(imageUrls.Keys))
            .Where(x => x.Value.IsSuccess)
            .ToDictionary(x => x.Key, x => x.Value.Data!);

        var results = new List<EvaluationRunEntity>();
        var batchProcessingTasks = imageUrls
            .Select(async x =>
            {
                var imageUrl = x.Key;
                var expectedTireCode = x.Value;
                var successfullyDownloaded = downloadedImages.TryGetValue(imageUrl, out var downloadedImage);

                var fallbackEvaluationRun = new EvaluationRunEntity(
                    batchId: batchId,
                    title: null,
                    inputImage: downloadedImage?.ToDomain() ?? new()
                    {
                        FileName = imageUrl,
                        ContentType = string.Empty,
                    },
                    preprocessingType: runConfig.PreprocessingType,
                    ocrType: runConfig.OcrType,
                    postprocessingType: runConfig.PostprocessingType,
                    dbMatchingType: runConfig.DbMatchingType
                );

                if (!successfullyDownloaded)
                {
                    _logger.LogError($"Failed to download image '{imageUrl}'");
                    fallbackEvaluationRun.SetFailure(
                        EvaluationRunFailureValueObject.UnexpectedFailure(
                            new Failure(500, $"Failed to download image '{imageUrl}'")
                        )
                    );
                    results.Add(fallbackEvaluationRun);
                }
                else if (downloadedImage is not null)
                {
                    var result = await RunSingleEvaluationAsync(
                        image: downloadedImage,
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
                                    EvaluationRunFailureValueObject.UnexpectedFailure(unexpectedFailures.First())
                                );
                                return fallbackEvaluationRun;
                            }
                        )
                    );
                }
                else
                {
                    _logger.LogError($"Image '{imageUrl}' was supposedly downloaded, but was 'null' ");
                }
            });

        await Task.WhenAll(batchProcessingTasks);
        return results;
    }
}