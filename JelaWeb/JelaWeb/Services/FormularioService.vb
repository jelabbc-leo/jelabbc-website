Imports System.Threading.Tasks
Imports Newtonsoft.Json

''' <summary>
''' Servicio principal para gestión de formularios dinámicos
''' </summary>
Public Class FormularioService
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
    ''' Obtiene todos los formularios con filtros opcionales
    ''' </summary>
    Public Function GetFormularios(Optional estado As String = Nothing, Optional plataforma As String = Nothing, Optional idEntidad As Integer = 0) As DataTable

        Try
            Dim whereConditions As New List(Of String)

            ' Filtrar por entidad si se especifica
            If idEntidad > 0 Then
                whereConditions.Add("f.IdEntidad = " & QueryBuilder.EscapeSqlInteger(idEntidad))
            End If

            If Not String.IsNullOrWhiteSpace(estado) Then
                whereConditions.Add("f.estado = " & QueryBuilder.EscapeSqlString(estado))
            End If

            If Not String.IsNullOrWhiteSpace(plataforma) Then
                whereConditions.Add("f.plataformas LIKE '%" & plataforma & "%'")
            End If

            Dim whereClause As String = ""

            If whereConditions.Count > 0 Then
                whereClause = "WHERE " & String.Join(" AND ", whereConditions)
            End If

            Dim query As String = "SELECT " &
                "f.formulario_id, " &
                "f.IdEntidad, " &
                "f.nombre_formulario, " &
                "f.descripcion, " &
                "f.plataformas, " &
                "f.estado, " &
                "f.version, " &
                "f.requiere_firma, " &
                "f.requiere_foto, " &
                "f.tiempo_estimado_minutos, " &
                "f.fecha_creacion, " &
                "(SELECT COUNT(*) FROM cat_campos_formulario c WHERE c.formulario_id = f.formulario_id) as total_campos " &
                "FROM cat_formularios f " &
                whereClause & " " &
                "ORDER BY f.nombre_formulario"

            Dim url As String = baseUrl & System.Web.HttpUtility.UrlEncode(query)
            Dim datos = apiConsumer.ObtenerDatos(url)

            Return apiConsumer.ConvertirADatatable(datos)

        Catch ex As Exception
            Logger.LogError("Error al obtener formularios: " & ex.Message, ex, "")
            Throw New ApplicationException("Error al obtener los formularios desde el API.", ex)

        End Try
    End Function

    ''' <summary>
    ''' Obtiene un formulario por ID
    ''' </summary>
    Public Function GetFormularioById(id As Integer) As FormularioDTO

        Try
            If id <= 0 Then
                Throw New ArgumentException("El ID debe ser mayor a cero")
            End If

            Dim query As String = "SELECT * FROM cat_formularios WHERE formulario_id = " & QueryBuilder.EscapeSqlInteger(id)
            Dim url As String = baseUrl & System.Web.HttpUtility.UrlEncode(query)
            Dim datos = apiConsumer.ObtenerDatos(url)

            If datos IsNot Nothing AndAlso datos.Count > 0 Then
                Dim campos = datos(0).Campos
                Dim dto As New FormularioDTO

                dto.FormularioId = Convert.ToInt32(campos("formulario_id").Valor)
                dto.NombreFormulario = If(campos.ContainsKey("nombre_formulario"), campos("nombre_formulario").Valor?.ToString(), "")
                dto.Descripcion = If(campos.ContainsKey("descripcion"), campos("descripcion").Valor?.ToString(), "")
                dto.Plataformas = If(campos.ContainsKey("plataformas"), campos("plataformas").Valor?.ToString(), "")
                dto.Estado = If(campos.ContainsKey("estado"), campos("estado").Valor?.ToString(), "")
                dto.Version = If(campos.ContainsKey("version") AndAlso campos("version").Valor IsNot Nothing, Convert.ToInt32(campos("version").Valor), 1)
                dto.RequiereFirma = If(campos.ContainsKey("requiere_firma") AndAlso campos("requiere_firma").Valor IsNot Nothing, Convert.ToBoolean(campos("requiere_firma").Valor), False)
                dto.RequiereFoto = If(campos.ContainsKey("requiere_foto") AndAlso campos("requiere_foto").Valor IsNot Nothing, Convert.ToBoolean(campos("requiere_foto").Valor), False)

                If campos.ContainsKey("fecha_creacion") AndAlso campos("fecha_creacion").Valor IsNot Nothing Then
                    dto.FechaCreacion = Convert.ToDateTime(campos("fecha_creacion").Valor)
                End If

                Return dto
            Else
                Return Nothing
            End If

        Catch ex As Exception
            Logger.LogError("Error al obtener formulario por ID: " & ex.Message, ex, "")
            Throw New ApplicationException("Error al obtener el formulario desde el API.", ex)

        End Try
    End Function

    ''' <summary>
    ''' Obtiene formulario completo con campos y opciones
    ''' </summary>
    Public Function GetFormularioCompleto(formularioId As Integer) As FormularioCompletoDTO

        Try
            Dim result As New FormularioCompletoDTO()

            result.Formulario = GetFormularioById(formularioId)

            If result.Formulario IsNot Nothing Then
                result.Campos = GetCamposFormulario(formularioId)

                ' Cargar opciones para campos dropdown/radio

                For Each campo In result.Campos.Where(Function(c) c.TipoCampo = "dropdown" OrElse c.TipoCampo = "radio")
                    campo.Opciones = GetOpcionesCampo(campo.CampoId)

                Next

            End If

            Return result

        Catch ex As Exception
            Logger.LogError("Error al obtener formulario completo: " & ex.Message, ex, "")
            Throw New ApplicationException("Error al obtener el formulario completo.", ex)

        End Try
    End Function

    ''' <summary>
    ''' Obtiene los campos de un formulario
    ''' </summary>
    Public Function GetCamposFormulario(formularioId As Integer) As List(Of CampoFormularioDTO)

        Try
            Dim campos As New List(Of CampoFormularioDTO)()
            Dim query As String = "SELECT * FROM cat_campos_formulario WHERE formulario_id = " &
                QueryBuilder.EscapeSqlInteger(formularioId) & " ORDER BY seccion, posicion_orden"

            Dim url As String = baseUrl & System.Web.HttpUtility.UrlEncode(query)
            Dim datos = apiConsumer.ObtenerDatos(url)

            If datos IsNot Nothing Then

                For Each item In datos
                    Dim c = item.Campos
                    Dim dto As New CampoFormularioDTO()

                    dto.CampoId = Convert.ToInt32(c("campo_id").Valor)
                    dto.FormularioId = Convert.ToInt32(c("formulario_id").Valor)
                    dto.NombreCampo = c("nombre_campo").Valor?.ToString()
                    dto.EtiquetaCampo = c("etiqueta_campo").Valor?.ToString()
                    dto.TipoCampo = c("tipo_campo").Valor?.ToString()
                    dto.EsRequerido = If(c.ContainsKey("es_requerido"), Convert.ToBoolean(c("es_requerido").Valor), False)
                    dto.EsVisible = If(c.ContainsKey("es_visible"), Convert.ToBoolean(c("es_visible").Valor), True)
                    dto.PosicionOrden = If(c.ContainsKey("posicion_orden"), Convert.ToInt32(c("posicion_orden").Valor), 0)
                    dto.Placeholder = c("placeholder")?.Valor?.ToString()
                    dto.AnchoColumna = If(c.ContainsKey("ancho_columna"), Convert.ToInt32(c("ancho_columna").Valor), 12)
                    dto.AlturaCampo = If(c.ContainsKey("altura_campo") AndAlso c("altura_campo").Valor IsNot Nothing AndAlso c("altura_campo").Valor IsNot DBNull.Value, 
                        Convert.ToInt32(c("altura_campo").Valor), Nothing)
                    dto.Seccion = c("seccion")?.Valor?.ToString()
                    campos.Add(dto)

                Next

            End If

            Return campos

        Catch ex As Exception
            Logger.LogError("Error al obtener campos: " & ex.Message, ex, "")
            Throw New ApplicationException("Error al obtener los campos del formulario.", ex)

        End Try
    End Function

    ''' <summary>
    ''' Obtiene las opciones de un campo dropdown/radio
    ''' </summary>
    Public Function GetOpcionesCampo(campoId As Integer) As List(Of OpcionCampoDTO)

        Try
            Dim opciones As New List(Of OpcionCampoDTO)()
            Dim query As String = "SELECT * FROM cat_opciones_campo WHERE campo_id = " &
                QueryBuilder.EscapeSqlInteger(campoId) & " AND activo = 1 ORDER BY posicion_orden"

            Dim url As String = baseUrl & System.Web.HttpUtility.UrlEncode(query)
            Dim datos = apiConsumer.ObtenerDatos(url)

            If datos IsNot Nothing Then

                For Each item In datos
                    Dim c = item.Campos
                    Dim dto As New OpcionCampoDTO()

                    dto.OpcionId = Convert.ToInt32(c("opcion_id").Valor)
                    dto.CampoId = Convert.ToInt32(c("campo_id").Valor)
                    dto.ValorOpcion = c("valor_opcion").Valor?.ToString()
                    dto.EtiquetaOpcion = c("etiqueta_opcion").Valor?.ToString()
                    dto.PosicionOrden = If(c.ContainsKey("posicion_orden"), Convert.ToInt32(c("posicion_orden").Valor), 0)
                    dto.EsDefault = If(c.ContainsKey("es_default"), Convert.ToBoolean(c("es_default").Valor), False)
                    opciones.Add(dto)

                Next

            End If

            Return opciones

        Catch ex As Exception
            Logger.LogError("Error al obtener opciones: " & ex.Message, ex, "")
            Return New List(Of OpcionCampoDTO)()

        End Try
    End Function

    ''' <summary>
    ''' Crea un nuevo formulario
    ''' </summary>
    Public Function CreateFormulario(formulario As FormularioDTO) As Integer

        Try
            Dim dto As New DynamicDto()

            dto("IdEntidad") = formulario.IdEntidad
            dto("nombre_formulario") = formulario.NombreFormulario
            dto("descripcion") = formulario.Descripcion
            dto("plataformas") = formulario.Plataformas
            dto("estado") = If(String.IsNullOrEmpty(formulario.Estado), "borrador", formulario.Estado)
            dto("version") = 1
            dto("requiere_firma") = formulario.RequiereFirma
            dto("requiere_foto") = formulario.RequiereFoto
            dto("tiempo_estimado_minutos") = formulario.TiempoEstimadoMinutos
            dto("instrucciones") = formulario.Instrucciones
            dto("creado_por") = SessionHelper.GetUserId()
            dto("fecha_creacion") = DateTime.Now

            Dim url As String = apiPostUrl & "cat_formularios"

            Return apiConsumerCRUD.EnviarPostId(url, dto)

        Catch ex As Exception
            Logger.LogError("Error al crear formulario: " & ex.Message, ex, "")
            Throw New ApplicationException("Error al crear el formulario.", ex)

        End Try
    End Function

    ''' <summary>
    ''' Actualiza un formulario existente
    ''' </summary>
    Public Function UpdateFormulario(formulario As FormularioDTO) As Boolean

        Try
            Dim dto As New DynamicDto()

            dto("formulario_id") = formulario.FormularioId
            dto("IdEntidad") = formulario.IdEntidad
            dto("nombre_formulario") = formulario.NombreFormulario
            dto("descripcion") = formulario.Descripcion
            dto("plataformas") = formulario.Plataformas
            dto("estado") = formulario.Estado
            dto("requiere_firma") = formulario.RequiereFirma
            dto("requiere_foto") = formulario.RequiereFoto
            dto("tiempo_estimado_minutos") = formulario.TiempoEstimadoMinutos
            dto("instrucciones") = formulario.Instrucciones
            dto("modificado_por") = SessionHelper.GetUserId()
            dto("fecha_modificacion") = DateTime.Now

            Dim url As String = apiPostUrl & "cat_formularios/" & formulario.FormularioId.ToString()

            Return apiConsumerCRUD.EnviarPut(url, dto)

        Catch ex As Exception
            Logger.LogError("Error al actualizar formulario: " & ex.Message, ex, "")
            Throw New ApplicationException("Error al actualizar el formulario.", ex)

        End Try
    End Function

    ''' <summary>
    ''' Elimina un formulario (solo si no tiene asignaciones activas)
    ''' </summary>
    Public Function DeleteFormulario(formularioId As Integer) As Boolean

        Try
            ' Verificar dependencias
            Dim queryDeps As String = "SELECT COUNT(*) as total FROM op_fallo_formulario WHERE formulario_id = " &
                QueryBuilder.EscapeSqlInteger(formularioId) & " AND estado <> 'cancelado'"

            Dim url As String = baseUrl & System.Web.HttpUtility.UrlEncode(queryDeps)
            Dim datos = apiConsumer.ObtenerDatos(url)

            If datos IsNot Nothing AndAlso datos.Count > 0 Then
                Dim total = Convert.ToInt32(datos(0).Campos("total").Valor)

                If total > 0 Then
                    Throw New ApplicationException("No se puede eliminar el formulario porque tiene asignaciones activas.")
                End If
            End If

            ' Eliminar campos primero
            Dim urlDeleteCampos As String = apiPostUrl & "Delete?table=cat_campos_formulario&idField=formulario_id&idValue=" & formularioId

            apiConsumerCRUD.EnviarDelete(urlDeleteCampos)

            ' Eliminar formulario
            Dim urlDelete As String = apiPostUrl & "Delete?table=cat_formularios&idField=formulario_id&idValue=" & formularioId

            Return apiConsumerCRUD.EnviarDelete(urlDelete)

        Catch ex As ApplicationException

            Throw

        Catch ex As Exception
            Logger.LogError("Error al eliminar formulario: " & ex.Message, ex, "")
            Throw New ApplicationException("Error al eliminar el formulario.", ex)

        End Try
    End Function

    ''' <summary>
    ''' Crea un formulario desde campos extraídos de PDF
    ''' </summary>
    Public Function CrearFormularioDesdeExtraccion(nombre As String, descripcion As String, campos As List(Of CampoExtraidoDTO)) As Integer

        Try
            ' Crear formulario
            Dim formulario As New FormularioDTO()

            formulario.NombreFormulario = nombre
            formulario.Descripcion = descripcion
            formulario.Plataformas = "web,movil"
            formulario.Estado = "borrador"

            Dim formularioId = CreateFormulario(formulario)

            ' Crear campos
            Dim orden As Integer = 1

            For Each campo In campos
                Dim campoDto As New DynamicDto()

                campoDto("formulario_id") = formularioId
                campoDto("nombre_campo") = campo.NombreCampo
                campoDto("etiqueta_campo") = campo.EtiquetaCampo
                campoDto("tipo_campo") = campo.TipoCampo
                campoDto("posicion_orden") = orden
                campoDto("seccion") = If(String.IsNullOrEmpty(campo.Seccion), "General", campo.Seccion)
                campoDto("ancho_columna") = 12
                campoDto("es_requerido") = False
                campoDto("es_visible") = True

                Dim urlCampo As String = apiPostUrl & "cat_campos_formulario"

                apiConsumerCRUD.EnviarPostId(urlCampo, campoDto)
                orden += 1

            Next

            Return formularioId

        Catch ex As Exception
            Logger.LogError("Error al crear formulario desde extracción: " & ex.Message, ex, "")
            Throw New ApplicationException("Error al crear el formulario desde PDF.", ex)

        End Try
    End Function

    ''' <summary>
    ''' Crea un campo en un formulario
    ''' </summary>
    Public Function CreateCampo(campo As CampoFormularioDTO) As Integer

        Try
            Dim dto As New DynamicDto()

            dto("formulario_id") = campo.FormularioId
            dto("nombre_campo") = campo.NombreCampo
            dto("etiqueta_campo") = campo.EtiquetaCampo
            dto("tipo_campo") = campo.TipoCampo
            dto("es_requerido") = campo.EsRequerido
            dto("es_visible") = campo.EsVisible
            dto("posicion_orden") = campo.PosicionOrden
            dto("placeholder") = campo.Placeholder
            dto("longitud_maxima") = campo.LongitudMaxima
            dto("ancho_columna") = If(campo.AnchoColumna > 0, campo.AnchoColumna, 12)
            dto("altura_campo") = If(campo.AlturaCampo.HasValue, campo.AlturaCampo.Value, DBNull.Value)
            dto("seccion") = If(String.IsNullOrEmpty(campo.Seccion), "General", campo.Seccion)

            Dim url As String = apiPostUrl & "cat_campos_formulario"

            Return apiConsumerCRUD.EnviarPostId(url, dto)

        Catch ex As Exception
            Logger.LogError("Error al crear campo: " & ex.Message, ex, "")
            Throw New ApplicationException("Error al crear el campo.", ex)

        End Try
    End Function

    ''' <summary>
    ''' Actualiza un campo existente
    ''' </summary>
    Public Function UpdateCampo(campo As CampoFormularioDTO) As Boolean

        Try
            Dim dto As New DynamicDto()

            dto("campo_id") = campo.CampoId
            dto("nombre_campo") = campo.NombreCampo
            dto("etiqueta_campo") = campo.EtiquetaCampo
            dto("tipo_campo") = campo.TipoCampo
            dto("es_requerido") = campo.EsRequerido
            dto("es_visible") = campo.EsVisible
            dto("posicion_orden") = campo.PosicionOrden
            dto("placeholder") = campo.Placeholder
            dto("longitud_maxima") = campo.LongitudMaxima
            dto("ancho_columna") = campo.AnchoColumna
            dto("altura_campo") = If(campo.AlturaCampo.HasValue, campo.AlturaCampo.Value, DBNull.Value)
            dto("seccion") = campo.Seccion

            Dim url As String = apiPostUrl & "cat_campos_formulario/" & campo.CampoId.ToString()

            Return apiConsumerCRUD.EnviarPut(url, dto)

        Catch ex As Exception
            Logger.LogError("Error al actualizar campo: " & ex.Message, ex, "")
            Throw New ApplicationException("Error al actualizar el campo.", ex)

        End Try
    End Function

    ''' <summary>
    ''' Elimina un campo
    ''' </summary>
    Public Function DeleteCampo(campoId As Integer) As Boolean

        Try
            ' Eliminar opciones primero
            Dim urlDeleteOpciones As String = apiPostUrl & "Delete?table=cat_opciones_campo&idField=campo_id&idValue=" & campoId

            apiConsumerCRUD.EnviarDelete(urlDeleteOpciones)

            ' Eliminar campo
            Dim urlDelete As String = apiPostUrl & "Delete?table=cat_campos_formulario&idField=campo_id&idValue=" & campoId

            Return apiConsumerCRUD.EnviarDelete(urlDelete)

        Catch ex As Exception
            Logger.LogError("Error al eliminar campo: " & ex.Message, ex, "")
            Throw New ApplicationException("Error al eliminar el campo.", ex)

        End Try
    End Function

    ''' <summary>
    ''' Obtiene campos como DataTable para grid
    ''' </summary>
    Public Function GetCamposDataTable(formularioId As Integer) As DataTable

        Try
            Dim query As String = "SELECT campo_id, formulario_id, nombre_campo, etiqueta_campo, tipo_campo, " &
                "es_requerido, es_visible, posicion_orden, placeholder, ancho_columna, seccion " &
                "FROM cat_campos_formulario WHERE formulario_id = " & QueryBuilder.EscapeSqlInteger(formularioId) &
                " ORDER BY seccion, posicion_orden"

            Dim url As String = baseUrl & System.Web.HttpUtility.UrlEncode(query)
            Dim datos = apiConsumer.ObtenerDatos(url)

            Return apiConsumer.ConvertirADatatable(datos)

        Catch ex As Exception
            Logger.LogError("Error al obtener campos DataTable: " & ex.Message, ex, "")
            Return New DataTable()

        End Try
    End Function
End Class
