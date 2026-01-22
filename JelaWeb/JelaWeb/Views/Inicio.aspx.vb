Imports System.Collections.Generic
Imports DevExpress.XtraCharts
Imports DevExpress.XtraCharts.Web
Imports System.Linq
Imports JelaWeb.Business.Operacion

Public Class Inicio
    Inherits BasePage

    Private dashboardBusiness As DashboardBusiness

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' Aplicar localización
        LocalizationHelper.ApplyCulture()

        ' Cargar textos localizados siempre (tanto en PostBack como no-PostBack)
        LoadLocalizedTexts()

        If Not IsPostBack Then
            ' Inicializar servicio de dashboard
            Dim apiBaseUrl = ConfigHelper.ApiBaseUrl

            If String.IsNullOrEmpty(apiBaseUrl) Then
                Logger.LogError("ApiBaseUrl no está configurado en Web.config", Nothing, "")
                Return
            End If
            dashboardBusiness = New DashboardBusiness(apiBaseUrl)

            ' Cargar datos del dashboard
            LoadDashboardData()
        End If
    End Sub

    ''' <summary>
    ''' Carga los datos del dashboard según el rol del usuario
    ''' </summary>
    Private Sub LoadDashboardData()

        Try
            Dim userId = SessionHelper.GetUserId()

            If Not userId.HasValue Then
                Logger.LogWarning("Usuario no autenticado intentando acceder al dashboard", "")
                Response.Redirect(Constants.ROUTE_LOGIN, True)
                Return
            End If

            ' Obtener rol del usuario (por defecto asumimos rol 1 = Admin si no está disponible)
            ' En un sistema real, esto vendría de la sesión o API
            Dim rolId As Integer? = GetUserRoleId()
            Dim entidadId As Integer? = Nothing ' Se puede obtener de la sesión si está disponible

            ' Obtener métricas
            Dim metrics = dashboardBusiness.GetDashboardMetrics(userId.Value, rolId, entidadId)

            ' Actualizar cards con métricas
            metricTicketsAbiertos.InnerHtml = metrics.TicketsAbiertos.ToString()
            metricTicketsCerrados.InnerHtml = metrics.TicketsCerrados.ToString()
            metricTicketsEnProceso.InnerHtml = metrics.TicketsEnProceso.ToString()
            metricTicketsPendientes.InnerHtml = metrics.TicketsPendientes.ToString()
            metricTicketsUrgentes.InnerHtml = metrics.TicketsUrgentes.ToString()
            metricTotalTickets.InnerHtml = metrics.TotalTickets.ToString()

            ' Cargar gráficas
            LoadStatusChart(metrics)
            LoadMonthlyChart()

        Catch ex As Exception
            Logger.LogError("Error al cargar datos del dashboard", ex, SessionHelper.GetNombre())
            ' Mostrar valores por defecto en caso de error
            metricTicketsAbiertos.InnerHtml = "0"
            metricTicketsCerrados.InnerHtml = "0"
            metricTicketsEnProceso.InnerHtml = "0"
            metricTicketsPendientes.InnerHtml = "0"
            metricTicketsUrgentes.InnerHtml = "0"
            metricTotalTickets.InnerHtml = "0"

        End Try
    End Sub

    ''' <summary>
    ''' Carga la gráfica de tickets por estado
    ''' </summary>
    Private Sub LoadStatusChart(metrics As Global.JelaWeb.DashboardMetricsDTO)

        Try
            ' Obtener datos de la gráfica
            Dim userId = SessionHelper.GetUserId()

            If Not userId.HasValue Then Return

            Dim rolId As Integer? = GetUserRoleId()
            Dim entidadId As Integer? = Nothing
            Dim chartData = dashboardBusiness.GetTicketsByStatusChartData(userId.Value, rolId, entidadId)

            ' Si no hay datos, usar métricas directas
            If chartData Is Nothing OrElse chartData.Count = 0 Then
                chartData = New List(Of Global.JelaWeb.ChartDataPointDTO) From {
                    New Global.JelaWeb.ChartDataPointDTO With {.Label = Global.JelaWeb.LocalizationHelper.GetString("MetricTicketsAbiertos"), .Value = metrics.TicketsAbiertos},
                    New Global.JelaWeb.ChartDataPointDTO With {.Label = Global.JelaWeb.LocalizationHelper.GetString("MetricTicketsEnProceso"), .Value = metrics.TicketsEnProceso},
                    New Global.JelaWeb.ChartDataPointDTO With {.Label = Global.JelaWeb.LocalizationHelper.GetString("MetricTicketsCerrados"), .Value = metrics.TicketsCerrados}
                }
            End If

            ' Configurar la gráfica
            chartTicketsByStatus.Series.Clear()

            Dim series As New Series(Global.JelaWeb.LocalizationHelper.GetString("ChartTicketsByStatus"), ViewType.Pie)

            For Each point In chartData
                If point.Value > 0 Then ' Solo agregar puntos con valor > 0
                    series.Points.Add(New SeriesPoint(point.Label, point.Value))
                End If

            Next

            ' Configurar vista de gráfica de pastel
            Dim pieView = DirectCast(series.View, PieSeriesView)

            pieView.ExplodedDistancePercentage = 5
            ' RuntimeExploding se configura automáticamente si está soportado

            Try
                pieView.RuntimeExploding = True

            Catch
                ' Si no está soportado, continuar sin runtime exploding

            End Try
            ' Configurar etiquetas
            Dim pieLabel = DirectCast(series.Label, PieSeriesLabel)

            pieLabel.TextPattern = "{A}: {V:F0} ({VP:P0})"
            pieLabel.Position = PieSeriesLabelPosition.TwoColumns
            ' Las etiquetas están visibles por defecto cuando se asigna un TextPattern

            chartTicketsByStatus.Series.Add(series)

        Catch ex As Exception
            Global.JelaWeb.Logger.LogError("Error al cargar gráfica de estado", ex, Global.JelaWeb.SessionHelper.GetNombre())

        End Try
    End Sub

    ''' <summary>
    ''' Carga la gráfica de tickets por mes
    ''' </summary>
    Private Sub LoadMonthlyChart()

        Try
            Dim userId = SessionHelper.GetUserId()

            If Not userId.HasValue Then Return

            Dim rolId As Integer? = GetUserRoleId()
            Dim entidadId As Integer? = Nothing
            Dim chartData = dashboardBusiness.GetTicketsByMonthChartData(userId.Value, rolId, entidadId)

            ' Si no hay datos, no mostrar gráfica
            If chartData Is Nothing OrElse chartData.Count = 0 Then
                chartTicketsByMonth.Visible = False
                Return
            End If

            chartTicketsByMonth.Series.Clear()

            Dim series As New Series(Global.JelaWeb.LocalizationHelper.GetString("ChartTicketsByMonth"), ViewType.Bar)

            For Each point In chartData
                series.Points.Add(New SeriesPoint(point.Label, point.Value))

            Next

            ' Configurar vista de barras
            Dim barView = DirectCast(series.View, SideBySideBarSeriesView)

            barView.FillStyle.FillMode = FillMode.Gradient
            barView.ColorEach = True

            chartTicketsByMonth.Series.Add(series)

        Catch ex As Exception
            Logger.LogError("Error al cargar gráfica mensual", ex, SessionHelper.GetNombre())
            chartTicketsByMonth.Visible = False

        End Try
    End Sub

    ''' <summary>
    ''' Carga los textos localizados
    ''' </summary>
    Private Sub LoadLocalizedTexts()

        Try
            litDashboardTitle.Text = LocalizationHelper.GetString("DashboardTitle")
            litWelcomeMessage.Text = LocalizationHelper.GetString("DashboardWelcome", SessionHelper.GetNombre())
            litGestionTickets.Text = LocalizationHelper.GetString("Tickets_Title")
            litTicketsAbiertos.Text = LocalizationHelper.GetString("MetricTicketsAbiertos")
            litTicketsCerrados.Text = LocalizationHelper.GetString("MetricTicketsCerrados")
            litTicketsEnProceso.Text = LocalizationHelper.GetString("MetricTicketsEnProceso")
            litTicketsPendientes.Text = LocalizationHelper.GetString("MetricTicketsPendientes")
            litTicketsUrgentes.Text = LocalizationHelper.GetString("MetricTicketsUrgentes")
            litTotalTickets.Text = LocalizationHelper.GetString("MetricTotalTickets")
            litChartTicketsByStatus.Text = LocalizationHelper.GetString("ChartTicketsByStatus")
            litChartTicketsByMonth.Text = LocalizationHelper.GetString("ChartTicketsByMonth")

            ' Configurar mensaje según rol
            Dim rolId = GetUserRoleId()

            If rolId.HasValue Then
                Select Case rolId.Value

                    Case 1 ' Admin
                        litRoleInfo.Text = LocalizationHelper.GetString("RoleAdmin")
                        litRoleDescription.Text = ": " & "Visualizando métricas globales del sistema."

                    Case 2 ' Técnico
                        litRoleInfo.Text = LocalizationHelper.GetString("RoleTecnico")
                        litRoleDescription.Text = ": " & "Visualizando tus tickets asignados."

                    Case 3 ' Residente
                        litRoleInfo.Text = LocalizationHelper.GetString("RoleResidente")
                        litRoleDescription.Text = ": " & "Visualizando tus tickets y solicitudes."

                    Case Else
                        litRoleInfo.Text = ""
                        litRoleDescription.Text = ""

                End Select
            End If

        Catch ex As Exception
            Logger.LogError("Error al cargar textos localizados", ex, SessionHelper.GetNombre())

        End Try
    End Sub

    ''' <summary>
    ''' Obtiene el ID del rol del usuario desde la sesión o opciones
    ''' Por defecto retorna 1 (Admin) si no está disponible
    ''' </summary>
    Private Function GetUserRoleId() As Integer?

        Try
            ' Intentar obtener desde la sesión
            Dim session = HttpContext.Current.Session

            If session IsNot Nothing AndAlso session("RolId") IsNot Nothing Then
                Return Convert.ToInt32(session("RolId"))
            End If

            ' Intentar obtener desde las opciones del menú
            Dim opciones = SessionHelper.GetOpciones()

            If opciones IsNot Nothing AndAlso opciones.Count > 0 Then
                ' Buscar rol en las opciones (ajustar según tu estructura de datos)

                For Each opcion In opciones
                    If opcion("rolId") IsNot Nothing Then
                        Return Convert.ToInt32(opcion("rolId"))
                    End If

                Next

            End If

            ' Por defecto, retornar 1 (Admin)
            ' En producción, esto debería venir del API o sesión
            Return 1

        Catch ex As Exception
            Logger.LogError("Error al obtener rol del usuario", ex, SessionHelper.GetNombre())
            Return 1 ' Por defecto Admin

        End Try
    End Function

    ''' <summary>
    ''' Maneja el click en el enlace de gestión de tickets
    ''' </summary>
    Protected Sub lnkGestionTickets_Click(sender As Object, e As EventArgs)
        Response.Redirect(Constants.ROUTE_TICKETS, False)
    End Sub

    ''' <summary>
    ''' Maneja el click en la card de tickets abiertos
    ''' </summary>
    Protected Sub lnkTicketsAbiertos_Click(sender As Object, e As EventArgs)
        Response.Redirect(Constants.ROUTE_TICKETS, False)
    End Sub

End Class
