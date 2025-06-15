using TireOcr.Shared.Result;

namespace AiPipeline.Orchestration.Runner.Domain.NodeTypeAggregate;

public class NodeType
{
    public string Id { get; }

    private readonly List<NodeProcedure> _availableProcedures;
    public IReadOnlyCollection<NodeProcedure> AvailableProcedures => _availableProcedures.AsReadOnly();

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
        return Result.Success();
    }

    public Result ClearProcedures()
    {
        _availableProcedures.Clear();
        return Result.Success();
    }
}