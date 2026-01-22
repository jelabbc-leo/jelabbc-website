Imports System.Linq
Imports System.Data
Imports DevExpress.Web
Imports System.Configuration
Imports JelaWeb.Utilities
Imports JelaWeb.Services
Imports JelaWeb.Infrastructure.Helpers

Partial Public Class Comunicados
    Inherits BasePage

#Region "Page Events"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load

        Try
            If Not IsPostBack Then
                ' Registrar eventos manualmente
                AddHandler gridComunicados.DataBound, AddressOf gridComunicados_DataBound
                AddHandler gridComunicados.CustomCallback, AddressOf gridComunicados_CustomCallback

                CargarCombos()
                CargarComunicados()
            End If

        Catch ex As Exception
            Logger.LogError("Comunicados.Page_Load", ex)
            ComunicadosHelper.MostrarMensaje(Me, "Error al cargar la página", "error")

        End Try
    End Sub

#End Region

#Region "Carga de Datos"

    Private Sub CargarComunicados()

        Try
            ' Obtener datos del servicio
            Dim dt As DataTable = ComunicadosService.ListarComunicados()

            GenerarColumnasDinamicas(gridComunicados, dt)
            Session("dtComunicados") = dt

            gridComunicados.DataSource = dt
            gridComunicados.DataBind()

        Catch ex As Exception
            Logger.LogError("Comunicados.CargarComunicados", ex)
            ComunicadosHelper.MostrarMensaje(Me, "Error al cargar comunicados", "error")

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

                    Case GetType(Integer), GetType(Long), GetType(Short)
                        gridCol = New GridViewDataTextColumn()
                        gridCol.Width = Unit.Pixel(100)

                    Case Else
                        gridCol = New GridViewDataTextColumn()
                        gridCol.Width = Unit.Pixel(200)
                        If nombreColumna.Equals("Contenido", StringComparison.OrdinalIgnoreCase) Then
                            gridCol.Width = Unit.Pixel(300)
                        End If

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
            Logger.LogError("Comunicados.GenerarColumnasDinamicas", ex)
            Throw

        End Try
    End Sub

    Private Sub CargarCombos()

        Try
            ' Cargar Entidades - ELIMINADO: El sistema usa IdEntidadActual automáticamente
            ' El filtrado por entidad se maneja automáticamente en DynamicCrudService

        Catch ex As Exception
            Logger.LogError("Comunicados.CargarCombos", ex)

        End Try
    End Sub

#End Region

#Region "Eventos del Grid"

    Protected Sub gridComunicados_DataBound(sender As Object, e As EventArgs)

        Try
            Dim tabla As DataTable = TryCast(Session("dtComunicados"), DataTable)

            If tabla IsNot Nothing Then
                FuncionesGridWeb.SUMColumn(gridComunicados, tabla)
            End If

        Catch ex As Exception
            Logger.LogError("Comunicados.gridComunicados_DataBound", ex)

        End Try
    End Sub

    Protected Sub gridComunicados_CustomCallback(sender As Object, e As ASPxGridViewCustomCallbackEventArgs)

        Try
            CargarComunicados()

        Catch ex As Exception
            Logger.LogError("Comunicados.gridComunicados_CustomCallback", ex)

        End Try
    End Sub

#End Region

#Region "WebMethods"

    <System.Web.Services.WebMethod()>
    Public Shared Function ObtenerComunicado(id As Integer) As Object
        Return ComunicadosService.ObtenerComunicado(id)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function GuardarComunicado(datos As Dictionary(Of String, Object)) As Object
        Return ComunicadosService.GuardarComunicado(datos)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function EliminarComunicado(id As Integer) As Object
        Return ComunicadosService.EliminarComunicado(id)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function PublicarComunicado(id As Integer) As Object
        Return ComunicadosService.PublicarComunicado(id)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function ListarSubEntidades(entidadId As Integer) As Object
        Return ComunicadosService.ListarSubEntidades(entidadId)
    End Function

#End Region

#Region "Helpers"

    ' Los Helpers se han movido a ComunicadosHelper

#End Region

End Class
