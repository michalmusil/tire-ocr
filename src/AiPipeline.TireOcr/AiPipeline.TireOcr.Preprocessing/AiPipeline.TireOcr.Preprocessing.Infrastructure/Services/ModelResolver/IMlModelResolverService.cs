using TireOcr.Preprocessing.Infrastructure.Models;
using TireOcr.Shared.Result;

namespace TireOcr.Preprocessing.Infrastructure.Services.ModelResolver;

public interface IMlModelResolverService
{
    public Task<DataResult<MlModel>> Resolve<T>();
    public Task<Result> EnsureAllModelsLoadedAsync();
}