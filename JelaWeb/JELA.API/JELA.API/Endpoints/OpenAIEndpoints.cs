using JELA.API.Models;
using JELA.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace JELA.API.Endpoints;

/// <summary>
/// Endpoints para integración con Azure OpenAI Service
/// </summary>
public static class OpenAIEndpoints
{
    public static void MapOpenAIEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/openai")
            .WithTags("OpenAI")
            .WithOpenApi();

        // POST: Generar respuesta con OpenAI
        // Acepta JSON con prompt y parámetros opcionales
        group.MapPost("", GenerarRespuesta)
            .WithName("GenerarRespuesta")
            .WithDescription("Genera una respuesta usando Azure OpenAI Service. Retorna datos en formato CrudDto.")
            .RequireAuthorization()
            .Accepts<GenerateOpenAIRequest>("application/json")
            .Produces<List<CrudDto>>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<ErrorResponse>(StatusCodes.Status401Unauthorized)
            .Produces<ErrorResponse>(StatusCodes.Status500InternalServerError);
    }

    /// <summary>
    /// Genera una respuesta usando Azure OpenAI Service
    /// </summary>
    private static async Task<IResult> GenerarRespuesta(
        [FromBody] GenerateOpenAIRequest request,
        IOpenAIService service,
        ILogger<Program> logger)
    {
        try
        {
            // Validar request
            if (request == null || string.IsNullOrWhiteSpace(request.Prompt))
            {
                return Results.BadRequest(new ErrorResponse { Mensaje = "El campo 'Prompt' es requerido" });
            }

            // Validar parámetros opcionales
            if (request.Temperature < 0 || request.Temperature > 1)
            {
                return Results.BadRequest(new ErrorResponse { Mensaje = "Temperature debe estar entre 0.0 y 1.0" });
            }

            if (request.MaxTokens < 1 || request.MaxTokens > 4000)
            {
                return Results.BadRequest(new ErrorResponse { Mensaje = "MaxTokens debe estar entre 1 y 4000" });
            }

            // Generar respuesta con OpenAI
            var respuesta = await service.GenerarRespuestaAsync(
                request.Prompt,
                request.SystemMessage,
                request.Temperature,
                request.MaxTokens);

            // Convertir respuesta a CrudDto (mantener consistencia con otros endpoints)
            var resultado = new List<CrudDto>();
            var dto = new CrudDto();
            
            // Agregar campos usando el indexer que maneja CampoConTipo automáticamente
            dto["Respuesta"] = respuesta;
            dto["Prompt"] = request.Prompt;
            dto["Temperature"] = request.Temperature;
            dto["MaxTokens"] = request.MaxTokens;
            
            resultado.Add(dto);

            return Results.Ok(resultado);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error generando respuesta con OpenAI");
            return Results.Problem(
                detail: ex.Message,
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Error al generar respuesta con OpenAI"
            );
        }
    }
}
