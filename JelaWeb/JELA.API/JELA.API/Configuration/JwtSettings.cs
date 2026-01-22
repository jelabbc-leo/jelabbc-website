namespace JELA.API.Configuration;

/// <summary>
/// Configuración de JWT
/// </summary>
public class JwtSettings
{
    public string SecretKey { get; set; } = string.Empty;
    public string Issuer { get; set; } = "JELA.API";
    public string Audience { get; set; } = "JelaWeb";
    public int ExpirationMinutes { get; set; } = 60;
    public int RefreshTokenExpirationDays { get; set; } = 7;
}
