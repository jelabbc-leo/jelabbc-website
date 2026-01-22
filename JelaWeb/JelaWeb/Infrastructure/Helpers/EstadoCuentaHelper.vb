Imports System.Web.UI

''' <summary>
''' Helper para funcionalidades auxiliares de Estado de Cuenta
''' </summary>
Public Class EstadoCuentaHelper

    ''' <summary>
    ''' Muestra un mensaje al usuario mediante JavaScript
    ''' </summary>
    Public Shared Sub MostrarMensaje(page As Page, mensaje As String, tipo As String)
        Dim script As String = $"if (typeof showToast !== 'undefined') {{ showToast('{tipo}', '{mensaje.Replace("'", "\'")}'); }} else {{ console.log('[{tipo.ToUpper()}] {mensaje}'); }}"

        ScriptManager.RegisterStartupScript(page, page.GetType(), "showAlert", script, True)
    End Sub

End Class
