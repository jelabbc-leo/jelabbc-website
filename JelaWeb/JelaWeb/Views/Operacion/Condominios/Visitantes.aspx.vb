Imports System.Linq
Imports System.Data
Imports DevExpress.Web
Imports System.Configuration
Imports JelaWeb.Utilities
Imports JelaWeb.Services
Imports JelaWeb.Infrastructure.Helpers

Partial Public Class Visitantes
    Inherits BasePage

#Region "Page Events"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load

        Try
            If Not IsPostBack Then
                ' Registrar eventos manualmente
                AddHandler gridVisitantes.DataBound, AddressOf gridVisitantes_DataBound
                AddHandler gridVisitantes.CustomCallback, AddressOf gridVisitantes_CustomCallback

                dtFechaDesde.Date = New DateTime(DateTime.Now.Year, DateTime.Now.Month, 1)
                dtFechaHasta.Date = DateTime.Now.Date

                CargarCombos()
                CargarVisitantes()
            End If

        Catch ex As Exception
            Logger.LogError("Visitantes.Page_Load", ex)
            VisitantesHelper.MostrarMensaje(Me, "Error al cargar la página", "error")

        End Try
    End Sub

#End Region

#Region "Carga de Datos"

    Private Sub CargarVisitantes()

        Try
            Dim fechaDesdeNullable As DateTime? = dtFechaDesde.Date
            Dim fechaHastaNullable As DateTime? = dtFechaHasta.Date
            Dim fechaDesde As DateTime = If(fechaDesdeNullable.HasValue, fechaDesdeNullable.Value, New DateTime(DateTime.Now.Year, DateTime.Now.Month, 1))
            Dim fechaHasta As DateTime = If(fechaHastaNullable.HasValue, fechaHastaNullable.Value, DateTime.Now.Date)

            ' Obtener datos del servicio
            Dim dt As DataTable = VisitantesService.ListarVisitantes(fechaDesde, fechaHasta)

            GenerarColumnasDinamicas(gridVisitantes, dt)
            Session("dtVisitantes") = dt

            gridVisitantes.DataSource = dt
            gridVisitantes.DataBind()

        Catch ex As Exception
            Logger.LogError("Visitantes.CargarVisitantes", ex)
            VisitantesHelper.MostrarMensaje(Me, "Error al cargar visitantes", "error")

        End Try
    End Sub

    Private Sub GenerarColumnasDinamicas(grid As ASPxGridView, tabla As DataTable)

        Try
            If tabla Is Nothing OrElse tabla.Columns.Count = 0 Then Return

            For i As Integer = grid.Columns.Count - 1 To 0 Step -1
                If Not TypeOf grid.Columns(i) Is GridViewCommandColumn Then
                    grid.Columns.RemoveAt(i)
                End If
            Next

            For Each col As DataColumn In tabla.Columns
                Dim nombreColumna = col.ColumnName

                If nombreColumna.Equals("Id", StringComparison.OrdinalIgnoreCase) Then Continue For

                Dim gridCol As GridViewDataColumn = Nothing

                Select Case col.DataType

                    Case GetType(Boolean)
                        gridCol = New GridViewDataCheckColumn()
                        gridCol.Width = Unit.Pixel(80)

                    Case GetType(DateTime), GetType(Date)
                        gridCol = New GridViewDataDateColumn()
                        gridCol.Width = Unit.Pixel(150)
                        CType(gridCol, GridViewDataDateColumn).PropertiesDateEdit.DisplayFormatString = "dd/MM/yyyy HH:mm"

                    Case GetType(Decimal), GetType(Double), GetType(Single)
                        gridCol = New GridViewDataTextColumn()
                        gridCol.Width = Unit.Pixel(120)
                        gridCol.PropertiesEdit.DisplayFormatString = "c2"

                    Case GetType(Integer), GetType(Long), GetType(Short)
                        gridCol = New GridViewDataTextColumn()
                        gridCol.Width = Unit.Pixel(100)

                    Case Else
                        gridCol = New GridViewDataTextColumn()
                        gridCol.Width = Unit.Pixel(150)

                End Select

                gridCol.FieldName = nombreColumna
                gridCol.Caption = nombreColumna
                gridCol.ReadOnly = True
                gridCol.Visible = True
                gridCol.Settings.AllowHeaderFilter = True
                gridCol.Settings.AllowGroup = True

                grid.Columns.Add(gridCol)
            Next

        Catch ex As Exception
            Logger.LogError("Visitantes.GenerarColumnasDinamicas", ex)
            Throw

        End Try
    End Sub

    Private Sub CargarCombos()

        Try
            ' Obtener datos del servicio
            Dim dtEntidades As DataTable = VisitantesService.ListarEntidades()

            cboEntidad.DataSource = dtEntidades
            cboEntidad.TextField = "RazonSocial"
            cboEntidad.ValueField = "Id"
            cboEntidad.DataBind()

        Catch ex As Exception
            Logger.LogError("Visitantes.CargarCombos", ex)

        End Try
    End Sub

#End Region

#Region "Eventos de Filtros"

    Protected Sub btnFiltrar_Click(sender As Object, e As EventArgs)
        CargarVisitantes()
    End Sub

    Protected Sub btnLimpiar_Click(sender As Object, e As EventArgs)
        dtFechaDesde.Date = New DateTime(DateTime.Now.Year, DateTime.Now.Month, 1)
        dtFechaHasta.Date = DateTime.Now.Date
        CargarVisitantes()
    End Sub

#End Region

#Region "Eventos del Grid"

    Protected Sub gridVisitantes_DataBound(sender As Object, e As EventArgs)

        Try
            Dim tabla As DataTable = TryCast(Session("dtVisitantes"), DataTable)

            If tabla IsNot Nothing Then
                FuncionesGridWeb.SUMColumn(gridVisitantes, tabla)
            End If

        Catch ex As Exception
            Logger.LogError("Visitantes.gridVisitantes_DataBound", ex)

        End Try
    End Sub

    Protected Sub gridVisitantes_CustomCallback(sender As Object, e As ASPxGridViewCustomCallbackEventArgs)

        Try
            CargarVisitantes()

        Catch ex As Exception
            Logger.LogError("Visitantes.gridVisitantes_CustomCallback", ex)

        End Try
    End Sub

#End Region

#Region "WebMethods"

    <System.Web.Services.WebMethod()>
    Public Shared Function ObtenerVisitante(id As Integer) As Object
        Return VisitantesService.ObtenerVisitante(id)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function GuardarVisitante(datos As Dictionary(Of String, Object)) As Object
        Return VisitantesService.GuardarVisitante(datos)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function EliminarVisitante(id As Integer) As Object
        Return VisitantesService.EliminarVisitante(id)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function RegistrarSalida(id As Integer) As Object
        Return VisitantesService.RegistrarSalida(id)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function ListarUnidades(entidadId As Integer) As Object
        Return VisitantesService.ListarUnidades(entidadId)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function ListarResidentes(unidadId As Integer) As Object
        Return VisitantesService.ListarResidentes(unidadId)
    End Function

#End Region

#Region "Helpers"

    ' Los Helpers se han movido a VisitantesHelper

#End Region

End Class
