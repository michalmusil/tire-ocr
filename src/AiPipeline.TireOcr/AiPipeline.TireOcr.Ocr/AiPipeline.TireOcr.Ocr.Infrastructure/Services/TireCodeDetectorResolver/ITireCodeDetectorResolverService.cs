using AiPipeline.TireOcr.Shared.Models;
using TireOcr.Ocr.Application.Services;
using TireOcr.Shared.Result;

namespace TireOcr.Ocr.Infrastructure.Services.TireCodeDetectorResolver;

public interface ITireCodeDetectorResolverService
{
    public DataResult<ITireCodeDetectorService> Resolve(TireCodeDetectorType detectorType);
}