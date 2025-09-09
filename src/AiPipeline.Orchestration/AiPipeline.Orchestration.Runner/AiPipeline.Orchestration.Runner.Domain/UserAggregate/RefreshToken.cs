namespace AiPipeline.Orchestration.Runner.Domain.UserAggregate;

public class RefreshToken
{
    public Guid UserId { get; private set; }
    public string Token { get; private set; }
    public bool Invalidated { get; private set; }

    public DateTime Created { get; private set; }
    public DateTime ExpiresAt { get; private set; }

    private RefreshToken()
    {
    }

    public RefreshToken(Guid userId, string token, DateTime expiresAt)
    {
        UserId = userId;
        Token = token;
        ExpiresAt = expiresAt;
        Invalidated = false;
        Created = DateTime.UtcNow;
    }

    public void Invalidate()
    {
        Invalidated = true;
    }
}