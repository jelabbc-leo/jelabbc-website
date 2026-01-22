
Imports DevExpress.Web
Imports DevExpress.XtraRichEdit.Model

Public Class CapturaDocumentos
    Inherits BasePage
    Private servicio As ApiService
    Private apiConsumer As ApiConsumer

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

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim apiUrl As String = ConfigurationManager.AppSettings("ApiBaseUrl")

        servicio = New ApiService(apiUrl)

        If Not IsPostBack Then
            dtDesde.Value = Now
            dtHasta.Value = Now
            glTipoDocumento.Value = 1

            CargarDocumentos()
            CargarTiposDoc()
            CargarSubEntidades()

            gridColonias.DataSource = GetTablaColonias()
            gridColonias.DataBind()

            gridConceptos.DataSource = GetTablaConceptos()
            gridConceptos.DataBind()

            LlenarConceptos()

        Else
            CargarDocumentos()

        End If

        ' Capturar el postback del botón Exportar
        Dim target As String = Request("__EVENTTARGET")

        If target = "ExportarGrid" Then
            ExportarDocumentos()
        End If

    End Sub

    Protected Sub ExportarDocumentos()
        ' Validar que haya datos
        If gridDocumentos.VisibleRowCount = 0 Then
            Throw New ApplicationException("No hay datos para exportar.")
        End If

        ' Validar columnas exportables
        Dim tieneColumnaExportable As Boolean = gridDocumentos.Columns.OfType(Of GridViewDataColumn)().Any(Function(c) c.Visible AndAlso c.GroupIndex < 0)

        If Not tieneColumnaExportable Then
            Throw New ApplicationException("No hay columnas exportables visibles en el grid.")
        End If

        ' Exportar con nombre dinámico
        Dim nombreArchivo As String = "Documentos_" & DateTime.Now.ToString("yyyyMMdd_HHmmss")

        gridExporter.FileName = nombreArchivo
        gridExporter.WriteXlsxToResponse(nombreArchivo, True)

    End Sub

    Protected Sub CargarDocumentos()

        Try
            Dim tblDocumentos As DataTable = GetServicio().ObtenerDocumentos(glTipoDocumento.Value, dtDesde.Value, dtHasta.Value)

            gridDocumentos.DataSource = tblDocumentos
            gridDocumentos.DataBind()

        Catch ex As ApplicationException

            ' Mostrar el mensaje del API en un alert de JavaScript
            Dim script As String = "<script>alert('" & ex.Message.Replace("'", "\'") & "');</script>"

            Response.Write(script)

        Catch ex As Exception
            ' Mostrar cualquier otro error inesperado
            Dim script As String = "<script>alert('Error inesperado: " & ex.Message.Replace("'", "\'") & "');</script>"

            Response.Write(script)

        End Try
    End Sub

    Private Function GetTablaColonias() As DataTable

        If Session("tablaColonias") Is Nothing Then
            Dim dt As New DataTable()

            dt.Columns.Add("Id", GetType(Integer))
            dt.Columns.Add("Clave", GetType(String))
            dt.Columns.Add("Colonia", GetType(String))
            dt.Columns.Add("MontoMin", GetType(Decimal))
            dt.Columns.Add("MontoMax", GetType(Decimal))
            Session("tablaColonias") = dt
        End If

        Return CType(Session("tablaColonias"), DataTable)

    End Function

    Private Function GetTablaConceptos() As DataTable
        Dim tabla As DataTable = TryCast(Session("TablaConceptos"), DataTable)

        If tabla Is Nothing Then
            tabla = New DataTable()
            tabla.Columns.Add("Id", GetType(Integer))
            tabla.Columns.Add("Clave", GetType(String))
            tabla.Columns.Add("Descripcion", GetType(String))
            tabla.Columns.Add("CostoUnitario", GetType(Decimal))
            ' Puedes inicializar con una fila vacía si lo requieres
            ' tabla.Rows.Add("", "")
            Session("TablaConceptos") = tabla
        End If

        Return tabla

    End Function

    Protected Sub CargarTiposDoc()
        Dim tipos_doc As DataTable = GetServicio().ListarTiposDocs()

        ' Normalizar para asegurar que Clave sea String
        Dim tablaLimpia As New DataTable()

        tablaLimpia.Columns.Add("Id", GetType(Integer))
        tablaLimpia.Columns.Add("Clave", GetType(String))
        tablaLimpia.Columns.Add("Nombre", GetType(String))

        For Each row As DataRow In tipos_doc.Rows
            tablaLimpia.Rows.Add(row("Id"), row("Clave").ToString(), row("Nombre").ToString())

        Next

        glTipoDocumento.DataSource = tablaLimpia
        glTipoDocumento.KeyFieldName = "Id"
        glTipoDocumento.DataBind()

    End Sub

    Protected Sub gridDocumentos_CustomCallback(sender As Object, e As DevExpress.Web.ASPxGridViewCustomCallbackEventArgs) Handles gridDocumentos.CustomCallback

        Dim parametros = e.Parameters.Split("|"c)
        Dim operacion As String = parametros(0)

        If operacion = "CARGAR_EDICION" Then
            ' Cargar documento para edición
            Dim idDocumento As Integer = Convert.ToInt32(parametros(1))

            CargarDocumentoParaEdicion(idDocumento)
        Else
            ' Filtrar documentos
            Dim tipoId As String = parametros(0)
            Dim fechaInicio As Date? = Nothing
            Dim fechaFin As Date? = Nothing

            If parametros.Length > 1 AndAlso Not String.IsNullOrEmpty(parametros(1)) Then
                fechaInicio = Date.Parse(parametros(1))
            End If
            If parametros.Length > 2 AndAlso Not String.IsNullOrEmpty(parametros(2)) Then
                fechaFin = Date.Parse(parametros(2))
            End If

            ' Aquí llamas a tu API o DataSource con los filtros
            Dim tblDocumentos As DataTable = GetServicio().ObtenerDocumentos(tipoId, fechaInicio, fechaFin)

            gridDocumentos.DataSource = tblDocumentos
            gridDocumentos.DataBind()
        End If

    End Sub

    Private Sub CargarDocumentoParaEdicion(idDocumento As Integer)

        Try
            ' 1. Obtener datos de la cabecera
            Dim tablaDocumento = GetServicio().ObtenerDocumentoPorId(idDocumento)

            If tablaDocumento Is Nothing OrElse tablaDocumento.Rows.Count = 0 Then
                Throw New ApplicationException("No se encontró el documento con ID: " & idDocumento)
            End If

            Dim filaDocumento = tablaDocumento.Rows(0)

            ' 2. Llenar controles del formulario
            hfIdDocumento.Set("Value", idDocumento.ToString())
            txtNoDocumento.Text = filaDocumento("NoDocumento").ToString()
            txtReferencia.Text = If(IsDBNull(filaDocumento("Referencia")), "", filaDocumento("Referencia").ToString())
            txtDocumentoRelacionado.Text = If(IsDBNull(filaDocumento("DocumentoRelacionado")), "", filaDocumento("DocumentoRelacionado").ToString())
            txtComentarios.Text = If(IsDBNull(filaDocumento("Comentarios")), "", filaDocumento("Comentarios").ToString())
            glSubEntidad.Value = filaDocumento("IdSubEntidad")
            dtFechaAsignacion.Value = filaDocumento("FechaAsignacion")

            ' 3. Cargar colonias
            Dim tablaColonias = GetServicio().ObtenerColoniasPorDocumento(idDocumento)
            Dim tablaColoniasFormato = New DataTable()

            tablaColoniasFormato.Columns.Add("Id", GetType(Integer))
            tablaColoniasFormato.Columns.Add("Clave", GetType(String))
            tablaColoniasFormato.Columns.Add("Colonia", GetType(String))
            tablaColoniasFormato.Columns.Add("MontoMin", GetType(Decimal))
            tablaColoniasFormato.Columns.Add("MontoMax", GetType(Decimal))

            If tablaColonias IsNot Nothing AndAlso tablaColonias.Rows.Count > 0 Then

                For Each fila As DataRow In tablaColonias.Rows
                    Dim nuevaFila = tablaColoniasFormato.NewRow()

                    nuevaFila("Id") = tablaColoniasFormato.Rows.Count + 1
                    nuevaFila("Clave") = If(IsDBNull(fila("Clave")), "", fila("Clave").ToString())
                    nuevaFila("Colonia") = If(IsDBNull(fila("Colonia")), "", fila("Colonia").ToString())
                    nuevaFila("MontoMin") = If(IsDBNull(fila("MontoMin")), DBNull.Value, fila("MontoMin"))
                    nuevaFila("MontoMax") = If(IsDBNull(fila("MontoMax")), DBNull.Value, fila("MontoMax"))
                    tablaColoniasFormato.Rows.Add(nuevaFila)

                Next

            End If

            Session("tablaColonias") = tablaColoniasFormato
            gridColonias.DataSource = tablaColoniasFormato
            gridColonias.DataBind()

            ' 4. Cargar conceptos
            Dim tablaConceptos = GetServicio().ObtenerConceptosPorDocumento(idDocumento)
            Dim tablaConceptosFormato = New DataTable()

            tablaConceptosFormato.Columns.Add("Id", GetType(Integer))
            tablaConceptosFormato.Columns.Add("Clave", GetType(String))
            tablaConceptosFormato.Columns.Add("Descripcion", GetType(String))
            tablaConceptosFormato.Columns.Add("CostoUnitario", GetType(Decimal))

            If tablaConceptos IsNot Nothing AndAlso tablaConceptos.Rows.Count > 0 Then

                For Each fila As DataRow In tablaConceptos.Rows
                    Dim nuevaFila = tablaConceptosFormato.NewRow()

                    nuevaFila("Id") = tablaConceptosFormato.Rows.Count + 1
                    nuevaFila("Clave") = If(IsDBNull(fila("ClaveConcepto")), "", fila("ClaveConcepto").ToString())
                    nuevaFila("Descripcion") = If(IsDBNull(fila("Descripcion")), "", fila("Descripcion").ToString())
                    nuevaFila("CostoUnitario") = If(IsDBNull(fila("CostoUnitario")), DBNull.Value, fila("CostoUnitario"))
                    tablaConceptosFormato.Rows.Add(nuevaFila)

                Next

            End If

            Session("TablaConceptos") = tablaConceptosFormato
            gridConceptos.DataSource = tablaConceptosFormato
            gridConceptos.DataBind()

            ' 5. Abrir popup y actualizar título
            popupDocumento.HeaderText = "Editar Documento"
            popupDocumento.ShowOnPageLoad = True

            ' Enviar script para abrir popup desde cliente
            Dim scriptAbrirPopup As String = "<script>

                if (typeof popupDocumento !== 'undefined') {
                    popupDocumento.SetHeaderText('Editar Documento');
                    popupDocumento.Show();
                }
            </script>"
            ClientScript.RegisterStartupScript(Me.GetType(), "AbrirPopupEdicion", scriptAbrirPopup)

        Catch ex As Exception
            Dim scriptError As String = "<script>alert('Error al cargar documento para edición: " & ex.Message.Replace("'", "\'").Replace(vbCrLf, " ") & "');</script>"

            ClientScript.RegisterStartupScript(Me.GetType(), "ErrorCargarEdicion", scriptError)
            Logger.LogError("Error al cargar documento para edición", ex)

        End Try
    End Sub

    Protected Sub CrearTablaColonias()
        Dim dt As New DataTable()

        dt.Columns.Add("Id", GetType(Integer))
        dt.Columns.Add("Clave", GetType(String))
        dt.Columns.Add("Colonia", GetType(String))
        dt.Columns.Add("MontoMin", GetType(Decimal))
        dt.Columns.Add("MontoMax", GetType(Decimal))

        ' Inicializa con una fila vacía opcional
        'dt.Rows.Add(Nothing, Nothing, Nothing, Nothing, Nothing)

        gridColonias.DataSource = dt
        gridColonias.DataBind()

    End Sub

    Protected Sub gridColonias_BatchUpdate(sender As Object, e As DevExpress.Web.Data.ASPxDataBatchUpdateEventArgs) Handles gridColonias.BatchUpdate

        For Each insert In e.InsertValues
            Dim clave As String = insert.NewValues("Clave")?.ToString()

            If Not String.IsNullOrEmpty(clave) Then
                Dim registro As ColoniasDTO = servicio.BuscarColporClave(clave)

                If registro IsNot Nothing Then
                    insert.NewValues("Colonia") = registro.UnidadTerritorial
                End If

            End If

        Next

    End Sub

    Protected Sub gridColonias_CustomCallback(sender As Object, e As DevExpress.Web.ASPxGridViewCustomCallbackEventArgs) Handles gridColonias.CustomCallback

        Try
            Dim parametros = e.Parameters.Split("|"c)
            Dim tipoOperacion As String = parametros(0)

            ' Manejar limpieza de tabla
            If tipoOperacion = "LIMPIAR" Then
                Session("tablaColonias") = Nothing
                Dim tablaColoniasLimpia = GetTablaColonias()

                gridColonias.DataSource = tablaColoniasLimpia
                gridColonias.DataBind()
                Return
            End If

            Dim valor As String = parametros(1)
            Dim rowIndex As Integer = -1

            If parametros.Length > 2 Then
                Integer.TryParse(parametros(2), rowIndex)
            End If

            Dim tabla = GetTablaColonias()
            Dim registro As ColoniasDTO = Nothing

            If tipoOperacion = "BUSCAR_CLAVE" Then
                ' Buscar por clave
                registro = GetServicio().BuscarColporClave(valor)

                If registro IsNot Nothing Then
                    ' Buscar o crear la fila
                    Dim fila = tabla.AsEnumerable().FirstOrDefault(Function(r) r("Clave").ToString() = valor)

                    If fila Is Nothing Then
                        fila = tabla.NewRow()
                        fila("Id") = If(tabla.Rows.Count > 0, tabla.AsEnumerable().Max(Function(r) Convert.ToInt32(r("Id"))) + 1, 1)
                        fila("Clave") = valor
                        tabla.Rows.Add(fila)
                    End If

                    fila("Clave") = registro.Clave
                    fila("Colonia") = registro.UnidadTerritorial

                    ' Enviar datos al cliente
                    gridColonias.JSProperties("cpEditIndex") = tabla.Rows.IndexOf(fila)
                    gridColonias.JSProperties("cpClave") = registro.Clave
                    gridColonias.JSProperties("cpColonia") = registro.UnidadTerritorial
                    gridColonias.JSProperties("cpExiste") = True
                Else
                    gridColonias.JSProperties("cpExiste") = False
                End If

            ElseIf tipoOperacion = "BUSCAR_DESCRIPCION" Then
                ' Buscar por descripción (desde dropdown)
                registro = GetServicio().BuscarColporDescripcion(valor)

                If registro IsNot Nothing Then
                    ' Buscar la fila por índice
                    If rowIndex >= 0 AndAlso rowIndex < tabla.Rows.Count Then
                        Dim fila = tabla.Rows(rowIndex)

                        fila("Clave") = registro.Clave
                        fila("Colonia") = registro.UnidadTerritorial

                        gridColonias.JSProperties("cpEditIndex") = rowIndex
                        gridColonias.JSProperties("cpClave") = registro.Clave
                        gridColonias.JSProperties("cpColonia") = registro.UnidadTerritorial
                        gridColonias.JSProperties("cpExiste") = True
                    End If
                Else
                    gridColonias.JSProperties("cpExiste") = False
                End If
            End If

            ' Persistir cambios en sesión
            Session("tablaColonias") = tabla

            ' Reasignar el DataSource
            gridColonias.DataSource = tabla
            gridColonias.DataBind()

        Catch ex As Exception
            gridColonias.JSProperties("cpError") = ex.Message

        End Try
    End Sub

    Protected Sub btnGuardarCambios_Click(sender As Object, e As EventArgs) Handles btnGuardarCambios.Click

        Try
            ' Validar controles vacíos (DevExpress ya lo hace automáticamente si usas RequiredField)
            Page.Validate()
            If Not Page.IsValid Then
                ' Ocultar loading si la validación falla
                Dim scriptOcultarLoading As String = "<script>ocultarLoading();</script>"

                ClientScript.RegisterStartupScript(Me.GetType(), "OcultarLoadingValidacion", scriptOcultarLoading)
                Exit Sub
            End If

            ' Sincronizar datos de los grids con las tablas de sesión antes de guardar
            SincronizarDatosGrids()

            ' Guardar el documento
            GuardarDocumento()

        Catch ex As Exception
            ' Ocultar loading en caso de error
            Dim scriptError As String = "<script>

                ocultarLoading();
                alert('Error al procesar: " & ex.Message.Replace("'", "\'") & "');
            </script>"
            ClientScript.RegisterStartupScript(Me.GetType(), "ErrorProcesar", scriptError)
            Logger.LogError("Error en btnGuardarCambios_Click", ex)

        End Try
    End Sub

    Private Sub SincronizarDatosGrids()

        Try
            ' Sincronizar gridColonias
            Dim tablaColonias = GetTablaColonias()

            tablaColonias.Clear()

            For i As Integer = 0 To gridColonias.VisibleRowCount - 1
                Dim fila = tablaColonias.NewRow()

                fila("Id") = i + 1
                fila("Clave") = If(gridColonias.GetRowValues(i, "Clave") Is Nothing, "", gridColonias.GetRowValues(i, "Clave").ToString())
                fila("Colonia") = If(gridColonias.GetRowValues(i, "Colonia") Is Nothing, "", gridColonias.GetRowValues(i, "Colonia").ToString())
                fila("MontoMin") = If(gridColonias.GetRowValues(i, "MontoMin") Is Nothing, DBNull.Value, gridColonias.GetRowValues(i, "MontoMin"))
                fila("MontoMax") = If(gridColonias.GetRowValues(i, "MontoMax") Is Nothing, DBNull.Value, gridColonias.GetRowValues(i, "MontoMax"))
                tablaColonias.Rows.Add(fila)

            Next

            Session("tablaColonias") = tablaColonias

            ' Sincronizar gridConceptos
            Dim tablaConceptos = GetTablaConceptos()

            tablaConceptos.Clear()

            For i As Integer = 0 To gridConceptos.VisibleRowCount - 1
                Dim fila = tablaConceptos.NewRow()

                fila("Id") = i + 1
                fila("Clave") = If(gridConceptos.GetRowValues(i, "Clave") Is Nothing, "", gridConceptos.GetRowValues(i, "Clave").ToString())
                fila("Descripcion") = If(gridConceptos.GetRowValues(i, "Descripcion") Is Nothing, "", gridConceptos.GetRowValues(i, "Descripcion").ToString())
                fila("CostoUnitario") = If(gridConceptos.GetRowValues(i, "CostoUnitario") Is Nothing, DBNull.Value, gridConceptos.GetRowValues(i, "CostoUnitario"))
                tablaConceptos.Rows.Add(fila)

            Next

            Session("TablaConceptos") = tablaConceptos

        Catch ex As Exception
            Throw New ApplicationException("Error al sincronizar datos de los grids: " & ex.Message, ex)

        End Try
    End Sub

    ''' <summary>
    ''' Limpia completamente el formulario, controles, grids y variables de sesión
    ''' </summary>
    Private Sub LimpiarFormularioCompleto()

        Try
            ' 1. Limpiar variables de sesión
            Session("tablaColonias") = Nothing
            Session("TablaConceptos") = Nothing
            ' Nota: No limpiamos Session("ConceptosCombo") porque es cache que se reutiliza

            ' 2. Limpiar controles del formulario
            txtNoDocumento.Text = ""
            txtReferencia.Text = ""
            txtDocumentoRelacionado.Text = ""
            txtComentarios.Text = ""
            glSubEntidad.Value = Nothing
            dtFechaAsignacion.Value = Nothing
            hfIdDocumento.Set("Value", "")

            ' 3. Limpiar y resetear grids
            ' Crear nuevas tablas vacías
            Dim tablaColoniasVacia = New DataTable()

            tablaColoniasVacia.Columns.Add("Id", GetType(Integer))
            tablaColoniasVacia.Columns.Add("Clave", GetType(String))
            tablaColoniasVacia.Columns.Add("Colonia", GetType(String))
            tablaColoniasVacia.Columns.Add("MontoMin", GetType(Decimal))
            tablaColoniasVacia.Columns.Add("MontoMax", GetType(Decimal))

            Dim tablaConceptosVacia = New DataTable()

            tablaConceptosVacia.Columns.Add("Id", GetType(Integer))
            tablaConceptosVacia.Columns.Add("Clave", GetType(String))
            tablaConceptosVacia.Columns.Add("Descripcion", GetType(String))
            tablaConceptosVacia.Columns.Add("CostoUnitario", GetType(Decimal))

            ' Asignar tablas vacías a los grids
            gridColonias.DataSource = tablaColoniasVacia
            gridColonias.DataBind()

            gridConceptos.DataSource = tablaConceptosVacia
            gridConceptos.DataBind()

            ' 4. Resetear variables de sesión con tablas vacías (para futuras ediciones)
            Session("tablaColonias") = tablaColoniasVacia
            Session("TablaConceptos") = tablaConceptosVacia

        Catch ex As Exception
            Logger.LogError("Error al limpiar formulario completo", ex)
            ' No lanzar excepción para no interrumpir el flujo de guardado

        End Try
    End Sub

    Protected Sub gridConceptos_CustomCallback(sender As Object, e As DevExpress.Web.ASPxGridViewCustomCallbackEventArgs) Handles gridConceptos.CustomCallback

        Try
            Dim parametros = e.Parameters.Split("|"c)
            Dim tipoOperacion As String = parametros(0)

            ' Manejar limpieza de tabla
            If tipoOperacion = "LIMPIAR" Then
                Session("TablaConceptos") = Nothing
                Dim tablaConceptosLimpia = GetTablaConceptos()

                gridConceptos.DataSource = tablaConceptosLimpia
                gridConceptos.DataBind()
                Return
            End If

            Dim valor As String = parametros(1)
            Dim rowIndex As Integer = -1

            If parametros.Length > 2 Then
                Integer.TryParse(parametros(2), rowIndex)
            End If

            Dim tabla = GetTablaConceptos()
            Dim concepto As ConceptosDTO = Nothing

            If tipoOperacion = "BUSCAR_CLAVE" Then
                ' Buscar por clave
                concepto = GetServicio().GetConceptoByClave(valor)

                If concepto IsNot Nothing Then
                    ' Buscar o crear la fila
                    Dim fila = tabla.AsEnumerable().FirstOrDefault(Function(r) r("Clave").ToString() = valor)

                    If fila Is Nothing Then
                        fila = tabla.NewRow()
                        fila("Id") = If(tabla.Rows.Count > 0, tabla.AsEnumerable().Max(Function(r) Convert.ToInt32(r("Id"))) + 1, 1)
                        fila("Clave") = valor
                        tabla.Rows.Add(fila)
                    End If

                    fila("Clave") = concepto.Clave
                    fila("Descripcion") = concepto.Concepto
                    fila("CostoUnitario") = concepto.Costo

                    ' Enviar datos al cliente
                    gridConceptos.JSProperties("cpEditIndex") = tabla.Rows.IndexOf(fila)
                    gridConceptos.JSProperties("cpClave") = concepto.Clave
                    gridConceptos.JSProperties("cpDescripcion") = concepto.Concepto
                    gridConceptos.JSProperties("cpCosto") = concepto.Costo
                    gridConceptos.JSProperties("cpExiste") = True
                Else
                    gridConceptos.JSProperties("cpExiste") = False
                End If

            ElseIf tipoOperacion = "BUSCAR_DESCRIPCION" Then
                ' Buscar por descripción (desde dropdown)
                concepto = GetServicio().BuscarConceptoPorDescripcion(valor)

                If concepto IsNot Nothing Then
                    ' Buscar la fila por índice
                    If rowIndex >= 0 AndAlso rowIndex < tabla.Rows.Count Then
                        Dim fila = tabla.Rows(rowIndex)

                        fila("Clave") = concepto.Clave
                        fila("Descripcion") = concepto.Concepto
                        fila("CostoUnitario") = concepto.Costo

                        gridConceptos.JSProperties("cpEditIndex") = rowIndex
                        gridConceptos.JSProperties("cpClave") = concepto.Clave
                        gridConceptos.JSProperties("cpDescripcion") = concepto.Concepto
                        gridConceptos.JSProperties("cpCosto") = concepto.Costo
                        gridConceptos.JSProperties("cpExiste") = True
                    End If
                Else
                    gridConceptos.JSProperties("cpExiste") = False
                End If
            End If

            ' Persistir cambios en sesión
            Session("TablaConceptos") = tabla

            ' Reasignar el DataSource
            gridConceptos.DataSource = tabla
            gridConceptos.DataBind()

        Catch ex As Exception
            gridConceptos.JSProperties("cpError") = ex.Message

        End Try
    End Sub

    Private Sub LlenarConceptos()

        Try
            ' Si ya tenemos los datos en sesión, no volvemos a llamar al API
            Dim conceptos As DataTable = TryCast(Session("ConceptosCombo"), DataTable)

            If conceptos Is Nothing Then
                ' Consumir API solo una vez
                conceptos = GetServicio().ListarConceptosCombo()
                Session("ConceptosCombo") = conceptos
            End If

        Catch ex As Exception
        End Try
    End Sub

    Protected Sub gridConceptos_CellEditorInitialize(sender As Object, e As ASPxGridViewEditorEventArgs) Handles gridConceptos.CellEditorInitialize
        If e.Column.FieldName = "Descripcion" Then
            Dim combo As ASPxComboBox = CType(e.Editor, ASPxComboBox)

            ' Usar los datos cacheados
            LlenarConceptos()
            Dim conceptos As DataTable = CType(Session("ConceptosCombo"), DataTable)

            If conceptos IsNot Nothing Then
                combo.DataSource = conceptos
                combo.TextField = "Descripcion"
                combo.ValueField = "Descripcion"
                combo.DataBind()
            End If
        End If

    End Sub

    Protected Sub gridColonias_CellEditorInitialize(sender As Object, e As ASPxGridViewEditorEventArgs) Handles gridColonias.CellEditorInitialize
        If e.Column.FieldName = "Colonia" Then
            Dim combo As ASPxComboBox = CType(e.Editor, ASPxComboBox)

            ' Obtener IdEntidad desde glSubEntidad si está disponible
            Dim idEntidad As Integer = 1 ' Valor por defecto

            If glSubEntidad.Value IsNot Nothing Then
                idEntidad = Convert.ToInt32(glSubEntidad.Value)
            End If

            ' Obtener colonias de la entidad
            ' Usar GetServicio para asegurar que servicio esté inicializado
            Dim colonias As DataTable = GetServicio().ListarColoniasCombo(idEntidad)

            If colonias IsNot Nothing Then
                combo.DataSource = colonias
                combo.TextField = "Descripcion"
                combo.ValueField = "Descripcion"
                combo.DataBind()
            End If
        End If

    End Sub

    Protected Sub gridColonias_DataBound(sender As Object, e As EventArgs) Handles gridColonias.DataBound
        Dim tabla As DataTable = TryCast(gridColonias.DataSource, DataTable)

        FuncionesGridWeb.SUMColumn(gridColonias, tabla)
    End Sub

    Protected Sub gridConceptos_DataBound(sender As Object, e As EventArgs) Handles gridConceptos.DataBound
        Dim tabla As DataTable = TryCast(gridConceptos.DataSource, DataTable)

        FuncionesGridWeb.SUMColumn(gridConceptos, tabla)
    End Sub

    Protected Sub GuardarDocumento()

        Try
            ' Validaciones previas
            If String.IsNullOrWhiteSpace(txtNoDocumento.Text) Then
                Throw New ApplicationException("El número de documento es obligatorio.")
            End If

            If glSubEntidad.Value Is Nothing Then
                Throw New ApplicationException("Debe seleccionar una sub-entidad.")
            End If

            If dtFechaAsignacion.Value Is Nothing Then
                Throw New ApplicationException("La fecha de asignación es obligatoria.")
            End If

            ' Obtener tablas de sesión
            Dim tablaColonias = GetTablaColonias()
            Dim tablaConceptos = GetTablaConceptos()

            ' Validar que haya al menos una colonia o un concepto
            Dim filasColoniasValidas = tablaColonias.AsEnumerable().Where(Function(r) Not String.IsNullOrWhiteSpace(r("Clave").ToString())).Count()
            Dim filasConceptosValidas = tablaConceptos.AsEnumerable().Where(Function(r) Not String.IsNullOrWhiteSpace(r("Clave").ToString())).Count()

            If filasColoniasValidas = 0 AndAlso filasConceptosValidas = 0 Then
                Throw New ApplicationException("Debe agregar al menos una colonia o un concepto al documento.")
            End If

            ' Determinar si es edición o creación
            Dim esEdicion As Boolean = False
            Dim idDocumento As Integer = 0
            Dim valorIdDocumento As String = hfIdDocumento.Get("Value")

            If Not String.IsNullOrWhiteSpace(valorIdDocumento) Then
                Integer.TryParse(valorIdDocumento, idDocumento)
                esEdicion = (idDocumento > 0)
            End If

            ' Consumidor de API
            Dim consumer As New ApiConsumerCRUD()

            ' 1. Construir DTO para la cabecera (op_documentos)
            Dim dtoCabecera As New DynamicDto()

            If esEdicion Then
                ' Para edición, incluir el ID
                dtoCabecera("Id") = idDocumento
            End If

            dtoCabecera("IdEntidad") = 1 ' TODO: Obtener de la sesión o configuración
            dtoCabecera("IdTipoDocumento") = Convert.ToInt32(glTipoDocumento.Value)
            dtoCabecera("NoDocumento") = txtNoDocumento.Text.Trim()
            dtoCabecera("IdSubEntidad") = Convert.ToInt32(glSubEntidad.Value)

            If Not esEdicion Then
                dtoCabecera("FechaCreacion") = Format(Now, "yyyy-MM-dd")
            End If

            dtoCabecera("FechaAsignacion") = Format(Convert.ToDateTime(dtFechaAsignacion.Value), "yyyy-MM-dd")
            dtoCabecera("NoPartidas") = filasConceptosValidas
            dtoCabecera("Referencia") = If(String.IsNullOrWhiteSpace(txtReferencia.Text), DBNull.Value, txtReferencia.Text.Trim())
            dtoCabecera("DocumentoRelacionado") = If(String.IsNullOrWhiteSpace(txtDocumentoRelacionado.Text), DBNull.Value, txtDocumentoRelacionado.Text.Trim())
            dtoCabecera("IdProceso") = 1 ' TODO: Obtener de la configuración
            dtoCabecera("Comentarios") = If(String.IsNullOrWhiteSpace(txtComentarios.Text), DBNull.Value, txtComentarios.Text.Trim())
            dtoCabecera("FechaUltimoMovimiento") = Format(Now, "yyyy-MM-dd")
            dtoCabecera("Activo") = 1

            ' 2. Guardar o actualizar cabecera
            Dim apiUrlCabecera As String = ConfigurationManager.AppSettings("APIPost") & "op_documentos"

            If esEdicion Then
                ' Actualizar con PUT
                consumer.EnviarPut(apiUrlCabecera, dtoCabecera)
            Else
                ' Crear con POST y obtener ID
                idDocumento = consumer.EnviarPostId(apiUrlCabecera, dtoCabecera)

                ' Validar que se obtuvo un ID válido
                If idDocumento <= 0 Then
                    Throw New ApplicationException("No se pudo guardar la cabecera del documento. Proceso cancelado.")
                End If
            End If

            ' 3. Si es edición, eliminar colonias y conceptos existentes antes de agregar los nuevos
            If esEdicion Then
                EliminarColoniasYConceptosExistentes(idDocumento)
            End If

            ' 4. Guardar colonias en op_documentos_colonias
            Dim apiUrlColonias As String = ConfigurationManager.AppSettings("APIPost") & "op_documentos_colonias"

            For Each fila As DataRow In tablaColonias.Rows
                ' Solo guardar filas con clave válida
                Dim claveColonia As String = If(fila("Clave") Is Nothing, "", fila("Clave").ToString().Trim())

                If Not String.IsNullOrWhiteSpace(claveColonia) Then
                    ' Obtener información completa de la colonia si es necesario
                    Dim coloniaDTO As ColoniasDTO = Nothing

                    Try
                        coloniaDTO = GetServicio().BuscarColporClave(claveColonia)

                    Catch
                        ' Si no se puede obtener, usar los datos de la fila

                    End Try
                    Dim dtoColonia As New DynamicDto()

                    dtoColonia("IdDocumento") = idDocumento
                    dtoColonia("Clave") = claveColonia

                    ' Usar Colonia de la fila o del DTO si está disponible
                    If coloniaDTO IsNot Nothing Then
                        dtoColonia("Colonia") = coloniaDTO.UnidadTerritorial
                        If Not String.IsNullOrWhiteSpace(coloniaDTO.CP) Then
                            dtoColonia("CodigoPostal") = coloniaDTO.CP
                        End If
                    Else
                        dtoColonia("Colonia") = If(fila("Colonia") Is Nothing, "", fila("Colonia").ToString().Trim())
                    End If

                    ' Montos
                    dtoColonia("MontoMin") = If(IsDBNull(fila("MontoMin")) OrElse fila("MontoMin") Is Nothing, DBNull.Value, fila("MontoMin"))
                    dtoColonia("MontoMax") = If(IsDBNull(fila("MontoMax")) OrElse fila("MontoMax") Is Nothing, DBNull.Value, fila("MontoMax"))

                    consumer.EnviarPost(apiUrlColonias, dtoColonia)
                End If

            Next

            ' 5. Guardar conceptos en op_documentos_detalle
            Dim apiUrlDetalle As String = ConfigurationManager.AppSettings("APIPost") & "op_documentos_detalle"

            For Each fila As DataRow In tablaConceptos.Rows
                ' Solo guardar filas con clave válida
                If Not String.IsNullOrWhiteSpace(fila("Clave").ToString()) Then
                    Dim dtoDetalle As New DynamicDto()

                    dtoDetalle("IdDocumento") = idDocumento
                    dtoDetalle("ClaveConcepto") = fila("Clave").ToString().Trim()
                    dtoDetalle("Descripcion") = If(String.IsNullOrWhiteSpace(fila("Descripcion").ToString()), DBNull.Value, fila("Descripcion").ToString().Trim())
                    dtoDetalle("CostoUnitario") = If(IsDBNull(fila("CostoUnitario")) OrElse fila("CostoUnitario") Is Nothing, 0, fila("CostoUnitario"))
                    ' Si la API requiere Cantidad e Importe, calcularlos o usar valores por defecto
                    dtoDetalle("Cantidad") = 1 ' Valor por defecto, ajustar según requerimientos
                    dtoDetalle("Importe") = If(IsDBNull(fila("CostoUnitario")) OrElse fila("CostoUnitario") Is Nothing, 0, fila("CostoUnitario"))

                    consumer.EnviarPost(apiUrlDetalle, dtoDetalle)
                End If

            Next

            ' 6. Si todo salió bien, limpiar y cerrar
            LimpiarFormularioCompleto()

            ' Cerrar popup
            popupDocumento.ShowOnPageLoad = False

            ' Recargar grid de documentos (esto actualiza el grid en el servidor)
            CargarDocumentos()

            ' Mostrar mensaje de éxito y cerrar popup mediante JavaScript
            Dim mensajeExito As String = If(esEdicion, "Documento actualizado correctamente con ID: " & idDocumento, "Documento guardado correctamente con ID: " & idDocumento)
            Dim scriptExito As String = "<script>

                // Ocultar loading overlay
                ocultarLoading();
                alert('" & mensajeExito & "');
                // Cerrar popup
                if (typeof popupDocumento !== 'undefined') {
                    popupDocumento.Hide();
                }
                // Actualizar grid principal - usar callback para refrescar datos
                if (typeof gridDocumentos !== 'undefined') {
                    FiltrarDocumentos();
                }
            </script>"
            ClientScript.RegisterStartupScript(Me.GetType(), "GuardadoExitoso", scriptExito)

        Catch ex As ApplicationException

            ' Ocultar loading en caso de error
            Dim scriptError As String = "<script>

                ocultarLoading();
                alert('Error al guardar: " & ex.Message.Replace("'", "\'").Replace(vbCrLf, " ") & "');
            </script>"
            ClientScript.RegisterStartupScript(Me.GetType(), "ErrorGuardado", scriptError)
            Logger.LogError("Error al guardar documento", ex)

        Catch ex As Exception
            ' Ocultar loading en caso de error
            Dim scriptError As String = "<script>

                ocultarLoading();
                alert('Error inesperado al guardar el documento: " & ex.Message.Replace("'", "\'").Replace(vbCrLf, " ") & "');
            </script>"
            ClientScript.RegisterStartupScript(Me.GetType(), "ErrorInesperado", scriptError)
            Logger.LogError("Error inesperado al guardar documento", ex)

        End Try
    End Sub

    Private Sub EliminarColoniasYConceptosExistentes(idDocumento As Integer)

        Try
            ' Usar el servicio para ejecutar queries DELETE
            ' Eliminar colonias existentes usando query DELETE
            Dim queryDeleteColonias As String = "DELETE FROM op_documentos_colonias WHERE IdDocumento = " & QueryBuilder.EscapeSqlInteger(idDocumento)
            Dim apiUrlDelete As String = ConfigurationManager.AppSettings("ApiBaseUrl") & System.Web.HttpUtility.UrlEncode(queryDeleteColonias)

            If apiConsumer Is Nothing Then
                apiConsumer = New ApiConsumer()
            End If

            apiConsumer.ObtenerDatos(apiUrlDelete)

            ' Eliminar conceptos existentes usando query DELETE
            Dim queryDeleteConceptos As String = "DELETE FROM op_documentos_detalle WHERE IdDocumento = " & QueryBuilder.EscapeSqlInteger(idDocumento)
            Dim apiUrlDeleteConceptos As String = ConfigurationManager.AppSettings("ApiBaseUrl") & System.Web.HttpUtility.UrlEncode(queryDeleteConceptos)

            apiConsumer.ObtenerDatos(apiUrlDeleteConceptos)

        Catch ex As Exception
            ' Log del error pero no lanzar excepción para no interrumpir el proceso
            Logger.LogError("Error al eliminar colonias y conceptos existentes del documento " & idDocumento, ex)

        End Try
    End Sub

    Protected Sub gridDocumentos_ToolbarItemClick(source As Object, e As Data.ASPxGridViewToolbarItemClickEventArgs) Handles gridDocumentos.ToolbarItemClick
        If e.Item.Name = "btnExportar" Then
            gridExporter.WriteXlsxToResponse("Documentos_" & DateTime.Now.ToString("yyyyMMdd_HHmmss"), True)
        End If

    End Sub
    Protected Sub CargarSubEntidades()
        'Dim tipos_doc As DataTable = 

        '' Normalizar para asegurar que Clave sea String
        'Dim tablaLimpia As New DataTable()
        'tablaLimpia.Columns.Add("Id", GetType(Integer))
        'tablaLimpia.Columns.Add("Clave", GetType(String))
        'tablaLimpia.Columns.Add("Nombre", GetType(String))

        'For Each row As DataRow In tipos_doc.Rows
        '    tablaLimpia.Rows.Add(row("Id"), row("Clave").ToString(), row("Nombre").ToString())
        'Next

        glSubEntidad.DataSource = GetServicio().ListarSubEntidades(1)
        glSubEntidad.KeyFieldName = "Id"
        glSubEntidad.DataBind()

    End Sub

    Protected Sub gridDocumentos_DataBound(sender As Object, e As EventArgs) Handles gridDocumentos.DataBound
        Dim tabla As DataTable = TryCast(gridDocumentos.DataSource, DataTable)

        FuncionesGridWeb.SUMColumn(gridDocumentos, tabla)
    End Sub
End Class