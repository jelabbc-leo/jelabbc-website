Imports System.Data
Imports System.Net.Http
Imports System.Text
Imports System.Threading.Tasks
Imports Newtonsoft.Json
Imports System.IO

''' <summary>
''' Servicio para gestión de tickets
''' </summary>
Public Class TicketService
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
    ''' Obtiene lista de tickets con filtros opcionales
    ''' </summary>
    Public Function GetTickets(Optional estado As String = Nothing, Optional categoriaId As Integer? = Nothing, Optional prioridad As String = Nothing, Optional folio As String = Nothing, Optional titulo As String = Nothing, Optional idEntidad As Integer = 0) As DataTable

        Try
            Dim whereConditions As New List(Of String)

            ' Filtrar por entidad si se especifica
            If idEntidad > 0 Then
                whereConditions.Add("t.IdEntidad = " & QueryBuilder.EscapeSqlInteger(idEntidad))
            End If

            If Not String.IsNullOrWhiteSpace(estado) Then
                whereConditions.Add("t.Estado = " & QueryBuilder.EscapeSqlString(estado))
            End If

            If categoriaId.HasValue AndAlso categoriaId.Value > 0 Then
                whereConditions.Add("t.IdCategoria = " & QueryBuilder.EscapeSqlInteger(categoriaId.Value))
            End If

            If Not String.IsNullOrWhiteSpace(prioridad) Then
                whereConditions.Add("t.Prioridad = " & QueryBuilder.EscapeSqlString(prioridad))
            End If

            If Not String.IsNullOrWhiteSpace(folio) Then
                whereConditions.Add("t.Folio LIKE '%" & QueryBuilder.EscapeSqlString(folio.Replace("'", "''")) & "%'")
            End If

            If Not String.IsNullOrWhiteSpace(titulo) Then
                whereConditions.Add("t.Titulo LIKE '%" & QueryBuilder.EscapeSqlString(titulo.Replace("'", "''")) & "%'")
            End If

            Dim whereClause As String = ""

            If whereConditions.Count > 0 Then
                whereClause = "WHERE " & String.Join(" AND ", whereConditions)
            End If

            Dim query As String = "SELECT " &
                "t.Id, " &
                "t.Folio, " &
                "t.Titulo, " &
                "c.Nombre AS CategoriaNombre, " &
                "t.Prioridad, " &
                "t.Estado, " &
                "t.FechaCreacion, " &
                "t.FechaModificacion, " &
                "u.Nombre AS UsuarioCreadorNombre, " &
                "e.RazonSocial AS EntidadNombre " &
                "FROM op_tickets t " &
                "LEFT JOIN cat_categorias_ticket c ON t.IdCategoria = c.Id " &
                "LEFT JOIN cat_usuarios u ON t.IdUsuarioCreador = u.Id " &
                "LEFT JOIN cat_entidades e ON t.IdEntidad = e.Id " &
                whereClause & " " &
                "ORDER BY t.FechaCreacion DESC"

            Dim url As String = baseUrl & System.Web.HttpUtility.UrlEncode(query)
            Dim datos = apiConsumer.ObtenerDatos(url)

            Return apiConsumer.ConvertirADatatable(datos)

        Catch ex As Exception
            Logger.LogError("Error al obtener tickets: " & ex.Message, ex, "")
            Throw New ApplicationException("Error al obtener los tickets desde el API.", ex)

        End Try
    End Function

    ''' <summary>
    ''' Obtiene un ticket por su ID
    ''' </summary>
    Public Function GetTicketById(id As Integer) As TicketDTO

        Try
            If id <= 0 Then
                Throw New ArgumentException("El ID debe ser mayor a cero")
            End If

            Dim query As String = "SELECT " &
                "t.Id, " &
                "t.Folio, " &
                "t.Titulo, " &
                "t.Descripcion, " &
                "t.IdCategoria AS CategoriaId, " &
                "c.Nombre AS CategoriaNombre, " &
                "t.Prioridad, " &
                "t.Estado, " &
                "t.IdUsuarioCreador AS UsuarioCreadorId, " &
                "u.Nombre AS UsuarioCreadorNombre, " &
                "t.IdEntidad AS EntidadId, " &
                "e.RazonSocial AS EntidadNombre, " &
                "t.IdSubEntidad AS SubEntidadId, " &
                "se.RazonSocial AS SubEntidadNombre, " &
                "t.FechaCreacion, " &
                "t.FechaModificacion " &
                "FROM op_tickets t " &
                "LEFT JOIN cat_categorias_ticket c ON t.IdCategoria = c.Id " &
                "LEFT JOIN cat_usuarios u ON t.IdUsuarioCreador = u.Id " &
                "LEFT JOIN cat_entidades e ON t.IdEntidad = e.Id " &
                "LEFT JOIN cat_sub_entidades se ON t.IdSubEntidad = se.Id " &
                "WHERE t.Id = " & QueryBuilder.EscapeSqlInteger(id)

            Dim url As String = baseUrl & System.Web.HttpUtility.UrlEncode(query)
            Dim datos = apiConsumer.ObtenerDatos(url)

            If datos IsNot Nothing AndAlso datos.Count > 0 Then
                Dim campos = datos(0).Campos
                Dim dto As New TicketDTO

                dto.Id = Convert.ToInt32(campos("Id").Valor)
                dto.Folio = If(campos.ContainsKey("Folio") AndAlso campos("Folio") IsNot Nothing AndAlso campos("Folio").Valor IsNot Nothing, campos("Folio").Valor.ToString(), "")
                dto.Titulo = If(campos.ContainsKey("Titulo") AndAlso campos("Titulo") IsNot Nothing AndAlso campos("Titulo").Valor IsNot Nothing, campos("Titulo").Valor.ToString(), "")
                dto.Descripcion = If(campos.ContainsKey("Descripcion") AndAlso campos("Descripcion") IsNot Nothing AndAlso campos("Descripcion").Valor IsNot Nothing, campos("Descripcion").Valor.ToString(), "")
                dto.CategoriaId = If(campos.ContainsKey("CategoriaId") AndAlso campos("CategoriaId") IsNot Nothing AndAlso campos("CategoriaId").Valor IsNot Nothing, Convert.ToInt32(campos("CategoriaId").Valor), 0)
                dto.CategoriaNombre = If(campos.ContainsKey("CategoriaNombre") AndAlso campos("CategoriaNombre") IsNot Nothing AndAlso campos("CategoriaNombre").Valor IsNot Nothing, campos("CategoriaNombre").Valor.ToString(), "")
                dto.Prioridad = If(campos.ContainsKey("Prioridad") AndAlso campos("Prioridad") IsNot Nothing AndAlso campos("Prioridad").Valor IsNot Nothing, campos("Prioridad").Valor.ToString(), "")
                dto.Estado = If(campos.ContainsKey("Estado") AndAlso campos("Estado") IsNot Nothing AndAlso campos("Estado").Valor IsNot Nothing, campos("Estado").Valor.ToString(), "")
                dto.UsuarioCreadorId = If(campos.ContainsKey("UsuarioCreadorId") AndAlso campos("UsuarioCreadorId") IsNot Nothing AndAlso campos("UsuarioCreadorId").Valor IsNot Nothing, Convert.ToInt32(campos("UsuarioCreadorId").Valor), 0)
                dto.UsuarioCreadorNombre = If(campos.ContainsKey("UsuarioCreadorNombre") AndAlso campos("UsuarioCreadorNombre") IsNot Nothing AndAlso campos("UsuarioCreadorNombre").Valor IsNot Nothing, campos("UsuarioCreadorNombre").Valor.ToString(), "")
                dto.EntidadId = If(campos.ContainsKey("EntidadId") AndAlso campos("EntidadId") IsNot Nothing AndAlso campos("EntidadId").Valor IsNot Nothing, Convert.ToInt32(campos("EntidadId").Valor), 0)
                dto.EntidadNombre = If(campos.ContainsKey("EntidadNombre") AndAlso campos("EntidadNombre") IsNot Nothing AndAlso campos("EntidadNombre").Valor IsNot Nothing, campos("EntidadNombre").Valor.ToString(), "")

                If campos.ContainsKey("SubEntidadId") AndAlso campos("SubEntidadId") IsNot Nothing AndAlso campos("SubEntidadId").Valor IsNot Nothing AndAlso Not IsDBNull(campos("SubEntidadId").Valor) Then
                    dto.SubEntidadId = Convert.ToInt32(campos("SubEntidadId").Valor)
                End If

                dto.SubEntidadNombre = If(campos.ContainsKey("SubEntidadNombre") AndAlso campos("SubEntidadNombre") IsNot Nothing AndAlso campos("SubEntidadNombre").Valor IsNot Nothing, campos("SubEntidadNombre").Valor.ToString(), "")

                If campos.ContainsKey("FechaCreacion") AndAlso campos("FechaCreacion").Valor IsNot Nothing Then
                    dto.FechaCreacion = Convert.ToDateTime(campos("FechaCreacion").Valor)
                End If

                If campos.ContainsKey("FechaModificacion") AndAlso campos("FechaModificacion").Valor IsNot Nothing Then
                    dto.FechaModificacion = Convert.ToDateTime(campos("FechaModificacion").Valor)
                End If

                Return dto
            Else
                Return Nothing
            End If

        Catch ex As Exception
            Logger.LogError("Error al obtener ticket por ID: " & ex.Message, ex, "")
            Throw New ApplicationException("Error al obtener el ticket desde el API.", ex)

        End Try
    End Function

    ''' <summary>
    ''' Crea un nuevo ticket
    ''' </summary>
    Public Function CreateTicket(ticketDTO As TicketDTO) As Integer

        Try
            Dim dto As New DynamicDto()

            dto("Titulo") = ticketDTO.Titulo
            dto("Descripcion") = ticketDTO.Descripcion
            dto("IdCategoria") = ticketDTO.CategoriaId
            dto("Prioridad") = ticketDTO.Prioridad
            dto("Estado") = "Abierto"
            dto("IdEntidad") = ticketDTO.EntidadId
            If ticketDTO.SubEntidadId.HasValue Then
                dto("IdSubEntidad") = ticketDTO.SubEntidadId.Value
            End If
            dto("IdUsuarioCreador") = SessionHelper.GetUserId()
            dto("FechaCreacion") = DateTime.Now
            dto("FechaModificacion") = DateTime.Now

            Dim url As String = apiPostUrl & "op_tickets"

            Return apiConsumerCRUD.EnviarPostId(url, dto)

        Catch ex As Exception
            Logger.LogError("Error al crear ticket: " & ex.Message, ex, "")
            Throw New ApplicationException("Error al crear el ticket.", ex)

        End Try
    End Function

    ''' <summary>
    ''' Actualiza un ticket existente
    ''' </summary>
    Public Function UpdateTicket(ticketDTO As TicketDTO) As Boolean

        Try
            Dim dto As New DynamicDto()

            dto("Id") = ticketDTO.Id
            dto("Titulo") = ticketDTO.Titulo
            dto("Descripcion") = ticketDTO.Descripcion
            dto("IdCategoria") = ticketDTO.CategoriaId
            dto("Prioridad") = ticketDTO.Prioridad
            dto("Estado") = ticketDTO.Estado
            dto("IdEntidad") = ticketDTO.EntidadId
            If ticketDTO.SubEntidadId.HasValue Then
                dto("IdSubEntidad") = ticketDTO.SubEntidadId.Value
            End If
            dto("FechaModificacion") = DateTime.Now

            Dim url As String = apiPostUrl & "op_tickets/" & ticketDTO.Id.ToString()

            Return apiConsumerCRUD.EnviarPut(url, dto)

        Catch ex As Exception
            Logger.LogError("Error al actualizar ticket: " & ex.Message, ex, "")
            Throw New ApplicationException("Error al actualizar el ticket.", ex)

        End Try
    End Function

    ''' <summary>
    ''' Obtiene las categorías de tickets activas
    ''' </summary>
    Public Function GetCategorias() As DataTable

        Try
            Dim query As String = "SELECT Id, Nombre FROM cat_categorias_ticket WHERE Activo = 1 ORDER BY Nombre"
            Dim url As String = baseUrl & System.Web.HttpUtility.UrlEncode(query)
            Dim datos = apiConsumer.ObtenerDatos(url)

            Return apiConsumer.ConvertirADatatable(datos)

        Catch ex As Exception
            Logger.LogError("Error al obtener categorías: " & ex.Message, ex, "")
            Throw New ApplicationException("Error al obtener las categorías desde el API.", ex)

        End Try
    End Function

    ''' <summary>
    ''' Obtiene las entidades activas
    ''' </summary>
    Public Function GetEntidades() As DataTable

        Try
            Dim query As String = "SELECT Id, RazonSocial AS Nombre FROM cat_entidades WHERE Activo = 1 ORDER BY RazonSocial"
            Dim url As String = baseUrl & System.Web.HttpUtility.UrlEncode(query)
            Dim datos = apiConsumer.ObtenerDatos(url)

            Return apiConsumer.ConvertirADatatable(datos)

        Catch ex As Exception
            Logger.LogError("Error al obtener entidades: " & ex.Message, ex, "")
            Throw New ApplicationException("Error al obtener las entidades desde el API.", ex)

        End Try
    End Function

    ''' <summary>
    ''' Obtiene las sub-entidades de una entidad
    ''' </summary>
    Public Function GetSubEntidades(entidadId As Integer) As DataTable

        Try
            If entidadId <= 0 Then
                Return New DataTable()
            End If

            Dim query As String = "SELECT Id, RazonSocial AS Nombre FROM cat_sub_entidades WHERE IdEntidad = " & QueryBuilder.EscapeSqlInteger(entidadId) & " AND Activo = 1 ORDER BY RazonSocial"
            Dim url As String = baseUrl & System.Web.HttpUtility.UrlEncode(query)
            Dim datos = apiConsumer.ObtenerDatos(url)

            Return apiConsumer.ConvertirADatatable(datos)

        Catch ex As Exception
            Logger.LogError("Error al obtener sub-entidades: " & ex.Message, ex, "")
            Throw New ApplicationException("Error al obtener las sub-entidades desde el API.", ex)

        End Try
    End Function

    ''' <summary>
    ''' Sube un archivo adjunto para un ticket
    ''' </summary>
    Public Function UploadAdjunto(ticketId As Integer, nombreArchivo As String, tipoArchivo As String, archivoBytes As Byte()) As Integer

        Try
            If archivoBytes Is Nothing OrElse archivoBytes.Length = 0 Then
                Throw New ArgumentException("El archivo no puede estar vacío")
            End If

            ' Validar tamaño máximo (10MB)
            Dim maxSizeBytes As Long = 10 * 1024 * 1024 ' 10MB

            If archivoBytes.Length > maxSizeBytes Then
                Throw New ArgumentException("El archivo excede el tamaño máximo permitido de 10MB")
            End If

            ' Convertir a Base64 para enviar en JSON
            Dim archivoBase64 As String = Convert.ToBase64String(archivoBytes)
            Dim dto As New DynamicDto()

            dto("IdTicket") = ticketId
            dto("NombreArchivo") = nombreArchivo
            dto("TipoArchivo") = tipoArchivo
            dto("TamanioArchivo") = archivoBytes.Length
            dto("ArchivoBase64") = archivoBase64
            dto("FechaCreacion") = DateTime.Now

            Dim url As String = apiPostUrl & "op_ticket_adjuntos"

            Return apiConsumerCRUD.EnviarPostId(url, dto)

        Catch ex As Exception
            Logger.LogError("Error al subir adjunto: " & ex.Message, ex, "")
            Throw New ApplicationException("Error al subir el archivo adjunto.", ex)

        End Try
    End Function

End Class
