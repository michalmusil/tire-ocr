using Microsoft.AspNetCore.Authentication;

namespace AiPipeline.TireOcr.EvaluationTool.WebApi.Common.AuthenticationSchemas.UserOrApiKey;

public class UserOrApiKeyAuthenticationOptions : AuthenticationSchemeOptions
{
    public static string SchemeName => "UserOrApiKeyAuthScheme"; 
}