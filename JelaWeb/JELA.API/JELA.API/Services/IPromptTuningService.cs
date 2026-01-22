using JELA.API.Models;

namespace JELA.API.Services;

/// <summary>
/// Interfaz para servicio de ajuste automático de prompts
/// </summary>
public interface IPromptTuningService
{
    /// <summary>
    /// Analiza el rendimiento de los prompts actuales
    /// </summary>
    /// <param name="idEntidad">ID de la entidad</param>
    /// <param name="diasAnalisis">Número de días a analizar</param>
    /// <returns>Análisis de rendimiento de prompts</returns>
    Task<Dictionary<int, PromptAnalisisDto>> AnalizarRendimientoPromptsAsync(int idEntidad, int diasAnalisis = 14);

    /// <summary>
    /// Propone ajustes automáticos a los prompts basados en métricas
    /// </summary>
    /// <param name="idPrompt">ID del prompt a ajustar</param>
    /// <param name="idEntidad">ID de la entidad</param>
    /// <returns>Propuesta de ajuste</returns>
    Task<PromptAjusteRequest?> ProponerAjusteAsync(int idPrompt, int idEntidad);

    /// <summary>
    /// Registra un ajuste de prompt en el log
    /// </summary>
    /// <param name="request">Datos del ajuste</param>
    /// <returns>Respuesta con ID del ajuste registrado</returns>
    Task<PromptAjusteResponse> RegistrarAjusteAsync(PromptAjusteRequest request);

    /// <summary>
    /// Aprueba un ajuste de prompt propuesto
    /// </summary>
    /// <param name="idAjuste">ID del ajuste a aprobar</param>
    /// <param name="idUsuarioAprobacion">ID del usuario que aprueba</param>
    /// <returns>True si se aprobó exitosamente</returns>
    Task<bool> AprobarAjusteAsync(int idAjuste, int idUsuarioAprobacion);

    /// <summary>
    /// Obtiene ajustes pendientes de aprobación
    /// </summary>
    /// <param name="idEntidad">ID de la entidad</param>
    /// <returns>Lista de ajustes pendientes</returns>
    Task<IEnumerable<CrudDto>> ObtenerAjustesPendientesAsync(int idEntidad);

    /// <summary>
    /// Obtiene un prompt por su nombre
    /// </summary>
    /// <param name="nombrePrompt">Nombre del prompt (ej: ChatWebSistema, ChatWebUsuario)</param>
    /// <param name="idEntidad">ID de la entidad</param>
    /// <returns>Contenido del prompt o null si no existe</returns>
    Task<string?> ObtenerPromptPorNombreAsync(string nombrePrompt, int idEntidad);
}

/// <summary>
/// DTO para análisis de rendimiento de prompt
/// </summary>
public class PromptAnalisisDto
{
    public int IdPrompt { get; set; }
    public int TotalUsos { get; set; }
    public int UsosExitosos { get; set; }
    public decimal TasaExito { get; set; }
    public decimal TiempoPromedioMs { get; set; }
    public int TokensPromedio { get; set; }
    public decimal CostoPromedio { get; set; }
    public decimal CSATPromedio { get; set; }
    public List<string> ProblemasDetectados { get; set; } = new();
    public List<string> SugerenciasMejora { get; set; } = new();
}
