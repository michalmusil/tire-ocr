namespace AiPipeline.Orchestration.Runner.Domain.PipelineResultAggregate;

public class PipelineFailureReason
{
    public int FailureCode { get; }
    public string FailureReason { get; }
    public string? ExceptionMessage { get; }

    public PipelineFailureReason(int failureCode, string failureReason, string? exceptionMessage)
    {
        FailureCode = failureCode;
        FailureReason = failureReason;
        ExceptionMessage = exceptionMessage;
    }
}