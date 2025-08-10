using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Encodings.Web;
using AiPipeline.Orchestration.Runner.Application.User.Repositories;
using AiPipeline.Orchestration.Runner.Infrastructure.User.Dtos;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AiPipeline.Orchestration.Runner.WebApi.AuthenticationSchemas.UserOrApiKey;

public class UserOrApiKeyAuthenticationHandler : AuthenticationHandler<UserOrApiKeyAuthenticationOptions>
{
    private readonly IUserEntityRepository _userRepository;
    private readonly IConfiguration _configuration;

    public UserOrApiKeyAuthenticationHandler(IOptionsMonitor<UserOrApiKeyAuthenticationOptions> options,
        ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, IUserEntityRepository userRepository,
        IConfiguration configuration)
        : base(options, logger, encoder, clock)
    {
        _userRepository = userRepository;
        _configuration = configuration;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.ContainsKey("Authorization"))
            return AuthenticateResult.NoResult();

        var authorizationHeaderValue = Request.Headers["Authorization"].ToString();
        if (authorizationHeaderValue.Trim().Length == 0)
            return AuthenticateResult.Fail("Authorization header value is empty");

        var isJwtAuthentication = authorizationHeaderValue.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase);
        if (isJwtAuthentication)
            return await PerformJwtAuthentication(authorizationHeaderValue);

        return await PerformApiKeyAuthentication(authorizationHeaderValue);
    }

    private async Task<AuthenticateResult> PerformJwtAuthentication(string authorizationHeaderValue)
    {
        var jwtToken = authorizationHeaderValue.Replace("Bearer ", "");
        try
        {
            var validationParameters = GetAccessTokenValidationParams(GetJwtOptions());
            var claimsPrincipal = new JwtSecurityTokenHandler()
                .ValidateToken(jwtToken, validationParameters, out var securityToken);
            if (claimsPrincipal is null || securityToken is null)
                return AuthenticateResult.Fail("Failed to decode provided access token");

            var ticket = new AuthenticationTicket(claimsPrincipal, UserOrApiKeyAuthenticationOptions.SchemeName);
            return AuthenticateResult.Success(ticket);
        }
        catch (SecurityTokenExpiredException ex)
        {
            return AuthenticateResult.Fail("Access token expired");
        }
        catch
        {
            return AuthenticateResult.Fail("Invalid access token");
        }
    }

    private async Task<AuthenticateResult> PerformApiKeyAuthentication(string authorizationHeaderValue)
    {
        throw new NotImplementedException();
    }

    private JwtOptions GetJwtOptions()
    {
        var jwtOptions = new JwtOptions();
        _configuration.GetSection(JwtOptions.Key).Bind(jwtOptions);
        if (!jwtOptions.IsValid)
            throw new InvalidOperationException("Failed to bind JwtOptions");

        return jwtOptions;
    }

    private TokenValidationParameters GetAccessTokenValidationParams(JwtOptions jwtOptions) =>
        new()
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = GetAccessTokenSigningKey(jwtOptions),
            ValidateIssuer = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtOptions.Audience,
            ValidateLifetime = true
        };

    private SymmetricSecurityKey GetAccessTokenSigningKey(JwtOptions jwtOptions) =>
        new(Encoding.UTF8.GetBytes(jwtOptions.Secret));
}