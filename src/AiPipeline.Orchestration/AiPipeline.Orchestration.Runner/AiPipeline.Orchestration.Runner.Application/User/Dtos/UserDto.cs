namespace AiPipeline.Orchestration.Runner.Application.User.Dtos;

public record UserDto(
    Guid Id,
    string Username,
    DateTime CreatedAt,
    DateTime UpdatedAt
)
{
    public static UserDto FromDomain(Domain.UserAggregate.User domain)
    {
        return new UserDto(domain.Id, domain.Username, domain.CreatedAt, domain.UpdatedAt);
    }
}