Imports System.Data
Imports System.Collections.Generic
Imports JelaWeb.Utilities

''' <summary>
''' Servicio para gestión de Áreas Comunes
''' </summary>
Public Class AreasComunesService

    Private Const TABLA_AREAS_COMUNES As String = "cat_areas_comunes"
    Private Const TABLA_SUB_ENTIDADES As String = "cat_sub_entidades"
    Private Const TABLA_ENTIDADES As String = "cat_entidades"

    ''' <summary>
    ''' Obtiene un área común por ID
    ''' </summary>
    <System.Web.Services.WebMethod()>
    Public Shared Function ObtenerArea(id As Integer) As Object

        Try
            Dim registro As DataRow = DynamicCrudService.ObtenerPorId(TABLA_AREAS_COMUNES, id)

            If registro Is Nothing Then
                Return New With {.success = False, .message = "Área no encontrada"}
            End If

            Dim datos As New Dictionary(Of String, Object)

            For Each col As DataColumn In registro.Table.Columns
                datos(col.ColumnName) = If(IsDBNull(registro(col)), Nothing, registro(col))

            Next

            Return New With {.success = True, .data = datos}

        Catch ex As Exception
            Logger.LogError("AreasComunesService.ObtenerArea", ex)
            Return New With {.success = False, .message = "Error al obtener área"}

        End Try
    End Function

    ''' <summary>
    ''' Elimina un área común
    ''' </summary>
    <System.Web.Services.WebMethod()>
    Public Shared Function EliminarArea(id As Integer) As Object

        Try
            Dim resultado As Boolean = DynamicCrudService.Eliminar(TABLA_AREAS_COMUNES, id)

            If resultado Then
                Logger.LogInfo($"Área común eliminada: Id={id}")
                Return New With {.success = True, .message = "Área común eliminada correctamente"}
            Else
                Return New With {.success = False, .message = "Error al eliminar área común"}
            End If

        Catch ex As Exception
            Logger.LogError("AreasComunesService.EliminarArea", ex)
            Return New With {.success = False, .message = "Error al eliminar área común"}

        End Try
    End Function

    ''' <summary>
    ''' Obtiene las sub-entidades de una entidad
    ''' </summary>
    <System.Web.Services.WebMethod()>
    Public Shared Function ObtenerSubEntidadesPorEntidad(entidadId As Integer) As Object

        Try
            Dim dt As DataTable = DynamicCrudService.ObtenerTodosConFiltro(TABLA_SUB_ENTIDADES, $"EntidadId = {entidadId} AND Activo = 1")
            Dim lista As New List(Of Object)

            For Each row As DataRow In dt.Rows
                lista.Add(New With {
                    .Id = row("Id"),
                    .RazonSocial = row("RazonSocial")?.ToString()
                })

            Next

            Return New With {.success = True, .data = lista}

        Catch ex As Exception
            Logger.LogError("AreasComunesService.ObtenerSubEntidadesPorEntidad", ex)
            Return New With {.success = False, .message = "Error al obtener sub-entidades"}

        End Try
    End Function

    ''' <summary>
    ''' Lista todas las áreas comunes con información de entidad y sub-entidad
    ''' </summary>
    Public Shared Function ListarAreas() As DataTable

        Try
            Dim query As String = "SELECT a.*, " &
                "e.RazonSocial AS EntidadNombre, " &
                "se.RazonSocial AS SubEntidadNombre " &
                "FROM " & TABLA_AREAS_COMUNES & " a " &
                "LEFT JOIN cat_entidades e ON a.EntidadId = e.Id " &
                "LEFT JOIN " & TABLA_SUB_ENTIDADES & " se ON a.SubEntidadId = se.Id " &
                "ORDER BY a.Nombre"

            Return DynamicCrudService.EjecutarConsulta(query)

        Catch ex As Exception
            Logger.LogError("AreasComunesService.ListarAreas", ex)
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
            Logger.LogError("AreasComunesService.ListarEntidades", ex)
            Return New DataTable()

        End Try
    End Function

    ''' <summary>
    ''' Guarda un área común (crea o actualiza)
    ''' </summary>
    Public Shared Function GuardarArea(areaId As Integer, datos As Dictionary(Of String, Object), userId As Integer?) As Boolean

        Try
            If areaId = 0 Then
                If userId.HasValue Then
                    datos("CreadoPor") = userId.Value
                End If

                Return DynamicCrudService.Insertar(TABLA_AREAS_COMUNES, datos)
            Else
                If userId.HasValue Then
                    datos("ModificadoPor") = userId.Value
                End If

                Return DynamicCrudService.Actualizar(TABLA_AREAS_COMUNES, areaId, datos)
            End If

        Catch ex As Exception
            Logger.LogError("AreasComunesService.GuardarArea", ex)
            Return False

        End Try
    End Function

End Class
