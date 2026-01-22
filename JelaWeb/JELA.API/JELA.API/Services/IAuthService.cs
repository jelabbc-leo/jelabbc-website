using JELA.API.Models;

namespace JELA.API.Services;

/// <summary>
/// Interfaz para servicio de autenticación
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Autentica un usuario con credenciales
    /// </summary>
    Task<AuthResponse> AuthenticateAsync(string username, string password);

    /// <summary>
    /// Genera un nuevo token usando el refresh token
    /// </summary>
    Task<AuthResponse> RefreshTokenAsync(string refreshToken);

    /// <summary>
    /// Valida un token JWT
    /// </summary>
    bool ValidateToken(string token);

    /// <summary>
    /// Genera un token JWT para un usuario
    /// </summary>
    string GenerateToken(UserInfo user);
}
