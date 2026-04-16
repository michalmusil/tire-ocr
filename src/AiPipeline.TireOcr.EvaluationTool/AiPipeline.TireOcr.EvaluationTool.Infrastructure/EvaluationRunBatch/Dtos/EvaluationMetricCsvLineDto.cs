namespace AiPipeline.TireOcr.EvaluationTool.Infrastructure.EvaluationRunBatch.Dtos;

public record EvaluationMetricCsvLineDto(
    string Name,
    string Shortcut,
    decimal Value
);