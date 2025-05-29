using TireOcr.Ocr.Application.Services;
using TireOcr.Ocr.Domain;
using TireOcr.Shared.Result;

namespace TireOcr.Ocr.Infrastructure.Services.TireCodeDetectorResolver;

public interface ITireCodeDetectorResolverService
{
    public DataResult<ITireCodeDetectorService> Resolve(TireCodeDetectorType detectorType);
}