Imports System.Collections.Generic
Imports System.Configuration
Imports System.Text
Imports System.Web
Imports System.Net.Http
Imports System.Net.Http.Headers
Imports System.Threading.Tasks
Imports Newtonsoft.Json
Imports JelaWeb.Services.API

Namespace Business.Operacion
    ''' <summary>
    ''' Clase de negocio para el procesamiento de tickets con IA
    ''' Módulo 07 - Tickets tipo Klarna + IA
    ''' MIGRADO: Ahora usa OpenAIProxy que llama a JELA.API
    ''' </summary>
    Public Class TicketsBusiness

    Private apiConsumer As ApiConsumer
    Private apiBaseUrl As String
    Private Const CACHE_KEY_PROMPTS As String = "Cache_TicketPrompts"
    Private Const CACHE_MINUTES_PROMPTS As Integer = 60 ' Los prompts se cachean por 60 minutos

    Public Sub New()
        apiConsumer = New Global.JelaWeb.ApiConsumer()
        apiBaseUrl = ConfigurationManager.AppSettings("ApiBaseUrl")
    End Sub

    ''' <summary>
    ''' Procesa un ticket con IA al momento de crearlo
    ''' Analiza el mensaje y genera: Resumen, Asunto, Categoría, Sentimiento, Prioridad, Urgencia
    ''' </summary>
    Public Async Function ProcesarTicketConIAAsync(mensaje As String) As Task(Of Dictionary(Of String, Object))

        Try
            If String.IsNullOrWhiteSpace(mensaje) Then
                Throw New ArgumentException("El mensaje no puede estar vacío")
            End If

            Dim prompt As String = ConstruirPromptAnalisis(mensaje)
            Dim respuestaIA As String = Await GenerarRespuestaConAPIAsync(prompt)

            ' Parsear respuesta JSON de la IA
            Dim resultado = ParsearRespuestaIA(respuestaIA)

            ' Validar y completar campos requeridos
            ValidarYCompletarResultado(resultado)

            Return resultado

        Catch ex As Exception
            Global.JelaWeb.Logger.LogError("Error al procesar ticket con IA: " & ex.Message, ex, "")
            ' Retornar valores por defecto en caso de error
            Return CrearResultadoPorDefecto()

        End Try
    End Function

    ''' <summary>
    ''' Resuelve un ticket con IA generando una respuesta automática
    ''' </summary>
    Public Async Function ResolverTicketConIAAsync(idTicket As Integer, mensajeOriginal As String) As Task(Of Dictionary(Of String, Object))

        Try
            If String.IsNullOrWhiteSpace(mensajeOriginal) Then
                Throw New ArgumentException("El mensaje original no puede estar vacío")
            End If

            Dim prompt As String = ConstruirPromptResolucion(mensajeOriginal)
            Dim respuestaIA As String = Await GenerarRespuestaConAPIAsync(prompt)

            ' Parsear respuesta JSON de la IA
            Dim resultado = ParsearRespuestaResolucion(respuestaIA)

            ' Validar respuesta
            If Not resultado.ContainsKey("Respuesta") OrElse resultado("Respuesta") Is Nothing OrElse String.IsNullOrWhiteSpace(resultado("Respuesta").ToString()) Then
                Throw New Exception("La IA no pudo generar una respuesta válida")
            End If

            Return resultado

        Catch ex As Exception
            Global.JelaWeb.Logger.LogError("Error al resolver ticket con IA: " & ex.Message, ex, "")
            Throw

        End Try
    End Function

    ''' <summary>
    ''' Asigna un ticket a un agente basándose en el análisis de IA
    ''' </summary>
    Public Function AsignarTicketAAgente(idTicket As Integer, resultadoIA As Dictionary(Of String, Object)) As Boolean

        Try
            ' Lógica de asignación basada en prioridad, categoría, etc.
            ' Por ahora, retornamos True (la asignación se hace manualmente desde la UI)
            Return True

        Catch ex As Exception
            Global.JelaWeb.Logger.LogError("Error al asignar ticket a agente: " & ex.Message, ex, "")
            Return False

        End Try
    End Function

    ''' <summary>
    ''' Construye el prompt para análisis inicial del ticket
    ''' Lee el prompt exclusivamente desde la base de datos usando la API dinámica
    ''' </summary>
    Private Function ConstruirPromptAnalisis(mensaje As String) As String
        ' Obtener el prompt desde la base de datos
        Dim promptTemplate As String = ObtenerPromptDesdeBD("AnalisisTicket")

        ' Validar que se obtuvo el prompt
        If String.IsNullOrWhiteSpace(promptTemplate) Then
            Throw New Exception("No se pudo obtener el prompt 'AnalisisTicket' desde la base de datos. Verifique que existe en la tabla conf_ticket_prompts y que está activo.")
        End If

        ' Reemplazar el placeholder {MENSAJE} con el mensaje real
        Return promptTemplate.Replace("{MENSAJE}", mensaje)
    End Function

    ''' <summary>
    ''' Construye el prompt para resolución del ticket
    ''' Lee el prompt exclusivamente desde la base de datos usando la API dinámica
    ''' </summary>
    Private Function ConstruirPromptResolucion(mensajeOriginal As String) As String
        ' Obtener el prompt desde la base de datos
        Dim promptTemplate As String = ObtenerPromptDesdeBD("ResolucionTicket")

        ' Validar que se obtuvo el prompt
        If String.IsNullOrWhiteSpace(promptTemplate) Then
            Throw New Exception("No se pudo obtener el prompt 'ResolucionTicket' desde la base de datos. Verifique que existe en la tabla conf_ticket_prompts y que está activo.")
        End If

        ' Reemplazar el placeholder {MENSAJE} con el mensaje real
        Return promptTemplate.Replace("{MENSAJE}", mensajeOriginal)
    End Function

    ''' <summary>
    ''' Parsea la respuesta JSON de la IA para análisis
    ''' </summary>
    Private Function ParsearRespuestaIA(respuestaIA As String) As Dictionary(Of String, Object)

        Try
            ' Limpiar respuesta (remover markdown code blocks si existen)
            Dim respuestaLimpia = respuestaIA.Trim()

            If respuestaLimpia.StartsWith("```") Then
                Dim inicioJson = respuestaLimpia.IndexOf("{"c)
                Dim finJson = respuestaLimpia.LastIndexOf("}"c)

                If inicioJson >= 0 AndAlso finJson > inicioJson Then
                    respuestaLimpia = respuestaLimpia.Substring(inicioJson, finJson - inicioJson + 1)
                End If
            End If

            ' Deserializar JSON
            Dim resultado = Newtonsoft.Json.JsonConvert.DeserializeObject(Of Dictionary(Of String, Object))(respuestaLimpia)

            If resultado Is Nothing Then
                Return CrearResultadoPorDefecto()
            End If

            Return resultado

        Catch ex As Exception
            Global.JelaWeb.Logger.LogError("Error al parsear respuesta IA: " & ex.Message, ex, "")
            Return CrearResultadoPorDefecto()

        End Try
    End Function

    ''' <summary>
    ''' Parsea la respuesta JSON de la IA para resolución
    ''' </summary>
    Private Function ParsearRespuestaResolucion(respuestaIA As String) As Dictionary(Of String, Object)

        Try
            ' Limpiar respuesta (remover markdown code blocks si existen)
            Dim respuestaLimpia = respuestaIA.Trim()

            If respuestaLimpia.StartsWith("```") Then
                Dim inicioJson = respuestaLimpia.IndexOf("{"c)
                Dim finJson = respuestaLimpia.LastIndexOf("}"c)

                If inicioJson >= 0 AndAlso finJson > inicioJson Then
                    respuestaLimpia = respuestaLimpia.Substring(inicioJson, finJson - inicioJson + 1)
                End If
            End If

            ' Deserializar JSON
            Dim resultado = Newtonsoft.Json.JsonConvert.DeserializeObject(Of Dictionary(Of String, Object))(respuestaLimpia)

            If resultado Is Nothing Then
                Throw New Exception("No se pudo parsear la respuesta de la IA")
            End If

            Return resultado

        Catch ex As Exception
            Global.JelaWeb.Logger.LogError("Error al parsear respuesta de resolución IA: " & ex.Message, ex, "")
            Throw

        End Try
    End Function

    ''' <summary>
    ''' Valida y completa el resultado con valores por defecto si faltan campos
    ''' </summary>
    Private Sub ValidarYCompletarResultado(resultado As Dictionary(Of String, Object))
        ' Validar y completar campos requeridos
        If Not resultado.ContainsKey("Resumen") OrElse resultado("Resumen") Is Nothing OrElse String.IsNullOrWhiteSpace(resultado("Resumen").ToString()) Then
            resultado("Resumen") = "Análisis no disponible"
        End If

        If Not resultado.ContainsKey("AsuntoCorto") OrElse resultado("AsuntoCorto") Is Nothing OrElse String.IsNullOrWhiteSpace(resultado("AsuntoCorto").ToString()) Then
            resultado("AsuntoCorto") = "Consulta del cliente"
        Else
            ' Limitar a 50 caracteres
            Dim asunto = If(resultado("AsuntoCorto") IsNot Nothing, resultado("AsuntoCorto").ToString(), "")

            If asunto.Length > 50 Then
                resultado("AsuntoCorto") = asunto.Substring(0, 47) & "..."
            End If
        End If

        If Not resultado.ContainsKey("Categoria") OrElse resultado("Categoria") Is Nothing OrElse String.IsNullOrWhiteSpace(resultado("Categoria").ToString()) Then
            resultado("Categoria") = "Otro"
        End If

        If Not resultado.ContainsKey("Subcategoria") OrElse resultado("Subcategoria") Is Nothing OrElse String.IsNullOrWhiteSpace(resultado("Subcategoria").ToString()) Then
            resultado("Subcategoria") = "General"
        End If

        If Not resultado.ContainsKey("Sentimiento") OrElse resultado("Sentimiento") Is Nothing OrElse String.IsNullOrWhiteSpace(resultado("Sentimiento").ToString()) Then
            resultado("Sentimiento") = "Neutral"
        End If

        If Not resultado.ContainsKey("Prioridad") OrElse resultado("Prioridad") Is Nothing OrElse String.IsNullOrWhiteSpace(resultado("Prioridad").ToString()) Then
            resultado("Prioridad") = "Media"
        End If

        If Not resultado.ContainsKey("Urgencia") OrElse resultado("Urgencia") Is Nothing OrElse String.IsNullOrWhiteSpace(resultado("Urgencia").ToString()) Then
            resultado("Urgencia") = "Media"
        End If

        If Not resultado.ContainsKey("PuedeResolverIA") Then
            resultado("PuedeResolverIA") = False
        Else
            ' Asegurar que sea booleano
            Dim puedeResolver = resultado("PuedeResolverIA")

            If TypeOf puedeResolver Is String Then
                resultado("PuedeResolverIA") = Boolean.Parse(puedeResolver.ToString())
            ElseIf TypeOf puedeResolver Is Boolean Then
                ' Ya es booleano, no hacer nada
            Else
                resultado("PuedeResolverIA") = False
            End If
        End If
    End Sub

    ''' <summary>
    ''' Crea un resultado por defecto cuando hay error en el procesamiento
    ''' </summary>
    Private Function CrearResultadoPorDefecto() As Dictionary(Of String, Object)
        Return New Dictionary(Of String, Object) From {
            {"Resumen", "No se pudo analizar el mensaje automáticamente"},
            {"AsuntoCorto", "Consulta del cliente"},
            {"Categoria", "Otro"},
            {"Subcategoria", "General"},
            {"Sentimiento", "Neutral"},
            {"Prioridad", "Media"},
            {"Urgencia", "Media"},
            {"PuedeResolverIA", False}
        }
    End Function

    ''' <summary>
    ''' Obtiene un prompt desde la base de datos usando la API dinámica
    ''' Utiliza caché para mejorar el rendimiento
    ''' </summary>
    Private Function ObtenerPromptDesdeBD(nombrePrompt As String) As String
        ' Validar configuración
        If String.IsNullOrWhiteSpace(apiBaseUrl) Then
            Global.JelaWeb.Logger.LogError("ApiBaseUrl no está configurado en Web.config", Nothing, "")
            Throw New Exception("La configuración de la API no está disponible. Verifique Web.config.")
        End If

        ' Verificar caché primero
        Dim cacheKey As String = CACHE_KEY_PROMPTS & "_" & nombrePrompt
        Dim promptCacheado As String = Global.JelaWeb.CacheHelper.GetValue(Of String)(cacheKey)

        If Not String.IsNullOrWhiteSpace(promptCacheado) Then
            Return promptCacheado
        End If

        Try
            ' Construir query SQL (escapar comillas simples para prevenir SQL injection)
            Dim nombrePromptEscapado = nombrePrompt.Replace("'", "''")
            Dim query As String = $"SELECT ContenidoPrompt FROM conf_ticket_prompts WHERE NombrePrompt = '{nombrePromptEscapado}' AND Activo = 1 LIMIT 1"
            Dim url As String = apiBaseUrl & System.Web.HttpUtility.UrlEncode(query)

            ' Obtener datos desde la API
            Dim datos = apiConsumer.ObtenerDatos(url)

            ' Validar que se obtuvieron datos
            If datos Is Nothing OrElse datos.Count = 0 Then
                Global.JelaWeb.Logger.LogError($"No se encontró el prompt '{nombrePrompt}' en la base de datos o no está activo.", Nothing, "")
                Return String.Empty
            End If

            ' Extraer el prompt del primer registro
            Dim primerRegistro = datos(0)

            If primerRegistro Is Nothing Then
                Global.JelaWeb.Logger.LogError($"El registro del prompt '{nombrePrompt}' está vacío.", Nothing, "")
                Return String.Empty
            End If

            ' Verificar que existe el campo ContenidoPrompt
            If Not primerRegistro.Campos.ContainsKey("ContenidoPrompt") Then
                Global.JelaWeb.Logger.LogError($"El campo 'ContenidoPrompt' no existe en el registro del prompt '{nombrePrompt}'.", Nothing, "")
                Return String.Empty
            End If

            ' Obtener el valor del prompt
            Dim campoPrompt = primerRegistro("ContenidoPrompt")

            If campoPrompt Is Nothing Then
                Global.JelaWeb.Logger.LogError($"El contenido del prompt '{nombrePrompt}' es nulo.", Nothing, "")
                Return String.Empty
            End If

            Dim prompt As String = campoPrompt.ToString()

            ' Validar que el prompt no esté vacío
            If String.IsNullOrWhiteSpace(prompt) Then
                Global.JelaWeb.Logger.LogError($"El contenido del prompt '{nombrePrompt}' está vacío.", Nothing, "")
                Return String.Empty
            End If

            ' Guardar en caché para próximas consultas
            Global.JelaWeb.CacheHelper.SetValue(cacheKey, prompt, TimeSpan.FromMinutes(CACHE_MINUTES_PROMPTS))

            Return prompt

        Catch ex As Exception
            Global.JelaWeb.Logger.LogError($"Error al obtener prompt '{nombrePrompt}' desde BD: " & ex.Message, ex, "")
            Throw New Exception($"Error al obtener el prompt '{nombrePrompt}' desde la base de datos: " & ex.Message, ex)

        End Try
    End Function

    ''' <summary>
    ''' Limpia el caché de prompts (útil cuando se actualizan los prompts en la BD)
    ''' </summary>
    ''' <param name="nombrePrompt">Nombre del prompt a limpiar. Si es Nothing o vacío, limpia todos los prompts.</param>
    Public Sub LimpiarCachePrompts(Optional nombrePrompt As String = Nothing)

        Try
            If String.IsNullOrWhiteSpace(nombrePrompt) Then
                ' Limpiar todos los prompts conocidos
                Global.JelaWeb.CacheHelper.Remove(CACHE_KEY_PROMPTS & "_AnalisisTicket")
                Global.JelaWeb.CacheHelper.Remove(CACHE_KEY_PROMPTS & "_ResolucionTicket")
            Else
                ' Limpiar solo el prompt especificado
                Global.JelaWeb.CacheHelper.Remove(CACHE_KEY_PROMPTS & "_" & nombrePrompt)
            End If

        Catch ex As Exception
            Global.JelaWeb.Logger.LogError("Error al limpiar caché de prompts: " & ex.Message, ex, "")

        End Try
    End Sub

    ''' <summary>
    ''' Genera una respuesta usando OpenAI a través de JELA.API
    ''' </summary>
    Private Async Function GenerarRespuestaConAPIAsync(prompt As String, Optional systemMessage As String = Nothing, Optional temperature As Double = 0.7, Optional maxTokens As Integer = 1000) As Task(Of String)
        Try
            ' Obtener URL base de la API
            If String.IsNullOrWhiteSpace(apiBaseUrl) Then
                Throw New Exception("ApiBaseUrl no está configurado en Web.config")
            End If

            ' Extraer URL base
            Dim idx = apiBaseUrl.IndexOf("/api/")
            Dim apiUrlBase As String
            If idx > 0 Then
                apiUrlBase = apiBaseUrl.Substring(0, idx)
            Else
                Dim queryIdx = apiBaseUrl.IndexOf("?")
                If queryIdx > 0 Then
                    apiUrlBase = apiBaseUrl.Substring(0, queryIdx)
                    If apiUrlBase.EndsWith("/CRUD") Then
                        apiUrlBase = apiUrlBase.Substring(0, apiUrlBase.Length - 5)
                    End If
                Else
                    Throw New Exception("ApiBaseUrl no tiene formato válido en Web.config")
                End If
            End If

            ' Construir URL del endpoint
            Dim apiEndpoint = $"{apiUrlBase}/api/openai"

            ' Obtener token JWT
            Dim token = JwtTokenService.Instance.GetToken()
            If String.IsNullOrEmpty(token) Then
                Throw New Exception("No se pudo obtener token de autenticación")
            End If

            ' Crear request body
            Dim requestBody As New Dictionary(Of String, Object)
            requestBody.Add("Prompt", prompt)
            If Not String.IsNullOrWhiteSpace(systemMessage) Then
                requestBody.Add("SystemMessage", systemMessage)
            End If
            requestBody.Add("Temperature", temperature)
            requestBody.Add("MaxTokens", maxTokens)

            ' Serializar request
            Dim jsonContent = JsonConvert.SerializeObject(requestBody)
            Dim content = New StringContent(jsonContent, Encoding.UTF8, "application/json")

            ' Crear request con token JWT
            Dim httpClient = HttpClientHelper.Client
            Dim request = New HttpRequestMessage(HttpMethod.Post, apiEndpoint)
            request.Headers.Authorization = New AuthenticationHeaderValue("Bearer", token)
            request.Content = content

            ' Enviar petición
            Dim httpResponse = Await httpClient.SendAsync(request).ConfigureAwait(False)

            Dim contenido = Await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(False)

            ' Validar respuesta
            If Not httpResponse.IsSuccessStatusCode Then
                If httpResponse.StatusCode = System.Net.HttpStatusCode.Unauthorized Then
                    JwtTokenService.Instance.ClearTokens()
                    Throw New Exception("Sesión expirada. Por favor, cierre sesión y vuelva a ingresar.")
                End If
                Throw New Exception($"Error al comunicarse con OpenAI: {httpResponse.StatusCode}")
            End If

            ' Parsear respuesta (viene en formato CrudDto - List<DynamicDto>)
            ' La API retorna List<CrudDto> que es compatible con List<DynamicDto>
            Dim apiResponse = JsonConvert.DeserializeObject(Of List(Of DynamicDto))(contenido)
            If apiResponse Is Nothing OrElse apiResponse.Count = 0 Then
                Throw New Exception("Respuesta inválida de la API")
            End If

            ' Extraer la respuesta del DynamicDto (equivalente a CrudDto)
            Dim dto = apiResponse(0)
            
            ' Usar el indexer de DynamicDto para acceder directamente al valor
            Dim respuestaTexto = dto("Respuesta")
            If respuestaTexto IsNot Nothing Then
                Return respuestaTexto.ToString()
            End If

            Throw New Exception("No se pudo extraer la respuesta de OpenAI")

        Catch ex As Exception
            Global.JelaWeb.Logger.LogError("Error al generar respuesta con OpenAI API: " & ex.Message, ex, "")
            Throw
        End Try
    End Function

    ''' <summary>
    ''' Versión síncrona para compatibilidad (usa la versión async internamente)
    ''' </summary>
    Public Function ProcesarTicketConIA(mensaje As String) As Dictionary(Of String, Object)
        Try
            Return Task.Run(Function() ProcesarTicketConIAAsync(mensaje)).Result
        Catch ex As AggregateException
            Throw ex.InnerException
        End Try
    End Function

    ''' <summary>
    ''' Versión síncrona para compatibilidad (usa la versión async internamente)
    ''' </summary>
    Public Function ResolverTicketConIA(idTicket As Integer, mensajeOriginal As String) As Dictionary(Of String, Object)
        Try
            Return Task.Run(Function() ResolverTicketConIAAsync(idTicket, mensajeOriginal)).Result
        Catch ex As AggregateException
            Throw ex.InnerException
        End Try
    End Function

    End Class
End Namespace
