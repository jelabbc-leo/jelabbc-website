Imports DevExpress.Web
Imports System.Data
Imports System.Linq
Imports JelaWeb.Utilities
Imports JelaWeb.Services
Imports JelaWeb.Infrastructure.Helpers

Partial Public Class AreasComunes
    Inherits BasePage

#Region "Page Events"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load

        Try
            ' Suscribir eventos del grid
            AddHandler gridAreas.DataBound, AddressOf gridAreas_DataBound

            If Not IsPostBack Then
                CargarCombos()
                CargarAreas()
            End If

        Catch ex As Exception
            Logger.LogError("AreasComunes.Page_Load", ex)
            AreasComunesHelper.MostrarMensaje(Me, "Error al cargar la página", "error")

        End Try
    End Sub

#End Region

#Region "Carga de Datos"

    Private Sub CargarAreas()

        Try
            ' Obtener datos del servicio
            Dim dt As DataTable = AreasComunesService.ListarAreas()

            ' Generar columnas dinámicamente desde el DataTable
            GenerarColumnasDinamicas(gridAreas, dt)

            ' Guardar DataTable en Session ANTES de DataBind para FuncionesGridWeb
            Session("dtAreas") = dt

            gridAreas.DataSource = dt
            gridAreas.DataBind()

        Catch ex As Exception
            Logger.LogError("AreasComunes.CargarAreas", ex)
            AreasComunesHelper.MostrarMensaje(Me, "Error al cargar áreas comunes", "error")

        End Try
    End Sub

    Private Sub CargarCombos()

        Try
            ' Cargar Entidades - ELIMINADO: El sistema usa IdEntidadActual automáticamente
            ' El filtrado por entidad se maneja automáticamente en DynamicCrudService

            ' SubEntidades se cargarán dinámicamente desde JavaScript
            cmbSubEntidad.Items.Clear()
            cmbSubEntidad.Items.Insert(0, New ListEditItem("(Compartida - Toda la entidad)", DBNull.Value))

        Catch ex As Exception
            Logger.LogError("AreasComunes.CargarCombos", ex)
            AreasComunesHelper.MostrarMensaje(Me, "Error al cargar combos", "error")

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
            Logger.LogError("AreasComunes.GenerarColumnasDinamicas", ex)
            Throw

        End Try
    End Sub

#End Region

#Region "Eventos del Grid"

    Protected Sub gridAreas_DataBound(sender As Object, e As EventArgs)

        Try
            ' Leer DataTable desde Session (guardado antes de DataBind)
            Dim tabla As DataTable = TryCast(Session("dtAreas"), DataTable)

            If tabla IsNot Nothing Then
                FuncionesGridWeb.SUMColumn(gridAreas, tabla)
            End If

        Catch ex As Exception
            Logger.LogError("AreasComunes.gridAreas_DataBound", ex)

        End Try
    End Sub

#End Region

#Region "Guardar Área"

    Protected Sub btnGuardar_Click(sender As Object, e As EventArgs)

        Try
            Dim areaId As Integer = Integer.Parse(hfAreaId.Value)
            Dim datos As New Dictionary(Of String, Object)

            ' Datos generales
            ' EntidadId se agrega automáticamente por DynamicCrudService
            If cmbSubEntidad.Value IsNot Nothing AndAlso Not IsDBNull(cmbSubEntidad.Value) Then
                datos("SubEntidadId") = CInt(cmbSubEntidad.Value)
            Else
                datos("SubEntidadId") = DBNull.Value
            End If
            datos("Clave") = txtClave.Text.Trim().ToUpper()
            datos("Nombre") = txtNombre.Text.Trim()
            datos("TipoArea") = cmbTipoArea.Value?.ToString()
            datos("Descripcion") = txtDescripcion.Text.Trim()
            datos("Ubicacion") = txtUbicacion.Text.Trim()
            datos("Capacidad") = CInt(spnCapacidad.Number)
            datos("Activo") = If(chkActivo.Checked, 1, 0)

            ' Reservación y costos
            datos("RequiereReservacion") = If(chkRequiereReservacion.Checked, 1, 0)
            datos("CostoReservacion") = spnCostoReservacion.Number
            datos("DepositoRequerido") = spnDeposito.Number

            ' Horarios
            If tmeHoraApertura.DateTime <> DateTime.MinValue Then
                datos("HoraApertura") = tmeHoraApertura.DateTime.ToString("HH:mm:ss")
            End If

            If tmeHoraCierre.DateTime <> DateTime.MinValue Then
                datos("HoraCierre") = tmeHoraCierre.DateTime.ToString("HH:mm:ss")
            End If

            ' Duración
            datos("DuracionMinimaHoras") = CInt(spnDuracionMinima.Number)
            datos("DuracionMaximaHoras") = CInt(spnDuracionMaxima.Number)

            ' Anticipación
            datos("AnticipacionMinimaDias") = CInt(spnAnticipacionMinima.Number)
            datos("AnticipacionMaximaDias") = CInt(spnAnticipacionMaxima.Number)

            ' Días disponibles
            datos("DiasDisponibles") = txtDiasDisponibles.Text.Trim()

            Dim currentUser = SessionHelper.GetCurrentUser()
            Dim userId As Integer? = If(currentUser IsNot Nothing, CInt(currentUser.Id), CType(Nothing, Integer?))

            ' Guardar a través del servicio
            Dim resultado As Boolean = AreasComunesService.GuardarArea(areaId, datos, userId)

            If areaId = 0 Then
                Logger.LogInfo($"Área común creada: {datos("Clave")} - {datos("Nombre")}")
            Else
                Logger.LogInfo($"Área común actualizada: Id={areaId}")
            End If

            If resultado Then
                AreasComunesHelper.MostrarMensaje(Me, "Área común guardada correctamente", "success")
                CargarAreas()
                ScriptManager.RegisterStartupScript(Me, Me.GetType(), "hidePopup", "popupArea.Hide();", True)
            Else
                AreasComunesHelper.MostrarMensaje(Me, "Error al guardar el área común", "error")
            End If

        Catch ex As Exception
            Logger.LogError("AreasComunes.btnGuardar_Click", ex)
            AreasComunesHelper.MostrarMensaje(Me, "Error al guardar el área común: " & ex.Message, "error")

        End Try
    End Sub

#End Region

#Region "WebMethods"

    <System.Web.Services.WebMethod()>
    Public Shared Function ObtenerArea(id As Integer) As Object
        Return AreasComunesService.ObtenerArea(id)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function EliminarArea(id As Integer) As Object
        Return AreasComunesService.EliminarArea(id)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function ObtenerSubEntidadesPorEntidad(entidadId As Integer) As Object
        Return AreasComunesService.ObtenerSubEntidadesPorEntidad(entidadId)
    End Function

#End Region

#Region "Helpers"

    ' Los Helpers se han movido a AreasComunesHelper

#End Region

End Class
