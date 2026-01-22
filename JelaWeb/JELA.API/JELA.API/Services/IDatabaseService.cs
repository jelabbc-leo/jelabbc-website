using JELA.API.Models;

namespace JELA.API.Services;

/// <summary>
/// Interfaz para operaciones de base de datos
/// </summary>
public interface IDatabaseService
{
    /// <summary>
    /// Ejecuta una consulta SELECT y retorna los resultados
    /// </summary>
    Task<IEnumerable<CrudDto>> EjecutarConsultaAsync(string query, Dictionary<string, object>? parametros = null);

    /// <summary>
    /// Ejecuta una consulta que no retorna resultados (INSERT, UPDATE, DELETE)
    /// </summary>
    Task<int> EjecutarNoConsultaAsync(string query, Dictionary<string, object>? parametros = null);

    /// <summary>
    /// Ejecuta una consulta y retorna un valor escalar
    /// </summary>
    Task<T?> EjecutarEscalarAsync<T>(string query, Dictionary<string, object>? parametros = null);

    /// <summary>
    /// Inserta un registro y retorna el ID generado
    /// </summary>
    Task<int> InsertarAsync(string tabla, Dictionary<string, object> campos);

    /// <summary>
    /// Actualiza un registro por ID
    /// </summary>
    Task<bool> ActualizarAsync(string tabla, int id, Dictionary<string, object> campos);

    /// <summary>
    /// Elimina un registro por ID
    /// </summary>
    Task<bool> EliminarAsync(string tabla, string campoId, object valorId);

    /// <summary>
    /// Verifica si la conexión está disponible
    /// </summary>
    Task<bool> VerificarConexionAsync();
}
