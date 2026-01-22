using JELA.API.Models;

namespace JELA.API.Services;

/// <summary>
/// Interfaz para servicio de integración con VAPI (Voice AI Platform)
/// </summary>
public interface IVapiService
{
    /// <summary>
    /// Procesa una llamada recibida de VAPI
    /// </summary>
    /// <param name="webhook">Datos del webhook de VAPI</param>
    /// <returns>True si se procesó exitosamente</returns>
    Task<bool> ProcesarLlamadaAsync(VapiWebhookRequest webhook);

    /// <summary>
    /// Valida la firma del webhook de VAPI
    /// </summary>
    /// <param name="payload">Payload del webhook</param>
    /// <param name="signature">Firma recibida en el header</param>
    /// <returns>True si la firma es válida</returns>
    bool ValidarWebhook(string payload, string signature);

    /// <summary>
    /// Obtiene la transcripción completa de una llamada
    /// </summary>
    /// <param name="callId">ID de la llamada en VAPI</param>
    /// <returns>Transcripción de la llamada</returns>
    Task<string?> ObtenerTranscripcionAsync(string callId);

    /// <summary>
    /// Procesa un evento de VAPI (call.started, call.ended, etc.)
    /// </summary>
    /// <param name="evento">Datos del evento</param>
    /// <returns>True si se procesó exitosamente</returns>
    Task<bool> ProcesarEventoAsync(VapiEventRequest evento);
}
