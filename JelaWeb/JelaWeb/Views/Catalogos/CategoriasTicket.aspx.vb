Imports System.Web.Script.Serialization
Imports System.Linq

Public Class CategoriasTicket
    Inherits BasePage
    Implements System.Web.UI.IPostBackEventHandler

    Private categoriaService As New CategoriaTicketService()

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load

        Try
            If Not IsPostBack Then
                CargarCategorias()
            End If

        Catch ex As Exception
            Logger.LogError("Error en Page_Load de CategoriasTicket", ex, "")
            MostrarMensaje("Error al cargar la página", "error")

        End Try
    End Sub

    Private Sub CargarCategorias()

        Try
            Dim filtroActivo As String = cboFiltroActivo.Value?.ToString()
            Dim busqueda As String = txtBuscar.Text
            Dim categorias = categoriaService.ObtenerTodos(SessionHelper.GetIdEntidadActual().GetValueOrDefault(1))

            ' Aplicar filtros
            If Not String.IsNullOrEmpty(filtroActivo) Then
                Dim activo As Boolean = Boolean.Parse(filtroActivo)

                categorias = categorias.Where(Function(c) c.Activo = activo).ToList()
            End If

            If Not String.IsNullOrEmpty(busqueda) Then
                categorias = categorias.Where(Function(c) c.Nombre.ToLower().Contains(busqueda.ToLower()) OrElse (c.Descripcion IsNot Nothing AndAlso c.Descripcion.ToLower().Contains(busqueda.ToLower()))).ToList()
            End If

            gridCategorias.DataSource = categorias
            gridCategorias.DataBind()

        Catch ex As Exception
            logger.LogError("Error al cargar categorías", ex)
            MostrarMensaje("Error al cargar las categorías", "error")

        End Try
    End Sub

    ''' <summary>
    ''' Evento DataBound para aplicar FuncionesGridWeb
    ''' </summary>
    Protected Sub gridCategorias_DataBound(sender As Object, e As EventArgs) Handles gridCategorias.DataBound

        Try
            Dim tabla As DataTable = TryCast(gridCategorias.DataSource, DataTable)

            If tabla IsNot Nothing Then
                FuncionesGridWeb.SUMColumn(gridCategorias, tabla)
            End If

        Catch ex As Exception
            Logger.LogError("CategoriasTicket.gridCategorias_DataBound", ex, "")

        End Try
    End Sub

    Protected Sub btnGuardar_Click(sender As Object, e As EventArgs)

        Try
            Dim categoriaId As Integer = Integer.Parse(hfCategoriaId.Value)
            Dim categoria As CategoriaTicketDTO

            If categoriaId = 0 Then
                ' Crear nueva
                categoria = New CategoriaTicketDTO()
            Else
                ' Editar existente
                categoria = categoriaService.ObtenerPorId(categoriaId)
                If categoria Is Nothing Then
                    MostrarMensaje("Categoría no encontrada", "error")
                    Return
                End If
            End If

            ' Asignar valores
            categoria.Nombre = txtNombre.Text.Trim()
            categoria.Descripcion = txtDescripcion.Text.Trim()
            categoria.IconoClase = txtIconoClase.Text.Trim()
            categoria.Color = txtColor.Text.Trim()
            categoria.Activo = chkActivo.Checked

            ' Guardar
            Dim resultado As Boolean

            If categoriaId = 0 Then
                resultado = categoriaService.Crear(categoria)
                logger.LogInfo($"Categoría creada: {categoria.Nombre}")
            Else
                resultado = categoriaService.Actualizar(categoria)
                logger.LogInfo($"Categoría actualizada: {categoria.Nombre}")
            End If

            If resultado Then
                MostrarMensaje("Categoría guardada correctamente", "success")
                CargarCategorias()
                ScriptManager.RegisterStartupScript(Me, Me.GetType(), "hidePopup", "popupCategoria.Hide();", True)
            Else
                MostrarMensaje("Error al guardar la categoría", "error")
            End If

        Catch ex As Exception
            logger.LogError("Error al guardar categoría", ex)
            MostrarMensaje("Error al guardar la categoría", "error")

        End Try
    End Sub

    Protected Sub cboFiltroActivo_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboFiltroActivo.SelectedIndexChanged
        CargarCategorias()
    End Sub

    Protected Sub btnRefrescar_Click(sender As Object, e As EventArgs) Handles btnRefrescar.Click
        CargarCategorias()
    End Sub

    ' Manejo de callbacks y postbacks
    Protected Shadows Sub RaisePostBackEvent(eventArgument As String) Implements System.Web.UI.IPostBackEventHandler.RaisePostBackEvent

        Try
            If eventArgument.StartsWith("EDIT:") Then
                Dim id As Integer = Integer.Parse(eventArgument.Replace("EDIT:", ""))

                CargarCategoriaParaEditar(id)
            ElseIf eventArgument.StartsWith("DELETE:") Then
                Dim id As Integer = Integer.Parse(eventArgument.Replace("DELETE:", ""))

                EliminarCategoria(id)
            ElseIf eventArgument.StartsWith("SLA:") Then
                Dim id As Integer = Integer.Parse(eventArgument.Replace("SLA:", ""))

                CargarConfiguracionSLA(id)
            End If

        Catch ex As Exception
            logger.LogError("Error en RaisePostBackEvent", ex)
            MostrarMensaje("Error al procesar la solicitud", "error")

        End Try
    End Sub

    Private Sub CargarCategoriaParaEditar(id As Integer)

        Try
            Dim categoria = categoriaService.ObtenerPorId(id)

            If categoria IsNot Nothing Then
                hfCategoriaId.Value = categoria.Id.ToString()
                txtNombre.Text = categoria.Nombre
                txtDescripcion.Text = categoria.Descripcion
                txtIconoClase.Text = categoria.IconoClase
                txtColor.Text = categoria.Color
                chkActivo.Checked = categoria.Activo

                ScriptManager.RegisterStartupScript(Me, Me.GetType(), "showPopup",
                    $"popupCategoria.SetHeaderText('Editar Categoría: {categoria.Nombre}'); popupCategoria.Show();", True)
            End If

        Catch ex As Exception
            logger.LogError($"Error al cargar categoría {id}", ex)
            MostrarMensaje("Error al cargar la categoría", "error")

        End Try
    End Sub

    Private Sub EliminarCategoria(id As Integer)

        Try
            Dim resultado = categoriaService.Eliminar(id)

            If resultado Then
                logger.LogInfo($"Categoría eliminada: {id}")
                MostrarMensaje("Categoría eliminada correctamente", "success")
                CargarCategorias()
            Else
                MostrarMensaje("Error al eliminar la categoría", "error")
            End If

        Catch ex As Exception
            logger.LogError($"Error al eliminar categoría {id}", ex)
            MostrarMensaje("Error al eliminar la categoría", "error")

        End Try
    End Sub

    Private Sub CargarConfiguracionSLA(categoriaId As Integer)

        Try
            Dim categoria = categoriaService.ObtenerPorId(categoriaId)

            If categoria IsNot Nothing Then
                hfCategoriaIdSLA.Value = categoriaId.ToString()

                ' Cargar configuraciones SLA
                Dim configuraciones = categoriaService.ObtenerConfiguracionesSLA(categoriaId)

                gridSLA.DataSource = configuraciones
                gridSLA.DataBind()

                ScriptManager.RegisterStartupScript(Me, Me.GetType(), "showSLA",
                    $"document.getElementById('spanNombreCategoria').innerText = '{categoria.Nombre}'; popupSLA.Show();", True)
            End If

        Catch ex As Exception
            logger.LogError($"Error al cargar SLA para categoría {categoriaId}", ex)
            MostrarMensaje("Error al cargar la configuración SLA", "error")

        End Try
    End Sub

    Private Sub MostrarMensaje(mensaje As String, tipo As String)
        ScriptManager.RegisterStartupScript(Me, Me.GetType(), "showAlert",
            $"CatalogoManager.showAlert('{mensaje}', '{tipo}');", True)
    End Sub

End Class
