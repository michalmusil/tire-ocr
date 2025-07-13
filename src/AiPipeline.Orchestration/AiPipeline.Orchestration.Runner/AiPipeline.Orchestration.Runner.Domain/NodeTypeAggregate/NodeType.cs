using AiPipeline.Orchestration.Runner.Domain.Common;
using TireOcr.Shared.Result;

namespace AiPipeline.Orchestration.Runner.Domain.NodeTypeAggregate;

public class NodeType: TimestampedEntity
{
    public string Id { get; }

    public readonly List<NodeProcedure> _availableProcedures;
    public IReadOnlyCollection<NodeProcedure> AvailableProcedures => _availableProcedures.AsReadOnly();

    private NodeType()
    {
    }

    public NodeType(string id, IEnumerable<NodeProcedure> availableProcedures)
    {
        Id = id;
        _availableProcedures = availableProcedures.ToList();
    }

    public Result AddProcedure(NodeProcedure nodeProcedure)
    {
        var alreadyExisting = AvailableProcedures.FirstOrDefault(x => x.Id == nodeProcedure.Id);
        if (alreadyExisting is not null)
            return Result.Conflict($"A procedure with id {nodeProcedure.Id} already exists in the node type {Id}");

        _availableProcedures.Add(nodeProcedure);
        SetUpdated();
        return Result.Success();
    }

    public Result ClearProcedures()
    {
        _availableProcedures.Clear();
        return Result.Success();
    }
}