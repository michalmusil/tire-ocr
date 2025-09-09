namespace AiPipeline.Orchestration.Runner.Application.User.Dtos;

public record UserDto(
    Guid Id,
    string Username,
    IEnumerable<ApiKeyDto> ApiKeys,
    DateTime CreatedAt,
    DateTime UpdatedAt
)
{
    public static UserDto FromDomain(Domain.UserAggregate.User domain)
    {
        return new UserDto(
            domain.Id,
            domain.Username,
            domain.ApiKeys
                .Select(ApiKeyDto.FromDomain),
            domain.CreatedAt,
            domain.UpdatedAt
        );
    }
}