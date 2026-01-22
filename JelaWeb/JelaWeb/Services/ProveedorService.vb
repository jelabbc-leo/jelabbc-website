Imports System
Imports System.Collections.Generic

''' <summary>
''' Servicio para gestión de Proveedores
''' </summary>
Public Class ProveedorService
    Private ReadOnly _apiBaseUrl As String
    Private Const API_BASE_URL As String = "/api/proveedores"

    Public Sub New()
        _apiBaseUrl = ConfigurationManager.AppSettings("ApiBaseUrl")
    End Sub

    Public Function GetProveedores(Optional activo As Boolean? = Nothing, Optional busqueda As String = Nothing, Optional idEntidad As Integer = 0) As List(Of ProveedorDTO)

        Try
            Dim url As String = API_BASE_URL
            Dim parametros As New List(Of String)

            If activo.HasValue Then parametros.Add($"activo={activo.Value}")
            If Not String.IsNullOrWhiteSpace(busqueda) Then parametros.Add($"busqueda={Uri.EscapeDataString(busqueda)}")
            If idEntidad > 0 Then parametros.Add($"idEntidad={idEntidad}")
            If parametros.Count > 0 Then url &= "?" & String.Join("&", parametros)

            Dim fullUrl = _apiBaseUrl & url

            Return If(HttpClientHelper.GetAsync(Of List(Of ProveedorDTO))(fullUrl), New List(Of ProveedorDTO)())

        Catch ex As Exception
            Logger.LogError("Error al obtener proveedores", ex, "")
            Throw New Exception("Error al obtener proveedores.", ex)

        End Try
    End Function

    Public Function GetProveedorById(id As Integer) As ProveedorDTO

        Try
            Dim fullUrl = _apiBaseUrl & $"{API_BASE_URL}/{id}"

            Return HttpClientHelper.GetAsync(Of ProveedorDTO)(fullUrl)

        Catch ex As Exception
            Logger.LogError($"Error al obtener proveedor ID: {id}", ex, "")
            Throw New Exception("Error al obtener el proveedor.", ex)

        End Try
    End Function

    Public Function CreateProveedor(proveedor As ProveedorDTO) As ProveedorDTO

        Try
            ValidarProveedor(proveedor)
            Dim fullUrl = _apiBaseUrl & API_BASE_URL
            Dim response = HttpClientHelper.PostAsync(Of ProveedorDTO)(fullUrl, proveedor)

            Logger.LogInfo($"Proveedor creado: {proveedor.RazonSocial}", "")
            Return response

        Catch ex As ArgumentException

            Throw

        Catch ex As Exception
            Logger.LogError("Error al crear proveedor", ex, "")
            Throw New Exception("Error al crear el proveedor.", ex)

        End Try
    End Function

    Public Function UpdateProveedor(proveedor As ProveedorDTO) As ProveedorDTO

        Try
            If proveedor.Id <= 0 Then Throw New ArgumentException("ID inválido")
            ValidarProveedor(proveedor)
            Dim fullUrl = _apiBaseUrl & $"{API_BASE_URL}/{proveedor.Id}"
            Dim response = HttpClientHelper.PutAsync(Of ProveedorDTO)(fullUrl, proveedor)

            Logger.LogInfo($"Proveedor actualizado: {proveedor.RazonSocial}", "")
            Return response

        Catch ex As ArgumentException

            Throw

        Catch ex As Exception
            Logger.LogError($"Error al actualizar proveedor ID: {proveedor.Id}", ex, "")
            Throw New Exception("Error al actualizar el proveedor.", ex)

        End Try
    End Function

    Public Function DeleteProveedor(id As Integer) As Boolean

        Try
            If id <= 0 Then Throw New ArgumentException("ID inválido")
            Dim fullUrl = _apiBaseUrl & $"{API_BASE_URL}/{id}"
            Dim response = HttpClientHelper.DeleteAsync(fullUrl)

            Logger.LogInfo($"Proveedor eliminado ID: {id}", "")
            Return response

        Catch ex As Exception
            Logger.LogError($"Error al eliminar proveedor ID: {id}", ex, "")
            Throw New Exception("Error al eliminar el proveedor.", ex)

        End Try
    End Function

    Private Sub ValidarProveedor(proveedor As ProveedorDTO)
        If String.IsNullOrWhiteSpace(proveedor.RazonSocial) Then
            Throw New ArgumentException("La razón social es requerida")
        End If
        If proveedor.RazonSocial.Length > 200 Then
            Throw New ArgumentException("La razón social no puede exceder 200 caracteres")
        End If
        If Not String.IsNullOrWhiteSpace(proveedor.RFC) AndAlso proveedor.RFC.Length > 13 Then
            Throw New ArgumentException("RFC inválido")
        End If
        If Not String.IsNullOrWhiteSpace(proveedor.Email) AndAlso Not proveedor.Email.Contains("@") Then
            Throw New ArgumentException("Email inválido")
        End If
    End Sub

    ''' <summary>
    ''' Alias para compatibilidad con código existente
    ''' </summary>
    Public Function ObtenerTodos(Optional idEntidad As Integer = 0) As List(Of ProveedorDTO)
        Return GetProveedores(Nothing, Nothing, idEntidad)
    End Function

    ''' <summary>
    ''' Alias para compatibilidad con código existente
    ''' </summary>
    Public Function ObtenerPorId(id As Integer) As ProveedorDTO
        Return GetProveedorById(id)
    End Function

    ''' <summary>
    ''' Alias para compatibilidad con código existente
    ''' </summary>
    Public Function Crear(proveedor As ProveedorDTO) As Boolean

        Try
            CreateProveedor(proveedor)
            Return True

        Catch
            Return False

        End Try
    End Function

    ''' <summary>
    ''' Alias para compatibilidad con código existente
    ''' </summary>
    Public Function Actualizar(proveedor As ProveedorDTO) As Boolean

        Try
            UpdateProveedor(proveedor)
            Return True

        Catch
            Return False

        End Try
    End Function

    ''' <summary>
    ''' Alias para compatibilidad con código existente
    ''' </summary>
    Public Function Eliminar(id As Integer) As Boolean
        Return DeleteProveedor(id)
    End Function
End Class
