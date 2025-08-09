using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AiPipeline.Orchestration.Runner.Application.User.Dtos;
using AiPipeline.Orchestration.Runner.Application.User.Repositories;
using AiPipeline.Orchestration.Runner.Application.User.Services;
using AiPipeline.Orchestration.Runner.Infrastructure.User.Dtos;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using TireOcr.Shared.Result;

namespace AiPipeline.Orchestration.Runner.Infrastructure.User.Services;

public class AuthService : IAuthService
{
    private readonly IConfiguration _configuration;
    private readonly IUserEntityRepository _userRepository;

    public AuthService(IConfiguration configuration, IUserEntityRepository userRepository)
    {
        _configuration = configuration;
        _userRepository = userRepository;
    }

    public async Task<DataResult<AccessRefreshTokenPair>> GetTokensForUserAsync(Domain.UserAggregate.User user)
    {
        var jwtOptions = GetJwtOptions();
        var accessToken = GetAccessTokenForUser(user, jwtOptions);
        if (accessToken is null)
            return DataResult<AccessRefreshTokenPair>.Failure(new Failure(500, "Access token generation failed"));

        var refreshToken = GetNewRefreshToken();
        var refreshTokenExpiration = DateTime.UtcNow.AddMonths(jwtOptions.RefreshTokenExpirationMonths);

        return DataResult<AccessRefreshTokenPair>.Success(
            new AccessRefreshTokenPair(
                AccessToken: accessToken,
                RefreshToken: refreshToken,
                RefreshExpiration: refreshTokenExpiration
            )
        );
    }

    public async Task<DataResult<Domain.UserAggregate.User>> GetUserFromAccessTokenAsync(string accessToken)
    {
        var userIdResult = await ValidateAndParseUserIdFromAccessTokenAsync(accessToken);
        if (userIdResult.IsFailure)
            return DataResult<Domain.UserAggregate.User>.Failure(userIdResult.Failures);

        var userId = userIdResult.Data;
        var foundUser = await _userRepository.GetByIdAsync(userId);
        if (foundUser is null)
            return DataResult<Domain.UserAggregate.User>.NotFound($"User with id {userId} not found");

        return DataResult<Domain.UserAggregate.User>.Success(foundUser);
    }

    private JwtOptions GetJwtOptions()
    {
        var jwtOptions = new JwtOptions();
        _configuration.GetSection(JwtOptions.Key).Bind(jwtOptions);
        if (!jwtOptions.IsValid)
            throw new InvalidOperationException("Failed to bind JwtOptions");

        return jwtOptions;
    }

    private async Task<DataResult<Guid>> ValidateAndParseUserIdFromAccessTokenAsync(string accessToken)
    {
        var jwtOptions = GetJwtOptions();
        var handler = new JwtSecurityTokenHandler();
        try
        {
            var validationParameters = GetAccessTokenValidationParams(jwtOptions);
            var claimsPrincipal = handler.ValidateToken(accessToken, validationParameters, out var securityToken);
            if (claimsPrincipal is null || securityToken is null)
                return DataResult<Guid>.Invalid("Failed to decode provided access token");

            var userId = claimsPrincipal.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (userId is null)
                return DataResult<Guid>.Invalid("Failed to decode provided access token");

            return DataResult<Guid>.Success(new Guid(userId));
        }
        catch (SecurityTokenValidationException)
        {
            return DataResult<Guid>.Unauthorized("Access token not valid");
        }
        catch
        {
            return DataResult<Guid>.Invalid("Failed to decode provided access token");
        }
    }

    private string? GetAccessTokenForUser(Domain.UserAggregate.User user, JwtOptions jwtOptions)
    {
        var key = GetAccessTokenSigningKey(jwtOptions);
        var validUntil = DateTime.Now.AddMinutes(jwtOptions.AccessTokenExpirationMinutes);

        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Username),
        };

        var token = new JwtSecurityToken(
            issuer: jwtOptions.Issuer,
            audience: jwtOptions.Audience,
            claims: claims,
            expires: validUntil,
            signingCredentials: credentials);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        return tokenString;
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
            ValidateLifetime = false
        };

    private SymmetricSecurityKey GetAccessTokenSigningKey(JwtOptions jwtOptions) =>
        new(Encoding.UTF8.GetBytes(jwtOptions.Secret));

    private string GetNewRefreshToken()
    {
        var randNum = new byte[256];
        using (var numberGenerator = RandomNumberGenerator.Create())
        {
            numberGenerator.GetBytes(randNum);
        }

        return Convert.ToBase64String(randNum);
    }
}