Imports System.Data

''' <summary>
''' Servicio genérico para operaciones CRUD dinámicas sobre cualquier tabla
''' </summary>
Public Class DynamicCrudService
    Private Shared ReadOnly apiConsumer As New ApiConsumer()
    Private Shared ReadOnly apiConsumerCRUD As New ApiConsumerCRUD()
    Private Shared ReadOnly baseUrl As String = ConfigurationManager.AppSettings("ApiBaseUrl")
    Private Shared ReadOnly apiPostUrl As String = ConfigurationManager.AppSettings("APIPost")

    ''' <summary>
    ''' Obtiene todos los registros de una tabla
    ''' NOTA: Automáticamente filtra por IdEntidad si el usuario tiene una entidad seleccionada
    ''' </summary>
    Public Shared Function ObtenerTodos(tableName As String) As DataTable

        Try
            If String.IsNullOrWhiteSpace(tableName) Then
                Throw New ArgumentException("El nombre de la tabla es requerido")
            End If

            Dim query As String = "SELECT * FROM " & SanitizeTableName(tableName)
            
            ' NUEVO: Agregar filtro de entidad automáticamente si existe
            Dim idEntidad = SessionHelper.GetIdEntidadActual()
            If idEntidad.HasValue Then
                query &= " WHERE IdEntidad = " & idEntidad.Value
            End If
            
            Dim url As String = baseUrl & System.Web.HttpUtility.UrlEncode(query)
            Dim datos = apiConsumer.ObtenerDatos(url)

            Return apiConsumer.ConvertirADatatable(datos)

        Catch ex As Exception
            Logger.LogError("DynamicCrudService.ObtenerTodos", ex)
            Throw New ApplicationException($"Error al obtener registros de {tableName}", ex)

        End Try
    End Function

    ''' <summary>
    ''' Obtiene todos los registros de una tabla con filtro WHERE
    ''' NOTA: Automáticamente agrega filtro por IdEntidad si el usuario tiene una entidad seleccionada
    ''' </summary>
    Public Shared Function ObtenerTodosConFiltro(tableName As String, whereClause As String) As DataTable

        Try
            If String.IsNullOrWhiteSpace(tableName) Then
                Throw New ArgumentException("El nombre de la tabla es requerido")
            End If

            Dim query As String = "SELECT * FROM " & SanitizeTableName(tableName)

            ' NUEVO: Agregar filtro de entidad automáticamente
            Dim idEntidad = SessionHelper.GetIdEntidadActual()
            Dim filtroCompleto As String = ""
            
            If idEntidad.HasValue Then
                filtroCompleto = "IdEntidad = " & idEntidad.Value
            End If
            
            If Not String.IsNullOrWhiteSpace(whereClause) Then
                If Not String.IsNullOrWhiteSpace(filtroCompleto) Then
                    filtroCompleto &= " AND " & whereClause
                Else
                    filtroCompleto = whereClause
                End If
            End If
            
            If Not String.IsNullOrWhiteSpace(filtroCompleto) Then
                query &= " WHERE " & filtroCompleto
            End If

            Dim url As String = baseUrl & System.Web.HttpUtility.UrlEncode(query)
            Dim datos = apiConsumer.ObtenerDatos(url)

            Return apiConsumer.ConvertirADatatable(datos)

        Catch ex As Exception
            Logger.LogError("DynamicCrudService.ObtenerTodosConFiltro", ex)
            Throw New ApplicationException($"Error al obtener registros de {tableName}", ex)

        End Try
    End Function

    ''' <summary>
    ''' Obtiene un registro por su ID
    ''' </summary>
    Public Shared Function ObtenerPorId(tableName As String, id As Integer) As DataRow

        Try
            If String.IsNullOrWhiteSpace(tableName) Then
                Throw New ArgumentException("El nombre de la tabla es requerido")
            End If

            Dim query As String = "SELECT * FROM " & SanitizeTableName(tableName) & " WHERE Id = " & QueryBuilder.EscapeSqlInteger(id)
            Dim url As String = baseUrl & System.Web.HttpUtility.UrlEncode(query)
            Dim datos = apiConsumer.ObtenerDatos(url)
            Dim dt = apiConsumer.ConvertirADatatable(datos)

            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                Return dt.Rows(0)
            End If
            Return Nothing

        Catch ex As Exception
            Logger.LogError("DynamicCrudService.ObtenerPorId", ex)
            Throw New ApplicationException($"Error al obtener registro de {tableName} con Id={id}", ex)

        End Try
    End Function

    ''' <summary>
    ''' Obtiene un registro por un campo específico
    ''' </summary>
    Public Shared Function ObtenerPorCampo(tableName As String, fieldName As String, value As Object) As DataRow

        Try
            If String.IsNullOrWhiteSpace(tableName) Then
                Throw New ArgumentException("El nombre de la tabla es requerido")
            End If
            If String.IsNullOrWhiteSpace(fieldName) Then
                Throw New ArgumentException("El nombre del campo es requerido")
            End If

            Dim whereValue As String

            If TypeOf value Is String Then
                whereValue = QueryBuilder.EscapeSqlString(value.ToString())
            ElseIf TypeOf value Is Date Then
                whereValue = QueryBuilder.EscapeSqlDate(CDate(value))
            ElseIf TypeOf value Is Boolean Then
                whereValue = If(CBool(value), "1", "0")
            Else
                whereValue = value.ToString()
            End If

            Dim query As String = "SELECT * FROM " & SanitizeTableName(tableName) & " WHERE " & SanitizeFieldName(fieldName) & " = " & whereValue
            Dim url As String = baseUrl & System.Web.HttpUtility.UrlEncode(query)
            Dim datos = apiConsumer.ObtenerDatos(url)
            Dim dt = apiConsumer.ConvertirADatatable(datos)

            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                Return dt.Rows(0)
            End If
            Return Nothing

        Catch ex As Exception
            Logger.LogError("DynamicCrudService.ObtenerPorCampo", ex)
            Throw New ApplicationException($"Error al obtener registro de {tableName} por {fieldName}", ex)

        End Try
    End Function

    ''' <summary>
    ''' Inserta un nuevo registro
    ''' NOTA: Automáticamente agrega IdEntidad si el usuario tiene una entidad seleccionada
    ''' </summary>
    Public Shared Function Insertar(tableName As String, datos As Dictionary(Of String, Object)) As Boolean

        Try
            If String.IsNullOrWhiteSpace(tableName) Then
                Throw New ArgumentException("El nombre de la tabla es requerido")
            End If
            If datos Is Nothing OrElse datos.Count = 0 Then
                Throw New ArgumentException("Los datos son requeridos")
            End If

            ' NUEVO: Agregar IdEntidad automáticamente si no existe
            Dim idEntidad = SessionHelper.GetIdEntidadActual()
            If idEntidad.HasValue AndAlso Not datos.ContainsKey("IdEntidad") Then
                datos.Add("IdEntidad", idEntidad.Value)
            End If

            Dim dto As New DynamicDto()

            For Each kvp In datos
                dto(kvp.Key) = kvp.Value
            Next

            Dim url As String = apiPostUrl & SanitizeTableName(tableName)

            Return apiConsumerCRUD.EnviarPost(url, dto)

        Catch ex As Exception
            Logger.LogError("DynamicCrudService.Insertar", ex)
            Throw New ApplicationException($"Error al insertar en {tableName}", ex)

        End Try
    End Function

    ''' <summary>
    ''' Inserta un nuevo registro y retorna el ID generado
    ''' NOTA: Automáticamente agrega IdEntidad si el usuario tiene una entidad seleccionada
    ''' </summary>
    Public Shared Function InsertarConId(tableName As String, datos As Dictionary(Of String, Object)) As Integer

        Try
            If String.IsNullOrWhiteSpace(tableName) Then
                Throw New ArgumentException("El nombre de la tabla es requerido")
            End If
            If datos Is Nothing OrElse datos.Count = 0 Then
                Throw New ArgumentException("Los datos son requeridos")
            End If

            ' NUEVO: Agregar IdEntidad automáticamente si no existe
            Dim idEntidad = SessionHelper.GetIdEntidadActual()
            If idEntidad.HasValue AndAlso Not datos.ContainsKey("IdEntidad") Then
                datos.Add("IdEntidad", idEntidad.Value)
            End If

            Dim dto As New DynamicDto()

            For Each kvp In datos
                dto(kvp.Key) = kvp.Value
            Next

            Dim url As String = apiPostUrl & SanitizeTableName(tableName)

            Return apiConsumerCRUD.EnviarPostId(url, dto)

        Catch ex As Exception
            Logger.LogError("DynamicCrudService.InsertarConId", ex)
            Throw New ApplicationException($"Error al insertar en {tableName}", ex)

        End Try
    End Function

    ''' <summary>
    ''' Actualiza un registro por su ID
    ''' NOTA: Valida que el registro pertenezca a la entidad actual antes de actualizar
    ''' </summary>
    Public Shared Function Actualizar(tableName As String, id As Integer, datos As Dictionary(Of String, Object)) As Boolean

        Try
            If String.IsNullOrWhiteSpace(tableName) Then
                Throw New ArgumentException("El nombre de la tabla es requerido")
            End If
            If datos Is Nothing OrElse datos.Count = 0 Then
                Throw New ArgumentException("Los datos son requeridos")
            End If

            ' NUEVO: Validar que el registro pertenece a la entidad actual
            Dim idEntidad = SessionHelper.GetIdEntidadActual()
            If idEntidad.HasValue Then
                If Not EntidadHelper.ValidarPerteneceAEntidadActual(id, tableName) Then
                    Throw New UnauthorizedAccessException($"El registro {id} no pertenece a la entidad actual")
                End If
            End If

            Dim dto As New DynamicDto()

            dto("Id") = id

            For Each kvp In datos
                dto(kvp.Key) = kvp.Value
            Next

            Dim url As String = apiPostUrl & SanitizeTableName(tableName) & "/" & id.ToString()

            Return apiConsumerCRUD.EnviarPut(url, dto)

        Catch ex As Exception
            Logger.LogError("DynamicCrudService.Actualizar", ex)
            Throw New ApplicationException($"Error al actualizar {tableName} con Id={id}", ex)

        End Try
    End Function

    ''' <summary>
    ''' Actualiza registros por un campo específico
    ''' </summary>
    Public Shared Function ActualizarPorCampo(tableName As String, fieldName As String, value As Object, datos As Dictionary(Of String, Object)) As Boolean

        Try
            ' Primero obtener el registro para conseguir el ID
            Dim registro = ObtenerPorCampo(tableName, fieldName, value)

            If registro Is Nothing Then
                Return False
            End If

            Dim id As Integer = Convert.ToInt32(registro("Id"))

            Return Actualizar(tableName, id, datos)

        Catch ex As Exception
            Logger.LogError("DynamicCrudService.ActualizarPorCampo", ex)
            Throw New ApplicationException($"Error al actualizar {tableName} por {fieldName}", ex)

        End Try
    End Function

    ''' <summary>
    ''' Elimina un registro por su ID
    ''' NOTA: Valida que el registro pertenezca a la entidad actual antes de eliminar
    ''' </summary>
    Public Shared Function Eliminar(tableName As String, id As Integer) As Boolean

        Try
            If String.IsNullOrWhiteSpace(tableName) Then
                Throw New ArgumentException("El nombre de la tabla es requerido")
            End If

            ' NUEVO: Validar que el registro pertenece a la entidad actual
            Dim idEntidad = SessionHelper.GetIdEntidadActual()
            If idEntidad.HasValue Then
                If Not EntidadHelper.ValidarPerteneceAEntidadActual(id, tableName) Then
                    Throw New UnauthorizedAccessException($"El registro {id} no pertenece a la entidad actual")
                End If
            End If

            Dim url As String = apiPostUrl & SanitizeTableName(tableName) & "/" & id.ToString()

            Return apiConsumerCRUD.EnviarDelete(url)

        Catch ex As Exception
            Logger.LogError("DynamicCrudService.Eliminar", ex)
            Throw New ApplicationException($"Error al eliminar de {tableName} con Id={id}", ex)

        End Try
    End Function

    ''' <summary>
    ''' Ejecuta una consulta SQL personalizada y retorna DataTable
    ''' </summary>
    Public Shared Function EjecutarConsulta(query As String) As DataTable

        Try
            If String.IsNullOrWhiteSpace(query) Then
                Throw New ArgumentException("La consulta es requerida")
            End If

            Dim url As String = baseUrl & System.Web.HttpUtility.UrlEncode(query)
            Dim datos = apiConsumer.ObtenerDatos(url)

            Return apiConsumer.ConvertirADatatable(datos)

        Catch ex As Exception
            Logger.LogError("DynamicCrudService.EjecutarConsulta", ex)
            Throw New ApplicationException("Error al ejecutar consulta", ex)

        End Try
    End Function

    ' Helpers para sanitizar nombres de tablas y campos
    Private Shared Function SanitizeTableName(tableName As String) As String
        ' Solo permitir caracteres alfanuméricos y guiones bajos
        Return System.Text.RegularExpressions.Regex.Replace(tableName, "[^a-zA-Z0-9_]", "")
    End Function

    Private Shared Function SanitizeFieldName(fieldName As String) As String
        ' Solo permitir caracteres alfanuméricos y guiones bajos
        Return System.Text.RegularExpressions.Regex.Replace(fieldName, "[^a-zA-Z0-9_]", "")
    End Function

End Class
