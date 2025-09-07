using TireOcr.RunnerPrototype.Dtos.DbMatching;
using TireOcr.RunnerPrototype.Dtos.Postprocessing;
using TireOcr.Shared.Result;

namespace TireOcr.RunnerPrototype.Clients.DbMatching;

public interface IDbMatchingClient
{
    public Task<DataResult<DbMatcherServiceResponseDto>> GetDbMatchesForTireCode(
        TirePostprocessingResultDto postprocessedTireCode
    );
}