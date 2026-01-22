Imports JelaWeb.Utilities
Imports DevExpress.Web
Imports System.Data

Partial Class Entidades
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            ' Suscribir evento DataBound
            AddHandler gridEntidades.DataBound, AddressOf gridEntidades_DataBound
            CargarDatos()
        End If
    End Sub

    Private Sub CargarDatos()
        Try
            ' Obtener datos del API dinámico
            Dim dt As DataTable = DynamicCrudService.ObtenerTodos("cat_entidades")

            ' Guardar DataTable en Session para FuncionesGridWeb
            Session("dtEntidades") = dt

            ' Generar columnas dinámicamente
            GenerarColumnasDinamicas(gridEntidades, dt)

            ' Asignar DataSource
            gridEntidades.DataSource = dt
            gridEntidades.DataBind()
        Catch ex As Exception
            ' Log error
            Console.WriteLine("Error cargando entidades: " & ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Genera columnas dinámicamente para un grid basándose en las columnas del DataTable.
    ''' Preserva columnas personalizadas (GridViewCommandColumn, columnas con DataItemTemplate).
    ''' </summary>
    Private Sub GenerarColumnasDinamicas(grid As ASPxGridView, tabla As DataTable)
        Try
            ' Limpiar columnas existentes (excepto las personalizadas)
            Dim columnasPersonalizadas As New List(Of GridViewColumn)()

            For Each col As GridViewColumn In grid.Columns
                If TypeOf col Is GridViewCommandColumn Then
                    columnasPersonalizadas.Add(col)
                End If
            Next

            grid.Columns.Clear()

            ' Agregar columnas dinámicas
            For Each columna As DataColumn In tabla.Columns
                Dim gridCol As GridViewDataColumn = Nothing

                Select Case columna.DataType
                    Case GetType(Boolean)
                        gridCol = New GridViewDataCheckColumn()
                    Case GetType(DateTime)
                        gridCol = New GridViewDataDateColumn()
                    Case GetType(Decimal), GetType(Double), GetType(Single)
                        gridCol = New GridViewDataSpinEditColumn()
                    Case Else
                        gridCol = New GridViewDataTextColumn()
                End Select

                gridCol.FieldName = columna.ColumnName
                gridCol.Caption = FuncionesGridWeb.SplitCamelCase(columna.ColumnName)

                grid.Columns.Add(gridCol)
            Next

            ' Re-agregar columnas personalizadas
            For Each col As GridViewColumn In columnasPersonalizadas
                grid.Columns.Add(col)
            Next

        Catch ex As Exception
            Console.WriteLine("Error generando columnas dinámicas: " & ex.Message)
        End Try
    End Sub

    Protected Sub gridEntidades_DataBound(sender As Object, e As EventArgs) Handles gridEntidades.DataBound
        Try
            ' Aplicar FuncionesGridWeb.SUMColumn para formateo automático
            If Session("dtEntidades") IsNot Nothing Then
                FuncionesGridWeb.SUMColumn(gridEntidades, Session("dtEntidades"))
            End If
        Catch ex As Exception
            Console.WriteLine("Error en DataBound: " & ex.Message)
        End Try
    End Sub

End Class