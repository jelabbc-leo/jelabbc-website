Imports System.Web.UI

Public Class DefaultRedirectPage
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' Redirigir a la página de login (página de entrada de la aplicación)
        Response.Redirect(Constants.ROUTE_LOGIN, True)
    End Sub

End Class
