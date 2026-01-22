Imports System.Web.UI
Imports System.Data
Imports System.Collections.Generic

''' <summary>
''' Helper para funcionalidades auxiliares de Unidades
''' </summary>
Public Class UnidadesHelper

    ''' <summary>
    ''' Convierte un DataTable a una lista de diccionarios
    ''' </summary>
    Public Shared Function ConvertDataTableToList(dt As DataTable) As List(Of Dictionary(Of String, Object))
        Return DataTableHelper.ConvertDataTableToList(dt)
    End Function

    ''' <summary>
    ''' Obtiene la clase CSS para el badge de estatus financiero
    ''' </summary>
    Public Shared Function GetStatusClass(estatus As Object) As String

        Try
            If estatus Is Nothing OrElse IsDBNull(estatus) Then Return "al-dia"
            Dim estatusStr As String = estatus.ToString().Trim()

            Select Case estatusStr.ToUpper()

                Case "AL DÍA", "AL DIA", "AL-DIA", "AL_DIA", "ALDIA"
                    Return "al-dia"

                Case "CON ADEUDO", "CON-ADEUDO", "CON_ADEUDO", "CONADEUDO", "ADEUDO"
                    Return "con-adeudo"

                Case "MOROSO"
                    Return "moroso"

                Case Else
                    Return "al-dia"

            End Select

        Catch
            Return "al-dia"

        End Try
    End Function

    ''' <summary>
    ''' Muestra un mensaje al usuario mediante JavaScript
    ''' </summary>
    Public Shared Sub MostrarMensaje(page As Page, mensaje As String, tipo As String)
        Dim script As String = $"if (typeof showToast !== 'undefined') {{ showToast('{tipo}', '{mensaje.Replace("'", "\'")}'); }} else {{ console.log('[{tipo.ToUpper()}] {mensaje}'); }}"
        ScriptManager.RegisterStartupScript(page, page.GetType(), "showAlert", script, True)
    End Sub

    ''' <summary>
    ''' Oculta el popup de unidad
    ''' </summary>
    Public Shared Sub OcultarPopupUnidad(page As Page)
        Dim script As String = "if (typeof popupUnidad !== 'undefined') { popupUnidad.Hide(); }"
        ScriptManager.RegisterStartupScript(page, page.GetType(), "hidePopup", script, True)
    End Sub

    ''' <summary>
    ''' Muestra el popup de unidad para edición
    ''' </summary>
    Public Shared Sub MostrarPopupUnidadEdicion(page As Page, titulo As String)
        Dim tituloEscapado As String = titulo.Replace("'", "\'")
        Dim script As String = $"if (typeof popupUnidad !== 'undefined') {{ popupUnidad.SetHeaderText('{tituloEscapado}'); popupUnidad.Show(); }}"
        ScriptManager.RegisterStartupScript(page, page.GetType(), "showPopup", script, True)
    End Sub

End Class
