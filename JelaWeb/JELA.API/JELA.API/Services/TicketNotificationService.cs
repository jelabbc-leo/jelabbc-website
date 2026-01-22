using JELA.API.Models;

namespace JELA.API.Services;

/// <summary>
/// Servicio para gestión de notificaciones de tickets
/// </summary>
public class TicketNotificationService : ITicketNotificationService
{
    private readonly IDatabaseService _db;
    private readonly ILogger<TicketNotificationService> _logger;
    private readonly IConfiguration _configuration;

    public TicketNotificationService(
        IDatabaseService databaseService,
        ILogger<TicketNotificationService> logger,
        IConfiguration configuration)
    {
        _db = databaseService;
        _logger = logger;
        _configuration = configuration;
    }

    /// <summary>
    /// Encola una notificación WhatsApp usando el stored procedure
    /// </summary>
    public async Task<NotificacionWhatsAppResponse> EncolarNotificacionAsync(NotificacionWhatsAppRequest request)
    {
        try
        {
            _logger.LogInformation(
                "Encolando notificación WhatsApp - Ticket: {IdTicket}, Número: {Numero}",
                request.IdTicket, request.NumeroWhatsApp);

            // Llamar al stored procedure sp_EncolarNotificacionWhatsApp
            var query = @"
                CALL sp_EncolarNotificacionWhatsApp(
                    @IdTicket,
                    @NumeroWhatsApp,
                    @TipoNotificacion,
                    @MensajeTexto,
                    @IdEntidad
                )";

            var parametros = new Dictionary<string, object>
            {
                { "@IdTicket", request.IdTicket },
                { "@NumeroWhatsApp", request.NumeroWhatsApp },
                { "@TipoNotificacion", request.TipoNotificacion },
                { "@MensajeTexto", request.MensajeTexto },
                { "@IdEntidad", request.IdEntidad }
            };

            var resultados = await _db.EjecutarConsultaAsync(query, parametros);
            var resultado = resultados.FirstOrDefault();

            if (resultado != null && resultado["IdNotificacion"] != null)
            {
                var idNotificacion = Convert.ToInt32(resultado["IdNotificacion"]);

                _logger.LogInformation(
                    "Notificación encolada exitosamente - ID: {IdNotificacion}",
                    idNotificacion);

                return new NotificacionWhatsAppResponse
                {
                    Success = true,
                    IdNotificacion = idNotificacion,
                    Mensaje = "Notificación encolada exitosamente"
                };
            }

            return new NotificacionWhatsAppResponse
            {
                Success = false,
                Mensaje = "Error al encolar notificación"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al encolar notificación WhatsApp");
            return new NotificacionWhatsAppResponse
            {
                Success = false,
                Mensaje = $"Error: {ex.Message}"
            };
        }
    }

    /// <summary>
    /// Procesa la cola de notificaciones pendientes
    /// </summary>
    public async Task<int> ProcesarColaAsync(int maxNotificaciones = 10)
    {
        try
        {
            _logger.LogInformation("Procesando cola de notificaciones - Máximo: {Max}", maxNotificaciones);

            // Obtener notificaciones pendientes que estén listas para enviar
            var query = @"
                SELECT * FROM op_ticket_notificaciones_whatsapp
                WHERE Estado IN ('Pendiente', 'Fallido')
                  AND IntentosEnvio < MaximoIntentos
                  AND (ProximoIntento IS NULL OR ProximoIntento <= NOW())
                  AND Activo = 1
                ORDER BY FechaCreacion ASC
                LIMIT @Limite";

            var parametros = new Dictionary<string, object>
            {
                { "@Limite", maxNotificaciones }
            };

            var notificaciones = await _db.EjecutarConsultaAsync(query, parametros);
            var procesadas = 0;

            foreach (var notificacion in notificaciones)
            {
                var idNotificacion = Convert.ToInt32(notificacion["Id"]);
                var numeroWhatsApp = notificacion["NumeroWhatsApp"]?.ToString() ?? "";
                var mensajeTexto = notificacion["MensajeTexto"]?.ToString() ?? "";

                try
                {
                    // Actualizar estado a "Enviando"
                    await ActualizarEstadoNotificacionAsync(idNotificacion, "Enviando");

                    // Enviar mensaje por WhatsApp
                    var enviado = await EnviarWhatsAppInmediatoAsync(numeroWhatsApp, mensajeTexto);

                    if (enviado)
                    {
                        // Actualizar estado a "Enviado"
                        await ActualizarEstadoNotificacionAsync(idNotificacion, "Enviado");
                        procesadas++;

                        _logger.LogInformation(
                            "Notificación enviada exitosamente - ID: {Id}",
                            idNotificacion);
                    }
                    else
                    {
                        // Incrementar intentos y programar reintento
                        await RegistrarFalloEnvioAsync(idNotificacion, "Error al enviar mensaje");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al procesar notificación {Id}", idNotificacion);
                    await RegistrarFalloEnvioAsync(idNotificacion, ex.Message);
                }
            }

            _logger.LogInformation(
                "Cola procesada - {Procesadas}/{Total} notificaciones enviadas",
                procesadas, notificaciones.Count());

            return procesadas;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al procesar cola de notificaciones");
            return 0;
        }
    }

    /// <summary>
    /// Envía una notificación WhatsApp inmediatamente
    /// </summary>
    public async Task<bool> EnviarWhatsAppInmediatoAsync(string numeroWhatsApp, string mensaje)
    {
        try
        {
            // TODO: Implementar integración con YCloud API
            // Por ahora, simular envío exitoso
            _logger.LogInformation(
                "Enviando WhatsApp a {Numero}: {Mensaje}",
                numeroWhatsApp, mensaje.Substring(0, Math.Min(50, mensaje.Length)));

            // Simular delay de red
            await Task.Delay(100);

            // TODO: Reemplazar con llamada real a YCloud API
            // var yCloudService = new YCloudService();
            // return await yCloudService.EnviarMensajeAsync(numeroWhatsApp, mensaje);

            return true; // Simular éxito por ahora
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al enviar WhatsApp");
            return false;
        }
    }

    /// <summary>
    /// Obtiene notificaciones pendientes
    /// </summary>
    public async Task<IEnumerable<CrudDto>> ObtenerNotificacionesPendientesAsync(int idEntidad, int limite = 50)
    {
        try
        {
            var query = @"
                SELECT 
                    n.*,
                    t.AsuntoCorto AS AsuntoTicket
                FROM op_ticket_notificaciones_whatsapp n
                LEFT JOIN op_tickets_v2 t ON n.IdTicket = t.Id
                WHERE n.IdEntidad = @IdEntidad
                  AND n.Estado IN ('Pendiente', 'Enviando', 'Fallido')
                  AND n.Activo = 1
                ORDER BY n.FechaCreacion DESC
                LIMIT @Limite";

            var parametros = new Dictionary<string, object>
            {
                { "@IdEntidad", idEntidad },
                { "@Limite", limite }
            };

            return await _db.EjecutarConsultaAsync(query, parametros);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener notificaciones pendientes");
            throw;
        }
    }

    /// <summary>
    /// Actualiza el estado de una notificación
    /// </summary>
    public async Task ActualizarEstadoNotificacionAsync(int idNotificacion, string estado, string? mensajeError = null)
    {
        try
        {
            var campos = new Dictionary<string, object>
            {
                { "Estado", estado },
                { "MensajeError", mensajeError ?? (object)DBNull.Value },
                { "FechaEnvio", estado == "Enviado" ? DateTime.Now : (object)DBNull.Value },
                { "FechaUltimaActualizacion", DateTime.Now }
            };

            // Si el estado es "Enviado", actualizar FechaEnvio
            if (estado == "Enviado")
            {
                campos["FechaEnvio"] = DateTime.Now;
            }

            await _db.ActualizarAsync("op_ticket_notificaciones_whatsapp", idNotificacion, campos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar estado de notificación");
            throw;
        }
    }

    /// <summary>
    /// Registra un fallo en el envío y programa reintento
    /// </summary>
    private async Task RegistrarFalloEnvioAsync(int idNotificacion, string mensajeError)
    {
        try
        {
            // Primero obtener los valores actuales
            var queryActual = @"
                SELECT IntentosEnvio, MaximoIntentos 
                FROM op_ticket_notificaciones_whatsapp 
                WHERE Id = @Id";
            
            var parametrosActual = new Dictionary<string, object>
            {
                { "@Id", idNotificacion }
            };
            
            var resultado = (await _db.EjecutarConsultaAsync(queryActual, parametrosActual)).FirstOrDefault();
            
            if (resultado != null)
            {
                var intentosActual = Convert.ToInt32(resultado["IntentosEnvio"] ?? 0);
                var maximoIntentos = Convert.ToInt32(resultado["MaximoIntentos"] ?? 3);
                var nuevoIntentos = intentosActual + 1;
                
                var campos = new Dictionary<string, object>
                {
                    { "IntentosEnvio", nuevoIntentos },
                    { "Estado", nuevoIntentos >= maximoIntentos ? "Fallido" : "Pendiente" },
                    { "MensajeError", mensajeError },
                    { "ProximoIntento", DateTime.Now.AddMinutes(nuevoIntentos * 5) },
                    { "FechaUltimaActualizacion", DateTime.Now }
                };

                await _db.ActualizarAsync("op_ticket_notificaciones_whatsapp", idNotificacion, campos);

                _logger.LogWarning(
                    "Fallo registrado para notificación {Id} - Reintento programado",
                    idNotificacion);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al registrar fallo de envío");
        }
    }
}
