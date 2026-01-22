using JELA.API.Models;
using JELA.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace JELA.API.Endpoints;

/// <summary>
/// Endpoints de autenticación
/// </summary>
public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/auth")
            .WithTags("Autenticación")
            .WithOpenApi();

        // POST: Login
        group.MapPost("/login", Login)
            .WithName("Login")
            .WithDescription("Autentica un usuario y retorna un token JWT")
            .AllowAnonymous()
            .Produces<AuthResponse>(StatusCodes.Status200OK)
            .Produces<AuthResponse>(StatusCodes.Status401Unauthorized);

        // POST: Refresh Token
        group.MapPost("/refresh", RefreshToken)
            .WithName("RefreshToken")
            .WithDescription("Genera un nuevo token usando el refresh token")
            .AllowAnonymous()
            .Produces<AuthResponse>(StatusCodes.Status200OK)
            .Produces<AuthResponse>(StatusCodes.Status401Unauthorized);

        // GET: Validar Token
        group.MapGet("/validate", ValidateToken)
            .WithName("ValidateToken")
            .WithDescription("Valida si el token actual es válido")
            .RequireAuthorization()
            .Produces<ApiResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized);

        // POST: Consumir Licencia
        app.MapPost("/api/usuarios/{id}/consumir-licencia", ConsumirLicencia)
            .WithName("ConsumirLicencia")
            .WithDescription("Consume una licencia del usuario para crear una nueva entidad")
            .WithTags("Autenticación")
            .RequireAuthorization()
            .Produces<ApiResponse>(StatusCodes.Status200OK)
            .Produces<ApiResponse>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> Login(
        [FromBody] LoginRequest request,
        IAuthService authService,
        ILogger<Program> logger)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
        {
            return Results.BadRequest(new AuthResponse
            {
                Success = false,
                Message = "Usuario y contraseña son requeridos"
            });
        }

        var response = await authService.AuthenticateAsync(request.Username, request.Password);

        if (!response.Success)
        {
            return Results.Json(response, statusCode: StatusCodes.Status401Unauthorized);
        }

        return Results.Ok(response);
    }

    private static async Task<IResult> RefreshToken(
        [FromBody] RefreshTokenRequest request,
        IAuthService authService)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            return Results.BadRequest(new AuthResponse
            {
                Success = false,
                Message = "Refresh token es requerido"
            });
        }

        var response = await authService.RefreshTokenAsync(request.RefreshToken);

        if (!response.Success)
        {
            return Results.Json(response, statusCode: StatusCodes.Status401Unauthorized);
        }

        return Results.Ok(response);
    }

    private static IResult ValidateToken(HttpContext context)
    {
        // Si llegamos aquí, el token es válido (RequireAuthorization lo validó)
        var userId = context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var username = context.User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;

        return Results.Ok(new ApiResponse
        {
            Success = true,
            Message = "Token válido",
            Data = new { UserId = userId, Username = username }
        });
    }

    /// <summary>
    /// Consume una licencia del usuario para crear una nueva entidad
    /// </summary>
    private static async Task<IResult> ConsumirLicencia(
        int id,
        IDatabaseService database,
        ILogger<Program> logger)
    {
        try
        {
            // 1. Obtener licencias actuales
            var query = "SELECT LicenciasDisponibles FROM conf_usuarios WHERE Id = @id AND Activo = 1";
            var parametros = new Dictionary<string, object> { { "@id", id } };
            var resultado = (await database.EjecutarConsultaAsync(query, parametros)).FirstOrDefault();

            if (resultado == null)
            {
                logger.LogWarning("Intento de consumir licencia para usuario inexistente: {UserId}", id);
                return Results.NotFound(new ApiResponse
                {
                    Success = false,
                    Message = "Usuario no encontrado"
                });
            }

            var licenciasActuales = resultado["LicenciasDisponibles"] != null && resultado["LicenciasDisponibles"] != DBNull.Value
                ? Convert.ToInt32(resultado["LicenciasDisponibles"])
                : 0;

            // 2. Validar que tenga licencias disponibles
            if (licenciasActuales <= 0)
            {
                logger.LogWarning("Usuario {UserId} intentó consumir licencia sin tener disponibles", id);
                return Results.BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "No tiene licencias disponibles"
                });
            }

            // 3. Decrementar licencia
            var updateQuery = @"
                UPDATE conf_usuarios 
                SET LicenciasDisponibles = LicenciasDisponibles - 1 
                WHERE Id = @id";

            await database.EjecutarNoConsultaAsync(updateQuery, parametros);

            // 4. Log
            logger.LogInformation("Usuario {UserId} consumió una licencia. Restantes: {LicenciasRestantes}",
                id, licenciasActuales - 1);

            // 5. Retornar licencias restantes
            return Results.Ok(new ApiResponse
            {
                Success = true,
                Message = "Licencia consumida exitosamente",
                Data = new
                {
                    LicenciasRestantes = licenciasActuales - 1
                }
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error al consumir licencia para usuario {UserId}", id);
            return Results.Problem("Error al consumir licencia");
        }
    }
}
