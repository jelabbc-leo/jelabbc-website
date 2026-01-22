Imports System
Imports System.Collections.Generic
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports Newtonsoft.Json

''' <summary>
''' Página de gestión de Roles del sistema
''' Permite CRUD completo y asignación de permisos
''' </summary>
Partial Public Class Roles
    Inherits BasePage
    Implements ICallbackEventHandler

    Private _rolService As RolService
    Private _callbackResult As String = String.Empty

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load

        Try
            ' Inicializar servicio
            _rolService = New RolService()

            If Not IsPostBack Then
                ' Verificar permisos del usuario
                If Not ValidarPermisos() Then
                    Response.Redirect(Constants.GetErrorUrl("403"), True)
                    Return
                End If

                ' Cargar datos iniciales
                CargarRoles()
            End If

        Catch ex As Exception
            Logger.LogError("Error en Page_Load de Roles", ex)
            MostrarMensaje("Error al cargar la página", "error")

        End Try
    End Sub

    ''' <summary>
    ''' Valida que el usuario tenga permisos para gestionar roles
    ''' </summary>
    Private Function ValidarPermisos() As Boolean

        Try
            Dim userId = SessionHelper.GetUserId()

            If Not userId.HasValue Then Return False

            ' Verificar permiso de gestión de roles
            Return AuthorizationHelper.TienePermiso(userId, "ROLES_GESTIONAR")

        Catch ex As Exception
            Logger.LogError("Error al validar permisos", ex)
            Return False

        End Try
    End Function

    ''' <summary>
    ''' Carga la lista de roles en el grid
    ''' </summary>
    Private Sub CargarRoles()

        Try
            Dim activo As Boolean? = Nothing

            ' Aplicar filtro de estado
            If Not String.IsNullOrEmpty(cboFiltroActivo.Value?.ToString()) Then
                activo = Boolean.Parse(cboFiltroActivo.Value.ToString())
            End If

            ' Aplicar filtro de búsqueda
            Dim busqueda As String = If(String.IsNullOrWhiteSpace(txtBuscar.Text), Nothing, txtBuscar.Text.Trim())

            ' Obtener roles del servicio
            Dim roles = _rolService.GetRoles(activo, busqueda)

            ' Bind al grid
            gridRoles.DataSource = roles
            gridRoles.DataBind()

        Catch ex As Exception
            Logger.LogError("Error al cargar roles", ex)
            MostrarMensaje("Error al cargar los roles", "error")

        End Try
    End Sub

    ''' <summary>
    ''' Evento DataBound para aplicar FuncionesGridWeb
    ''' </summary>
    Protected Sub gridRoles_DataBound(sender As Object, e As EventArgs) Handles gridRoles.DataBound

        Try
            Dim tabla As DataTable = TryCast(gridRoles.DataSource, DataTable)

            If tabla IsNot Nothing Then
                FuncionesGridWeb.SUMColumn(gridRoles, tabla)
            End If

        Catch ex As Exception
            Logger.LogError("Roles.gridRoles_DataBound", ex)

        End Try
    End Sub

    ''' <summary>
    ''' Evento click del botón Guardar
    ''' </summary>
    Protected Sub btnGuardar_Click(sender As Object, e As EventArgs)

        Try
            ' Validar campos requeridos
            If String.IsNullOrWhiteSpace(txtNombre.Text) Then
                MostrarMensaje("El nombre del rol es requerido", "warning")
                Return
            End If

            ' Crear DTO
            Dim rol As New RolDTO With {
                .Id = Integer.Parse(hfRolId.Value),
                .Nombre = txtNombre.Text.Trim(),
                .Descripcion = txtDescripcion.Text?.Trim(),
                .Activo = chkActivo.Checked
            }

            ' Validar nombre duplicado
            If _rolService.ExisteNombreRol(rol.Nombre, rol.Id) Then
                MostrarMensaje("Ya existe un rol con ese nombre", "warning")
                Return
            End If

            ' Guardar (crear o actualizar)
            If rol.Id = 0 Then
                ' Crear nuevo
                _rolService.CreateRol(rol)
                MostrarMensaje("Rol creado exitosamente", "success")
                Logger.LogInfo($"Rol creado: {rol.Nombre} por usuario {SessionHelper.GetNombre()}")
            Else
                ' Actualizar existente
                _rolService.UpdateRol(rol)
                MostrarMensaje("Rol actualizado exitosamente", "success")
                Logger.LogInfo($"Rol actualizado: {rol.Nombre} por usuario {SessionHelper.GetNombre()}")
            End If

            ' Recargar grid
            CargarRoles()

            ' Cerrar popup (desde cliente)
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "closePopup", "popupRol.Hide();", True)

        Catch ex As ArgumentException

            MostrarMensaje(ex.Message, "warning")

        Catch ex As Exception
            Logger.LogError("Error al guardar rol", ex)
            MostrarMensaje("Error al guardar el rol", "error")

        End Try
    End Sub

    ''' <summary>
    ''' Evento click del botón Refrescar
    ''' </summary>
    Protected Sub btnRefrescar_Click(sender As Object, e As EventArgs) Handles btnRefrescar.Click

        Try
            CargarRoles()
            MostrarMensaje("Datos actualizados", "info")

        Catch ex As Exception
            Logger.LogError("Error al refrescar", ex)
            MostrarMensaje("Error al actualizar los datos", "error")

        End Try
    End Sub

    ''' <summary>
    ''' Evento change del filtro de estado
    ''' </summary>
    Protected Sub cboFiltroActivo_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboFiltroActivo.SelectedIndexChanged

        Try
            CargarRoles()

        Catch ex As Exception
            Logger.LogError("Error al filtrar", ex)
            MostrarMensaje("Error al aplicar filtro", "error")

        End Try
    End Sub

    ''' <summary>
    ''' Maneja las llamadas de callback desde JavaScript
    ''' </summary>
    Public Sub RaiseCallbackEvent(eventArgument As String) Implements ICallbackEventHandler.RaiseCallbackEvent

        Try
            If eventArgument.StartsWith("DELETE:") Then
                ' Eliminar rol
                Dim id As Integer = Integer.Parse(eventArgument.Replace("DELETE:", ""))

                _rolService.DeleteRol(id)
                _callbackResult = "SUCCESS"
                Logger.LogInfo($"Rol eliminado ID: {id} por usuario {SessionHelper.GetNombre()}")

            ElseIf eventArgument.StartsWith("LOAD_PERMISOS:") Then
                ' Cargar permisos para asignación
                Dim id As Integer = Integer.Parse(eventArgument.Replace("LOAD_PERMISOS:", ""))

                CargarPermisosParaAsignacion(id)

            ElseIf IsNumeric(eventArgument) Then
                ' Cargar datos de un rol específico
                Dim id As Integer = Integer.Parse(eventArgument)
                Dim rol = _rolService.GetRolById(id)

                _callbackResult = JsonConvert.SerializeObject(rol)
            End If

        Catch ex As Exception
            Logger.LogError("Error en RaiseCallbackEvent", ex)
            _callbackResult = "ERROR"

        End Try
    End Sub

    ''' <summary>
    ''' Retorna el resultado del callback
    ''' </summary>
    Public Function GetCallbackResult() As String Implements ICallbackEventHandler.GetCallbackResult
        Return _callbackResult
    End Function

    ''' <summary>
    ''' Carga los permisos disponibles y los asignados al rol
    ''' </summary>
    Private Sub CargarPermisosParaAsignacion(rolId As Integer)

        Try
            hfRolIdPermisos.Value = rolId.ToString()

            ' Obtener rol
            Dim rol = _rolService.GetRolById(rolId)

            If rol Is Nothing Then
                MostrarMensaje("Rol no encontrado", "error")
                Return
            End If

            ' Obtener todos los permisos agrupados por módulo
            Dim permisosPorModulo = _rolService.GetPermisos()

            ' Obtener permisos ya asignados al rol
            Dim permisosAsignados = _rolService.GetPermisosByRol(rolId)
            Dim idsAsignados = permisosAsignados.Select(Function(p) p.Id).ToList()

            ' Bind al repeater
            rptModulos.DataSource = permisosPorModulo
            rptModulos.DataBind()

            ' Marcar checkboxes de permisos asignados (desde cliente)
            Dim script As String = $"

                document.getElementById('spanNombreRol').innerText = '{rol.Nombre}';
                var idsAsignados = [{String.Join(",", idsAsignados)}];
                idsAsignados.forEach(function(id) {{
                    var chk = document.getElementById('chk_' + id);
                    if (chk) chk.checked = true;
                }});
                popupPermisos.Show();
            "
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "showPermisos", script, True)

        Catch ex As Exception
            Logger.LogError($"Error al cargar permisos para rol ID: {rolId}", ex)
            MostrarMensaje("Error al cargar permisos", "error")

        End Try
    End Sub

    ''' <summary>
    ''' Guarda los permisos asignados al rol
    ''' </summary>
    Protected Sub btnGuardarPermisos_Click(sender As Object, e As EventArgs)

        Try
            Dim rolId As Integer = Integer.Parse(hfRolIdPermisos.Value)

            ' Obtener IDs de permisos seleccionados desde el Request
            Dim permisosIds As New List(Of Integer)

            For Each key As String In Request.Form.AllKeys
                If key.StartsWith("chk_") AndAlso Request.Form(key) = "on" Then
                    Dim permisoId As Integer = Integer.Parse(key.Replace("chk_", ""))

                    permisosIds.Add(permisoId)
                End If

            Next

            ' Guardar permisos
            _rolService.AsignarPermisos(rolId, permisosIds)

            MostrarMensaje($"Permisos asignados exitosamente ({permisosIds.Count} permisos)", "success")
            Logger.LogInfo($"Permisos asignados al rol ID: {rolId} por usuario {SessionHelper.GetNombre()}")

            ' Cerrar popup
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "closePermisosPopup", "popupPermisos.Hide();", True)

        Catch ex As Exception
            Logger.LogError("Error al guardar permisos", ex)
            MostrarMensaje("Error al guardar los permisos", "error")

        End Try
    End Sub

    ''' <summary>
    ''' Muestra un mensaje al usuario
    ''' </summary>
    Private Sub MostrarMensaje(mensaje As String, tipo As String)
        Dim script As String = $"CatalogoManager.showAlert('{mensaje}', '{tipo}');"

        ScriptManager.RegisterStartupScript(Me, Me.GetType(), "showAlert", script, True)
    End Sub

    ''' <summary>
    ''' Maneja eventos de postback personalizados
    ''' </summary>
    Protected Overrides Sub OnLoad(e As EventArgs)
        MyBase.OnLoad(e)

        ' Manejar postbacks personalizados
        If IsPostBack Then
            Dim eventTarget As String = Request.Form("__EVENTTARGET")
            Dim eventArgument As String = Request.Form("__EVENTARGUMENT")

            If Not String.IsNullOrEmpty(eventArgument) Then
                If eventArgument.StartsWith("DELETE:") Then
                    ' Eliminar rol
                    Dim id As Integer = Integer.Parse(eventArgument.Replace("DELETE:", ""))

                    Try
                        _rolService.DeleteRol(id)
                        MostrarMensaje("Rol eliminado exitosamente", "success")
                        CargarRoles()

                    Catch ex As Exception
                        Logger.LogError($"Error al eliminar rol ID: {id}", ex)
                        MostrarMensaje("Error al eliminar el rol", "error")

                    End Try
                ElseIf eventArgument.StartsWith("LOAD_PERMISOS:") Then
                    ' Cargar permisos
                    Dim id As Integer = Integer.Parse(eventArgument.Replace("LOAD_PERMISOS:", ""))

                    CargarPermisosParaAsignacion(id)
                End If
            End If
        End If
    End Sub

End Class
