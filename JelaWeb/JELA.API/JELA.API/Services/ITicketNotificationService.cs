using JELA.API.Models;

namespace JELA.API.Services;

/// <summary>
/// Interfaz para servicio de notificaciones de tickets
/// </summary>
public interface ITicketNotificationService
{
    /// <summary>
    /// Encola una notificación WhatsApp para envío posterior
    /// </summary>
    /// <param name="request">Datos de la notificación</param>
    /// <returns>Respuesta con ID de notificación encolada</returns>
    Task<NotificacionWhatsAppResponse> EncolarNotificacionAsync(NotificacionWhatsAppRequest request);

    /// <summary>
    /// Procesa la cola de notificaciones pendientes
    /// </summary>
    /// <param name="maxNotificaciones">Número máximo de notificaciones a procesar</param>
    /// <returns>Número de notificaciones procesadas</returns>
    Task<int> ProcesarColaAsync(int maxNotificaciones = 10);

    /// <summary>
    /// Envía una notificación WhatsApp inmediatamente (sin cola)
    /// </summary>
    /// <param name="numeroWhatsApp">Número de WhatsApp destino</param>
    /// <param name="mensaje">Mensaje a enviar</param>
    /// <returns>True si se envió exitosamente</returns>
    Task<bool> EnviarWhatsAppInmediatoAsync(string numeroWhatsApp, string mensaje);

    /// <summary>
    /// Obtiene notificaciones pendientes de la cola
    /// </summary>
    /// <param name="idEntidad">ID de la entidad</param>
    /// <param name="limite">Número máximo de notificaciones a obtener</param>
    /// <returns>Lista de notificaciones pendientes</returns>
    Task<IEnumerable<CrudDto>> ObtenerNotificacionesPendientesAsync(int idEntidad, int limite = 50);

    /// <summary>
    /// Actualiza el estado de una notificación
    /// </summary>
    /// <param name="idNotificacion">ID de la notificación</param>
    /// <param name="estado">Nuevo estado</param>
    /// <param name="mensajeError">Mensaje de error (opcional)</param>
    Task ActualizarEstadoNotificacionAsync(int idNotificacion, string estado, string? mensajeError = null);
}
