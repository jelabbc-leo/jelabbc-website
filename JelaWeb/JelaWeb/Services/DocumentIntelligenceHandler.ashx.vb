Imports System.Web
Imports System.IO
Imports System.Collections.Generic
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports System.Configuration
Imports System.Net.Http
Imports System.Net.Http.Headers
Imports System.Threading.Tasks

''' <summary>
''' Handler para procesar imágenes de INE con Azure Document Intelligence
''' MIGRADO: Ahora usa DocumentIntelligenceProxy que llama a JELA.API
''' </summary>
Public Class DocumentIntelligenceHandler
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
            Dim reader As New StreamReader(context.Request.InputStream)
            Dim requestBody = reader.ReadToEnd()

            If String.IsNullOrWhiteSpace(requestBody) Then
                EnviarError(context, "No se recibió ninguna imagen", 400)
                Return
            End If

            ' Parsear JSON
            Dim requestData As Dictionary(Of String, String) = Nothing

            Try
                requestData = JsonConvert.DeserializeObject(Of Dictionary(Of String, String))(requestBody)

            Catch jsonEx As Exception
                Logger.LogError("Error al parsear JSON en DocumentIntelligenceHandler: " & jsonEx.Message, jsonEx, "")
                EnviarError(context, "Error al parsear la petición JSON: " & jsonEx.Message, 400)
                Return
            End Try

            If requestData Is Nothing OrElse Not requestData.ContainsKey("imagen") Then
                EnviarError(context, "Formato de petición inválido. Se requiere el campo 'imagen'", 400)
                Return
            End If

            Dim imagenBase64 = requestData("imagen")
            Dim tipoDocumento = If(requestData.ContainsKey("tipoDocumento"), requestData("tipoDocumento"), "INE")

            If String.IsNullOrWhiteSpace(imagenBase64) Then
                EnviarError(context, "La imagen está vacía", 400)
                Return
            End If

            ' Convertir base64 a bytes y crear archivo en memoria
            Dim base64Limpio = imagenBase64
            Dim contentType As String = "application/octet-stream"
            Dim extension As String = ".jpg"

            ' Extraer tipo MIME y limpiar base64
            If base64Limpio.Contains(",") Then
                Dim dataUrlParts = base64Limpio.Split(","c)
                Dim prefix = dataUrlParts(0).ToLower()

                base64Limpio = dataUrlParts(1)

                ' Detectar Content-Type
                If prefix.Contains("application/pdf") Then
                    contentType = "application/pdf"
                    extension = ".pdf"
                ElseIf prefix.Contains("image/jpeg") Or prefix.Contains("image/jpg") Then
                    contentType = "image/jpeg"
                    extension = ".jpg"
                ElseIf prefix.Contains("image/png") Then
                    contentType = "image/png"
                    extension = ".png"
                End If
            End If

            ' Convertir base64 a bytes
            Dim imageBytes As Byte() = Nothing
            Try
                imageBytes = Convert.FromBase64String(base64Limpio)
            Catch base64Ex As Exception
                Logger.LogError("Error al convertir base64 a bytes: " & base64Ex.Message, base64Ex, "")
                EnviarError(context, "Error al procesar la imagen base64", 400)
                Return
            End Try

            ' Llamar directamente a JELA.API (igual que DocumentIntelligenceProxy)
            Dim apiBaseUrl = ConfigurationManager.AppSettings("ApiBaseUrl")
            If String.IsNullOrEmpty(apiBaseUrl) Then
                EnviarError(context, "ApiBaseUrl no está configurado en Web.config", 500)
                Return
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
                    EnviarError(context, "ApiBaseUrl no tiene formato válido en Web.config", 500)
                    Return
                End If
            End If

            ' Construir URL del endpoint
            Dim apiEndpoint = $"{apiUrlBase}/api/document-intelligence"

            ' Obtener token JWT
            Dim token = JwtTokenService.Instance.GetToken()
            If String.IsNullOrEmpty(token) Then
                EnviarError(context, "No se pudo obtener token de autenticación. Por favor, cierre sesión y vuelva a ingresar.", 401)
                Return
            End If

            ' Crear multipart/form-data
            Dim httpClient = HttpClientHelper.Client
            Dim multipartContent = New MultipartFormDataContent()

            ' Agregar archivo
            Dim fileContent = New ByteArrayContent(imageBytes)
            fileContent.Headers.ContentType = New MediaTypeHeaderValue(contentType)
            Dim fileName = $"documento{extension}"
            multipartContent.Add(fileContent, "archivo", fileName)

            ' Agregar tipo de documento (convertir formato si es necesario)
            Dim tipoDocParaAPI = If(tipoDocumento.ToUpper() = "TARJETA_CIRCULACION", "TarjetaCirculacion", "INE")
            multipartContent.Add(New StringContent(tipoDocParaAPI), "tipoDocumento")

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
                JwtTokenService.Instance.ClearTokens()
                EnviarError(context, "Sesión expirada. Por favor, cierre sesión y vuelva a ingresar.", 401)
                Return
            End If

            ' La API devuelve CrudDto, pero el handler espera el formato antiguo
            ' Convertir respuesta de CrudDto al formato esperado por el cliente
            Try
                Dim apiResponse = JsonConvert.DeserializeObject(Of Dictionary(Of String, Object))(contenido)
                
                ' Si la API devuelve el formato CrudDto, convertirlo al formato antiguo
                If apiResponse IsNot Nothing AndAlso apiResponse.ContainsKey("data") Then
                    Dim data = apiResponse("data")
                    Dim response As New Dictionary(Of String, Object)
                    
                    ' Extraer exito y mensaje
                    response.Add("exito", If(apiResponse.ContainsKey("exito"), apiResponse("exito"), False))
                    response.Add("mensaje", If(apiResponse.ContainsKey("mensaje"), apiResponse("mensaje"), ""))
                    
                    ' Convertir datos al formato esperado
                    If data IsNot Nothing Then
                        Dim datosDict As Dictionary(Of String, Object) = Nothing
                        If TypeOf data Is Dictionary(Of String, Object) Then
                            datosDict = CType(data, Dictionary(Of String, Object))
                        ElseIf TypeOf data Is JObject Then
                            datosDict = JsonConvert.DeserializeObject(Of Dictionary(Of String, Object))(data.ToString())
                        End If
                        
                        If datosDict IsNot Nothing Then
                            response.Add("datos", datosDict)
                        End If
                    End If
                    
                    context.Response.StatusCode = CInt(respuesta.StatusCode)
                    context.Response.Write(JsonConvert.SerializeObject(response))
                Else
                    ' Si ya viene en el formato correcto, enviarlo tal cual
                    context.Response.StatusCode = CInt(respuesta.StatusCode)
                    context.Response.Write(contenido)
                End If
            Catch convEx As Exception
                ' Si falla la conversión, enviar respuesta original
                Logger.LogWarning("Error al convertir respuesta de API: " & convEx.Message, "")
                context.Response.StatusCode = CInt(respuesta.StatusCode)
                context.Response.Write(contenido)
            End Try

        Catch ex As Exception
            Dim errorDetails = String.Format("Error en DocumentIntelligenceHandler. Message: {0}, StackTrace: {1}", 
                                             ex.Message, 
                                             If(ex.StackTrace IsNot Nothing, ex.StackTrace, "N/A"))
            Logger.LogError(errorDetails, ex, "")

            Dim errorMessage As String = "Error interno del servidor"
            If ex.InnerException IsNot Nothing Then
                errorMessage += ": " & ex.InnerException.Message
            Else
                errorMessage += ": " & ex.Message
            End If

            EnviarError(context, errorMessage, 500)
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
