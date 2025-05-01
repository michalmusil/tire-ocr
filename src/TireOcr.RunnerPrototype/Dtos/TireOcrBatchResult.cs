namespace TireOcr.RunnerPrototype.Dtos;

public record TireOcrBatchResult(
    BatchSummaryDto Summary,
    IEnumerable<TireOcrResult> SuccessfulResults,
    IEnumerable<TireOcrFailure> Failures
);