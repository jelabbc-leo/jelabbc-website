namespace JELA.API.Models;

// ============================================================================
// WEBHOOK MODELS - Estructuras de APIs externas
// ============================================================================
// Modelos tipados para webhooks de servicios externos (VAPI, YCloud, Telegram, Firebase)
// Estas estructuras son fijas y definidas por terceros
// ============================================================================

#region VAPI (Llamadas Telefónicas)

/// <summary>
/// Webhook de VAPI para llamadas telefónicas
/// </summary>
public record VapiWebhookRequest(
    string CallId,
    string PhoneNumber,
    string Transcription,
    int DurationSeconds,
    string Status,
    string? DisconnectReason,
    DateTime? StartTime,
    DateTime? EndTime,
    Dictionary<string, object>? Metadata
);

/// <summary>
/// Evento de VAPI
/// </summary>
public record VapiEventRequest(
    string EventType,
    string CallId,
    DateTime Timestamp,
    Dictionary<string, object>? Data
);

#endregion

#region YCloud (WhatsApp Business)

/// <summary>
/// Webhook de YCloud para WhatsApp
/// </summary>
public record YCloudWebhookRequest(
    string MessageId,
    string From,
    string To,
    string Text,
    DateTime Timestamp,
    string Type,
    YCloudMessageStatus? Status,
    Dictionary<string, object>? Metadata
);

/// <summary>
/// Estado del mensaje de YCloud
/// </summary>
public record YCloudMessageStatus(
    string Status,
    DateTime? DeliveredAt,
    DateTime? ReadAt,
    string? ErrorCode,
    string? ErrorMessage
);

/// <summary>
/// Request para enviar mensaje por YCloud
/// </summary>
public class YCloudSendMessageRequest
{
    public string To { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public string? TemplateId { get; set; }
    public Dictionary<string, string>? TemplateParams { get; set; }
}

/// <summary>
/// Respuesta de YCloud al enviar mensaje
/// </summary>
public record YCloudSendMessageResponse(
    bool Success,
    string? MessageId,
    string? ErrorCode,
    string? ErrorMessage
);

#endregion

#region Telegram Bot

/// <summary>
/// Webhook de Telegram Bot
/// </summary>
public record TelegramWebhookRequest(
    long UpdateId,
    TelegramMessage? Message,
    TelegramCallbackQuery? CallbackQuery
);

/// <summary>
/// Mensaje de Telegram
/// </summary>
public record TelegramMessage(
    long MessageId,
    TelegramUser From,
    TelegramChat Chat,
    long Date,
    string? Text,
    TelegramUser? ForwardFrom,
    TelegramMessage? ReplyToMessage
);

/// <summary>
/// Usuario de Telegram
/// </summary>
public record TelegramUser(
    long Id,
    bool IsBot,
    string FirstName,
    string? LastName,
    string? Username,
    string? LanguageCode
);

/// <summary>
/// Chat de Telegram
/// </summary>
public record TelegramChat(
    long Id,
    string Type,
    string? Title,
    string? FirstName,
    string? LastName,
    string? Username
);

/// <summary>
/// Callback query de Telegram (botones inline)
/// </summary>
public record TelegramCallbackQuery(
    string Id,
    TelegramUser From,
    TelegramMessage? Message,
    string? Data
);

/// <summary>
/// Request para enviar mensaje por Telegram
/// </summary>
public class TelegramSendMessageRequest
{
    public long ChatId { get; set; }
    public string Text { get; set; } = string.Empty;
    public string? ParseMode { get; set; }
    public bool? DisableNotification { get; set; }
    public TelegramReplyMarkup? ReplyMarkup { get; set; }
}

/// <summary>
/// Markup para respuestas de Telegram (teclados)
/// </summary>
public class TelegramReplyMarkup
{
    public List<List<TelegramInlineButton>>? InlineKeyboard { get; set; }
}

/// <summary>
/// Botón inline de Telegram
/// </summary>
public record TelegramInlineButton(
    string Text,
    string? CallbackData,
    string? Url
);

#endregion

#region Chat Web (Widget)

/// <summary>
/// Request de Chat Web desde widget
/// </summary>
public class ChatWebRequest
{
    public string Email { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Mensaje { get; set; } = string.Empty;
    public string IPOrigen { get; set; } = string.Empty;
    public int IdEntidad { get; set; }
    public string? SessionId { get; set; }
    public Dictionary<string, string>? Metadata { get; set; }
}

/// <summary>
/// Respuesta para Chat Web
/// </summary>
public class ChatWebResponse
{
    public bool Success { get; set; }
    public int? TicketId { get; set; }
    public string? RespuestaIA { get; set; }
    public string? Mensaje { get; set; }
    public string? SessionId { get; set; }
}

#endregion

#region Firebase (Chat App Móvil)

/// <summary>
/// Webhook de Firebase para Chat App
/// </summary>
public record FirebaseWebhookRequest(
    string UserId,
    string Message,
    DateTime Timestamp,
    string DeviceId,
    string? DeviceToken,
    Dictionary<string, object>? Metadata
);

/// <summary>
/// Request para enviar notificación push por Firebase
/// </summary>
public class FirebasePushNotificationRequest
{
    public string DeviceToken { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public Dictionary<string, string>? Data { get; set; }
}

/// <summary>
/// Respuesta de Firebase
/// </summary>
public record FirebasePushNotificationResponse(
    bool Success,
    string? MessageId,
    string? ErrorCode,
    string? ErrorMessage
);

#endregion

#region Respuestas Comunes

/// <summary>
/// Respuesta estándar para webhooks
/// </summary>
public class WebhookResponse
{
    public bool Success { get; set; }
    public int? TicketId { get; set; }
    public string? Mensaje { get; set; }
    public string? RespuestaIA { get; set; }
    public Dictionary<string, object>? Metadata { get; set; }
}

/// <summary>
/// Respuesta de error para webhooks
/// </summary>
public class WebhookErrorResponse
{
    public bool Success { get; set; } = false;
    public string ErrorCode { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

#endregion
