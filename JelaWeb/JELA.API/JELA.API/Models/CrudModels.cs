namespace JELA.API.Models;

/// <summary>
/// Campo con valor y tipo para operaciones CRUD
/// </summary>
public class CampoConTipo
{
    public object? Valor { get; set; }
    public string Tipo { get; set; } = "System.String";
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
