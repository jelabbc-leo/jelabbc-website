using JELA.API.Models;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace JELA.API.Services;

/// <summary>
/// Servicio para integración con VAPI (Voice AI Platform)
/// </summary>
public class VapiService : IVapiService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<VapiService> _logger;
    private readonly string _apiKey;
    private readonly string _baseUrl;
    private readonly string _webhookSecret;

    public VapiService(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<VapiService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _logger = logger;

        _apiKey = configuration["VAPI:ApiKey"] ?? throw new InvalidOperationException("VAPI:ApiKey not configured");
        _baseUrl = configuration["VAPI:BaseUrl"] ?? "https://api.vapi.ai";
        _webhookSecret = configuration["VAPI:WebhookSecret"] ?? "";
    }

    /// <summary>
    /// Procesa una llamada recibida de VAPI
    /// </summary>
    public async Task<bool> ProcesarLlamadaAsync(VapiWebhookRequest webhook)
    {
        try
        {
            _logger.LogInformation(
                "Procesando llamada VAPI - CallId: {CallId}, Teléfono: {Phone}, Duración: {Duration}s",
                webhook.CallId, webhook.PhoneNumber, webhook.DurationSeconds);

            // TODO: Implementar lógica de procesamiento
            // 1. Validar cliente duplicado por teléfono
            // 2. Procesar transcripción con Azure OpenAI
            // 3. Crear ticket con tipo "LlamadaCortada" o "Accion"
            // 4. Registrar duración y momento de corte
            // 5. Registrar interacción en logs

            _logger.LogInformation(
                "Transcripción: {Transcription}",
                webhook.Transcription.Substring(0, Math.Min(100, webhook.Transcription.Length)));

            await Task.CompletedTask; // Placeholder

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al procesar llamada VAPI");
            return false;
        }
    }

    /// <summary>
    /// Valida la firma del webhook de VAPI usando HMAC-SHA256
    /// </summary>
    public bool ValidarWebhook(string payload, string signature)
    {
        try
        {
            if (string.IsNullOrEmpty(_webhookSecret))
            {
                _logger.LogWarning("Webhook secret no configurado - Validación omitida");
                return true; // En desarrollo, permitir sin validación
            }

            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_webhookSecret));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
            var computedSignature = "sha256=" + BitConverter.ToString(hash).Replace("-", "").ToLower();

            var isValid = computedSignature.Equals(signature, StringComparison.OrdinalIgnoreCase);

            if (!isValid)
            {
                _logger.LogWarning(
                    "Firma de webhook inválida - Esperada: {Expected}, Recibida: {Received}",
                    computedSignature, signature);
            }

            return isValid;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al validar firma de webhook");
            return false;
        }
    }

    /// <summary>
    /// Obtiene la transcripción completa de una llamada
    /// </summary>
    public async Task<string?> ObtenerTranscripcionAsync(string callId)
    {
        try
        {
            _logger.LogInformation("Obteniendo transcripción - CallId: {CallId}", callId);

            var httpClient = _httpClientFactory.CreateClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");

            var response = await httpClient.GetAsync($"{_baseUrl}/call/{callId}");
            var responseBody = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var result = JsonSerializer.Deserialize<JsonElement>(responseBody);
                
                if (result.TryGetProperty("transcript", out var transcript))
                {
                    return transcript.GetString();
                }
            }

            _logger.LogWarning("No se pudo obtener transcripción - CallId: {CallId}", callId);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener transcripción");
            return null;
        }
    }

    /// <summary>
    /// Procesa un evento de VAPI
    /// </summary>
    public async Task<bool> ProcesarEventoAsync(VapiEventRequest evento)
    {
        try
        {
            _logger.LogInformation(
                "Procesando evento VAPI - Tipo: {EventType}, CallId: {CallId}",
                evento.EventType, evento.CallId);

            switch (evento.EventType)
            {
                case "call.started":
                    _logger.LogInformation("Llamada iniciada - CallId: {CallId}", evento.CallId);
                    // TODO: Registrar inicio de llamada
                    break;

                case "call.ended":
                    _logger.LogInformation("Llamada terminada - CallId: {CallId}", evento.CallId);
                    // TODO: Procesar llamada completa
                    break;

                case "call.disconnected":
                    _logger.LogWarning("Llamada cortada - CallId: {CallId}", evento.CallId);
                    // TODO: Crear ticket de llamada cortada
                    break;

                case "transcription.partial":
                    _logger.LogDebug("Transcripción parcial recibida - CallId: {CallId}", evento.CallId);
                    // TODO: Actualizar transcripción en tiempo real
                    break;

                case "transcription.final":
                    _logger.LogInformation("Transcripción final recibida - CallId: {CallId}", evento.CallId);
                    // TODO: Guardar transcripción completa
                    break;

                default:
                    _logger.LogWarning("Evento desconocido: {EventType}", evento.EventType);
                    break;
            }

            await Task.CompletedTask; // Placeholder

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al procesar evento VAPI");
            return false;
        }
    }
}
