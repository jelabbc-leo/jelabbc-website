using JELA.API.Configuration;
using JELA.API.Models;
using JELA.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace JELA.API.Endpoints;

/// <summary>
/// Endpoints CRUD dinámicos
/// </summary>
public static class CrudEndpoints
{
    public static void MapCrudEndpoints(this WebApplication app)
    {
        // ========================================
        // Rutas CRUD - usar solo minúsculas (Windows es case-insensitive)
        // Las rutas /api/CRUD y /api/crud apuntan al mismo endpoint
        // ========================================
        
        var group = app.MapGroup("/api/crud")
            .WithTags("CRUD")
            .WithOpenApi();

        // GET: Ejecutar consulta SELECT
        group.MapGet("", GetRegistros)
            .WithName("GetRegistros")
            .WithDescription("Ejecuta una consulta SELECT y retorna los resultados")
            .RequireAuthorization() // Requiere autenticación JWT
            .Produces<IEnumerable<CrudDto>>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<ErrorResponse>(StatusCodes.Status401Unauthorized)
            .Produces<ErrorResponse>(StatusCodes.Status500InternalServerError);

        // POST: Insertar registro usando ?table= query parameter (compatibilidad API vieja)
        group.MapPost("", InsertarRegistroCompat)
            .WithName("InsertarRegistroCompat")
            .WithDescription("Inserta registro usando ?table= (compatibilidad con API vieja)")
            .RequireAuthorization()
            .Produces<InsertResponse>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<ErrorResponse>(StatusCodes.Status401Unauthorized)
            .Produces<ErrorResponse>(StatusCodes.Status403Forbidden);

        // POST: Insertar registro con path parameter (nueva forma)
        group.MapPost("/{tabla}", InsertarRegistro)
            .WithName("InsertarRegistro")
            .WithDescription("Inserta un nuevo registro en la tabla especificada")
            .RequireAuthorization()
            .Produces<InsertResponse>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<ErrorResponse>(StatusCodes.Status401Unauthorized)
            .Produces<ErrorResponse>(StatusCodes.Status403Forbidden)
            .Produces<ErrorResponse>(StatusCodes.Status500InternalServerError);

        // PUT: Actualizar registro
        group.MapPut("/{tabla}/{id:int}", ActualizarRegistro)
            .WithName("ActualizarRegistro")
            .WithDescription("Actualiza un registro existente por ID")
            .RequireAuthorization()
            .Produces<string>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<ErrorResponse>(StatusCodes.Status401Unauthorized)
            .Produces<ErrorResponse>(StatusCodes.Status403Forbidden)
            .Produces<ErrorResponse>(StatusCodes.Status500InternalServerError);

        // DELETE: Eliminar registro por ID (compatibilidad API vieja)
        group.MapDelete("/{tabla}/{id:int}", EliminarRegistroPorId)
            .WithName("EliminarRegistroPorId")
            .WithDescription("Elimina un registro por ID")
            .RequireAuthorization()
            .Produces<string>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<ErrorResponse>(StatusCodes.Status401Unauthorized)
            .Produces<ErrorResponse>(StatusCodes.Status403Forbidden);

        // DELETE: Eliminar registro con query params (nueva forma)
        group.MapDelete("/{tabla}", EliminarRegistro)
            .WithName("EliminarRegistro")
            .WithDescription("Elimina un registro usando idField e idValue")
            .RequireAuthorization()
            .Produces<string>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<ErrorResponse>(StatusCodes.Status401Unauthorized)
            .Produces<ErrorResponse>(StatusCodes.Status403Forbidden)
            .Produces<ErrorResponse>(StatusCodes.Status500InternalServerError);
    }

    /// <summary>
    /// Ejecuta una consulta SELECT
    /// </summary>
    private static async Task<IResult> GetRegistros(
        [FromQuery] string strQuery,
        IDatabaseService database,
        ILogger<Program> logger)
    {
        try
        {
            // Validar que sea una consulta SELECT
            if (string.IsNullOrWhiteSpace(strQuery))
            {
                return Results.BadRequest(new ErrorResponse { Mensaje = "El parámetro strQuery es requerido" });
            }

            var queryTrimmed = strQuery.Trim();
            if (!queryTrimmed.StartsWith("SELECT", StringComparison.OrdinalIgnoreCase))
            {
                return Results.BadRequest(new ErrorResponse { Mensaje = "Solo se permiten consultas SELECT" });
            }

            // Ejecutar consulta
            var datos = await database.EjecutarConsultaAsync(strQuery);
            return Results.Ok(datos);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error ejecutando consulta: {Query}", strQuery);
            return Results.Problem(
                detail: ex.Message,
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Error al ejecutar la consulta"
            );
        }
    }

    /// <summary>
    /// Inserta un nuevo registro
    /// </summary>
    private static async Task<IResult> InsertarRegistro(
        string tabla,
        [FromBody] CrudRequest request,
        IDatabaseService database,
        IOptions<AllowedTablesSettings> tablesSettings,
        ILogger<Program> logger)
    {
        return await InsertarRegistroInterno(tabla, request, database, tablesSettings, logger);
    }

    /// <summary>
    /// Inserta registro (compatibilidad con API vieja usando ?table= query parameter)
    /// </summary>
    private static async Task<IResult> InsertarRegistroCompat(
        [FromQuery] string table,
        [FromBody] CrudRequest request,
        IDatabaseService database,
        IOptions<AllowedTablesSettings> tablesSettings,
        ILogger<Program> logger)
    {
        if (string.IsNullOrWhiteSpace(table))
        {
            return Results.BadRequest(new ErrorResponse { Mensaje = "El parámetro 'table' es requerido" });
        }
        return await InsertarRegistroInterno(table, request, database, tablesSettings, logger);
    }

    /// <summary>
    /// Método interno para insertar registro
    /// </summary>
    private static async Task<IResult> InsertarRegistroInterno(
        string tabla,
        CrudRequest request,
        IDatabaseService database,
        IOptions<AllowedTablesSettings> tablesSettings,
        ILogger<Program> logger)
    {
        try
        {
            // Validar tabla permitida
            if (!tablesSettings.Value.IsTableAllowed(tabla))
            {
                return Results.Json(
                    new ErrorResponse { Mensaje = $"Operaciones no permitidas en la tabla '{tabla}'" },
                    statusCode: StatusCodes.Status403Forbidden
                );
            }

            // Validar campos
            if (request.Campos == null || request.Campos.Count == 0)
            {
                return Results.BadRequest(new ErrorResponse { Mensaje = "Se requieren campos para insertar" });
            }

            // Convertir campos a diccionario simple
            // Usar GetValorNativo() para convertir JsonElement a tipos primitivos
            // (MySqlConnector no acepta System.Text.Json.JsonElement como parámetro)
            var campos = request.Campos.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.GetValorNativo() ?? DBNull.Value
            );

            // Insertar y obtener ID
            var id = await database.InsertarAsync(tabla, campos!);

            // Respuesta compatible con API vieja
            return Results.Ok(new { id = id, mensaje = "Registro insertado correctamente." });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error insertando en {Tabla}", tabla);
            return Results.Problem(
                detail: ex.Message,
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Error al insertar registro"
            );
        }
    }

    /// <summary>
    /// Actualiza un registro existente
    /// </summary>
    private static async Task<IResult> ActualizarRegistro(
        string tabla,
        int id,
        [FromBody] CrudRequest request,
        IDatabaseService database,
        IOptions<AllowedTablesSettings> tablesSettings,
        ILogger<Program> logger)
    {
        try
        {
            // Validar tabla permitida
            if (!tablesSettings.Value.IsTableAllowed(tabla))
            {
                return Results.Json(
                    new ErrorResponse { Mensaje = $"Operaciones no permitidas en la tabla '{tabla}'" },
                    statusCode: StatusCodes.Status403Forbidden
                );
            }

            // Validar campos
            if (request.Campos == null || request.Campos.Count == 0)
            {
                return Results.BadRequest(new ErrorResponse { Mensaje = "Se requieren campos para actualizar" });
            }

            // Convertir campos (con conversión de JsonElement a primitivos)
            var campos = request.Campos.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.GetValorNativo() ?? DBNull.Value
            );

            // Actualizar
            var actualizado = await database.ActualizarAsync(tabla, id, campos!);

            if (!actualizado)
            {
                return Results.NotFound(new ErrorResponse { Mensaje = $"Registro con ID {id} no encontrado" });
            }

            return Results.Ok("Registro actualizado correctamente.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error actualizando en {Tabla} ID {Id}", tabla, id);
            return Results.Problem(
                detail: ex.Message,
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Error al actualizar registro"
            );
        }
    }

    /// <summary>
    /// Elimina un registro
    /// </summary>
    private static async Task<IResult> EliminarRegistro(
        string tabla,
        [FromQuery] string idField,
        [FromQuery] string idValue,
        IDatabaseService database,
        IOptions<AllowedTablesSettings> tablesSettings,
        ILogger<Program> logger)
    {
        try
        {
            // Validar tabla permitida
            if (!tablesSettings.Value.IsTableAllowed(tabla))
            {
                return Results.Json(
                    new ErrorResponse { Mensaje = $"Operaciones no permitidas en la tabla '{tabla}'" },
                    statusCode: StatusCodes.Status403Forbidden
                );
            }

            // Validar parámetros
            if (string.IsNullOrWhiteSpace(idField) || string.IsNullOrWhiteSpace(idValue))
            {
                return Results.BadRequest(new ErrorResponse { Mensaje = "Se requieren idField e idValue" });
            }

            // Eliminar
            var eliminado = await database.EliminarAsync(tabla, idField, idValue);

            if (!eliminado)
            {
                return Results.NotFound(new ErrorResponse { Mensaje = "Registro no encontrado" });
            }

            return Results.Ok("Registro eliminado correctamente.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error eliminando de {Tabla}", tabla);
            return Results.Problem(
                detail: ex.Message,
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Error al eliminar registro"
            );
        }
    }

    /// <summary>
    /// Elimina un registro por ID (compatibilidad con API vieja)
    /// </summary>
    private static async Task<IResult> EliminarRegistroPorId(
        string tabla,
        int id,
        IDatabaseService database,
        IOptions<AllowedTablesSettings> tablesSettings,
        ILogger<Program> logger)
    {
        try
        {
            // Validar tabla permitida
            if (!tablesSettings.Value.IsTableAllowed(tabla))
            {
                return Results.Json(
                    new ErrorResponse { Mensaje = $"Operaciones no permitidas en la tabla '{tabla}'" },
                    statusCode: StatusCodes.Status403Forbidden
                );
            }

            // Eliminar usando "Id" como campo por defecto
            var eliminado = await database.EliminarAsync(tabla, "Id", id);

            if (!eliminado)
            {
                return Results.NotFound(new ErrorResponse { Mensaje = $"Registro con ID {id} no encontrado" });
            }

            return Results.Ok("Registro eliminado correctamente.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error eliminando de {Tabla} ID {Id}", tabla, id);
            return Results.Problem(
                detail: ex.Message,
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Error al eliminar registro"
            );
        }
    }
}
