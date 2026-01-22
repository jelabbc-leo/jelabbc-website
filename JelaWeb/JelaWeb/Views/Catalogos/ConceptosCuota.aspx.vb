Imports DevExpress.Web
Imports System.Data
Imports System.Linq
Imports JelaWeb.Utilities
Imports JelaWeb.Services
Imports JelaWeb.Infrastructure.Helpers

Partial Public Class ConceptosCuota
    Inherits BasePage

#Region "Page Events"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load

        Try
            ' Suscribir eventos del grid
            AddHandler gridConceptos.DataBound, AddressOf gridConceptos_DataBound

            If Not IsPostBack Then
                CargarCombos()
                CargarConceptos()
            End If

        Catch ex As Exception
            Logger.LogError("ConceptosCuota.Page_Load", ex)
            ConceptosCuotaHelper.MostrarMensaje(Me, "Error al cargar la página", "error")

        End Try
    End Sub

#End Region

#Region "Carga de Datos"

    Private Sub CargarConceptos()

        Try
            ' Obtener datos del servicio
            Dim dt As DataTable = ConceptosCuotaService.ListarConceptos()

            ' Generar columnas dinámicamente desde el DataTable
            GenerarColumnasDinamicas(gridConceptos, dt)

            ' Guardar DataTable en Session ANTES de DataBind para FuncionesGridWeb
            Session("dtConceptos") = dt

            gridConceptos.DataSource = dt
            gridConceptos.DataBind()

        Catch ex As Exception
            Logger.LogError("ConceptosCuota.CargarConceptos", ex)
            ConceptosCuotaHelper.MostrarMensaje(Me, "Error al cargar conceptos", "error")

        End Try
    End Sub

    Private Sub CargarCombos()

        Try
            ' Cargar Entidades desde el servicio
            Dim dtEntidades As DataTable = ConceptosCuotaService.ListarEntidades()

            cmbEntidad.DataSource = dtEntidades
            cmbEntidad.TextField = "RazonSocial"
            cmbEntidad.ValueField = "Id"
            cmbEntidad.DataBind()

        Catch ex As Exception
            Logger.LogError("ConceptosCuota.CargarCombos", ex)
            ConceptosCuotaHelper.MostrarMensaje(Me, "Error al cargar combos", "error")

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
            Logger.LogError("ConceptosCuota.GenerarColumnasDinamicas", ex)
            Throw

        End Try
    End Sub

#End Region

#Region "Eventos del Grid"

    Protected Sub gridConceptos_DataBound(sender As Object, e As EventArgs)

        Try
            ' Leer DataTable desde Session (guardado antes de DataBind)
            Dim tabla As DataTable = TryCast(Session("dtConceptos"), DataTable)

            If tabla IsNot Nothing Then
                FuncionesGridWeb.SUMColumn(gridConceptos, tabla)
            End If

        Catch ex As Exception
            Logger.LogError("ConceptosCuota.gridConceptos_DataBound", ex)

        End Try
    End Sub

#End Region

#Region "Guardar Concepto"

    Protected Sub btnGuardar_Click(sender As Object, e As EventArgs)

        Try
            Dim conceptoId As Integer = Integer.Parse(hfConceptoId.Value)
            Dim datos As New Dictionary(Of String, Object)

            ' Datos generales
            datos("EntidadId") = CInt(cmbEntidad.Value)
            datos("Clave") = txtClave.Text.Trim().ToUpper()
            datos("Nombre") = txtNombre.Text.Trim()
            datos("Descripcion") = txtDescripcion.Text.Trim()
            datos("TipoCuota") = cmbTipoCuota.Value?.ToString()
            datos("MontoBase") = spnMontoBase.Number
            datos("CuentaContable") = txtCuentaContable.Text.Trim()
            datos("Activo") = If(chkActivo.Checked, 1, 0)

            ' Configuración de cobro
            datos("EsRecurrente") = If(chkEsRecurrente.Checked, 1, 0)
            datos("Periodicidad") = cmbPeriodicidad.Value?.ToString()
            datos("DiaVencimiento") = CInt(spnDiaVencimiento.Number)
            datos("DiasGracia") = CInt(spnDiasGracia.Number)

            ' Recargos
            datos("AplicaRecargo") = If(chkAplicaRecargo.Checked, 1, 0)
            datos("PorcentajeRecargo") = spnPorcentajeRecargo.Number

            ' Descuentos
            datos("AplicaDescuentoProntoPago") = If(chkAplicaDescuento.Checked, 1, 0)
            datos("PorcentajeDescuento") = spnPorcentajeDescuento.Number
            datos("DiaLimiteDescuento") = CInt(spnDiaLimiteDescuento.Number)

            Dim currentUser = SessionHelper.GetCurrentUser()
            Dim userId As Integer? = If(currentUser IsNot Nothing, CInt(currentUser.Id), CType(Nothing, Integer?))

            ' Guardar a través del servicio
            Dim resultado As Boolean = ConceptosCuotaService.GuardarConcepto(conceptoId, datos, userId)

            If conceptoId = 0 Then
                Logger.LogInfo($"Concepto creado: {datos("Clave")} - {datos("Nombre")}")
            Else
                Logger.LogInfo($"Concepto actualizado: Id={conceptoId}")
            End If

            If resultado Then
                ConceptosCuotaHelper.MostrarMensaje(Me, "Concepto guardado correctamente", "success")
                CargarConceptos()
                ScriptManager.RegisterStartupScript(Me, Me.GetType(), "hidePopup", "popupConcepto.Hide();", True)
            Else
                ConceptosCuotaHelper.MostrarMensaje(Me, "Error al guardar el concepto", "error")
            End If

        Catch ex As Exception
            Logger.LogError("ConceptosCuota.btnGuardar_Click", ex)
            ConceptosCuotaHelper.MostrarMensaje(Me, "Error al guardar el concepto: " & ex.Message, "error")

        End Try
    End Sub

#End Region

#Region "WebMethods"

    <System.Web.Services.WebMethod()>
    Public Shared Function ObtenerConcepto(id As Integer) As Object
        Return ConceptosCuotaService.ObtenerConcepto(id)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function EliminarConcepto(id As Integer) As Object
        Return ConceptosCuotaService.EliminarConcepto(id)
    End Function

#End Region

#Region "Helpers"

    ' Los Helpers se han movido a ConceptosCuotaHelper

#End Region

End Class
