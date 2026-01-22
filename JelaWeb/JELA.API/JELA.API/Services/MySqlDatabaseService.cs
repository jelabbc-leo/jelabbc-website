using System.Data;
using Dapper;
using JELA.API.Models;
using MySqlConnector;

namespace JELA.API.Services;

/// <summary>
/// Implementación de IDatabaseService para MySQL
/// </summary>
public class MySqlDatabaseService : IDatabaseService
{
    private readonly string _connectionString;
    private readonly ILogger<MySqlDatabaseService> _logger;

    public MySqlDatabaseService(IConfiguration configuration, ILogger<MySqlDatabaseService> logger)
    {
        _connectionString = configuration.GetConnectionString("MySQL") 
            ?? throw new InvalidOperationException("Connection string 'MySQL' not found");
        _logger = logger;
    }

    public async Task<IEnumerable<CrudDto>> EjecutarConsultaAsync(string query, Dictionary<string, object>? parametros = null)
    {
        _logger.LogDebug("Ejecutando consulta: {Query}", query);

        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var command = new MySqlCommand(query, connection);
        
        if (parametros != null)
        {
            foreach (var param in parametros)
            {
                command.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
            }
        }

        var resultados = new List<CrudDto>();

        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            var dto = new CrudDto();

            for (int i = 0; i < reader.FieldCount; i++)
            {
                var nombre = reader.GetName(i);
                var valor = reader.IsDBNull(i) ? null : reader.GetValue(i);
                var tipo = reader.GetFieldType(i).FullName ?? "System.Object";

                dto.Campos[nombre] = new CampoConTipo
                {
                    Valor = valor,
                    Tipo = tipo
                };
            }

            resultados.Add(dto);
        }

        _logger.LogDebug("Consulta retornó {Count} registros", resultados.Count);
        return resultados;
    }

    public async Task<int> EjecutarNoConsultaAsync(string query, Dictionary<string, object>? parametros = null)
    {
        _logger.LogDebug("Ejecutando comando: {Query}", query);

        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var command = new MySqlCommand(query, connection);

        if (parametros != null)
        {
            foreach (var param in parametros)
            {
                command.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
            }
        }

        var rowsAffected = await command.ExecuteNonQueryAsync();
        _logger.LogDebug("Comando afectó {Rows} filas", rowsAffected);

        return rowsAffected;
    }

    public async Task<T?> EjecutarEscalarAsync<T>(string query, Dictionary<string, object>? parametros = null)
    {
        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var command = new MySqlCommand(query, connection);

        if (parametros != null)
        {
            foreach (var param in parametros)
            {
                command.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
            }
        }

        var result = await command.ExecuteScalarAsync();

        if (result == null || result == DBNull.Value)
            return default;

        return (T)Convert.ChangeType(result, typeof(T));
    }

    public async Task<int> InsertarAsync(string tabla, Dictionary<string, object> campos)
    {
        var columnas = string.Join(",", campos.Keys.Select(k => $"`{k}`"));
        var parametros = string.Join(",", campos.Keys.Select(k => $"@{k}"));
        var query = $"INSERT INTO `{tabla}` ({columnas}) VALUES ({parametros}); SELECT LAST_INSERT_ID();";

        _logger.LogDebug("Insertando en {Tabla}: {Query}", tabla, query);

        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var command = new MySqlCommand(query, connection);

        foreach (var campo in campos)
        {
            command.Parameters.AddWithValue($"@{campo.Key}", campo.Value ?? DBNull.Value);
        }

        var result = await command.ExecuteScalarAsync();
        var id = Convert.ToInt32(result);

        _logger.LogInformation("Registro insertado en {Tabla} con ID {Id}", tabla, id);
        return id;
    }

    public async Task<bool> ActualizarAsync(string tabla, int id, Dictionary<string, object> campos)
    {
        var asignaciones = string.Join(",", campos.Keys.Select(k => $"`{k}` = @{k}"));
        var query = $"UPDATE `{tabla}` SET {asignaciones} WHERE Id = @id";

        _logger.LogDebug("Actualizando en {Tabla} ID {Id}: {Query}", tabla, id, query);

        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var command = new MySqlCommand(query, connection);

        foreach (var campo in campos)
        {
            command.Parameters.AddWithValue($"@{campo.Key}", campo.Value ?? DBNull.Value);
        }
        command.Parameters.AddWithValue("@id", id);

        var rowsAffected = await command.ExecuteNonQueryAsync();

        _logger.LogInformation("Registro {Id} actualizado en {Tabla}, filas afectadas: {Rows}", id, tabla, rowsAffected);
        return rowsAffected > 0;
    }

    public async Task<bool> EliminarAsync(string tabla, string campoId, object valorId)
    {
        var query = $"DELETE FROM `{tabla}` WHERE `{campoId}` = @id";

        _logger.LogDebug("Eliminando de {Tabla} donde {Campo} = {Valor}", tabla, campoId, valorId);

        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@id", valorId);

        var rowsAffected = await command.ExecuteNonQueryAsync();

        _logger.LogInformation("Registro eliminado de {Tabla}, filas afectadas: {Rows}", tabla, rowsAffected);
        return rowsAffected > 0;
    }

    public async Task<bool> VerificarConexionAsync()
    {
        try
        {
            await using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verificando conexión a MySQL");
            return false;
        }
    }
}
