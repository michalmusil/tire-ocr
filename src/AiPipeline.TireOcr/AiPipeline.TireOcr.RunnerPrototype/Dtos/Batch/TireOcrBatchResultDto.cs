namespace TireOcr.RunnerPrototype.Dtos.Batch;

public record TireOcrBatchResultDto(
    BatchSummaryDto Summary,
    IEnumerable<TireOcrResultDto> SuccessfulResults,
    IEnumerable<TireOcrFailureDto> Failures
);