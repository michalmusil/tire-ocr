using AiPipeline.TireOcr.EvaluationTool.WebApi.Common.AuthenticationSchemas.UserOrApiKey;

namespace AiPipeline.TireOcr.EvaluationTool.WebApi.User.Contracts.Auth;

public record LoggedInUser(
    Guid Id,
    string Username,
    AuthenticationMethodType AuthenticationMethod
)
{
    public bool IsAuthenticatedViaApiKey => AuthenticationMethod == AuthenticationMethodType.ApiKey;
}