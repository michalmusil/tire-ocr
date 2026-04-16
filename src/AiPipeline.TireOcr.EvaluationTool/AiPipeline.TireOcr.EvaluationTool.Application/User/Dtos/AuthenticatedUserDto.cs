namespace AiPipeline.TireOcr.EvaluationTool.Application.User.Dtos;

public record AuthenticatedUserDto(
    Guid Id,
    string Username,
    AccessRefreshTokenPair AccessRefreshTokenPair
)
{
    public static AuthenticatedUserDto FromDomain(Domain.UserAggregate.User domain, AccessRefreshTokenPair tokens)
    {
        return new AuthenticatedUserDto(domain.Id, domain.Username, tokens);
    }
}