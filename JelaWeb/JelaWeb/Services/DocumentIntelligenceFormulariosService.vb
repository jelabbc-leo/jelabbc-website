Imports System.IO
Imports System.Net.Http
Imports System.Threading.Tasks
Imports Newtonsoft.Json.Linq

''' <summary>
''' Servicio para integración con Azure Document Intelligence
''' Extrae campos de PDFs SIN almacenar el PDF original
''' Para uso en formularios dinámicos
''' </summary>
Public Class DocumentIntelligenceFormulariosService
    Private ReadOnly _endpoint As String
    Private ReadOnly _apiKey As String
    Private ReadOnly _httpClient As HttpClient

    Public Sub New()
        _endpoint = ConfigurationManager.AppSettings("AzureDocIntelEndpoint")
        _apiKey = ConfigurationManager.AppSettings("AzureDocIntelKey")
        _httpClient = New HttpClient()
        _httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _apiKey)
    End Sub

    ''' <summary>
    ''' Extrae campos de un PDF usando Azure Document Intelligence
    ''' El PDF NO se almacena, solo se procesa para extracción
    ''' </summary>
    Public Async Function ExtraerCamposDePDF(archivo As Stream) As Task(Of List(Of CampoExtraidoDTO))
        Dim campos As New List(Of CampoExtraidoDTO)()

        Try
            ' Preparar request a Azure Document Intelligence
            Dim content As New StreamContent(archivo)

            content.Headers.ContentType = New Headers.MediaTypeHeaderValue("application/pdf")

            ' Llamar al endpoint de análisis de documentos
            Dim analyzeUrl As String = $"{_endpoint}/formrecognizer/documentModels/prebuilt-document:analyze?api-version=2023-07-31"
            Dim response = Await _httpClient.PostAsync(analyzeUrl, content)

            If response.IsSuccessStatusCode Then
                ' Obtener URL de operación para polling
                Dim operationLocation = response.Headers.GetValues("Operation-Location").FirstOrDefault()

                ' Esperar resultado (polling)
                Dim resultado = Await EsperarResultadoAnalisis(operationLocation)

                ' Procesar campos detectados
                campos = ProcesarCamposDetectados(resultado)
            Else
                Dim errorContent = Await response.Content.ReadAsStringAsync()

                Logger.LogError("DocumentIntelligenceFormulariosService.ExtraerCamposDePDF", $"Error HTTP: {response.StatusCode} - {errorContent}")
                Throw New ApplicationException($"Error al procesar PDF: {response.StatusCode}")
            End If

        Catch ex As Exception
            Logger.LogError("DocumentIntelligenceFormulariosService.ExtraerCamposDePDF", ex.Message)
            Throw

        End Try
        Return campos
    End Function

    Private Async Function EsperarResultadoAnalisis(operationUrl As String) As Task(Of JObject)
        Dim maxIntentos As Integer = 30
        Dim intentos As Integer = 0

        While intentos < maxIntentos

            Await Task.Delay(1000) ' Esperar 1 segundo entre intentos

            Dim response = Await _httpClient.GetAsync(operationUrl)
            Dim json = Await response.Content.ReadAsStringAsync()
            Dim resultado = JObject.Parse(json)
            Dim status = resultado("status")?.ToString()

            If status = "succeeded" Then
                Return resultado
            ElseIf status = "failed" Then
                Throw New Exception("Error en análisis de documento")
            End If

            intentos += 1
        End While

        Throw New TimeoutException("Timeout esperando análisis de documento")
    End Function

    Private Function ProcesarCamposDetectados(resultado As JObject) As List(Of CampoExtraidoDTO)
        Dim campos As New List(Of CampoExtraidoDTO)()
        Dim keyValuePairs = resultado.SelectTokens("$.analyzeResult.keyValuePairs[*]")

        For Each kvp In keyValuePairs
            Dim key = kvp("key")?("content")?.ToString()
            Dim value = kvp("value")?("content")?.ToString()

            If Not String.IsNullOrEmpty(key) Then
                Dim campo As New CampoExtraidoDTO() With {

                    .NombreCampo = LimpiarNombreCampo(key),
                    .EtiquetaCampo = key,
                    .TipoCampo = InferirTipoCampo(value),
                    .ValorEjemplo = value,
                    .Seccion = "General"
                }
                campos.Add(campo)
            End If

        Next

        Return campos
    End Function

    ''' <summary>
    ''' Limpia el nombre del campo para uso como identificador
    ''' </summary>
    Public Function LimpiarNombreCampo(nombre As String) As String
        Dim limpio = nombre.ToLower().Trim()

        limpio = System.Text.RegularExpressions.Regex.Replace(limpio, "[^a-z0-9]", "_")
        limpio = System.Text.RegularExpressions.Regex.Replace(limpio, "_+", "_")
        Return limpio.Trim("_"c)
    End Function

    ''' <summary>
    ''' Infiere el tipo de campo basado en el valor de ejemplo
    ''' </summary>
    Public Function InferirTipoCampo(valor As String) As String
        If String.IsNullOrEmpty(valor) Then Return "texto"

        ' Detectar fecha
        Dim fechaPatterns = {"^\d{1,2}/\d{1,2}/\d{2,4}$", "^\d{4}-\d{2}-\d{2}$"}

        For Each pattern In fechaPatterns
            If System.Text.RegularExpressions.Regex.IsMatch(valor, pattern) Then
                Return "fecha"
            End If

        Next

        ' Detectar número
        Dim numero As Decimal

        If Decimal.TryParse(valor.Replace(",", "").Replace("$", ""), numero) Then
            If valor.Contains(".") OrElse valor.Contains(",") Then
                Return "decimal"
            End If
            Return "numero"
        End If

        ' Detectar email
        If valor.Contains("@") AndAlso valor.Contains(".") Then
            Return "email"
        End If

        ' Detectar teléfono
        If System.Text.RegularExpressions.Regex.IsMatch(valor, "^\+?[\d\s\-\(\)]{7,}$") Then
            Return "telefono"
        End If

        ' Por defecto texto
        If valor.Length > 100 Then
            Return "textarea"
        End If

        Return "texto"
    End Function
End Class
