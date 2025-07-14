using AiPipeline.Orchestration.Shared.Contracts.Schema;

namespace AiPipeline.Orchestration.Shared.Contracts.Commands.RunPipelineStep;

public record RunPipelineStep(
    Guid PipelineId,
    ProcedureIdentifier CurrentStep,
    IApElement CurrentStepInput,
    List<ProcedureIdentifier> NextSteps,
    List<FileReference> FileReferences
);