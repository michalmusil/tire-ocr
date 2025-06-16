using System.Reflection;
using AiPipeline.Orchestration.Shared.Contracts.Commands.RunPipelineStep;
using AiPipeline.Orchestration.Shared.Contracts.Schema;
using TireOcr.Shared.Result;

namespace AiPipeline.Orchestration.Shared.Procedures;

public class ProcedureRouter
{
    private readonly List<IProcedure> _procedures = [];

    public Task<DataResult<IApElement>> ProcessPipelineStep(RunPipelineStep step)
    {
        // TODO: route to local procedure, execute, route to next procedure, finish or publish error
        throw new NotImplementedException();
    }

    public void RegisterProceduresFromAssembly(Assembly assembly)
    {
        // TODO: add all services from current assembly to collection
        throw new NotImplementedException();
    }
}