using JELA.API.Models;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace JELA.API.Services;

/// <summary>
/// Servicio para integración con YCloud (WhatsApp Business API)
/// </summary>
public class YCloudService : IYCloudService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<YCloudService> _logger;
    private readonly string _apiKey;
    private readonly string _baseUrl;
    private readonly string _webhookSecret;

    public YCloudService(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<YCloudService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _logger = logger;

        _apiKey = configuration["YCloud:ApiKey"] ?? throw new InvalidOperationException("YCloud:ApiKey not configured");
        _baseUrl = configuration["YCloud:BaseUrl"] ?? "https://api.ycloud.com/v2";
        _webhookSecret = configuration["YCloud:WebhookSecret"] ?? "";
    }

    /// <summary>
    /// Envía un mensaje de WhatsApp a través de YCloud
    /// </summary>
    public async Task<YCloudSendMessageResponse> EnviarMensajeAsync(YCloudSendMessageRequest request)
    {
        try
        {
            _logger.LogInformation(
                "Enviando mensaje WhatsApp a {To} - Texto: {Text}",
                request.To, request.Text.Substring(0, Math.Min(50, request.Text.Length)));

            var httpClient = _httpClientFactory.CreateClient();
            httpClient.DefaultRequestHeaders.Add("X-API-Key", _apiKey);

            var payload = new
            {
                to = request.To,
                type = "text",
                text = new { body = request.Text }
            };

            var content = new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json");

            var response = await httpClient.PostAsync($"{_baseUrl}/whatsapp/messages", content);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var result = JsonSerializer.Deserialize<JsonElement>(responseBody);
                var messageId = result.GetProperty("id").GetString();

                _logger.LogInformation(
                    "Mensaje WhatsApp enviado exitosamente - MessageId: {MessageId}",
                    messageId);

                return new YCloudSendMessageResponse(
                    Success: true,
                    MessageId: messageId,
                    ErrorCode: null,
                    ErrorMessage: null
                );
            }
            else
            {
                _logger.LogError(
                    "Error al enviar mensaje WhatsApp - Status: {Status}, Response: {Response}",
                    response.StatusCode, responseBody);

                return new YCloudSendMessageResponse(
                    Success: false,
                    MessageId: null,
                    ErrorCode: response.StatusCode.ToString(),
                    ErrorMessage: responseBody
                );
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Excepción al enviar mensaje WhatsApp");
            return new YCloudSendMessageResponse(
                Success: false,
                MessageId: null,
                ErrorCode: "EXCEPTION",
                ErrorMessage: ex.Message
            );
        }
    }

    /// <summary>
    /// Valida la firma del webhook de YCloud usando HMAC-SHA256
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
    /// Procesa la respuesta de un webhook de YCloud
    /// </summary>
    public async Task<bool> ProcesarRespuestaAsync(YCloudWebhookRequest webhook)
    {
        try
        {
            _logger.LogInformation(
                "Procesando webhook YCloud - MessageId: {MessageId}, From: {From}",
                webhook.MessageId, webhook.From);

            // TODO: Implementar lógica de procesamiento
            // 1. Buscar ticket relacionado por número de teléfono
            // 2. Actualizar estado del ticket
            // 3. Registrar interacción
            // 4. Notificar al agente si es necesario

            await Task.CompletedTask; // Placeholder

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al procesar webhook YCloud");
            return false;
        }
    }

    /// <summary>
    /// Obtiene el estado de un mensaje enviado
    /// </summary>
    public async Task<YCloudMessageStatus?> ObtenerEstadoMensajeAsync(string messageId)
    {
        try
        {
            _logger.LogInformation("Obteniendo estado de mensaje - MessageId: {MessageId}", messageId);

            var httpClient = _httpClientFactory.CreateClient();
            httpClient.DefaultRequestHeaders.Add("X-API-Key", _apiKey);

            var response = await httpClient.GetAsync($"{_baseUrl}/whatsapp/messages/{messageId}");
            var responseBody = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var result = JsonSerializer.Deserialize<JsonElement>(responseBody);
                var status = result.GetProperty("status").GetString() ?? "unknown";

                return new YCloudMessageStatus(
                    Status: status,
                    DeliveredAt: null,
                    ReadAt: null,
                    ErrorCode: null,
                    ErrorMessage: null
                );
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener estado de mensaje");
            return null;
        }
    }
}
