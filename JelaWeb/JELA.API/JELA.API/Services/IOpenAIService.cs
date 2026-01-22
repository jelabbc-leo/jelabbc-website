namespace JELA.API.Services;

/// <summary>
/// Servicio para integración con Azure OpenAI Service
/// </summary>
public interface IOpenAIService
{
    /// <summary>
    /// Genera una respuesta usando Azure OpenAI Service
    /// </summary>
    /// <param name="prompt">Prompt para enviar a OpenAI</param>
    /// <param name="systemMessage">Mensaje del sistema (opcional). Por defecto usa un mensaje estándar.</param>
    /// <param name="temperature">Temperatura para la generación (0.0-1.0). Por defecto 0.7</param>
    /// <param name="maxTokens">Máximo de tokens en la respuesta. Por defecto 1000</param>
    /// <returns>Respuesta generada por OpenAI</returns>
    Task<string> GenerarRespuestaAsync(
        string prompt,
        string? systemMessage = null,
        double temperature = 0.7,
        int maxTokens = 1000);
}
