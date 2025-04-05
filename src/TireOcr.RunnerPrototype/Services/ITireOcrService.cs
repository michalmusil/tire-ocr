using TireOcr.RunnerPrototype.Dtos;
using TireOcr.RunnerPrototype.Models;
using TireOcr.Shared.Result;

namespace TireOcr.RunnerPrototype.Services;

public interface ITireOcrService
{
    Task<DataResult<TireOcrResult>> RunSingleOcrPipelineAsync(Image image, TireCodeDetectorType detectorType);
}