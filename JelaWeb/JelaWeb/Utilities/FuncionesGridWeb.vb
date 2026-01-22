Imports System.Drawing
Imports System.Text.RegularExpressions
Imports DevExpress.Data
Imports DevExpress.Utils
Imports DevExpress.Web

Public Class FuncionesGridWeb

    ''' <summary>
    ''' Aplica encabezados legibles, fuentes limpias, sumatorias y ancho automático en ASPxGridView.
    ''' </summary>
    Public Shared Sub SUMColumn(ByVal MiGrid As ASPxGridView, ByVal tabla As DataTable)
        ' SIEMPRE limpiar sumatorias para regenerarlas correctamente después de callbacks
        ' Esto asegura que los formatos se mantengan
        MiGrid.TotalSummary.Clear()

        ' Habilitar filtros de header
        MiGrid.Settings.ShowHeaderFilterButton = True

        For Each colBase As GridViewColumn In MiGrid.Columns
            Dim col As GridViewDataColumn = TryCast(colBase, GridViewDataColumn)

            If col Is Nothing OrElse String.IsNullOrEmpty(col.FieldName) Then Continue For

            ' Habilitar filtro de header para cada columna
            col.Settings.AllowHeaderFilter = True

            ' Estilo visual
            col.HeaderStyle.Font.Bold = True
            col.HeaderStyle.Font.Name = "Segoe UI"
            col.HeaderStyle.Font.Size = 8

            ' Solo cambiar caption si no tiene uno personalizado (diferente al FieldName)
            If String.IsNullOrEmpty(col.Caption) OrElse col.Caption = col.FieldName Then
                col.Caption = SplitCamelCase(col.FieldName)
            End If

            col.CellStyle.Font.Name = "Segoe UI"
            col.CellStyle.Font.Size = 8

            ' Obtener tipo de datos desde el DataTable
            Dim tipoDato As Type = Nothing

            If tabla.Columns.Contains(col.FieldName) Then
                tipoDato = tabla.Columns(col.FieldName).DataType
            End If

            ' Calcular ancho automático basado en contenido
            CalcularAnchoColumna(col, tabla)

            ' Primera columna visible: contador
            If col.VisibleIndex = 0 AndAlso col.Visible Then
                Dim contador As New ASPxSummaryItem(col.FieldName, SummaryItemType.Count)
                contador.DisplayFormat = "Reg: {0}"
                MiGrid.TotalSummary.Add(contador)

            ElseIf tipoDato IsNot Nothing Then
                Dim resumen As ASPxSummaryItem = Nothing
                Dim tieneFormatoPersonalizado As Boolean = Not String.IsNullOrEmpty(col.PropertiesEdit.DisplayFormatString)

                Select Case tipoDato

                    Case GetType(Int16), GetType(Int32), GetType(Int64), GetType(Single)
                        resumen = New ASPxSummaryItem(col.FieldName, SummaryItemType.Sum)
                        resumen.DisplayFormat = "{0:n0}"
                        If Not tieneFormatoPersonalizado Then
                            col.PropertiesEdit.DisplayFormatString = "n0"
                        End If

                    Case GetType(Double)
                        resumen = New ASPxSummaryItem(col.FieldName, SummaryItemType.Sum)
                        resumen.DisplayFormat = "{0:n2}"
                        If Not tieneFormatoPersonalizado Then
                            col.PropertiesEdit.DisplayFormatString = "n2"
                        End If

                    Case GetType(Decimal)
                        resumen = New ASPxSummaryItem(col.FieldName, SummaryItemType.Sum)
                        resumen.DisplayFormat = "{0:c2}"
                        If Not tieneFormatoPersonalizado Then
                            col.PropertiesEdit.DisplayFormatString = "c2"
                        End If

                    Case GetType(DateTime)
                        If Not tieneFormatoPersonalizado Then
                            col.PropertiesEdit.DisplayFormatString = "dd/MM/yyyy HH:mm"
                        End If

                    Case GetType(Boolean)
                        ' No hacer nada especial para booleanos

                End Select

                If resumen IsNot Nothing Then
                    MiGrid.TotalSummary.Add(resumen)
                End If
            End If

            ' Ajuste proporcional de ancho
            col.CellStyle.Wrap = DefaultBoolean.False

        Next

        MiGrid.Settings.ShowFooter = True
        MiGrid.SettingsResizing.ColumnResizeMode = ColumnResizeMode.NextColumn

    End Sub

    ''' <summary>
    ''' Calcula el ancho óptimo de una columna basándose en el contenido más largo.
    ''' </summary>
    Private Shared Sub CalcularAnchoColumna(col As GridViewDataColumn, tabla As DataTable)
        Try
            ' Si la columna ya tiene un ancho definido manualmente, respetarlo
            If col.Width.Value > 0 Then
                Return
            End If

            If Not tabla.Columns.Contains(col.FieldName) Then
                Return
            End If

            Dim maxLength As Integer = 0
            Dim columnName As String = col.FieldName

            ' Considerar la longitud del caption
            If Not String.IsNullOrEmpty(col.Caption) Then
                maxLength = col.Caption.Length
            End If

            ' Buscar el contenido más largo en la columna
            For Each row As DataRow In tabla.Rows
                If Not IsDBNull(row(columnName)) AndAlso row(columnName) IsNot Nothing Then
                    Dim valorTexto As String = row(columnName).ToString()
                    If valorTexto.Length > maxLength Then
                        maxLength = valorTexto.Length
                    End If
                End If
            Next

            ' Calcular ancho en píxeles
            ' Fórmula: caracteres * 8 píxeles por carácter + padding (20px)
            ' Mínimo: 80px, Máximo: 400px
            Dim anchoCalculado As Integer = Math.Min(Math.Max((maxLength * 8) + 20, 80), 400)

            ' Ajustes especiales por tipo de dato
            Dim tipoDato As Type = tabla.Columns(columnName).DataType

            Select Case tipoDato
                Case GetType(Boolean)
                    anchoCalculado = 80 ' Columnas booleanas son pequeñas
                Case GetType(DateTime), GetType(Date)
                    anchoCalculado = Math.Max(anchoCalculado, 150) ' Fechas necesitan más espacio
                Case GetType(Decimal), GetType(Double)
                    anchoCalculado = Math.Max(anchoCalculado, 120) ' Números con decimales
                Case GetType(Int16), GetType(Int32), GetType(Int64)
                    anchoCalculado = Math.Max(anchoCalculado, 100) ' Números enteros
            End Select

            ' Aplicar el ancho calculado
            col.Width = Unit.Pixel(anchoCalculado)

        Catch ex As Exception
            ' Si hay error, usar ancho por defecto
            col.Width = Unit.Pixel(150)
        End Try
    End Sub

    ''' <summary>
    ''' Agrega sumatoria al grid.
    ''' </summary>
    Private Shared Sub AddSum(grid As ASPxGridView, fieldName As String, displayFormat As String)
        Dim sumItem As New ASPxSummaryItem()

        sumItem.FieldName = fieldName
        sumItem.SummaryType = SummaryItemType.Sum
        sumItem.DisplayFormat = displayFormat
        grid.TotalSummary.Add(sumItem)
    End Sub

    ''' <summary>
    ''' Convierte "ClaveProducto" → "Clave Producto"
    ''' </summary>
    Public Shared Function SplitCamelCase(input As String) As String
        Return Regex.Replace(input, "([a-z])([A-Z])", "$1 $2")
    End Function

    ''' <summary>
    ''' Detecta columnas numéricas por nombre.
    ''' </summary>
    Private Shared Function IsNumericColumnName(fieldName As String) As Boolean
        Dim claves = New String() {"Cantidad", "Total", "Importe", "Precio", "Costo", "Existencia", "Stock", "Id", "Clave", "Anio", "Mes"}

        For Each c In claves
            If fieldName.IndexOf(c, StringComparison.OrdinalIgnoreCase) >= 0 Then
                Return True
            End If

        Next

        Return False
    End Function

End Class
