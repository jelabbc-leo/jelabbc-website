Imports System.Data
Imports System.Collections.Generic
Imports JelaWeb.Utilities
Imports JelaWeb.Infrastructure.Helpers

''' <summary>
''' Servicio para gestión de Residentes
''' </summary>
Public Class ResidentesService

    Private Const TABLA_RESIDENTES As String = "cat_residentes"
    Private Const TABLA_SUB_ENTIDADES As String = "cat_sub_entidades"
    Private Const TABLA_UNIDADES As String = "cat_unidades"
    Private Const TABLA_VEHICULOS As String = "cat_vehiculos_residentes"
    Private Const TABLA_TAGS As String = "cat_tags_acceso"
    Private Const TABLA_ENTIDADES As String = "cat_entidades"

    ''' <summary>
    ''' Obtiene un residente por ID
    ''' </summary>
    <System.Web.Services.WebMethod()>
    Public Shared Function ObtenerResidente(id As Integer) As Object

        Try
            Dim registro As DataRow = DynamicCrudService.ObtenerPorId(TABLA_RESIDENTES, id)

            If registro Is Nothing Then
                Return New With {.success = False, .message = "Residente no encontrado"}
            End If

            Dim datos As New Dictionary(Of String, Object)

            For Each col As DataColumn In registro.Table.Columns
                datos(col.ColumnName) = If(IsDBNull(registro(col)), Nothing, registro(col))

            Next

            Return New With {.success = True, .data = datos}

        Catch ex As Exception
            Logger.LogError("ResidentesService.ObtenerResidente", ex)
            Return New With {.success = False, .message = "Error al obtener residente"}

        End Try
    End Function

    ''' <summary>
    ''' Elimina un residente
    ''' </summary>
    <System.Web.Services.WebMethod()>
    Public Shared Function EliminarResidente(id As Integer) As Object

        Try
            Dim resultado As Boolean = DynamicCrudService.Eliminar(TABLA_RESIDENTES, id)

            If resultado Then
                Logger.LogInfo($"Residente eliminado: Id={id}")
                Return New With {.success = True, .message = "Residente eliminado correctamente"}
            Else
                Return New With {.success = False, .message = "Error al eliminar residente"}
            End If

        Catch ex As Exception
            Logger.LogError("ResidentesService.EliminarResidente", ex)
            Return New With {.success = False, .message = "Error al eliminar residente"}

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
            Logger.LogError("ResidentesService.ObtenerSubEntidadesPorEntidad", ex)
            Return New With {.success = False, .message = "Error al obtener sub-entidades"}

        End Try
    End Function

    ''' <summary>
    ''' Obtiene las unidades por sub-entidad
    ''' </summary>
    <System.Web.Services.WebMethod()>
    Public Shared Function ObtenerUnidadesPorSubEntidad(subEntidadId As Integer) As Object

        Try
            Dim whereClause As String = If(subEntidadId > 0, $"SubEntidadId = {subEntidadId} AND Activo = 1", "Activo = 1")
            Dim dt As DataTable = DynamicCrudService.ObtenerTodosConFiltro(TABLA_UNIDADES, whereClause)
            Dim lista As New List(Of Object)

            For Each row As DataRow In dt.Rows
                lista.Add(New With {
                    .Id = row("Id"),
                    .Nombre = row("Nombre")?.ToString()
                })

            Next

            Return New With {.success = True, .data = lista}

        Catch ex As Exception
            Logger.LogError("ResidentesService.ObtenerUnidadesPorSubEntidad", ex)
            Return New With {.success = False, .message = "Error al obtener unidades"}

        End Try
    End Function

    ''' <summary>
    ''' Obtiene un vehículo por ID
    ''' </summary>
    <System.Web.Services.WebMethod()>
    Public Shared Function ObtenerVehiculo(id As Integer) As Object

        Try
            Dim registro As DataRow = DynamicCrudService.ObtenerPorId(TABLA_VEHICULOS, id)

            If registro Is Nothing Then
                Return New With {.success = False, .message = "Vehículo no encontrado"}
            End If

            Dim datos As New Dictionary(Of String, Object)

            For Each col As DataColumn In registro.Table.Columns
                datos(col.ColumnName) = If(IsDBNull(registro(col)), Nothing, registro(col))

            Next

            Return New With {.success = True, .data = datos}

        Catch ex As Exception
            Logger.LogError("ResidentesService.ObtenerVehiculo", ex)
            Return New With {.success = False, .message = "Error al obtener vehículo"}

        End Try
    End Function

    ''' <summary>
    ''' Guarda un vehículo (crea o actualiza)
    ''' </summary>
    <System.Web.Services.WebMethod()>
    Public Shared Function GuardarVehiculo(datos As Dictionary(Of String, Object)) As Object

        Try
            Dim vehiculoId As Integer = Convert.ToInt32(datos("id"))
            Dim datosGuardar As New Dictionary(Of String, Object)
            datosGuardar.Add("ResidenteId", Convert.ToInt32(datos("residenteId")))
            datosGuardar.Add("Placas", datos("placas")?.ToString())
            datosGuardar.Add("TipoVehiculo", datos("tipoVehiculo")?.ToString())
            datosGuardar.Add("Marca", datos("marca")?.ToString())
            datosGuardar.Add("Modelo", datos("modelo")?.ToString())
            datosGuardar.Add("Anio", If(datos("anio") IsNot Nothing, Convert.ToInt32(datos("anio")), DBNull.Value))
            datosGuardar.Add("Color", datos("color")?.ToString())
            datosGuardar.Add("NumeroTarjeton", datos("numeroTarjeton")?.ToString())
            datosGuardar.Add("EsPrincipal", If(Convert.ToBoolean(datos("esPrincipal")), 1, 0))
            datosGuardar.Add("Observaciones", datos("observaciones")?.ToString())
            datosGuardar.Add("Activo", If(Convert.ToBoolean(datos("activo")), 1, 0))

            Dim resultado As Boolean

            If vehiculoId = 0 Then
                resultado = DynamicCrudService.Insertar(TABLA_VEHICULOS, datosGuardar)
            Else
                resultado = DynamicCrudService.Actualizar(TABLA_VEHICULOS, vehiculoId, datosGuardar)
            End If

            If resultado Then
                Return New With {.success = True, .message = "Vehículo guardado correctamente"}
            Else
                Return New With {.success = False, .message = "Error al guardar vehículo"}
            End If

        Catch ex As Exception
            Logger.LogError("ResidentesService.GuardarVehiculo", ex)
            Return New With {.success = False, .message = "Error al guardar vehículo: " & ex.Message}

        End Try
    End Function

    ''' <summary>
    ''' Elimina un vehículo
    ''' </summary>
    <System.Web.Services.WebMethod()>
    Public Shared Function EliminarVehiculo(id As Integer) As Object

        Try
            Dim resultado As Boolean = DynamicCrudService.Eliminar(TABLA_VEHICULOS, id)

            If resultado Then
                Return New With {.success = True, .message = "Vehículo eliminado correctamente"}
            Else
                Return New With {.success = False, .message = "Error al eliminar vehículo"}
            End If

        Catch ex As Exception
            Logger.LogError("ResidentesService.EliminarVehiculo", ex)
            Return New With {.success = False, .message = "Error al eliminar vehículo"}

        End Try
    End Function

    ''' <summary>
    ''' Obtiene un tag de acceso por ID
    ''' </summary>
    <System.Web.Services.WebMethod()>
    Public Shared Function ObtenerTag(id As Integer) As Object

        Try
            Dim registro As DataRow = DynamicCrudService.ObtenerPorId(TABLA_TAGS, id)

            If registro Is Nothing Then
                Return New With {.success = False, .message = "Tag no encontrado"}
            End If

            Dim datos As New Dictionary(Of String, Object)

            For Each col As DataColumn In registro.Table.Columns
                datos(col.ColumnName) = If(IsDBNull(registro(col)), Nothing, registro(col))

            Next

            Return New With {.success = True, .data = datos}

        Catch ex As Exception
            Logger.LogError("ResidentesService.ObtenerTag", ex)
            Return New With {.success = False, .message = "Error al obtener tag"}

        End Try
    End Function

    ''' <summary>
    ''' Guarda un tag de acceso (crea o actualiza)
    ''' </summary>
    <System.Web.Services.WebMethod()>
    Public Shared Function GuardarTag(datos As Dictionary(Of String, Object)) As Object

        Try
            Dim tagId As Integer = Convert.ToInt32(datos("id"))
            Dim datosGuardar As New Dictionary(Of String, Object)
            datosGuardar.Add("ResidenteId", Convert.ToInt32(datos("residenteId")))
            datosGuardar.Add("CodigoTag", datos("codigoTag")?.ToString())
            datosGuardar.Add("TipoTag", datos("tipoTag")?.ToString())
            datosGuardar.Add("Descripcion", datos("descripcion")?.ToString())
            datosGuardar.Add("FechaAsignacion", If(datos("fechaAsignacion") IsNot Nothing, Convert.ToDateTime(datos("fechaAsignacion")), DBNull.Value))
            datosGuardar.Add("FechaVencimiento", If(datos("fechaVencimiento") IsNot Nothing, Convert.ToDateTime(datos("fechaVencimiento")), DBNull.Value))
            datosGuardar.Add("PermiteAccesoPeatonal", If(Convert.ToBoolean(datos("permiteAccesoPeatonal")), 1, 0))
            datosGuardar.Add("PermiteAccesoVehicular", If(Convert.ToBoolean(datos("permiteAccesoVehicular")), 1, 0))
            datosGuardar.Add("PermiteAccesoAreas", If(Convert.ToBoolean(datos("permiteAccesoAreas")), 1, 0))
            datosGuardar.Add("EsPrincipal", If(Convert.ToBoolean(datos("esPrincipal")), 1, 0))
            datosGuardar.Add("Observaciones", datos("observaciones")?.ToString())
            datosGuardar.Add("Activo", If(Convert.ToBoolean(datos("activo")), 1, 0))

            Dim resultado As Boolean

            If tagId = 0 Then
                resultado = DynamicCrudService.Insertar(TABLA_TAGS, datosGuardar)
            Else
                resultado = DynamicCrudService.Actualizar(TABLA_TAGS, tagId, datosGuardar)
            End If

            If resultado Then
                Return New With {.success = True, .message = "Tag guardado correctamente"}
            Else
                Return New With {.success = False, .message = "Error al guardar tag"}
            End If

        Catch ex As Exception
            Logger.LogError("ResidentesService.GuardarTag", ex)
            Return New With {.success = False, .message = "Error al guardar tag: " & ex.Message}

        End Try
    End Function

    ''' <summary>
    ''' Elimina un tag de acceso
    ''' </summary>
    <System.Web.Services.WebMethod()>
    Public Shared Function EliminarTag(id As Integer) As Object

        Try
            Dim resultado As Boolean = DynamicCrudService.Eliminar(TABLA_TAGS, id)

            If resultado Then
                Return New With {.success = True, .message = "Tag eliminado correctamente"}
            Else
                Return New With {.success = False, .message = "Error al eliminar tag"}
            End If

        Catch ex As Exception
            Logger.LogError("ResidentesService.EliminarTag", ex)
            Return New With {.success = False, .message = "Error al eliminar tag"}

        End Try
    End Function

    ''' <summary>
    ''' Lista todos los residentes con información de entidad, sub-entidad y unidad
    ''' </summary>
    Public Shared Function ListarResidentes() As DataTable

        Try
            Dim query As String = "SELECT r.*, " &
                "e.RazonSocial AS EntidadNombre, " &
                "se.RazonSocial AS SubEntidadNombre, " &
                "u.Nombre AS UnidadNombre " &
                "FROM " & TABLA_RESIDENTES & " r " &
                "LEFT JOIN cat_entidades e ON r.EntidadId = e.Id " &
                "LEFT JOIN " & TABLA_SUB_ENTIDADES & " se ON r.SubEntidadId = se.Id " &
                "LEFT JOIN " & TABLA_UNIDADES & " u ON r.UnidadId = u.Id " &
                "ORDER BY r.NombreCompleto"

            Return DynamicCrudService.EjecutarConsulta(query)

        Catch ex As Exception
            Logger.LogError("ResidentesService.ListarResidentes", ex)
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
            Logger.LogError("ResidentesService.ListarEntidades", ex)
            Return New DataTable()

        End Try
    End Function

    ''' <summary>
    ''' Guarda un residente (crea o actualiza)
    ''' </summary>
    Public Shared Function GuardarResidente(residenteId As Integer, datos As Dictionary(Of String, Object), userId As Integer?) As Boolean

        Try
            If residenteId = 0 Then
                If userId.HasValue Then
                    datos("CreadoPor") = userId.Value
                End If

                Return DynamicCrudService.Insertar(TABLA_RESIDENTES, datos)
            Else
                If userId.HasValue Then
                    datos("ModificadoPor") = userId.Value
                End If

                Return DynamicCrudService.Actualizar(TABLA_RESIDENTES, residenteId, datos)
            End If

        Catch ex As Exception
            Logger.LogError("ResidentesService.GuardarResidente", ex)
            Return False

        End Try
    End Function

End Class
