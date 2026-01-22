Imports System.Linq
Imports System.Data
Imports DevExpress.Web
Imports System.Configuration
Imports JelaWeb.Utilities
Imports JelaWeb.Services
Imports JelaWeb.Infrastructure.Helpers

Partial Public Class CalendarioReservaciones
    Inherits BasePage

#Region "Page Events"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load

        Try
            If Not IsPostBack Then
                ' Registrar eventos manualmente
                AddHandler gridCalendario.DataBound, AddressOf gridCalendario_DataBound
                AddHandler gridCalendario.CustomCallback, AddressOf gridCalendario_CustomCallback

                CargarCombos()
                CargarCalendario()
            End If

        Catch ex As Exception
            Logger.LogError("CalendarioReservaciones.Page_Load", ex)
            CalendarioReservacionesHelper.MostrarMensaje(Me, "Error al cargar la página", "error")

        End Try
    End Sub

#End Region

#Region "Carga de Datos"

    Private Sub CargarCalendario()

        Try
            Dim mes As Integer = If(cboMes.Value IsNot Nothing, CInt(cboMes.Value), DateTime.Now.Month)
            Dim anio As Integer = If(cboAnio.Value IsNot Nothing, CInt(cboAnio.Value), DateTime.Now.Year)
            Dim areaComunId As Integer = If(cboAreaComun.Value IsNot Nothing, CInt(cboAreaComun.Value), 0)

            ' Obtener datos del servicio
            Dim dt As DataTable = ReservacionesService.ListarReservacionesCalendario(mes, anio, areaComunId)

            GenerarColumnasDinamicas(gridCalendario, dt)
            Session("dtCalendario") = dt

            gridCalendario.DataSource = dt
            gridCalendario.DataBind()

        Catch ex As Exception
            Logger.LogError("CalendarioReservaciones.CargarCalendario", ex)
            CalendarioReservacionesHelper.MostrarMensaje(Me, "Error al cargar calendario", "error")

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
                        CType(gridCol, GridViewDataDateColumn).PropertiesDateEdit.DisplayFormatString = "dd/MM/yyyy"

                    Case GetType(Decimal), GetType(Double), GetType(Single)
                        gridCol = New GridViewDataTextColumn()
                        gridCol.Width = Unit.Pixel(120)
                        gridCol.PropertiesEdit.DisplayFormatString = "c2"
                        gridCol.CellStyle.HorizontalAlign = HorizontalAlign.Right

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
            Logger.LogError("CalendarioReservaciones.GenerarColumnasDinamicas", ex)
            Throw

        End Try
    End Sub

    Private Sub CargarCombos()

        Try
            ' Meses

            For i As Integer = 1 To 12
                Dim mes As String = New DateTime(2024, i, 1).ToString("MMMM", New Globalization.CultureInfo("es-MX"))
                cboMes.Items.Add(mes, i)
            Next

            cboMes.Value = DateTime.Now.Month

            ' Años (actual y siguiente)

            For i As Integer = DateTime.Now.Year - 1 To DateTime.Now.Year + 1
                cboAnio.Items.Add(i.ToString(), i)
            Next

            cboAnio.Value = DateTime.Now.Year

            ' Áreas Comunes desde el servicio
            Dim dtAreas As DataTable = ReservacionesService.ListarAreasComunes()

            cboAreaComun.Items.Clear()
            cboAreaComun.Items.Add("-- Todas --", 0)

            For Each row As DataRow In dtAreas.Rows
                cboAreaComun.Items.Add(row("Nombre").ToString(), row("Id"))
            Next

        Catch ex As Exception
            Logger.LogError("CalendarioReservaciones.CargarCombos", ex)
            CalendarioReservacionesHelper.MostrarMensaje(Me, "Error al cargar combos", "error")

        End Try
    End Sub

#End Region

#Region "Eventos del Grid"

    Protected Sub gridCalendario_DataBound(sender As Object, e As EventArgs)

        Try
            Dim tabla As DataTable = TryCast(Session("dtCalendario"), DataTable)

            If tabla IsNot Nothing Then
                FuncionesGridWeb.SUMColumn(gridCalendario, tabla)
            End If

        Catch ex As Exception
            Logger.LogError("CalendarioReservaciones.gridCalendario_DataBound", ex)

        End Try
    End Sub

    Protected Sub gridCalendario_CustomCallback(sender As Object, e As ASPxGridViewCustomCallbackEventArgs)

        Try
            CargarCalendario()

        Catch ex As Exception
            Logger.LogError("CalendarioReservaciones.gridCalendario_CustomCallback", ex)

        End Try
    End Sub

#End Region

#Region "Helpers"

    ' Los Helpers se han movido a CalendarioReservacionesHelper

#End Region

End Class
