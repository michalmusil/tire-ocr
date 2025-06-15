using AiPipeline.Orchestration.Shared.Contracts.Schema;

namespace AiPipeline.Orchestration.Shared.Contracts.Commands.RunPipelineStep;

public record RunPipelineStep(
    ProcedureIdentifier CurrentStep,
    IApElement CurrentStepInput,
    ProcedureIdentifier? NextStep
);