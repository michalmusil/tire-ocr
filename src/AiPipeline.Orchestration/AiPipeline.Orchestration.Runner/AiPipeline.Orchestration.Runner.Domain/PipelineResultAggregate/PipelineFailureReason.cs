namespace AiPipeline.Orchestration.Runner.Domain.PipelineResultAggregate;

public class PipelineFailureReason
{
    public int Code { get; }
    public string Reason { get; }
    public string? ExceptionMessage { get; }

    public PipelineFailureReason(int code, string reason, string? exceptionMessage)
    {
        Code = code;
        Reason = reason;
        ExceptionMessage = exceptionMessage;
    }
}