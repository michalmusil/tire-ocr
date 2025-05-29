using TireOcr.RunnerPrototype.Dtos;
using TireOcr.Shared.Result;

namespace TireOcr.RunnerPrototype.Clients.Postprocessing;

public interface IPostprocessingClient
{
    public Task<DataResult<TirePostprocessingResultDto>> PostprocessTireCode(string rawTireCode);
}