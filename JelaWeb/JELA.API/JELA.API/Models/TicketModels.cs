namespace JELA.API.Models;

// ============================================================================
// TICKET MODELS - Módulo de Tickets Colaborativos con IA
// ============================================================================
// NOTA: Este proyecto usa DTOs dinámicos (CrudDto) para operaciones CRUD.
// Estos modelos son solo para casos específicos donde se requiere tipado fuerte.
// ============================================================================

/// <summary>
/// Request para validar cliente duplicado
/// </summary>
public class TicketValidationRequest
{
    public string? Telefono { get; set; }
    public string? Email { get; set; }
    public string? IPOrigen { get; set; }
    public int IdEntidad { get; set; }
}

/// <summary>
/// Respuesta de validación de cliente
/// </summary>
public class TicketValidationResponse
{
    public bool TieneTicketAbierto { get; set; }
    public int? IdTicketAbierto { get; set; }
    public int NumeroTicketsHistoricos { get; set; }
    public DateTime? UltimaInteraccion { get; set; }
    public bool Bloqueado { get; set; }
    public string? MotivoBloqueo { get; set; }
}

/// <summary>
/// Request para encolar notificación WhatsApp
/// </summary>
public class NotificacionWhatsAppRequest
{
    public int IdTicket { get; set; }
    public string NumeroWhatsApp { get; set; } = string.Empty;
    public string TipoNotificacion { get; set; } = string.Empty;
    public string MensajeTexto { get; set; } = string.Empty;
    public string? PlantillaId { get; set; }
    public Dictionary<string, string>? ParametrosPlantilla { get; set; }
    public int IdEntidad { get; set; }
}

/// <summary>
/// Respuesta de notificación encolada
/// </summary>
public class NotificacionWhatsAppResponse
{
    public bool Success { get; set; }
    public int? IdNotificacion { get; set; }
    public string? Mensaje { get; set; }
}

/// <summary>
/// DTO para métricas de tickets en tiempo real
/// </summary>
public class TicketMetricsDto
{
    public int TotalTicketsCreados { get; set; }
    public int TotalTicketsResueltos { get; set; }
    public int TotalTicketsResueltosIA { get; set; }
    public decimal PorcentajeResolucionIA { get; set; }
    public decimal? TiempoPromedioResolucionMinutos { get; set; }
    public decimal? TiempoPromedioRespuestaMinutos { get; set; }
    public decimal? CSATPromedio { get; set; }
    public int TotalLlamadasVAPI { get; set; }
    public int TotalMensajesWhatsApp { get; set; }
    public int TotalChatWeb { get; set; }
    public int TotalChatApp { get; set; }
    public int TokensIAUtilizados { get; set; }
    public decimal CostoEstimadoIA { get; set; }
}

/// <summary>
/// Request para calcular métricas diarias
/// </summary>
public class CalcularMetricasRequest
{
    public DateTime Fecha { get; set; }
    public int IdEntidad { get; set; }
    public string? Canal { get; set; }
}

/// <summary>
/// DTO para log de interacción multicanal
/// </summary>
public class LogInteraccionDto
{
    public int IdTicket { get; set; }
    public string Canal { get; set; } = string.Empty;
    public string TipoInteraccion { get; set; } = string.Empty;
    public string? IdExternoCanal { get; set; }
    public Dictionary<string, object>? DatosInteraccion { get; set; }
    public int? Duracion { get; set; }
    public bool Exitosa { get; set; }
    public string? MensajeError { get; set; }
    public int IdEntidad { get; set; }
}

/// <summary>
/// DTO para log de prompt de IA
/// </summary>
public class LogPromptDto
{
    public int? IdTicket { get; set; }
    public int? IdPrompt { get; set; }
    public string PromptEnviado { get; set; } = string.Empty;
    public string? RespuestaIA { get; set; }
    public string? ModeloUtilizado { get; set; }
    public int? TokensUtilizados { get; set; }
    public int? TiempoRespuestaMs { get; set; }
    public bool Exitoso { get; set; }
    public string? MensajeError { get; set; }
    public int IdEntidad { get; set; }
}

/// <summary>
/// Request para validación de cliente Telegram
/// </summary>
public class TelegramValidationRequest
{
    public long ChatId { get; set; }
    public string? Username { get; set; }
    public int IdEntidad { get; set; }
}

/// <summary>
/// Respuesta de validación Telegram (4 niveles)
/// </summary>
public class TelegramValidationResponse
{
    public bool Valido { get; set; }
    public int NivelAlcanzado { get; set; }
    public string? RazonRechazo { get; set; }
    public TelegramClienteInfo? ClienteInfo { get; set; }
}

/// <summary>
/// Información del cliente Telegram
/// </summary>
public class TelegramClienteInfo
{
    public long ChatId { get; set; }
    public string? Username { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string EstadoCliente { get; set; } = string.Empty;
    public string TipoCliente { get; set; } = string.Empty;
    public DateTime? FechaVencimiento { get; set; }
    public int CreditosDisponibles { get; set; }
    public int TicketsMesActual { get; set; }
    public int LimiteTicketsMes { get; set; }
}

/// <summary>
/// Request para ajuste de prompt
/// </summary>
public class PromptAjusteRequest
{
    public int IdPrompt { get; set; }
    public string VersionAnterior { get; set; } = string.Empty;
    public string VersionNueva { get; set; } = string.Empty;
    public string? MotivoAjuste { get; set; }
    public Dictionary<string, object>? MetricasAntes { get; set; }
    public Dictionary<string, object>? MetricasDespues { get; set; }
    public decimal? PorcentajeMejora { get; set; }
    public int IdEntidad { get; set; }
}

/// <summary>
/// Respuesta de ajuste de prompt
/// </summary>
public class PromptAjusteResponse
{
    public bool Success { get; set; }
    public int? IdAjuste { get; set; }
    public bool RequiereAprobacion { get; set; }
    public string? Mensaje { get; set; }
}
