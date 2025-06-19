using System.Reflection;
using AiPipeline.Orchestration.Shared.Contracts.Commands.RunPipelineStep;
using AiPipeline.Orchestration.Shared.Contracts.Schema;
using TireOcr.Shared.Result;

namespace AiPipeline.Orchestration.Shared.Procedures;

public interface IProcedureRouter
{
    public Task<DataResult<IApElement>> ProcessPipelineStep(RunPipelineStep step);
    public void RegisterProceduresFromAssemblies(params Assembly[] assemblies);
}