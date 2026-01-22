Imports System.Web
Imports System.Web.SessionState
Imports Newtonsoft.Json.Linq

''' <summary>
''' Clase helper para gestión centralizada de sesiones
''' Proporciona acceso tipado a los valores de sesión y validaciones
''' </summary>
Public NotInheritable Class SessionHelper
    Private Sub New()
        ' Clase estática, no instanciable
    End Sub

    ''' <summary>
    ''' Obtiene el ID del usuario actual desde la sesión
    ''' </summary>
    Public Shared Function GetUserId() As Integer?
        Dim session = HttpContext.Current.Session

        If session IsNot Nothing AndAlso session(Constants.SESSION_USER_ID) IsNot Nothing Then
            Return Convert.ToInt32(session(Constants.SESSION_USER_ID))
        End If
        Return Nothing
    End Function

    ''' <summary>
    ''' Obtiene el ID de la entidad actual desde la sesión (multi-tenant)
    ''' </summary>
    Public Shared Function GetIdEntidad() As Integer
        Dim session = HttpContext.Current.Session

        If session IsNot Nothing AndAlso session(Constants.SESSION_ID_ENTIDAD) IsNot Nothing Then
            Return Convert.ToInt32(session(Constants.SESSION_ID_ENTIDAD))
        End If
        Return 0
    End Function

    ''' <summary>
    ''' Obtiene el nombre del usuario actual desde la sesión
    ''' </summary>
    Public Shared Function GetNombre() As String
        Dim session = HttpContext.Current.Session

        If session IsNot Nothing AndAlso session(Constants.SESSION_NOMBRE) IsNot Nothing Then
            Return session(Constants.SESSION_NOMBRE).ToString()
        End If
        Return String.Empty
    End Function

    ''' <summary>
    ''' Obtiene las opciones del menú desde la sesión
    ''' </summary>
    Public Shared Function GetOpciones() As JArray
        Dim session = HttpContext.Current.Session

        If session IsNot Nothing AndAlso session(Constants.SESSION_OPCIONES) IsNot Nothing Then
            Return TryCast(session(Constants.SESSION_OPCIONES), JArray)
        End If
        Return Nothing
    End Function

    ''' <summary>
    ''' Verifica si el usuario está autenticado
    ''' </summary>
    Public Shared Function IsAuthenticated() As Boolean
        Dim session = HttpContext.Current.Session

        Return session IsNot Nothing AndAlso GetUserId().HasValue
    End Function

    ''' <summary>
    ''' Inicializa la sesión del usuario después del login
    ''' </summary>
    Public Shared Sub InitializeSession(userId As Object, nombre As String, opciones As JArray, 
                                        Optional tipoUsuario As String = "Residente",
                                        Optional entidades As JArray = Nothing,
                                        Optional licenciasDisponibles As Integer = 0,
                                        Optional idEntidadPrincipal As Integer? = Nothing,
                                        Optional idEntidad As Integer = 0)
        Dim session = HttpContext.Current.Session

        If session IsNot Nothing Then
            ' Limpiar cualquier sesión anterior antes de inicializar
            session.Clear()

            ' Inicializar valores de sesión básicos
            session(Constants.SESSION_USER_ID) = userId
            session(Constants.SESSION_ID_ENTIDAD) = idEntidad
            session(Constants.SESSION_NOMBRE) = nombre
            session(Constants.SESSION_OPCIONES) = opciones
            session(Constants.SESSION_LOGIN_TIME) = DateTime.Now
            session(Constants.SESSION_LAST_ACTIVITY) = DateTime.Now
            
            ' NUEVO: Inicializar valores multi-entidad
            session(Constants.SESSION_TIPO_USUARIO) = tipoUsuario
            session(Constants.SESSION_ENTIDADES) = entidades
            session(Constants.SESSION_LICENCIAS_DISPONIBLES) = licenciasDisponibles
            
            ' Si es usuario de una sola entidad (no AdministradorCondominios), establecer automáticamente
            If tipoUsuario <> Constants.TIPO_USUARIO_ADMIN_CONDOMINIOS AndAlso idEntidadPrincipal.HasValue Then
                session(Constants.SESSION_ID_ENTIDAD_ACTUAL) = idEntidadPrincipal.Value
                If entidades IsNot Nothing AndAlso entidades.Count > 0 Then
                    session(Constants.SESSION_ENTIDAD_ACTUAL_NOMBRE) = entidades(0)("Nombre").ToString()
                End If
            End If

            ' Forzar que la sesión se guarde inmediatamente
            session.Timeout = Constants.SESSION_TIMEOUT_MINUTES
            session("SessionInitialized") = DateTime.Now
        End If
    End Sub

    ''' <summary>
    ''' Actualiza la última actividad del usuario
    ''' </summary>
    Public Shared Sub UpdateLastActivity()
        Dim session = HttpContext.Current.Session

        If session IsNot Nothing Then
            session(Constants.SESSION_LAST_ACTIVITY) = DateTime.Now
        End If
    End Sub

    ''' <summary>
    ''' Obtiene el tiempo de inactividad del usuario en minutos
    ''' </summary>
    Public Shared Function GetInactivityMinutes() As Integer
        Dim session = HttpContext.Current.Session

        If session IsNot Nothing AndAlso session(Constants.SESSION_LAST_ACTIVITY) IsNot Nothing Then
            Dim lastActivity = DirectCast(session(Constants.SESSION_LAST_ACTIVITY), DateTime)

            Return CInt((DateTime.Now - lastActivity).TotalMinutes)
        End If
        Return 0
    End Function

    ''' <summary>
    ''' Limpia toda la sesión del usuario
    ''' </summary>
    Public Shared Sub ClearSession()
        ' Limpiar tokens JWT
        Try
            JwtTokenService.Instance.ClearTokens()
        Catch
            ' Ignorar errores al limpiar tokens
        End Try

        Dim session = HttpContext.Current.Session

        If session IsNot Nothing Then
            session.Clear()
            session.Abandon()
        End If
    End Sub

    ''' <summary>
    ''' Verifica si la sesión ha expirado
    ''' </summary>
    Public Shared Function IsSessionExpired() As Boolean
        Return Not IsAuthenticated()
    End Function

    ''' <summary>
    ''' Obtiene el usuario actual (objeto simple con información básica)
    ''' </summary>
    Public Shared Function GetCurrentUser() As Object
        Dim session = HttpContext.Current.Session

        If session IsNot Nothing AndAlso IsAuthenticated() Then
            Return New With {
                .Id = GetUserId(),
                .Nombre = GetNombre(),
                .Email = If(session("Email") IsNot Nothing, session("Email").ToString(), "")
            }
        End If
        Return Nothing
    End Function
    
    ' ============================================
    ' MÉTODOS MULTI-ENTIDAD
    ' ============================================
    
    ''' <summary>
    ''' Obtiene el tipo de usuario actual
    ''' </summary>
    Public Shared Function GetTipoUsuario() As String
        Dim session = HttpContext.Current.Session
        
        If session IsNot Nothing AndAlso session(Constants.SESSION_TIPO_USUARIO) IsNot Nothing Then
            Return session(Constants.SESSION_TIPO_USUARIO).ToString()
        End If
        Return "Residente" ' Valor por defecto
    End Function
    
    ''' <summary>
    ''' Obtiene las entidades asignadas al usuario
    ''' </summary>
    Public Shared Function GetEntidades() As JArray
        Dim session = HttpContext.Current.Session
        
        If session IsNot Nothing AndAlso session(Constants.SESSION_ENTIDADES) IsNot Nothing Then
            Return TryCast(session(Constants.SESSION_ENTIDADES), JArray)
        End If
        Return New JArray() ' Retornar array vacío si no existe
    End Function
    
    ''' <summary>
    ''' Obtiene el ID de la entidad actualmente seleccionada
    ''' </summary>
    Public Shared Function GetIdEntidadActual() As Integer?
        Dim session = HttpContext.Current.Session
        
        If session IsNot Nothing AndAlso session(Constants.SESSION_ID_ENTIDAD_ACTUAL) IsNot Nothing Then
            Return Convert.ToInt32(session(Constants.SESSION_ID_ENTIDAD_ACTUAL))
        End If
        Return Nothing
    End Function
    
    ''' <summary>
    ''' Obtiene el nombre de la entidad actualmente seleccionada
    ''' </summary>
    Public Shared Function GetEntidadActualNombre() As String
        Dim session = HttpContext.Current.Session
        
        If session IsNot Nothing AndAlso session(Constants.SESSION_ENTIDAD_ACTUAL_NOMBRE) IsNot Nothing Then
            Return session(Constants.SESSION_ENTIDAD_ACTUAL_NOMBRE).ToString()
        End If
        Return String.Empty
    End Function
    
    ''' <summary>
    ''' Establece la entidad actual del usuario
    ''' </summary>
    Public Shared Sub SetEntidadActual(idEntidad As Integer, nombreEntidad As String)
        Dim session = HttpContext.Current.Session
        
        If session IsNot Nothing Then
            session(Constants.SESSION_ID_ENTIDAD_ACTUAL) = idEntidad
            session(Constants.SESSION_ENTIDAD_ACTUAL_NOMBRE) = nombreEntidad
            session(Constants.SESSION_LAST_ACTIVITY) = DateTime.Now
        End If
    End Sub
    
    ''' <summary>
    ''' Verifica si el usuario es Administrador de Condominios
    ''' </summary>
    Public Shared Function IsAdministradorCondominios() As Boolean
        Return GetTipoUsuario() = Constants.TIPO_USUARIO_ADMIN_CONDOMINIOS
    End Function
    
    ''' <summary>
    ''' Verifica si el usuario tiene múltiples entidades asignadas
    ''' </summary>
    Public Shared Function TieneMultiplesEntidades() As Boolean
        Dim entidades = GetEntidades()
        Return entidades IsNot Nothing AndAlso entidades.Count > 1
    End Function
    
    ''' <summary>
    ''' Obtiene el número de licencias disponibles del usuario
    ''' </summary>
    Public Shared Function GetLicenciasDisponibles() As Integer
        Dim session = HttpContext.Current.Session
        
        If session IsNot Nothing AndAlso session(Constants.SESSION_LICENCIAS_DISPONIBLES) IsNot Nothing Then
            Return Convert.ToInt32(session(Constants.SESSION_LICENCIAS_DISPONIBLES))
        End If
        Return 0
    End Function
    
    ''' <summary>
    ''' Verifica si el usuario tiene licencias disponibles
    ''' </summary>
    Public Shared Function TieneLicenciasDisponibles() As Boolean
        Return GetLicenciasDisponibles() > 0
    End Function

End Class
