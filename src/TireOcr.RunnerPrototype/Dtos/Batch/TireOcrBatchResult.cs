namespace TireOcr.RunnerPrototype.Dtos.Batch;

public record TireOcrBatchResult(
    BatchSummaryDto Summary,
    IEnumerable<TireOcrResult> SuccessfulResults,
    IEnumerable<TireOcrFailure> Failures
);