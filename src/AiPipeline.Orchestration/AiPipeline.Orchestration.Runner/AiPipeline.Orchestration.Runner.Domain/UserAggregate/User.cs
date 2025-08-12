using AiPipeline.Orchestration.Runner.Domain.Common;
using TireOcr.Shared.Result;

namespace AiPipeline.Orchestration.Runner.Domain.UserAggregate;

public class User : TimestampedEntity
{
    public Guid Id { get; private set; }
    public string Username { get; private set; }
    public string PasswordHash { get; private set; }

    public readonly List<RefreshToken> _refreshTokens = new();
    public IReadOnlyCollection<RefreshToken> RefreshTokens => _refreshTokens.AsReadOnly();
    public readonly List<ApiKey> _apiKeys = new();
    public IReadOnlyCollection<ApiKey> ApiKeys => _apiKeys.AsReadOnly();

    private User()
    {
    }

    public User(string username, string passwordHash, Guid? id = null)
    {
        Id = id ?? Guid.NewGuid();
        Username = username;
        PasswordHash = passwordHash;
    }

    public Result SetUsername(string username)
    {
        Username = username;
        SetUpdated();
        return Result.Success();
    }

    public Result SetPasswordHash(string passwordHash)
    {
        PasswordHash = passwordHash;
        SetUpdated();
        return Result.Success();
    }

    public Result AddRefreshToken(string token, DateTime expiresAt)
    {
        var existingRefreshToken = GetExistingRefreshToken(token);
        if (existingRefreshToken != null)
            return Result.Conflict("Failed to add refresh token");

        _refreshTokens.Add(
            new RefreshToken(
                Id,
                token,
                new DateTime(expiresAt.Ticks, DateTimeKind.Utc)
            )
        );
        SetUpdated();
        return Result.Success();
    }

    public Result RemoveRefreshToken(string token)
    {
        var existingRefreshToken = GetExistingRefreshToken(token);
        if (existingRefreshToken == null)
            return Result.NotFound($"Refresh token not found: {token}");

        _refreshTokens.Remove(existingRefreshToken);
        return Result.Success();
    }

    public Result InvalidateRefreshToken(string token)
    {
        var existingRefreshToken = GetExistingRefreshToken(token);
        if (existingRefreshToken is null)
            return Result.NotFound($"Refresh token not found: {token}");

        existingRefreshToken.Invalidate();
        return Result.Success();
    }

    public Result AddApiKey(ApiKey apiKey)
    {
        if (apiKey.UserId != Id)
            return Result.Forbidden($"Api key user id {apiKey.UserId} doesn't match user id: {Id}");
        var existingApiKey = GetExistingApiKey(apiKey.Key);
        if (existingApiKey is not null)
            return Result.Conflict("User already owns the same api key.");

        _apiKeys.Add(apiKey);
        return Result.Success();
    }

    public Result RemoveApiKey(string key)
    {
        var existingApiKey = GetExistingApiKey(key);
        if (existingApiKey is null)
            return Result.NotFound("User has no such api key.");

        _apiKeys.Remove(existingApiKey);
        return Result.Success();
    }

    private RefreshToken? GetExistingRefreshToken(string token) =>
        _refreshTokens.FirstOrDefault(rt => rt.Token == token);

    private ApiKey? GetExistingApiKey(string key) =>
        _apiKeys.FirstOrDefault(ak => ak.Key == key);
}