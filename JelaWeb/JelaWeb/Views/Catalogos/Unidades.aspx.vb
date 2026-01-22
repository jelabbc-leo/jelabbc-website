Imports System.Linq
Imports System.Data
Imports DevExpress.Web
Imports System.Configuration
Imports JelaWeb.Utilities
Imports JelaWeb.Services
Imports JelaWeb.Infrastructure.Helpers

Partial Public Class Unidades
    Inherits BasePage
    Implements System.Web.UI.IPostBackEventHandler

    Private servicio As ApiService

    ''' <summary>
    ''' Propiedad para obtener o inicializar el servicio de API
    ''' </summary>
    Private ReadOnly Property GetServicio() As ApiService
        Get
            If servicio Is Nothing Then
                Dim apiUrl As String = ConfigurationManager.AppSettings("ApiBaseUrl")

                servicio = New ApiService(apiUrl)
            End If
            Return servicio
        End Get
    End Property

#Region "Page Events"

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Init

        Try
            Dim dt As DataTable = TryCast(Session("dtUnidades"), DataTable)

            If dt Is Nothing OrElse dt.Columns.Count = 0 Then
                dt = GetServicio().ListarUnidades()
            End If

            If dt Is Nothing OrElse dt.Columns.Count = 0 Then Return

            ' Normalizar nombre de columna Id para que coincida con KeyFieldName
            For Each col As DataColumn In dt.Columns
                If col.ColumnName.Equals("id", StringComparison.OrdinalIgnoreCase) AndAlso col.ColumnName <> "Id" Then
                    col.ColumnName = "Id"
                    Exit For
                End If
            Next

            ' Crear columnas en Init para evitar conflictos con ViewState en callbacks
            GenerarColumnasGridPrincipal(dt)

        Catch ex As Exception
            Logger.LogError("Unidades.Page_Init", ex)
        End Try
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load

        Try
            ' Cargar combos solo en el primer load
            If Not IsPostBack Then
                Try
                    CargarCombos()
                Catch ex As Exception
                    Logger.LogError("Unidades.Page_Load - Error en CargarCombos", ex)
                End Try
            End If

            ' SIEMPRE cargar datos del grid (como en Entidades.aspx - sin verificar IsPostBack)
            ' Esto asegura que el grid tenga datos incluso después de postbacks
            CargarDatos()

        Catch ex As Exception
            Logger.LogError("Unidades.Page_Load", ex)
            If Not IsPostBack Then
                UnidadesHelper.MostrarMensaje(Me, "Error al cargar la página", "error")
            End If

        End Try
    End Sub

#End Region

#Region "Carga de Datos"

    Private Sub CargarDatos()

        Try
            Dim dt As DataTable = GetServicio().ListarUnidades()

            If dt Is Nothing OrElse dt.Rows.Count = 0 Then
                Logger.LogInfo("Unidades.CargarDatos: No hay datos para mostrar")
                Return
            End If

            ' Normalizar nombre de columna Id para que coincida con KeyFieldName
            For Each col As DataColumn In dt.Columns
                If col.ColumnName.Equals("id", StringComparison.OrdinalIgnoreCase) AndAlso col.ColumnName <> "Id" Then
                    col.ColumnName = "Id"
                    Exit For
                End If
            Next

            ' Guardar en sesión para reusar en callbacks
            Session("dtUnidades") = dt
            
            ' Asignar DataSource y hacer DataBind
            gridUnidades.DataSource = dt
            gridUnidades.DataBind()
            
            Logger.LogInfo($"Unidades.CargarDatos: {dt.Rows.Count} unidades cargadas")

        Catch ex As Exception
            Logger.LogError("Unidades.CargarDatos", ex)
            Throw

        End Try
    End Sub

    ''' <summary>
    ''' Genera columnas dinámicamente para el grid principal de Unidades.
    ''' Las columnas de Estatus y Acciones se generan en gridUnidades_DataBound.
    ''' </summary>
    Private Sub GenerarColumnasGridPrincipal(tabla As DataTable)
        Try
            If tabla Is Nothing OrElse tabla.Columns.Count = 0 Then Return

            Dim columnasOcultar As String() = {"Id", "Latitud", "Longitud", "EstatusFinanciero", "SaldoPendiente"}
            
            ' Limpiar todas las columnas existentes
            gridUnidades.Columns.Clear()

            ' Crear columnas dinámicamente desde el DataTable
            For Each dtCol As DataColumn In tabla.Columns
                Dim fieldName = dtCol.ColumnName

                ' Omitir columnas que se manejan especialmente
                If fieldName.Equals("EstatusFinanciero", StringComparison.OrdinalIgnoreCase) OrElse
                   fieldName.Equals("SaldoPendiente", StringComparison.OrdinalIgnoreCase) Then
                    Continue For
                End If

                Dim gridCol As New GridViewDataTextColumn()
                gridCol.FieldName = fieldName
                gridCol.Caption = FuncionesGridWeb.SplitCamelCase(fieldName)
                gridCol.ReadOnly = True
                gridCol.Settings.AllowHeaderFilter = True
                gridCol.Settings.AllowSort = True
                gridCol.Settings.AllowGroup = True

                If columnasOcultar.Contains(fieldName, StringComparer.OrdinalIgnoreCase) Then
                    gridCol.Visible = False
                End If

                If fieldName.IndexOf("Saldo", StringComparison.OrdinalIgnoreCase) >= 0 Then
                    gridCol.PropertiesEdit.DisplayFormatString = "$ #,##0.00"
                End If

                gridUnidades.Columns.Add(gridCol)
            Next
            
            ' Las columnas de Estatus y Acciones se agregarán en gridUnidades_DataBound

        Catch ex As Exception
            Logger.LogError("Unidades.GenerarColumnasGridPrincipal", ex)
        End Try
    End Sub

    ''' <summary>
    ''' Genera columnas dinámicamente para un grid basándose en las columnas del DataTable.
    ''' Usado por los grids secundarios (Residentes, Vehículos, Tags, Documentos, etc.)
    ''' </summary>
    Private Sub GenerarColumnasDinamicas(grid As ASPxGridView, tabla As DataTable)
        Try
            If tabla Is Nothing OrElse tabla.Columns.Count = 0 Then Return

            ' Limpiar columnas existentes generadas dinámicamente (mantener las del ASPX)
            Dim columnasARemover As New List(Of GridViewColumn)
            For Each col As GridViewColumn In grid.Columns
                If TypeOf col Is GridViewDataColumn Then
                    Dim dataCol = CType(col, GridViewDataColumn)
                    ' Remover solo columnas que no tienen DataItemTemplate (las generadas dinámicamente)
                    If dataCol.DataItemTemplate Is Nothing AndAlso Not TypeOf col Is GridViewCommandColumn Then
                        columnasARemover.Add(col)
                    End If
                End If
            Next

            For Each col In columnasARemover
                grid.Columns.Remove(col)
            Next

            ' Crear columnas dinámicamente desde el DataTable
            For Each col As DataColumn In tabla.Columns
                Dim nombreColumna = col.ColumnName

                ' Verificar si la columna ya existe
                Dim columnaExiste As Boolean = False
                For Each colExistente As GridViewColumn In grid.Columns
                    If TypeOf colExistente Is GridViewDataColumn Then
                        Dim dataCol = CType(colExistente, GridViewDataColumn)
                        If dataCol.FieldName = nombreColumna Then
                            columnaExiste = True
                            Exit For
                        End If
                    End If
                Next

                If Not columnaExiste Then
                    Dim gridCol As New GridViewDataTextColumn()
                    gridCol.FieldName = nombreColumna
                    gridCol.Caption = nombreColumna
                    gridCol.ReadOnly = True
                    gridCol.Settings.AllowHeaderFilter = True

                    ' Ocultar columna Id
                    If nombreColumna.Equals("Id", StringComparison.OrdinalIgnoreCase) Then
                        gridCol.Visible = False
                    End If

                    grid.Columns.Add(gridCol)
                End If
            Next

        Catch ex As Exception
            Logger.LogError("Unidades.GenerarColumnasDinamicas", ex)
        End Try
    End Sub

    Private Sub CargarCombos()

        Try
            ' Cargar Entidades - ELIMINADO: El sistema usa IdEntidadActual automáticamente
            ' El filtrado por entidad se maneja automáticamente en DynamicCrudService

            ' SubEntidades se cargan dinámicamente cuando se selecciona una entidad
            ' Solo inicializamos con el item "-- Ninguna --"
            cboSubEntidad.Items.Clear()
            cboSubEntidad.Items.Insert(0, New ListEditItem("-- Ninguna --", DBNull.Value))

        Catch ex As Exception
            Logger.LogError("Unidades.CargarCombos", ex)
            ' No mostrar mensaje aquí para no bloquear, el JavaScript intentará cargar los combos

        End Try
    End Sub

#End Region

#Region "Eventos del Grid"

    Protected Sub gridCallback_Callback(sender As Object, e As DevExpress.Web.CallbackEventArgsBase)
        ' Callback del CallbackPanel (como en Entidades.aspx)
        CargarDatos()
    End Sub

    Protected Sub gridUnidades_DataBound(sender As Object, e As EventArgs) Handles gridUnidades.DataBound

        Try
            Dim tabla As DataTable = TryCast(Session("dtUnidades"), DataTable)

            If tabla Is Nothing Then
                tabla = TryCast(gridUnidades.DataSource, DataTable)
            End If

            If tabla IsNot Nothing Then
                ' Generar columnas de Estatus y Acciones dinámicamente
                ' Esto se hace en DataBound para que el template se regenere correctamente
                GenerarColumnasEstatusYAcciones(tabla)
                
                For Each col As GridViewColumn In gridUnidades.Columns
                    Dim dataCol = TryCast(col, GridViewDataColumn)
                    If dataCol Is Nothing Then Continue For

                    If dataCol.FieldName.IndexOf("Saldo", StringComparison.OrdinalIgnoreCase) >= 0 Then
                        dataCol.PropertiesEdit.DisplayFormatString = "$ #,##0.00"
                    End If
                Next

                FuncionesGridWeb.SUMColumn(gridUnidades, tabla)
            End If

        Catch ex As Exception
            Logger.LogError("Unidades.gridUnidades_DataBound", ex)

        End Try
    End Sub
    
    ''' <summary>
    ''' Genera dinámicamente las columnas de Estatus Financiero y Acciones.
    ''' SIEMPRE las regenera para mantener el template y CSS después de callbacks.
    ''' </summary>
    Private Sub GenerarColumnasEstatusYAcciones(tabla As DataTable)
        Try
            ' SIEMPRE remover las columnas especiales para regenerarlas
            ' Esto asegura que el template se mantenga después de callbacks
            For i As Integer = gridUnidades.Columns.Count - 1 To 0 Step -1
                Dim col = gridUnidades.Columns(i)
                Dim debeRemover As Boolean = False
                
                If TypeOf col Is GridViewDataColumn Then
                    Dim dataCol = CType(col, GridViewDataColumn)
                    If dataCol.FieldName = "EstatusFinanciero" Then
                        debeRemover = True
                    End If
                ElseIf TypeOf col Is GridViewCommandColumn Then
                    Dim cmdCol = CType(col, GridViewCommandColumn)
                    If cmdCol.Caption = "Acciones" Then
                        debeRemover = True
                    End If
                End If
                
                If debeRemover Then
                    gridUnidades.Columns.RemoveAt(i)
                End If
            Next
            
            ' Crear columna de Estatus Financiero con template dinámico
            If tabla.Columns.Contains("EstatusFinanciero") Then
                Dim colEstatus As New GridViewDataTextColumn()
                colEstatus.FieldName = "EstatusFinanciero"
                colEstatus.Caption = "Estatus"
                colEstatus.Width = Unit.Pixel(120)
                colEstatus.Settings.AllowHeaderFilter = True
                colEstatus.Settings.AllowGroup = True
                
                ' Crear template dinámico para el estatus
                colEstatus.DataItemTemplate = New EstatusFinancieroTemplate()
                
                ' Agregar AL FINAL (antes de la última posición)
                gridUnidades.Columns.Add(colEstatus)
            End If
            
            ' Crear columna de Acciones con botones personalizados
            Dim colAcciones As New GridViewCommandColumn()
            colAcciones.Caption = "Acciones"
            colAcciones.Width = Unit.Pixel(150)
            colAcciones.ShowSelectCheckbox = False
            
            ' Agregar botones personalizados SIN texto, solo iconos
            ' Usar espacio en blanco como texto para evitar que muestre el ID
            Dim btnResidentes As New GridViewCommandColumnCustomButton()
            btnResidentes.ID = "btnResidentes"
            btnResidentes.Text = " "
            btnResidentes.Image.Url = "~/Content/Images/Iconos/colaboradores.png"
            colAcciones.CustomButtons.Add(btnResidentes)
            
            Dim btnVehiculos As New GridViewCommandColumnCustomButton()
            btnVehiculos.ID = "btnVehiculos"
            btnVehiculos.Text = " "
            btnVehiculos.Image.Url = "~/Content/Images/Iconos/casa.png"
            colAcciones.CustomButtons.Add(btnVehiculos)
            
            Dim btnTags As New GridViewCommandColumnCustomButton()
            btnTags.ID = "btnTags"
            btnTags.Text = " "
            btnTags.Image.Url = "~/Content/Images/Iconos/servicios.png"
            colAcciones.CustomButtons.Add(btnTags)
            
            Dim btnDocumentos As New GridViewCommandColumnCustomButton()
            btnDocumentos.ID = "btnDocumentos"
            btnDocumentos.Text = " "
            btnDocumentos.Image.Url = "~/Content/Images/Iconos/documentos.png"
            colAcciones.CustomButtons.Add(btnDocumentos)
            
            ' Agregar AL FINAL
            gridUnidades.Columns.Add(colAcciones)
            
        Catch ex As Exception
            Logger.LogError("Unidades.GenerarColumnasEstatusYAcciones", ex)
        End Try
    End Sub
    
    ''' <summary>
    ''' Template personalizado para la columna de Estatus Financiero.
    ''' Genera el HTML con las clases CSS correctas para los badges de estatus.
    ''' </summary>
    Private Class EstatusFinancieroTemplate
        Implements ITemplate
        
        Public Sub InstantiateIn(container As Control) Implements ITemplate.InstantiateIn
            Dim literal As New Literal()
            AddHandler literal.DataBinding, AddressOf OnDataBinding
            container.Controls.Add(literal)
        End Sub
        
        Private Sub OnDataBinding(sender As Object, e As EventArgs)
            Dim literal As Literal = CType(sender, Literal)
            Dim container As GridViewDataItemTemplateContainer = CType(literal.NamingContainer, GridViewDataItemTemplateContainer)
            
            ' Obtener valores
            Dim estatusFinanciero As Object = DataBinder.Eval(container.DataItem, "EstatusFinanciero")
            Dim saldoPendiente As Object = DataBinder.Eval(container.DataItem, "SaldoPendiente")
            
            ' Obtener clase CSS para el estatus
            Dim statusClass As String = UnidadesHelper.GetStatusClass(estatusFinanciero)
            
            ' Generar HTML con badge
            Dim html As New System.Text.StringBuilder()
            html.Append("<span class=""status-badge status-")
            html.Append(statusClass)
            html.Append(""">")
            html.Append(If(estatusFinanciero IsNot Nothing, estatusFinanciero.ToString(), ""))
            html.Append("</span>")
            
            ' Agregar saldo si existe
            If saldoPendiente IsNot Nothing AndAlso Not IsDBNull(saldoPendiente) Then
                Dim saldoDecimal As Decimal = Convert.ToDecimal(saldoPendiente)
                If saldoDecimal > 0 Then
                    html.Append("<br/><small class='text-muted'>$")
                    html.Append(saldoDecimal.ToString("N2"))
                    html.Append("</small>")
                End If
            End If
            
            literal.Text = html.ToString()
        End Sub
    End Class

    Protected Sub gridResidentes_CustomCallback(sender As Object, e As ASPxGridViewCustomCallbackEventArgs)

        Try
            Dim unidadIdActual As String = If(hfUnidadIdActual IsNot Nothing, hfUnidadIdActual.Value, "")
            Dim dt As DataTable = UnidadesService.ProcesarCustomCallbackResidentes(e.Parameters, Session, unidadIdActual)

            If dt IsNot Nothing Then
                ' Generar columnas dinámicamente
                GenerarColumnasDinamicas(gridResidentes, dt)

                Session("dtResidentes") = dt
                gridResidentes.DataSource = dt
                gridResidentes.DataBind()
            End If

        Catch ex As Exception
            Logger.LogError("Unidades.gridResidentes_CustomCallback", ex)

        End Try
    End Sub

    Protected Sub gridResidentes_DataBound(sender As Object, e As EventArgs)

        Try
            Dim tabla As DataTable = TryCast(Session("dtResidentes"), DataTable)

            If tabla IsNot Nothing Then
                FuncionesGridWeb.SUMColumn(gridResidentes, tabla)
            End If

        Catch ex As Exception
            Logger.LogError("Unidades.gridResidentes_DataBound", ex)

        End Try
    End Sub

    Protected Sub gridVehiculos_CustomCallback(sender As Object, e As ASPxGridViewCustomCallbackEventArgs)

        Try
            Dim unidadIdActual As String = If(hfUnidadIdActual IsNot Nothing, hfUnidadIdActual.Value, "")
            Dim dt As DataTable = UnidadesService.ProcesarCustomCallbackVehiculos(e.Parameters, Session, unidadIdActual)

            If dt IsNot Nothing Then
                ' Generar columnas dinámicamente
                GenerarColumnasDinamicas(gridVehiculos, dt)

                Session("dtVehiculos") = dt
                gridVehiculos.DataSource = dt
                gridVehiculos.DataBind()
            End If

        Catch ex As Exception
            Logger.LogError("Unidades.gridVehiculos_CustomCallback", ex)

        End Try
    End Sub

    Protected Sub gridVehiculos_DataBound(sender As Object, e As EventArgs)

        Try
            Dim tabla As DataTable = TryCast(Session("dtVehiculos"), DataTable)

            If tabla IsNot Nothing Then
                FuncionesGridWeb.SUMColumn(gridVehiculos, tabla)
            End If

        Catch ex As Exception
            Logger.LogError("Unidades.gridVehiculos_DataBound", ex)

        End Try
    End Sub

    Protected Sub gridTags_CustomCallback(sender As Object, e As ASPxGridViewCustomCallbackEventArgs)

        Try
            Dim unidadIdActual As String = If(hfUnidadIdActual IsNot Nothing, hfUnidadIdActual.Value, "")
            Dim dt As DataTable = UnidadesService.ProcesarCustomCallbackTags(e.Parameters, Session, unidadIdActual)

            If dt IsNot Nothing Then
                ' Generar columnas dinámicamente
                GenerarColumnasDinamicas(gridTags, dt)

                Session("dtTags") = dt
                gridTags.DataSource = dt
                gridTags.DataBind()
            End If

        Catch ex As Exception
            Logger.LogError("Unidades.gridTags_CustomCallback", ex)

        End Try
    End Sub

    Protected Sub gridTags_DataBound(sender As Object, e As EventArgs)

        Try
            Dim tabla As DataTable = TryCast(Session("dtTags"), DataTable)

            If tabla IsNot Nothing Then
                FuncionesGridWeb.SUMColumn(gridTags, tabla)
            End If

        Catch ex As Exception
            Logger.LogError("Unidades.gridTags_DataBound", ex)

        End Try
    End Sub

    Protected Sub gridDocumentos_CustomCallback(sender As Object, e As ASPxGridViewCustomCallbackEventArgs)

        Try
            Dim unidadIdActual As String = If(hfUnidadIdActual IsNot Nothing, hfUnidadIdActual.Value, "")
            Dim dt As DataTable = UnidadesService.ProcesarCustomCallbackDocumentos(e.Parameters, Session, unidadIdActual)

            If dt IsNot Nothing Then
                ' Generar columnas dinámicamente
                GenerarColumnasDinamicas(gridDocumentos, dt)

                Session("dtDocumentos") = dt
                gridDocumentos.DataSource = dt
                gridDocumentos.DataBind()
            End If

        Catch ex As Exception
            Logger.LogError("Unidades.gridDocumentos_CustomCallback", ex)

        End Try
    End Sub

    Protected Sub gridDocumentos_DataBound(sender As Object, e As EventArgs)

        Try
            Dim tabla As DataTable = TryCast(Session("dtDocumentos"), DataTable)

            If tabla IsNot Nothing Then
                FuncionesGridWeb.SUMColumn(gridDocumentos, tabla)
            End If

        Catch ex As Exception
            Logger.LogError("Unidades.gridDocumentos_DataBound", ex)

        End Try
    End Sub

#End Region

#Region "Guardar"

    Protected Sub btnGuardar_Click(sender As Object, e As EventArgs)

        Try
            Dim id As Integer = Integer.Parse(hfId.Value)
            Dim datos As New Dictionary(Of String, Object)

            datos("Codigo") = txtCodigo.Text.Trim()
            datos("Nombre") = txtNombre.Text.Trim()
            ' IdEntidad se agrega automáticamente por DynamicCrudService

            If cboSubEntidad.Value IsNot Nothing AndAlso Not IsDBNull(cboSubEntidad.Value) Then
                datos("SubEntidadId") = CInt(cboSubEntidad.Value)
            Else
                datos("SubEntidadId") = DBNull.Value
            End If

            datos("Torre") = txtTorre.Text.Trim()
            datos("Edificio") = txtEdificio.Text.Trim()
            datos("Piso") = txtPiso.Text.Trim()
            datos("Numero") = txtNumero.Text.Trim()
            datos("Calle") = txtCalle.Text.Trim()
            datos("NumeroExterior") = txtNumeroExterior.Text.Trim()
            datos("NumeroInterior") = txtNumeroInterior.Text.Trim()
            datos("Referencia") = txtReferencia.Text.Trim()
            datos("Latitud") = If(txtLatitud.Value IsNot Nothing, txtLatitud.Number, DBNull.Value)
            datos("Longitud") = If(txtLongitud.Value IsNot Nothing, txtLongitud.Number, DBNull.Value)
            datos("Superficie") = txtSuperficie.Number
            datos("Descripcion") = txtDescripcion.Text.Trim()
            datos("Activo") = If(chkActivo.Checked, 1, 0)

            Dim currentUser = SessionHelper.GetCurrentUser()
            Dim userId As Integer? = If(currentUser IsNot Nothing, CInt(currentUser.Id), CType(Nothing, Integer?))

            ' Guardar a través del servicio
            Dim resultado As Boolean = UnidadesService.GuardarUnidadDesdeCodeBehind(id, datos, userId)

            If id = 0 Then
                Logger.LogInfo($"Unidad creada: {datos("Codigo")} - {datos("Nombre")}")
            Else
                Logger.LogInfo($"Unidad actualizada: Id={id}")
            End If

            If resultado Then
                UnidadesHelper.MostrarMensaje(Me, "Guardado correctamente", "success")
                ' Recargar datos (como en Entidades.aspx)
                CargarDatos()
                ' Refrescar el CallbackPanel mediante JavaScript (usar ClientScript como en otras páginas)
                ClientScript.RegisterStartupScript(Me.GetType(), "refreshGrid", "if (typeof gridCallback !== 'undefined' && gridCallback) { gridCallback.PerformCallback(); }", True)
                UnidadesHelper.OcultarPopupUnidad(Me)
            Else
                UnidadesHelper.MostrarMensaje(Me, "Error al guardar", "error")
            End If

        Catch ex As Exception
            Logger.LogError("Unidades.btnGuardar_Click", ex)
            UnidadesHelper.MostrarMensaje(Me, "Error al guardar: " & ex.Message, "error")

        End Try
    End Sub

#End Region

#Region "PostBack Handler"

    Protected Shadows Sub RaisePostBackEvent(eventArgument As String) Implements System.Web.UI.IPostBackEventHandler.RaisePostBackEvent

        Try
            If eventArgument.StartsWith("EDIT:") Then
                CargarParaEditar(Integer.Parse(eventArgument.Replace("EDIT:", "")))
            ElseIf eventArgument.StartsWith("DELETE:") Then
                Eliminar(Integer.Parse(eventArgument.Replace("DELETE:", "")))
            End If

        Catch ex As Exception
            Logger.LogError("Unidades.RaisePostBackEvent", ex)

        End Try
    End Sub

    Private Sub CargarParaEditar(id As Integer)

        Try
            ' Obtener datos del servicio
            Dim registro As DataRow = UnidadesService.ObtenerUnidadDataRow(id)

            If registro IsNot Nothing Then
                hfId.Value = id.ToString()
                txtCodigo.Text = registro("Codigo")?.ToString()
                txtNombre.Text = registro("Nombre")?.ToString()
                ' cboEntidad eliminado - El sistema usa IdEntidadActual automáticamente
                cboSubEntidad.Value = If(IsDBNull(registro("SubEntidadId")), Nothing, registro("SubEntidadId"))
                txtTorre.Text = registro("Torre")?.ToString()
                txtEdificio.Text = registro("Edificio")?.ToString()
                txtPiso.Text = registro("Piso")?.ToString()
                txtNumero.Text = registro("Numero")?.ToString()
                txtCalle.Text = registro("Calle")?.ToString()
                txtNumeroExterior.Text = registro("NumeroExterior")?.ToString()
                txtNumeroInterior.Text = registro("NumeroInterior")?.ToString()
                txtReferencia.Text = registro("Referencia")?.ToString()
                txtLatitud.Number = If(IsDBNull(registro("Latitud")), Nothing, CDec(registro("Latitud")))
                txtLongitud.Number = If(IsDBNull(registro("Longitud")), Nothing, CDec(registro("Longitud")))
                txtSuperficie.Number = If(IsDBNull(registro("Superficie")), 0, CDec(registro("Superficie")))
                txtDescripcion.Text = registro("Descripcion")?.ToString()
                chkActivo.Checked = If(IsDBNull(registro("Activo")), True, CBool(registro("Activo")))

                ScriptManager.RegisterStartupScript(Me, Me.GetType(), "showPopup", $"popupUnidad.SetHeaderText('Editar: {registro("Codigo")}'); popupUnidad.Show();", True)
            End If

        Catch ex As Exception
            Logger.LogError($"Unidades.CargarParaEditar: {id}", ex)

        End Try
    End Sub

    Private Sub Eliminar(id As Integer)

        Try
            ' Eliminar a través del servicio
            If UnidadesService.EliminarUnidadDesdeCodeBehind(id) Then
                Logger.LogInfo($"Unidad eliminada: Id={id}")
                UnidadesHelper.MostrarMensaje(Me, "Eliminado correctamente", "success")
                ' Recargar datos (como en Entidades.aspx)
                CargarDatos()
                ' Refrescar el CallbackPanel mediante JavaScript (usar ClientScript como en otras páginas)
                ClientScript.RegisterStartupScript(Me.GetType(), "refreshGrid", "if (typeof gridCallback !== 'undefined' && gridCallback) { gridCallback.PerformCallback(); }", True)
            Else
                UnidadesHelper.MostrarMensaje(Me, "Error al eliminar", "error")
            End If

        Catch ex As Exception
            Logger.LogError($"Unidades.Eliminar: {id}", ex)
            UnidadesHelper.MostrarMensaje(Me, "Error al eliminar", "error")

        End Try
    End Sub

#End Region

#Region "WebMethods"

    <System.Web.Services.WebMethod()>
    Public Shared Function ObtenerUnidad(id As Integer) As Object
        Return UnidadesService.ObtenerUnidad(id)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function EliminarUnidad(id As Integer) As Object
        Return UnidadesService.EliminarUnidad(id)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function ObtenerEntidades() As Object
        Return UnidadesService.ObtenerEntidades()
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function ObtenerSubEntidadesPorEntidad(entidadId As Integer) As Object
        Return UnidadesService.ObtenerSubEntidadesPorEntidad(entidadId)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function GuardarUnidad(datos As Dictionary(Of String, Object)) As Object
        Return UnidadesService.GuardarUnidad(datos)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function ObtenerResidentesUnidad(unidadId As Integer) As Object
        Return UnidadesService.ObtenerResidentesUnidad(unidadId)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function ObtenerResidente(id As Integer) As Object
        Return UnidadesService.ObtenerResidente(id)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function GuardarResidente(datos As Dictionary(Of String, Object)) As Object
        Return UnidadesService.GuardarResidente(datos)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function EliminarResidente(id As Integer) As Object
        Return UnidadesService.EliminarResidente(id)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function ObtenerVehiculosUnidad(unidadId As Integer) As Object
        Return UnidadesService.ObtenerVehiculosUnidad(unidadId)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function ObtenerVehiculo(id As Integer) As Object
        Return UnidadesService.ObtenerVehiculo(id)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function GuardarVehiculo(datos As Dictionary(Of String, Object)) As Object
        Return UnidadesService.GuardarVehiculo(datos)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function EliminarVehiculo(id As Integer) As Object
        Return UnidadesService.EliminarVehiculo(id)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function ObtenerTagsUnidad(unidadId As Integer) As Object
        Return UnidadesService.ObtenerTagsUnidad(unidadId)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function ObtenerTag(id As Integer) As Object
        Return UnidadesService.ObtenerTag(id)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function GuardarTag(datos As Dictionary(Of String, Object)) As Object
        Return UnidadesService.GuardarTag(datos)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function EliminarTag(id As Integer) As Object
        Return UnidadesService.EliminarTag(id)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function ObtenerDocumentosUnidad(unidadId As Integer) As Object
        Return UnidadesService.ObtenerDocumentosUnidad(unidadId)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function ObtenerDocumento(id As Integer) As Object
        Return UnidadesService.ObtenerDocumento(id)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function GuardarDocumento(datos As Dictionary(Of String, Object)) As Object
        Return UnidadesService.GuardarDocumento(datos)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function EliminarDocumento(id As Integer) As Object
        Return UnidadesService.EliminarDocumento(id)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function ObtenerArchivosResidente(residenteId As Integer) As Object
        Return UnidadesService.ObtenerArchivosResidente(residenteId)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function GuardarArchivoResidente(datos As Dictionary(Of String, Object)) As Object
        Return UnidadesService.GuardarArchivoResidente(datos)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function EliminarArchivoResidente(id As Integer) As Object
        Return UnidadesService.EliminarArchivoResidente(id)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function ObtenerArchivosVehiculo(vehiculoId As Integer) As Object
        Return UnidadesService.ObtenerArchivosVehiculo(vehiculoId)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function GuardarArchivoVehiculo(datos As Dictionary(Of String, Object)) As Object
        Return UnidadesService.GuardarArchivoVehiculo(datos)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function EliminarArchivoVehiculo(id As Integer) As Object
        Return UnidadesService.EliminarArchivoVehiculo(id)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function ObtenerArchivosDocumento(documentoId As Integer) As Object
        Return UnidadesService.ObtenerArchivosDocumento(documentoId)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function GuardarArchivoDocumento(datos As Dictionary(Of String, Object)) As Object
        Return UnidadesService.GuardarArchivoDocumento(datos)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function EliminarArchivoDocumento(id As Integer) As Object
        Return UnidadesService.EliminarArchivoDocumento(id)
    End Function

#End Region

#Region "Helpers"

    ''' <summary>
    ''' Obtiene la clase CSS para el badge de estatus financiero
    ''' </summary>
    Public Function GetStatusClass(estatus As Object) As String
        Return UnidadesHelper.GetStatusClass(estatus)
    End Function

    ''' <summary>
    ''' Muestra el saldo pendiente si existe
    ''' </summary>
    Public Function GetSaldoDisplay(saldo As Object) As String

        Try
            If saldo Is Nothing OrElse IsDBNull(saldo) Then Return ""
            Dim saldoDecimal As Decimal = Convert.ToDecimal(saldo)

            If saldoDecimal > 0 Then
                Return "<br/><small class='text-muted'>$" & saldoDecimal.ToString("N2") & "</small>"
            End If
            Return ""

        Catch
            Return ""

        End Try
    End Function

    ' Los Helpers se han movido a UnidadesHelper

    Protected Sub tabsUnidad_ActiveTabChanged(source As Object, e As TabControlEventArgs) Handles tabsUnidad.ActiveTabChanged

    End Sub

    Protected Sub gridArchivosResidente_CustomCallback(sender As Object, e As ASPxGridViewCustomCallbackEventArgs)

        Try
            Dim dt As DataTable = UnidadesService.ProcesarCustomCallbackArchivosResidente(e.Parameters, Session)

            If dt IsNot Nothing Then
                ' Generar columnas dinámicamente
                GenerarColumnasDinamicas(gridArchivosResidente, dt)

                Session("dtArchivosResidente") = dt
                gridArchivosResidente.DataSource = dt
                gridArchivosResidente.DataBind()
            End If

        Catch ex As Exception
            Logger.LogError("Unidades.gridArchivosResidente_CustomCallback", ex)

        End Try
    End Sub

    Protected Sub gridArchivosResidente_DataBound(sender As Object, e As EventArgs)

        Try
            Dim tabla As DataTable = TryCast(Session("dtArchivosResidente"), DataTable)

            If tabla IsNot Nothing Then
                FuncionesGridWeb.SUMColumn(gridArchivosResidente, tabla)
            End If

        Catch ex As Exception
            Logger.LogError("Unidades.gridArchivosResidente_DataBound", ex)

        End Try
    End Sub

    Protected Sub gridArchivosVehiculo_CustomCallback(sender As Object, e As ASPxGridViewCustomCallbackEventArgs)

        Try
            Dim dt As DataTable = UnidadesService.ProcesarCustomCallbackArchivosVehiculo(e.Parameters, Session)

            If dt IsNot Nothing Then
                ' Generar columnas dinámicamente
                GenerarColumnasDinamicas(gridArchivosVehiculo, dt)

                Session("dtArchivosVehiculo") = dt
                gridArchivosVehiculo.DataSource = dt
                gridArchivosVehiculo.DataBind()
            End If

        Catch ex As Exception
            Logger.LogError("Unidades.gridArchivosVehiculo_CustomCallback", ex)

        End Try
    End Sub

    Protected Sub gridArchivosVehiculo_DataBound(sender As Object, e As EventArgs)

        Try
            Dim tabla As DataTable = TryCast(Session("dtArchivosVehiculo"), DataTable)

            If tabla IsNot Nothing Then
                FuncionesGridWeb.SUMColumn(gridArchivosVehiculo, tabla)
            End If

        Catch ex As Exception
            Logger.LogError("Unidades.gridArchivosVehiculo_DataBound", ex)

        End Try
    End Sub

    Protected Sub gridArchivosDocumento_CustomCallback(sender As Object, e As ASPxGridViewCustomCallbackEventArgs)

        Try
            Dim dt As DataTable = UnidadesService.ProcesarCustomCallbackArchivosDocumento(e.Parameters, Session)

            If dt IsNot Nothing Then
                ' Generar columnas dinámicamente
                GenerarColumnasDinamicas(gridArchivosDocumento, dt)

                Session("dtArchivosDocumento") = dt
                gridArchivosDocumento.DataSource = dt
                gridArchivosDocumento.DataBind()
            End If

        Catch ex As Exception
            Logger.LogError("Unidades.gridArchivosDocumento_CustomCallback", ex)

        End Try
    End Sub

    Protected Sub gridArchivosDocumento_DataBound(sender As Object, e As EventArgs)

        Try
            Dim tabla As DataTable = TryCast(Session("dtArchivosDocumento"), DataTable)

            If tabla IsNot Nothing Then
                FuncionesGridWeb.SUMColumn(gridArchivosDocumento, tabla)
            End If

        Catch ex As Exception
            Logger.LogError("Unidades.gridArchivosDocumento_DataBound", ex)

        End Try
    End Sub

#End Region

End Class
