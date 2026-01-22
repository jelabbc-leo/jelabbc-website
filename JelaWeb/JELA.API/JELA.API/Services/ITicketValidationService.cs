using JELA.API.Models;

namespace JELA.API.Services;

/// <summary>
/// Interfaz para servicio de validación de tickets
/// </summary>
public interface ITicketValidationService
{
    /// <summary>
    /// Valida si un cliente tiene tickets abiertos (prevención de duplicados)
    /// </summary>
    /// <param name="request">Datos del cliente a validar</param>
    /// <returns>Resultado de la validación</returns>
    Task<TicketValidationResponse> ValidarClienteDuplicadoAsync(TicketValidationRequest request);

    /// <summary>
    /// Obtiene el historial de tickets de un cliente por teléfono
    /// </summary>
    /// <param name="telefono">Número de teléfono del cliente</param>
    /// <param name="idEntidad">ID de la entidad</param>
    /// <returns>Lista de tickets del cliente</returns>
    Task<IEnumerable<CrudDto>> ObtenerHistorialClienteAsync(string telefono, int idEntidad);

    /// <summary>
    /// Registra o actualiza información de validación de cliente
    /// </summary>
    /// <param name="telefono">Teléfono del cliente</param>
    /// <param name="email">Email del cliente</param>
    /// <param name="ipOrigen">IP de origen</param>
    /// <param name="idTicket">ID del ticket creado</param>
    /// <param name="idEntidad">ID de la entidad</param>
    Task ActualizarValidacionClienteAsync(string? telefono, string? email, string? ipOrigen, int idTicket, int idEntidad);

    /// <summary>
    /// Bloquea un cliente por spam o abuso
    /// </summary>
    /// <param name="telefono">Teléfono del cliente</param>
    /// <param name="email">Email del cliente</param>
    /// <param name="motivo">Motivo del bloqueo</param>
    /// <param name="idEntidad">ID de la entidad</param>
    Task BloquearClienteAsync(string? telefono, string? email, string motivo, int idEntidad);
}
