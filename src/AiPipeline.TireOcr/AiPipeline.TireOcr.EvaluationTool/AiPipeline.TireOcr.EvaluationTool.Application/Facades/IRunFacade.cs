using AiPipeline.TireOcr.EvaluationTool.Application.Dtos;
using AiPipeline.TireOcr.EvaluationTool.Application.Dtos.EvaluationRun;
using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate;
using AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunBatchAggregate;
using TireOcr.Shared.Result;

namespace AiPipeline.TireOcr.EvaluationTool.Application.Facades;

public interface IRunFacade
{
    /// <summary>
    /// Starts an evaluation run on the input image based on the specified run configuration.
    /// </summary>
    /// <param name="image">Data of the input image</param>
    /// <param name="runConfig">Configuration of the evaluation run</param>
    /// <param name="expectedTireCode">Optional expected tire code. No evaluation can be calculated if this is not provided</param>
    /// <param name="runEntityInputDetailsDto">Optional metadata of created evaluation run</param>
    /// <returns>A DataResult containing the full evaluation run result if successful, failure otherwise</returns>
    public Task<DataResult<EvaluationRunEntity>> RunSingleEvaluationAsync(ImageDto image, RunConfigDto runConfig,
        TireCodeValueObject? expectedTireCode, RunEntityInputDetailsDto? runEntityInputDetailsDto);

    /// <summary>
    /// Downloads input image and starts an evaluation run on it based on the specified run configuration.
    /// </summary>
    /// <param name="imageUrl">URL of the input image</param>
    /// <param name="runConfig">Configuration of the evaluation run</param>
    /// <param name="expectedTireCode">Optional expected tire code. No evaluation can be calculated if this is not provided</param>
    /// <param name="runEntityInputDetailsDto">Optional metadata of created evaluation run</param>
    /// <returns>A DataResult containing the full evaluation run result if successful, failure otherwise</returns>
    public Task<DataResult<EvaluationRunEntity>> RunSingleEvaluationAsync(string imageUrl, RunConfigDto runConfig,
        TireCodeValueObject? expectedTireCode, RunEntityInputDetailsDto? runEntityInputDetailsDto);

    /// <summary>
    /// Downloads all provided images from their URLs, then runs evaluation run for all of them. The evaluation
    /// on images is run in sequential batches one by one to prevent resource overloading. The size of the batch can
    /// be specified by the batch size parameter.
    /// </summary>
    /// <param name="imageUrls">A dictionary with image URLs as keys and optional expected tire codes as values</param>
    /// <param name="batchSize">Size of one evaluation processing batch. Must be >= 1</param>
    /// <param name="runConfig">Configuration used for all evaluation runs</param>
    /// <param name="runEntityInputDetailsDto">Optional metadata of created evaluation run batch</param>
    /// <returns>A DataResult containing results of all the evaluations aggregated in one EvaluationRunBatch if successful, failure otherwise</returns>
    public Task<DataResult<EvaluationRunBatchEntity>> RunEvaluationBatchAsync(
        Dictionary<string, TireCodeValueObject?> imageUrls,
        int batchSize,
        RunConfigDto runConfig,
        RunEntityInputDetailsDto? runEntityInputDetailsDto
    );
}