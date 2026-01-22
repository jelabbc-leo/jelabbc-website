Imports System.Data
Imports System.Collections.Generic
Imports JelaWeb.Utilities
Imports JelaWeb.Infrastructure.Helpers

''' <summary>
''' Servicio para gestión de Visitantes
''' </summary>
Public Class VisitantesService

    Private Const TABLA_VISITANTES As String = "op_visitantes"
    Private Const TABLA_ENTIDADES As String = "cat_entidades"
    Private Const TABLA_UNIDADES As String = "cat_unidades"
    Private Const TABLA_RESIDENTES As String = "cat_residentes"

    ''' <summary>
    ''' Obtiene un visitante por ID
    ''' </summary>
    <System.Web.Services.WebMethod()>
    Public Shared Function ObtenerVisitante(id As Integer) As Object

        Try
            Dim registro As DataRow = DynamicCrudService.ObtenerPorId(TABLA_VISITANTES, id)

            If registro Is Nothing Then Return New With {.success = False, .message = "No encontrado"}

            Dim datos As New Dictionary(Of String, Object)

            For Each col As DataColumn In registro.Table.Columns
                datos(col.ColumnName) = If(IsDBNull(registro(col)), Nothing, registro(col))

            Next

            Return New With {.success = True, .data = datos}

        Catch ex As Exception
            Logger.LogError("VisitantesService.ObtenerVisitante", ex)
            Return New With {.success = False, .message = "Error: " & ex.Message}

        End Try
    End Function

    ''' <summary>
    ''' Guarda un visitante (crea o actualiza)
    ''' </summary>
    <System.Web.Services.WebMethod()>
    Public Shared Function GuardarVisitante(datos As Dictionary(Of String, Object)) As Object

        Try
            Dim id As Integer = Convert.ToInt32(datos("id"))
            Dim datosGuardar As New Dictionary(Of String, Object)
            datosGuardar.Add("EntidadId", Convert.ToInt32(datos("entidadId")))
            datosGuardar.Add("UnidadId", Convert.ToInt32(datos("unidadId")))
            datosGuardar.Add("ResidenteId", If(datos("residenteId") IsNot Nothing AndAlso Convert.ToInt32(datos("residenteId")) > 0, Convert.ToInt32(datos("residenteId")), DBNull.Value))
            datosGuardar.Add("NombreVisitante", datos("nombreVisitante")?.ToString())
            datosGuardar.Add("TipoIdentificacion", datos("tipoIdentificacion")?.ToString())
            datosGuardar.Add("NumeroIdentificacion", datos("numeroIdentificacion")?.ToString())
            datosGuardar.Add("Vehiculo", datos("vehiculo")?.ToString())
            datosGuardar.Add("ColorVehiculo", datos("colorVehiculo")?.ToString())
            datosGuardar.Add("MarcaVehiculo", datos("marcaVehiculo")?.ToString())
            datosGuardar.Add("MotivoVisita", datos("motivoVisita")?.ToString())
            datosGuardar.Add("TipoVisita", datos("tipoVisita")?.ToString())
            datosGuardar.Add("FechaEntrada", Convert.ToDateTime(datos("fechaEntrada")))
            datosGuardar.Add("FechaSalida", If(datos("fechaSalida") IsNot Nothing, Convert.ToDateTime(datos("fechaSalida")), DBNull.Value))
            datosGuardar.Add("Estado", datos("estado")?.ToString())
            datosGuardar.Add("AutorizadoPor", datos("autorizadoPor")?.ToString())
            datosGuardar.Add("Observaciones", datos("observaciones")?.ToString())

            Dim resultado As Boolean

            If id = 0 Then
                datosGuardar("Folio") = "VIS-" & DateTime.Now.ToString("yyyyMMdd") & "-" & Guid.NewGuid().ToString("N").Substring(0, 4).ToUpper()
                resultado = DynamicCrudService.Insertar(TABLA_VISITANTES, datosGuardar)
            Else
                resultado = DynamicCrudService.Actualizar(TABLA_VISITANTES, id, datosGuardar)
            End If

            Return New With {.success = resultado, .message = If(resultado, "Guardado correctamente", "Error al guardar")}

        Catch ex As Exception
            Logger.LogError("VisitantesService.GuardarVisitante", ex)
            Return New With {.success = False, .message = "Error: " & ex.Message}

        End Try
    End Function

    ''' <summary>
    ''' Elimina un visitante
    ''' </summary>
    <System.Web.Services.WebMethod()>
    Public Shared Function EliminarVisitante(id As Integer) As Object

        Try
            Dim resultado = DynamicCrudService.Eliminar(TABLA_VISITANTES, id)

            Return New With {.success = resultado, .message = If(resultado, "Eliminado", "Error")}

        Catch ex As Exception
            Logger.LogError("VisitantesService.EliminarVisitante", ex)
            Return New With {.success = False, .message = "Error: " & ex.Message}

        End Try
    End Function

    ''' <summary>
    ''' Registra la salida de un visitante
    ''' </summary>
    <System.Web.Services.WebMethod()>
    Public Shared Function RegistrarSalida(id As Integer) As Object

        Try
            Dim registro As DataRow = DynamicCrudService.ObtenerPorId(TABLA_VISITANTES, id)

            If registro Is Nothing Then Return New With {.success = False, .message = "No encontrado"}

            Dim datos As New Dictionary(Of String, Object)
            datos.Add("FechaSalida", DateTime.Now)
            datos.Add("Estado", "Salida")

            Dim resultado = DynamicCrudService.Actualizar(TABLA_VISITANTES, id, datos)

            Return New With {.success = resultado, .message = If(resultado, "Salida registrada", "Error")}

        Catch ex As Exception
            Logger.LogError("VisitantesService.RegistrarSalida", ex)
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
            Return New With {.success = False, .message = "Error: " & ex.Message}

        End Try
    End Function

    ''' <summary>
    ''' Lista todos los visitantes con información relacionada
    ''' </summary>
    Public Shared Function ListarVisitantes(fechaDesde As DateTime, fechaHasta As DateTime) As DataTable

        Try
            Dim query As String = "SELECT v.*, " &
                "e.RazonSocial AS EntidadNombre, " &
                "u.Nombre AS UnidadNombre, " &
                "u.Codigo AS UnidadCodigo, " &
                "CONCAT(r.Nombre, ' ', r.ApellidoPaterno) AS ResidenteNombre " &
                "FROM " & TABLA_VISITANTES & " v " &
                "LEFT JOIN cat_entidades e ON v.EntidadId = e.Id " &
                "LEFT JOIN " & TABLA_UNIDADES & " u ON v.UnidadId = u.Id " &
                "LEFT JOIN " & TABLA_RESIDENTES & " r ON v.ResidenteId = r.Id " &
                "WHERE DATE(v.FechaEntrada) >= '" & fechaDesde.ToString("yyyy-MM-dd") & "' " &
                "AND DATE(v.FechaEntrada) <= '" & fechaHasta.ToString("yyyy-MM-dd") & "' " &
                "ORDER BY v.FechaEntrada DESC"

            Return DynamicCrudService.EjecutarConsulta(query)

        Catch ex As Exception
            Logger.LogError("VisitantesService.ListarVisitantes", ex)
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
            Logger.LogError("VisitantesService.ListarEntidades", ex)
            Return New DataTable()

        End Try
    End Function

End Class
