namespace AiPipeline.Orchestration.Runner.Domain.NodeTypeEntity;

public class NodeProcedure
{
    public string Id { get; }
    public int SchemaVersion { get; }
    public string InputDescription { get; }
    public string OutputDescription { get; }

    public NodeProcedure(string id, int schemaVersion, string inputDescription, string outputDescription)
    {
        Id = id;
        SchemaVersion = schemaVersion;
        InputDescription = inputDescription;
        OutputDescription = outputDescription;
    }
}