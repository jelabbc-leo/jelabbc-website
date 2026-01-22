using JELA.API.Models;

namespace JELA.API.Services;

/// <summary>
/// Servicio para ajuste automático de prompts de IA
/// </summary>
public class PromptTuningService : IPromptTuningService
{
    private readonly IDatabaseService _db;
    private readonly ILogger<PromptTuningService> _logger;

    public PromptTuningService(
        IDatabaseService databaseService,
        ILogger<PromptTuningService> logger)
    {
        _db = databaseService;
        _logger = logger;
    }

    /// <summary>
    /// Analiza el rendimiento de los prompts en los últimos N días
    /// </summary>
    public async Task<Dictionary<int, PromptAnalisisDto>> AnalizarRendimientoPromptsAsync(int idEntidad, int diasAnalisis = 14)
    {
        try
        {
            _logger.LogInformation(
                "Analizando rendimiento de prompts - Entidad: {IdEntidad}, Días: {Dias}",
                idEntidad, diasAnalisis);

            var fechaInicio = DateTime.Now.AddDays(-diasAnalisis);

            var query = @"
                SELECT 
                    IdPrompt,
                    COUNT(*) AS TotalUsos,
                    SUM(CASE WHEN Exitoso = 1 THEN 1 ELSE 0 END) AS UsosExitosos,
                    ROUND((SUM(CASE WHEN Exitoso = 1 THEN 1 ELSE 0 END) * 100.0) / COUNT(*), 2) AS TasaExito,
                    AVG(TiempoRespuestaMs) AS TiempoPromedioMs,
                    AVG(TokensUtilizados) AS TokensPromedio
                FROM op_ticket_logprompts
                WHERE IdEntidad = @IdEntidad
                  AND FechaCreacion >= @FechaInicio
                  AND IdPrompt IS NOT NULL
                  AND Activo = 1
                GROUP BY IdPrompt";

            var parametros = new Dictionary<string, object>
            {
                { "@IdEntidad", idEntidad },
                { "@FechaInicio", fechaInicio }
            };

            var resultados = await _db.EjecutarConsultaAsync(query, parametros);
            var analisis = new Dictionary<int, PromptAnalisisDto>();

            foreach (var resultado in resultados)
            {
                var idPrompt = Convert.ToInt32(resultado["IdPrompt"]);
                var totalUsos = Convert.ToInt32(resultado["TotalUsos"]);
                var usosExitosos = Convert.ToInt32(resultado["UsosExitosos"]);
                var tasaExito = Convert.ToDecimal(resultado["TasaExito"] ?? 0);

                var dto = new PromptAnalisisDto
                {
                    IdPrompt = idPrompt,
                    TotalUsos = totalUsos,
                    UsosExitosos = usosExitosos,
                    TasaExito = tasaExito,
                    TiempoPromedioMs = Convert.ToDecimal(resultado["TiempoPromedioMs"] ?? 0),
                    TokensPromedio = Convert.ToInt32(resultado["TokensPromedio"] ?? 0),
                    CostoPromedio = 0 // TODO: Calcular costo promedio
                };

                // Detectar problemas
                if (tasaExito < 80)
                {
                    dto.ProblemasDetectados.Add($"Tasa de éxito baja: {tasaExito}%");
                }

                if (dto.TiempoPromedioMs > 5000)
                {
                    dto.ProblemasDetectados.Add($"Tiempo de respuesta alto: {dto.TiempoPromedioMs}ms");
                }

                if (dto.TokensPromedio > 2000)
                {
                    dto.ProblemasDetectados.Add($"Consumo alto de tokens: {dto.TokensPromedio}");
                }

                // Generar sugerencias
                if (dto.ProblemasDetectados.Count > 0)
                {
                    dto.SugerenciasMejora.Add("Revisar y simplificar el prompt");
                    dto.SugerenciasMejora.Add("Agregar ejemplos más claros");
                    dto.SugerenciasMejora.Add("Reducir contexto innecesario");
                }

                analisis[idPrompt] = dto;
            }

            _logger.LogInformation(
                "Análisis completado - {Count} prompts analizados",
                analisis.Count);

            return analisis;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al analizar rendimiento de prompts");
            throw;
        }
    }

    /// <summary>
    /// Propone un ajuste automático para un prompt
    /// </summary>
    public async Task<PromptAjusteRequest?> ProponerAjusteAsync(int idPrompt, int idEntidad)
    {
        try
        {
            _logger.LogInformation(
                "Proponiendo ajuste para prompt - IdPrompt: {IdPrompt}",
                idPrompt);

            // Obtener análisis del prompt
            var analisis = await AnalizarRendimientoPromptsAsync(idEntidad, 14);
            
            if (!analisis.ContainsKey(idPrompt))
            {
                _logger.LogWarning("No hay datos suficientes para proponer ajuste");
                return null;
            }

            var promptAnalisis = analisis[idPrompt];

            // Solo proponer ajuste si hay problemas detectados
            if (promptAnalisis.ProblemasDetectados.Count == 0)
            {
                _logger.LogInformation("Prompt funcionando correctamente - No se requiere ajuste");
                return null;
            }

            // Obtener versión actual del prompt
            var queryPrompt = @"
                SELECT * FROM conf_ticket_prompts
                WHERE Id = @IdPrompt AND Activo = 1";

            var parametros = new Dictionary<string, object>
            {
                { "@IdPrompt", idPrompt }
            };

            var promptActual = (await _db.EjecutarConsultaAsync(queryPrompt, parametros)).FirstOrDefault();

            if (promptActual == null)
            {
                return null;
            }

            var versionAnterior = promptActual["ContenidoPrompt"]?.ToString() ?? "";

            // TODO: Usar Azure OpenAI para generar versión mejorada del prompt
            var versionNueva = versionAnterior; // Placeholder

            var metricasAntes = new Dictionary<string, object>
            {
                { "TasaExito", promptAnalisis.TasaExito },
                { "TiempoPromedio", promptAnalisis.TiempoPromedioMs },
                { "TokensPromedio", promptAnalisis.TokensPromedio }
            };

            var request = new PromptAjusteRequest
            {
                IdPrompt = idPrompt,
                VersionAnterior = versionAnterior,
                VersionNueva = versionNueva,
                MotivoAjuste = string.Join("; ", promptAnalisis.ProblemasDetectados),
                MetricasAntes = metricasAntes,
                MetricasDespues = null,
                PorcentajeMejora = null,
                IdEntidad = idEntidad
            };

            _logger.LogInformation("Ajuste propuesto para prompt {IdPrompt}", idPrompt);

            return request;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al proponer ajuste");
            return null;
        }
    }

    /// <summary>
    /// Registra un ajuste de prompt en el log
    /// </summary>
    public async Task<PromptAjusteResponse> RegistrarAjusteAsync(PromptAjusteRequest request)
    {
        try
        {
            _logger.LogInformation(
                "Registrando ajuste de prompt - IdPrompt: {IdPrompt}",
                request.IdPrompt);

            var campos = new Dictionary<string, object>
            {
                { "IdEntidad", request.IdEntidad },
                { "IdPrompt", request.IdPrompt },
                { "FechaAjuste", DateTime.Now },
                { "VersionAnterior", request.VersionAnterior },
                { "VersionNueva", request.VersionNueva },
                { "MotivoAjuste", request.MotivoAjuste ?? (object)DBNull.Value },
                { "MetricasAntes", request.MetricasAntes != null 
                    ? System.Text.Json.JsonSerializer.Serialize(request.MetricasAntes) 
                    : (object)DBNull.Value },
                { "MetricasDespues", request.MetricasDespues != null 
                    ? System.Text.Json.JsonSerializer.Serialize(request.MetricasDespues) 
                    : (object)DBNull.Value },
                { "MejoraDetectada", (object)DBNull.Value },
                { "PorcentajeMejora", request.PorcentajeMejora ?? (object)DBNull.Value },
                { "Aprobado", false },
                { "IdUsuarioCreacion", 1 },
                { "Activo", true }
            };

            var idAjuste = await _db.InsertarAsync("op_ticket_prompt_ajustes_log", campos);

            _logger.LogInformation("Ajuste registrado - ID: {IdAjuste}", idAjuste);

            return new PromptAjusteResponse
            {
                Success = true,
                IdAjuste = idAjuste,
                RequiereAprobacion = true,
                Mensaje = "Ajuste registrado exitosamente - Pendiente de aprobación"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al registrar ajuste");
            return new PromptAjusteResponse
            {
                Success = false,
                Mensaje = $"Error: {ex.Message}"
            };
        }
    }

    /// <summary>
    /// Aprueba un ajuste de prompt
    /// </summary>
    public async Task<bool> AprobarAjusteAsync(int idAjuste, int idUsuarioAprobacion)
    {
        try
        {
            _logger.LogInformation(
                "Aprobando ajuste - IdAjuste: {IdAjuste}, Usuario: {IdUsuario}",
                idAjuste, idUsuarioAprobacion);

            var campos = new Dictionary<string, object>
            {
                { "Aprobado", true },
                { "IdUsuarioAprobacion", idUsuarioAprobacion },
                { "FechaAprobacion", DateTime.Now }
            };

            var actualizado = await _db.ActualizarAsync("op_ticket_prompt_ajustes_log", idAjuste, campos);

            if (actualizado)
            {
                // TODO: Aplicar el ajuste al prompt en conf_ticket_prompts
                _logger.LogInformation("Ajuste aprobado exitosamente");
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al aprobar ajuste");
            return false;
        }
    }

    /// <summary>
    /// Obtiene ajustes pendientes de aprobación
    /// </summary>
    public async Task<IEnumerable<CrudDto>> ObtenerAjustesPendientesAsync(int idEntidad)
    {
        try
        {
            var query = @"
                SELECT 
                    a.*,
                    p.NombrePrompt
                FROM op_ticket_prompt_ajustes_log a
                LEFT JOIN conf_ticket_prompts p ON a.IdPrompt = p.Id
                WHERE a.IdEntidad = @IdEntidad
                  AND a.Aprobado = FALSE
                  AND a.Activo = 1
                ORDER BY a.FechaAjuste DESC";

            var parametros = new Dictionary<string, object>
            {
                { "@IdEntidad", idEntidad }
            };

            return await _db.EjecutarConsultaAsync(query, parametros);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener ajustes pendientes");
            throw;
        }
    }

    /// <summary>
    /// Obtiene un prompt por su nombre
    /// </summary>
    public async Task<string?> ObtenerPromptPorNombreAsync(string nombrePrompt, int idEntidad)
    {
        try
        {
            _logger.LogInformation(
                "Obteniendo prompt - Nombre: {NombrePrompt}, Entidad: {IdEntidad}",
                nombrePrompt, idEntidad);

            var query = @"
                SELECT ContenidoPrompt
                FROM conf_ticket_prompts
                WHERE NombrePrompt = @NombrePrompt
                  AND IdEntidad = @IdEntidad
                  AND Activo = 1
                LIMIT 1";

            var parametros = new Dictionary<string, object>
            {
                { "@NombrePrompt", nombrePrompt },
                { "@IdEntidad", idEntidad }
            };

            var resultados = await _db.EjecutarConsultaAsync(query, parametros);
            var resultado = resultados.FirstOrDefault();

            if (resultado != null)
            {
                var contenido = resultado["ContenidoPrompt"]?.ToString();
                if (!string.IsNullOrEmpty(contenido))
                {
                    _logger.LogInformation("Prompt encontrado: {NombrePrompt}", nombrePrompt);
                    return contenido;
                }
            }

            _logger.LogWarning("Prompt no encontrado: {NombrePrompt}", nombrePrompt);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener prompt por nombre");
            return null;
        }
    }
}
