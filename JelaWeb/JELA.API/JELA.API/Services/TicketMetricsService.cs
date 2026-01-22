using JELA.API.Models;
using System.Text.Json;

namespace JELA.API.Services;

/// <summary>
/// Servicio para gestión de métricas de tickets
/// </summary>
public class TicketMetricsService : ITicketMetricsService
{
    private readonly IDatabaseService _db;
    private readonly ILogger<TicketMetricsService> _logger;

    public TicketMetricsService(
        IDatabaseService databaseService,
        ILogger<TicketMetricsService> logger)
    {
        _db = databaseService;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene métricas de tickets en tiempo real
    /// </summary>
    public async Task<TicketMetricsDto> ObtenerMetricasTiempoRealAsync(int idEntidad, DateTime fechaInicio, DateTime fechaFin)
    {
        try
        {
            _logger.LogInformation(
                "Obteniendo métricas en tiempo real - Entidad: {IdEntidad}, Periodo: {Inicio} - {Fin}",
                idEntidad, fechaInicio, fechaFin);

            var query = @"
                SELECT 
                    COUNT(*) AS TotalTicketsCreados,
                    SUM(CASE WHEN Estado IN ('Cerrado', 'Resuelto') THEN 1 ELSE 0 END) AS TotalTicketsResueltos,
                    SUM(CASE WHEN ResueltoporIA = 1 THEN 1 ELSE 0 END) AS TotalTicketsResueltosIA,
                    ROUND(
                        (SUM(CASE WHEN ResueltoporIA = 1 THEN 1 ELSE 0 END) * 100.0) / 
                        NULLIF(SUM(CASE WHEN Estado IN ('Cerrado', 'Resuelto') THEN 1 ELSE 0 END), 0), 
                        2
                    ) AS PorcentajeResolucionIA,
                    AVG(CASE 
                        WHEN Estado IN ('Cerrado', 'Resuelto') AND FechaResolucion IS NOT NULL 
                        THEN TIMESTAMPDIFF(MINUTE, FechaCreacion, FechaResolucion) 
                    END) AS TiempoPromedioResolucionMinutos,
                    AVG(CASE 
                        WHEN FechaPrimeraRespuesta IS NOT NULL 
                        THEN TIMESTAMPDIFF(MINUTE, FechaCreacion, FechaPrimeraRespuesta) 
                    END) AS TiempoPromedioRespuestaMinutos,
                    AVG(CSATScore) AS CSATPromedio,
                    SUM(CASE WHEN TipoTicket = 'LlamadaCortada' THEN 1 ELSE 0 END) AS TotalLlamadasVAPI,
                    SUM(CASE WHEN TipoTicket = 'WhatsApp' THEN 1 ELSE 0 END) AS TotalMensajesWhatsApp,
                    SUM(CASE WHEN TipoTicket = 'ChatWeb' THEN 1 ELSE 0 END) AS TotalChatWeb,
                    SUM(CASE WHEN TipoTicket = 'ChatApp' THEN 1 ELSE 0 END) AS TotalChatApp
                FROM op_tickets_v2
                WHERE IdEntidad = @IdEntidad
                  AND FechaCreacion BETWEEN @FechaInicio AND @FechaFin
                  AND Activo = 1";

            var parametros = new Dictionary<string, object>
            {
                { "@IdEntidad", idEntidad },
                { "@FechaInicio", fechaInicio },
                { "@FechaFin", fechaFin }
            };

            var resultado = (await _db.EjecutarConsultaAsync(query, parametros)).FirstOrDefault();

            if (resultado == null)
            {
                return new TicketMetricsDto();
            }

            // Obtener tokens de IA utilizados
            var tokensQuery = @"
                SELECT 
                    COALESCE(SUM(TokensUtilizados), 0) AS TokensIAUtilizados
                FROM op_ticket_logprompts
                WHERE IdEntidad = @IdEntidad
                  AND FechaCreacion BETWEEN @FechaInicio AND @FechaFin
                  AND Activo = 1";

            var tokensResultado = (await _db.EjecutarConsultaAsync(tokensQuery, parametros)).FirstOrDefault();
            var tokensUtilizados = Convert.ToInt32(tokensResultado?["TokensIAUtilizados"] ?? 0);

            // Calcular costo estimado (GPT-4o-mini: $0.15 por 1M tokens input, $0.60 por 1M tokens output)
            // Asumiendo 70% input, 30% output
            var costoEstimado = (tokensUtilizados * 0.7m * 0.15m / 1_000_000m) + 
                               (tokensUtilizados * 0.3m * 0.60m / 1_000_000m);

            var metricas = new TicketMetricsDto
            {
                TotalTicketsCreados = Convert.ToInt32(resultado["TotalTicketsCreados"] ?? 0),
                TotalTicketsResueltos = Convert.ToInt32(resultado["TotalTicketsResueltos"] ?? 0),
                TotalTicketsResueltosIA = Convert.ToInt32(resultado["TotalTicketsResueltosIA"] ?? 0),
                PorcentajeResolucionIA = Convert.ToDecimal(resultado["PorcentajeResolucionIA"] ?? 0),
                TiempoPromedioResolucionMinutos = resultado["TiempoPromedioResolucionMinutos"] != null 
                    ? Convert.ToDecimal(resultado["TiempoPromedioResolucionMinutos"]) 
                    : null,
                TiempoPromedioRespuestaMinutos = resultado["TiempoPromedioRespuestaMinutos"] != null 
                    ? Convert.ToDecimal(resultado["TiempoPromedioRespuestaMinutos"]) 
                    : null,
                CSATPromedio = resultado["CSATPromedio"] != null 
                    ? Convert.ToDecimal(resultado["CSATPromedio"]) 
                    : null,
                TotalLlamadasVAPI = Convert.ToInt32(resultado["TotalLlamadasVAPI"] ?? 0),
                TotalMensajesWhatsApp = Convert.ToInt32(resultado["TotalMensajesWhatsApp"] ?? 0),
                TotalChatWeb = Convert.ToInt32(resultado["TotalChatWeb"] ?? 0),
                TotalChatApp = Convert.ToInt32(resultado["TotalChatApp"] ?? 0),
                TokensIAUtilizados = tokensUtilizados,
                CostoEstimadoIA = costoEstimado
            };

            _logger.LogInformation(
                "Métricas obtenidas - Total: {Total}, Resueltos: {Resueltos}, IA: {IA}%",
                metricas.TotalTicketsCreados, metricas.TotalTicketsResueltos, metricas.PorcentajeResolucionIA);

            return metricas;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener métricas en tiempo real");
            throw;
        }
    }

    /// <summary>
    /// Calcula y almacena métricas diarias usando stored procedure
    /// </summary>
    public async Task<bool> CalcularMetricasDiariasAsync(CalcularMetricasRequest request)
    {
        try
        {
            _logger.LogInformation(
                "Calculando métricas diarias - Fecha: {Fecha}, Entidad: {IdEntidad}",
                request.Fecha, request.IdEntidad);

            // Llamar al stored procedure sp_CalcularMetricasDiarias
            var query = @"
                CALL sp_CalcularMetricasDiarias(
                    @Fecha,
                    @IdEntidad
                )";

            var parametros = new Dictionary<string, object>
            {
                { "@Fecha", request.Fecha.Date },
                { "@IdEntidad", request.IdEntidad }
            };

            await _db.EjecutarNoConsultaAsync(query, parametros);

            _logger.LogInformation("Métricas diarias calculadas exitosamente");

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al calcular métricas diarias");
            return false;
        }
    }

    /// <summary>
    /// Obtiene métricas diarias almacenadas
    /// </summary>
    public async Task<IEnumerable<CrudDto>> ObtenerMetricasDiariasAsync(
        int idEntidad, 
        DateTime fechaInicio, 
        DateTime fechaFin, 
        string? canal = null)
    {
        try
        {
            var query = @"
                SELECT * FROM op_ticket_metricas
                WHERE IdEntidad = @IdEntidad
                  AND FechaMetrica BETWEEN @FechaInicio AND @FechaFin
                  AND TipoAgregacion = 'Diaria'
                  AND (@Canal IS NULL OR Canal = @Canal)
                  AND Activo = 1
                ORDER BY FechaMetrica DESC";

            var parametros = new Dictionary<string, object>
            {
                { "@IdEntidad", idEntidad },
                { "@FechaInicio", fechaInicio.Date },
                { "@FechaFin", fechaFin.Date },
                { "@Canal", canal ?? (object)DBNull.Value }
            };

            return await _db.EjecutarConsultaAsync(query, parametros);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener métricas diarias");
            throw;
        }
    }

    /// <summary>
    /// Obtiene métricas agrupadas por canal
    /// </summary>
    public async Task<Dictionary<string, TicketMetricsDto>> ObtenerMetricasPorCanalAsync(int idEntidad, DateTime fecha)
    {
        try
        {
            var canales = new[] { "VAPI", "YCloud", "ChatWeb", "Firebase", "Telegram" };
            var metricasPorCanal = new Dictionary<string, TicketMetricsDto>();

            foreach (var canal in canales)
            {
                var metricas = await ObtenerMetricasTiempoRealAsync(
                    idEntidad, 
                    fecha.Date, 
                    fecha.Date.AddDays(1).AddSeconds(-1));

                metricasPorCanal[canal] = metricas;
            }

            return metricasPorCanal;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener métricas por canal");
            throw;
        }
    }

    /// <summary>
    /// Registra log de interacción multicanal
    /// </summary>
    public async Task<int> RegistrarInteraccionAsync(LogInteraccionDto log)
    {
        try
        {
            _logger.LogInformation(
                "Registrando interacción - Ticket: {IdTicket}, Canal: {Canal}",
                log.IdTicket, log.Canal);

            var campos = new Dictionary<string, object>
            {
                { "IdEntidad", log.IdEntidad },
                { "IdTicket", log.IdTicket },
                { "Canal", log.Canal },
                { "TipoInteraccion", log.TipoInteraccion },
                { "IdExternoCanal", log.IdExternoCanal ?? (object)DBNull.Value },
                { "DatosInteraccion", log.DatosInteraccion != null 
                    ? JsonSerializer.Serialize(log.DatosInteraccion) 
                    : (object)DBNull.Value },
                { "Duracion", log.Duracion ?? (object)DBNull.Value },
                { "Exitosa", log.Exitosa },
                { "MensajeError", log.MensajeError ?? (object)DBNull.Value },
                { "IdUsuarioCreacion", 1 },
                { "Activo", true }
            };

            var idLog = await _db.InsertarAsync("op_ticket_logs_interacciones", campos);

            _logger.LogInformation("Interacción registrada - ID: {IdLog}", idLog);

            return idLog;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al registrar interacción");
            throw;
        }
    }

    /// <summary>
    /// Registra log de prompt de IA
    /// </summary>
    public async Task<int> RegistrarPromptAsync(LogPromptDto log)
    {
        try
        {
            _logger.LogInformation(
                "Registrando prompt - Ticket: {IdTicket}, Modelo: {Modelo}",
                log.IdTicket, log.ModeloUtilizado);

            var campos = new Dictionary<string, object>
            {
                { "IdEntidad", log.IdEntidad },
                { "IdTicket", log.IdTicket ?? (object)DBNull.Value },
                { "IdPrompt", log.IdPrompt ?? (object)DBNull.Value },
                { "PromptEnviado", log.PromptEnviado },
                { "RespuestaIA", log.RespuestaIA ?? (object)DBNull.Value },
                { "ModeloUtilizado", log.ModeloUtilizado ?? (object)DBNull.Value },
                { "TokensUtilizados", log.TokensUtilizados ?? (object)DBNull.Value },
                { "TiempoRespuestaMs", log.TiempoRespuestaMs ?? (object)DBNull.Value },
                { "Exitoso", log.Exitoso },
                { "MensajeError", log.MensajeError ?? (object)DBNull.Value },
                { "Activo", true }
            };

            var idLog = await _db.InsertarAsync("op_ticket_logprompts", campos);

            _logger.LogInformation(
                "Prompt registrado - ID: {IdLog}, Tokens: {Tokens}",
                idLog, log.TokensUtilizados);

            return idLog;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al registrar prompt");
            throw;
        }
    }
}
