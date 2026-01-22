namespace JELA.API.Models;

/// <summary>
/// Request para procesar un documento con Document Intelligence
/// </summary>
public class ProcessDocumentRequest
{
    /// <summary>
    /// Imagen o PDF en formato Base64 (puede incluir data URL prefix)
    /// </summary>
    public string Imagen { get; set; } = string.Empty;

    /// <summary>
    /// Tipo de documento: "INE" o "TARJETA_CIRCULACION"
    /// </summary>
    public string TipoDocumento { get; set; } = "INE";
}

/// <summary>
/// Respuesta genérica de procesamiento de documento
/// </summary>
public class ProcessDocumentResponse
{
    public bool Exito { get; set; }
    public string Mensaje { get; set; } = string.Empty;
    public object? Datos { get; set; }
}

/// <summary>
/// Datos extraídos de una INE
/// </summary>
public class DatosINE
{
    public string Nombre { get; set; } = string.Empty;
    public string ApellidoPaterno { get; set; } = string.Empty;
    public string ApellidoMaterno { get; set; } = string.Empty;
    public string CURP { get; set; } = string.Empty;
    public string ClaveElector { get; set; } = string.Empty;
    public string NumeroIdentificacion { get; set; } = string.Empty;
    public string FechaNacimiento { get; set; } = string.Empty;
    public string Sexo { get; set; } = string.Empty;
    public string Domicilio { get; set; } = string.Empty;
    public string Seccion { get; set; } = string.Empty;
    public string Vigencia { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public string Municipio { get; set; } = string.Empty;
    public string Localidad { get; set; } = string.Empty;
    public string Emision { get; set; } = string.Empty;
    public string AnioRegistro { get; set; } = string.Empty;
}

/// <summary>
/// Datos extraídos de una Tarjeta de Circulación
/// </summary>
public class DatosTarjetaCirculacion
{
    public string Placas { get; set; } = string.Empty;
    public string NumeroTarjeta { get; set; } = string.Empty;
    public string Marca { get; set; } = string.Empty;
    public string Modelo { get; set; } = string.Empty;
    public string Anio { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public string NumeroMotor { get; set; } = string.Empty;
    public string NumeroSerie { get; set; } = string.Empty;
    public string TipoCombustible { get; set; } = string.Empty;
    public string CapacidadPasajeros { get; set; } = string.Empty;
    public string UsoVehiculo { get; set; } = string.Empty;
    public string PropietarioRegistrado { get; set; } = string.Empty;
    public string FechaExpedicion { get; set; } = string.Empty;
    public string FechaVigencia { get; set; } = string.Empty;
}
