namespace AiPipeline.Orchestration.Shared.All.Contracts.Schema;

public interface IApElement
{
    public bool HasCompatibleSchemaWith(IApElement other);

    public List<T> GetAllDescendantsOfType<T>()
        where T : IApElement;
};