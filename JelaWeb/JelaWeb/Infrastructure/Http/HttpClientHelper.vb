Imports System.Net.Http
Imports System.Threading
Imports System.Threading.Tasks
Imports System.Text
Imports Newtonsoft.Json

''' <summary>
''' Clase helper para gestión centralizada de HttpClient
''' Evita problemas de socket exhaustion mediante reutilización
''' </summary>
Public NotInheritable Class HttpClientHelper
    Private Sub New()
        ' Clase estática, no instanciable
    End Sub

    ' Instancia estática compartida de HttpClient
    Private Shared ReadOnly _httpClient As Lazy(Of HttpClient) = New Lazy(Of HttpClient)(
        Function()
            Dim client As New HttpClient()

            ' Configurar timeout
            client.Timeout = TimeSpan.FromSeconds(Constants.API_TIMEOUT_SECONDS)
            ' Configurar headers por defecto
            client.DefaultRequestHeaders.Add("User-Agent", "JELA-WebApp/1.0")
            Return client
        End Function,
        LazyThreadSafetyMode.ExecutionAndPublication
    )

    ''' <summary>
    ''' Obtiene la instancia compartida de HttpClient
    ''' </summary>
    Public Shared ReadOnly Property Client As HttpClient
        Get
            Return _httpClient.Value
        End Get
    End Property

    ''' <summary>
    ''' Crea un nuevo HttpClient con configuración personalizada
    ''' Usar solo cuando se necesite configuración específica
    ''' </summary>
    Public Shared Function CreateClient(Optional timeoutSeconds As Integer = Constants.API_TIMEOUT_SECONDS) As HttpClient
        Dim client As New HttpClient()

        client.Timeout = TimeSpan.FromSeconds(timeoutSeconds)
        client.DefaultRequestHeaders.Add("User-Agent", "JELA-WebApp/1.0")
        Return client
    End Function

    ''' <summary>
    ''' Realiza una petición GET y deserializa la respuesta
    ''' </summary>
    Public Shared Function GetAsync(Of T)(url As String) As T

        Try
            Dim taskRespuesta = Task.Run(Async Function()

                                            Return Await Client.GetAsync(url).ConfigureAwait(False)
                                        End Function)
            Dim respuesta = taskRespuesta.Result

            If Not respuesta.IsSuccessStatusCode Then
                Dim contenidoError = Task.Run(Async Function()

                                                  Return Await respuesta.Content.ReadAsStringAsync().ConfigureAwait(False)
                                              End Function).Result
                Dim mensaje = ApiErrorHelper.ExtraerMensaje(contenidoError)

                If String.IsNullOrWhiteSpace(mensaje) Then
                    Throw New Exception($"Error GET: {respuesta.StatusCode}")
                Else
                    Throw New Exception($"Error GET: {mensaje} (HTTP {respuesta.StatusCode})")
                End If
            End If

            Dim contenido = Task.Run(Async Function()

                                         Return Await respuesta.Content.ReadAsStringAsync().ConfigureAwait(False)
                                     End Function).Result

            If String.IsNullOrWhiteSpace(contenido) Then
                Return Nothing
            End If

            Return JsonConvert.DeserializeObject(Of T)(contenido)

        Catch ex As Exception
            Logger.LogError($"Error en GetAsync para URL: {url}", ex, "")
            Throw

        End Try
    End Function

    ''' <summary>
    ''' Realiza una petición POST y deserializa la respuesta
    ''' </summary>
    Public Shared Function PostAsync(Of T)(url As String, data As Object) As T

        Try
            Dim json = JsonConvert.SerializeObject(data)
            Dim contenido = New StringContent(json, Encoding.UTF8, "application/json")
            Dim taskRespuesta = Task.Run(Async Function()

                                            Return Await Client.PostAsync(url, contenido).ConfigureAwait(False)
                                        End Function)
            Dim respuesta = taskRespuesta.Result

            If Not respuesta.IsSuccessStatusCode Then
                Dim contenidoError = Task.Run(Async Function()

                                                  Return Await respuesta.Content.ReadAsStringAsync().ConfigureAwait(False)
                                              End Function).Result
                Dim mensaje = ApiErrorHelper.ExtraerMensaje(contenidoError)

                If String.IsNullOrWhiteSpace(mensaje) Then
                    Throw New Exception($"Error POST: {respuesta.StatusCode}")
                Else
                    Throw New Exception($"Error POST: {mensaje} (HTTP {respuesta.StatusCode})")
                End If
            End If

            Dim contenidoRespuesta = Task.Run(Async Function()

                                                 Return Await respuesta.Content.ReadAsStringAsync().ConfigureAwait(False)
                                             End Function).Result

            If String.IsNullOrWhiteSpace(contenidoRespuesta) Then
                Return Nothing
            End If

            Return JsonConvert.DeserializeObject(Of T)(contenidoRespuesta)

        Catch ex As Exception
            Logger.LogError($"Error en PostAsync para URL: {url}", ex, "")
            Throw

        End Try
    End Function

    ''' <summary>
    ''' Realiza una petición PUT y deserializa la respuesta
    ''' </summary>
    Public Shared Function PutAsync(Of T)(url As String, data As Object) As T

        Try
            Dim json = JsonConvert.SerializeObject(data)
            Dim contenido = New StringContent(json, Encoding.UTF8, "application/json")
            Dim taskRespuesta = Task.Run(Async Function()

                                            Return Await Client.PutAsync(url, contenido).ConfigureAwait(False)
                                        End Function)
            Dim respuesta = taskRespuesta.Result

            If Not respuesta.IsSuccessStatusCode Then
                Dim contenidoError = Task.Run(Async Function()

                                                  Return Await respuesta.Content.ReadAsStringAsync().ConfigureAwait(False)
                                              End Function).Result
                Dim mensaje = ApiErrorHelper.ExtraerMensaje(contenidoError)

                If String.IsNullOrWhiteSpace(mensaje) Then
                    Throw New Exception($"Error PUT: {respuesta.StatusCode}")
                Else
                    Throw New Exception($"Error PUT: {mensaje} (HTTP {respuesta.StatusCode})")
                End If
            End If

            Dim contenidoRespuesta = Task.Run(Async Function()

                                                 Return Await respuesta.Content.ReadAsStringAsync().ConfigureAwait(False)
                                             End Function).Result

            If String.IsNullOrWhiteSpace(contenidoRespuesta) Then
                Return Nothing
            End If

            Return JsonConvert.DeserializeObject(Of T)(contenidoRespuesta)

        Catch ex As Exception
            Logger.LogError($"Error en PutAsync para URL: {url}", ex, "")
            Throw

        End Try
    End Function

    ''' <summary>
    ''' Realiza una petición DELETE
    ''' </summary>
    Public Shared Function DeleteAsync(url As String) As Boolean

        Try
            Dim taskRespuesta = Task.Run(Async Function()

                                            Return Await Client.DeleteAsync(url).ConfigureAwait(False)
                                        End Function)
            Dim respuesta = taskRespuesta.Result

            If Not respuesta.IsSuccessStatusCode Then
                Dim contenidoError = Task.Run(Async Function()

                                                  Return Await respuesta.Content.ReadAsStringAsync().ConfigureAwait(False)
                                              End Function).Result
                Dim mensaje = ApiErrorHelper.ExtraerMensaje(contenidoError)

                If String.IsNullOrWhiteSpace(mensaje) Then
                    Throw New Exception($"Error DELETE: {respuesta.StatusCode}")
                Else
                    Throw New Exception($"Error DELETE: {mensaje} (HTTP {respuesta.StatusCode})")
                End If
            End If

            Return True

        Catch ex As Exception
            Logger.LogError($"Error en DeleteAsync para URL: {url}", ex, "")
            Throw

        End Try
    End Function

    ''' <summary>
    ''' Limpia y libera recursos (solo usar en Application_End)
    ''' </summary>
    Public Shared Sub Dispose()
        If _httpClient.IsValueCreated Then
            _httpClient.Value.Dispose()
        End If
    End Sub

End Class
