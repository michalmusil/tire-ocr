namespace TireOcr.RunnerPrototype.Dtos.Batch;

public record TireOcrFailure(
    string ImageFileName,
    string Message
);