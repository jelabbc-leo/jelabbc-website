Imports System.Web.UI

''' <summary>
''' Helper para funcionalidades auxiliares de Residentes
''' </summary>
Public Class ResidentesHelper

    ''' <summary>
    ''' Muestra un mensaje al usuario mediante JavaScript
    ''' </summary>
    Public Shared Sub MostrarMensaje(page As Page, mensaje As String, tipo As String)
        ScriptManager.RegisterStartupScript(page, page.GetType(), "showAlert", $"toastr.{tipo}('{mensaje}');", True)
    End Sub

End Class
