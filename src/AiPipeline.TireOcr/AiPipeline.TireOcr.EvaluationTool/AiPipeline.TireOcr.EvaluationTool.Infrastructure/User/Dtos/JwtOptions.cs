namespace AiPipeline.TireOcr.EvaluationTool.Infrastructure.User.Dtos;

public class JwtOptions
{
    public const string Key = "Jwt";

    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public string Secret { get; set; } = string.Empty;
    public int AccessTokenExpirationMinutes { get; set; }
    public int RefreshTokenExpirationMonths { get; set; }

    public bool IsValid => !string.IsNullOrEmpty(Issuer) && !string.IsNullOrEmpty(Audience) &&
                           !string.IsNullOrEmpty(Secret) && AccessTokenExpirationMinutes > 0 &&
                           RefreshTokenExpirationMonths > 0;
}