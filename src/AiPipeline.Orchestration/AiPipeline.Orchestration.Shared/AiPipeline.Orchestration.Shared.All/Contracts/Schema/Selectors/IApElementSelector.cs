using TireOcr.Shared.Result;

namespace AiPipeline.Orchestration.Shared.All.Contracts.Schema.Selectors;

public interface IApElementSelector<T>
{
    public DataResult<T> Select(IApElement element);
}