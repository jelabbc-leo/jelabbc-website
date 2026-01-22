Imports System.Data
Imports System.Collections.Generic
Imports JelaWeb.Utilities

''' <summary>
''' Servicio para gestión de Sub Entidades
''' </summary>
Public Class SubEntidadesService

    Private Const TABLA_SUB_ENTIDADES As String = "cat_sub_entidades"

    ''' <summary>
    ''' Obtiene una sub entidad por ID
    ''' </summary>
    <System.Web.Services.WebMethod()>
    Public Shared Function ObtenerSubEntidad(id As Integer) As Object

        Try
            Dim registro As DataRow = DynamicCrudService.ObtenerPorId(TABLA_SUB_ENTIDADES, id)

            If registro Is Nothing Then
                Return New With {.success = False, .message = "Sub entidad no encontrada"}
            End If

            Dim datos As New Dictionary(Of String, Object)

            For Each col As DataColumn In registro.Table.Columns
                datos(col.ColumnName) = If(IsDBNull(registro(col)), Nothing, registro(col))

            Next

            Return New With {.success = True, .data = datos}

        Catch ex As Exception
            Logger.LogError("SubEntidadesService.ObtenerSubEntidad", ex)
            Return New With {.success = False, .message = "Error al obtener sub entidad"}

        End Try
    End Function

    ''' <summary>
    ''' Elimina una sub entidad
    ''' </summary>
    <System.Web.Services.WebMethod()>
    Public Shared Function EliminarSubEntidad(id As Integer) As Object

        Try
            Dim resultado As Boolean = DynamicCrudService.Eliminar(TABLA_SUB_ENTIDADES, id)

            If resultado Then
                Logger.LogInfo($"Sub entidad eliminada: Id={id}")
                Return New With {.success = True, .message = "Sub entidad eliminada correctamente"}
            Else
                Return New With {.success = False, .message = "Error al eliminar sub entidad"}
            End If

        Catch ex As Exception
            Logger.LogError("SubEntidadesService.EliminarSubEntidad", ex)
            Return New With {.success = False, .message = "Error al eliminar sub entidad"}

        End Try
    End Function

    ''' <summary>
    ''' Lista sub entidades
    ''' </summary>
    Public Shared Function ListarSubEntidades() As DataTable

        Try
            Return DynamicCrudService.ObtenerTodos(TABLA_SUB_ENTIDADES)

        Catch ex As Exception
            Logger.LogError("SubEntidadesService.ListarSubEntidades", ex)
            Return New DataTable()

        End Try
    End Function

    ''' <summary>
    ''' Guarda una sub entidad (crea o actualiza)
    ''' </summary>
    Public Shared Function GuardarSubEntidad(subEntidadId As Integer, datos As Dictionary(Of String, Object)) As Integer

        Try
            If subEntidadId = 0 Then
                Return DynamicCrudService.InsertarConId(TABLA_SUB_ENTIDADES, datos)
            Else
                Dim actualizado As Boolean = DynamicCrudService.Actualizar(TABLA_SUB_ENTIDADES, subEntidadId, datos)

                If actualizado Then
                    Return subEntidadId
                End If

                Return 0
            End If

        Catch ex As Exception
            Logger.LogError("SubEntidadesService.GuardarSubEntidad", ex)
            Return 0

        End Try
    End Function

End Class
