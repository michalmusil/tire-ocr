using TireOcr.RunnerPrototype.Models;
using TireOcr.Shared.Result;

namespace TireOcr.RunnerPrototype.Clients.Preprocessing;

public interface IPreprocessingClient
{
    public Task<DataResult<Image>> PreprocessImage(Image image);
}

