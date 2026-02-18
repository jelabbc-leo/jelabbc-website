using System.Text.Json;
using JELA.API.Models;
using JELA.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace JELA.API.Endpoints;

/// <summary>
/// Endpoints para recibir webhooks de canales externos
/// Punto de entrada para: VAPI, YCloud, ChatWeb, Firebase
/// </summary>
public static class WebhookEndpoints
{
    public static void MapWebhookEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/webhooks")
            .WithTags("Webhooks")
            .WithOpenApi();

        // POST /api/webhooks/vapi - Llamadas telefónicas
        group.MapPost("/vapi", ProcesarWebhookVAPI)
            .WithName("WebhookVAPI")
            .WithDescription("Recibe y procesa webhooks de llamadas telefónicas de VAPI")
            .Produces<WebhookResponse>(StatusCodes.Status200OK)
            .Produces<WebhookErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<WebhookErrorResponse>(StatusCodes.Status500InternalServerError);

        // POST /api/webhooks/ycloud - WhatsApp Business
        group.MapPost("/ycloud", ProcesarWebhookYCloud)
            .WithName("WebhookYCloud")
            .WithDescription("Recibe y procesa webhooks de mensajes de WhatsApp")
            .Produces<WebhookResponse>(StatusCodes.Status200OK)
            .Produces<WebhookErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<WebhookErrorResponse>(StatusCodes.Status500InternalServerError);

        // POST /api/webhooks/chatweb - Widget de chat web
        group.MapPost("/chatweb", ProcesarWebhookChatWeb)
            .WithName("WebhookChatWeb")
            .WithDescription("Recibe y procesa mensajes del widget de chat web")
            .Produces<ChatWebResponse>(StatusCodes.Status200OK)
            .Produces<WebhookErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<WebhookErrorResponse>(StatusCodes.Status500InternalServerError);

        // POST /api/webhooks/firebase - Chat App móvil
        group.MapPost("/firebase", ProcesarWebhookFirebase)
            .WithName("WebhookFirebase")
            .WithDescription("Recibe y procesa mensajes de la app móvil")
            .Produces<WebhookResponse>(StatusCodes.Status200OK)
            .Produces<WebhookErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<WebhookErrorResponse>(StatusCodes.Status500InternalServerError);
    }

    #region Endpoint Handlers

    /// <summary>
    /// Procesa webhook de VAPI (llamadas telefónicas)
    /// </summary>
    private static async Task<IResult> ProcesarWebhookVAPI(
        [FromBody] VapiWebhookRequest request,
        [FromServices] ITicketValidationService validationService,
        [FromServices] IOpenAIService openAIService,
        [FromServices] IPromptTuningService promptService,
        [FromServices] IDatabaseService db,
        [FromServices] ILogger<Program> logger)
    {
        try
        {
            logger.LogInformation(
                "Webhook VAPI recibido - CallId: {CallId}, Teléfono: {Phone}, Duración: {Duration}s",
                request.CallId, request.PhoneNumber, request.DurationSeconds);

            // 1. Validar cliente duplicado por teléfono
            var validationRequest = new TicketValidationRequest
            {
                Telefono = request.PhoneNumber,
                Email = null,
                IPOrigen = null,
                IdEntidad = 1
            };
            var validation = await validationService.ValidarClienteDuplicadoAsync(validationRequest);

            if (validation.TieneTicketAbierto)
            {
                logger.LogWarning(
                    "Cliente {Phone} ya tiene ticket abierto #{TicketId}",
                    request.PhoneNumber, validation.IdTicketAbierto);

                return Results.Ok(new WebhookResponse
                {
                    Success = true,
                    TicketId = validation.IdTicketAbierto,
                    Mensaje = $"Cliente ya tiene ticket abierto #{validation.IdTicketAbierto}",
                    RespuestaIA = null
                });
            }

            // 2. Obtener prompts de la base de datos
            var promptSistema = await promptService.ObtenerPromptPorNombreAsync("VAPISistema", 1);
            var promptUsuarioTemplate = await promptService.ObtenerPromptPorNombreAsync("VAPIUsuario", 1);

            // Validar que los prompts existan - NO usar fallbacks hardcodeados
            if (string.IsNullOrEmpty(promptSistema))
            {
                logger.LogError("Prompt VAPISistema no encontrado en BD - Sistema requiere configuración");
                throw new InvalidOperationException(
                    "Prompt 'VAPISistema' no encontrado en conf_ticket_prompts. " +
                    "Ejecute el script insert-prompts-iniciales.sql para configurar los prompts.");
            }

            if (string.IsNullOrEmpty(promptUsuarioTemplate))
            {
                logger.LogError("Prompt VAPIUsuario no encontrado en BD - Sistema requiere configuración");
                throw new InvalidOperationException(
                    "Prompt 'VAPIUsuario' no encontrado en conf_ticket_prompts. " +
                    "Ejecute el script insert-prompts-iniciales.sql para configurar los prompts.");
            }

            // Reemplazar variables en el prompt de usuario
            var promptUsuario = promptUsuarioTemplate
                .Replace("{PhoneNumber}", request.PhoneNumber)
                .Replace("{DurationSeconds}", request.DurationSeconds.ToString())
                .Replace("{Status}", request.Status)
                .Replace("{DisconnectReason}", request.DisconnectReason != null ? $"Razón de desconexión: {request.DisconnectReason}" : "")
                .Replace("{Transcription}", request.Transcription);

            // 3. Procesar con Azure OpenAI
            var respuestaIA = await openAIService.GenerarRespuestaAsync(
                promptUsuario, promptSistema, temperature: 0.3);

            logger.LogInformation("Respuesta de IA generada para CallId: {CallId}", request.CallId);

            // 3. Determinar tipo de ticket
            var tipoTicket = request.Status == "disconnected" || request.DurationSeconds < 30
                ? "LlamadaCortada"
                : "Accion";

            // 4. Crear ticket
            var ticketId = await CrearTicketVAPI(db, request, respuestaIA, tipoTicket, 1);

            logger.LogInformation(
                "Ticket #{TicketId} creado para llamada {CallId}",
                ticketId, request.CallId);

            // 5. Registrar interacción
            await RegistrarInteraccion(db, ticketId, "VAPI", "LlamadaRecibida",
                request.CallId, request, request.DurationSeconds, true, null, 1);

            // Fase 6: Si esta llamada proviene del sistema de monitoreo,
            // actualizar log_monitoreo_sesiones con la transcripcion y resultado.
            // Identificamos llamadas de monitoreo por el vapi_call_id en la tabla.
            try
            {
                await ActualizarSesionMonitoreo(db, request, logger);
            }
            catch (Exception monEx)
            {
                logger.LogWarning(monEx,
                    "No se pudo actualizar sesion de monitoreo para CallId: {CallId} (no critico)",
                    request.CallId);
            }

            return Results.Ok(new WebhookResponse
            {
                Success = true,
                TicketId = ticketId,
                Mensaje = "Ticket creado exitosamente",
                RespuestaIA = respuestaIA
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error procesando webhook VAPI - CallId: {CallId}", request.CallId);

            return Results.Json(
                new WebhookErrorResponse
                {
                    Success = false,
                    ErrorCode = "VAPI_ERROR",
                    ErrorMessage = "Error procesando llamada telefónica"
                },
                statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    /// <summary>
    /// Procesa webhook de YCloud (WhatsApp Business)
    /// </summary>
    private static async Task<IResult> ProcesarWebhookYCloud(
        [FromBody] YCloudWebhookRequest request,
        [FromServices] ITicketValidationService validationService,
        [FromServices] IOpenAIService openAIService,
        [FromServices] IPromptTuningService promptService,
        [FromServices] IYCloudService yCloudService,
        [FromServices] IDatabaseService db,
        [FromServices] ILogger<Program> logger)
    {
        try
        {
            logger.LogInformation(
                "Webhook YCloud recibido - MessageId: {MessageId}, De: {From}",
                request.MessageId, request.From);

            // 1. Validar cliente duplicado por teléfono
            var validationRequest = new TicketValidationRequest
            {
                Telefono = request.From,
                Email = null,
                IPOrigen = null,
                IdEntidad = 1
            };
            var validation = await validationService.ValidarClienteDuplicadoAsync(validationRequest);

            if (validation.TieneTicketAbierto)
            {
                logger.LogWarning(
                    "Cliente {Phone} ya tiene ticket abierto #{TicketId}",
                    request.From, validation.IdTicketAbierto);

                // Enviar respuesta de ticket existente
                var mensajeExistente = $"Hola, ya tienes un ticket abierto (#{validation.IdTicketAbierto}). " +
                    "Un agente te atenderá pronto. Si es urgente, por favor llama al soporte.";

                var sendRequest = new YCloudSendMessageRequest
                {
                    To = request.From,
                    Text = mensajeExistente
                };
                await yCloudService.EnviarMensajeAsync(sendRequest);

                return Results.Ok(new WebhookResponse
                {
                    Success = true,
                    TicketId = validation.IdTicketAbierto,
                    Mensaje = mensajeExistente,
                    RespuestaIA = null
                });
            }

            // 2. Obtener prompts de la base de datos
            var promptSistema = await promptService.ObtenerPromptPorNombreAsync("YCloudSistema", 1);
            var promptUsuarioTemplate = await promptService.ObtenerPromptPorNombreAsync("YCloudUsuario", 1);

            // Validar que los prompts existan - NO usar fallbacks hardcodeados
            if (string.IsNullOrEmpty(promptSistema))
            {
                logger.LogError("Prompt YCloudSistema no encontrado en BD - Sistema requiere configuración");
                throw new InvalidOperationException(
                    "Prompt 'YCloudSistema' no encontrado en conf_ticket_prompts. " +
                    "Ejecute el script insert-prompts-iniciales.sql para configurar los prompts.");
            }

            if (string.IsNullOrEmpty(promptUsuarioTemplate))
            {
                logger.LogError("Prompt YCloudUsuario no encontrado en BD - Sistema requiere configuración");
                throw new InvalidOperationException(
                    "Prompt 'YCloudUsuario' no encontrado en conf_ticket_prompts. " +
                    "Ejecute el script insert-prompts-iniciales.sql para configurar los prompts.");
            }

            // Reemplazar variables en el prompt de usuario
            var promptUsuario = promptUsuarioTemplate
                .Replace("{From}", request.From)
                .Replace("{Text}", request.Text);

            // 3. Procesar con Azure OpenAI
            var respuestaIA = await openAIService.GenerarRespuestaAsync(
                promptUsuario, promptSistema, temperature: 0.5);

            logger.LogInformation("Respuesta de IA generada para MessageId: {MessageId}", request.MessageId);

            // 3. Crear ticket
            var ticketId = await CrearTicketYCloud(db, request, respuestaIA, 1);

            logger.LogInformation(
                "Ticket #{TicketId} creado para mensaje WhatsApp {MessageId}",
                ticketId, request.MessageId);

            // 4. Registrar interacción
            await RegistrarInteraccion(db, ticketId, "YCloud", "MensajeRecibido",
                request.MessageId, request, null, true, null, 1);

            // 5. Enviar respuesta automática
            var mensajeRespuesta = $"Hola, hemos recibido tu mensaje y creado el ticket #{ticketId}. " +
                $"{respuestaIA.Substring(0, Math.Min(100, respuestaIA.Length))}... " +
                "Un agente te atenderá pronto.";

            var sendResponseRequest = new YCloudSendMessageRequest
            {
                To = request.From,
                Text = mensajeRespuesta
            };
            await yCloudService.EnviarMensajeAsync(sendResponseRequest);

            return Results.Ok(new WebhookResponse
            {
                Success = true,
                TicketId = ticketId,
                Mensaje = "Ticket creado y respuesta enviada",
                RespuestaIA = respuestaIA
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error procesando webhook YCloud - MessageId: {MessageId}", request.MessageId);

            return Results.Json(
                new WebhookErrorResponse
                {
                    Success = false,
                    ErrorCode = "YCLOUD_ERROR",
                    ErrorMessage = "Error procesando mensaje de WhatsApp"
                },
                statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    /// <summary>
    /// Procesa webhook de Chat Web (widget)
    /// </summary>
    private static async Task<IResult> ProcesarWebhookChatWeb(
        [FromBody] ChatWebRequest request,
        [FromServices] ITicketValidationService validationService,
        [FromServices] IOpenAIService openAIService,
        [FromServices] IPromptTuningService promptService,
        [FromServices] IDatabaseService db,
        [FromServices] IConfiguration configuration,
        [FromServices] ILogger<Program> logger)
    {
        try
        {
            logger.LogInformation(
                "Mensaje Chat Web recibido - Email: {Email}, IP: {IP}",
                request.Email, request.IPOrigen);

            // 1. Validar cliente duplicado por email e IP (configurable)
            var validarDuplicados = configuration.GetValue<bool>("TicketValidation:ValidarClientesDuplicados", true);
            
            if (validarDuplicados)
            {
                logger.LogInformation("Paso 1: Validando cliente duplicado...");
                var validationRequest = new TicketValidationRequest
                {
                    Telefono = null,
                    Email = request.Email,
                    IPOrigen = request.IPOrigen,
                    IdEntidad = request.IdEntidad
                };
                var validation = await validationService.ValidarClienteDuplicadoAsync(validationRequest);
                logger.LogInformation("Paso 1: Validación completada. TieneTicketAbierto: {TieneTicket}", validation.TieneTicketAbierto);

                if (validation.TieneTicketAbierto)
                {
                    logger.LogWarning(
                        "Cliente {Email} ya tiene ticket abierto #{TicketId}",
                        request.Email, validation.IdTicketAbierto);

                    return Results.Ok(new ChatWebResponse
                    {
                        Success = true,
                        TicketId = validation.IdTicketAbierto,
                        Mensaje = $"Ya tienes un ticket abierto (#{validation.IdTicketAbierto}). Un agente te atenderá pronto.",
                        RespuestaIA = null,
                        SessionId = request.SessionId
                    });
                }
            }
            else
            {
                logger.LogWarning("Paso 1: Validación de duplicados DESHABILITADA por configuración");
            }

            // 2. Obtener prompts de la base de datos
            logger.LogInformation("Paso 2: Obteniendo prompts de la base de datos...");
            var promptSistema = await promptService.ObtenerPromptPorNombreAsync("ChatWebSistema", request.IdEntidad);
            var promptUsuarioTemplate = await promptService.ObtenerPromptPorNombreAsync("ChatWebUsuario", request.IdEntidad);

            // Validar que los prompts existan - NO usar fallbacks hardcodeados
            if (string.IsNullOrEmpty(promptSistema))
            {
                logger.LogError("Prompt ChatWebSistema no encontrado en BD - Sistema requiere configuración");
                throw new InvalidOperationException(
                    "Prompt 'ChatWebSistema' no encontrado en conf_ticket_prompts. " +
                    "Ejecute el script insert-prompts-iniciales.sql para configurar los prompts.");
            }

            if (string.IsNullOrEmpty(promptUsuarioTemplate))
            {
                logger.LogError("Prompt ChatWebUsuario no encontrado en BD - Sistema requiere configuración");
                throw new InvalidOperationException(
                    "Prompt 'ChatWebUsuario' no encontrado en conf_ticket_prompts. " +
                    "Ejecute el script insert-prompts-iniciales.sql para configurar los prompts.");
            }

            // Reemplazar variables en el prompt de usuario
            var promptUsuario = promptUsuarioTemplate
                .Replace("{Nombre}", request.Nombre)
                .Replace("{Email}", request.Email)
                .Replace("{Mensaje}", request.Mensaje);

            // 3. Procesar con Azure OpenAI
            logger.LogInformation("Paso 3: Llamando a Azure OpenAI...");
            var respuestaIA = await openAIService.GenerarRespuestaAsync(
                promptUsuario, promptSistema, temperature: 0.7);

            logger.LogInformation("Paso 3: Respuesta de IA generada para email: {Email}", request.Email);

            // 4. Crear ticket
            logger.LogInformation("Paso 4: Creando ticket en base de datos...");
            var ticketId = await CrearTicketChatWeb(db, request, respuestaIA, request.IdEntidad);

            logger.LogInformation(
                "Paso 4: Ticket #{TicketId} creado para chat web de {Email}",
                ticketId, request.Email);

            // 5. Registrar interacción
            logger.LogInformation("Paso 5: Registrando interacción...");
            await RegistrarInteraccion(db, ticketId, "ChatWeb", "MensajeRecibido",
                request.SessionId, request, null, true, null, request.IdEntidad);
            logger.LogInformation("Paso 5: Interacción registrada exitosamente");

            // 6. Guardar mensaje del cliente en conversación
            logger.LogInformation("Paso 6: Guardando mensaje del cliente en conversación...");
            await GuardarMensajeConversacion(db, ticketId, "Cliente", request.Mensaje, 
                false, null, request.Nombre);
            logger.LogInformation("Paso 6: Mensaje del cliente guardado");

            // 7. Guardar respuesta de IA en conversación
            logger.LogInformation("Paso 7: Guardando respuesta de IA en conversación...");
            await GuardarMensajeConversacion(db, ticketId, "IA", respuestaIA, 
                true, null, "Asistente JELA");
            logger.LogInformation("Paso 7: Respuesta de IA guardada");

            return Results.Ok(new ChatWebResponse
            {
                Success = true,
                TicketId = ticketId,
                Mensaje = $"Ticket #{ticketId} creado exitosamente",
                RespuestaIA = respuestaIA,
                SessionId = request.SessionId
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "ERROR DETALLADO en chat web - Email: {Email}, Mensaje: {Message}, StackTrace: {StackTrace}", 
                request.Email, ex.Message, ex.StackTrace);

            return Results.Json(
                new WebhookErrorResponse
                {
                    Success = false,
                    ErrorCode = "CHATWEB_ERROR",
                    ErrorMessage = $"Error procesando mensaje de chat: {ex.Message}"
                },
                statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    /// <summary>
    /// Procesa webhook de Firebase (app móvil)
    /// </summary>
    private static async Task<IResult> ProcesarWebhookFirebase(
        [FromBody] FirebaseWebhookRequest request,
        [FromServices] ITicketValidationService validationService,
        [FromServices] IOpenAIService openAIService,
        [FromServices] IPromptTuningService promptService,
        [FromServices] IDatabaseService db,
        [FromServices] ILogger<Program> logger)
    {
        try
        {
            logger.LogInformation(
                "Webhook Firebase recibido - UserId: {UserId}, DeviceId: {DeviceId}",
                request.UserId, request.DeviceId);

            // 1. Validar cliente duplicado por userId (usar como teléfono temporalmente)
            var validationRequest = new TicketValidationRequest
            {
                Telefono = request.UserId,
                Email = null,
                IPOrigen = null,
                IdEntidad = 1
            };
            var validation = await validationService.ValidarClienteDuplicadoAsync(validationRequest);

            if (validation.TieneTicketAbierto)
            {
                logger.LogWarning(
                    "Usuario {UserId} ya tiene ticket abierto #{TicketId}",
                    request.UserId, validation.IdTicketAbierto);

                // TODO: Enviar notificación push de ticket existente
                return Results.Ok(new WebhookResponse
                {
                    Success = true,
                    TicketId = validation.IdTicketAbierto,
                    Mensaje = $"Ya tienes un ticket abierto (#{validation.IdTicketAbierto})",
                    RespuestaIA = null
                });
            }

            // 2. Obtener prompts de la base de datos
            var promptSistema = await promptService.ObtenerPromptPorNombreAsync("FirebaseSistema", 1);
            var promptUsuarioTemplate = await promptService.ObtenerPromptPorNombreAsync("FirebaseUsuario", 1);

            // Validar que los prompts existan - NO usar fallbacks hardcodeados
            if (string.IsNullOrEmpty(promptSistema))
            {
                logger.LogError("Prompt FirebaseSistema no encontrado en BD - Sistema requiere configuración");
                throw new InvalidOperationException(
                    "Prompt 'FirebaseSistema' no encontrado en conf_ticket_prompts. " +
                    "Ejecute el script insert-prompts-iniciales.sql para configurar los prompts.");
            }

            if (string.IsNullOrEmpty(promptUsuarioTemplate))
            {
                logger.LogError("Prompt FirebaseUsuario no encontrado en BD - Sistema requiere configuración");
                throw new InvalidOperationException(
                    "Prompt 'FirebaseUsuario' no encontrado en conf_ticket_prompts. " +
                    "Ejecute el script insert-prompts-iniciales.sql para configurar los prompts.");
            }

            // Reemplazar variables en el prompt de usuario
            var promptUsuario = promptUsuarioTemplate
                .Replace("{UserId}", request.UserId)
                .Replace("{Message}", request.Message);

            // 3. Procesar con Azure OpenAI
            var respuestaIA = await openAIService.GenerarRespuestaAsync(
                promptUsuario, promptSistema, temperature: 0.7);

            logger.LogInformation("Respuesta de IA generada para UserId: {UserId}", request.UserId);

            // 3. Crear ticket
            var ticketId = await CrearTicketFirebase(db, request, respuestaIA, 1);

            logger.LogInformation(
                "Ticket #{TicketId} creado para usuario Firebase {UserId}",
                ticketId, request.UserId);

            // 4. Registrar interacción
            await RegistrarInteraccion(db, ticketId, "Firebase", "MensajeRecibido",
                request.DeviceId, request, null, true, null, 1);

            // TODO: Enviar notificación push con respuesta
            // await firebaseService.EnviarNotificacionPush(request.DeviceToken, respuestaIA);

            return Results.Ok(new WebhookResponse
            {
                Success = true,
                TicketId = ticketId,
                Mensaje = "Ticket creado exitosamente",
                RespuestaIA = respuestaIA
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error procesando webhook Firebase - UserId: {UserId}", request.UserId);

            return Results.Json(
                new WebhookErrorResponse
                {
                    Success = false,
                    ErrorCode = "FIREBASE_ERROR",
                    ErrorMessage = "Error procesando mensaje de app móvil"
                },
                statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    #endregion

    #region Métodos Auxiliares

    /// <summary>
    /// Actualiza log_monitoreo_sesiones si el CallId corresponde a una sesion de monitoreo.
    /// Busca por vapi_call_id; si no existe, ignora silenciosamente (no es de monitoreo).
    /// </summary>
    private static async Task ActualizarSesionMonitoreo(
        IDatabaseService db,
        VapiWebhookRequest request,
        ILogger<Program> logger)
    {
        if (string.IsNullOrEmpty(request.CallId)) return;

        var queryBuscar = $"SELECT id FROM log_monitoreo_sesiones WHERE vapi_call_id = '{request.CallId}' LIMIT 1";
        var resultado = await db.EjecutarConsultaAsync(queryBuscar);

        var primerRegistro = resultado?.FirstOrDefault();
        if (primerRegistro == null) return;

        var sesionId = Convert.ToInt32(primerRegistro["id"]);

        var estado = request.Status == "completed" || request.DurationSeconds > 10
            ? "completada" : "fallida";

        var transcripcion = request.Transcription?.Replace("'", "''") ?? "";
        var resumen = transcripcion.Length > 200
            ? transcripcion.Substring(0, 200) + "..."
            : transcripcion;

        var metadataJson = JsonSerializer.Serialize(new { request.Status, request.DisconnectReason })
            .Replace("'", "''");

        var queryUpdate = $@"UPDATE log_monitoreo_sesiones SET 
            estado = '{estado}',
            transcripcion_completa = '{transcripcion}',
            resumen = '{resumen}',
            duracion_segundos = {request.DurationSeconds},
            fin_llamada = NOW(),
            metadata = '{metadataJson}',
            actualizado_en = NOW()
            WHERE id = {sesionId}";

        await db.EjecutarNoConsultaAsync(queryUpdate);

        logger.LogInformation(
            "Sesion de monitoreo #{SesionId} actualizada a '{Estado}' para CallId: {CallId}",
            sesionId, estado, request.CallId);
    }

    /// <summary>
    /// Crea un ticket para llamada de VAPI
    /// </summary>
    private static async Task<int> CrearTicketVAPI(
        IDatabaseService db,
        VapiWebhookRequest request,
        string respuestaIA,
        string tipoTicket,
        int idEntidad)
    {
        var asunto = $"Llamada telefónica - {request.PhoneNumber}";
        var momentoCorte = request.DisconnectReason ?? request.Status;

        var campos = new Dictionary<string, object>
        {
            { "IdEntidad", idEntidad },
            { "AsuntoCorto", asunto },
            { "MensajeOriginal", request.Transcription },
            { "Canal", "VAPI" },
            { "TelefonoCliente", request.PhoneNumber },
            { "TipoTicket", tipoTicket },
            { "DuracionLlamadaSegundos", request.DurationSeconds },
            { "MomentoCorte", momentoCorte },
            { "RespuestaIA", respuestaIA },
            { "Estado", "Abierto" },
            { "IdUsuarioCreacion", 1 },
            { "FechaCreacion", DateTime.Now }
        };

        var ticketId = await db.InsertarAsync("op_tickets_v2", campos);
        return ticketId;
    }

    /// <summary>
    /// Crea un ticket para mensaje de YCloud (WhatsApp)
    /// </summary>
    private static async Task<int> CrearTicketYCloud(
        IDatabaseService db,
        YCloudWebhookRequest request,
        string respuestaIA,
        int idEntidad)
    {
        var asunto = $"WhatsApp - {request.From}";

        var campos = new Dictionary<string, object>
        {
            { "IdEntidad", idEntidad },
            { "AsuntoCorto", asunto },
            { "MensajeOriginal", request.Text },
            { "Canal", "WhatsApp" },
            { "TelefonoCliente", request.From },
            { "TipoTicket", "WhatsApp" },
            { "RespuestaIA", respuestaIA },
            { "Estado", "Abierto" },
            { "IdUsuarioCreacion", 1 },
            { "FechaCreacion", DateTime.Now }
        };

        var ticketId = await db.InsertarAsync("op_tickets_v2", campos);
        return ticketId;
    }

    /// <summary>
    /// Crea un ticket para mensaje de Chat Web
    /// </summary>
    private static async Task<int> CrearTicketChatWeb(
        IDatabaseService db,
        ChatWebRequest request,
        string respuestaIA,
        int idEntidad)
    {
        var asunto = $"Chat Web - {request.Nombre}";

        var campos = new Dictionary<string, object>
        {
            { "IdEntidad", idEntidad },
            { "AsuntoCorto", asunto },
            { "MensajeOriginal", request.Mensaje },
            { "Canal", "ChatWeb" },
            { "NombreCompleto", request.Nombre },
            { "EmailCliente", request.Email },
            { "IPOrigen", request.IPOrigen },
            { "TipoTicket", "ChatWeb" },
            { "RespuestaIA", respuestaIA },
            { "Estado", "Abierto" },
            { "IdUsuarioCreacion", 1 },
            { "FechaCreacion", DateTime.Now }
        };

        var ticketId = await db.InsertarAsync("op_tickets_v2", campos);
        return ticketId;
    }

    /// <summary>
    /// Crea un ticket para mensaje de Firebase (app móvil)
    /// </summary>
    private static async Task<int> CrearTicketFirebase(
        IDatabaseService db,
        FirebaseWebhookRequest request,
        string respuestaIA,
        int idEntidad)
    {
        var asunto = $"App Móvil - {request.UserId}";

        var campos = new Dictionary<string, object>
        {
            { "IdEntidad", idEntidad },
            { "AsuntoCorto", asunto },
            { "MensajeOriginal", request.Message },
            { "Canal", "ChatApp" },
            { "TelefonoCliente", request.UserId },
            { "TipoTicket", "ChatApp" },
            { "RespuestaIA", respuestaIA },
            { "Estado", "Abierto" },
            { "IdUsuarioCreacion", 1 },
            { "FechaCreacion", DateTime.Now }
        };

        var ticketId = await db.InsertarAsync("op_tickets_v2", campos);
        return ticketId;
    }

    /// <summary>
    /// Registra una interacción en op_ticket_logs_interacciones
    /// </summary>
    private static async Task RegistrarInteraccion(
        IDatabaseService db,
        int ticketId,
        string canal,
        string tipoInteraccion,
        string? idExternoCanal,
        object datosInteraccion,
        int? duracion,
        bool exitosa,
        string? mensajeError,
        int idEntidad)
    {
        var datosJson = JsonSerializer.Serialize(datosInteraccion);

        var campos = new Dictionary<string, object>
        {
            { "IdEntidad", idEntidad },
            { "IdTicket", ticketId },
            { "Canal", canal },
            { "TipoInteraccion", tipoInteraccion },
            { "IdExternoCanal", idExternoCanal ?? (object)DBNull.Value },
            { "DatosInteraccion", datosJson },
            { "Duracion", duracion ?? (object)DBNull.Value },
            { "Exitosa", exitosa },
            { "MensajeError", mensajeError ?? (object)DBNull.Value },
            { "IdUsuarioCreacion", 1 },
            { "FechaCreacion", DateTime.Now }
        };

        await db.InsertarAsync("op_ticket_logs_interacciones", campos);
    }

    /// <summary>
    /// Guarda un mensaje en la tabla op_ticket_conversacion
    /// </summary>
    private static async Task GuardarMensajeConversacion(
        IDatabaseService db,
        int ticketId,
        string tipoMensaje,
        string mensaje,
        bool esRespuestaIA,
        int? idUsuarioEnvio,
        string? nombreUsuarioEnvio)
    {
        var campos = new Dictionary<string, object>
        {
            { "IdTicket", ticketId },
            { "TipoMensaje", tipoMensaje },
            { "Mensaje", mensaje },
            { "EsRespuestaIA", esRespuestaIA },
            { "IdUsuarioEnvio", idUsuarioEnvio ?? (object)DBNull.Value },
            { "NombreUsuarioEnvio", nombreUsuarioEnvio ?? (object)DBNull.Value },
            { "FechaEnvio", DateTime.Now },
            { "Leido", 0 }
        };

        await db.InsertarAsync("op_ticket_conversacion", campos);
    }

    #endregion
}
