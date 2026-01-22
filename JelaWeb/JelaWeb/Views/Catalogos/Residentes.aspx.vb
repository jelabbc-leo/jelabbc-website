Imports DevExpress.Web
Imports System.Data
Imports System.Linq
Imports JelaWeb.Utilities
Imports JelaWeb.Services
Imports JelaWeb.Infrastructure.Helpers

Partial Public Class Residentes
    Inherits BasePage

#Region "Page Events"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load

        Try
            ' Suscribir eventos del grid
            AddHandler gridResidentes.DataBound, AddressOf gridResidentes_DataBound

            If Not IsPostBack Then
                CargarCombos()
                CargarResidentes()
            End If

        Catch ex As Exception
            Logger.LogError("Residentes.Page_Load", ex)
            ResidentesHelper.MostrarMensaje(Me, "Error al cargar la página", "error")

        End Try
    End Sub

#End Region

#Region "Carga de Datos"

    Private Sub CargarResidentes()

        Try
            ' Obtener datos del servicio
            Dim dt As DataTable = ResidentesService.ListarResidentes()

            ' Generar columnas dinámicamente desde el DataTable
            GenerarColumnasDinamicas(gridResidentes, dt)

            ' Guardar DataTable en Session ANTES de DataBind para FuncionesGridWeb
            Session("dtResidentes") = dt

            gridResidentes.DataSource = dt
            gridResidentes.DataBind()

        Catch ex As Exception
            Logger.LogError("Residentes.CargarResidentes", ex)
            ResidentesHelper.MostrarMensaje(Me, "Error al cargar residentes", "error")

        End Try
    End Sub

    Private Sub CargarCombos()

        Try
            ' Cargar Entidades - ELIMINADO: El sistema usa IdEntidadActual automáticamente
            ' El filtrado por entidad se maneja automáticamente en DynamicCrudService

            ' Cargar SubEntidades (se actualizará dinámicamente desde JavaScript)
            cmbSubEntidad.Items.Clear()
            cmbSubEntidad.Items.Insert(0, New ListEditItem("-- Ninguna --", DBNull.Value))

            ' Cargar Unidades (se actualizará dinámicamente desde JavaScript)
            cmbUnidad.Items.Clear()

        Catch ex As Exception
            Logger.LogError("Residentes.CargarCombos", ex)
            ResidentesHelper.MostrarMensaje(Me, "Error al cargar combos", "error")

        End Try
    End Sub

#End Region

#Region "Generación de Columnas Dinámicas"

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
                        gridCol.Width = Unit.Pixel(100)
                        gridCol.PropertiesEdit.DisplayFormatString = "c2"

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
            Logger.LogError("Residentes.GenerarColumnasDinamicas", ex)
            Throw

        End Try
    End Sub

#End Region

#Region "Eventos del Grid"

    Protected Sub gridResidentes_DataBound(sender As Object, e As EventArgs)

        Try
            ' Leer DataTable desde Session (guardado antes de DataBind)
            Dim tabla As DataTable = TryCast(Session("dtResidentes"), DataTable)

            If tabla IsNot Nothing Then
                FuncionesGridWeb.SUMColumn(gridResidentes, tabla)
            End If

        Catch ex As Exception
            Logger.LogError("Residentes.gridResidentes_DataBound", ex)

        End Try
    End Sub

#End Region

#Region "Guardar Residente"

    Protected Sub btnGuardar_Click(sender As Object, e As EventArgs)

        Try
            Dim residenteId As Integer = Integer.Parse(hfResidenteId.Value)
            Dim datos As New Dictionary(Of String, Object)

            ' Datos personales
            ' EntidadId se agrega automáticamente por DynamicCrudService
            If cmbSubEntidad.Value IsNot Nothing AndAlso Not IsDBNull(cmbSubEntidad.Value) Then
                datos("SubEntidadId") = CInt(cmbSubEntidad.Value)
            Else
                datos("SubEntidadId") = DBNull.Value
            End If
            datos("UnidadId") = CInt(cmbUnidad.Value)
            datos("TipoResidente") = cmbTipoResidente.Value?.ToString()
            datos("Nombre") = txtNombre.Text.Trim()
            datos("ApellidoPaterno") = txtApellidoPaterno.Text.Trim()
            datos("ApellidoMaterno") = txtApellidoMaterno.Text.Trim()
            datos("EsPrincipal") = If(chkEsPrincipal.Checked, 1, 0)
            datos("Activo") = If(chkActivo.Checked, 1, 0)

            If dteFechaIngreso.Date <> DateTime.MinValue Then
                datos("FechaIngreso") = dteFechaIngreso.Date
            End If

            ' Contacto
            datos("Email") = txtEmail.Text.Trim()
            datos("Telefono") = txtTelefono.Text.Trim()
            datos("TelefonoCelular") = txtCelular.Text.Trim()
            datos("TelefonoEmergencia") = txtTelefonoEmergencia.Text.Trim()

            ' Telegram
            datos("TelegramChatId") = txtTelegramChatId.Text.Trim()
            datos("TelegramUsername") = txtTelegramUsername.Text.Trim()
            datos("TelegramActivo") = If(chkTelegramActivo.Checked, 1, 0)

            ' Notificaciones
            datos("RecibirNotificacionesEmail") = If(chkNotifEmail.Checked, 1, 0)
            datos("RecibirNotificacionesTelegram") = If(chkNotifTelegram.Checked, 1, 0)
            datos("RecibirNotificacionesPush") = If(chkNotifPush.Checked, 1, 0)

            ' Identificación
            datos("TipoIdentificacion") = cmbTipoIdentificacion.Value?.ToString()
            datos("NumeroIdentificacion") = txtNumeroIdentificacion.Text.Trim()
            datos("RFC") = txtRFC.Text.Trim()
            datos("CURP") = txtCURP.Text.Trim()
            datos("ClaveElector") = txtClaveElector.Text.Trim()
            datos("VigenciaINE") = txtVigenciaINE.Text.Trim()

            ' Vehículo
            datos("TieneVehiculo") = If(chkTieneVehiculo.Checked, 1, 0)
            datos("PlacasVehiculo") = txtPlacas.Text.Trim()

            Dim currentUser = SessionHelper.GetCurrentUser()
            Dim userId As Integer? = If(currentUser IsNot Nothing, CInt(currentUser.Id), CType(Nothing, Integer?))

            ' Guardar a través del servicio
            Dim resultado As Boolean = ResidentesService.GuardarResidente(residenteId, datos, userId)

            If residenteId = 0 Then
                Logger.LogInfo($"Residente creado: {datos("Nombre")} {datos("ApellidoPaterno")}")
            Else
                Logger.LogInfo($"Residente actualizado: Id={residenteId}")
            End If

            If resultado Then
                ResidentesHelper.MostrarMensaje(Me, "Residente guardado correctamente", "success")
                CargarResidentes()
                ScriptManager.RegisterStartupScript(Me, Me.GetType(), "hidePopup", "popupResidente.Hide();", True)
            Else
                ResidentesHelper.MostrarMensaje(Me, "Error al guardar el residente", "error")
            End If

        Catch ex As Exception
            Logger.LogError("Residentes.btnGuardar_Click", ex)
            ResidentesHelper.MostrarMensaje(Me, "Error al guardar el residente: " & ex.Message, "error")

        End Try
    End Sub

#End Region

#Region "WebMethods"

    <System.Web.Services.WebMethod()>
    Public Shared Function ObtenerResidente(id As Integer) As Object
        Return ResidentesService.ObtenerResidente(id)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function EliminarResidente(id As Integer) As Object
        Return ResidentesService.EliminarResidente(id)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function ObtenerSubEntidadesPorEntidad(entidadId As Integer) As Object
        Return ResidentesService.ObtenerSubEntidadesPorEntidad(entidadId)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function ObtenerUnidadesPorSubEntidad(subEntidadId As Integer) As Object
        Return ResidentesService.ObtenerUnidadesPorSubEntidad(subEntidadId)
    End Function

#End Region

#Region "WebMethods - Vehículos"

    <System.Web.Services.WebMethod()>
    Public Shared Function ObtenerVehiculo(id As Integer) As Object
        Return ResidentesService.ObtenerVehiculo(id)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function GuardarVehiculo(datos As Dictionary(Of String, Object)) As Object
        Return ResidentesService.GuardarVehiculo(datos)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function EliminarVehiculo(id As Integer) As Object
        Return ResidentesService.EliminarVehiculo(id)
    End Function

#End Region

#Region "WebMethods - Tags de Acceso"

    <System.Web.Services.WebMethod()>
    Public Shared Function ObtenerTag(id As Integer) As Object
        Return ResidentesService.ObtenerTag(id)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function GuardarTag(datos As Dictionary(Of String, Object)) As Object
        Return ResidentesService.GuardarTag(datos)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function EliminarTag(id As Integer) As Object
        Return ResidentesService.EliminarTag(id)
    End Function

#End Region

#Region "Helpers"

    ' Los Helpers se han movido a ResidentesHelper

#End Region

End Class
