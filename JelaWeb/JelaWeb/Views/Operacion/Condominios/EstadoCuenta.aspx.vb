Imports System.Linq
Imports System.Data
Imports DevExpress.Web
Imports System.Configuration
Imports JelaWeb.Utilities
Imports JelaWeb.Services
Imports JelaWeb.Infrastructure.Helpers

Partial Public Class EstadoCuenta
    Inherits BasePage

#Region "Page Events"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load

        Try
            If Not IsPostBack Then
                ' Registrar eventos manualmente
                AddHandler gridEstadoCuenta.DataBound, AddressOf gridEstadoCuenta_DataBound
                AddHandler gridEstadoCuenta.CustomCallback, AddressOf gridEstadoCuenta_CustomCallback

                ' Establecer fechas por defecto (últimos 3 meses)
                dtFechaDesde.Date = New DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(-3)
                dtFechaHasta.Date = DateTime.Now.Date

                CargarCombos()
                ' No cargar datos hasta que se seleccione una unidad
            End If

        Catch ex As Exception
            Logger.LogError("EstadoCuenta.Page_Load", ex)
            EstadoCuentaHelper.MostrarMensaje(Me, "Error al cargar la página", "error")

        End Try
    End Sub

#End Region

#Region "Carga de Datos"

    Private Sub CargarEstadoCuenta()

        Try
            Dim unidadId As Integer = If(cboFiltroUnidad.Value IsNot Nothing, CInt(cboFiltroUnidad.Value), 0)

            If unidadId = 0 Then
                EstadoCuentaHelper.MostrarMensaje(Me, "Seleccione una unidad para consultar el estado de cuenta", "warning")
                Return
            End If

            Dim fechaDesdeNullable As DateTime? = dtFechaDesde.Date
            Dim fechaHastaNullable As DateTime? = dtFechaHasta.Date
            Dim fechaDesde As DateTime = If(fechaDesdeNullable.HasValue, fechaDesdeNullable.Value, New DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(-3))
            Dim fechaHasta As DateTime = If(fechaHastaNullable.HasValue, fechaHastaNullable.Value, DateTime.Now.Date)

            ' Obtener datos del servicio
            Dim dt As DataTable = EstadoCuentaService.ObtenerEstadoCuenta(unidadId, fechaDesde, fechaHasta)

            ' Generar columnas dinámicamente desde el DataTable
            GenerarColumnasDinamicas(gridEstadoCuenta, dt)

            ' Guardar DataTable en Session ANTES de DataBind para FuncionesGridWeb
            Session("dtEstadoCuenta") = dt

            gridEstadoCuenta.DataSource = dt
            gridEstadoCuenta.DataBind()

            ' Calcular resumen
            CalcularResumen(unidadId, fechaDesde, fechaHasta)

        Catch ex As Exception
            Logger.LogError("EstadoCuenta.CargarEstadoCuenta", ex)
            EstadoCuentaHelper.MostrarMensaje(Me, "Error al cargar estado de cuenta", "error")

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

                ' Omitir columnas internas
                If nombreColumna.Equals("Id", StringComparison.OrdinalIgnoreCase) OrElse
                   nombreColumna.Equals("UnidadId", StringComparison.OrdinalIgnoreCase) Then
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

                ' Configurar filtros y agrupación según estándares
                gridCol.Settings.AllowHeaderFilter = True
                gridCol.Settings.AllowGroup = True
                gridCol.Visible = True

                grid.Columns.Add(gridCol)
            Next

        Catch ex As Exception
            Logger.LogError("EstadoCuenta.GenerarColumnasDinamicas", ex)
            Throw

        End Try
    End Sub

    Private Sub CalcularResumen(unidadId As Integer, fechaDesde As Date, fechaHasta As Date)

        Try
            ' Obtener resumen del servicio
            Dim resumen As Dictionary(Of String, Object) = EstadoCuentaService.CalcularResumen(unidadId, fechaDesde, fechaHasta)

            Dim saldoAnterior As Decimal = If(resumen.ContainsKey("SaldoAnterior"), Convert.ToDecimal(resumen("SaldoAnterior")), 0)
            Dim totalCargos As Decimal = If(resumen.ContainsKey("TotalCargos"), Convert.ToDecimal(resumen("TotalCargos")), 0)
            Dim totalPagos As Decimal = If(resumen.ContainsKey("TotalPagos"), Convert.ToDecimal(resumen("TotalPagos")), 0)
            Dim saldoActual As Decimal = If(resumen.ContainsKey("SaldoActual"), Convert.ToDecimal(resumen("SaldoActual")), 0)
            Dim cuotasPendientes As Integer = If(resumen.ContainsKey("CuotasPendientes"), Convert.ToInt32(resumen("CuotasPendientes")), 0)
            Dim cuotasVencidas As Integer = If(resumen.ContainsKey("CuotasVencidas"), Convert.ToInt32(resumen("CuotasVencidas")), 0)

            ' Actualizar labels
            lblSaldoAnterior.Text = saldoAnterior.ToString("C2")
            lblCargos.Text = totalCargos.ToString("C2")
            lblPagos.Text = totalPagos.ToString("C2")
            lblSaldoActual.Text = saldoActual.ToString("C2")
            lblCuotasPendientes.Text = cuotasPendientes.ToString()
            lblCuotasVencidas.Text = cuotasVencidas.ToString()

            ' Colorear saldo actual según sea positivo o negativo
            If saldoActual < 0 Then
                lblSaldoActual.ForeColor = System.Drawing.Color.FromArgb(220, 53, 69) ' Rojo
            ElseIf saldoActual > 0 Then
                lblSaldoActual.ForeColor = System.Drawing.Color.FromArgb(255, 193, 7) ' Amarillo
            Else
                lblSaldoActual.ForeColor = System.Drawing.Color.FromArgb(40, 167, 69) ' Verde
            End If

        Catch ex As Exception
            Logger.LogError("EstadoCuenta.CalcularResumen", ex)

        End Try
    End Sub

    Private Sub CargarCombos()

        Try
            ' Cargar Entidades - ELIMINADO: El sistema usa IdEntidadActual automáticamente
            ' El filtrado por entidad se maneja automáticamente en DynamicCrudService

        Catch ex As Exception
            Logger.LogError("EstadoCuenta.CargarCombos", ex)
            EstadoCuentaHelper.MostrarMensaje(Me, "Error al cargar combos", "error")

        End Try
    End Sub

#End Region

#Region "Eventos de Filtros"

    Protected Sub btnConsultar_Click(sender As Object, e As EventArgs)
        CargarEstadoCuenta()
    End Sub

    Protected Sub btnLimpiar_Click(sender As Object, e As EventArgs)
        ' cboFiltroEntidad eliminado - El sistema usa IdEntidadActual automáticamente
        cboFiltroUnidad.Value = Nothing
        dtFechaDesde.Date = New DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(-3)
        dtFechaHasta.Date = DateTime.Now.Date
        gridEstadoCuenta.DataSource = Nothing
        gridEstadoCuenta.DataBind()

        ' Limpiar resumen
        lblSaldoAnterior.Text = "$0.00"
        lblCargos.Text = "$0.00"
        lblPagos.Text = "$0.00"
        lblSaldoActual.Text = "$0.00"
        lblCuotasPendientes.Text = "0"
        lblCuotasVencidas.Text = "0"
    End Sub

#End Region

#Region "Eventos del Grid"

    Protected Sub gridEstadoCuenta_DataBound(sender As Object, e As EventArgs)

        Try
            ' Leer DataTable desde Session (guardado antes de DataBind)
            Dim tabla As DataTable = TryCast(Session("dtEstadoCuenta"), DataTable)

            If tabla IsNot Nothing Then
                FuncionesGridWeb.SUMColumn(gridEstadoCuenta, tabla)
            End If

        Catch ex As Exception
            Logger.LogError("EstadoCuenta.gridEstadoCuenta_DataBound", ex)

        End Try
    End Sub

    Protected Sub gridEstadoCuenta_CustomCallback(sender As Object, e As ASPxGridViewCustomCallbackEventArgs)

        Try
            Dim parametros = e.Parameters.Split("|"c)
            Dim operacion As String = parametros(0)

            Select Case operacion.ToUpper()

                Case "CARGAR"
                    CargarEstadoCuenta()

                Case Else
                    CargarEstadoCuenta()

            End Select

        Catch ex As Exception
            Logger.LogError("EstadoCuenta.gridEstadoCuenta_CustomCallback", ex)

        End Try
    End Sub

    Protected Sub gridDetalleMovimiento_BeforePerformDataSelect(sender As Object, e As EventArgs)
        ' Este método se ejecuta cuando se expande un detalle
        ' Puede usarse para cargar detalles adicionales si es necesario
    End Sub

#End Region

#Region "WebMethods"

    <System.Web.Services.WebMethod()>
    Public Shared Function ListarUnidades(entidadId As Integer) As Object
        Return EstadoCuentaService.ListarUnidades(entidadId)
    End Function

#End Region

#Region "Helpers"

    ' Los Helpers se han movido a EstadoCuentaHelper

#End Region

End Class
