using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace JELA.API.Services;

/// <summary>
/// Servicio para integración con Azure OpenAI Service
/// </summary>
public class AzureOpenAIService : IOpenAIService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _endpoint;
    private readonly string _deploymentName;
    private readonly string _apiVersion;
    private readonly ILogger<AzureOpenAIService> _logger;

    public AzureOpenAIService(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<AzureOpenAIService> logger)
    {
        _httpClient = httpClientFactory.CreateClient();
        _logger = logger;

        // Obtener configuración desde appsettings
        var apiKeyRaw = configuration["AzureOpenAI:ApiKey"] ?? string.Empty;
        _endpoint = configuration["AzureOpenAI:Endpoint"] ?? string.Empty;
        _deploymentName = configuration["AzureOpenAI:DeploymentName"] ?? string.Empty;
        _apiVersion = configuration["AzureOpenAI:ApiVersion"] ?? "2024-12-01-preview";

        // Limpiar el apiKey: remover prefijos como "api-key: " si existen
        if (!string.IsNullOrWhiteSpace(apiKeyRaw))
        {
            _apiKey = apiKeyRaw.Trim();
            if (_apiKey.StartsWith("api-key:", StringComparison.OrdinalIgnoreCase))
            {
                _apiKey = _apiKey.Substring("api-key:".Length).Trim();
            }
        }

        // Valores por defecto
        if (string.IsNullOrWhiteSpace(_deploymentName))
        {
            _deploymentName = "gpt-4o-mini";
        }

        // Validar configuración
        if (string.IsNullOrWhiteSpace(_apiKey) || string.IsNullOrWhiteSpace(_endpoint) || string.IsNullOrWhiteSpace(_deploymentName))
        {
            _logger.LogWarning("Azure OpenAI no está configurado correctamente");
        }
    }

    public async Task<string> GenerarRespuestaAsync(
        string prompt,
        string? systemMessage = null,
        double temperature = 0.7,
        int maxTokens = 1000)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(_apiKey) || string.IsNullOrWhiteSpace(_endpoint) || string.IsNullOrWhiteSpace(_deploymentName))
            {
                throw new Exception("La configuración de OpenAI no está completa. Verifique appsettings.json");
            }

            if (string.IsNullOrWhiteSpace(prompt))
            {
                throw new ArgumentException("El prompt no puede estar vacío", nameof(prompt));
            }

            // Construir URL de la API
            var url = $"{_endpoint.TrimEnd('/')}/openai/deployments/{_deploymentName}/chat/completions?api-version={_apiVersion}";

            // Construir cuerpo de la petición
            var messages = new List<Dictionary<string, string>>();

            // Mensaje del sistema - REQUERIDO, no usar fallback
            if (string.IsNullOrWhiteSpace(systemMessage))
            {
                _logger.LogError("systemMessage es requerido - no se permiten prompts por defecto");
                throw new ArgumentException(
                    "El parámetro 'systemMessage' es requerido. " +
                    "Todos los prompts deben cargarse desde conf_ticket_prompts.", 
                    nameof(systemMessage));
            }

            messages.Add(new Dictionary<string, string>
            {
                { "role", "system" },
                { "content", systemMessage }
            });

            // Mensaje del usuario
            messages.Add(new Dictionary<string, string>
            {
                { "role", "user" },
                { "content", prompt }
            });

            var requestBody = new Dictionary<string, object>
            {
                { "messages", messages },
                { "temperature", temperature },
                { "max_tokens", maxTokens },
                { "top_p", 0.95 },
                { "frequency_penalty", 0 },
                { "presence_penalty", 0 }
            };

            var jsonBody = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            // Agregar headers
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("api-key", _apiKey);

            // Realizar petición
            var response = await _httpClient.PostAsync(url, content);

            // Leer respuesta
            var responseBody = await response.Content.ReadAsStringAsync();

            // Validar respuesta
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Error en OpenAI API: {StatusCode} - {Error}", response.StatusCode, responseBody);
                throw new Exception($"Error al comunicarse con OpenAI: {response.StatusCode}");
            }

            // Parsear respuesta JSON
            using var doc = JsonDocument.Parse(responseBody);
            var root = doc.RootElement;

            if (!root.TryGetProperty("choices", out var choices) || choices.GetArrayLength() == 0)
            {
                throw new Exception("Respuesta inválida de OpenAI");
            }

            var firstChoice = choices[0];
            if (!firstChoice.TryGetProperty("message", out var message) ||
                !message.TryGetProperty("content", out var contentElement))
            {
                throw new Exception("No se recibió ninguna respuesta de OpenAI");
            }

            return contentElement.GetString() ?? string.Empty;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al generar respuesta con OpenAI");
            throw;
        }
    }
}
