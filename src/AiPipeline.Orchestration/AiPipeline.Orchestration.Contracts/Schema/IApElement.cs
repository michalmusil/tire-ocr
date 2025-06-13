namespace AiPipeline.Orchestration.Contracts.Schema;

public interface IApElement
{
    bool HasEquivalentSchemaWith(IApElement other);
};