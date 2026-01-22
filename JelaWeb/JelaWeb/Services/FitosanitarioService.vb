Imports System.Data
Imports Newtonsoft.Json

''' <summary>
''' Servicio para gestión de Fitosanitarios
''' </summary>
Public Class FitosanitarioService
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
    ''' Obtiene todos los fitosanitarios filtrados por entidad
    ''' </summary>
    Public Function ObtenerTodos(Optional idEntidad As Integer = 0) As DataTable

        Try
            Dim query As String = "SELECT Id, IdEntidad, Nombre, NombreComercial, TipoProducto, Fabricante, IngredienteActivo, Concentracion, DosisRecomendada, UnidadDosis, TiempoCarencia, Toxicidad, Stock, UnidadStock, Activo, FechaCreacion, FechaModificacion FROM cat_fitosanitarios"

            ' Filtrar por entidad si se especifica
            If idEntidad > 0 Then
                query &= $" WHERE IdEntidad = {QueryBuilder.EscapeSqlInteger(idEntidad)}"
            End If

            query &= " ORDER BY Nombre"

            Dim url As String = baseUrl & System.Web.HttpUtility.UrlEncode(query)
            Dim datos = apiConsumer.ObtenerDatos(url)

            Return apiConsumer.ConvertirADatatable(datos)

        Catch ex As Exception
            Logger.LogError("Error al obtener fitosanitarios", ex)
            Throw New ApplicationException("Error al obtener los fitosanitarios desde el API.", ex)

        End Try
    End Function

    ''' <summary>
    ''' Obtiene un fitosanitario por ID
    ''' </summary>
    Public Function ObtenerPorId(id As Integer) As FitosanitarioDTO

        Try
            Dim query As String = $"SELECT Id, Nombre, NombreComercial, TipoProducto, Fabricante, IngredienteActivo, Concentracion, DosisRecomendada, UnidadDosis, TiempoCarencia, Toxicidad, Stock, UnidadStock, Activo, FechaCreacion, FechaModificacion FROM cat_fitosanitarios WHERE Id = {QueryBuilder.EscapeSqlInteger(id)}"
            Dim url As String = baseUrl & System.Web.HttpUtility.UrlEncode(query)
            Dim datos = apiConsumer.ObtenerDatos(url)

            If datos IsNot Nothing AndAlso datos.Count > 0 Then
                Dim campos = datos(0).Campos

                Return New FitosanitarioDTO With {
                    .Id = Convert.ToInt32(campos("Id").Valor),
                    .Nombre = If(campos.ContainsKey("Nombre"), campos("Nombre").Valor?.ToString(), ""),
                    .NombreComercial = If(campos.ContainsKey("NombreComercial"), campos("NombreComercial").Valor?.ToString(), ""),
                    .TipoProducto = If(campos.ContainsKey("TipoProducto"), campos("TipoProducto").Valor?.ToString(), ""),
                    .Fabricante = If(campos.ContainsKey("Fabricante"), campos("Fabricante").Valor?.ToString(), ""),
                    .IngredienteActivo = If(campos.ContainsKey("IngredienteActivo"), campos("IngredienteActivo").Valor?.ToString(), ""),
                    .Concentracion = If(campos.ContainsKey("Concentracion"), campos("Concentracion").Valor?.ToString(), ""),
                    .DosisRecomendada = If(campos.ContainsKey("DosisRecomendada"), campos("DosisRecomendada").Valor?.ToString(), ""),
                    .UnidadDosis = If(campos.ContainsKey("UnidadDosis"), campos("UnidadDosis").Valor?.ToString(), ""),
                    .TiempoCarencia = If(campos.ContainsKey("TiempoCarencia") AndAlso campos("TiempoCarencia").Valor IsNot Nothing, Convert.ToInt32(campos("TiempoCarencia").Valor), Nothing),
                    .Toxicidad = If(campos.ContainsKey("Toxicidad"), campos("Toxicidad").Valor?.ToString(), ""),
                    .Stock = If(campos.ContainsKey("Stock") AndAlso campos("Stock").Valor IsNot Nothing, Convert.ToDecimal(campos("Stock").Valor), 0),
                    .UnidadStock = If(campos.ContainsKey("UnidadStock"), campos("UnidadStock").Valor?.ToString(), ""),
                    .Activo = If(campos.ContainsKey("Activo"), Convert.ToBoolean(campos("Activo").Valor), True),
                    .FechaCreacion = If(campos.ContainsKey("FechaCreacion") AndAlso campos("FechaCreacion").Valor IsNot Nothing, Convert.ToDateTime(campos("FechaCreacion").Valor), DateTime.Now),
                    .FechaModificacion = If(campos.ContainsKey("FechaModificacion") AndAlso campos("FechaModificacion").Valor IsNot Nothing, Convert.ToDateTime(campos("FechaModificacion").Valor), DateTime.Now)
                }
            End If
            Return Nothing

        Catch ex As Exception
            Logger.LogError($"Error al obtener fitosanitario ID: {id}", ex)
            Throw New ApplicationException("Error al obtener el fitosanitario desde el API.", ex)

        End Try
    End Function

    ''' <summary>
    ''' Crea un nuevo fitosanitario
    ''' </summary>
    Public Function Crear(dto As FitosanitarioDTO) As Boolean

        Try
            Dim dynamicDto As New DynamicDto()

            dynamicDto("IdEntidad") = dto.IdEntidad
            dynamicDto("Nombre") = dto.Nombre
            dynamicDto("NombreComercial") = dto.NombreComercial
            dynamicDto("TipoProducto") = dto.TipoProducto
            dynamicDto("Fabricante") = dto.Fabricante
            dynamicDto("IngredienteActivo") = dto.IngredienteActivo
            dynamicDto("Concentracion") = dto.Concentracion
            dynamicDto("DosisRecomendada") = dto.DosisRecomendada
            dynamicDto("UnidadDosis") = dto.UnidadDosis
            If dto.TiempoCarencia.HasValue Then dynamicDto("TiempoCarencia") = dto.TiempoCarencia.Value
            dynamicDto("Toxicidad") = dto.Toxicidad
            dynamicDto("Stock") = dto.Stock
            dynamicDto("UnidadStock") = dto.UnidadStock
            dynamicDto("Activo") = dto.Activo
            dynamicDto("FechaCreacion") = DateTime.Now
            dynamicDto("FechaModificacion") = DateTime.Now

            Dim url As String = apiPostUrl & "cat_fitosanitarios"
            Dim id As Integer = apiConsumerCRUD.EnviarPostId(url, dynamicDto)

            Return id > 0

        Catch ex As Exception
            Logger.LogError("Error al crear fitosanitario", ex)
            Throw New ApplicationException("Error al crear el fitosanitario.", ex)

        End Try
    End Function

    ''' <summary>
    ''' Actualiza un fitosanitario existente
    ''' </summary>
    Public Function Actualizar(dto As FitosanitarioDTO) As Boolean

        Try
            Dim dynamicDto As New DynamicDto()

            dynamicDto("Id") = dto.Id
            dynamicDto("IdEntidad") = dto.IdEntidad
            dynamicDto("Nombre") = dto.Nombre
            dynamicDto("NombreComercial") = dto.NombreComercial
            dynamicDto("TipoProducto") = dto.TipoProducto
            dynamicDto("Fabricante") = dto.Fabricante
            dynamicDto("IngredienteActivo") = dto.IngredienteActivo
            dynamicDto("Concentracion") = dto.Concentracion
            dynamicDto("DosisRecomendada") = dto.DosisRecomendada
            dynamicDto("UnidadDosis") = dto.UnidadDosis
            If dto.TiempoCarencia.HasValue Then dynamicDto("TiempoCarencia") = dto.TiempoCarencia.Value
            dynamicDto("Toxicidad") = dto.Toxicidad
            dynamicDto("Stock") = dto.Stock
            dynamicDto("UnidadStock") = dto.UnidadStock
            dynamicDto("Activo") = dto.Activo
            dynamicDto("FechaModificacion") = DateTime.Now

            Dim url As String = apiPostUrl & "cat_fitosanitarios/" & dto.Id.ToString()

            Return apiConsumerCRUD.EnviarPut(url, dynamicDto)

        Catch ex As Exception
            Logger.LogError($"Error al actualizar fitosanitario ID: {dto.Id}", ex)
            Throw New ApplicationException("Error al actualizar el fitosanitario.", ex)

        End Try
    End Function

    ''' <summary>
    ''' Elimina un fitosanitario
    ''' </summary>
    Public Function Eliminar(id As Integer) As Boolean

        Try
            Dim query As String = $"DELETE FROM cat_fitosanitarios WHERE Id = {QueryBuilder.EscapeSqlInteger(id)}"
            Dim url As String = baseUrl & System.Web.HttpUtility.UrlEncode(query)

            apiConsumer.ObtenerDatos(url)
            Return True

        Catch ex As Exception
            Logger.LogError($"Error al eliminar fitosanitario ID: {id}", ex)
            Throw New ApplicationException("Error al eliminar el fitosanitario.", ex)

        End Try
    End Function
End Class
