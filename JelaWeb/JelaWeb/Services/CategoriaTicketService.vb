Imports System
Imports System.Collections.Generic
Imports System.Net.Http
Imports Newtonsoft.Json

''' <summary>
''' Servicio para gestión de Categorías de Tickets
''' </summary>
Public Class CategoriaTicketService
    Private ReadOnly _apiBaseUrl As String
    Private Const API_BASE_URL As String = "/api/categorias-ticket"
    Private Const API_SLA_URL As String = "/api/ticket-sla"

    Public Sub New()
        _apiBaseUrl = ConfigurationManager.AppSettings("ApiBaseUrl")
    End Sub

    ''' <summary>
    ''' Obtiene todas las categorías con filtros opcionales
    ''' </summary>
    Public Function GetCategorias(Optional activo As Boolean? = Nothing, Optional busqueda As String = Nothing, Optional idEntidad As Integer = 0) As List(Of CategoriaTicketDTO)

        Try
            Dim url As String = API_BASE_URL
            Dim parametros As New List(Of String)

            If activo.HasValue Then
                parametros.Add($"activo={activo.Value}")
            End If

            If Not String.IsNullOrWhiteSpace(busqueda) Then
                parametros.Add($"busqueda={Uri.EscapeDataString(busqueda)}")
            End If

            If idEntidad > 0 Then
                parametros.Add($"idEntidad={idEntidad}")
            End If

            If parametros.Count > 0 Then
                url &= "?" & String.Join("&", parametros)
            End If

            Dim fullUrl = _apiBaseUrl & url
            Dim response = HttpClientHelper.GetAsync(Of List(Of CategoriaTicketDTO))(fullUrl)

            Return If(response, New List(Of CategoriaTicketDTO)())

        Catch ex As Exception
            Logger.LogError("Error al obtener categorías de tickets", ex, "")
            Throw New Exception("Error al obtener las categorías. Por favor, intente nuevamente.", ex)

        End Try
    End Function

    ''' <summary>
    ''' Obtiene una categoría por ID
    ''' </summary>
    Public Function GetCategoriaById(id As Integer) As CategoriaTicketDTO

        Try
            Dim url As String = $"{API_BASE_URL}/{id}"
            Dim fullUrl = _apiBaseUrl & url

            Return HttpClientHelper.GetAsync(Of CategoriaTicketDTO)(fullUrl)

        Catch ex As Exception
            Logger.LogError($"Error al obtener categoría ID: {id}", ex, "")
            Throw New Exception("Error al obtener la categoría. Por favor, intente nuevamente.", ex)

        End Try
    End Function

    ''' <summary>
    ''' Crea una nueva categoría
    ''' </summary>
    Public Function CreateCategoria(categoria As CategoriaTicketDTO) As CategoriaTicketDTO

        Try
            ' Validaciones
            If String.IsNullOrWhiteSpace(categoria.Nombre) Then
                Throw New ArgumentException("El nombre de la categoría es requerido")
            End If

            If categoria.Nombre.Length > 100 Then
                Throw New ArgumentException("El nombre no puede exceder 100 caracteres")
            End If

            Dim fullUrl = _apiBaseUrl & API_BASE_URL
            Dim response = HttpClientHelper.PostAsync(Of CategoriaTicketDTO)(fullUrl, categoria)

            Logger.LogInfo($"Categoría creada: {categoria.Nombre}", "")
            Return response

        Catch ex As ArgumentException

            Throw

        Catch ex As Exception
            Logger.LogError("Error al crear categoría", ex, "")
            Throw New Exception("Error al crear la categoría. Por favor, intente nuevamente.", ex)

        End Try
    End Function

    ''' <summary>
    ''' Actualiza una categoría existente
    ''' </summary>
    Public Function UpdateCategoria(categoria As CategoriaTicketDTO) As CategoriaTicketDTO

        Try
            If categoria.Id <= 0 Then
                Throw New ArgumentException("ID de categoría inválido")
            End If

            If String.IsNullOrWhiteSpace(categoria.Nombre) Then
                Throw New ArgumentException("El nombre de la categoría es requerido")
            End If

            Dim url As String = $"{API_BASE_URL}/{categoria.Id}"
            Dim fullUrl = _apiBaseUrl & url
            Dim response = HttpClientHelper.PutAsync(Of CategoriaTicketDTO)(fullUrl, categoria)

            Logger.LogInfo($"Categoría actualizada: {categoria.Nombre}", "")
            Return response

        Catch ex As ArgumentException

            Throw

        Catch ex As Exception
            Logger.LogError($"Error al actualizar categoría ID: {categoria.Id}", ex, "")
            Throw New Exception("Error al actualizar la categoría. Por favor, intente nuevamente.", ex)

        End Try
    End Function

    ''' <summary>
    ''' Elimina una categoría
    ''' </summary>
    Public Function DeleteCategoria(id As Integer) As Boolean

        Try
            If id <= 0 Then
                Throw New ArgumentException("ID de categoría inválido")
            End If

            Dim url As String = $"{API_BASE_URL}/{id}"
            Dim fullUrl = _apiBaseUrl & url
            Dim response = HttpClientHelper.DeleteAsync(fullUrl)

            Logger.LogInfo($"Categoría eliminada ID: {id}", "")
            Return response

        Catch ex As Exception
            Logger.LogError($"Error al eliminar categoría ID: {id}", ex, "")
            Throw New Exception("Error al eliminar la categoría. Por favor, intente nuevamente.", ex)

        End Try
    End Function

    ''' <summary>
    ''' Obtiene configuraciones SLA de una categoría
    ''' </summary>
    Public Function GetConfiguracionesSLA(categoriaId As Integer) As List(Of ConfiguracionSLADTO)

        Try
            Dim url As String = $"{API_SLA_URL}?categoriaId={categoriaId}"
            Dim fullUrl = _apiBaseUrl & url
            Dim response = HttpClientHelper.GetAsync(Of List(Of ConfiguracionSLADTO))(fullUrl)

            Return If(response, New List(Of ConfiguracionSLADTO)())

        Catch ex As Exception
            Logger.LogError($"Error al obtener SLA de categoría ID: {categoriaId}", ex, "")
            Throw New Exception("Error al obtener configuraciones SLA. Por favor, intente nuevamente.", ex)

        End Try
    End Function

    ''' <summary>
    ''' Guarda configuraciones SLA para una categoría
    ''' </summary>
    Public Function GuardarConfiguracionesSLA(categoriaId As Integer, configuraciones As List(Of ConfiguracionSLADTO)) As Boolean

        Try
            If categoriaId <= 0 Then
                Throw New ArgumentException("ID de categoría inválido")
            End If

            Dim url As String = $"{API_SLA_URL}/categoria/{categoriaId}"
            Dim fullUrl = _apiBaseUrl & url
            Dim response = HttpClientHelper.PostAsync(Of Boolean)(fullUrl, configuraciones)

            Logger.LogInfo($"Configuraciones SLA guardadas para categoría ID: {categoriaId}", "")
            Return response

        Catch ex As ArgumentException

            Throw

        Catch ex As Exception
            Logger.LogError($"Error al guardar SLA de categoría ID: {categoriaId}", ex, "")
            Throw New Exception("Error al guardar configuraciones SLA. Por favor, intente nuevamente.", ex)

        End Try
    End Function

    ''' <summary>
    ''' Alias para compatibilidad con código existente
    ''' </summary>
    Public Function ObtenerTodos(Optional idEntidad As Integer = 0) As List(Of CategoriaTicketDTO)
        Return GetCategorias(Nothing, Nothing, idEntidad)
    End Function

    ''' <summary>
    ''' Alias para compatibilidad con código existente
    ''' </summary>
    Public Function ObtenerPorId(id As Integer) As CategoriaTicketDTO
        Return GetCategoriaById(id)
    End Function

    ''' <summary>
    ''' Alias para compatibilidad con código existente
    ''' </summary>
    Public Function Crear(categoria As CategoriaTicketDTO) As Boolean

        Try
            CreateCategoria(categoria)
            Return True

        Catch
            Return False

        End Try
    End Function

    ''' <summary>
    ''' Alias para compatibilidad con código existente
    ''' </summary>
    Public Function Actualizar(categoria As CategoriaTicketDTO) As Boolean

        Try
            UpdateCategoria(categoria)
            Return True

        Catch
            Return False

        End Try
    End Function

    ''' <summary>
    ''' Alias para compatibilidad con código existente
    ''' </summary>
    Public Function Eliminar(id As Integer) As Boolean
        Return DeleteCategoria(id)
    End Function

    ''' <summary>
    ''' Alias para compatibilidad con código existente
    ''' </summary>
    Public Function ObtenerConfiguracionesSLA(categoriaId As Integer) As List(Of ConfiguracionSLADTO)
        Return GetConfiguracionesSLA(categoriaId)
    End Function
End Class
