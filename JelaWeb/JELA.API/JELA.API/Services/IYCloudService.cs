using JELA.API.Models;

namespace JELA.API.Services;

/// <summary>
/// Interfaz para servicio de integración con YCloud (WhatsApp Business)
/// </summary>
public interface IYCloudService
{
    /// <summary>
    /// Envía un mensaje de WhatsApp a través de YCloud
    /// </summary>
    /// <param name="request">Datos del mensaje a enviar</param>
    /// <returns>Respuesta de YCloud con ID del mensaje</returns>
    Task<YCloudSendMessageResponse> EnviarMensajeAsync(YCloudSendMessageRequest request);

    /// <summary>
    /// Valida la firma del webhook de YCloud
    /// </summary>
    /// <param name="payload">Payload del webhook</param>
    /// <param name="signature">Firma recibida en el header</param>
    /// <returns>True si la firma es válida</returns>
    bool ValidarWebhook(string payload, string signature);

    /// <summary>
    /// Procesa la respuesta de un webhook de YCloud
    /// </summary>
    /// <param name="webhook">Datos del webhook</param>
    /// <returns>True si se procesó exitosamente</returns>
    Task<bool> ProcesarRespuestaAsync(YCloudWebhookRequest webhook);

    /// <summary>
    /// Obtiene el estado de un mensaje enviado
    /// </summary>
    /// <param name="messageId">ID del mensaje en YCloud</param>
    /// <returns>Estado del mensaje</returns>
    Task<YCloudMessageStatus?> ObtenerEstadoMensajeAsync(string messageId);
}
