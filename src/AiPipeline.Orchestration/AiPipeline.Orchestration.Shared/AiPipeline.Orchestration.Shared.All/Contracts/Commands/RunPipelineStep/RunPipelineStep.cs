using AiPipeline.Orchestration.Shared.All.Contracts.Schema;

namespace AiPipeline.Orchestration.Shared.All.Contracts.Commands.RunPipelineStep;

public record RunPipelineStep(
    Guid PipelineId,
    ProcedureIdentifier CurrentStep,
    IApElement CurrentStepInput,
    List<ProcedureIdentifier> NextSteps,
    List<FileReference> FileReferences
);