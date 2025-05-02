namespace TireOcr.RunnerPrototype.Dtos.Batch;

public record PipelineCompletionSuccessRateDto(
    int TotalImagesCount,
    int PipelineFinishedCount,
    int PipelineFailedCount,
    double SuccessRate
);