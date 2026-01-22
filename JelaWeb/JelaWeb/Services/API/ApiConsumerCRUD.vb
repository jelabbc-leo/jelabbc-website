Imports System.Net.Http
Imports System.Net.Http.Headers
Imports System.Text
Imports System.Threading.Tasks
Imports Newtonsoft.Json

Public Class ApiConsumerCRUD
    Private ReadOnly cliente As HttpClient

    Public Sub New()
        ' Usar HttpClientHelper para reutilizar la instancia
        cliente = HttpClientHelper.Client
    End Sub

    ''' <summary>
    ''' Crea un HttpRequestMessage con header de autorización JWT
    ''' </summary>
    Private Function CrearRequestConAuth(url As String, method As HttpMethod, Optional content As HttpContent = Nothing) As HttpRequestMessage
        Dim request = New HttpRequestMessage(method, url)
        Dim token = JwtTokenService.Instance.GetToken()

        If Not String.IsNullOrEmpty(token) Then
            request.Headers.Authorization = New AuthenticationHeaderValue("Bearer", token)
        End If

        If content IsNot Nothing Then
            request.Content = content
        End If

        Return request
    End Function

    Public Function EnviarPost(url As String, dto As DynamicDto) As Boolean
        Dim json = JsonConvert.SerializeObject(dto)
        Dim contenido = New StringContent(json, Encoding.UTF8, "application/json")
        Dim request = CrearRequestConAuth(url, HttpMethod.Post, contenido)

        ' Usar Task.Run para ejecutar en thread pool separado y evitar deadlocks
        Dim taskRespuesta = Task.Run(Async Function()
                                         Return Await cliente.SendAsync(request).ConfigureAwait(False)
                                     End Function)
        Dim respuesta = taskRespuesta.Result
        Dim taskCuerpo = Task.Run(Async Function()

                                      Return Await respuesta.Content.ReadAsStringAsync().ConfigureAwait(False)
                                  End Function)
        Dim cuerpo = taskCuerpo.Result

        If Not respuesta.IsSuccessStatusCode Then
            Dim mensaje = ApiErrorHelper.ExtraerMensaje(cuerpo)

            If String.IsNullOrWhiteSpace(mensaje) Then
                Throw New Exception("Error al enviar POST: " & respuesta.StatusCode & " - " & cuerpo)
            Else
                Throw New Exception("Error al enviar POST: " & mensaje & " (HTTP " & respuesta.StatusCode & ")")
            End If
        End If

        Return True
    End Function

    ''' <summary>
    ''' Versión asíncrona para enviar POST
    ''' </summary>
    Public Async Function EnviarPostAsync(url As String, dto As DynamicDto) As Task(Of Boolean)
        Dim json = JsonConvert.SerializeObject(dto)
        Dim contenido = New StringContent(json, Encoding.UTF8, "application/json")
        Dim request = CrearRequestConAuth(url, HttpMethod.Post, contenido)
        Dim respuesta = Await cliente.SendAsync(request)
        Dim cuerpo = Await respuesta.Content.ReadAsStringAsync()

        If Not respuesta.IsSuccessStatusCode Then
            Dim mensaje = ApiErrorHelper.ExtraerMensaje(cuerpo)

            If String.IsNullOrWhiteSpace(mensaje) Then
                Throw New Exception("Error al enviar POST: " & respuesta.StatusCode & " - " & cuerpo)
            Else
                Throw New Exception("Error al enviar POST: " & mensaje & " (HTTP " & respuesta.StatusCode & ")")
            End If
        End If

        Return True
    End Function

    Public Function EnviarPostId(url As String, dto As DynamicDto) As Integer
        Dim json = JsonConvert.SerializeObject(dto)
        Dim contenido = New StringContent(json, Encoding.UTF8, "application/json")
        Dim request = CrearRequestConAuth(url, HttpMethod.Post, contenido)

        ' Usar Task.Run para ejecutar en thread pool separado y evitar deadlocks
        Dim taskRespuesta = Task.Run(Async Function()
                                         Return Await cliente.SendAsync(request).ConfigureAwait(False)
                                     End Function)
        Dim respuesta = taskRespuesta.Result
        Dim taskCuerpo = Task.Run(Async Function()

                                      Return Await respuesta.Content.ReadAsStringAsync().ConfigureAwait(False)
                                  End Function)
        Dim cuerpo = taskCuerpo.Result

        If Not respuesta.IsSuccessStatusCode Then
            Dim mensaje = ApiErrorHelper.ExtraerMensaje(cuerpo)

            If String.IsNullOrWhiteSpace(mensaje) Then
                Throw New Exception("Error al enviar POST: " & respuesta.StatusCode & " - " & cuerpo)
            Else
                Throw New Exception("Error al enviar POST: " & mensaje & " (HTTP " & respuesta.StatusCode & ")")
            End If
        End If

        Dim resultado = JsonConvert.DeserializeObject(Of Dictionary(Of String, Object))(cuerpo)

        Return Convert.ToInt32(resultado("id"))

    End Function

    ''' <summary>
    ''' Versión asíncrona para enviar POST y obtener ID
    ''' </summary>
    Public Async Function EnviarPostIdAsync(url As String, dto As DynamicDto) As Task(Of Integer)
        Dim json = JsonConvert.SerializeObject(dto)
        Dim contenido = New StringContent(json, Encoding.UTF8, "application/json")
        Dim request = CrearRequestConAuth(url, HttpMethod.Post, contenido)
        Dim respuesta = Await cliente.SendAsync(request)
        Dim cuerpo = Await respuesta.Content.ReadAsStringAsync()

        If Not respuesta.IsSuccessStatusCode Then
            Dim mensaje = ApiErrorHelper.ExtraerMensaje(cuerpo)

            If String.IsNullOrWhiteSpace(mensaje) Then
                Throw New Exception("Error al enviar POST: " & respuesta.StatusCode & " - " & cuerpo)
            Else
                Throw New Exception("Error al enviar POST: " & mensaje & " (HTTP " & respuesta.StatusCode & ")")
            End If
        End If

        Dim resultado = JsonConvert.DeserializeObject(Of Dictionary(Of String, Object))(cuerpo)

        Return Convert.ToInt32(resultado("id"))
    End Function

    Public Function EnviarPut(url As String, dto As DynamicDto) As Boolean
        Dim json = JsonConvert.SerializeObject(dto)
        Dim contenido = New StringContent(json, Encoding.UTF8, "application/json")
        Dim request = CrearRequestConAuth(url, HttpMethod.Put, contenido)

        ' Usar Task.Run para ejecutar en thread pool separado y evitar deadlocks
        Dim taskRespuesta = Task.Run(Async Function()
                                         Return Await cliente.SendAsync(request).ConfigureAwait(False)
                                     End Function)
        Dim respuesta = taskRespuesta.Result
        Dim taskCuerpo = Task.Run(Async Function()

                                      Return Await respuesta.Content.ReadAsStringAsync().ConfigureAwait(False)
                                  End Function)
        Dim cuerpo = taskCuerpo.Result

        If Not respuesta.IsSuccessStatusCode Then
            Dim mensaje = ApiErrorHelper.ExtraerMensaje(cuerpo)

            If String.IsNullOrWhiteSpace(mensaje) Then
                Throw New Exception("Error PUT: " & respuesta.StatusCode & " - " & cuerpo)
            Else
                Throw New Exception("Error PUT: " & mensaje & " (HTTP " & respuesta.StatusCode & ")")
            End If
        End If

        Return True
    End Function

    ''' <summary>
    ''' Versión asíncrona para enviar PUT
    ''' </summary>
    Public Async Function EnviarPutAsync(url As String, dto As DynamicDto) As Task(Of Boolean)
        Dim json = JsonConvert.SerializeObject(dto)
        Dim contenido = New StringContent(json, Encoding.UTF8, "application/json")
        Dim request = CrearRequestConAuth(url, HttpMethod.Put, contenido)
        Dim respuesta = Await cliente.SendAsync(request)
        Dim cuerpo = Await respuesta.Content.ReadAsStringAsync()

        If Not respuesta.IsSuccessStatusCode Then
            Dim mensaje = ApiErrorHelper.ExtraerMensaje(cuerpo)

            If String.IsNullOrWhiteSpace(mensaje) Then
                Throw New Exception("Error PUT: " & respuesta.StatusCode & " - " & cuerpo)
            Else
                Throw New Exception("Error PUT: " & mensaje & " (HTTP " & respuesta.StatusCode & ")")
            End If
        End If

        Return True
    End Function

    ''' <summary>
    ''' Envía una solicitud DELETE a la URL especificada
    ''' </summary>
    Public Function EnviarDelete(url As String) As Boolean
        Dim request = CrearRequestConAuth(url, HttpMethod.Delete)

        ' Usar Task.Run para ejecutar en thread pool separado y evitar deadlocks
        Dim taskRespuesta = Task.Run(Async Function()
                                         Return Await cliente.SendAsync(request).ConfigureAwait(False)
                                     End Function)
        Dim respuesta = taskRespuesta.Result
        Dim taskCuerpo = Task.Run(Async Function()

                                      Return Await respuesta.Content.ReadAsStringAsync().ConfigureAwait(False)
                                  End Function)
        Dim cuerpo = taskCuerpo.Result

        If Not respuesta.IsSuccessStatusCode Then
            Dim mensaje = ApiErrorHelper.ExtraerMensaje(cuerpo)

            If String.IsNullOrWhiteSpace(mensaje) Then
                Throw New Exception("Error DELETE: " & respuesta.StatusCode & " - " & cuerpo)
            Else
                Throw New Exception("Error DELETE: " & mensaje & " (HTTP " & respuesta.StatusCode & ")")
            End If
        End If

        Return True
    End Function

    ''' <summary>
    ''' Versión asíncrona para enviar DELETE
    ''' </summary>
    Public Async Function EnviarDeleteAsync(url As String) As Task(Of Boolean)
        Dim request = CrearRequestConAuth(url, HttpMethod.Delete)
        Dim respuesta = Await cliente.SendAsync(request)
        Dim cuerpo = Await respuesta.Content.ReadAsStringAsync()

        If Not respuesta.IsSuccessStatusCode Then
            Dim mensaje = ApiErrorHelper.ExtraerMensaje(cuerpo)

            If String.IsNullOrWhiteSpace(mensaje) Then
                Throw New Exception("Error DELETE: " & respuesta.StatusCode & " - " & cuerpo)
            Else
                Throw New Exception("Error DELETE: " & mensaje & " (HTTP " & respuesta.StatusCode & ")")
            End If
        End If

        Return True
    End Function

End Class
