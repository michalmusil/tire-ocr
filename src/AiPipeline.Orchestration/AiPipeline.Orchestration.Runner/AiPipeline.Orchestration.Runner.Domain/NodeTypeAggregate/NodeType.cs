namespace AiPipeline.Orchestration.Runner.Domain.NodeTypeAggregate;

public class NodeType
{
    public string Id { get; }
    public IEnumerable<NodeProcedure> AvailableProcedures { get; }

    public NodeType(string id, IEnumerable<NodeProcedure> availableProcedures)
    {
        Id = id;
        AvailableProcedures = availableProcedures;
    }
}