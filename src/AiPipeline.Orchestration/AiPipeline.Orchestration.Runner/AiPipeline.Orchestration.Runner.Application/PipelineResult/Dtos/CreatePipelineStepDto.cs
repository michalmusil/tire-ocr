using AiPipeline.Orchestration.Runner.Domain.PipelineResultAggregate;
using AiPipeline.Orchestration.Shared.All.Contracts.Schema;

namespace AiPipeline.Orchestration.Runner.Application.PipelineResult.Dtos;

public class CreatePipelineStepDto
{
    public Guid Id { get; }
    public string NodeId { get; }
    public string NodeProcedureId { get; }
    public DateTime FinishedAt { get; }
    public bool WasSuccessful { get; }
    public PipelineFailureReason? FailureReason { get; }
    public IApElement? Result { get; }

    private CreatePipelineStepDto(string nodeId, string nodeProcedureId, DateTime finishedAt,
        bool wasSuccessful, PipelineFailureReason? failureReason, IApElement? result, Guid? id = null)
    {
        Id = id ?? Guid.NewGuid();
        NodeId = nodeId;
        NodeProcedureId = nodeProcedureId;
        FinishedAt = finishedAt;
        WasSuccessful = wasSuccessful;
        FailureReason = failureReason;
        Result = result;
    }

    public static CreatePipelineStepDto SuccessfulStep(string nodeId, string nodeProcedureId, DateTime finishedAt,
        IApElement? result, Guid? id = null)
        => new CreatePipelineStepDto(nodeId, nodeProcedureId, finishedAt, true, null, result, id);

    public static CreatePipelineStepDto FailedStep(string nodeId, string nodeProcedureId, DateTime finishedAt,
        PipelineFailureReason failureReason, Guid? id = null)
        => new CreatePipelineStepDto(nodeId, nodeProcedureId, finishedAt, false, failureReason, null, id);

    public PipelineStepResult ToDomain(Guid resultId)
    {
        return new PipelineStepResult(
            id: Id,
            resultId: resultId,
            wasSuccessful: WasSuccessful,
            nodeId: NodeId,
            nodeProcedureId: NodeProcedureId,
            finishedAt: FinishedAt,
            failureReason: WasSuccessful ? null : FailureReason,
            result: WasSuccessful ? Result : null
        );
    }
}