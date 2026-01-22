Imports System.Data
Imports Newtonsoft.Json

''' <summary>
''' Servicio para gestión de Tipos de Sensor
''' </summary>
Public Class TipoSensorService
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
    ''' Obtiene todos los tipos de sensor filtrados por entidad
    ''' </summary>
    Public Function ObtenerTodos(Optional idEntidad As Integer = 0) As DataTable

        Try
            Dim query As String = "SELECT Id, IdEntidad, Nombre, Descripcion, UnidadMedida, SimboloUnidad, UmbralMinimo, UmbralMaximo, Activo, FechaCreacion, FechaModificacion FROM cat_tipos_sensor"

            ' Filtrar por entidad si se especifica
            If idEntidad > 0 Then
                query &= $" WHERE IdEntidad = {QueryBuilder.EscapeSqlInteger(idEntidad)}"
            End If

            query &= " ORDER BY Nombre"

            Dim url As String = baseUrl & System.Web.HttpUtility.UrlEncode(query)
            Dim datos = apiConsumer.ObtenerDatos(url)

            Return apiConsumer.ConvertirADatatable(datos)

        Catch ex As Exception
            Logger.LogError("Error al obtener tipos de sensor", ex)
            Throw New ApplicationException("Error al obtener los tipos de sensor desde el API.", ex)

        End Try
    End Function

    ''' <summary>
    ''' Obtiene un tipo de sensor por ID
    ''' </summary>
    Public Function ObtenerPorId(id As Integer) As TipoSensorDTO

        Try
            Dim query As String = $"SELECT Id, Nombre, Descripcion, UnidadMedida, SimboloUnidad, UmbralMinimo, UmbralMaximo, Activo, FechaCreacion, FechaModificacion FROM cat_tipos_sensor WHERE Id = {QueryBuilder.EscapeSqlInteger(id)}"
            Dim url As String = baseUrl & System.Web.HttpUtility.UrlEncode(query)
            Dim datos = apiConsumer.ObtenerDatos(url)

            If datos IsNot Nothing AndAlso datos.Count > 0 Then
                Dim campos = datos(0).Campos

                Return New TipoSensorDTO With {
                    .Id = Convert.ToInt32(campos("Id").Valor),
                    .Nombre = If(campos.ContainsKey("Nombre"), campos("Nombre").Valor?.ToString(), ""),
                    .Descripcion = If(campos.ContainsKey("Descripcion"), campos("Descripcion").Valor?.ToString(), ""),
                    .UnidadMedida = If(campos.ContainsKey("UnidadMedida"), campos("UnidadMedida").Valor?.ToString(), ""),
                    .SimboloUnidad = If(campos.ContainsKey("SimboloUnidad"), campos("SimboloUnidad").Valor?.ToString(), ""),
                    .UmbralMinimo = If(campos.ContainsKey("UmbralMinimo") AndAlso campos("UmbralMinimo").Valor IsNot Nothing, Convert.ToDecimal(campos("UmbralMinimo").Valor), Nothing),
                    .UmbralMaximo = If(campos.ContainsKey("UmbralMaximo") AndAlso campos("UmbralMaximo").Valor IsNot Nothing, Convert.ToDecimal(campos("UmbralMaximo").Valor), Nothing),
                    .Activo = If(campos.ContainsKey("Activo"), Convert.ToBoolean(campos("Activo").Valor), True),
                    .FechaCreacion = If(campos.ContainsKey("FechaCreacion") AndAlso campos("FechaCreacion").Valor IsNot Nothing, Convert.ToDateTime(campos("FechaCreacion").Valor), DateTime.Now),
                    .FechaModificacion = If(campos.ContainsKey("FechaModificacion") AndAlso campos("FechaModificacion").Valor IsNot Nothing, Convert.ToDateTime(campos("FechaModificacion").Valor), DateTime.Now)
                }
            End If
            Return Nothing

        Catch ex As Exception
            Logger.LogError($"Error al obtener tipo de sensor ID: {id}", ex)
            Throw New ApplicationException("Error al obtener el tipo de sensor desde el API.", ex)

        End Try
    End Function

    ''' <summary>
    ''' Crea un nuevo tipo de sensor
    ''' </summary>
    Public Function Crear(dto As TipoSensorDTO) As Boolean

        Try
            Dim dynamicDto As New DynamicDto()

            dynamicDto("IdEntidad") = dto.IdEntidad
            dynamicDto("Nombre") = dto.Nombre
            dynamicDto("Descripcion") = dto.Descripcion
            dynamicDto("UnidadMedida") = dto.UnidadMedida
            dynamicDto("SimboloUnidad") = dto.SimboloUnidad
            If dto.UmbralMinimo.HasValue Then dynamicDto("UmbralMinimo") = dto.UmbralMinimo.Value
            If dto.UmbralMaximo.HasValue Then dynamicDto("UmbralMaximo") = dto.UmbralMaximo.Value
            dynamicDto("Activo") = dto.Activo
            dynamicDto("FechaCreacion") = DateTime.Now
            dynamicDto("FechaModificacion") = DateTime.Now

            Dim url As String = apiPostUrl & "cat_tipos_sensor"
            Dim id As Integer = apiConsumerCRUD.EnviarPostId(url, dynamicDto)

            Return id > 0

        Catch ex As Exception
            Logger.LogError("Error al crear tipo de sensor", ex)
            Throw New ApplicationException("Error al crear el tipo de sensor.", ex)

        End Try
    End Function

    ''' <summary>
    ''' Actualiza un tipo de sensor existente
    ''' </summary>
    Public Function Actualizar(dto As TipoSensorDTO) As Boolean

        Try
            Dim dynamicDto As New DynamicDto()

            dynamicDto("Id") = dto.Id
            dynamicDto("IdEntidad") = dto.IdEntidad
            dynamicDto("Nombre") = dto.Nombre
            dynamicDto("Descripcion") = dto.Descripcion
            dynamicDto("UnidadMedida") = dto.UnidadMedida
            dynamicDto("SimboloUnidad") = dto.SimboloUnidad
            If dto.UmbralMinimo.HasValue Then dynamicDto("UmbralMinimo") = dto.UmbralMinimo.Value
            If dto.UmbralMaximo.HasValue Then dynamicDto("UmbralMaximo") = dto.UmbralMaximo.Value
            dynamicDto("Activo") = dto.Activo
            dynamicDto("FechaModificacion") = DateTime.Now

            Dim url As String = apiPostUrl & "cat_tipos_sensor/" & dto.Id.ToString()

            Return apiConsumerCRUD.EnviarPut(url, dynamicDto)

        Catch ex As Exception
            Logger.LogError($"Error al actualizar tipo de sensor ID: {dto.Id}", ex)
            Throw New ApplicationException("Error al actualizar el tipo de sensor.", ex)

        End Try
    End Function

    ''' <summary>
    ''' Elimina un tipo de sensor
    ''' </summary>
    Public Function Eliminar(id As Integer) As Boolean

        Try
            Dim query As String = $"DELETE FROM cat_tipos_sensor WHERE Id = {QueryBuilder.EscapeSqlInteger(id)}"
            Dim url As String = baseUrl & System.Web.HttpUtility.UrlEncode(query)

            apiConsumer.ObtenerDatos(url)
            Return True

        Catch ex As Exception
            Logger.LogError($"Error al eliminar tipo de sensor ID: {id}", ex)
            Throw New ApplicationException("Error al eliminar el tipo de sensor.", ex)

        End Try
    End Function
End Class
