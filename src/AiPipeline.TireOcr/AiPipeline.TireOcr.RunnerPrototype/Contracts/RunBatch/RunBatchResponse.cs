using TireOcr.RunnerPrototype.Dtos.Batch;

namespace TireOcr.RunnerPrototype.Contracts.RunBatch;

public record RunBatchResponse(
    TireOcrBatchResultDto Result
);