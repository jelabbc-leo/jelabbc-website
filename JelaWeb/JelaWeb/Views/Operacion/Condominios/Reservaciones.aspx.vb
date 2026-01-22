Imports System.Linq
Imports System.Data
Imports DevExpress.Web
Imports System.Configuration
Imports DevExpress.Web.ASPxScheduler
Imports DevExpress.XtraCharts
Imports DevExpress.XtraCharts.Web
Imports JelaWeb.Utilities
Imports JelaWeb.Services
Imports JelaWeb.Infrastructure.Helpers

Partial Public Class Reservaciones
    Inherits BasePage

#Region "Page Events"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load

        Try
            ConfigurarVistasScheduler()

            If Not IsPostBack Then
                ' Establecer fechas por defecto (mes actual)
                dtFechaDesde.Date = New DateTime(DateTime.Now.Year, DateTime.Now.Month, 1)
                dtFechaHasta.Date = DateTime.Now.Date.AddMonths(1).AddDays(-1)

                CargarCombos()
                CargarReservaciones()
            End If

        Catch ex As Exception
            Logger.LogError("Reservaciones.Page_Load", ex)
            ReservacionesHelper.MostrarMensaje(Me, "Error al cargar la página", "error")

        End Try
    End Sub

#End Region

#Region "Carga de Datos"

    Private Sub CargarReservaciones()

        Try
            Dim fechaDesdeNullable As DateTime? = dtFechaDesde.Date
            Dim fechaHastaNullable As DateTime? = dtFechaHasta.Date
            Dim fechaDesde As DateTime = If(fechaDesdeNullable.HasValue, fechaDesdeNullable.Value, New DateTime(DateTime.Now.Year, DateTime.Now.Month, 1))
            Dim fechaHasta As DateTime = If(fechaHastaNullable.HasValue, fechaHastaNullable.Value, DateTime.Now.Date.AddMonths(1).AddDays(-1))

            ' Obtener entidadId desde la sesión del usuario
            Dim currentUser = SessionHelper.GetCurrentUser()
            Dim entidadId As Integer = If(currentUser IsNot Nothing AndAlso currentUser.EntidadId.HasValue, currentUser.EntidadId.Value, 0)

            ' Obtener datos del servicio
            Dim dt As DataTable = ReservacionesService.ListarReservaciones(entidadId, fechaDesde, fechaHasta)

            Dim dtScheduler As DataTable = ConstruirDataTableScheduler(dt)

            schedulerReservaciones.Start = fechaDesde
            schedulerReservaciones.Storage.Appointments.DataSource = dtScheduler
            schedulerReservaciones.DataBind()

            CargarDashboard(dt)

        Catch ex As Exception
            Logger.LogError("Reservaciones.CargarReservaciones", ex)
            ReservacionesHelper.MostrarMensaje(Me, "Error al cargar reservaciones", "error")

        End Try
    End Sub

    Private Function ConstruirDataTableScheduler(tabla As DataTable) As DataTable

        Dim schedulerTable As New DataTable()
        schedulerTable.Columns.Add("Id", GetType(Integer))
        schedulerTable.Columns.Add("StartDate", GetType(DateTime))
        schedulerTable.Columns.Add("EndDate", GetType(DateTime))
        schedulerTable.Columns.Add("Subject", GetType(String))
        schedulerTable.Columns.Add("Description", GetType(String))
        schedulerTable.Columns.Add("Location", GetType(String))

        If tabla Is Nothing OrElse tabla.Rows.Count = 0 Then
            Return schedulerTable
        End If

        For Each row As DataRow In tabla.Rows
            Dim id As Integer = If(IsDBNull(row("Id")), 0, Convert.ToInt32(row("Id")))
            Dim fecha As DateTime = If(IsDBNull(row("FechaReservacion")), DateTime.Today, Convert.ToDateTime(row("FechaReservacion")))
            Dim areaComun As String = If(IsDBNull(row("AreaComunNombre")), "", row("AreaComunNombre").ToString())
            Dim unidadCodigo As String = If(IsDBNull(row("UnidadCodigo")), "", row("UnidadCodigo").ToString())
            Dim residente As String = If(IsDBNull(row("ResidenteNombre")), "", row("ResidenteNombre").ToString())
            Dim observaciones As String = If(IsDBNull(row("Observaciones")), "", row("Observaciones").ToString())
            Dim horaInicio As TimeSpan = TimeSpan.FromHours(8)
            Dim horaFin As TimeSpan = TimeSpan.FromHours(9)

            If Not IsDBNull(row("HoraInicio")) Then
                TimeSpan.TryParse(row("HoraInicio").ToString(), horaInicio)
            End If

            If Not IsDBNull(row("HoraFin")) Then
                TimeSpan.TryParse(row("HoraFin").ToString(), horaFin)
            End If

            Dim subject As String = areaComun

            If Not String.IsNullOrWhiteSpace(unidadCodigo) Then
                subject &= " - " & unidadCodigo
            End If

            If Not String.IsNullOrWhiteSpace(residente) Then
                subject &= " - " & residente
            End If

            Dim startDate As DateTime = fecha.Date.Add(horaInicio)
            Dim endDate As DateTime = fecha.Date.Add(horaFin)

            If endDate <= startDate Then
                endDate = startDate.AddHours(1)
            End If

            Dim newRow = schedulerTable.NewRow()
            newRow("Id") = id
            newRow("StartDate") = startDate
            newRow("EndDate") = endDate
            newRow("Subject") = subject
            newRow("Description") = observaciones
            newRow("Location") = areaComun

            schedulerTable.Rows.Add(newRow)
        Next

        Return schedulerTable

    End Function

    Private Sub CargarCombos()

        Try
            ' Cargar Entidades - ELIMINADO: El sistema usa IdEntidadActual automáticamente
            ' El filtrado por entidad se maneja automáticamente en DynamicCrudService

        Catch ex As Exception
            Logger.LogError("Reservaciones.CargarCombos", ex)
            ReservacionesHelper.MostrarMensaje(Me, "Error al cargar combos", "error")

        End Try
    End Sub

    Private Sub ConfigurarVistasScheduler()
        Try
            Dim cultureName = Threading.Thread.CurrentThread.CurrentUICulture.Name
            Dim isSpanish = cultureName.StartsWith("es", StringComparison.OrdinalIgnoreCase)

            If isSpanish Then
                schedulerReservaciones.Views.DayView.DisplayName = "Día"
                schedulerReservaciones.Views.DayView.ShortDisplayName = "Día"
                schedulerReservaciones.Views.WorkWeekView.DisplayName = "Semana laboral"
                schedulerReservaciones.Views.WorkWeekView.ShortDisplayName = "Sem. laboral"
                schedulerReservaciones.Views.WeekView.DisplayName = "Semana"
                schedulerReservaciones.Views.WeekView.ShortDisplayName = "Semana"
                schedulerReservaciones.Views.MonthView.DisplayName = "Mes"
                schedulerReservaciones.Views.MonthView.ShortDisplayName = "Mes"
                schedulerReservaciones.Views.TimelineView.DisplayName = "Línea de tiempo"
                schedulerReservaciones.Views.TimelineView.ShortDisplayName = "Línea de tiempo"
                schedulerReservaciones.Views.AgendaView.DisplayName = "Agenda"
                schedulerReservaciones.Views.AgendaView.ShortDisplayName = "Agenda"
            Else
                schedulerReservaciones.Views.DayView.DisplayName = "Day"
                schedulerReservaciones.Views.DayView.ShortDisplayName = "Day"
                schedulerReservaciones.Views.WorkWeekView.DisplayName = "Work Week"
                schedulerReservaciones.Views.WorkWeekView.ShortDisplayName = "Work Week"
                schedulerReservaciones.Views.WeekView.DisplayName = "Week"
                schedulerReservaciones.Views.WeekView.ShortDisplayName = "Week"
                schedulerReservaciones.Views.MonthView.DisplayName = "Month"
                schedulerReservaciones.Views.MonthView.ShortDisplayName = "Month"
                schedulerReservaciones.Views.TimelineView.DisplayName = "Timeline"
                schedulerReservaciones.Views.TimelineView.ShortDisplayName = "Timeline"
                schedulerReservaciones.Views.AgendaView.DisplayName = "Agenda"
                schedulerReservaciones.Views.AgendaView.ShortDisplayName = "Agenda"
            End If
        Catch ex As Exception
            Logger.LogError("Reservaciones.ConfigurarVistasScheduler", ex)
        End Try
    End Sub

#End Region

#Region "Eventos de Filtros"

    Protected Sub btnFiltrar_Click(sender As Object, e As EventArgs)
        schedulerReservaciones.Start = dtFechaDesde.Date
        CargarReservaciones()
    End Sub

    Protected Sub btnLimpiar_Click(sender As Object, e As EventArgs)
        dtFechaDesde.Date = New DateTime(DateTime.Now.Year, DateTime.Now.Month, 1)
        dtFechaHasta.Date = DateTime.Now.Date.AddMonths(1).AddDays(-1)
        schedulerReservaciones.Start = dtFechaDesde.Date
        CargarReservaciones()
    End Sub

#End Region

#Region "Eventos del Calendario"

    Protected Sub schedulerReservaciones_AppointmentFormShowing(ByVal sender As Object, ByVal e As AppointmentFormEventArgs)
        e.Cancel = True
    End Sub

    Protected Sub schedulerReservaciones_VisibleIntervalChanged(ByVal sender As Object, ByVal e As EventArgs)
        Dim baseDate As DateTime = schedulerReservaciones.Start
        Dim fechaDesde As New DateTime(baseDate.Year, baseDate.Month, 1)
        Dim fechaHasta As DateTime = fechaDesde.AddMonths(1).AddDays(-1)

        dtFechaDesde.Date = fechaDesde
        dtFechaHasta.Date = fechaHasta

        CargarReservaciones()
    End Sub

#End Region

#Region "Dashboard"

    Private Sub CargarDashboard(tabla As DataTable)
        Try
            Dim totalMes As Integer = If(tabla IsNot Nothing, tabla.Rows.Count, 0)
            Dim abiertas As Integer = ContarPorEstado(tabla, New String() {"Pendiente", "Confirmada"})
            Dim canceladas As Integer = ContarPorEstado(tabla, New String() {"Cancelada"})
            Dim enUso As Integer = ContarPorEstado(tabla, New String() {"EnUso"})
            Dim completadas As Integer = ContarPorEstado(tabla, New String() {"Completada"})
            Dim proximas As Integer = ContarPorRangoFechas(tabla, DateTime.Today, DateTime.Today.AddDays(7))

            kpiTotalMes.InnerText = totalMes.ToString()
            kpiAbiertas.InnerText = abiertas.ToString()
            kpiCanceladas.InnerText = canceladas.ToString()
            kpiEnUso.InnerText = enUso.ToString()
            kpiCompletadas.InnerText = completadas.ToString()
            kpiProximas.InnerText = proximas.ToString()

            CargarGraficaEstados(tabla)

            Dim dtAreas As DataTable = ConstruirResumenPorArea(tabla)
            gridAreasResumen.DataSource = dtAreas
            gridAreasResumen.DataBind()

        Catch ex As Exception
            Logger.LogError("Reservaciones.CargarDashboard", ex)
        End Try
    End Sub

    Private Function ContarPorEstado(tabla As DataTable, estados As IEnumerable(Of String)) As Integer
        If tabla Is Nothing OrElse tabla.Rows.Count = 0 OrElse Not tabla.Columns.Contains("Estado") Then
            Return 0
        End If

        Dim estadoSet = New HashSet(Of String)(estados, StringComparer.OrdinalIgnoreCase)

        Return tabla.AsEnumerable().Count(Function(r)
                                             Dim valor = If(r("Estado") IsNot DBNull.Value, r("Estado").ToString(), "")
                                             Return estadoSet.Contains(valor)
                                         End Function)
    End Function

    Private Function ContarPorRangoFechas(tabla As DataTable, fechaInicio As DateTime, fechaFin As DateTime) As Integer
        If tabla Is Nothing OrElse tabla.Rows.Count = 0 OrElse Not tabla.Columns.Contains("FechaReservacion") Then
            Return 0
        End If

        Return tabla.AsEnumerable().Count(Function(r)
                                             If r("FechaReservacion") Is DBNull.Value Then
                                                 Return False
                                             End If

                                             Dim fecha As DateTime = Convert.ToDateTime(r("FechaReservacion"))
                                             Return fecha.Date >= fechaInicio.Date AndAlso fecha.Date <= fechaFin.Date
                                         End Function)
    End Function

    Private Sub CargarGraficaEstados(tabla As DataTable)
        Try
            chartReservacionesEstado.Series.Clear()

            If tabla Is Nothing OrElse tabla.Rows.Count = 0 OrElse Not tabla.Columns.Contains("Estado") Then
                chartReservacionesEstado.Visible = False
                Return
            End If

            Dim resumen = tabla.AsEnumerable().GroupBy(Function(r)
                                                           Dim estado = If(r("Estado") IsNot DBNull.Value, r("Estado").ToString(), "Sin estado")
                                                           Return If(String.IsNullOrWhiteSpace(estado), "Sin estado", estado)
                                                       End Function)

            Dim series As New Series("Reservaciones por estado", ViewType.Pie)

            For Each grupo In resumen
                Dim total = grupo.Count()

                If total > 0 Then
                    series.Points.Add(New SeriesPoint(grupo.Key, total))
                End If

            Next

            Dim pieLabel = DirectCast(series.Label, PieSeriesLabel)

            pieLabel.TextPattern = "{A}: {V:F0} ({VP:P0})"
            pieLabel.Position = PieSeriesLabelPosition.TwoColumns

            chartReservacionesEstado.Series.Add(series)
            chartReservacionesEstado.Visible = True

        Catch ex As Exception
            Logger.LogError("Reservaciones.CargarGraficaEstados", ex)
            chartReservacionesEstado.Visible = False
        End Try
    End Sub

    Private Function ConstruirResumenPorArea(tabla As DataTable) As DataTable
        Dim dt As New DataTable()
        dt.Columns.Add("AreaComun", GetType(String))
        dt.Columns.Add("Total", GetType(Integer))
        dt.Columns.Add("Abiertas", GetType(Integer))
        dt.Columns.Add("Canceladas", GetType(Integer))
        dt.Columns.Add("EnUso", GetType(Integer))
        dt.Columns.Add("Completadas", GetType(Integer))

        If tabla Is Nothing OrElse tabla.Rows.Count = 0 Then
            Return dt
        End If

        Dim grupos = tabla.AsEnumerable().GroupBy(Function(r)
                                                      Dim area = If(r.Table.Columns.Contains("AreaComunNombre") AndAlso r("AreaComunNombre") IsNot DBNull.Value, r("AreaComunNombre").ToString(), "")
                                                      Return If(String.IsNullOrWhiteSpace(area), "Sin área", area)
                                                  End Function)

        For Each grupo In grupos
            Dim fila = dt.NewRow()
            fila("AreaComun") = grupo.Key
            fila("Total") = grupo.Count()
            fila("Abiertas") = grupo.Count(Function(r)
                                               Dim estado = If(r("Estado") IsNot DBNull.Value, r("Estado").ToString(), "")
                                               Return estado.Equals("Pendiente", StringComparison.OrdinalIgnoreCase) OrElse estado.Equals("Confirmada", StringComparison.OrdinalIgnoreCase)
                                           End Function)
            fila("Canceladas") = grupo.Count(Function(r) If(r("Estado") IsNot DBNull.Value, r("Estado").ToString(), "").Equals("Cancelada", StringComparison.OrdinalIgnoreCase))
            fila("EnUso") = grupo.Count(Function(r) If(r("Estado") IsNot DBNull.Value, r("Estado").ToString(), "").Equals("EnUso", StringComparison.OrdinalIgnoreCase))
            fila("Completadas") = grupo.Count(Function(r) If(r("Estado") IsNot DBNull.Value, r("Estado").ToString(), "").Equals("Completada", StringComparison.OrdinalIgnoreCase))

            dt.Rows.Add(fila)
        Next

        Return dt
    End Function

#End Region

#Region "WebMethods"

    <System.Web.Services.WebMethod()>
    Public Shared Function ObtenerReservacion(id As Integer) As Object
        Return ReservacionesService.ObtenerReservacion(id)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function GuardarReservacion(datos As Dictionary(Of String, Object)) As Object
        Return ReservacionesService.GuardarReservacion(datos)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function EliminarReservacion(id As Integer) As Object
        Return ReservacionesService.EliminarReservacion(id)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function ListarAreasComunes(entidadId As Integer) As Object
        Return ReservacionesService.ListarAreasComunes(entidadId)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function ListarUnidades(entidadId As Integer) As Object
        Return ReservacionesService.ListarUnidades(entidadId)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function ListarResidentes(unidadId As Integer) As Object
        Return ReservacionesService.ListarResidentes(unidadId)
    End Function

#End Region

#Region "Helpers"

    ' Los Helpers se han movido a ReservacionesHelper

#End Region

End Class
