Imports System.Web
Imports System.Net.Http
Imports System.Net.Http.Headers
Imports System.Text
Imports System.Threading.Tasks
Imports System.Collections.Generic
Imports Newtonsoft.Json

''' <summary>
''' Proxy para OpenAI que reenvía peticiones a JELA.API con autenticación JWT
''' Recibe JSON con prompt y lo reenvía directamente a JELA.API
''' </summary>
Public Class OpenAIProxy
    Implements IHttpHandler

    Public Sub ProcessRequest(context As HttpContext) Implements IHttpHandler.ProcessRequest
        context.Response.ContentType = "application/json"
        context.Response.AddHeader("Cache-Control", "no-cache")
        context.Response.AddHeader("Access-Control-Allow-Origin", "*")

        Try
            ' Verificar que sea POST
            If context.Request.HttpMethod <> "POST" Then
                EnviarError(context, "Método no permitido", 405)
                Return
            End If

            ' Leer el cuerpo de la petición
            Dim reader As New System.IO.StreamReader(context.Request.InputStream)
            Dim requestBody = reader.ReadToEnd()

            If String.IsNullOrWhiteSpace(requestBody) Then
                EnviarError(context, "No se recibió ningún prompt", 400)
                Return
            End If

            ' Parsear JSON
            Dim requestData As Dictionary(Of String, Object) = Nothing

            Try
                requestData = JsonConvert.DeserializeObject(Of Dictionary(Of String, Object))(requestBody)
            Catch jsonEx As Exception
                Logger.LogError("Error al parsear JSON en OpenAIProxy: " & jsonEx.Message, jsonEx, "")
                EnviarError(context, "Error al parsear la petición JSON: " & jsonEx.Message, 400)
                Return
            End Try

            If requestData Is Nothing OrElse Not requestData.ContainsKey("Prompt") Then
                EnviarError(context, "Formato de petición inválido. Se requiere el campo 'Prompt'", 400)
                Return
            End If

            Dim prompt = requestData("Prompt")?.ToString()
            If String.IsNullOrWhiteSpace(prompt) Then
                EnviarError(context, "El prompt está vacío", 400)
                Return
            End If

            ' Obtener URL base de la API desde configuración
            Dim apiBaseUrl = ConfigurationManager.AppSettings("ApiBaseUrl")
            If String.IsNullOrEmpty(apiBaseUrl) Then
                EnviarError(context, "ApiBaseUrl no está configurado en Web.config", 500)
                Return
            End If

            ' Extraer URL base (quitar /api/CRUD?strQuery=)
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
                    EnviarError(context, "ApiBaseUrl no tiene formato válido en Web.config", 500)
                    Return
                End If
            End If

            ' Construir URL del endpoint de OpenAI
            Dim apiEndpoint = $"{apiUrlBase}/api/openai"

            ' Obtener token JWT
            Dim token = JwtTokenService.Instance.GetToken()
            If String.IsNullOrEmpty(token) Then
                EnviarError(context, "No se pudo obtener token de autenticación. Por favor, cierre sesión y vuelva a ingresar.", 401)
                Return
            End If

            ' Crear request body para la API
            Dim apiRequest As New Dictionary(Of String, Object)
            apiRequest.Add("Prompt", prompt)

            ' Agregar parámetros opcionales si existen
            If requestData.ContainsKey("SystemMessage") Then
                apiRequest.Add("SystemMessage", requestData("SystemMessage"))
            End If

            If requestData.ContainsKey("Temperature") Then
                Dim temp As Double
                If Double.TryParse(requestData("Temperature").ToString(), temp) Then
                    apiRequest.Add("Temperature", temp)
                End If
            End If

            If requestData.ContainsKey("MaxTokens") Then
                Dim maxTokens As Integer
                If Integer.TryParse(requestData("MaxTokens").ToString(), maxTokens) Then
                    apiRequest.Add("MaxTokens", maxTokens)
                End If
            End If

            ' Serializar request
            Dim jsonContent = JsonConvert.SerializeObject(apiRequest)
            Dim content = New StringContent(jsonContent, Encoding.UTF8, "application/json")

            ' Crear request con token JWT
            Dim httpClient = HttpClientHelper.Client
            Dim request = New HttpRequestMessage(HttpMethod.Post, apiEndpoint)
            request.Headers.Authorization = New AuthenticationHeaderValue("Bearer", token)
            request.Content = content

            ' Enviar petición a JELA.API
            Dim taskRespuesta = Task.Run(Async Function()
                                             Return Await httpClient.SendAsync(request).ConfigureAwait(False)
                                         End Function)
            Dim respuesta = taskRespuesta.Result

            Dim taskContenido = Task.Run(Async Function()
                                             Return Await respuesta.Content.ReadAsStringAsync().ConfigureAwait(False)
                                         End Function)
            Dim contenido = taskContenido.Result

            ' Si la respuesta es 401, el token puede haber expirado
            If respuesta.StatusCode = System.Net.HttpStatusCode.Unauthorized Then
                JwtTokenService.Instance.ClearTokens()
                EnviarError(context, "Sesión expirada. Por favor, cierre sesión y vuelva a ingresar.", 401)
                Return
            End If

            ' Retornar respuesta de la API (ya viene en formato CrudDto)
            context.Response.StatusCode = CInt(respuesta.StatusCode)
            context.Response.Write(contenido)

        Catch ex As Exception
            Logger.LogError("Error en OpenAIProxy: " & ex.Message, ex, "")
            EnviarError(context, "Error interno del servidor: " & ex.Message, 500)
        End Try
    End Sub

    Private Sub EnviarError(context As HttpContext, mensaje As String, statusCode As Integer)
        context.Response.StatusCode = statusCode
        Dim response As New Dictionary(Of String, Object)
        response.Add("exito", False)
        response.Add("mensaje", mensaje)
        context.Response.Write(JsonConvert.SerializeObject(response))
    End Sub

    Public ReadOnly Property IsReusable As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

End Class
