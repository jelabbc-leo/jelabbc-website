Imports System.Data
Imports System.Collections.Generic

''' <summary>
''' Helper para conversión de DataTables
''' </summary>
Public Class DataTableHelper

    ''' <summary>
    ''' Convierte un DataTable a una lista de diccionarios
    ''' </summary>
    Public Shared Function ConvertDataTableToList(dt As DataTable) As List(Of Dictionary(Of String, Object))
        Dim lista As New List(Of Dictionary(Of String, Object))

        If dt Is Nothing Then Return lista

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
