using System.Security.Claims;
using AiPipeline.Orchestration.Runner.WebApi.Contracts.Common;

namespace AiPipeline.Orchestration.Runner.WebApi.Extensions;

public static class HttpContextExtension
{
    public static LoggedInUser? GetLoggedInUser(this HttpContext httpContext)
    {
        var claims = httpContext.User;
        var idClaim = claims.FindFirstValue(ClaimTypes.NameIdentifier);
        var usernameClaim = claims.FindFirstValue(ClaimTypes.Name);
        if (idClaim is null || usernameClaim is null)
            return null;
        var validGuid = Guid.TryParse(idClaim, out var userGuid);
        if (!validGuid)
            return null;

        return new LoggedInUser(Id: userGuid, Username: usernameClaim);
    }
}