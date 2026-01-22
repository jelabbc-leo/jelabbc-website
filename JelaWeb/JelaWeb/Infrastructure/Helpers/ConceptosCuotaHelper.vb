Imports System.Web.UI

''' <summary>
''' Helper para funcionalidades auxiliares de Conceptos de Cuota
''' </summary>
Public Class ConceptosCuotaHelper

    ''' <summary>
    ''' Muestra un mensaje al usuario mediante JavaScript
    ''' </summary>
    Public Shared Sub MostrarMensaje(page As Page, mensaje As String, tipo As String)
        ScriptManager.RegisterStartupScript(page, page.GetType(), "showAlert", $"toastr.{tipo}('{mensaje}');", True)
    End Sub

End Class
