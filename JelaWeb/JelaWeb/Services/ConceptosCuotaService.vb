Imports System.Data
Imports System.Collections.Generic
Imports JelaWeb.Utilities

''' <summary>
''' Servicio para gestión de Conceptos de Cuota
''' </summary>
Public Class ConceptosCuotaService

    Private Const TABLA_CONCEPTOS_CUOTA As String = "cat_conceptos_cuota"
    Private Const TABLA_ENTIDADES As String = "cat_entidades"

    ''' <summary>
    ''' Obtiene un concepto de cuota por ID
    ''' </summary>
    <System.Web.Services.WebMethod()>
    Public Shared Function ObtenerConcepto(id As Integer) As Object

        Try
            Dim registro As DataRow = DynamicCrudService.ObtenerPorId(TABLA_CONCEPTOS_CUOTA, id)

            If registro Is Nothing Then
                Return New With {.success = False, .message = "Concepto no encontrado"}
            End If

            Dim datos As New Dictionary(Of String, Object)

            For Each col As DataColumn In registro.Table.Columns
                datos(col.ColumnName) = If(IsDBNull(registro(col)), Nothing, registro(col))

            Next

            Return New With {.success = True, .data = datos}

        Catch ex As Exception
            Logger.LogError("ConceptosCuotaService.ObtenerConcepto", ex)
            Return New With {.success = False, .message = "Error al obtener concepto"}

        End Try
    End Function

    ''' <summary>
    ''' Elimina un concepto de cuota
    ''' </summary>
    <System.Web.Services.WebMethod()>
    Public Shared Function EliminarConcepto(id As Integer) As Object

        Try
            Dim resultado As Boolean = DynamicCrudService.Eliminar(TABLA_CONCEPTOS_CUOTA, id)

            If resultado Then
                Logger.LogInfo($"Concepto eliminado: Id={id}")
                Return New With {.success = True, .message = "Concepto eliminado correctamente"}
            Else
                Return New With {.success = False, .message = "Error al eliminar concepto"}
            End If

        Catch ex As Exception
            Logger.LogError("ConceptosCuotaService.EliminarConcepto", ex)
            Return New With {.success = False, .message = "Error al eliminar concepto"}

        End Try
    End Function

    ''' <summary>
    ''' Lista todos los conceptos de cuota con información de entidad
    ''' </summary>
    Public Shared Function ListarConceptos() As DataTable

        Try
            Dim query As String = "SELECT c.*, " &
                "e.RazonSocial AS EntidadNombre " &
                "FROM " & TABLA_CONCEPTOS_CUOTA & " c " &
                "LEFT JOIN cat_entidades e ON c.EntidadId = e.Id " &
                "ORDER BY c.Nombre"

            Return DynamicCrudService.EjecutarConsulta(query)

        Catch ex As Exception
            Logger.LogError("ConceptosCuotaService.ListarConceptos", ex)
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
            Logger.LogError("ConceptosCuotaService.ListarEntidades", ex)
            Return New DataTable()

        End Try
    End Function

    ''' <summary>
    ''' Guarda un concepto de cuota (crea o actualiza)
    ''' </summary>
    Public Shared Function GuardarConcepto(conceptoId As Integer, datos As Dictionary(Of String, Object), userId As Integer?) As Boolean

        Try
            If conceptoId = 0 Then
                If userId.HasValue Then
                    datos("CreadoPor") = userId.Value
                End If

                Return DynamicCrudService.Insertar(TABLA_CONCEPTOS_CUOTA, datos)
            Else
                If userId.HasValue Then
                    datos("ModificadoPor") = userId.Value
                End If

                Return DynamicCrudService.Actualizar(TABLA_CONCEPTOS_CUOTA, conceptoId, datos)
            End If

        Catch ex As Exception
            Logger.LogError("ConceptosCuotaService.GuardarConcepto", ex)
            Return False

        End Try
    End Function

End Class
