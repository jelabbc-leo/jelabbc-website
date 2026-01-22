Imports System.Data
Imports System.Collections.Generic
Imports JelaWeb.Utilities
Imports JelaWeb.Infrastructure.Helpers

''' <summary>
''' Servicio para gestión de Reservaciones
''' </summary>
Public Class ReservacionesService

    Private Const TABLA_RESERVACIONES As String = "op_reservaciones"
    Private Const TABLA_AREAS_COMUNES As String = "cat_areas_comunes"
    Private Const TABLA_UNIDADES As String = "cat_unidades"
    Private Const TABLA_RESIDENTES As String = "cat_residentes"
    Private Const TABLA_ENTIDADES As String = "cat_entidades"

    ''' <summary>
    ''' Obtiene una reservación por ID
    ''' </summary>
    <System.Web.Services.WebMethod()>
    Public Shared Function ObtenerReservacion(id As Integer) As Object

        Try
            Dim registro As DataRow = DynamicCrudService.ObtenerPorId(TABLA_RESERVACIONES, id)

            If registro Is Nothing Then Return New With {.success = False, .message = "No encontrado"}

            Dim datos As New Dictionary(Of String, Object)

            For Each col As DataColumn In registro.Table.Columns
                datos(col.ColumnName) = If(IsDBNull(registro(col)), Nothing, registro(col))

            Next

            Return New With {.success = True, .data = datos}

        Catch ex As Exception
            Logger.LogError("ReservacionesService.ObtenerReservacion", ex)
            Return New With {.success = False, .message = "Error: " & ex.Message}

        End Try
    End Function

    ''' <summary>
    ''' Guarda una reservación (crea o actualiza)
    ''' </summary>
    <System.Web.Services.WebMethod()>
    Public Shared Function GuardarReservacion(datos As Dictionary(Of String, Object)) As Object

        Try
            Dim id As Integer = Convert.ToInt32(datos("id"))
            Dim datosGuardar As New Dictionary(Of String, Object)
            datosGuardar.Add("EntidadId", Convert.ToInt32(datos("entidadId")))
            datosGuardar.Add("AreaComunId", Convert.ToInt32(datos("areaComunId")))
            datosGuardar.Add("UnidadId", Convert.ToInt32(datos("unidadId")))
            datosGuardar.Add("ResidenteId", If(datos("residenteId") IsNot Nothing AndAlso Convert.ToInt32(datos("residenteId")) > 0, Convert.ToInt32(datos("residenteId")), DBNull.Value))
            datosGuardar.Add("FechaReservacion", Convert.ToDateTime(datos("fechaReservacion")))
            datosGuardar.Add("HoraInicio", TimeSpan.Parse(datos("horaInicio").ToString()))
            datosGuardar.Add("HoraFin", TimeSpan.Parse(datos("horaFin").ToString()))
            datosGuardar.Add("NumeroInvitados", If(datos("numeroInvitados") IsNot Nothing, Convert.ToInt32(datos("numeroInvitados")), 0))
            datosGuardar.Add("Motivo", datos("motivo")?.ToString())
            datosGuardar.Add("CostoTotal", If(datos("costoTotal") IsNot Nothing, Convert.ToDecimal(datos("costoTotal")), 0))
            datosGuardar.Add("DepositoPagado", If(datos("depositoPagado") IsNot Nothing, Convert.ToDecimal(datos("depositoPagado")), 0))
            datosGuardar.Add("DepositoDevuelto", If(datos("depositoDevuelto") IsNot Nothing, Convert.ToDecimal(datos("depositoDevuelto")), 0))
            datosGuardar.Add("Estado", datos("estado")?.ToString())
            datosGuardar.Add("Observaciones", datos("observaciones")?.ToString())

            Dim resultado As Boolean

            If id = 0 Then
                ' Generar folio automático
                datosGuardar("Folio") = "RES-" & DateTime.Now.ToString("yyyyMMdd") & "-" & Guid.NewGuid().ToString("N").Substring(0, 4).ToUpper()
                resultado = DynamicCrudService.Insertar(TABLA_RESERVACIONES, datosGuardar)
            Else
                resultado = DynamicCrudService.Actualizar(TABLA_RESERVACIONES, id, datosGuardar)
            End If

            Return New With {.success = resultado, .message = If(resultado, "Guardado correctamente", "Error al guardar")}

        Catch ex As Exception
            Logger.LogError("ReservacionesService.GuardarReservacion", ex)
            Return New With {.success = False, .message = "Error: " & ex.Message}

        End Try
    End Function

    ''' <summary>
    ''' Elimina una reservación
    ''' </summary>
    <System.Web.Services.WebMethod()>
    Public Shared Function EliminarReservacion(id As Integer) As Object

        Try
            Dim resultado = DynamicCrudService.Eliminar(TABLA_RESERVACIONES, id)

            Return New With {.success = resultado, .message = If(resultado, "Eliminado", "Error")}

        Catch ex As Exception
            Logger.LogError("ReservacionesService.EliminarReservacion", ex)
            Return New With {.success = False, .message = "Error: " & ex.Message}

        End Try
    End Function

    ''' <summary>
    ''' Lista las áreas comunes de una entidad
    ''' </summary>
    <System.Web.Services.WebMethod()>
    Public Shared Function ListarAreasComunes(entidadId As Integer) As Object

        Try
            Dim query As String = "SELECT Id, Nombre " &
                                  "FROM " & TABLA_AREAS_COMUNES & " " &
                                  "WHERE EntidadId = " & entidadId & " AND Activo = 1 " &
                                  "ORDER BY Nombre"
            Dim dt As DataTable = DynamicCrudService.EjecutarConsulta(query)

            Return New With {.success = True, .data = DataTableHelper.ConvertDataTableToList(dt)}

        Catch ex As Exception
            Logger.LogError("ReservacionesService.ListarAreasComunes", ex)
            Return New With {.success = False, .message = "Error: " & ex.Message}

        End Try
    End Function

    ''' <summary>
    ''' Lista las unidades de una entidad
    ''' </summary>
    <System.Web.Services.WebMethod()>
    Public Shared Function ListarUnidades(entidadId As Integer) As Object

        Try
            Dim query As String = "SELECT Id, CONCAT(Codigo, ' - ', Nombre) AS NombreCompleto " &
                                  "FROM " & TABLA_UNIDADES & " " &
                                  "WHERE EntidadId = " & entidadId & " AND Activo = 1 " &
                                  "ORDER BY Codigo, Nombre"
            Dim dt As DataTable = DynamicCrudService.EjecutarConsulta(query)

            Return New With {.success = True, .data = DataTableHelper.ConvertDataTableToList(dt)}

        Catch ex As Exception
            Logger.LogError("ReservacionesService.ListarUnidades", ex)
            Return New With {.success = False, .message = "Error: " & ex.Message}

        End Try
    End Function

    ''' <summary>
    ''' Lista los residentes de una unidad
    ''' </summary>
    <System.Web.Services.WebMethod()>
    Public Shared Function ListarResidentes(unidadId As Integer) As Object

        Try
            Dim query As String = "SELECT Id, CONCAT(Nombre, ' ', ApellidoPaterno, ' ', IFNULL(ApellidoMaterno, '')) AS NombreCompleto " &
                                  "FROM " & TABLA_RESIDENTES & " " &
                                  "WHERE UnidadId = " & unidadId & " AND Activo = 1 " &
                                  "ORDER BY Nombre"
            Dim dt As DataTable = DynamicCrudService.EjecutarConsulta(query)

            Return New With {.success = True, .data = DataTableHelper.ConvertDataTableToList(dt)}

        Catch ex As Exception
            Logger.LogError("ReservacionesService.ListarResidentes", ex)
            Return New With {.success = False, .message = "Error: " & ex.Message}

        End Try
    End Function

    ''' <summary>
    ''' Lista reservaciones para el calendario
    ''' </summary>
    Public Shared Function ListarReservacionesCalendario(mes As Integer, anio As Integer, areaComunId As Integer) As DataTable

        Try
            Dim fechaInicio As New DateTime(anio, mes, 1)
            Dim fechaFin As DateTime = fechaInicio.AddMonths(1).AddDays(-1)
            Dim whereClause As String = "FechaReservacion >= '" & fechaInicio.ToString("yyyy-MM-dd") & "' " &
                                         "AND FechaReservacion <= '" & fechaFin.ToString("yyyy-MM-dd") & "'"

            If areaComunId > 0 Then
                whereClause &= " AND AreaComunId = " & areaComunId
            End If

            Dim query As String = "SELECT r.*, " &
                "a.Nombre AS AreaComunNombre, " &
                "u.Nombre AS UnidadNombre, " &
                "u.Codigo AS UnidadCodigo, " &
                "CONCAT(res.Nombre, ' ', res.ApellidoPaterno) AS ResidenteNombre " &
                "FROM " & TABLA_RESERVACIONES & " r " &
                "LEFT JOIN " & TABLA_AREAS_COMUNES & " a ON r.AreaComunId = a.Id " &
                "LEFT JOIN cat_unidades u ON r.UnidadId = u.Id " &
                "LEFT JOIN cat_residentes res ON r.ResidenteId = res.Id " &
                "WHERE " & whereClause & " " &
                "ORDER BY r.FechaReservacion, r.HoraInicio"

            Return DynamicCrudService.EjecutarConsulta(query)

        Catch ex As Exception
            Logger.LogError("ReservacionesService.ListarReservacionesCalendario", ex)
            Return New DataTable()

        End Try
    End Function

    ''' <summary>
    ''' Lista todas las reservaciones
    ''' </summary>
    Public Shared Function ListarReservaciones(entidadId As Integer, fechaDesde As DateTime?, fechaHasta As DateTime?) As DataTable

        Try
            Dim whereClause As String = "r.EntidadId = " & entidadId

            If fechaDesde.HasValue Then
                whereClause &= " AND r.FechaReservacion >= '" & fechaDesde.Value.ToString("yyyy-MM-dd") & "'"
            End If

            If fechaHasta.HasValue Then
                whereClause &= " AND r.FechaReservacion <= '" & fechaHasta.Value.ToString("yyyy-MM-dd") & "'"
            End If

            Dim query As String = "SELECT r.*, " &
                "a.Nombre AS AreaComunNombre, " &
                "u.Nombre AS UnidadNombre, " &
                "u.Codigo AS UnidadCodigo, " &
                "CONCAT(res.Nombre, ' ', res.ApellidoPaterno) AS ResidenteNombre " &
                "FROM " & TABLA_RESERVACIONES & " r " &
                "LEFT JOIN " & TABLA_AREAS_COMUNES & " a ON r.AreaComunId = a.Id " &
                "LEFT JOIN " & TABLA_UNIDADES & " u ON r.UnidadId = u.Id " &
                "LEFT JOIN " & TABLA_RESIDENTES & " res ON r.ResidenteId = res.Id " &
                "WHERE " & whereClause & " " &
                "ORDER BY r.FechaReservacion DESC, r.HoraInicio"

            Return DynamicCrudService.EjecutarConsulta(query)

        Catch ex As Exception
            Logger.LogError("ReservacionesService.ListarReservaciones", ex)
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
            Logger.LogError("ReservacionesService.ListarEntidades", ex)
            Return New DataTable()

        End Try
    End Function

    ''' <summary>
    ''' Lista las áreas comunes activas para combos
    ''' </summary>
    Public Shared Function ListarAreasComunes() As DataTable

        Try
            Return DynamicCrudService.ObtenerTodosConFiltro(TABLA_AREAS_COMUNES, "Activo = 1")

        Catch ex As Exception
            Logger.LogError("ReservacionesService.ListarAreasComunes", ex)
            Return New DataTable()

        End Try
    End Function

End Class
