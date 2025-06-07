using TireOcr.Postprocessing.Domain.TireCodeEntity;
using TireOcr.Shared.Result;

namespace TireOcr.Postprocessing.Application.Services;

public interface ICodeFeatureExtractionService
{
    public DataResult<IEnumerable<TireCode>> ExtractTireCodes(string rawTireCodeValue);
    public DataResult<TireCode> PickBestMatchingTireCode(IEnumerable<TireCode> tireCodes);
}