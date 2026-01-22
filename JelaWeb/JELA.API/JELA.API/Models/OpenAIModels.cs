namespace JELA.API.Models;

/// <summary>
/// Request para generar respuesta con OpenAI
/// </summary>
public class GenerateOpenAIRequest
{
    /// <summary>
    /// Prompt para enviar a OpenAI
    /// </summary>
    public string Prompt { get; set; } = string.Empty;

    /// <summary>
    /// Mensaje del sistema (opcional). Por defecto usa un mensaje estándar.
    /// </summary>
    public string? SystemMessage { get; set; }

    /// <summary>
    /// Temperatura para la generación (0.0-1.0). Por defecto 0.7
    /// </summary>
    public double Temperature { get; set; } = 0.7;

    /// <summary>
    /// Máximo de tokens en la respuesta. Por defecto 1000
    /// </summary>
    public int MaxTokens { get; set; } = 1000;
}
