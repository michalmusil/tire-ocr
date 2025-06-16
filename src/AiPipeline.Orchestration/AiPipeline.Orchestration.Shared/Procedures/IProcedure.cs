using AiPipeline.Orchestration.Shared.Contracts.Schema;
using TireOcr.Shared.Result;

namespace AiPipeline.Orchestration.Shared.Procedures;

public interface IProcedure
{
    public string Name { get; }
    public int SchemaVersion { get; }
    public IApElement InputSchema { get; }
    public IApElement OutputSchema { get; }

    public Task<DataResult<IApElement>> Execute(IApElement input);
}