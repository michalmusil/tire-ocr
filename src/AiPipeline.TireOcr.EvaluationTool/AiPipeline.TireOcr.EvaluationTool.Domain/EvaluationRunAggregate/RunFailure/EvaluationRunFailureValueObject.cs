using TireOcr.Shared.Domain;
using TireOcr.Shared.Result;

namespace AiPipeline.TireOcr.EvaluationTool.Domain.EvaluationRunAggregate.RunFailure;

public class EvaluationRunFailureValueObject : ValueObject
{
    public required EvaluationRunFailureReason Reason { get; init; }
    public required int Code { get; init; }
    public required string Message { get; init; }
    protected override IEnumerable<object?> GetEqualityComponents() => [Reason, Code, Message];

    public static EvaluationRunFailureValueObject PreprocessingFailure(Failure failure) => new()
    {
        Reason = EvaluationRunFailureReason.Preprocessing,
        Code = failure.Code,
        Message = failure.Message,
    };

    public static EvaluationRunFailureValueObject OcrFailure(Failure failure) => new()
    {
        Reason = EvaluationRunFailureReason.Ocr,
        Code = failure.Code,
        Message = failure.Message,
    };

    public static EvaluationRunFailureValueObject PostprocessingFailure(Failure failure) => new()
    {
        Reason = EvaluationRunFailureReason.Postprocessing,
        Code = failure.Code,
        Message = failure.Message,
    };

    public static EvaluationRunFailureValueObject UnexpectedFailure(Failure failure) => new()
    {
        Reason = EvaluationRunFailureReason.Unexpected,
        Code = failure.Code,
        Message = failure.Message,
    };
}