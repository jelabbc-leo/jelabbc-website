using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using SHA256 = System.Security.Cryptography.SHA256;
using System.Text;
using JELA.API.Configuration;
using JELA.API.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace JELA.API.Services;

/// <summary>
/// Implementación de autenticación con JWT
/// </summary>
public class JwtAuthService : IAuthService
{
    private readonly IDatabaseService _database;
    private readonly JwtSettings _jwtSettings;
    private readonly ILogger<JwtAuthService> _logger;

    public JwtAuthService(
        IDatabaseService database,
        IOptions<JwtSettings> jwtSettings,
        ILogger<JwtAuthService> logger)
    {
        _database = database;
        _jwtSettings = jwtSettings.Value;
        _logger = logger;
    }

    public async Task<AuthResponse> AuthenticateAsync(string username, string password)
    {
        try
        {
            // Buscar usuario en la base de datos con campos multi-entidad
            var query = @"
                SELECT u.Id, u.Username, u.Nombre, u.email as Email, u.PasswordHash,
                       u.TipoUsuario, u.IdEntidadPrincipal, u.LicenciasDisponibles,
                       COALESCE(e.RazonSocial, e.Alias) as EntidadPrincipalNombre
                FROM conf_usuarios u
                LEFT JOIN cat_entidades e ON u.IdEntidadPrincipal = e.Id
                WHERE u.Username = @username AND u.Activo = 1";

            var parametros = new Dictionary<string, object> { { "@username", username } };
            var resultados = await _database.EjecutarConsultaAsync(query, parametros);
            var usuario = resultados.FirstOrDefault();

            if (usuario == null)
            {
                _logger.LogWarning("Intento de login fallido: usuario {Username} no encontrado", username);
                return new AuthResponse { Success = false, Message = "Usuario o contraseña incorrectos" };
            }

            // Verificar contraseña (usando SHA256 como en JelaWeb AuthService)
            var passwordHash = usuario["PasswordHash"]?.ToString();
            var inputHash = ComputeSHA256Hash(password);

            if (passwordHash != inputHash)
            {
                _logger.LogWarning("Intento de login fallido: contraseña incorrecta para {Username}", username);
                return new AuthResponse { Success = false, Message = "Usuario o contraseña incorrectos" };
            }

            // Obtener entidades del usuario
            var userId = Convert.ToInt32(usuario["Id"]);
            var entidades = await ObtenerEntidadesUsuario(userId);

            // Crear información del usuario
            var userInfo = new UserInfo
            {
                Id = userId,
                Username = usuario["Username"]?.ToString() ?? string.Empty,
                Nombre = usuario["Nombre"]?.ToString() ?? string.Empty,
                Email = usuario["Email"]?.ToString(),
                RolId = null,
                RolNombre = null,
                EntidadId = null,
                EntidadNombre = null,
                // Nuevos campos multi-entidad
                TipoUsuario = usuario["TipoUsuario"]?.ToString() ?? "Residente",
                IdEntidadPrincipal = usuario["IdEntidadPrincipal"] != null && usuario["IdEntidadPrincipal"] != DBNull.Value
                    ? Convert.ToInt32(usuario["IdEntidadPrincipal"])
                    : null,
                EntidadPrincipalNombre = usuario["EntidadPrincipalNombre"]?.ToString(),
                LicenciasDisponibles = usuario["LicenciasDisponibles"] != null && usuario["LicenciasDisponibles"] != DBNull.Value
                    ? Convert.ToInt32(usuario["LicenciasDisponibles"])
                    : 0,
                Entidades = entidades
            };

            // Generar tokens
            var token = GenerateToken(userInfo);
            var refreshToken = GenerateRefreshToken();
            var expiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes);

            // Guardar refresh token en la base de datos (opcional)
            await SaveRefreshTokenAsync(userInfo.Id, refreshToken);

            _logger.LogInformation("Login exitoso para usuario {Username} (Tipo: {TipoUsuario}, Entidades: {NumEntidades})", 
                username, userInfo.TipoUsuario, entidades?.Count ?? 0);

            return new AuthResponse
            {
                Success = true,
                Token = token,
                RefreshToken = refreshToken,
                ExpiresAt = expiresAt,
                User = userInfo
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error durante autenticación de {Username}", username);
            return new AuthResponse { Success = false, Message = "Error interno de autenticación" };
        }
    }

    public async Task<AuthResponse> RefreshTokenAsync(string refreshToken)
    {
        try
        {
            // Buscar refresh token válido
            var query = @"
                SELECT u.Id, u.Username, u.Nombre, u.email as Email
                FROM conf_usuarios u
                INNER JOIN conf_refresh_tokens rt ON u.Id = rt.UserId
                WHERE rt.Token = @token AND rt.ExpiresAt > NOW() AND u.Activo = 1";

            var parametros = new Dictionary<string, object> { { "@token", refreshToken } };
            var resultados = await _database.EjecutarConsultaAsync(query, parametros);
            var usuario = resultados.FirstOrDefault();

            if (usuario == null)
            {
                return new AuthResponse { Success = false, Message = "Refresh token inválido o expirado" };
            }

            var userInfo = new UserInfo
            {
                Id = Convert.ToInt32(usuario["Id"]),
                Username = usuario["Username"]?.ToString() ?? string.Empty,
                Nombre = usuario["Nombre"]?.ToString() ?? string.Empty,
                Email = usuario["Email"]?.ToString(),
                RolId = null,
                RolNombre = null,
                EntidadId = null,
                EntidadNombre = null
            };

            // Generar nuevos tokens
            var newToken = GenerateToken(userInfo);
            var newRefreshToken = GenerateRefreshToken();

            // Actualizar refresh token
            await SaveRefreshTokenAsync(userInfo.Id, newRefreshToken);

            return new AuthResponse
            {
                Success = true,
                Token = newToken,
                RefreshToken = newRefreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes),
                User = userInfo
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error durante refresh token");
            return new AuthResponse { Success = false, Message = "Error interno" };
        }
    }

    public string GenerateToken(UserInfo user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Username),
            new("nombre", user.Nombre),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        if (!string.IsNullOrEmpty(user.Email))
            claims.Add(new Claim(ClaimTypes.Email, user.Email));

        if (user.RolId.HasValue)
        {
            claims.Add(new Claim("rolId", user.RolId.Value.ToString()));
            if (!string.IsNullOrEmpty(user.RolNombre))
                claims.Add(new Claim(ClaimTypes.Role, user.RolNombre));
        }

        if (user.EntidadId.HasValue)
        {
            claims.Add(new Claim("entidadId", user.EntidadId.Value.ToString()));
            if (!string.IsNullOrEmpty(user.EntidadNombre))
                claims.Add(new Claim("entidadNombre", user.EntidadNombre));
        }

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public bool ValidateToken(string token)
    {
        try
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
            var handler = new JwtSecurityTokenHandler();

            handler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _jwtSettings.Issuer,
                ValidAudience = _jwtSettings.Audience,
                IssuerSigningKey = key
            }, out _);

            return true;
        }
        catch
        {
            return false;
        }
    }

    private static string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    private async Task SaveRefreshTokenAsync(int userId, string refreshToken)
    {
        try
        {
            // Eliminar tokens anteriores del usuario
            await _database.EjecutarNoConsultaAsync(
                "DELETE FROM conf_refresh_tokens WHERE UserId = @userId",
                new Dictionary<string, object> { { "@userId", userId } });

            // Insertar nuevo token
            var campos = new Dictionary<string, object>
            {
                { "UserId", userId },
                { "Token", refreshToken },
                { "ExpiresAt", DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays) },
                { "CreatedAt", DateTime.UtcNow }
            };

            await _database.InsertarAsync("conf_refresh_tokens", campos);
        }
        catch (Exception ex)
        {
            // Si la tabla no existe, solo logueamos (no es crítico)
            _logger.LogWarning(ex, "No se pudo guardar refresh token (la tabla puede no existir)");
        }
    }

    /// <summary>
    /// Obtiene las entidades asignadas a un usuario
    /// </summary>
    /// <param name="userId">ID del usuario</param>
    /// <returns>Lista de entidades del usuario</returns>
    private async Task<List<EntidadInfo>> ObtenerEntidadesUsuario(int userId)
    {
        try
        {
            var query = @"
                SELECT e.Id, COALESCE(e.RazonSocial, e.Alias) as Nombre, 
                       CONCAT(e.NombreVialidad, ' ', e.NoExterior, ', ', e.Colonia) as Direccion, 
                       e.LogoPath as Logotipo, ue.EsPrincipal
                FROM cat_entidades e
                INNER JOIN conf_usuario_entidades ue ON e.Id = ue.IdEntidad
                WHERE ue.IdUsuario = @userId AND ue.Activo = 1 AND e.Activo = 1
                ORDER BY ue.EsPrincipal DESC, COALESCE(e.RazonSocial, e.Alias)";

            var parametros = new Dictionary<string, object> { { "@userId", userId } };
            var resultados = await _database.EjecutarConsultaAsync(query, parametros);

            return resultados.Select(r => new EntidadInfo
            {
                Id = Convert.ToInt32(r["Id"]),
                Nombre = r["Nombre"]?.ToString() ?? string.Empty,
                Direccion = r["Direccion"]?.ToString(),
                Logotipo = r["Logotipo"]?.ToString(),
                EsPrincipal = r["EsPrincipal"] != null && r["EsPrincipal"] != DBNull.Value && Convert.ToBoolean(r["EsPrincipal"])
            }).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener entidades del usuario {UserId}", userId);
            return new List<EntidadInfo>();
        }
    }

    private static string ComputeSHA256Hash(string input)
    {
        var inputBytes = Encoding.UTF8.GetBytes(input);
        var hashBytes = SHA256.HashData(inputBytes);
        var builder = new StringBuilder();
        foreach (var b in hashBytes)
        {
            builder.Append(b.ToString("x2"));
        }
        return builder.ToString();
    }
}
