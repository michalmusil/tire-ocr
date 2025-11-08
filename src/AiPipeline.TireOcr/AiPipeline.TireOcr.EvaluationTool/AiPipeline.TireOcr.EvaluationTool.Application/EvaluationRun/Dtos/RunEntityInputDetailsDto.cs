namespace AiPipeline.TireOcr.EvaluationTool.Application.EvaluationRun.Dtos;

public record RunEntityInputDetailsDto(
    Guid? Id,
    string? Title,
    string? Description
);