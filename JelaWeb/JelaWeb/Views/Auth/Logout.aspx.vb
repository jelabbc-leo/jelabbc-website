Public Class Logout
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' Log del logout
        Dim usuario As String = SessionHelper.GetNombre()

        Logger.LogInfo($"Usuario cerrando sesión: {usuario}", usuario)

        ' Limpiar sesión
        SessionHelper.ClearSession()

        ' Limpiar cookies de autenticación si existen
        If Request.Cookies("AuthToken") IsNot Nothing Then
            Dim cookie As New HttpCookie("AuthToken")
            cookie.Expires = DateTime.Now.AddDays(-1)
            Response.Cookies.Add(cookie)
        End If

        ' Prevenir caché del navegador - CRÍTICO para evitar accesos no autorizados
        Response.Cache.SetCacheability(HttpCacheability.NoCache)
        Response.Cache.SetExpires(DateTime.UtcNow.AddMinutes(-1))
        Response.Cache.SetNoStore()
        Response.AppendHeader("Pragma", "no-cache")
        Response.AppendHeader("Cache-Control", "no-cache, no-store, must-revalidate, private")
        Response.AppendHeader("Expires", "0")
        
        ' Prevenir que el navegador vuelva a la página anterior
        Response.AppendHeader("Clear-Site-Data", """cache"", ""cookies"", ""storage""")

        ' Redirigir al login con parámetro de logout
        Response.Redirect(Constants.ROUTE_LOGIN & "?logout=1", True)
    End Sub

End Class
