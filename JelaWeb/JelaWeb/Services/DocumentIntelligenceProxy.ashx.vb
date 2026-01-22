Imports System.Web
Imports System.Net.Http
Imports System.Net.Http.Headers
Imports System.IO
Imports System.Threading.Tasks
Imports System.Collections.Generic
Imports Newtonsoft.Json

''' <summary>
''' Proxy para Document Intelligence que reenvía peticiones a JELA.API con autenticación JWT
''' Recibe multipart/form-data con archivo y lo reenvía directamente a JELA.API
''' </summary>
Public Class DocumentIntelligenceProxy
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

            ' Validar que sea multipart/form-data
            Dim contentType = context.Request.ContentType
            If String.IsNullOrEmpty(contentType) OrElse Not contentType.Contains("multipart/form-data") Then
                EnviarError(context, "El contenido debe ser multipart/form-data", 400)
                Return
            End If

            ' Obtener archivo del request
            Dim archivo = context.Request.Files("archivo")

            If archivo Is Nothing OrElse archivo.ContentLength = 0 Then
                EnviarError(context, "Se requiere un archivo en el campo 'archivo'", 400)
                Return
            End If

            ' Validar tipo de archivo
            Dim extension = IO.Path.GetExtension(archivo.FileName).ToLower()
            Dim archivoContentType = If(String.IsNullOrEmpty(archivo.ContentType), "application/octet-stream", archivo.ContentType)

            If Not EsTipoArchivoValido(extension, archivoContentType) Then
                EnviarError(context, "El archivo debe ser PDF, JPG o PNG", 400)
                Return
            End If

            ' Obtener tipo de documento (opcional, por defecto INE)
            Dim tipoDocumento = context.Request.Form("tipoDocumento")
            If String.IsNullOrWhiteSpace(tipoDocumento) Then
                tipoDocumento = "INE"
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
                ' Si no se encuentra /api/, usar la URL completa sin el query string
                Dim queryIdx = apiBaseUrl.IndexOf("?")
                If queryIdx > 0 Then
                    apiUrlBase = apiBaseUrl.Substring(0, queryIdx)
                    ' Quitar /CRUD si está presente
                    If apiUrlBase.EndsWith("/CRUD") Then
                        apiUrlBase = apiUrlBase.Substring(0, apiUrlBase.Length - 5)
                    End If
                Else
                    EnviarError(context, "ApiBaseUrl no tiene formato válido en Web.config", 500)
                    Return
                End If
            End If

            ' Construir URL del endpoint de Document Intelligence
            Dim apiEndpoint = $"{apiUrlBase}/api/document-intelligence"

            ' Obtener token JWT
            Dim token = JwtTokenService.Instance.GetToken()

            If String.IsNullOrEmpty(token) Then
                ' Intentar autenticar desde sesión
                Dim currentUser = SessionHelper.GetCurrentUser()
                If currentUser IsNot Nothing Then
                    ' Si hay usuario en sesión pero no token, intentar autenticar
                    ' Nota: Esto requiere que el password esté disponible, lo cual no es seguro
                    ' Por ahora, retornar error
                    EnviarError(context, "No se pudo obtener token de autenticación. Por favor, cierre sesión y vuelva a ingresar.", 401)
                    Return
                Else
                    EnviarError(context, "Sesión no válida", 401)
                    Return
                End If
            End If

            ' Crear MultipartFormDataContent para reenviar el archivo
            Dim httpClient = HttpClientHelper.Client
            Dim multipartContent = New MultipartFormDataContent()

            ' Leer archivo en memoria
            Dim archivoBytes(archivo.ContentLength - 1) As Byte
            archivo.InputStream.Read(archivoBytes, 0, archivo.ContentLength)

            ' Agregar archivo al contenido
            Dim fileContent = New ByteArrayContent(archivoBytes)
            fileContent.Headers.ContentType = New MediaTypeHeaderValue(archivoContentType)
            multipartContent.Add(fileContent, "archivo", archivo.FileName)

            ' Agregar tipo de documento
            multipartContent.Add(New StringContent(tipoDocumento), "tipoDocumento")

            ' Crear request con token JWT
            Dim request = New HttpRequestMessage(HttpMethod.Post, apiEndpoint)
            request.Headers.Authorization = New AuthenticationHeaderValue("Bearer", token)
            request.Content = multipartContent

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
                ' Limpiar token y pedir nuevo login
                JwtTokenService.Instance.ClearTokens()
                EnviarError(context, "Sesión expirada. Por favor, cierre sesión y vuelva a ingresar.", 401)
                Return
            End If

            ' Retornar respuesta de la API (ya viene en formato CrudDto)
            context.Response.StatusCode = CInt(respuesta.StatusCode)
            context.Response.Write(contenido)

        Catch ex As Exception
            Logger.LogError("Error en DocumentIntelligenceProxy: " & ex.Message, ex, "")
            EnviarError(context, "Error interno del servidor: " & ex.Message, 500)
        End Try
    End Sub

    Private Function EsTipoArchivoValido(extension As String, contentType As String) As Boolean
        Dim extensionesValidas = {"pdf", "jpg", "jpeg", "png"}
        Dim tiposContenidoValidos = {"application/pdf", "image/jpeg", "image/jpg", "image/png"}

        Return extensionesValidas.Contains(extension.TrimStart("."c).ToLower()) OrElse
               tiposContenidoValidos.Contains(contentType.ToLower())
    End Function

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
