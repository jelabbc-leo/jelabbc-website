using JELA.API.Models;

namespace JELA.API.Services;

/// <summary>
/// Interfaz para servicio de métricas de tickets
/// </summary>
public interface ITicketMetricsService
{
    /// <summary>
    /// Obtiene métricas de tickets en tiempo real
    /// </summary>
    /// <param name="idEntidad">ID de la entidad</param>
    /// <param name="fechaInicio">Fecha de inicio del periodo</param>
    /// <param name="fechaFin">Fecha de fin del periodo</param>
    /// <returns>Métricas calculadas</returns>
    Task<TicketMetricsDto> ObtenerMetricasTiempoRealAsync(int idEntidad, DateTime fechaInicio, DateTime fechaFin);

    /// <summary>
    /// Calcula y almacena métricas diarias usando stored procedure
    /// </summary>
    /// <param name="request">Datos para calcular métricas</param>
    /// <returns>True si se calcularon exitosamente</returns>
    Task<bool> CalcularMetricasDiariasAsync(CalcularMetricasRequest request);

    /// <summary>
    /// Obtiene métricas diarias almacenadas
    /// </summary>
    /// <param name="idEntidad">ID de la entidad</param>
    /// <param name="fechaInicio">Fecha de inicio</param>
    /// <param name="fechaFin">Fecha de fin</param>
    /// <param name="canal">Canal específico (opcional)</param>
    /// <returns>Lista de métricas diarias</returns>
    Task<IEnumerable<CrudDto>> ObtenerMetricasDiariasAsync(int idEntidad, DateTime fechaInicio, DateTime fechaFin, string? canal = null);

    /// <summary>
    /// Obtiene métricas por canal
    /// </summary>
    /// <param name="idEntidad">ID de la entidad</param>
    /// <param name="fecha">Fecha de las métricas</param>
    /// <returns>Métricas agrupadas por canal</returns>
    Task<Dictionary<string, TicketMetricsDto>> ObtenerMetricasPorCanalAsync(int idEntidad, DateTime fecha);

    /// <summary>
    /// Registra log de interacción multicanal
    /// </summary>
    /// <param name="log">Datos de la interacción</param>
    /// <returns>ID del log creado</returns>
    Task<int> RegistrarInteraccionAsync(LogInteraccionDto log);

    /// <summary>
    /// Registra log de prompt de IA
    /// </summary>
    /// <param name="log">Datos del prompt</param>
    /// <returns>ID del log creado</returns>
    Task<int> RegistrarPromptAsync(LogPromptDto log);
}
