using AiPipeline.Orchestration.Runner.Domain.Common;

namespace AiPipeline.Orchestration.Runner.Domain.UserAggregate;

public class ApiKey : TimestampedEntity
{
    public Guid UserId { get; private set; }
    public string Name { get; private set; }
    public string Key { get; private set; }
    public DateTime? ValidUntil { get; private set; }

    private ApiKey()
    {
    }

    public ApiKey(Guid userId, string name, string key, DateTime? validUntil = null)
    {
        UserId = userId;
        Name = name;
        Key = key;
        ValidUntil = validUntil;
    }
}