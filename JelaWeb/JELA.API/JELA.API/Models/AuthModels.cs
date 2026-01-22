namespace JELA.API.Models;

/// <summary>
/// Request para login
/// </summary>
public class LoginRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

/// <summary>
/// Respuesta de autenticación
/// </summary>
public class AuthResponse
{
    public bool Success { get; set; }
    public string? Token { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public UserInfo? User { get; set; }
    public string? Message { get; set; }
}

/// <summary>
/// Información del usuario autenticado
/// </summary>
public class UserInfo
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string? Email { get; set; }
    public int? RolId { get; set; }
    public string? RolNombre { get; set; }
    public int? EntidadId { get; set; }
    public string? EntidadNombre { get; set; }
    
    // NUEVO: Campos para sistema multi-entidad
    /// <summary>
    /// Tipo de usuario: AdministradorCondominios, MesaDirectiva, Residente, Empleado
    /// </summary>
    public string TipoUsuario { get; set; } = "Residente";
    
    /// <summary>
    /// Lista de entidades a las que el usuario tiene acceso
    /// </summary>
    public List<EntidadInfo>? Entidades { get; set; }
    
    /// <summary>
    /// ID de la entidad principal del usuario (para usuarios de una sola entidad)
    /// </summary>
    public int? IdEntidadPrincipal { get; set; }
    
    /// <summary>
    /// Nombre de la entidad principal del usuario
    /// </summary>
    public string? EntidadPrincipalNombre { get; set; }
    
    /// <summary>
    /// Número de licencias disponibles para crear nuevas entidades (solo para AdministradorCondominios)
    /// </summary>
    public int LicenciasDisponibles { get; set; } = 0;
}

/// <summary>
/// Información de una entidad asignada a un usuario
/// </summary>
public class EntidadInfo
{
    /// <summary>
    /// ID de la entidad
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Nombre de la entidad/condominio
    /// </summary>
    public string Nombre { get; set; } = string.Empty;
    
    /// <summary>
    /// Dirección de la entidad
    /// </summary>
    public string? Direccion { get; set; }
    
    /// <summary>
    /// Indica si es la entidad principal del usuario
    /// </summary>
    public bool EsPrincipal { get; set; }
    
    /// <summary>
    /// URL o ruta del logotipo de la entidad
    /// </summary>
    public string? Logotipo { get; set; }
}

/// <summary>
/// Request para refresh token
/// </summary>
public class RefreshTokenRequest
{
    public string RefreshToken { get; set; } = string.Empty;
}
