using TireOcr.Preprocessing.Infrastructure.Models;

namespace TireOcr.Preprocessing.Infrastructure.Services.ModelResolver;

public interface IMlModelResolver
{
    public MlModel? Resolve<T>();
    public Task EnsureAllModelsLoadedAsync();
}