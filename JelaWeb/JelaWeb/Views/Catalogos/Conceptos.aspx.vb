Imports DevExpress.Web
Imports DevExpress.XtraRichEdit.Model
Imports System.Data

Public Class Conceptos
    Inherits BasePage
    Private servicio As ApiService

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        servicio = New ApiService(ConfigHelper.ApiBaseUrl)

        If Not IsPostBack Then
            ' Suscribir evento DataBound
            AddHandler grdCatConceptos.DataBound, AddressOf grdCatConceptos_DataBound
            LlenarGrid()
        End If
    End Sub

    Protected Sub LlenarGrid()
        Try
            Dim dt As DataTable = servicio.ListarConceptos()
            
            ' Generar columnas dinámicamente
            GenerarColumnasDinamicas(grdCatConceptos, dt)
            
            ' Guardar DataTable en Session para FuncionesGridWeb
            Session("dtConceptos") = dt
            
            grdCatConceptos.DataSource = dt
            grdCatConceptos.DataBind()

        Catch ex As ApplicationException
            ' Mostrar el mensaje del API en un alert de JavaScript
            Dim script As String = "<script>alert('" & ex.Message.Replace("'", "\'") & "');</script>"
            Response.Write(script)

        Catch ex As Exception
            ' Mostrar cualquier otro error inesperado
            Dim script As String = "<script>alert('Error inesperado: " & ex.Message.Replace("'", "\'") & "');</script>"
            Response.Write(script)
        End Try
    End Sub

    ''' <summary>
    ''' Genera columnas dinámicamente para un grid basándose en las columnas del DataTable.
    ''' </summary>
    Private Sub GenerarColumnasDinamicas(grid As ASPxGridView, tabla As DataTable)
        Try
            If tabla Is Nothing OrElse tabla.Columns.Count = 0 Then Return
            
            ' Limpiar columnas previas (excepto columnas personalizadas)
            Dim columnasParaMantener As New List(Of GridViewColumn)
            
            For Each col As GridViewColumn In grid.Columns
                If TypeOf col Is GridViewCommandColumn Then
                    columnasParaMantener.Add(col)
                End If
            Next
            
            ' Limpiar solo las columnas de datos
            For i As Integer = grid.Columns.Count - 1 To 0 Step -1
                Dim col = grid.Columns(i)
                If Not TypeOf col Is GridViewCommandColumn Then
                    grid.Columns.RemoveAt(i)
                End If
            Next
            
            ' Crear columnas dinámicamente desde el DataTable
            For Each col As DataColumn In tabla.Columns
                Dim nombreColumna = col.ColumnName
                
                ' Crear columna según el tipo de dato
                Dim gridCol As GridViewDataColumn = Nothing
                
                Select Case col.DataType
                    Case GetType(Boolean)
                        gridCol = New GridViewDataCheckColumn()
                        gridCol.Width = Unit.Pixel(80)
                    Case GetType(DateTime), GetType(Date)
                        gridCol = New GridViewDataDateColumn()
                        gridCol.Width = Unit.Pixel(150)
                        CType(gridCol, GridViewDataDateColumn).PropertiesDateEdit.DisplayFormatString = "dd/MM/yyyy"
                    Case GetType(Decimal), GetType(Double), GetType(Single)
                        gridCol = New GridViewDataTextColumn()
                        gridCol.Width = Unit.Pixel(100)
                        gridCol.PropertiesEdit.DisplayFormatString = "c2"
                    Case GetType(Integer), GetType(Long), GetType(Short)
                        gridCol = New GridViewDataTextColumn()
                        gridCol.Width = Unit.Pixel(100)
                        gridCol.PropertiesEdit.DisplayFormatString = "n0"
                    Case Else
                        gridCol = New GridViewDataTextColumn()
                        gridCol.Width = Unit.Pixel(150)
                End Select
                
                gridCol.FieldName = nombreColumna
                gridCol.Caption = FuncionesGridWeb.SplitCamelCase(nombreColumna)
                gridCol.ReadOnly = True
                gridCol.Visible = True
                
                ' Configurar filtros y agrupación
                gridCol.Settings.AllowHeaderFilter = True
                gridCol.Settings.AllowGroup = True
                
                ' Ocultar columna Id si existe
                If nombreColumna.Equals("Id", StringComparison.OrdinalIgnoreCase) Then
                    gridCol.Visible = False
                End If
                
                grid.Columns.Add(gridCol)
            Next
            
        Catch ex As Exception
            Logger.LogError("GenerarColumnasDinamicas", ex)
            Throw
        End Try
    End Sub

    Protected Sub grdCatConceptos_DataBound(sender As Object, e As EventArgs) Handles grdCatConceptos.DataBound
        Try
            ' Aplicar FuncionesGridWeb.SUMColumn para formateo automático
            If Session("dtConceptos") IsNot Nothing Then
                FuncionesGridWeb.SUMColumn(grdCatConceptos, Session("dtConceptos"))
            End If
        Catch ex As Exception
            Logger.LogError("grdCatConceptos_DataBound", ex)
        End Try
    End Sub

End Class