Imports System.Data
Imports Newtonsoft.Json

''' <summary>
''' Servicio para gestión de Parcelas
''' </summary>
Public Class ParcelaService
    Private ReadOnly apiConsumer As ApiConsumer
    Private ReadOnly apiConsumerCRUD As ApiConsumerCRUD
    Private ReadOnly baseUrl As String
    Private ReadOnly apiPostUrl As String

    Public Sub New()
        apiConsumer = New ApiConsumer()
        apiConsumerCRUD = New ApiConsumerCRUD()
        baseUrl = ConfigurationManager.AppSettings("ApiBaseUrl")
        apiPostUrl = ConfigurationManager.AppSettings("APIPost")
    End Sub

    ''' <summary>
    ''' Obtiene todas las parcelas filtradas por entidad
    ''' </summary>
    Public Function ObtenerTodos(Optional idEntidad As Integer = 0) As DataTable

        Try
            Dim query As String = "SELECT p.Id, p.IdEntidad, p.Nombre, p.Descripcion, p.Superficie, p.UnidadSuperficie, p.Latitud, p.Longitud, p.EntidadId, e.Alias AS EntidadNombre, p.Activo, p.FechaCreacion, p.FechaModificacion FROM cat_parcelas p LEFT JOIN cat_entidades e ON p.IdEntidad = e.Id"

            ' Filtrar por entidad si se especifica
            If idEntidad > 0 Then
                query &= $" WHERE p.IdEntidad = {QueryBuilder.EscapeSqlInteger(idEntidad)}"
            End If

            query &= " ORDER BY p.Nombre"

            Dim url As String = baseUrl & System.Web.HttpUtility.UrlEncode(query)
            Dim datos = apiConsumer.ObtenerDatos(url)

            Return apiConsumer.ConvertirADatatable(datos)

        Catch ex As Exception
            Logger.LogError("Error al obtener parcelas", ex)
            Throw New ApplicationException("Error al obtener las parcelas desde el API.", ex)

        End Try
    End Function

    ''' <summary>
    ''' Obtiene una parcela por ID
    ''' </summary>
    Public Function ObtenerPorId(id As Integer) As ParcelaDTO

        Try
            Dim query As String = $"SELECT Id, Nombre, Descripcion, Superficie, UnidadSuperficie, Latitud, Longitud, EntidadId, EntidadNombre, Activo, FechaCreacion, FechaModificacion FROM cat_parcelas WHERE Id = {QueryBuilder.EscapeSqlInteger(id)}"
            Dim url As String = baseUrl & System.Web.HttpUtility.UrlEncode(query)
            Dim datos = apiConsumer.ObtenerDatos(url)

            If datos IsNot Nothing AndAlso datos.Count > 0 Then
                Dim campos = datos(0).Campos

                Return New ParcelaDTO With {
                    .Id = Convert.ToInt32(campos("Id").Valor),
                    .Nombre = If(campos.ContainsKey("Nombre"), campos("Nombre").Valor?.ToString(), ""),
                    .Descripcion = If(campos.ContainsKey("Descripcion"), campos("Descripcion").Valor?.ToString(), ""),
                    .Superficie = If(campos.ContainsKey("Superficie") AndAlso campos("Superficie").Valor IsNot Nothing, Convert.ToDecimal(campos("Superficie").Valor), 0),
                    .UnidadSuperficie = If(campos.ContainsKey("UnidadSuperficie"), campos("UnidadSuperficie").Valor?.ToString(), ""),
                    .Latitud = If(campos.ContainsKey("Latitud") AndAlso campos("Latitud").Valor IsNot Nothing, Convert.ToDecimal(campos("Latitud").Valor), Nothing),
                    .Longitud = If(campos.ContainsKey("Longitud") AndAlso campos("Longitud").Valor IsNot Nothing, Convert.ToDecimal(campos("Longitud").Valor), Nothing),
                    .EntidadId = If(campos.ContainsKey("EntidadId") AndAlso campos("EntidadId").Valor IsNot Nothing, Convert.ToInt32(campos("EntidadId").Valor), 0),
                    .EntidadNombre = If(campos.ContainsKey("EntidadNombre"), campos("EntidadNombre").Valor?.ToString(), ""),
                    .Activo = If(campos.ContainsKey("Activo"), Convert.ToBoolean(campos("Activo").Valor), True),
                    .FechaCreacion = If(campos.ContainsKey("FechaCreacion") AndAlso campos("FechaCreacion").Valor IsNot Nothing, Convert.ToDateTime(campos("FechaCreacion").Valor), DateTime.Now),
                    .FechaModificacion = If(campos.ContainsKey("FechaModificacion") AndAlso campos("FechaModificacion").Valor IsNot Nothing, Convert.ToDateTime(campos("FechaModificacion").Valor), DateTime.Now)
                }
            End If
            Return Nothing

        Catch ex As Exception
            Logger.LogError($"Error al obtener parcela ID: {id}", ex)
            Throw New ApplicationException("Error al obtener la parcela desde el API.", ex)

        End Try
    End Function

    ''' <summary>
    ''' Crea una nueva parcela
    ''' </summary>
    Public Function Crear(dto As ParcelaDTO) As Boolean

        Try
            Dim dynamicDto As New DynamicDto()

            dynamicDto("IdEntidad") = dto.IdEntidad
            dynamicDto("Nombre") = dto.Nombre
            dynamicDto("Descripcion") = dto.Descripcion
            dynamicDto("Superficie") = dto.Superficie
            dynamicDto("UnidadSuperficie") = dto.UnidadSuperficie
            If dto.Latitud.HasValue Then dynamicDto("Latitud") = dto.Latitud.Value
            If dto.Longitud.HasValue Then dynamicDto("Longitud") = dto.Longitud.Value
            dynamicDto("EntidadId") = dto.EntidadId
            dynamicDto("Activo") = dto.Activo
            dynamicDto("FechaCreacion") = DateTime.Now
            dynamicDto("FechaModificacion") = DateTime.Now

            Dim url As String = apiPostUrl & "cat_parcelas"
            Dim id As Integer = apiConsumerCRUD.EnviarPostId(url, dynamicDto)

            Return id > 0

        Catch ex As Exception
            Logger.LogError("Error al crear parcela", ex)
            Throw New ApplicationException("Error al crear la parcela.", ex)

        End Try
    End Function

    ''' <summary>
    ''' Actualiza una parcela existente
    ''' </summary>
    Public Function Actualizar(dto As ParcelaDTO) As Boolean

        Try
            Dim dynamicDto As New DynamicDto()

            dynamicDto("Id") = dto.Id
            dynamicDto("IdEntidad") = dto.IdEntidad
            dynamicDto("Nombre") = dto.Nombre
            dynamicDto("Descripcion") = dto.Descripcion
            dynamicDto("Superficie") = dto.Superficie
            dynamicDto("UnidadSuperficie") = dto.UnidadSuperficie
            If dto.Latitud.HasValue Then dynamicDto("Latitud") = dto.Latitud.Value
            If dto.Longitud.HasValue Then dynamicDto("Longitud") = dto.Longitud.Value
            dynamicDto("EntidadId") = dto.EntidadId
            dynamicDto("Activo") = dto.Activo
            dynamicDto("FechaModificacion") = DateTime.Now

            Dim url As String = apiPostUrl & "cat_parcelas/" & dto.Id.ToString()

            Return apiConsumerCRUD.EnviarPut(url, dynamicDto)

        Catch ex As Exception
            Logger.LogError($"Error al actualizar parcela ID: {dto.Id}", ex)
            Throw New ApplicationException("Error al actualizar la parcela.", ex)

        End Try
    End Function

    ''' <summary>
    ''' Elimina una parcela
    ''' </summary>
    Public Function Eliminar(id As Integer) As Boolean

        Try
            Dim query As String = $"DELETE FROM cat_parcelas WHERE Id = {QueryBuilder.EscapeSqlInteger(id)}"
            Dim url As String = baseUrl & System.Web.HttpUtility.UrlEncode(query)

            apiConsumer.ObtenerDatos(url)
            Return True

        Catch ex As Exception
            Logger.LogError($"Error al eliminar parcela ID: {id}", ex)
            Throw New ApplicationException("Error al eliminar la parcela.", ex)

        End Try
    End Function
End Class
