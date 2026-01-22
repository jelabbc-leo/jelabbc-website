Imports System.Linq
Imports System.Data

Public Class Fitosanitarios
    Inherits BasePage
    Implements System.Web.UI.IPostBackEventHandler
    Private fitosanitarioService As New FitosanitarioService()

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load

        Try
            If Not IsPostBack Then
                CargarDatos()
            End If

        Catch ex As Exception
            Logger.LogError("Error en Page_Load", ex, "")
            MostrarMensaje("Error al cargar la página", "error")

        End Try
    End Sub

    Private Sub CargarDatos()

        Try
            Dim filtroActivo As String = cboFiltroActivo.Value?.ToString()
            Dim filtroTipo As String = cboFiltroTipo.Value?.ToString()
            Dim busqueda As String = txtBuscar.Text
            Dim datosTable = fitosanitarioService.ObtenerTodos(SessionHelper.GetIdEntidadActual().GetValueOrDefault(1))
            Dim datos = datosTable.AsEnumerable().Select(Function(row) New With {
                .Id = Convert.ToInt32(row("Id")),
                .Nombre = row("Nombre").ToString(),
                .NombreComercial = If(IsDBNull(row("NombreComercial")), "", row("NombreComercial").ToString()),
                .TipoProducto = row("TipoProducto").ToString(),
                .Fabricante = If(IsDBNull(row("Fabricante")), "", row("Fabricante").ToString()),
                .IngredienteActivo = If(IsDBNull(row("IngredienteActivo")), "", row("IngredienteActivo").ToString()),
                .Concentracion = If(IsDBNull(row("Concentracion")), "", row("Concentracion").ToString()),
                .DosisRecomendada = If(IsDBNull(row("DosisRecomendada")), "", row("DosisRecomendada").ToString()),
                .UnidadDosis = If(IsDBNull(row("UnidadDosis")), "", row("UnidadDosis").ToString()),
                .TiempoCarencia = If(IsDBNull(row("TiempoCarencia")), Nothing, CType(row("TiempoCarencia"), Integer?)),
                .Toxicidad = If(IsDBNull(row("Toxicidad")), "", row("Toxicidad").ToString()),
                .Stock = If(IsDBNull(row("Stock")), 0, Convert.ToDecimal(row("Stock"))),
                .UnidadStock = If(IsDBNull(row("UnidadStock")), "", row("UnidadStock").ToString()),
                .Activo = Convert.ToBoolean(row("Activo")),
                .FechaCreacion = Convert.ToDateTime(row("FechaCreacion")),
                .FechaModificacion = Convert.ToDateTime(row("FechaModificacion"))
            }).ToList()

            If Not String.IsNullOrEmpty(filtroActivo) Then
                Dim activo As Boolean = Boolean.Parse(filtroActivo)

                datos = datos.Where(Function(d) d.Activo = activo).ToList()
            End If

            If Not String.IsNullOrEmpty(filtroTipo) Then
                datos = datos.Where(Function(d) d.TipoProducto = filtroTipo).ToList()
            End If

            If Not String.IsNullOrEmpty(busqueda) Then
                datos = datos.Where(Function(d) d.Nombre.ToLower().Contains(busqueda.ToLower()) OrElse (d.NombreComercial IsNot Nothing AndAlso d.NombreComercial.ToLower().Contains(busqueda.ToLower()))).ToList()
            End If

            gridFito.DataSource = datos
            gridFito.DataBind()

        Catch ex As Exception
            logger.LogError("Error al cargar datos", ex)
            MostrarMensaje("Error al cargar los datos", "error")

        End Try
    End Sub

    ''' <summary>
    ''' Evento DataBound para aplicar FuncionesGridWeb
    ''' </summary>
    Protected Sub gridFito_DataBound(sender As Object, e As EventArgs) Handles gridFito.DataBound

        Try
            Dim tabla As DataTable = TryCast(gridFito.DataSource, DataTable)

            If tabla IsNot Nothing Then
                FuncionesGridWeb.SUMColumn(gridFito, tabla)
            End If

        Catch ex As Exception
            Logger.LogError("Fitosanitarios.gridFito_DataBound", ex, "")

        End Try
    End Sub

    Protected Sub btnGuardar_Click(sender As Object, e As EventArgs)

        Try
            Dim id As Integer = Integer.Parse(hfId.Value)
            Dim dto As FitosanitarioDTO = If(id = 0, New FitosanitarioDTO(), fitosanitarioService.ObtenerPorId(id))

            If dto Is Nothing Then
                MostrarMensaje("Registro no encontrado", "error")
                Return
            End If

            dto.Nombre = txtNombre.Text.Trim()
            dto.NombreComercial = txtNombreComercial.Text.Trim()
            dto.TipoProducto = cboTipoProducto.Value?.ToString()
            dto.Fabricante = txtFabricante.Text.Trim()
            dto.IngredienteActivo = txtIngredienteActivo.Text.Trim()
            dto.Concentracion = txtConcentracion.Text.Trim()
            dto.DosisRecomendada = txtDosisRecomendada.Text.Trim()
            dto.UnidadDosis = txtUnidadDosis.Text.Trim()
            dto.TiempoCarencia = If(txtTiempoCarencia.Value IsNot Nothing, CInt(txtTiempoCarencia.Value), Nothing)
            dto.Toxicidad = cboToxicidad.Value?.ToString()
            dto.Stock = If(txtStock.Value IsNot Nothing, CDec(txtStock.Value), 0)
            dto.UnidadStock = cboUnidadStock.Value?.ToString()
            dto.Activo = chkActivo.Checked

            Dim resultado As Boolean = If(id = 0, fitosanitarioService.Crear(dto), fitosanitarioService.Actualizar(dto))

            If resultado Then
                MostrarMensaje("Guardado correctamente", "success")
                CargarDatos()
                ScriptManager.RegisterStartupScript(Me, Me.GetType(), "hidePopup", "popupFito.Hide();", True)
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

    Protected Sub cboFiltroTipo_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboFiltroTipo.SelectedIndexChanged
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
            Dim dto = fitosanitarioService.ObtenerPorId(id)

            If dto IsNot Nothing Then
                hfId.Value = dto.Id.ToString()
                txtNombre.Text = dto.Nombre
                txtNombreComercial.Text = dto.NombreComercial
                cboTipoProducto.Value = dto.TipoProducto
                txtFabricante.Text = dto.Fabricante
                txtIngredienteActivo.Text = dto.IngredienteActivo
                txtConcentracion.Text = dto.Concentracion
                txtDosisRecomendada.Text = dto.DosisRecomendada
                txtUnidadDosis.Text = dto.UnidadDosis
                txtTiempoCarencia.Value = dto.TiempoCarencia
                cboToxicidad.Value = dto.Toxicidad
                txtStock.Value = dto.Stock
                cboUnidadStock.Value = dto.UnidadStock
                chkActivo.Checked = dto.Activo
                ScriptManager.RegisterStartupScript(Me, Me.GetType(), "showPopup", $"popupFito.SetHeaderText('Editar: {dto.Nombre}'); popupFito.Show();", True)
            End If

        Catch ex As Exception
            logger.LogError($"Error al cargar {id}", ex)

        End Try
    End Sub

    Private Sub Eliminar(id As Integer)

        Try
            If fitosanitarioService.Eliminar(id) Then
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
