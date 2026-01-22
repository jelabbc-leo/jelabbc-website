Imports System.Net.Http
Imports System.Text
Imports System.Threading
Imports System.Threading.Tasks
Imports Newtonsoft.Json

''' <summary>
''' Servicio para gestión de tokens JWT para la API
''' Maneja la autenticación, almacenamiento y renovación automática de tokens
''' </summary>
Public Class JwtTokenService
    Private Shared _instance As JwtTokenService
    Private Shared ReadOnly _lock As New Object()

    Private _token As String
    Private _refreshToken As String
    Private _expiresAt As DateTime
    Private ReadOnly _apiBaseUrl As String

    ' Margen de tiempo antes de expiración para renovar (5 minutos)
    Private Const REFRESH_MARGIN_MINUTES As Integer = 5

    Private Sub New()
        _apiBaseUrl = ConfigurationManager.AppSettings("ApiBaseUrl")
        If Not String.IsNullOrEmpty(_apiBaseUrl) Then
            ' Extraer URL base (quitar ?strQuery=)
            Dim idx = _apiBaseUrl.IndexOf("/api/")
            If idx > 0 Then
                _apiBaseUrl = _apiBaseUrl.Substring(0, idx)
            End If
        End If
    End Sub

    ''' <summary>
    ''' Obtiene la instancia singleton del servicio
    ''' </summary>
    Public Shared ReadOnly Property Instance As JwtTokenService
        Get
            If _instance Is Nothing Then
                SyncLock _lock
                    If _instance Is Nothing Then
                        _instance = New JwtTokenService()
                    End If
                End SyncLock
            End If
            Return _instance
        End Get
    End Property

    ''' <summary>
    ''' Obtiene el token actual, renovándolo si es necesario
    ''' </summary>
    Public Function GetToken() As String
        SyncLock _lock
            ' Si no hay token o está por expirar, intentar renovar
            If String.IsNullOrEmpty(_token) OrElse IsTokenExpiringSoon() Then
                ' Intentar renovar con refresh token
                If Not String.IsNullOrEmpty(_refreshToken) Then
                    Try
                        RefreshTokenInternal()
                    Catch ex As Exception
                        Logger.LogWarning($"Error al renovar token: {ex.Message}. Se requiere nuevo login.")
                        _token = Nothing
                        _refreshToken = Nothing
                    End Try
                End If
            End If

            Return _token
        End SyncLock
    End Function

    ''' <summary>
    ''' Indica si hay un token válido disponible
    ''' </summary>
    Public Function HasValidToken() As Boolean
        Return Not String.IsNullOrEmpty(_token) AndAlso _expiresAt > DateTime.UtcNow
    End Function

    ''' <summary>
    ''' Autentica con las credenciales del usuario actual de sesión
    ''' </summary>
    Public Function AuthenticateFromSession() As Boolean
        Try
            Dim currentUser = SessionHelper.GetCurrentUser()
            If currentUser Is Nothing Then
                Return False
            End If

            ' Usar las credenciales almacenadas en sesión para autenticar
            ' Nota: Esto requiere que el password esté disponible o usar otro mecanismo
            ' Por ahora, autenticamos usando el username del usuario logueado
            Return AuthenticateWithCredentials(currentUser.Username, GetSessionPassword())
        Catch ex As Exception
            Logger.LogError("Error en AuthenticateFromSession", ex)
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Autentica con username y password
    ''' </summary>
    Public Function AuthenticateWithCredentials(username As String, password As String) As Boolean
        Try
            If String.IsNullOrEmpty(username) OrElse String.IsNullOrEmpty(password) Then
                Return False
            End If

            Dim loginUrl = $"{_apiBaseUrl}/api/auth/login"
            Dim loginRequest = New With {
                .username = username,
                .password = password
            }

            Dim json = JsonConvert.SerializeObject(loginRequest)
            Dim content = New StringContent(json, Encoding.UTF8, "application/json")

            Dim taskRespuesta = Task.Run(Async Function()
                                             Return Await HttpClientHelper.Client.PostAsync(loginUrl, content).ConfigureAwait(False)
                                         End Function)
            Dim respuesta = taskRespuesta.Result

            Dim taskContenido = Task.Run(Async Function()
                                             Return Await respuesta.Content.ReadAsStringAsync().ConfigureAwait(False)
                                         End Function)
            Dim contenido = taskContenido.Result

            If Not respuesta.IsSuccessStatusCode Then
                Logger.LogWarning($"Error de autenticación JWT: {respuesta.StatusCode} - {contenido}")
                Return False
            End If

            Dim authResponse = JsonConvert.DeserializeObject(Of JwtAuthResponse)(contenido)

            If authResponse IsNot Nothing AndAlso authResponse.Success Then
                SyncLock _lock
                    _token = authResponse.Token
                    _refreshToken = authResponse.RefreshToken
                    _expiresAt = If(authResponse.ExpiresAt.HasValue, authResponse.ExpiresAt.Value, DateTime.UtcNow.AddHours(1))
                End SyncLock

                Logger.LogInfo($"Autenticación JWT exitosa para usuario: {username}")
                Return True
            End If

            Return False
        Catch ex As Exception
            Logger.LogError("Error en AuthenticateWithCredentials", ex)
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Establece el token directamente (usado después del login de JelaWeb)
    ''' </summary>
    Public Sub SetToken(token As String, refreshToken As String, expiresAt As DateTime)
        SyncLock _lock
            _token = token
            _refreshToken = refreshToken
            _expiresAt = expiresAt
        End SyncLock
    End Sub

    ''' <summary>
    ''' Limpia los tokens (logout)
    ''' </summary>
    Public Sub ClearTokens()
        SyncLock _lock
            _token = Nothing
            _refreshToken = Nothing
            _expiresAt = DateTime.MinValue
        End SyncLock
    End Sub

    ''' <summary>
    ''' Verifica si el token está por expirar
    ''' </summary>
    Private Function IsTokenExpiringSoon() As Boolean
        Return _expiresAt <= DateTime.UtcNow.AddMinutes(REFRESH_MARGIN_MINUTES)
    End Function

    ''' <summary>
    ''' Renueva el token usando el refresh token
    ''' </summary>
    Private Sub RefreshTokenInternal()
        Dim refreshUrl = $"{_apiBaseUrl}/api/auth/refresh"
        Dim refreshRequest = New With {.refreshToken = _refreshToken}

        Dim json = JsonConvert.SerializeObject(refreshRequest)
        Dim content = New StringContent(json, Encoding.UTF8, "application/json")

        Dim taskRespuesta = Task.Run(Async Function()
                                         Return Await HttpClientHelper.Client.PostAsync(refreshUrl, content).ConfigureAwait(False)
                                     End Function)
        Dim respuesta = taskRespuesta.Result

        Dim taskContenido = Task.Run(Async Function()
                                         Return Await respuesta.Content.ReadAsStringAsync().ConfigureAwait(False)
                                     End Function)
        Dim contenido = taskContenido.Result

        If Not respuesta.IsSuccessStatusCode Then
            Throw New Exception($"Error al renovar token: {respuesta.StatusCode}")
        End If

        Dim authResponse = JsonConvert.DeserializeObject(Of JwtAuthResponse)(contenido)

        If authResponse IsNot Nothing AndAlso authResponse.Success Then
            _token = authResponse.Token
            _refreshToken = authResponse.RefreshToken
            _expiresAt = If(authResponse.ExpiresAt.HasValue, authResponse.ExpiresAt.Value, DateTime.UtcNow.AddHours(1))
            Logger.LogInfo("Token JWT renovado exitosamente")
        Else
            Throw New Exception("Respuesta de renovación inválida")
        End If
    End Sub

    ''' <summary>
    ''' Obtiene el password de la sesión si está disponible
    ''' </summary>
    Private Function GetSessionPassword() As String
        ' Si el password está almacenado en sesión (encriptado), obtenerlo aquí
        ' Por seguridad, normalmente no se almacena el password
        ' Este método puede ser implementado según las necesidades del proyecto
        Try
            If HttpContext.Current IsNot Nothing AndAlso HttpContext.Current.Session IsNot Nothing Then
                Return TryCast(HttpContext.Current.Session("_jela_pwd"), String)
            End If
        Catch
            ' Ignorar errores de sesión
        End Try
        Return Nothing
    End Function

End Class

''' <summary>
''' DTO para respuesta de autenticación JWT
''' </summary>
Public Class JwtAuthResponse
    Public Property Success As Boolean
    Public Property Token As String
    Public Property RefreshToken As String
    Public Property ExpiresAt As DateTime?
    Public Property User As JwtUserInfo
    Public Property Message As String
End Class

''' <summary>
''' DTO para información del usuario JWT
''' </summary>
Public Class JwtUserInfo
    Public Property Id As Integer
    Public Property Username As String
    Public Property Nombre As String
    Public Property Email As String
    Public Property RolId As Integer?
    Public Property RolNombre As String
    Public Property EntidadId As Integer?
    Public Property EntidadNombre As String
    
    ' NUEVO: Propiedades multi-entidad
    Public Property TipoUsuario As String
    Public Property Entidades As Object ' Puede ser JArray o List
    Public Property IdEntidadPrincipal As Integer?
    Public Property LicenciasDisponibles As Integer
End Class
