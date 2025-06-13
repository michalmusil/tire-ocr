namespace AiPipeline.Orchestration.Contracts.Schema;

public interface IApElement
{
    bool HasCompatibleSchemaWith(IApElement other);
};