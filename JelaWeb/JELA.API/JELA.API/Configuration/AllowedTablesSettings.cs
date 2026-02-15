namespace JELA.API.Configuration;

/// <summary>
/// Configuración de tablas permitidas para operaciones CRUD
/// </summary>
public class AllowedTablesSettings
{
    /// <summary>
    /// Lista de prefijos de tablas permitidos
    /// </summary>
    public List<string> AllowedPrefixes { get; set; } = new()
    {
        "cat_",      // Catálogos
        "conf_",     // Configuración
        "op_",       // Operaciones
        "log_",      // Logs
        "vw_"        // Vistas
    };

    /// <summary>
    /// Lista de tablas específicas permitidas (sin prefijo)
    /// Para el módulo de tracking/IA que necesita escribir en estas tablas
    /// </summary>
    public List<string> AllowedTables { get; set; } = new()
    {
        "unidades_viajes",
        "contactos_viaje",
        "eventos_unidad"
    };

    /// <summary>
    /// Lista de tablas específicas bloqueadas
    /// </summary>
    public List<string> BlockedTables { get; set; } = new()
    {
        "conf_usuarios",        // No permitir manipulación directa de usuarios
        "conf_refresh_tokens"   // No permitir manipulación de tokens
    };

    /// <summary>
    /// Valida si una tabla está permitida
    /// </summary>
    public bool IsTableAllowed(string tableName)
    {
        var lowerTable = tableName.ToLower();

        // Verificar si está bloqueada
        if (BlockedTables.Any(t => t.Equals(lowerTable, StringComparison.OrdinalIgnoreCase)))
            return false;

        // Verificar si está en la lista de tablas permitidas explícitamente
        if (AllowedTables.Any(t => t.Equals(lowerTable, StringComparison.OrdinalIgnoreCase)))
            return true;

        // Verificar si tiene un prefijo permitido
        return AllowedPrefixes.Any(p => lowerTable.StartsWith(p.ToLower()));
    }
}
