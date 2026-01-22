Imports System.Data
Imports System.Collections.Generic
Imports System.Web.UI

''' <summary>
''' Helper para funcionalidades auxiliares de Comunicados
''' </summary>
Public Class ComunicadosHelper

    ''' <summary>
    ''' Muestra un mensaje al usuario mediante JavaScript
    ''' </summary>
    Public Shared Sub MostrarMensaje(page As Page, mensaje As String, tipo As String)
        Dim script As String = $"if (typeof showToast !== 'undefined') {{ showToast('{tipo}', '{mensaje.Replace("'", "\'")}'); }}"

        ScriptManager.RegisterStartupScript(page, page.GetType(), "showAlert", script, True)
    End Sub

    ''' <summary>
    ''' Convierte un DataTable a una lista de diccionarios
    ''' </summary>
    Public Shared Function ConvertDataTableToList(dt As DataTable) As List(Of Dictionary(Of String, Object))
        Dim lista As New List(Of Dictionary(Of String, Object))

        For Each row As DataRow In dt.Rows
            Dim dict As New Dictionary(Of String, Object)

            For Each col As DataColumn In dt.Columns
                dict(col.ColumnName) = If(IsDBNull(row(col)), Nothing, row(col))

            Next

            lista.Add(dict)

        Next

        Return lista
    End Function

End Class
