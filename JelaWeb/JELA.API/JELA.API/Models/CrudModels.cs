using System.Text.Json;

namespace JELA.API.Models;

/// <summary>
/// Campo con valor y tipo para operaciones CRUD.
/// Incluye conversión automática de JsonElement a tipos primitivos
/// para compatibilidad con MySqlConnector.
/// </summary>
public class CampoConTipo
{
    public object? Valor { get; set; }
    public string Tipo { get; set; } = "System.String";

    /// <summary>
    /// Obtiene el valor convertido a un tipo primitivo .NET.
    /// MySqlConnector no acepta System.Text.Json.JsonElement como parámetro,
    /// por lo que necesitamos convertirlo a string, int, decimal, bool, etc.
    /// </summary>
    public object? GetValorNativo()
    {
        if (Valor is null)
            return DBNull.Value;

        // Si es JsonElement (viene de System.Text.Json), convertir a primitivo
        if (Valor is JsonElement jsonElement)
        {
            return ConvertirJsonElement(jsonElement);
        }

        // Si ya es un tipo primitivo, retornar directo
        return Valor;
    }

    /// <summary>
    /// Convierte un JsonElement al tipo primitivo .NET correspondiente
    /// </summary>
    private object? ConvertirJsonElement(JsonElement element)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.String:
                return element.GetString();

            case JsonValueKind.Number:
                // Intentar int primero, luego long, luego decimal
                if (element.TryGetInt32(out int intVal))
                    return intVal;
                if (element.TryGetInt64(out long longVal))
                    return longVal;
                if (element.TryGetDecimal(out decimal decVal))
                    return decVal;
                return element.GetDouble();

            case JsonValueKind.True:
                return true;

            case JsonValueKind.False:
                return false;

            case JsonValueKind.Null:
            case JsonValueKind.Undefined:
                return DBNull.Value;

            default:
                // Para objetos/arrays, serializar como string JSON
                return element.GetRawText();
        }
    }
}

/// <summary>
/// DTO para operaciones CRUD dinámicas
/// </summary>
public class CrudDto
{
    public Dictionary<string, CampoConTipo> Campos { get; set; } = new();

    public object? this[string key]
    {
        get => Campos.TryGetValue(key, out var campo) ? campo.Valor : null;
        set
        {
            var tipo = value?.GetType().FullName ?? "System.Object";
            Campos[key] = new CampoConTipo { Valor = value, Tipo = tipo };
        }
    }

    public string TipoDe(string key) =>
        Campos.TryGetValue(key, out var campo) ? campo.Tipo : "desconocido";
}

/// <summary>
/// Request para insertar/actualizar registros
/// </summary>
public class CrudRequest
{
    public Dictionary<string, CampoConTipo> Campos { get; set; } = new();
}

/// <summary>
/// Respuesta estándar del API
/// </summary>
public class ApiResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public object? Data { get; set; }
}

/// <summary>
/// Respuesta de inserción con ID
/// </summary>
public class InsertResponse
{
    public int Id { get; set; }
    public string Mensaje { get; set; } = string.Empty;
}

/// <summary>
/// Respuesta de error
/// </summary>
public class ErrorResponse
{
    public string Mensaje { get; set; } = string.Empty;
    public string? Detalle { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
