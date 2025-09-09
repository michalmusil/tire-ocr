using Microsoft.AspNetCore.Authentication;

namespace AiPipeline.Orchestration.Runner.WebApi.AuthenticationSchemas.UserOrApiKey;

public class UserOrApiKeyAuthenticationOptions : AuthenticationSchemeOptions
{
    public static string SchemeName => "UserOrApiKeyAuthScheme"; 
}