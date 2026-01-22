Imports System.Linq
Imports System.Data
Imports DevExpress.Web
Imports System.Configuration
Imports JelaWeb.Utilities
Imports JelaWeb.Services
Imports JelaWeb.Infrastructure.Helpers

Partial Public Class Cuotas
    Inherits BasePage

#Region "Page Events"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load

        Try
            If Not IsPostBack Then
                ' Registrar eventos manualmente
                AddHandler gridCuotas.DataBound, AddressOf gridCuotas_DataBound
                AddHandler gridCuotas.CustomCallback, AddressOf gridCuotas_CustomCallback

                ' Establecer fechas por defecto (mes actual)
                dtFechaDesde.Date = New DateTime(DateTime.Now.Year, DateTime.Now.Month, 1)
                dtFechaHasta.Date = DateTime.Now.Date

                CargarCombos()
                CargarCuotas()
            End If

        Catch ex As Exception
            Logger.LogError("Cuotas.Page_Load", ex)
            CuotasHelper.MostrarMensaje(Me, "Error al cargar la página", "error")

        End Try
    End Sub

#End Region

#Region "Carga de Datos"

    Private Sub CargarCuotas()

        Try
            Dim fechaDesdeNullable As DateTime? = dtFechaDesde.Date
            Dim fechaHastaNullable As DateTime? = dtFechaHasta.Date
            Dim fechaDesde As DateTime = If(fechaDesdeNullable.HasValue, fechaDesdeNullable.Value, New DateTime(DateTime.Now.Year, DateTime.Now.Month, 1))
            Dim fechaHasta As DateTime = If(fechaHastaNullable.HasValue, fechaHastaNullable.Value, DateTime.Now.Date)

            ' Obtener datos del servicio
            Dim dt As DataTable = CuotasService.ListarCuotas(fechaDesde, fechaHasta)

            ' Generar columnas dinámicamente desde el DataTable
            GenerarColumnasDinamicas(gridCuotas, dt)

            ' Guardar DataTable en Session ANTES de DataBind para FuncionesGridWeb
            Session("dtCuotas") = dt

            gridCuotas.DataSource = dt
            gridCuotas.DataBind()

        Catch ex As Exception
            Logger.LogError("Cuotas.CargarCuotas", ex)
            CuotasHelper.MostrarMensaje(Me, "Error al cargar cuotas", "error")

        End Try
    End Sub

    ''' <summary>
    ''' Genera columnas dinámicamente para un grid basándose en las columnas del DataTable.
    ''' Preserva columnas personalizadas (GridViewCommandColumn, columnas con DataItemTemplate).
    ''' </summary>
    Private Sub GenerarColumnasDinamicas(grid As ASPxGridView, tabla As DataTable)

        Try
            If tabla Is Nothing OrElse tabla.Columns.Count = 0 Then Return

            ' Limpiar columnas previas (excepto columnas personalizadas)
            Dim indicesColumnasParaMantener As New List(Of Integer)

            ' Guardar índices de columnas personalizadas antes de limpiar

            For i As Integer = 0 To grid.Columns.Count - 1
                Dim col As GridViewColumn = grid.Columns(i)
                Dim debeMantener As Boolean = False

                ' Mantener GridViewCommandColumn (botones de acciones)
                If TypeOf col Is GridViewCommandColumn Then
                    debeMantener = True
                ElseIf TypeOf col Is GridViewDataColumn Then
                    Dim dataCol = CType(col, GridViewDataColumn)

                    ' Mantener columnas con DataItemTemplate
                    If dataCol.DataItemTemplate IsNot Nothing Then
                        debeMantener = True
                    End If
                End If

                If debeMantener Then
                    indicesColumnasParaMantener.Add(i)
                End If
            Next

            ' Limpiar solo las columnas de datos, no las personalizadas

            For i As Integer = grid.Columns.Count - 1 To 0 Step -1
                If Not indicesColumnasParaMantener.Contains(i) Then
                    grid.Columns.RemoveAt(i)
                End If
            Next

            ' Crear columnas dinámicamente desde el DataTable

            For Each col As DataColumn In tabla.Columns
                Dim nombreColumna = col.ColumnName

                ' Omitir columna Id (se ocultará)
                If nombreColumna.Equals("Id", StringComparison.OrdinalIgnoreCase) Then
                    Continue For
                End If

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
                        gridCol.Width = Unit.Pixel(120)
                        gridCol.PropertiesEdit.DisplayFormatString = "c2"
                        gridCol.CellStyle.HorizontalAlign = HorizontalAlign.Right

                    Case GetType(Integer), GetType(Long), GetType(Short)
                        gridCol = New GridViewDataTextColumn()
                        gridCol.Width = Unit.Pixel(100)
                        gridCol.PropertiesEdit.DisplayFormatString = "n0"

                    Case Else
                        gridCol = New GridViewDataTextColumn()
                        gridCol.Width = Unit.Pixel(150)

                End Select

                gridCol.FieldName = nombreColumna
                gridCol.Caption = nombreColumna ' FuncionesGridWeb.SUMColumn aplicará SplitCamelCase
                gridCol.ReadOnly = True

                ' Ocultar columna Id si existe
                If nombreColumna.Equals("Id", StringComparison.OrdinalIgnoreCase) Then
                    gridCol.Visible = False
                Else
                    gridCol.Visible = True
                    ' Configurar filtros y agrupación según estándares
                    gridCol.Settings.AllowHeaderFilter = True
                    gridCol.Settings.AllowGroup = True
                End If

                grid.Columns.Add(gridCol)
            Next

        Catch ex As Exception
            Logger.LogError("Cuotas.GenerarColumnasDinamicas", ex)
            Throw

        End Try
    End Sub

    Private Sub CargarCombos()

        Try
            ' Cargar Entidades eliminado - El sistema usa IdEntidadActual automáticamente
            ' Los combos de Unidades, Conceptos y Residentes se cargan dinámicamente desde JavaScript

        Catch ex As Exception
            Logger.LogError("Cuotas.CargarCombos", ex)
            CuotasHelper.MostrarMensaje(Me, "Error al cargar combos", "error")

        End Try
    End Sub

#End Region

#Region "Eventos de Filtros"

    Protected Sub btnFiltrar_Click(sender As Object, e As EventArgs)
        CargarCuotas()
    End Sub

    Protected Sub btnLimpiar_Click(sender As Object, e As EventArgs)
        dtFechaDesde.Date = New DateTime(DateTime.Now.Year, DateTime.Now.Month, 1)
        dtFechaHasta.Date = DateTime.Now.Date
        CargarCuotas()
    End Sub

#End Region

#Region "Eventos del Grid"

    Protected Sub gridCuotas_DataBound(sender As Object, e As EventArgs)

        Try
            ' Leer DataTable desde Session (guardado antes de DataBind)
            Dim tabla As DataTable = TryCast(Session("dtCuotas"), DataTable)

            If tabla IsNot Nothing Then
                FuncionesGridWeb.SUMColumn(gridCuotas, tabla)
            End If

        Catch ex As Exception
            Logger.LogError("Cuotas.gridCuotas_DataBound", ex)

        End Try
    End Sub

    Protected Sub gridCuotas_CustomCallback(sender As Object, e As ASPxGridViewCustomCallbackEventArgs)

        Try
            Dim parametros = e.Parameters.Split("|"c)
            Dim operacion As String = parametros(0)

            Select Case operacion.ToUpper()

                Case "CARGAR"
                    CargarCuotas()

                Case Else
                    CargarCuotas()

            End Select

        Catch ex As Exception
            Logger.LogError("Cuotas.gridCuotas_CustomCallback", ex)

        End Try
    End Sub

#End Region

#Region "WebMethods"

    <System.Web.Services.WebMethod()>
    Public Shared Function ListarUnidades(entidadId As Integer) As Object
        Return CuotasService.ListarUnidades(entidadId)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function ListarConceptosCuota(entidadId As Integer) As Object
        Return CuotasService.ListarConceptosCuota(entidadId)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function ListarResidentes(unidadId As Integer) As Object
        Return CuotasService.ListarResidentes(unidadId)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function ObtenerCuota(id As Integer) As Object
        Return CuotasService.ObtenerCuota(id)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function GuardarCuota(datos As Dictionary(Of String, Object)) As Object
        Return CuotasService.GuardarCuota(datos)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function EliminarCuota(id As Integer) As Object
        Return CuotasService.EliminarCuota(id)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function GenerarCuotasMasivas(datos As Dictionary(Of String, Object)) As Object
        Return CuotasService.GenerarCuotasMasivas(datos)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function AplicarRecargosMora() As Object
        Return CuotasService.AplicarRecargosMora()
    End Function

#End Region

#Region "Helpers"

    ' Los Helpers se han movido a CuotasHelper

#End Region

End Class