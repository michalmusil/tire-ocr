namespace TireOcr.RunnerPrototype.Dtos.Batch;

public record TireOcrFailureDto(
    string ImageFileName,
    string Message
);