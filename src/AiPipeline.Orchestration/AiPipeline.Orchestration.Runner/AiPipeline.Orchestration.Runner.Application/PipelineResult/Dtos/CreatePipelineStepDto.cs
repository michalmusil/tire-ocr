using AiPipeline.Orchestration.Runner.Domain.PipelineResultAggregate;
using AiPipeline.Orchestration.Shared.Contracts.Schema;

namespace AiPipeline.Orchestration.Runner.Application.PipelineResult.Dtos;

public class CreatePipelineStepDto
{
    public Guid Id { get; }
    public Guid ResultId { get; }
    public string NodeId { get; }
    public string NodeProcedureId { get; }
    public DateTime FinishedAt { get; }
    public bool WasSuccessful { get; }
    public PipelineFailureReason? FailureReason { get; }
    public IApElement? Result { get; }

    private CreatePipelineStepDto(Guid resultId, string nodeId, string nodeProcedureId, DateTime finishedAt,
        bool wasSuccessful, PipelineFailureReason? failureReason, IApElement? result, Guid? id = null)
    {
        Id = id ?? Guid.NewGuid();
        ResultId = resultId;
        NodeId = nodeId;
        NodeProcedureId = nodeProcedureId;
        FinishedAt = finishedAt;
        WasSuccessful = wasSuccessful;
        FailureReason = failureReason;
        Result = result;
    }

    public static CreatePipelineStepDto SuccessfulStep(Guid resultId, string nodeId, string nodeProcedureId,
        DateTime finishedAt, IApElement? result, Guid? id = null)
        => new CreatePipelineStepDto(
            resultId, nodeId, nodeProcedureId, finishedAt, true, null, result, id
        );

    public static CreatePipelineStepDto FailedStep(Guid resultId, string nodeId, string nodeProcedureId,
        DateTime finishedAt, PipelineFailureReason failureReason, Guid? id = null)
        => new CreatePipelineStepDto(
            resultId, nodeId, nodeProcedureId, finishedAt, false, failureReason, null, id
        );

    public PipelineStepResult ToDomain()
    {
        return new PipelineStepResult(
            id: Id,
            resultId: ResultId,
            wasSuccessful: WasSuccessful,
            nodeId: NodeId,
            nodeProcedureId: NodeProcedureId,
            finishedAt: FinishedAt,
            failureReason: WasSuccessful ? null : FailureReason,
            result: WasSuccessful ? Result : null
        );
    }
}