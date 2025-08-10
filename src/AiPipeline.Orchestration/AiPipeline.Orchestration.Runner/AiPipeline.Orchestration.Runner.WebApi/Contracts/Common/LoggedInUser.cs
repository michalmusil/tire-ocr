namespace AiPipeline.Orchestration.Runner.WebApi.Contracts.Common;

public record LoggedInUser(
    Guid Id,
    string Username
);