Imports System.Web
Imports System.IO

''' <summary>
''' Clase helper centralizada para validación de autenticación
''' Define qué rutas son públicas y cuáles requieren autenticación
''' </summary>
Public NotInheritable Class AuthHelper
    Private Sub New()
        ' Clase estática, no instanciable
    End Sub

    ''' <summary>
    ''' Rutas públicas que no requieren autenticación
    ''' </summary>
    Private Shared ReadOnly PublicRoutes As String() = {
        "/Views/Auth/Ingreso.aspx",
        "/Views/Auth/Login.aspx",
        "Ingreso.aspx",
        "Login.aspx",
        "/Views/Error/",
        "/Views/Auth/Logout.aspx",
        "Logout.aspx",
        "/Views/Auth/SelectorEntidades.aspx",
        "SelectorEntidades.aspx",
        "/Views/Auth/TestSelector.aspx",
        "TestSelector.aspx",
        "/Content/",
        "/Scripts/",
        "/App_Data/",
        "/bin/",
        "/Default.aspx",
        "Default.aspx"
    }

    ''' <summary>
    ''' Nombres de archivos públicos que no requieren autenticación
    ''' </summary>
    Private Shared ReadOnly PublicPageNames As String() = {
        "Default.aspx",
        "Ingreso.aspx",
        "Login.aspx",
        "Logout.aspx",
        "Error.aspx",
        "SelectorEntidades.aspx",
        "TestSelector.aspx"
    }

    ''' <summary>
    ''' Extensiones de archivos estáticos que no requieren autenticación
    ''' </summary>
    Private Shared ReadOnly StaticFileExtensions As String() = {
        ".css", ".js", ".jpg", ".jpeg", ".png", ".gif", ".ico", 
        ".svg", ".woff", ".woff2", ".ttf", ".eot", ".pdf", ".xml"
    }

    ''' <summary>
    ''' Verifica si una ruta es pública y no requiere autenticación
    ''' </summary>
    Public Shared Function IsPublicRoute(path As String) As Boolean
        If String.IsNullOrEmpty(path) Then Return False

        ' Normalizar la ruta: convertir a minúsculas y normalizar separadores
        Dim normalizedPath = path.ToLower().Replace("\", "/").Trim()

        ' Si la ruta comienza con ~/, remover el ~
        If normalizedPath.StartsWith("~/") Then
            normalizedPath = normalizedPath.Substring(2)
        End If

        ' Si no comienza con /, agregarlo
        If Not normalizedPath.StartsWith("/") Then
            normalizedPath = "/" & normalizedPath
        End If

        ' Primero verificar por nombre de archivo (más rápido y directo)
        Dim pathFileName = System.IO.Path.GetFileName(normalizedPath)

        If Not String.IsNullOrEmpty(pathFileName) Then

            For Each publicPageName In PublicPageNames
                If pathFileName = publicPageName.ToLower() Then
                    Return True
                End If

            Next

        End If

        ' Luego verificar rutas públicas completas

        For Each route In PublicRoutes
            Dim routeNormalized = route.ToLower().Replace("\", "/").Trim()

            ' Si la ruta pública comienza con ~/, remover el ~
            If routeNormalized.StartsWith("~/") Then
                routeNormalized = routeNormalized.Substring(2)
            End If

            ' Si no comienza con /, agregarlo
            If Not routeNormalized.StartsWith("/") Then
                routeNormalized = "/" & routeNormalized
            End If

            ' Comparación exacta
            If normalizedPath = routeNormalized Then
                Return True
            End If

            ' Si la ruta pública termina con /, verificar si la ruta actual está dentro de esa carpeta
            If routeNormalized.EndsWith("/") Then
                If normalizedPath.StartsWith(routeNormalized) Then
                    Return True
                End If
            Else
                ' Si no termina con /, verificar si contiene la ruta completa
                If normalizedPath.Contains(routeNormalized) Then
                    Return True
                End If
            End If

            ' Verificar por nombre de archivo (útil para rutas como Ingreso.aspx)
            Dim routeFileName = System.IO.Path.GetFileName(routeNormalized)

            If Not String.IsNullOrEmpty(routeFileName) AndAlso Not String.IsNullOrEmpty(pathFileName) Then
                If pathFileName = routeFileName.ToLower() Then
                    Return True
                End If
            End If

        Next

        ' Verificar si es un archivo estático
        Dim extension = System.IO.Path.GetExtension(normalizedPath)

        If Not String.IsNullOrEmpty(extension) Then
            If StaticFileExtensions.Contains(extension.ToLower()) Then
                Return True
            End If
        End If

        ' Verificar handlers de DevExpress (DX.ashx, ASPxUploadProgressHandlerPage.ashx, etc.)
        If normalizedPath.Contains(".ashx") Or 
           normalizedPath.Contains("dx.ashx") Or 
           normalizedPath.Contains("aspxuploadprogresshandlerpage.ashx") Then
            Return True
        End If

        ' Verificar si es un WebResource (recursos embebidos de ASP.NET)
        If normalizedPath.Contains("webresource.axd") Or normalizedPath.Contains("scriptresource.axd") Then
            Return True
        End If

        Return False
    End Function

    ''' <summary>
    ''' Verifica si la request actual requiere autenticación
    ''' </summary>
    Public Shared Function RequiresAuthentication() As Boolean
        Dim request = HttpContext.Current.Request

        If request Is Nothing Then Return True

        Dim path = request.Path

        Return Not IsPublicRoute(path)
    End Function

    ''' <summary>
    ''' Valida y redirige si no está autenticado
    ''' Retorna True si está autenticado o es ruta pública
    ''' </summary>
    Public Shared Function ValidateAndRedirect() As Boolean
        ' Si es ruta pública, permitir acceso
        Dim request = HttpContext.Current.Request

        If request Is Nothing Then Return False

        ' Verificar tanto la ruta absoluta como la relativa
        Dim appRelativePath = request.AppRelativeCurrentExecutionFilePath
        Dim absolutePath = request.Path

        If IsPublicRoute(appRelativePath) OrElse IsPublicRoute(absolutePath) Then
            Return True
        End If

        ' Si requiere autenticación, validar sesión
        If Not SessionHelper.IsAuthenticated() Then
            Dim response = HttpContext.Current.Response

            ' Detectar diferentes escenarios de acceso no autorizado
            Dim isFromCache = request.Headers("Cache-Control") = "max-age=0" OrElse
                             request.Headers("Pragma") = "no-cache"
            
            ' Detectar si la aplicación acaba de reiniciarse (IIS Express restart en desarrollo)
            Dim isAfterRestart As Boolean = False
            Try
                ' Usar reflexión para acceder a Global_asax.IsWithinRestartGracePeriod
                Dim globalType = Type.GetType("Global_asax")
                If globalType IsNot Nothing Then
                    Dim method = globalType.GetMethod("IsWithinRestartGracePeriod", 
                        Reflection.BindingFlags.Public Or Reflection.BindingFlags.Static)
                    If method IsNot Nothing Then
                        isAfterRestart = CBool(method.Invoke(Nothing, Nothing))
                    End If
                End If
            Catch
                ' Si falla la detección, continuar sin ella
            End Try

            ' Determinar el tipo de acceso no autorizado y loguear apropiadamente
            If isAfterRestart Then
                ' Reinicio de aplicación detectado (común en desarrollo con VS)
                Logger.LogInfo($"Redirección después de reinicio de aplicación: {absolutePath}", SecurityHelper.GetClientIP())
            ElseIf isFromCache Then
                ' Carga desde caché del navegador
                Logger.LogInfo($"Redirección desde caché detectada para: {absolutePath}", SecurityHelper.GetClientIP())
            Else
                ' Intento real de acceso no autorizado
                Logger.LogWarning($"Intento de acceso no autorizado a: {absolutePath}", SecurityHelper.GetClientIP())
            End If

            ' Redirigir al login en lugar de error 403 para mejor UX
            response.Redirect(Constants.ROUTE_LOGIN & "?returnUrl=" & HttpContext.Current.Server.UrlEncode(absolutePath), True)
            Return False
        End If

        Return True
    End Function

End Class
