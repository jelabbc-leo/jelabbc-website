Imports System.Configuration

Public Class TestChatWidget
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' Página de prueba para el Chat Widget
        ' No requiere lógica adicional en el code-behind
        
        ' Opcional: Verificar que el usuario esté autenticado
        ' If Session("IdUsuario") Is Nothing Then
        '     Response.Redirect("~/Views/Auth/Ingreso.aspx")
        ' End If
    End Sub

End Class
