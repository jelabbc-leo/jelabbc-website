using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using JELA.API.Models;
using Microsoft.Extensions.Configuration;

namespace JELA.API.Services;

/// <summary>
/// Servicio para integración con Azure Document Intelligence
/// </summary>
public class AzureDocumentIntelligenceService : IDocumentIntelligenceService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _endpoint;
    private readonly ILogger<AzureDocumentIntelligenceService> _logger;

    public AzureDocumentIntelligenceService(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<AzureDocumentIntelligenceService> logger)
    {
        _httpClient = httpClientFactory.CreateClient();
        _apiKey = configuration["AzureDocumentIntelligence:ApiKey"] ?? string.Empty;
        _endpoint = configuration["AzureDocumentIntelligence:Endpoint"] ?? string.Empty;
        _logger = logger;

        if (string.IsNullOrWhiteSpace(_apiKey) || string.IsNullOrWhiteSpace(_endpoint))
        {
            _logger.LogWarning("Azure Document Intelligence no está configurado correctamente");
        }
    }

    public async Task<List<CrudDto>> ProcesarINEAsync(byte[] archivoBytes, string contentType)
    {
        var resultado = new List<CrudDto>();
        var dto = new CrudDto();

        try
        {
            if (string.IsNullOrWhiteSpace(_apiKey) || string.IsNullOrWhiteSpace(_endpoint))
            {
                throw new Exception("La configuración de Azure Document Intelligence no está completa");
            }

            // Construir URL para prebuilt-idDocument
            var url = ConstruirUrlModelo("prebuilt-idDocument");

            // Enviar archivo directamente
            var content = new ByteArrayContent(archivoBytes);
            content.Headers.ContentType = new MediaTypeHeaderValue(contentType);

            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Add("Ocp-Apim-Subscription-Key", _apiKey);
            request.Content = content;

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                _logger.LogError("Error en Document Intelligence API: {StatusCode} - {Error}", response.StatusCode, errorBody);
                throw new Exception($"Error al procesar el archivo: {response.StatusCode}");
            }

            // Obtener Operation-Location
            if (!response.Headers.TryGetValues("Operation-Location", out var operationLocations))
            {
                throw new Exception("No se recibió la ubicación de la operación");
            }

            var operationLocation = operationLocations.FirstOrDefault();
            if (string.IsNullOrWhiteSpace(operationLocation))
            {
                throw new Exception("No se recibió la ubicación de la operación");
            }

            // Hacer polling y procesar resultados
            var datosINE = await ObtenerYProcesarResultadosINEAsync(operationLocation);

            // Convertir a CrudDto
            ConvertirDatosINEACrudDto(datosINE, dto);
            resultado.Add(dto);

            return resultado;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al procesar INE");
            throw;
        }
    }

    public async Task<List<CrudDto>> ProcesarTarjetaCirculacionAsync(byte[] archivoBytes, string contentType)
    {
        var resultado = new List<CrudDto>();
        var dto = new CrudDto();

        try
        {
            if (string.IsNullOrWhiteSpace(_apiKey) || string.IsNullOrWhiteSpace(_endpoint))
            {
                throw new Exception("La configuración de Azure Document Intelligence no está completa");
            }

            // Construir URL para prebuilt-document
            var url = ConstruirUrlModelo("prebuilt-document");

            // Enviar archivo directamente
            var content = new ByteArrayContent(archivoBytes);
            content.Headers.ContentType = new MediaTypeHeaderValue(contentType);

            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Add("Ocp-Apim-Subscription-Key", _apiKey);
            request.Content = content;

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                _logger.LogError("Error en Document Intelligence API: {StatusCode} - {Error}", response.StatusCode, errorBody);
                throw new Exception($"Error al procesar el archivo: {response.StatusCode}");
            }

            // Obtener Operation-Location
            if (!response.Headers.TryGetValues("Operation-Location", out var operationLocations))
            {
                throw new Exception("No se recibió la ubicación de la operación");
            }

            var operationLocation = operationLocations.FirstOrDefault();
            if (string.IsNullOrWhiteSpace(operationLocation))
            {
                throw new Exception("No se recibió la ubicación de la operación");
            }

            // Hacer polling y procesar resultados
            var datosTarjeta = await ObtenerYProcesarResultadosTarjetaCirculacionAsync(operationLocation);

            // Convertir a CrudDto
            ConvertirDatosTarjetaACrudDto(datosTarjeta, dto);
            resultado.Add(dto);

            return resultado;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al procesar Tarjeta de Circulación");
            throw;
        }
    }

    private string ConstruirUrlModelo(string modelo)
    {
        if (_endpoint.Contains("prebuilt-document"))
            return _endpoint.Replace("prebuilt-document", modelo);
        else if (_endpoint.Contains("prebuilt-idDocument"))
            return _endpoint.Replace("prebuilt-idDocument", modelo);
        else
            return $"{_endpoint.TrimEnd('/')}/formrecognizer/documentModels/{modelo}:analyze?api-version=2023-07-31";
    }

    private async Task<DatosINE> ObtenerYProcesarResultadosINEAsync(string operationUrl)
    {
        var resultado = new DatosINE();
        var maxIntentos = 30;
        var intento = 0;

        while (intento < maxIntentos)
        {
            await Task.Delay(1000);

            var request = new HttpRequestMessage(HttpMethod.Get, operationUrl);
            request.Headers.Add("Ocp-Apim-Subscription-Key", _apiKey);

            var response = await _httpClient.SendAsync(request);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Error al obtener resultados del análisis");
            }

            var jsonResponse = JsonDocument.Parse(responseBody);
            var root = jsonResponse.RootElement;

            if (root.TryGetProperty("status", out var statusElement))
            {
                var status = statusElement.GetString();

                if (status == "succeeded")
                {
                    resultado = ProcesarResultadosINE(root);
                    return resultado;
                }
                else if (status == "failed")
                {
                    throw new Exception("El análisis del documento falló");
                }
            }

            intento++;
        }

        throw new Exception("Tiempo de espera agotado para el análisis");
    }

    private async Task<DatosTarjetaCirculacion> ObtenerYProcesarResultadosTarjetaCirculacionAsync(string operationUrl)
    {
        var resultado = new DatosTarjetaCirculacion();
        var maxIntentos = 30;
        var intento = 0;

        while (intento < maxIntentos)
        {
            await Task.Delay(1000);

            var request = new HttpRequestMessage(HttpMethod.Get, operationUrl);
            request.Headers.Add("Ocp-Apim-Subscription-Key", _apiKey);

            var response = await _httpClient.SendAsync(request);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Error al obtener resultados del análisis");
            }

            var jsonResponse = JsonDocument.Parse(responseBody);
            var root = jsonResponse.RootElement;

            if (root.TryGetProperty("status", out var statusElement))
            {
                var status = statusElement.GetString();

                if (status == "succeeded")
                {
                    resultado = ProcesarResultadosTarjetaCirculacion(root);
                    return resultado;
                }
                else if (status == "failed")
                {
                    throw new Exception("El análisis del documento falló");
                }
            }

            intento++;
        }

        throw new Exception("Tiempo de espera agotado para el análisis");
    }

    private DatosINE ProcesarResultadosINE(JsonElement jsonResponse)
    {
        var resultado = new DatosINE();

        try
        {
            if (!jsonResponse.TryGetProperty("analyzeResult", out var analyzeResult))
                return resultado;

            // Extraer campos estructurados
            if (analyzeResult.TryGetProperty("documents", out var documents) && documents.ValueKind == JsonValueKind.Array && documents.GetArrayLength() > 0)
            {
                var doc = documents[0];
                if (doc.TryGetProperty("fields", out var fields))
                {
                    resultado.Nombre = ObtenerValorCampo(fields, "FirstName");
                    resultado.ApellidoPaterno = ObtenerValorCampo(fields, "LastName");
                    resultado.NumeroIdentificacion = ObtenerValorCampo(fields, "DocumentNumber");
                    resultado.FechaNacimiento = ObtenerValorCampo(fields, "DateOfBirth");
                    resultado.Sexo = ObtenerValorCampo(fields, "Sex");
                    resultado.Domicilio = ObtenerValorCampo(fields, "Address");
                    resultado.CURP = ObtenerValorCampo(fields, "PersonalNumber");
                    resultado.ClaveElector = ObtenerValorCampo(fields, "DocumentNumber");
                    resultado.Vigencia = ObtenerValorCampo(fields, "DateOfExpiration");
                    resultado.Emision = ObtenerValorCampo(fields, "DateOfIssue");

                    // Procesar apellidos
                    var apellidoCompleto = ObtenerValorCampo(fields, "LastName");
                    if (!string.IsNullOrWhiteSpace(apellidoCompleto))
                    {
                        var partes = apellidoCompleto.Split(' ');
                        if (partes.Length >= 2)
                        {
                            resultado.ApellidoPaterno = partes[0];
                            resultado.ApellidoMaterno = string.Join(" ", partes.Skip(1));
                        }
                        else
                        {
                            resultado.ApellidoPaterno = apellidoCompleto;
                        }
                    }
                }
            }

            // Buscar CURP en el contenido si no se encontró
            if (string.IsNullOrWhiteSpace(resultado.CURP) && analyzeResult.TryGetProperty("content", out var contentElement))
            {
                var content = contentElement.GetString() ?? string.Empty;
                resultado.CURP = BuscarCURPEnContenido(content);
            }

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al procesar resultados de INE");
        }

        return resultado;
    }

    private DatosTarjetaCirculacion ProcesarResultadosTarjetaCirculacion(JsonElement jsonResponse)
    {
        var resultado = new DatosTarjetaCirculacion();

        try
        {
            if (!jsonResponse.TryGetProperty("analyzeResult", out var analyzeResult))
                return resultado;

            if (analyzeResult.TryGetProperty("content", out var contentElement))
            {
                var content = contentElement.GetString() ?? string.Empty;
                resultado = ExtraerDatosPorTexto(content);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al procesar resultados de Tarjeta de Circulación");
        }

        return resultado;
    }

    private string ObtenerValorCampo(JsonElement fields, string fieldName)
    {
        try
        {
            if (!fields.TryGetProperty(fieldName, out var field))
                return string.Empty;

            if (field.TryGetProperty("valueString", out var valueString))
                return valueString.GetString() ?? string.Empty;

            if (field.TryGetProperty("content", out var content))
                return content.GetString() ?? string.Empty;

            if (field.TryGetProperty("valueDate", out var valueDate))
                return valueDate.GetString() ?? string.Empty;

            return string.Empty;
        }
        catch
        {
            return string.Empty;
        }
    }

    private string BuscarCURPEnContenido(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            return string.Empty;

        // Patrón CURP: 4 letras, 6 dígitos, 1 letra (H/M), 3 alfanuméricos, 2 dígitos
        var curpPattern = new Regex(@"([A-Z]{4}[0-9]{6}[HM][A-Z0-9]{3}[0-9]{2})", RegexOptions.IgnoreCase);
        var matches = curpPattern.Matches(content);

        if (matches.Count > 0)
        {
            // Buscar el que está cerca de la palabra "CURP"
            foreach (Match match in matches)
            {
                var curpValue = match.Groups[1].Value.ToUpper();
                var matchIndex = match.Index;
                var inicioContexto = Math.Max(0, matchIndex - 200);
                var finContexto = Math.Min(content.Length, matchIndex + match.Length + 200);
                var contexto = content.Substring(inicioContexto, finContexto - inicioContexto);

                if (Regex.IsMatch(contexto, "CURP", RegexOptions.IgnoreCase))
                {
                    return curpValue;
                }
            }

            // Si no hay uno cerca de "CURP", devolver el primero
            return matches[0].Groups[1].Value.ToUpper();
        }

        return string.Empty;
    }

    private DatosTarjetaCirculacion ExtraerDatosPorTexto(string texto)
    {
        var resultado = new DatosTarjetaCirculacion();

        try
        {
            // Placas
            var placasMatch = Regex.Match(texto, @"\b([A-Z]{3}[-\s]?[0-9]{3,4}[-\s]?[A-Z]{0,2})\b", RegexOptions.IgnoreCase);
            if (placasMatch.Success)
                resultado.Placas = placasMatch.Groups[1].Value.Replace("-", "").Replace(" ", "").ToUpper();

            // Número de tarjeta
            var tarjetaMatch = Regex.Match(texto, @"(?:TARJETA|N[UÚ]MERO|FOLIO)[\s:]*([0-9A-Z]+)", RegexOptions.IgnoreCase);
            if (tarjetaMatch.Success)
                resultado.NumeroTarjeta = tarjetaMatch.Groups[1].Value;

            // Marca
            var marcaMatch = Regex.Match(texto, @"(?:MARCA|BRAND)[\s:]+([A-ZÁÉÍÓÚÑ\s]{3,30})", RegexOptions.IgnoreCase);
            if (marcaMatch.Success)
                resultado.Marca = marcaMatch.Groups[1].Value.Trim();

            // Modelo
            var modeloMatch = Regex.Match(texto, @"(?:MODELO|MODEL)[\s:]+([A-Z0-9ÁÉÍÓÚÑ\s\-]{2,30})", RegexOptions.IgnoreCase);
            if (modeloMatch.Success)
                resultado.Modelo = modeloMatch.Groups[1].Value.Trim();

            // Año
            var anioMatch = Regex.Match(texto, @"\b(19[0-9]{2}|20[0-9]{2})\b");
            if (anioMatch.Success)
                resultado.Anio = anioMatch.Groups[1].Value;

            // Color
            var colorMatch = Regex.Match(texto, @"(?:COLOR)[\s:]+([A-ZÁÉÍÓÚÑ\s]{3,20})", RegexOptions.IgnoreCase);
            if (colorMatch.Success)
                resultado.Color = colorMatch.Groups[1].Value.Trim();

            // Número de motor
            var motorMatch = Regex.Match(texto, @"(?:MOTOR|ENGINE|N[UÚ]MERO\s+MOTOR)[\s:]+([A-Z0-9\-\s]{3,30})", RegexOptions.IgnoreCase);
            if (motorMatch.Success)
                resultado.NumeroMotor = motorMatch.Groups[1].Value.Trim();

            // VIN (17 caracteres)
            var vinMatch = Regex.Match(texto, @"\b([A-HJ-NPR-Z0-9]{17})\b", RegexOptions.IgnoreCase);
            if (vinMatch.Success)
                resultado.NumeroSerie = vinMatch.Groups[1].Value.ToUpper();

            // Tipo de combustible
            var combustibleKeywords = new[] { "GASOLINA", "DIESEL", "DIESÉL", "ELECTRICO", "ELÉCTRICO", "HIBRIDO", "HÍBRIDO", "GAS NATURAL" };
            foreach (var keyword in combustibleKeywords)
            {
                if (texto.ToUpper().Contains(keyword))
                {
                    resultado.TipoCombustible = keyword;
                    break;
                }
            }

            // Capacidad de pasajeros
            var capacidadMatch = Regex.Match(texto, @"(?:PASAJEROS|CAPACIDAD|ASIENTOS)[\s:]+([0-9]{1,2})", RegexOptions.IgnoreCase);
            if (capacidadMatch.Success)
                resultado.CapacidadPasajeros = capacidadMatch.Groups[1].Value;

            // Uso del vehículo
            var usoKeywords = new[] { "PARTICULAR", "COMERCIAL", "PÚBLICO", "PUBLICO" };
            foreach (var keyword in usoKeywords)
            {
                if (texto.ToUpper().Contains(keyword))
                {
                    resultado.UsoVehiculo = keyword;
                    break;
                }
            }

            // Propietario
            var propietarioMatch = Regex.Match(texto, @"(?:PROPIETARIO|TITULAR|REGISTRADO|A NOMBRE DE)[\s:]+([A-ZÁÉÍÓÚÑ\s,\.]{5,100})", RegexOptions.IgnoreCase);
            if (propietarioMatch.Success)
                resultado.PropietarioRegistrado = propietarioMatch.Groups[1].Value.Trim();

            // Fechas
            var fechaExpMatch = Regex.Match(texto, @"(?:EXPEDICI[OÓ]N|EMISI[OÓ]N|FECHA)[\s:]+([0-9]{1,2}[/\-][0-9]{1,2}[/\-][0-9]{2,4})", RegexOptions.IgnoreCase);
            if (fechaExpMatch.Success)
                resultado.FechaExpedicion = fechaExpMatch.Groups[1].Value;

            var vigenciaMatch = Regex.Match(texto, @"(?:VIGENCIA|VÁLIDA HASTA|VÁLIDA)[\s:]+([0-9]{1,2}[/\-][0-9]{1,2}[/\-][0-9]{2,4})", RegexOptions.IgnoreCase);
            if (vigenciaMatch.Success)
                resultado.FechaVigencia = vigenciaMatch.Groups[1].Value;

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al extraer datos por texto");
        }

        return resultado;
    }

    private void ConvertirDatosINEACrudDto(DatosINE datos, CrudDto dto)
    {
        dto["nombre"] = datos.Nombre;
        dto["apellidoPaterno"] = datos.ApellidoPaterno;
        dto["apellidoMaterno"] = datos.ApellidoMaterno;
        dto["curp"] = datos.CURP;
        dto["claveElector"] = datos.ClaveElector;
        dto["numeroIdentificacion"] = datos.NumeroIdentificacion;
        dto["fechaNacimiento"] = datos.FechaNacimiento;
        dto["sexo"] = datos.Sexo;
        dto["domicilio"] = datos.Domicilio;
        dto["estado"] = datos.Estado;
        dto["municipio"] = datos.Municipio;
        dto["vigencia"] = datos.Vigencia;
    }

    private void ConvertirDatosTarjetaACrudDto(DatosTarjetaCirculacion datos, CrudDto dto)
    {
        dto["placas"] = datos.Placas;
        dto["numeroTarjeta"] = datos.NumeroTarjeta;
        dto["marca"] = datos.Marca;
        dto["modelo"] = datos.Modelo;
        dto["anio"] = datos.Anio;
        dto["color"] = datos.Color;
        dto["numeroMotor"] = datos.NumeroMotor;
        dto["numeroSerie"] = datos.NumeroSerie;
        dto["tipoCombustible"] = datos.TipoCombustible;
        dto["capacidadPasajeros"] = datos.CapacidadPasajeros;
        dto["usoVehiculo"] = datos.UsoVehiculo;
        dto["propietarioRegistrado"] = datos.PropietarioRegistrado;
        dto["fechaExpedicion"] = datos.FechaExpedicion;
        dto["fechaVigencia"] = datos.FechaVigencia;
    }
}
