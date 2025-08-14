using System.Security.Claims;
using AiPipeline.Orchestration.Runner.WebApi.AuthenticationSchemas.UserOrApiKey;
using AiPipeline.Orchestration.Runner.WebApi.Contracts.Common;

namespace AiPipeline.Orchestration.Runner.WebApi.Extensions;

public static class HttpContextExtension
{
    public static LoggedInUser? GetLoggedInUser(this HttpContext httpContext)
    {
        var claims = httpContext.User;
        var idClaim = claims.FindFirstValue(ClaimTypes.NameIdentifier);
        var usernameClaim = claims.FindFirstValue(ClaimTypes.Name);
        var authenticationType = claims.FindFirstValue(ClaimTypes.AuthenticationMethod);
        if (idClaim is null || usernameClaim is null || authenticationType is null)
            return null;

        var validGuid = Guid.TryParse(idClaim, out var userGuid);
        if (!validGuid)
            return null;

        var validAuthMethodType =
            Enum.TryParse<AuthenticationMethodType>(authenticationType, out var authenticationMethod);
        if (!validGuid)
            return null;

        return new LoggedInUser(
            Id: userGuid,
            Username: usernameClaim,
            AuthenticationMethod: authenticationMethod
        );
    }
}