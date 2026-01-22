using JELA.API.Models;
using JELA.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace JELA.API.Endpoints;

/// <summary>
/// Endpoints para procesamiento de documentos con Azure Document Intelligence
/// </summary>
public static class DocumentIntelligenceEndpoints
{
    public static void MapDocumentIntelligenceEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/document-intelligence")
            .WithTags("Document Intelligence")
            .WithOpenApi();

        // POST: Procesar documento (INE o Tarjeta de Circulación)
        // Acepta multipart/form-data con archivo
        group.MapPost("", ProcesarDocumento)
            .WithName("ProcesarDocumento")
            .WithDescription("Procesa un archivo PDF, JPG o PNG de INE o Tarjeta de Circulación usando Azure Document Intelligence. Retorna datos en formato CrudDto.")
            .RequireAuthorization()
            .DisableAntiforgery() // Necesario para multipart/form-data
            .Produces<List<CrudDto>>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<ErrorResponse>(StatusCodes.Status401Unauthorized)
            .Produces<ErrorResponse>(StatusCodes.Status500InternalServerError);
    }

    /// <summary>
    /// Procesa un documento (INE o Tarjeta de Circulación) desde un archivo
    /// </summary>
    private static async Task<IResult> ProcesarDocumento(
        HttpRequest request,
        IDocumentIntelligenceService service,
        ILogger<Program> logger)
    {
        try
        {
            // Validar que sea multipart/form-data
            if (!request.HasFormContentType)
            {
                return Results.BadRequest(new ErrorResponse { Mensaje = "El contenido debe ser multipart/form-data" });
            }

            var form = await request.ReadFormAsync();
            var file = form.Files.GetFile("archivo");

            if (file == null || file.Length == 0)
            {
                return Results.BadRequest(new ErrorResponse { Mensaje = "Se requiere un archivo en el campo 'archivo'" });
            }

            // Validar tipo de archivo
            var extension = Path.GetExtension(file.FileName).ToLower();
            var contentType = file.ContentType;

            if (!IsValidFileType(extension, contentType))
            {
                return Results.BadRequest(new ErrorResponse { Mensaje = "El archivo debe ser PDF, JPG o PNG" });
            }

            // Obtener tipo de documento (opcional, por defecto INE)
            var tipoDocumento = form["tipoDocumento"].ToString().ToUpper();
            if (string.IsNullOrWhiteSpace(tipoDocumento))
            {
                tipoDocumento = "INE";
            }

            // Leer archivo en memoria
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            var fileBytes = memoryStream.ToArray();

            // Procesar según el tipo de documento
            List<CrudDto> resultado;

            if (tipoDocumento == "TARJETA_CIRCULACION")
            {
                resultado = await service.ProcesarTarjetaCirculacionAsync(fileBytes, contentType);
            }
            else
            {
                resultado = await service.ProcesarINEAsync(fileBytes, contentType);
            }

            return Results.Ok(resultado);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error procesando documento");
            return Results.Problem(
                detail: ex.Message,
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Error al procesar el documento"
            );
        }
    }

    private static bool IsValidFileType(string extension, string contentType)
    {
        var validExtensions = new[] { ".pdf", ".jpg", ".jpeg", ".png" };
        var validContentTypes = new[] { "application/pdf", "image/jpeg", "image/jpg", "image/png" };

        return validExtensions.Contains(extension) || validContentTypes.Contains(contentType.ToLower());
    }
}
