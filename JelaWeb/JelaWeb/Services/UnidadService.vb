Imports System.Data
Imports Newtonsoft.Json

''' <summary>
''' Servicio para gestión de Unidades
''' </summary>
Public Class UnidadService
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
    ''' Obtiene todas las unidades filtradas por entidad
    ''' </summary>
    Public Function ObtenerTodos(Optional idEntidad As Integer = 0) As DataTable

        Try
            Dim query As String = "SELECT u.id AS Id, u.codigo AS Codigo, u.nombre AS Nombre, u.entidad_id AS EntidadId, e.Alias AS EntidadNombre, u.torre AS Torre, u.edificio AS Edificio, u.piso AS Piso, u.numero AS Numero, u.superficie AS Superficie, u.numero_residentes AS NumeroResidentes, u.activo AS Activo, u.fecha_creacion AS FechaCreacion, u.fecha_modificacion AS FechaModificacion FROM cat_unidades u LEFT JOIN cat_entidades e ON u.entidad_id = e.Id"

            ' Filtrar por entidad si se especifica
            If idEntidad > 0 Then
                query &= $" WHERE u.entidad_id = {QueryBuilder.EscapeSqlInteger(idEntidad)}"
            End If

            query &= " ORDER BY u.torre, u.edificio, u.piso, u.numero"

            Dim url As String = baseUrl & System.Web.HttpUtility.UrlEncode(query)
            Dim datos = apiConsumer.ObtenerDatos(url)

            Return apiConsumer.ConvertirADatatable(datos)

        Catch ex As Exception
            Logger.LogError("Error al obtener unidades", ex)
            Throw New ApplicationException("Error al obtener las unidades desde el API.", ex)

        End Try
    End Function

    ''' <summary>
    ''' Obtiene una unidad por ID
    ''' </summary>
    Public Function ObtenerPorId(id As Integer) As UnidadDTO

        Try
            Dim query As String = $"SELECT Id, Codigo, Nombre, EntidadId, EntidadNombre, Torre, Edificio, Piso, Numero, Superficie, NumeroResidentes, Activo, FechaCreacion, FechaModificacion FROM cat_unidades WHERE Id = {QueryBuilder.EscapeSqlInteger(id)}"
            Dim url As String = baseUrl & System.Web.HttpUtility.UrlEncode(query)
            Dim datos = apiConsumer.ObtenerDatos(url)

            If datos IsNot Nothing AndAlso datos.Count > 0 Then
                Dim campos = datos(0).Campos

                Return New UnidadDTO With {
                    .Id = Convert.ToInt32(campos("Id").Valor),
                    .Codigo = If(campos.ContainsKey("Codigo"), campos("Codigo").Valor?.ToString(), ""),
                    .Nombre = If(campos.ContainsKey("Nombre"), campos("Nombre").Valor?.ToString(), ""),
                    .EntidadId = If(campos.ContainsKey("EntidadId") AndAlso campos("EntidadId").Valor IsNot Nothing, Convert.ToInt32(campos("EntidadId").Valor), 0),
                    .EntidadNombre = If(campos.ContainsKey("EntidadNombre"), campos("EntidadNombre").Valor?.ToString(), ""),
                    .Torre = If(campos.ContainsKey("Torre"), campos("Torre").Valor?.ToString(), ""),
                    .Edificio = If(campos.ContainsKey("Edificio"), campos("Edificio").Valor?.ToString(), ""),
                    .Piso = If(campos.ContainsKey("Piso"), campos("Piso").Valor?.ToString(), ""),
                    .Numero = If(campos.ContainsKey("Numero"), campos("Numero").Valor?.ToString(), ""),
                    .Superficie = If(campos.ContainsKey("Superficie") AndAlso campos("Superficie").Valor IsNot Nothing, Convert.ToDecimal(campos("Superficie").Valor), Nothing),
                    .NumeroResidentes = If(campos.ContainsKey("NumeroResidentes") AndAlso campos("NumeroResidentes").Valor IsNot Nothing, Convert.ToInt32(campos("NumeroResidentes").Valor), 0),
                    .Activo = If(campos.ContainsKey("Activo"), Convert.ToBoolean(campos("Activo").Valor), True),
                    .FechaCreacion = If(campos.ContainsKey("FechaCreacion") AndAlso campos("FechaCreacion").Valor IsNot Nothing, Convert.ToDateTime(campos("FechaCreacion").Valor), DateTime.Now),
                    .FechaModificacion = If(campos.ContainsKey("FechaModificacion") AndAlso campos("FechaModificacion").Valor IsNot Nothing, Convert.ToDateTime(campos("FechaModificacion").Valor), DateTime.Now)
                }
            End If
            Return Nothing

        Catch ex As Exception
            Logger.LogError($"Error al obtener unidad ID: {id}", ex)
            Throw New ApplicationException("Error al obtener la unidad desde el API.", ex)

        End Try
    End Function

    ''' <summary>
    ''' Crea una nueva unidad
    ''' </summary>
    Public Function Crear(dto As UnidadDTO) As Boolean

        Try
            Dim dynamicDto As New DynamicDto()

            dynamicDto("Codigo") = dto.Codigo
            dynamicDto("Nombre") = dto.Nombre
            dynamicDto("EntidadId") = dto.EntidadId
            dynamicDto("Torre") = dto.Torre
            dynamicDto("Edificio") = dto.Edificio
            dynamicDto("Piso") = dto.Piso
            dynamicDto("Numero") = dto.Numero
            If dto.Superficie.HasValue Then dynamicDto("Superficie") = dto.Superficie.Value
            dynamicDto("NumeroResidentes") = dto.NumeroResidentes
            dynamicDto("Activo") = dto.Activo
            dynamicDto("FechaCreacion") = DateTime.Now
            dynamicDto("FechaModificacion") = DateTime.Now

            Dim url As String = apiPostUrl & "cat_unidades"
            Dim id As Integer = apiConsumerCRUD.EnviarPostId(url, dynamicDto)

            Return id > 0

        Catch ex As Exception
            Logger.LogError("Error al crear unidad", ex)
            Throw New ApplicationException("Error al crear la unidad.", ex)

        End Try
    End Function

    ''' <summary>
    ''' Actualiza una unidad existente
    ''' </summary>
    Public Function Actualizar(dto As UnidadDTO) As Boolean

        Try
            Dim dynamicDto As New DynamicDto()

            dynamicDto("Id") = dto.Id
            dynamicDto("Codigo") = dto.Codigo
            dynamicDto("Nombre") = dto.Nombre
            dynamicDto("EntidadId") = dto.EntidadId
            dynamicDto("Torre") = dto.Torre
            dynamicDto("Edificio") = dto.Edificio
            dynamicDto("Piso") = dto.Piso
            dynamicDto("Numero") = dto.Numero
            If dto.Superficie.HasValue Then dynamicDto("Superficie") = dto.Superficie.Value
            dynamicDto("NumeroResidentes") = dto.NumeroResidentes
            dynamicDto("Activo") = dto.Activo
            dynamicDto("FechaModificacion") = DateTime.Now

            Dim url As String = apiPostUrl & "cat_unidades/" & dto.Id.ToString()

            Return apiConsumerCRUD.EnviarPut(url, dynamicDto)

        Catch ex As Exception
            Logger.LogError($"Error al actualizar unidad ID: {dto.Id}", ex)
            Throw New ApplicationException("Error al actualizar la unidad.", ex)

        End Try
    End Function

    ''' <summary>
    ''' Elimina una unidad
    ''' </summary>
    Public Function Eliminar(id As Integer) As Boolean

        Try
            Dim query As String = $"DELETE FROM cat_unidades WHERE Id = {QueryBuilder.EscapeSqlInteger(id)}"
            Dim url As String = baseUrl & System.Web.HttpUtility.UrlEncode(query)

            apiConsumer.ObtenerDatos(url)
            Return True

        Catch ex As Exception
            Logger.LogError($"Error al eliminar unidad ID: {id}", ex)
            Throw New ApplicationException("Error al eliminar la unidad.", ex)

        End Try
    End Function
End Class
