using JELA.API.Models;
using MySqlConnector;
using System.Data;

namespace JELA.API.Services;

/// <summary>
/// Servicio para validación de tickets y prevención de duplicados
/// </summary>
public class TicketValidationService : ITicketValidationService
{
    private readonly IDatabaseService _db;
    private readonly ILogger<TicketValidationService> _logger;

    public TicketValidationService(
        IDatabaseService databaseService,
        ILogger<TicketValidationService> logger)
    {
        _db = databaseService;
        _logger = logger;
    }

    /// <summary>
    /// Valida si un cliente tiene tickets abiertos usando el stored procedure
    /// </summary>
    public async Task<TicketValidationResponse> ValidarClienteDuplicadoAsync(TicketValidationRequest request)
    {
        try
        {
            _logger.LogInformation(
                "Validando cliente duplicado - Teléfono: {Telefono}, Email: {Email}, IP: {IP}",
                request.Telefono, request.Email, request.IPOrigen);

            // Llamar al stored procedure sp_ValidarClienteDuplicado
            var query = @"
                CALL sp_ValidarClienteDuplicado(
                    @Telefono, 
                    @Email, 
                    @IPOrigen, 
                    @IdEntidad, 
                    @TieneTicketAbierto, 
                    @IdTicketAbierto
                )";

            var parametros = new Dictionary<string, object>
            {
                { "@Telefono", request.Telefono ?? (object)DBNull.Value },
                { "@Email", request.Email ?? (object)DBNull.Value },
                { "@IPOrigen", request.IPOrigen ?? (object)DBNull.Value },
                { "@IdEntidad", request.IdEntidad }
            };

            // Ejecutar el stored procedure
            var resultados = await _db.EjecutarConsultaAsync(query, parametros);
            var resultado = resultados.FirstOrDefault();

            if (resultado == null)
            {
                return new TicketValidationResponse
                {
                    TieneTicketAbierto = false,
                    NumeroTicketsHistoricos = 0,
                    Bloqueado = false
                };
            }

            // Mapear resultado del stored procedure
            var response = new TicketValidationResponse
            {
                TieneTicketAbierto = Convert.ToBoolean(resultado["TieneTicketAbierto"] ?? false),
                IdTicketAbierto = resultado["IdTicketAbierto"] as int?,
                NumeroTicketsHistoricos = Convert.ToInt32(resultado["NumeroTicketsHistoricos"] ?? 0),
                UltimaInteraccion = resultado["UltimaInteraccion"] as DateTime?,
                Bloqueado = Convert.ToBoolean(resultado["Bloqueado"] ?? false),
                MotivoBloqueo = resultado["MotivoBloqueo"] as string
            };

            _logger.LogInformation(
                "Validación completada - Tiene ticket abierto: {TieneTicket}, Bloqueado: {Bloqueado}",
                response.TieneTicketAbierto, response.Bloqueado);

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al validar cliente duplicado");
            throw;
        }
    }

    /// <summary>
    /// Obtiene el historial de tickets de un cliente
    /// </summary>
    public async Task<IEnumerable<CrudDto>> ObtenerHistorialClienteAsync(string telefono, int idEntidad)
    {
        try
        {
            _logger.LogInformation("Obteniendo historial de cliente - Teléfono: {Telefono}", telefono);

            var query = @"
                SELECT 
                    t.*,
                    u.Nombre AS NombreAgente
                FROM op_tickets_v2 t
                LEFT JOIN conf_usuarios u ON t.IdUsuarioAsignado = u.Id
                WHERE t.IdEntidad = @IdEntidad
                  AND (t.TelefonoCliente = @Telefono OR t.EmailCliente LIKE CONCAT('%', @Telefono, '%'))
                  AND t.Activo = 1
                ORDER BY t.FechaCreacion DESC
                LIMIT 50";

            var parametros = new Dictionary<string, object>
            {
                { "@Telefono", telefono },
                { "@IdEntidad", idEntidad }
            };

            var tickets = await _db.EjecutarConsultaAsync(query, parametros);

            _logger.LogInformation("Historial obtenido - {Count} tickets encontrados", tickets.Count());

            return tickets;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener historial de cliente");
            throw;
        }
    }

    /// <summary>
    /// Actualiza o crea registro de validación de cliente
    /// </summary>
    public async Task ActualizarValidacionClienteAsync(
        string? telefono, 
        string? email, 
        string? ipOrigen, 
        int idTicket, 
        int idEntidad)
    {
        try
        {
            _logger.LogInformation(
                "Actualizando validación de cliente - Ticket: {IdTicket}",
                idTicket);

            // Verificar si ya existe un registro
            var queryExiste = @"
                SELECT Id FROM op_ticket_validacion_cliente
                WHERE IdEntidad = @IdEntidad
                  AND (
                      (@Telefono IS NOT NULL AND TelefonoCliente = @Telefono) OR
                      (@Email IS NOT NULL AND EmailCliente = @Email) OR
                      (@IPOrigen IS NOT NULL AND IPOrigen = @IPOrigen)
                  )
                LIMIT 1";

            var parametrosExiste = new Dictionary<string, object>
            {
                { "@IdEntidad", idEntidad },
                { "@Telefono", telefono ?? (object)DBNull.Value },
                { "@Email", email ?? (object)DBNull.Value },
                { "@IPOrigen", ipOrigen ?? (object)DBNull.Value }
            };

            var idExistente = await _db.EjecutarEscalarAsync<int?>(queryExiste, parametrosExiste);

            if (idExistente.HasValue)
            {
                // Actualizar registro existente
                // Primero obtener el valor actual de NumeroTicketsHistoricos
                var queryHistoricos = @"
                    SELECT NumeroTicketsHistoricos FROM op_ticket_validacion_cliente
                    WHERE Id = @Id";
                
                var parametrosHistoricos = new Dictionary<string, object>
                {
                    { "@Id", idExistente.Value }
                };
                
                var historicosActual = await _db.EjecutarEscalarAsync<int>(queryHistoricos, parametrosHistoricos);

                var camposUpdate = new Dictionary<string, object>
                {
                    { "TieneTicketAbierto", true },
                    { "IdTicketAbierto", idTicket },
                    { "NumeroTicketsHistoricos", historicosActual + 1 },
                    { "UltimaInteraccion", DateTime.Now },
                    { "FechaUltimaActualizacion", DateTime.Now }
                };

                await _db.ActualizarAsync("op_ticket_validacion_cliente", idExistente.Value, camposUpdate);
            }
            else
            {
                // Crear nuevo registro
                var campos = new Dictionary<string, object>
                {
                    { "IdEntidad", idEntidad },
                    { "TelefonoCliente", telefono ?? (object)DBNull.Value },
                    { "EmailCliente", email ?? (object)DBNull.Value },
                    { "IPOrigen", ipOrigen ?? (object)DBNull.Value },
                    { "TieneTicketAbierto", true },
                    { "IdTicketAbierto", idTicket },
                    { "NumeroTicketsHistoricos", 1 },
                    { "UltimaInteraccion", DateTime.Now },
                    { "Bloqueado", false },
                    { "IdUsuarioCreacion", 1 },
                    { "Activo", true }
                };

                await _db.InsertarAsync("op_ticket_validacion_cliente", campos);
            }

            _logger.LogInformation("Validación de cliente actualizada exitosamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar validación de cliente");
            throw;
        }
    }

    /// <summary>
    /// Bloquea un cliente por spam o abuso
    /// </summary>
    public async Task BloquearClienteAsync(string? telefono, string? email, string motivo, int idEntidad)
    {
        try
        {
            _logger.LogWarning(
                "Bloqueando cliente - Teléfono: {Telefono}, Email: {Email}, Motivo: {Motivo}",
                telefono, email, motivo);

            // Buscar el registro existente
            var queryBuscar = @"
                SELECT Id FROM op_ticket_validacion_cliente
                WHERE IdEntidad = @IdEntidad
                  AND (
                      (@Telefono IS NOT NULL AND TelefonoCliente = @Telefono) OR
                      (@Email IS NOT NULL AND EmailCliente = @Email)
                  )
                LIMIT 1";

            var parametrosBuscar = new Dictionary<string, object>
            {
                { "@IdEntidad", idEntidad },
                { "@Telefono", telefono ?? (object)DBNull.Value },
                { "@Email", email ?? (object)DBNull.Value }
            };

            var idExistente = await _db.EjecutarEscalarAsync<int?>(queryBuscar, parametrosBuscar);

            if (idExistente.HasValue)
            {
                // Actualizar registro existente
                var camposUpdate = new Dictionary<string, object>
                {
                    { "Bloqueado", true },
                    { "MotivoBloqueo", motivo },
                    { "FechaUltimaActualizacion", DateTime.Now }
                };

                await _db.ActualizarAsync("op_ticket_validacion_cliente", idExistente.Value, camposUpdate);
            }
            else
            {
                // Si no existe, crear registro bloqueado
                var campos = new Dictionary<string, object>
                {
                    { "IdEntidad", idEntidad },
                    { "TelefonoCliente", telefono ?? (object)DBNull.Value },
                    { "EmailCliente", email ?? (object)DBNull.Value },
                    { "TieneTicketAbierto", false },
                    { "NumeroTicketsHistoricos", 0 },
                    { "Bloqueado", true },
                    { "MotivoBloqueo", motivo },
                    { "IdUsuarioCreacion", 1 },
                    { "Activo", true }
                };

                await _db.InsertarAsync("op_ticket_validacion_cliente", campos);
            }

            _logger.LogWarning("Cliente bloqueado exitosamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al bloquear cliente");
            throw;
        }
    }
}
