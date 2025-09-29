using TireOcr.Shared.Domain;
using TireOcr.Shared.Result;

namespace AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate;

public class EvaluationRunFailure : ValueObject
{
    public required EvaluationRunFailureReason Reason { get; init; }
    public required int Code { get; init; }
    public required string Message { get; init; }
    protected override IEnumerable<object?> GetEqualityComponents() => [Reason, Code, Message];

    public static EvaluationRunFailure PreprocessingFailure(Failure failure) => new()
    {
        Reason = EvaluationRunFailureReason.Postprocessing,
        Code = failure.Code,
        Message = failure.Message,
    };

    public static EvaluationRunFailure OcrFailure(Failure failure) => new()
    {
        Reason = EvaluationRunFailureReason.Ocr,
        Code = failure.Code,
        Message = failure.Message,
    };

    public static EvaluationRunFailure PostprocessingFailure(Failure failure) => new()
    {
        Reason = EvaluationRunFailureReason.Postprocessing,
        Code = failure.Code,
        Message = failure.Message,
    };

    public static EvaluationRunFailure UnexpectedFailure(Failure failure) => new()
    {
        Reason = EvaluationRunFailureReason.Unexpected,
        Code = failure.Code,
        Message = failure.Message,
    };
}