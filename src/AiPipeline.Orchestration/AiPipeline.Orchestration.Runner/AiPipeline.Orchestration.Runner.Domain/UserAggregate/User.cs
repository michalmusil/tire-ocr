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
        SetUpdated();
        return Result.Success();
    }

    public Result InvalidateRefreshToken(string token)
    {
        var existingRefreshToken = GetExistingRefreshToken(token);
        if (existingRefreshToken == null)
            return Result.NotFound($"Refresh token not found: {token}");

        existingRefreshToken.Invalidate();
        SetUpdated();
        return Result.Success();
    }

    private RefreshToken? GetExistingRefreshToken(string token) =>
        _refreshTokens.FirstOrDefault(rt => rt.Token == token);
}