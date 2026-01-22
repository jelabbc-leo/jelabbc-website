Imports DevExpress.Web
Imports System.Data
Imports JelaWeb.Utilities
Imports JelaWeb.Services
Imports JelaWeb.Infrastructure.Helpers

Partial Public Class SubEntidades
    Inherits BasePage

#Region "Page Events"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load

        Try
            If Not IsPostBack Then
                CargarSubEntidades()
            End If

        Catch ex As Exception
            Logger.LogError("SubEntidades.Page_Load", ex)
            SubEntidadesHelper.MostrarMensaje(Me, "Error al cargar la página", "error")

        End Try
    End Sub

#End Region

#Region "Carga de Datos"

    Private Sub CargarSubEntidades()

        Try
            Dim dt As DataTable = SubEntidadesService.ListarSubEntidades()

            GenerarColumnasDinamicas(gridSubEntidades, dt)

            Session("dtSubEntidades") = dt

            gridSubEntidades.DataSource = dt
            gridSubEntidades.DataBind()

        Catch ex As Exception
            Logger.LogError("SubEntidades.CargarSubEntidades", ex)
            SubEntidadesHelper.MostrarMensaje(Me, "Error al cargar sub entidades", "error")

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

            Dim columnasOcultar As String() = {"Id", "IdEntidad", "Contraseña", "Usuario", "IdPerfil"}

            For i As Integer = grid.Columns.Count - 1 To 0 Step -1
                grid.Columns.RemoveAt(i)
            Next

            For Each col As DataColumn In tabla.Columns
                Dim nombreColumna = col.ColumnName
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
                        gridCol.PropertiesEdit.DisplayFormatString = "n2"

                    Case GetType(Integer), GetType(Long), GetType(Short)
                        gridCol = New GridViewDataTextColumn()
                        gridCol.Width = Unit.Pixel(120)
                        gridCol.PropertiesEdit.DisplayFormatString = "n0"

                    Case Else
                        gridCol = New GridViewDataTextColumn()
                        gridCol.Width = Unit.Pixel(180)

                End Select

                gridCol.FieldName = nombreColumna
                gridCol.Caption = nombreColumna
                gridCol.ReadOnly = True

                If columnasOcultar.Contains(nombreColumna, StringComparer.OrdinalIgnoreCase) Then
                    gridCol.Visible = False
                Else
                    gridCol.Settings.AllowHeaderFilter = True
                    gridCol.Settings.AllowGroup = True
                End If

                grid.Columns.Add(gridCol)
            Next

        Catch ex As Exception
            Logger.LogError("SubEntidades.GenerarColumnasDinamicas", ex)
            Throw

        End Try
    End Sub

#End Region

#Region "Eventos del Grid"

    Protected Sub gridSubEntidades_DataBound(sender As Object, e As EventArgs)

        Try
            Dim tabla As DataTable = TryCast(Session("dtSubEntidades"), DataTable)

            If tabla IsNot Nothing Then
                FuncionesGridWeb.SUMColumn(gridSubEntidades, tabla)
            End If

        Catch ex As Exception
            Logger.LogError("SubEntidades.gridSubEntidades_DataBound", ex)

        End Try
    End Sub

#End Region

#Region "Guardar Sub Entidad"

    Protected Sub btnGuardarSubEntidad_Click(sender As Object, e As EventArgs)

        Try
            Dim subEntidadId As Integer = Integer.Parse(hfSubEntidadId.Value)
            Dim datos As New Dictionary(Of String, Object)

            datos("Clave") = txtClave.Text.Trim()
            datos("Alias") = If(String.IsNullOrWhiteSpace(txtAlias.Text), DBNull.Value, txtAlias.Text.Trim())
            datos("CIF") = If(String.IsNullOrWhiteSpace(txtCIF.Text), DBNull.Value, txtCIF.Text.Trim())
            datos("RFC") = If(String.IsNullOrWhiteSpace(txtRFC.Text), DBNull.Value, txtRFC.Text.Trim())
            datos("RazonSocial") = txtRazonSocial.Text.Trim()

            If dateFechaAlta.Value IsNot Nothing Then
                datos("FechaAlta") = dateFechaAlta.Date
            Else
                datos("FechaAlta") = DBNull.Value
            End If

            datos("Activo") = If(chkActivo.Checked, 1, 0)

            datos("Telefonos") = If(String.IsNullOrWhiteSpace(txtTelefonos.Text), DBNull.Value, txtTelefonos.Text.Trim())
            datos("Whatsapp") = If(String.IsNullOrWhiteSpace(txtWhatsapp.Text), DBNull.Value, txtWhatsapp.Text.Trim())
            datos("Mail") = If(String.IsNullOrWhiteSpace(txtMail.Text), DBNull.Value, txtMail.Text.Trim())
            datos("Administrador") = If(String.IsNullOrWhiteSpace(txtAdministrador.Text), DBNull.Value, txtAdministrador.Text.Trim())
            datos("TelefonoVigilancia") = If(String.IsNullOrWhiteSpace(txtTelefonoVigilancia.Text), DBNull.Value, txtTelefonoVigilancia.Text.Trim())

            datos("CP") = If(String.IsNullOrWhiteSpace(txtCP.Text), DBNull.Value, txtCP.Text.Trim())
            datos("TipoVialidad") = If(String.IsNullOrWhiteSpace(txtTipoVialidad.Text), DBNull.Value, txtTipoVialidad.Text.Trim())
            datos("NombreVialidad") = If(String.IsNullOrWhiteSpace(txtNombreVialidad.Text), DBNull.Value, txtNombreVialidad.Text.Trim())
            datos("NoExterior") = If(String.IsNullOrWhiteSpace(txtNoExterior.Text), DBNull.Value, txtNoExterior.Text.Trim())
            datos("NoInterior") = If(String.IsNullOrWhiteSpace(txtNoInterior.Text), DBNull.Value, txtNoInterior.Text.Trim())
            datos("Colonia") = If(String.IsNullOrWhiteSpace(txtColonia.Text), DBNull.Value, txtColonia.Text.Trim())
            datos("Localidad") = If(String.IsNullOrWhiteSpace(txtLocalidad.Text), DBNull.Value, txtLocalidad.Text.Trim())
            datos("Municipio") = If(String.IsNullOrWhiteSpace(txtMunicipio.Text), DBNull.Value, txtMunicipio.Text.Trim())
            datos("EntidadFederativa") = If(String.IsNullOrWhiteSpace(txtEntidadFederativa.Text), DBNull.Value, txtEntidadFederativa.Text.Trim())
            datos("EntreCalle") = If(String.IsNullOrWhiteSpace(txtEntreCalle.Text), DBNull.Value, txtEntreCalle.Text.Trim())

            datos("EsSeccionCondominio") = If(chkEsSeccionCondominio.Checked, 1, 0)
            datos("TipoSeccion") = If(cmbTipoSeccion.Value IsNot Nothing, cmbTipoSeccion.Value.ToString(), DBNull.Value)
            datos("NumeroNiveles") = CInt(spnNumeroNiveles.Number)
            datos("NumeroUnidades") = CInt(spnNumeroUnidades.Number)
            datos("TieneElevador") = If(chkTieneElevador.Checked, 1, 0)
            datos("NumeroElevadores") = CInt(spnNumeroElevadores.Number)
            datos("TieneEstacionamiento") = If(chkTieneEstacionamiento.Checked, 1, 0)
            datos("CajonesEstacionamiento") = CInt(spnCajonesEstacionamiento.Number)

            datos("Latitud") = spnLatitud.Number
            datos("Longitud") = spnLongitud.Number

            Dim nuevoId As Integer = SubEntidadesService.GuardarSubEntidad(subEntidadId, datos)

            If nuevoId > 0 Then
                hfSubEntidadId.Value = nuevoId.ToString()
                CargarSubEntidades()
                SubEntidadesHelper.MostrarMensaje(Me, "Guardado correctamente", "success")
                ScriptManager.RegisterStartupScript(Me, Me.GetType(), "hidePopup", "popupSubEntidad.Hide();", True)
            Else
                SubEntidadesHelper.MostrarMensaje(Me, "Error al guardar", "error")
            End If

        Catch ex As Exception
            Logger.LogError("SubEntidades.btnGuardarSubEntidad_Click", ex)
            SubEntidadesHelper.MostrarMensaje(Me, "Error al guardar", "error")

        End Try
    End Sub

#End Region

#Region "WebMethods"

    <System.Web.Services.WebMethod()>
    Public Shared Function ObtenerSubEntidad(id As Integer) As Object
        Return SubEntidadesService.ObtenerSubEntidad(id)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function EliminarSubEntidad(id As Integer) As Object
        Return SubEntidadesService.EliminarSubEntidad(id)
    End Function

#End Region

End Class
