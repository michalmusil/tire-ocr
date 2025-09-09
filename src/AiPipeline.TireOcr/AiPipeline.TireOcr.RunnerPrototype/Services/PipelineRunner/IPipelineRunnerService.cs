using TireOcr.RunnerPrototype.Dtos;
using TireOcr.RunnerPrototype.Dtos.Batch;
using TireOcr.RunnerPrototype.Models;
using TireOcr.Shared.Result;

namespace TireOcr.RunnerPrototype.Services.PipelineRunner;

public interface IPipelineRunnerService
{
    Task<DataResult<TireOcrResultDto>> RunSingleOcrPipelineAsync(Image image, TireCodeDetectorType detectorType);

    Task<DataResult<TireOcrBatchResultDto>> RunOcrPipelineBatchAsync(
        IEnumerable<string> imageUrls,
        int batchSize,
        TireCodeDetectorType detectorType
    );
}