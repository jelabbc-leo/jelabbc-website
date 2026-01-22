using JELA.API.Models;

namespace JELA.API.Services;

/// <summary>
/// Interfaz para servicio de Azure Document Intelligence
/// </summary>
public interface IDocumentIntelligenceService
{
    /// <summary>
    /// Procesa un archivo de INE y retorna datos en formato CrudDto
    /// </summary>
    Task<List<CrudDto>> ProcesarINEAsync(byte[] archivoBytes, string contentType);

    /// <summary>
    /// Procesa un archivo de Tarjeta de Circulación y retorna datos en formato CrudDto
    /// </summary>
    Task<List<CrudDto>> ProcesarTarjetaCirculacionAsync(byte[] archivoBytes, string contentType);
}
