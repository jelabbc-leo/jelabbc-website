Imports System.Web.UI

''' <summary>
''' Clase base para todas las páginas que requieren autenticación
''' Proporciona validación de sesión automática y funcionalidades comunes
''' </summary>
Public MustInherit Class BasePage
    Inherits Page

    ''' <summary>
    ''' Páginas públicas que no requieren autenticación
    ''' </summary>
    Private ReadOnly PublicPages As String() = {
        "Ingreso.aspx", 
        "Login.aspx", 
        "Error.aspx",
        "Logout.aspx"
    }

    Protected Overrides Sub OnPreInit(e As EventArgs)
        MyBase.OnPreInit(e)

        ' Aplicar cultura lo más temprano posible para formateos (moneda/fechas)
        LocalizationHelper.ApplyCulture()

        ' Forzar HTTPS en producción
        HttpsHelper.EnforceHttpsInProduction()

        ' Aplicar headers de seguridad
        SecurityHelper.ApplySecurityHeaders()
        
        ' Prevenir caché en páginas protegidas (no públicas)
        If Not IsPublicPage() Then
            Response.Cache.SetCacheability(HttpCacheability.NoCache)
            Response.Cache.SetExpires(DateTime.UtcNow.AddMinutes(-1))
            Response.Cache.SetNoStore()
            Response.AppendHeader("Pragma", "no-cache")
            Response.AppendHeader("Cache-Control", "no-cache, no-store, must-revalidate, private")
        End If
    End Sub

    Protected Overrides Sub OnInit(e As EventArgs)
        MyBase.OnInit(e)

        ' La validación de sesión se hace en el MasterPage
        ' No validamos aquí para evitar conflictos con el MasterPage
        ' Si no es una página pública, el MasterPage se encargará de validar
    End Sub

    Protected Overrides Sub OnLoad(e As EventArgs)
        MyBase.OnLoad(e)

        ' Actualizar última actividad si el usuario está autenticado
        If SessionHelper.IsAuthenticated() Then
            SessionHelper.UpdateLastActivity()
        End If
    End Sub

    ''' <summary>
    ''' Verifica si la página actual es pública
    ''' </summary>
    Private Function IsPublicPage() As Boolean
        Dim pageName As String = System.IO.Path.GetFileName(Request.Path)
        Dim path = Request.Path.ToLower()

        ' Verificar por nombre de archivo
        If PublicPages.Contains(pageName, StringComparer.OrdinalIgnoreCase) Then
            Return True
        End If

        ' Verificar si está en carpeta de errores
        If path.Contains("/error/") Or path.Contains("\error\") Then
            Return True
        End If

        ' Verificar si está en carpeta de autenticación
        If path.Contains("/views/auth/") Or path.Contains("\views\auth\") Then
            Return True
        End If

        Return False
    End Function

    ''' <summary>
    ''' Valida que el usuario tenga una sesión activa
    ''' </summary>
    Private Sub ValidateSession()
        If SessionHelper.IsSessionExpired() Then
            Logger.LogWarning($"Intento de acceso sin sesión a: {Request.Path}", SessionHelper.GetNombre())
            ' Redirigir a página de error 403 (No autorizado) en lugar de login
            Response.Redirect(Constants.GetErrorUrl("403"), True)
        End If
    End Sub

    ''' <summary>
    ''' Maneja errores no controlados en la página
    ''' </summary>
    Protected Overrides Sub OnError(e As EventArgs)
        Dim ex As Exception = Server.GetLastError()

        If ex IsNot Nothing Then
            Logger.LogError($"Error en página {Request.Path}", ex, SessionHelper.GetNombre())
        End If
        MyBase.OnError(e)
    End Sub

End Class
