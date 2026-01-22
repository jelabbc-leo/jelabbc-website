Imports System.Linq
Imports System.Data
Imports DevExpress.Web
Imports System.Configuration
Imports JelaWeb.Utilities
Imports JelaWeb.Services
Imports JelaWeb.Infrastructure.Helpers

Partial Public Class Pagos
    Inherits BasePage

#Region "Page Events"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load

        Try
            If Not IsPostBack Then
                ' Registrar eventos manualmente
                AddHandler gridPagos.DataBound, AddressOf gridPagos_DataBound
                AddHandler gridPagos.CustomCallback, AddressOf gridPagos_CustomCallback
                AddHandler gridCuotasPendientes.CustomCallback, AddressOf gridCuotasPendientes_CustomCallback

                ' Establecer fechas por defecto (mes actual)
                dtFechaDesde.Date = New DateTime(DateTime.Now.Year, DateTime.Now.Month, 1)
                dtFechaHasta.Date = DateTime.Now.Date

                CargarCombos()
                CargarPagos()
            End If

        Catch ex As Exception
            Logger.LogError("Pagos.Page_Load", ex)
            PagosHelper.MostrarMensaje(Me, "Error al cargar la página", "error")

        End Try
    End Sub

#End Region

#Region "Carga de Datos"

    Private Sub CargarPagos()

        Try
            Dim fechaDesdeNullable As DateTime? = dtFechaDesde.Date
            Dim fechaHastaNullable As DateTime? = dtFechaHasta.Date
            Dim fechaDesde As DateTime = If(fechaDesdeNullable.HasValue, fechaDesdeNullable.Value, New DateTime(DateTime.Now.Year, DateTime.Now.Month, 1))
            Dim fechaHasta As DateTime = If(fechaHastaNullable.HasValue, fechaHastaNullable.Value, DateTime.Now.Date)

            ' Obtener datos del servicio
            Dim dt As DataTable = PagosService.ListarPagos(fechaDesde, fechaHasta)

            ' Generar columnas dinámicamente desde el DataTable
            GenerarColumnasDinamicas(gridPagos, dt)

            ' Guardar DataTable en Session ANTES de DataBind para FuncionesGridWeb
            Session("dtPagos") = dt

            gridPagos.DataSource = dt
            gridPagos.DataBind()

        Catch ex As Exception
            Logger.LogError("Pagos.CargarPagos", ex)
            PagosHelper.MostrarMensaje(Me, "Error al cargar pagos", "error")

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
            Logger.LogError("Pagos.GenerarColumnasDinamicas", ex)
            Throw

        End Try
    End Sub

    Private Sub CargarCombos()

        Try
            ' Cargar Entidades - ELIMINADO: El sistema usa IdEntidadActual automáticamente
            ' El filtrado por entidad se maneja automáticamente en DynamicCrudService

        Catch ex As Exception
            Logger.LogError("Pagos.CargarCombos", ex)
            PagosHelper.MostrarMensaje(Me, "Error al cargar combos", "error")

        End Try
    End Sub

#End Region

#Region "Eventos de Filtros"

    Protected Sub btnFiltrar_Click(sender As Object, e As EventArgs)
        CargarPagos()
    End Sub

    Protected Sub btnLimpiar_Click(sender As Object, e As EventArgs)
        dtFechaDesde.Date = New DateTime(DateTime.Now.Year, DateTime.Now.Month, 1)
        dtFechaHasta.Date = DateTime.Now.Date
        CargarPagos()
    End Sub

#End Region

#Region "Eventos del Grid"

    Protected Sub gridPagos_DataBound(sender As Object, e As EventArgs)

        Try
            ' Leer DataTable desde Session (guardado antes de DataBind)
            Dim tabla As DataTable = TryCast(Session("dtPagos"), DataTable)

            If tabla IsNot Nothing Then
                FuncionesGridWeb.SUMColumn(gridPagos, tabla)
            End If

        Catch ex As Exception
            Logger.LogError("Pagos.gridPagos_DataBound", ex)

        End Try
    End Sub

    Protected Sub gridPagos_CustomCallback(sender As Object, e As ASPxGridViewCustomCallbackEventArgs)

        Try
            Dim parametros = e.Parameters.Split("|"c)
            Dim operacion As String = parametros(0)

            Select Case operacion.ToUpper()

                Case "CARGAR"
                    CargarPagos()

                Case Else
                    CargarPagos()

            End Select

        Catch ex As Exception
            Logger.LogError("Pagos.gridPagos_CustomCallback", ex)

        End Try
    End Sub

    Protected Sub gridCuotasPendientes_CustomCallback(sender As Object, e As ASPxGridViewCustomCallbackEventArgs)

        Try
            Dim parametros = e.Parameters.Split("|"c)
            Dim operacion As String = parametros(0)

            If operacion = "cargar" AndAlso parametros.Length >= 2 Then
                Dim unidadId As Integer = Convert.ToInt32(parametros(1))
                CargarCuotasPendientes(unidadId)
            End If

        Catch ex As Exception
            Logger.LogError("Pagos.gridCuotasPendientes_CustomCallback", ex)

        End Try
    End Sub

    Private Sub CargarCuotasPendientes(unidadId As Integer)

        Try
            ' Obtener datos del servicio
            Dim dt As DataTable = PagosService.ListarCuotasPendientesGrid(unidadId)

            gridCuotasPendientes.DataSource = dt
            gridCuotasPendientes.DataBind()

        Catch ex As Exception
            Logger.LogError("Pagos.CargarCuotasPendientes", ex)

        End Try
    End Sub

#End Region

#Region "WebMethods"

    <System.Web.Services.WebMethod()>
    Public Shared Function ListarUnidades(entidadId As Integer) As Object
        Return PagosService.ListarUnidades(entidadId)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function ListarResidentes(unidadId As Integer) As Object
        Return PagosService.ListarResidentes(unidadId)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function ObtenerCuotasPendientes(unidadId As Integer) As Object
        Return PagosService.ObtenerCuotasPendientes(unidadId)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function ObtenerPago(id As Integer) As Object
        Return PagosService.ObtenerPago(id)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function GuardarPago(datos As Dictionary(Of String, Object)) As Object
        Return PagosService.GuardarPago(datos)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function AplicarPagoACuotas(pagoId As Integer, aplicaciones As List(Of Dictionary(Of String, Object))) As Object
        Return PagosService.AplicarPagoACuotas(pagoId, aplicaciones)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function EliminarPago(id As Integer) As Object
        Return PagosService.EliminarPago(id)
    End Function

#End Region

#Region "Helpers"

    ' Los Helpers se han movido a PagosHelper

#End Region

End Class
