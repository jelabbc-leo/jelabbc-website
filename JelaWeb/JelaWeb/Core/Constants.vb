Imports System.Web

''' <summary>
''' Constantes centralizadas de la aplicación
''' Facilita el mantenimiento y evita valores mágicos en el código
''' </summary>
Public NotInheritable Class Constants
    Private Sub New()
        ' Clase estática, no instanciable
    End Sub

    ' ============================================
    ' RUTAS DE LA APLICACIÓN
    ' ============================================
    Public Const ROUTE_LOGIN As String = "~/Views/Auth/Ingreso.aspx"
    Public Const ROUTE_LOGOUT As String = "~/Views/Auth/Logout.aspx"
    Public Const ROUTE_INICIO As String = "~/Views/Inicio.aspx"
    Public Const ROUTE_TICKETS As String = "~/Views/Operacion/Tickets/Tickets.aspx"
    Public Const ROUTE_ERROR As String = "~/Views/Error/Error.aspx"

    ' NUEVO: Ruta para selector de entidades
    Public Const ROUTE_SELECTOR_ENTIDADES As String = "/Views/Auth/SelectorEntidades.aspx"

    ' Métodos helper para construir URLs de error con parámetros
    Public Shared Function GetErrorUrl(code As String, Optional message As String = Nothing, Optional description As String = Nothing) As String
        Dim url As String = ROUTE_ERROR & "?code=" & code

        If Not String.IsNullOrEmpty(message) Then
            url &= "&msg=" & HttpUtility.UrlEncode(message)
        End If
        If Not String.IsNullOrEmpty(description) Then
            url &= "&desc=" & HttpUtility.UrlEncode(description)
        End If
        Return url
    End Function

    ' ============================================
    ' CLAVES DE CACHÉ
    ' ============================================
    Public Const CACHE_REGIMEN As String = "Cache_RegimenFiscal"
    Public Const CACHE_FORMA_PAGO As String = "Cache_FormaPago"
    Public Const CACHE_METODO_PAGO As String = "Cache_MetodoPago"
    Public Const CACHE_USOS_CFDI As String = "Cache_UsosCFDI"
    Public Const CACHE_TIPOS_DOC As String = "Cache_TiposDocumentos"
    Public Const CACHE_CONCEPTOS As String = "Cache_Conceptos"
    Public Const CACHE_CONCEPTOS_COMBO As String = "Cache_ConceptosCombo"

    ' ============================================
    ' CONFIGURACIÓN DE SESIÓN
    ' ============================================
    Public Const SESSION_TIMEOUT_MINUTES As Integer = 30
    Public Const SESSION_USER_ID As String = "UserId"
    Public Const SESSION_ID_ENTIDAD As String = "IdEntidad"
    Public Const SESSION_NOMBRE As String = "Nombre"
    Public Const SESSION_OPCIONES As String = "Opciones"
    Public Const SESSION_LAST_ACTIVITY As String = "LastActivity"
    Public Const SESSION_LOGIN_TIME As String = "LoginTime"
    
    ' NUEVO: Constantes para sistema multi-entidad
    Public Const SESSION_TIPO_USUARIO As String = "TipoUsuario"
    Public Const SESSION_ENTIDADES As String = "Entidades"
    Public Const SESSION_ID_ENTIDAD_ACTUAL As String = "IdEntidadActual"
    Public Const SESSION_ENTIDAD_ACTUAL_NOMBRE As String = "EntidadActualNombre"
    Public Const SESSION_LICENCIAS_DISPONIBLES As String = "LicenciasDisponibles"

    ' ============================================
    ' CONFIGURACIÓN DE API
    ' ============================================
    Public Const API_TIMEOUT_SECONDS As Integer = 30
    Public Const API_MAX_RETRIES As Integer = 3

    ' ============================================
    ' CONFIGURACIÓN DE CACHÉ
    ' ============================================
    Public Const CACHE_DEFAULT_MINUTES As Integer = 30
    Public Const CACHE_SHORT_MINUTES As Integer = 5
    Public Const CACHE_LONG_MINUTES As Integer = 60

    ' ============================================
    ' CONFIGURACIÓN DE LOGGING
    ' ============================================
    Public Const LOG_MAX_FILE_SIZE_MB As Long = 10
    Public Const LOG_RETENTION_DAYS As Integer = 30

    ' ============================================
    ' CONFIGURACIÓN DE SEGURIDAD
    ' ============================================
    Public Const MAX_LOGIN_ATTEMPTS As Integer = 5
    Public Const LOCKOUT_DURATION_MINUTES As Integer = 15

    ' ============================================
    ' INFORMACIÓN DEL SISTEMA
    ' ============================================
    Public Const SYSTEM_VERSION As String = "1.0.0"
    Public Const SYSTEM_NAME As String = "JELA Web"
    
    ' ============================================
    ' TIPOS DE USUARIO (Sistema Multi-Entidad)
    ' ============================================
    Public Const TIPO_USUARIO_ADMIN_CONDOMINIOS As String = "AdministradorCondominios"
    Public Const TIPO_USUARIO_MESA_DIRECTIVA As String = "MesaDirectiva"
    Public Const TIPO_USUARIO_RESIDENTE As String = "Residente"
    Public Const TIPO_USUARIO_EMPLEADO As String = "Empleado"

End Class
