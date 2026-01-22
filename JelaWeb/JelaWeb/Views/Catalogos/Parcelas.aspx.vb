Imports System.Linq
Imports System.Data

Public Class Parcelas
    Inherits BasePage
    Implements System.Web.UI.IPostBackEventHandler
    Private parcelaService As New ParcelaService()

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
            Dim datosTable = parcelaService.ObtenerTodos(SessionHelper.GetIdEntidadActual().GetValueOrDefault(1))
            Dim datos = datosTable.AsEnumerable().Select(Function(row) New With {
                .Id = Convert.ToInt32(row("Id")),
                .Nombre = row("Nombre").ToString(),
                .Descripcion = If(IsDBNull(row("Descripcion")), "", row("Descripcion").ToString()),
                .Superficie = If(IsDBNull(row("Superficie")), 0, Convert.ToDecimal(row("Superficie"))),
                .UnidadSuperficie = If(IsDBNull(row("UnidadSuperficie")), "", row("UnidadSuperficie").ToString()),
                .Latitud = If(IsDBNull(row("Latitud")), Nothing, CType(row("Latitud"), Decimal?)),
                .Longitud = If(IsDBNull(row("Longitud")), Nothing, CType(row("Longitud"), Decimal?)),
                .EntidadId = If(IsDBNull(row("EntidadId")), 0, Convert.ToInt32(row("EntidadId"))),
                .EntidadNombre = If(IsDBNull(row("EntidadNombre")), "", row("EntidadNombre").ToString()),
                .Activo = Convert.ToBoolean(row("Activo")),
                .FechaCreacion = Convert.ToDateTime(row("FechaCreacion")),
                .FechaModificacion = Convert.ToDateTime(row("FechaModificacion"))
            }).ToList()

            If Not String.IsNullOrEmpty(filtroActivo) Then
                Dim activo As Boolean = Boolean.Parse(filtroActivo)

                datos = datos.Where(Function(d) d.Activo = activo).ToList()
            End If

            If Not String.IsNullOrEmpty(busqueda) Then
                datos = datos.Where(Function(d) d.Nombre.ToLower().Contains(busqueda.ToLower())).ToList()
            End If

            gridParcelas.DataSource = datos
            gridParcelas.DataBind()

        Catch ex As Exception
            Logger.LogError("Error al cargar datos", ex, "")
            MostrarMensaje("Error al cargar los datos", "error")

        End Try
    End Sub

    ''' <summary>
    ''' Evento DataBound para aplicar FuncionesGridWeb
    ''' </summary>
    Protected Sub gridParcelas_DataBound(sender As Object, e As EventArgs) Handles gridParcelas.DataBound

        Try
            Dim tabla As DataTable = TryCast(gridParcelas.DataSource, DataTable)

            If tabla IsNot Nothing Then
                FuncionesGridWeb.SUMColumn(gridParcelas, tabla)
            End If

        Catch ex As Exception
            Logger.LogError("Parcelas.gridParcelas_DataBound", ex, "")

        End Try
    End Sub

    Protected Sub btnGuardar_Click(sender As Object, e As EventArgs)

        Try
            Dim id As Integer = Integer.Parse(hfId.Value)
            Dim dto As ParcelaDTO = If(id = 0, New ParcelaDTO(), parcelaService.ObtenerPorId(id))

            If dto Is Nothing Then
                MostrarMensaje("Registro no encontrado", "error")
                Return
            End If

            dto.Nombre = txtNombre.Text.Trim()
            dto.Descripcion = txtDescripcion.Text.Trim()
            dto.Superficie = CDec(txtSuperficie.Value)
            dto.UnidadSuperficie = cboUnidadSuperficie.Value?.ToString()
            dto.EntidadId = If(cboEntidad.Value IsNot Nothing, CInt(cboEntidad.Value), 0)
            dto.Latitud = If(txtLatitud.Value IsNot Nothing, CDec(txtLatitud.Value), Nothing)
            dto.Longitud = If(txtLongitud.Value IsNot Nothing, CDec(txtLongitud.Value), Nothing)
            dto.Activo = chkActivo.Checked

            Dim resultado As Boolean = If(id = 0, parcelaService.Crear(dto), parcelaService.Actualizar(dto))

            If resultado Then
                MostrarMensaje("Guardado correctamente", "success")
                CargarDatos()
                ScriptManager.RegisterStartupScript(Me, Me.GetType(), "hidePopup", "popupParcela.Hide();", True)
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
                CargarParaEditar(Integer.Parse(eventArgument.Replace("EDIT:", "")))
            ElseIf eventArgument.StartsWith("DELETE:") Then
                Eliminar(Integer.Parse(eventArgument.Replace("DELETE:", "")))
            End If

        Catch ex As Exception
            logger.LogError("Error en RaisePostBackEvent", ex)

        End Try
    End Sub

    Private Sub CargarParaEditar(id As Integer)

        Try
            Dim dto = parcelaService.ObtenerPorId(id)

            If dto IsNot Nothing Then
                hfId.Value = dto.Id.ToString()
                txtNombre.Text = dto.Nombre
                txtDescripcion.Text = dto.Descripcion
                txtSuperficie.Value = dto.Superficie
                cboUnidadSuperficie.Value = dto.UnidadSuperficie
                cboEntidad.Value = dto.EntidadId
                txtLatitud.Value = dto.Latitud
                txtLongitud.Value = dto.Longitud
                chkActivo.Checked = dto.Activo
                ScriptManager.RegisterStartupScript(Me, Me.GetType(), "showPopup", $"popupParcela.SetHeaderText('Editar: {dto.Nombre}'); popupParcela.Show();", True)
            End If

        Catch ex As Exception
            logger.LogError($"Error al cargar {id}", ex)

        End Try
    End Sub

    Private Sub Eliminar(id As Integer)

        Try
            If parcelaService.Eliminar(id) Then
                MostrarMensaje("Eliminado correctamente", "success")
                CargarDatos()
            Else
                MostrarMensaje("Error al eliminar", "error")
            End If

        Catch ex As Exception
            logger.LogError($"Error al eliminar {id}", ex)

        End Try
    End Sub

    Private Sub MostrarMensaje(mensaje As String, tipo As String)
        ScriptManager.RegisterStartupScript(Me, Me.GetType(), "showAlert", $"CatalogoManager.showAlert('{mensaje}', '{tipo}');", True)
    End Sub
End Class
