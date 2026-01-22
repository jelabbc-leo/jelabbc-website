Imports System.Collections.Generic
Imports System.Data
Imports System.Text
Imports System.Web

Namespace Business.Operacion

    ''' <summary>
    ''' Clase de negocio para obtener métricas y datos del dashboard
    ''' </summary>
    Public Class DashboardBusiness
        Private ReadOnly api As ApiConsumer
        Private ReadOnly baseUrl As String

        Public Sub New(apiBaseUrl As String)
            api = New Global.JelaWeb.ApiConsumer()
            baseUrl = apiBaseUrl
        End Sub

        ''' <summary>
        ''' Obtiene métricas del dashboard según el rol del usuario
        ''' </summary>
        Public Function GetDashboardMetrics(userId As Integer, rolId As Integer?, entidadId As Integer?) As Global.JelaWeb.DashboardMetricsDTO

            Try
                Dim query As String = BuildMetricsQuery(rolId, userId, entidadId)

                If String.IsNullOrEmpty(query) Then
                    Return GetDefaultMetrics()
                End If

                Dim url As String = baseUrl & System.Web.HttpUtility.UrlEncode(query)
                Dim datos = api.ObtenerDatos(url)

                If datos Is Nothing OrElse datos.Count = 0 Then
                    Return GetDefaultMetrics()
                End If

                Dim metrics As New Global.JelaWeb.DashboardMetricsDTO()
                Dim primeraFila = datos(0)

                metrics.TicketsAbiertos = GetIntegerValue(primeraFila("TicketsAbiertos"))
                metrics.TicketsCerrados = GetIntegerValue(primeraFila("TicketsCerrados"))
                metrics.TicketsEnProceso = GetIntegerValue(primeraFila("TicketsEnProceso"))
                metrics.TicketsPendientes = GetIntegerValue(primeraFila("TicketsPendientes"))
                metrics.TicketsUrgentes = GetIntegerValue(primeraFila("TicketsUrgentes"))
                metrics.TotalTickets = GetIntegerValue(primeraFila("TotalTickets"))

                Return metrics

            Catch ex As Exception
                Global.JelaWeb.Logger.LogError("Error al obtener métricas del dashboard", ex, "")
                Return GetDefaultMetrics()

            End Try
        End Function

        ''' <summary>
        ''' Obtiene datos para la gráfica de tickets por estado
        ''' </summary>
        Public Function GetTicketsByStatusChartData(userId As Integer, rolId As Integer?, entidadId As Integer?) As List(Of Global.JelaWeb.ChartDataPointDTO)

            Try
                Dim query As String = BuildChartQuery(rolId, userId, entidadId)

                If String.IsNullOrEmpty(query) Then
                    Return GetDefaultChartData()
                End If

                Dim url As String = baseUrl & System.Web.HttpUtility.UrlEncode(query)
                Dim datos = api.ObtenerDatos(url)

                If datos Is Nothing OrElse datos.Count = 0 Then
                    Return GetDefaultChartData()
                End If

                Dim chartData As New List(Of ChartDataPointDTO)()

                For Each fila In datos
                    Dim point As New ChartDataPointDTO()

                    point.Label = If(fila("Estado") IsNot Nothing, fila("Estado").ToString(), "Desconocido")
                    point.Value = GetIntegerValue(fila("Cantidad"))
                    chartData.Add(point)

                Next

                Return chartData

            Catch ex As Exception
                Global.JelaWeb.Logger.LogError("Error al obtener datos de gráfica por estado", ex, "")
                Return GetDefaultChartData()

            End Try
        End Function

        ''' <summary>
        ''' Obtiene datos para la gráfica de tickets por mes
        ''' </summary>
        Public Function GetTicketsByMonthChartData(userId As Integer, rolId As Integer?, entidadId As Integer?) As List(Of Global.JelaWeb.ChartDataPointDTO)

            Try
                Dim query As String = BuildMonthlyChartQuery(rolId, userId, entidadId)

                If String.IsNullOrEmpty(query) Then
                    Return GetDefaultMonthlyChartData()
                End If

                Dim url As String = baseUrl & System.Web.HttpUtility.UrlEncode(query)
                Dim datos = api.ObtenerDatos(url)

                If datos Is Nothing OrElse datos.Count = 0 Then
                    Return GetDefaultMonthlyChartData()
                End If

                Dim chartData As New List(Of ChartDataPointDTO)()

                For Each fila In datos
                    Dim point As New Global.JelaWeb.ChartDataPointDTO()

                    point.Label = If(fila("Mes") IsNot Nothing, fila("Mes").ToString(), "Desconocido")
                    point.Value = GetIntegerValue(fila("Cantidad"))
                    chartData.Add(point)

                Next

                Return chartData

            Catch ex As Exception
                Global.JelaWeb.Logger.LogError("Error al obtener datos de gráfica mensual", ex, "")
                Return GetDefaultMonthlyChartData()

            End Try
        End Function

        ''' <summary>
        ''' Construye la query SQL para obtener métricas según el rol
        ''' </summary>
        Private Function BuildMetricsQuery(rolId As Integer?, userId As Integer, entidadId As Integer?) As String
            ' IDs de roles comunes (ajustar según tu sistema)
            ' 1 = Admin, 2 = Técnico, 3 = Residente
            Dim query As New StringBuilder()

            If Not rolId.HasValue Then
                Return String.Empty
            End If

            query.Append("SELECT ")
            query.Append("SUM(CASE WHEN Estado = 'Abierto' THEN 1 ELSE 0 END) AS TicketsAbiertos, ")
            query.Append("SUM(CASE WHEN Estado = 'Cerrado' OR Estado = 'Resuelto' THEN 1 ELSE 0 END) AS TicketsCerrados, ")
            query.Append("SUM(CASE WHEN Estado = 'En Proceso' THEN 1 ELSE 0 END) AS TicketsEnProceso, ")
            query.Append("SUM(CASE WHEN Estado = 'Pendiente' THEN 1 ELSE 0 END) AS TicketsPendientes, ")
            query.Append("SUM(CASE WHEN UrgenciaAsignada = 'Alta' OR PrioridadAsignada = 'Alta' THEN 1 ELSE 0 END) AS TicketsUrgentes, ")
            query.Append("COUNT(*) AS TotalTickets ")
            query.Append("FROM op_tickets_v2 WHERE 1=1 ")

            Select Case rolId.Value

                Case 1 ' Admin - ver todos los tickets de la entidad
                    If entidadId.HasValue Then
                        query.Append("AND IdEntidad = " & Global.JelaWeb.QueryBuilder.EscapeSqlInteger(entidadId.Value) & " ")
                    End If

                Case 2 ' Técnico - ver solo tickets asignados
                    query.Append("AND IdAgenteAsignado = " & Global.JelaWeb.QueryBuilder.EscapeSqlInteger(userId) & " ")

                Case 3 ' Residente - ver solo sus tickets
                    query.Append("AND IdUsuarioCreacion = " & Global.JelaWeb.QueryBuilder.EscapeSqlInteger(userId) & " ")

                Case Else
                    Return String.Empty

            End Select

            Return query.ToString()
        End Function

        ''' <summary>
        ''' Construye la query SQL para obtener datos de gráfica por estado
        ''' </summary>
        Private Function BuildChartQuery(rolId As Integer?, userId As Integer, entidadId As Integer?) As String
            Dim query As New StringBuilder()

            If Not rolId.HasValue Then
                Return String.Empty
            End If

            query.Append("SELECT Estado, COUNT(*) AS Cantidad ")
            query.Append("FROM op_tickets_v2 WHERE 1=1 ")

            Select Case rolId.Value

                Case 1 ' Admin
                    If entidadId.HasValue Then
                        query.Append("AND IdEntidad = " & Global.JelaWeb.QueryBuilder.EscapeSqlInteger(entidadId.Value) & " ")
                    End If

                Case 2 ' Técnico
                    query.Append("AND IdAgenteAsignado = " & Global.JelaWeb.QueryBuilder.EscapeSqlInteger(userId) & " ")

                Case 3 ' Residente
                    query.Append("AND IdUsuarioCreacion = " & Global.JelaWeb.QueryBuilder.EscapeSqlInteger(userId) & " ")

                Case Else
                    Return String.Empty

            End Select

            query.Append("GROUP BY Estado")

            Return query.ToString()
        End Function

        ''' <summary>
        ''' Construye la query SQL para obtener datos de gráfica mensual
        ''' </summary>
        Private Function BuildMonthlyChartQuery(rolId As Integer?, userId As Integer, entidadId As Integer?) As String
            Dim query As New StringBuilder()

            If Not rolId.HasValue Then
                Return String.Empty
            End If

            query.Append("SELECT DATE_FORMAT(FechaCreacion, '%Y-%m') AS Mes, COUNT(*) AS Cantidad ")
            query.Append("FROM op_tickets_v2 WHERE 1=1 ")

            Select Case rolId.Value

                Case 1 ' Admin
                    If entidadId.HasValue Then
                        query.Append("AND IdEntidad = " & Global.JelaWeb.QueryBuilder.EscapeSqlInteger(entidadId.Value) & " ")
                    End If

                Case 2 ' Técnico
                    query.Append("AND IdAgenteAsignado = " & Global.JelaWeb.QueryBuilder.EscapeSqlInteger(userId) & " ")

                Case 3 ' Residente
                    query.Append("AND IdUsuarioCreacion = " & Global.JelaWeb.QueryBuilder.EscapeSqlInteger(userId) & " ")

                Case Else
                    Return String.Empty

            End Select

            query.Append("AND FechaCreacion >= DATE_SUB(NOW(), INTERVAL 12 MONTH) ")
            query.Append("GROUP BY DATE_FORMAT(FechaCreacion, '%Y-%m') ")
            query.Append("ORDER BY Mes ASC")

            Return query.ToString()
        End Function

        ''' <summary>
        ''' Convierte un valor a Integer de forma segura
        ''' </summary>
        Private Function GetIntegerValue(value As Object) As Integer
            If value Is Nothing OrElse IsDBNull(value) Then
                Return 0
            End If

            Try
                Return Convert.ToInt32(value)

            Catch
                Return 0

            End Try
        End Function

        ''' <summary>
        ''' Retorna métricas por defecto cuando hay error
        ''' </summary>
        Private Function GetDefaultMetrics() As Global.JelaWeb.DashboardMetricsDTO
            Return New Global.JelaWeb.DashboardMetricsDTO With {
                .TicketsAbiertos = 0,
                .TicketsCerrados = 0,
                .TicketsEnProceso = 0,
                .TicketsPendientes = 0,
                .TicketsUrgentes = 0,
                .TotalTickets = 0
            }
        End Function

        ''' <summary>
        ''' Retorna datos de gráfica por defecto cuando hay error
        ''' </summary>
        Private Function GetDefaultChartData() As List(Of Global.JelaWeb.ChartDataPointDTO)
            Return New List(Of Global.JelaWeb.ChartDataPointDTO)()
        End Function

        ''' <summary>
        ''' Retorna datos de gráfica mensual por defecto cuando hay error
        ''' </summary>
        Private Function GetDefaultMonthlyChartData() As List(Of Global.JelaWeb.ChartDataPointDTO)
            Return New List(Of Global.JelaWeb.ChartDataPointDTO)()
        End Function

    End Class

End Namespace
