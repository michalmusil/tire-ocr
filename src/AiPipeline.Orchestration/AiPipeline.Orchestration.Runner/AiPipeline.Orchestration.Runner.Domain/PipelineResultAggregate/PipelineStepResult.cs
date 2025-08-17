using AiPipeline.Orchestration.Runner.Domain.Common;
using AiPipeline.Orchestration.Shared.All.Contracts.Schema;

namespace AiPipeline.Orchestration.Runner.Domain.PipelineResultAggregate;

public class PipelineStepResult : TimestampedEntity
{
    public Guid Id { get; }
    public Guid ResultId { get; }
    public string NodeId { get; }
    public string NodeProcedureId { get; }
    public DateTime FinishedAt { get; }
    public bool WasSuccessful { get; }
    public PipelineFailureReason? FailureReason { get; }
    public IApElement? Result { get; }
    public int Order { get; }

    private PipelineStepResult()
    {
    }

    public PipelineStepResult(bool wasSuccessful, Guid resultId, string nodeId, string nodeProcedureId,
        DateTime finishedAt, PipelineFailureReason? failureReason, IApElement? result, int order, Guid? id = null)
    {
        Id = id ?? Guid.NewGuid();
        WasSuccessful = wasSuccessful;
        ResultId = resultId;
        NodeId = nodeId;
        NodeProcedureId = nodeProcedureId;
        FinishedAt = finishedAt;
        FailureReason = failureReason;
        Result = result;
        Order = order;
    }
}