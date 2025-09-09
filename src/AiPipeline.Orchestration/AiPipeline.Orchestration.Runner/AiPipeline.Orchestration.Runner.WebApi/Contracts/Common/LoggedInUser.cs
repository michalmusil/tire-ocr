using AiPipeline.Orchestration.Runner.WebApi.AuthenticationSchemas.UserOrApiKey;

namespace AiPipeline.Orchestration.Runner.WebApi.Contracts.Common;

public record LoggedInUser(
    Guid Id,
    string Username,
    AuthenticationMethodType AuthenticationMethod
)
{
    public bool IsAuthenticatedViaApiKey => AuthenticationMethod == AuthenticationMethodType.ApiKey;
}