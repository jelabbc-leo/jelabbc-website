Imports System.Data
Imports System.Collections.Generic
Imports JelaWeb.Utilities

''' <summary>
''' Servicio para gestión de Comunicados
''' </summary>
Public Class ComunicadosService

    Private Const TABLA_COMUNICADOS As String = "op_comunicados"
    Private Const TABLA_SUBENTIDADES As String = "cat_subentidades"
    Private Const TABLA_ENTIDADES As String = "cat_entidades"

    ''' <summary>
    ''' Obtiene un comunicado por ID
    ''' </summary>
    <System.Web.Services.WebMethod()>
    Public Shared Function ObtenerComunicado(id As Integer) As Object

        Try
            Dim registro As DataRow = DynamicCrudService.ObtenerPorId(TABLA_COMUNICADOS, id)

            If registro Is Nothing Then Return New With {.success = False, .message = "No encontrado"}

            Dim datos As New Dictionary(Of String, Object)

            For Each col As DataColumn In registro.Table.Columns
                datos(col.ColumnName) = If(IsDBNull(registro(col)), Nothing, registro(col))

            Next

            Return New With {.success = True, .data = datos}

        Catch ex As Exception
            Logger.LogError("ComunicadosService.ObtenerComunicado", ex)
            Return New With {.success = False, .message = "Error: " & ex.Message}

        End Try
    End Function

    ''' <summary>
    ''' Guarda un comunicado (crea o actualiza)
    ''' </summary>
    <System.Web.Services.WebMethod()>
    Public Shared Function GuardarComunicado(datos As Dictionary(Of String, Object)) As Object

        Try
            Dim id As Integer = Convert.ToInt32(datos("id"))
            Dim datosGuardar As New Dictionary(Of String, Object)
            datosGuardar.Add("EntidadId", Convert.ToInt32(datos("entidadId")))
            datosGuardar.Add("SubEntidadId", If(datos("subEntidadId") IsNot Nothing AndAlso Convert.ToInt32(datos("subEntidadId")) > 0, Convert.ToInt32(datos("subEntidadId")), DBNull.Value))
            datosGuardar.Add("Titulo", datos("titulo")?.ToString())
            datosGuardar.Add("Contenido", datos("contenido")?.ToString())
            datosGuardar.Add("TipoComunicado", datos("tipoComunicado")?.ToString())
            datosGuardar.Add("FechaPublicacion", Convert.ToDateTime(datos("fechaPublicacion")))
            datosGuardar.Add("FechaExpiracion", If(datos("fechaExpiracion") IsNot Nothing, Convert.ToDateTime(datos("fechaExpiracion")), DBNull.Value))
            datosGuardar.Add("EnviarEmail", If(Convert.ToBoolean(datos("enviarEmail")), 1, 0))
            datosGuardar.Add("EnviarTelegram", If(Convert.ToBoolean(datos("enviarTelegram")), 1, 0))
            datosGuardar.Add("EnviarPush", If(Convert.ToBoolean(datos("enviarPush")), 1, 0))
            datosGuardar.Add("Destinatarios", datos("destinatarios")?.ToString())
            datosGuardar.Add("ArchivoAdjunto", datos("archivoAdjunto")?.ToString())
            datosGuardar.Add("Estado", datos("estado")?.ToString())

            Dim resultado As Boolean

            If id = 0 Then
                resultado = DynamicCrudService.Insertar(TABLA_COMUNICADOS, datosGuardar)
            Else
                resultado = DynamicCrudService.Actualizar(TABLA_COMUNICADOS, id, datosGuardar)
            End If

            Return New With {.success = resultado, .message = If(resultado, "Guardado correctamente", "Error al guardar")}

        Catch ex As Exception
            Logger.LogError("ComunicadosService.GuardarComunicado", ex)
            Return New With {.success = False, .message = "Error: " & ex.Message}

        End Try
    End Function

    ''' <summary>
    ''' Elimina un comunicado
    ''' </summary>
    <System.Web.Services.WebMethod()>
    Public Shared Function EliminarComunicado(id As Integer) As Object

        Try
            Dim resultado = DynamicCrudService.Eliminar(TABLA_COMUNICADOS, id)

            Return New With {.success = resultado, .message = If(resultado, "Eliminado", "Error")}

        Catch ex As Exception
            Logger.LogError("ComunicadosService.EliminarComunicado", ex)
            Return New With {.success = False, .message = "Error: " & ex.Message}

        End Try
    End Function

    ''' <summary>
    ''' Publica un comunicado
    ''' </summary>
    <System.Web.Services.WebMethod()>
    Public Shared Function PublicarComunicado(id As Integer) As Object

        Try
            Dim datos As New Dictionary(Of String, Object)
            datos.Add("Estado", "Publicado")
            datos.Add("FechaPublicacion", DateTime.Now)

            Dim resultado = DynamicCrudService.Actualizar(TABLA_COMUNICADOS, id, datos)

            Return New With {.success = resultado, .message = If(resultado, "Comunicado publicado", "Error")}

        Catch ex As Exception
            Logger.LogError("ComunicadosService.PublicarComunicado", ex)
            Return New With {.success = False, .message = "Error: " & ex.Message}

        End Try
    End Function

    ''' <summary>
    ''' Lista las subentidades de una entidad
    ''' </summary>
    <System.Web.Services.WebMethod()>
    Public Shared Function ListarSubEntidades(entidadId As Integer) As Object

        Try
            Dim query As String = "SELECT Id, RazonSocial AS Nombre " &
                                  "FROM " & TABLA_SUBENTIDADES & " " &
                                  "WHERE EntidadId = " & entidadId & " AND Activo = 1 " &
                                  "ORDER BY RazonSocial"
            Dim dt As DataTable = DynamicCrudService.EjecutarConsulta(query)

            Return New With {.success = True, .data = ComunicadosHelper.ConvertDataTableToList(dt)}

        Catch ex As Exception
            Return New With {.success = False, .message = "Error: " & ex.Message}

        End Try
    End Function

    ''' <summary>
    ''' Lista todos los comunicados con información relacionada
    ''' </summary>
    Public Shared Function ListarComunicados() As DataTable

        Try
            Dim query As String = "SELECT c.*, " &
                "e.RazonSocial AS EntidadNombre, " &
                "se.RazonSocial AS SubEntidadNombre " &
                "FROM " & TABLA_COMUNICADOS & " c " &
                "LEFT JOIN cat_entidades e ON c.EntidadId = e.Id " &
                "LEFT JOIN " & TABLA_SUBENTIDADES & " se ON c.SubEntidadId = se.Id " &
                "ORDER BY c.FechaPublicacion DESC, c.FechaCreacion DESC"

            Return DynamicCrudService.EjecutarConsulta(query)

        Catch ex As Exception
            Logger.LogError("ComunicadosService.ListarComunicados", ex)
            Return New DataTable()

        End Try
    End Function

    ''' <summary>
    ''' Lista las entidades activas para combos
    ''' </summary>
    Public Shared Function ListarEntidades() As DataTable

        Try
            Return DynamicCrudService.ObtenerTodosConFiltro(TABLA_ENTIDADES, "Activo = 1")

        Catch ex As Exception
            Logger.LogError("ComunicadosService.ListarEntidades", ex)
            Return New DataTable()

        End Try
    End Function

End Class
