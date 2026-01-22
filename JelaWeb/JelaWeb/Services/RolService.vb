Imports System
Imports System.Collections.Generic
Imports System.Net.Http
Imports Newtonsoft.Json

''' <summary>
''' Servicio para gestión de Roles
''' Arquitectura API-First: Toda la lógica está en el backend API
''' </summary>
Public Class RolService
    Private ReadOnly _apiBaseUrl As String
    Private Const API_BASE_URL As String = "/api/roles"
    Private Const API_PERMISOS_URL As String = "/api/permisos"

    Public Sub New()
        _apiBaseUrl = ConfigurationManager.AppSettings("ApiBaseUrl")
    End Sub

    ''' <summary>
    ''' Obtiene todos los roles con filtros opcionales
    ''' </summary>
    Public Function GetRoles(Optional activo As Boolean? = Nothing, Optional busqueda As String = Nothing) As List(Of RolDTO)

        Try
            Dim url As String = API_BASE_URL
            Dim parametros As New List(Of String)

            If activo.HasValue Then
                parametros.Add($"activo={activo.Value}")
            End If

            If Not String.IsNullOrWhiteSpace(busqueda) Then
                parametros.Add($"busqueda={Uri.EscapeDataString(busqueda)}")
            End If

            If parametros.Count > 0 Then
                url &= "?" & String.Join("&", parametros)
            End If

            Dim fullUrl = _apiBaseUrl & url
            Dim response = HttpClientHelper.GetAsync(Of List(Of RolDTO))(fullUrl)

            Return If(response, New List(Of RolDTO)())

        Catch ex As Exception
            Logger.LogError("Error al obtener roles", ex, "")
            Throw New Exception("Error al obtener la lista de roles. Por favor, intente nuevamente.", ex)

        End Try
    End Function

    ''' <summary>
    ''' Obtiene un rol por ID
    ''' </summary>
    Public Function GetRolById(id As Integer) As RolDTO

        Try
            Dim url As String = $"{API_BASE_URL}/{id}"
            Dim fullUrl = _apiBaseUrl & url
            Dim response = HttpClientHelper.GetAsync(Of RolDTO)(fullUrl)

            Return response

        Catch ex As Exception
            Logger.LogError($"Error al obtener rol ID: {id}", ex, "")
            Throw New Exception("Error al obtener el rol. Por favor, intente nuevamente.", ex)

        End Try
    End Function

    ''' <summary>
    ''' Crea un nuevo rol
    ''' </summary>
    Public Function CreateRol(rol As RolDTO) As RolDTO

        Try
            ' Validaciones
            If String.IsNullOrWhiteSpace(rol.Nombre) Then
                Throw New ArgumentException("El nombre del rol es requerido")
            End If

            If rol.Nombre.Length > 100 Then
                Throw New ArgumentException("El nombre del rol no puede exceder 100 caracteres")
            End If

            Dim fullUrl = _apiBaseUrl & API_BASE_URL
            Dim response = HttpClientHelper.PostAsync(Of RolDTO)(fullUrl, rol)

            Logger.LogInfo($"Rol creado exitosamente: {rol.Nombre}", "")
            Return response

        Catch ex As ArgumentException

            Throw

        Catch ex As Exception
            Logger.LogError("Error al crear rol", ex, "")
            Throw New Exception("Error al crear el rol. Por favor, intente nuevamente.", ex)

        End Try
    End Function

    ''' <summary>
    ''' Actualiza un rol existente
    ''' </summary>
    Public Function UpdateRol(rol As RolDTO) As RolDTO

        Try
            ' Validaciones
            If rol.Id <= 0 Then
                Throw New ArgumentException("ID de rol inválido")
            End If

            If String.IsNullOrWhiteSpace(rol.Nombre) Then
                Throw New ArgumentException("El nombre del rol es requerido")
            End If

            If rol.Nombre.Length > 100 Then
                Throw New ArgumentException("El nombre del rol no puede exceder 100 caracteres")
            End If

            Dim url As String = $"{API_BASE_URL}/{rol.Id}"
            Dim fullUrl = _apiBaseUrl & url
            Dim response = HttpClientHelper.PutAsync(Of RolDTO)(fullUrl, rol)

            Logger.LogInfo($"Rol actualizado exitosamente: {rol.Nombre}", "")
            Return response

        Catch ex As ArgumentException

            Throw

        Catch ex As Exception
            Logger.LogError($"Error al actualizar rol ID: {rol.Id}", ex, "")
            Throw New Exception("Error al actualizar el rol. Por favor, intente nuevamente.", ex)

        End Try
    End Function

    ''' <summary>
    ''' Elimina un rol (soft delete)
    ''' </summary>
    Public Function DeleteRol(id As Integer) As Boolean

        Try
            If id <= 0 Then
                Throw New ArgumentException("ID de rol inválido")
            End If

            Dim url As String = $"{API_BASE_URL}/{id}"
            Dim fullUrl = _apiBaseUrl & url
            Dim response = HttpClientHelper.DeleteAsync(fullUrl)

            Logger.LogInfo($"Rol eliminado exitosamente ID: {id}", "")
            Return response

        Catch ex As ArgumentException

            Throw

        Catch ex As Exception
            Logger.LogError($"Error al eliminar rol ID: {id}", ex, "")
            Throw New Exception("Error al eliminar el rol. Por favor, intente nuevamente.", ex)

        End Try
    End Function

    ''' <summary>
    ''' Obtiene todos los permisos disponibles agrupados por módulo
    ''' </summary>
    Public Function GetPermisos() As Dictionary(Of String, List(Of PermisoDTO))

        Try
            Dim url As String = API_PERMISOS_URL
            Dim fullUrl = _apiBaseUrl & url
            Dim permisos = HttpClientHelper.GetAsync(Of List(Of PermisoDTO))(fullUrl)

            ' Agrupar por módulo
            Dim permisosPorModulo As New Dictionary(Of String, List(Of PermisoDTO))

            If permisos IsNot Nothing AndAlso permisos.Count > 0 Then

                For Each permiso In permisos
                    If Not permisosPorModulo.ContainsKey(permiso.Modulo) Then
                        permisosPorModulo(permiso.Modulo) = New List(Of PermisoDTO)()
                    End If
                    permisosPorModulo(permiso.Modulo).Add(permiso)

                Next

            End If

            Return permisosPorModulo

        Catch ex As Exception
            Logger.LogError("Error al obtener permisos", ex, "")
            Throw New Exception("Error al obtener la lista de permisos. Por favor, intente nuevamente.", ex)

        End Try
    End Function

    ''' <summary>
    ''' Asigna permisos a un rol
    ''' </summary>
    Public Function AsignarPermisos(rolId As Integer, permisosIds As List(Of Integer)) As Boolean

        Try
            If rolId <= 0 Then
                Throw New ArgumentException("ID de rol inválido")
            End If

            Dim url As String = $"{API_BASE_URL}/{rolId}/permisos"
            Dim data As New With {.PermisosIds = permisosIds}
            Dim fullUrl = _apiBaseUrl & url
            Dim response = HttpClientHelper.PostAsync(Of Boolean)(fullUrl, data)

            Logger.LogInfo($"Permisos asignados exitosamente al rol ID: {rolId}", "")
            Return response

        Catch ex As ArgumentException

            Throw

        Catch ex As Exception
            Logger.LogError($"Error al asignar permisos al rol ID: {rolId}", ex, "")
            Throw New Exception("Error al asignar permisos. Por favor, intente nuevamente.", ex)

        End Try
    End Function

    ''' <summary>
    ''' Obtiene los permisos asignados a un rol
    ''' </summary>
    Public Function GetPermisosByRol(rolId As Integer) As List(Of PermisoDTO)

        Try
            If rolId <= 0 Then
                Throw New ArgumentException("ID de rol inválido")
            End If

            Dim url As String = $"{API_BASE_URL}/{rolId}/permisos"
            Dim fullUrl = _apiBaseUrl & url
            Dim response = HttpClientHelper.GetAsync(Of List(Of PermisoDTO))(fullUrl)

            Return If(response, New List(Of PermisoDTO)())

        Catch ex As ArgumentException

            Throw

        Catch ex As Exception
            Logger.LogError($"Error al obtener permisos del rol ID: {rolId}", ex, "")
            Throw New Exception("Error al obtener permisos del rol. Por favor, intente nuevamente.", ex)

        End Try
    End Function

    ''' <summary>
    ''' Valida si un nombre de rol ya existe
    ''' </summary>
    Public Function ExisteNombreRol(nombre As String, Optional idExcluir As Integer = 0) As Boolean

        Try
            Dim roles = GetRoles(activo:=True)

            Return roles.Any(Function(r) r.Nombre.Equals(nombre, StringComparison.OrdinalIgnoreCase) AndAlso r.Id <> idExcluir)

        Catch ex As Exception
            Logger.LogError("Error al validar nombre de rol", ex, "")
            Return False

        End Try
    End Function
End Class
