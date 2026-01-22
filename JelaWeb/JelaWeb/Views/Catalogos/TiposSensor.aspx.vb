Imports System.Linq
Imports System.Data

Public Class TiposSensor
    Inherits BasePage
    Implements System.Web.UI.IPostBackEventHandler

    Private tipoSensorService As New TipoSensorService()

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load

        Try
            If Not IsPostBack Then
                CargarDatos()
            End If

        Catch ex As Exception
            logger.LogError("Error en Page_Load", ex)
            MostrarMensaje("Error al cargar la página", "error")

        End Try
    End Sub

    Private Sub CargarDatos()

        Try
            Dim filtroActivo As String = cboFiltroActivo.Value?.ToString()
            Dim busqueda As String = txtBuscar.Text
            Dim datosTable = tipoSensorService.ObtenerTodos(SessionHelper.GetIdEntidadActual().GetValueOrDefault(1))
            Dim datos = datosTable.AsEnumerable().Select(Function(row) New With {
                .Id = Convert.ToInt32(row("Id")),
                .Nombre = row("Nombre").ToString(),
                .Descripcion = If(IsDBNull(row("Descripcion")), "", row("Descripcion").ToString()),
                .UnidadMedida = If(IsDBNull(row("UnidadMedida")), "", row("UnidadMedida").ToString()),
                .SimboloUnidad = If(IsDBNull(row("SimboloUnidad")), "", row("SimboloUnidad").ToString()),
                .UmbralMinimo = If(IsDBNull(row("UmbralMinimo")), Nothing, CType(row("UmbralMinimo"), Decimal?)),
                .UmbralMaximo = If(IsDBNull(row("UmbralMaximo")), Nothing, CType(row("UmbralMaximo"), Decimal?)),
                .Activo = Convert.ToBoolean(row("Activo")),
                .FechaCreacion = Convert.ToDateTime(row("FechaCreacion")),
                .FechaModificacion = Convert.ToDateTime(row("FechaModificacion"))
            }).ToList()

            If Not String.IsNullOrEmpty(filtroActivo) Then
                Dim activo As Boolean = Boolean.Parse(filtroActivo)

                datos = datos.Where(Function(d) d.Activo = activo).ToList()
            End If

            If Not String.IsNullOrEmpty(busqueda) Then
                datos = datos.Where(Function(d) d.Nombre.ToLower().Contains(busqueda.ToLower()) OrElse (d.Descripcion IsNot Nothing AndAlso d.Descripcion.ToLower().Contains(busqueda.ToLower()))).ToList()
            End If

            gridTipos.DataSource = datos
            gridTipos.DataBind()

        Catch ex As Exception
            Logger.LogError("Error al cargar datos", ex, "")
            MostrarMensaje("Error al cargar los datos", "error")

        End Try
    End Sub

    ''' <summary>
    ''' Evento DataBound para aplicar FuncionesGridWeb
    ''' </summary>
    Protected Sub gridTipos_DataBound(sender As Object, e As EventArgs) Handles gridTipos.DataBound

        Try
            Dim tabla As DataTable = TryCast(gridTipos.DataSource, DataTable)

            If tabla IsNot Nothing Then
                FuncionesGridWeb.SUMColumn(gridTipos, tabla)
            End If

        Catch ex As Exception
            Logger.LogError("TiposSensor.gridTipos_DataBound", ex, "")

        End Try
    End Sub

    Protected Sub btnGuardar_Click(sender As Object, e As EventArgs)

        Try
            Dim id As Integer = Integer.Parse(hfId.Value)
            Dim dto As TipoSensorDTO

            If id = 0 Then
                dto = New TipoSensorDTO()
            Else
                dto = tipoSensorService.ObtenerPorId(id)
                If dto Is Nothing Then
                    MostrarMensaje("Registro no encontrado", "error")
                    Return
                End If
            End If

            dto.Nombre = txtNombre.Text.Trim()
            dto.Descripcion = txtDescripcion.Text.Trim()
            dto.UnidadMedida = txtUnidadMedida.Text.Trim()
            dto.SimboloUnidad = txtSimboloUnidad.Text.Trim()
            dto.UmbralMinimo = If(txtUmbralMinimo.Value IsNot Nothing, CDec(txtUmbralMinimo.Value), Nothing)
            dto.UmbralMaximo = If(txtUmbralMaximo.Value IsNot Nothing, CDec(txtUmbralMaximo.Value), Nothing)
            dto.Activo = chkActivo.Checked

            Dim resultado As Boolean

            If id = 0 Then
                resultado = tipoSensorService.Crear(dto)
                logger.LogInfo($"Tipo de sensor creado: {dto.Nombre}")
            Else
                resultado = tipoSensorService.Actualizar(dto)
                logger.LogInfo($"Tipo de sensor actualizado: {dto.Nombre}")
            End If

            If resultado Then
                MostrarMensaje("Guardado correctamente", "success")
                CargarDatos()
                ScriptManager.RegisterStartupScript(Me, Me.GetType(), "hidePopup", "popupTipo.Hide();", True)
            Else
                MostrarMensaje("Error al guardar", "error")
            End If

        Catch ex As Exception
            logger.LogError("Error al guardar", ex)
            MostrarMensaje("Error al guardar", "error")

        End Try
    End Sub

    Protected Sub cboFiltroActivo_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboFiltroActivo.SelectedIndexChanged
        CargarDatos()
    End Sub

    Protected Sub btnRefrescar_Click(sender As Object, e As EventArgs) Handles btnRefrescar.Click
        CargarDatos()
    End Sub

    Protected Shadows Sub RaisePostBackEvent(eventArgument As String) Implements System.Web.UI.IPostBackEventHandler.RaisePostBackEvent

        Try
            If eventArgument.StartsWith("EDIT:") Then
                Dim id As Integer = Integer.Parse(eventArgument.Replace("EDIT:", ""))

                CargarParaEditar(id)
            ElseIf eventArgument.StartsWith("DELETE:") Then
                Dim id As Integer = Integer.Parse(eventArgument.Replace("DELETE:", ""))

                Eliminar(id)
            End If

        Catch ex As Exception
            logger.LogError("Error en RaisePostBackEvent", ex)
            MostrarMensaje("Error al procesar la solicitud", "error")

        End Try
    End Sub

    Private Sub CargarParaEditar(id As Integer)

        Try
            Dim dto = tipoSensorService.ObtenerPorId(id)

            If dto IsNot Nothing Then
                hfId.Value = dto.Id.ToString()
                txtNombre.Text = dto.Nombre
                txtDescripcion.Text = dto.Descripcion
                txtUnidadMedida.Text = dto.UnidadMedida
                txtSimboloUnidad.Text = dto.SimboloUnidad
                txtUmbralMinimo.Value = dto.UmbralMinimo
                txtUmbralMaximo.Value = dto.UmbralMaximo
                chkActivo.Checked = dto.Activo
                ScriptManager.RegisterStartupScript(Me, Me.GetType(), "showPopup",
                    $"popupTipo.SetHeaderText('Editar: {dto.Nombre}'); popupTipo.Show();", True)
            End If

        Catch ex As Exception
            logger.LogError($"Error al cargar {id}", ex)
            MostrarMensaje("Error al cargar el registro", "error")

        End Try
    End Sub

    Private Sub Eliminar(id As Integer)

        Try
            Dim resultado = tipoSensorService.Eliminar(id)

            If resultado Then
                logger.LogInfo($"Eliminado: {id}")
                MostrarMensaje("Eliminado correctamente", "success")
                CargarDatos()
            Else
                MostrarMensaje("Error al eliminar", "error")
            End If

        Catch ex As Exception
            logger.LogError($"Error al eliminar {id}", ex)
            MostrarMensaje("Error al eliminar", "error")

        End Try
    End Sub

    Private Sub MostrarMensaje(mensaje As String, tipo As String)
        ScriptManager.RegisterStartupScript(Me, Me.GetType(), "showAlert",
            $"CatalogoManager.showAlert('{mensaje}', '{tipo}');", True)
    End Sub

End Class
