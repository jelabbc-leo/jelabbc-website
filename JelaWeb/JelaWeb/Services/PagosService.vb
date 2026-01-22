Imports System.Data
Imports System.Collections.Generic
Imports JelaWeb.Utilities
Imports JelaWeb.Infrastructure.Helpers

''' <summary>
''' Servicio para gestión de Pagos
''' </summary>
Public Class PagosService

    Private Const TABLA_PAGOS As String = "op_pagos"
    Private Const TABLA_PAGOS_DETALLE As String = "op_pagos_detalle"
    Private Const TABLA_CUOTAS As String = "op_cuotas"
    Private Const TABLA_ENTIDADES As String = "cat_entidades"

    ''' <summary>
    ''' Lista las unidades de una entidad
    ''' </summary>
    <System.Web.Services.WebMethod()>
    Public Shared Function ListarUnidades(entidadId As Integer) As Object

        Try
            Dim query As String = "SELECT Id, CONCAT(Codigo, ' - ', Nombre) AS NombreCompleto " &
                                  "FROM cat_unidades " &
                                  "WHERE EntidadId = " & entidadId & " AND Activo = 1 " &
                                  "ORDER BY Codigo, Nombre"
            Dim dt As DataTable = DynamicCrudService.EjecutarConsulta(query)

            Return New With {.success = True, .data = DataTableHelper.ConvertDataTableToList(dt)}

        Catch ex As Exception
            Logger.LogError("PagosService.ListarUnidades", ex)
            Return New With {.success = False, .message = "Error: " & ex.Message}

        End Try
    End Function

    ''' <summary>
    ''' Lista los residentes de una unidad
    ''' </summary>
    <System.Web.Services.WebMethod()>
    Public Shared Function ListarResidentes(unidadId As Integer) As Object

        Try
            Dim query As String = "SELECT Id, NombreCompleto " &
                                  "FROM cat_residentes " &
                                  "WHERE UnidadId = " & unidadId & " AND Activo = 1 " &
                                  "ORDER BY NombreCompleto"
            Dim dt As DataTable = DynamicCrudService.EjecutarConsulta(query)

            Return New With {.success = True, .data = DataTableHelper.ConvertDataTableToList(dt)}

        Catch ex As Exception
            Logger.LogError("PagosService.ListarResidentes", ex)
            Return New With {.success = False, .message = "Error: " & ex.Message}

        End Try
    End Function

    ''' <summary>
    ''' Obtiene las cuotas pendientes de una unidad
    ''' </summary>
    <System.Web.Services.WebMethod()>
    Public Shared Function ObtenerCuotasPendientes(unidadId As Integer) As Object

        Try
            Dim query As String = "SELECT c.*, " &
                "cc.Nombre AS ConceptoNombre " &
                "FROM op_cuotas c " &
                "LEFT JOIN cat_conceptos_cuota cc ON c.ConceptoCuotaId = cc.Id " &
                "WHERE c.UnidadId = " & unidadId &
                " AND c.Saldo > 0 " &
                "ORDER BY c.FechaVencimiento ASC"
            Dim dt As DataTable = DynamicCrudService.EjecutarConsulta(query)

            Return New With {.success = True, .data = DataTableHelper.ConvertDataTableToList(dt)}

        Catch ex As Exception
            Logger.LogError("PagosService.ObtenerCuotasPendientes", ex)
            Return New With {.success = False, .message = "Error: " & ex.Message}

        End Try
    End Function

    ''' <summary>
    ''' Obtiene un pago por ID
    ''' </summary>
    <System.Web.Services.WebMethod()>
    Public Shared Function ObtenerPago(id As Integer) As Object

        Try
            Dim registro As DataRow = DynamicCrudService.ObtenerPorId(TABLA_PAGOS, id)

            If registro Is Nothing Then Return New With {.success = False, .message = "No encontrado"}
            Dim datos As New Dictionary(Of String, Object)

            For Each col As DataColumn In registro.Table.Columns
                datos(col.ColumnName) = If(IsDBNull(registro(col)), Nothing, registro(col))

            Next

            Return New With {.success = True, .data = datos}

        Catch ex As Exception
            Logger.LogError("PagosService.ObtenerPago", ex)
            Return New With {.success = False, .message = "Error: " & ex.Message}

        End Try
    End Function

    ''' <summary>
    ''' Guarda un pago (crea o actualiza)
    ''' </summary>
    <System.Web.Services.WebMethod()>
    Public Shared Function GuardarPago(datos As Dictionary(Of String, Object)) As Object

        Try
            Dim id As Integer = Convert.ToInt32(datos("id"))
            Dim datosGuardar As New Dictionary(Of String, Object)
            datosGuardar.Add("EntidadId", Convert.ToInt32(datos("entidadId")))
            datosGuardar.Add("UnidadId", Convert.ToInt32(datos("unidadId")))
            datosGuardar.Add("ResidenteId", If(datos("residenteId") IsNot Nothing AndAlso Convert.ToInt32(datos("residenteId")) > 0, Convert.ToInt32(datos("residenteId")), DBNull.Value))
            datosGuardar.Add("Monto", Convert.ToDecimal(datos("monto")))
            datosGuardar.Add("FormaPago", datos("formaPago")?.ToString())
            datosGuardar.Add("Referencia", datos("referencia")?.ToString())
            datosGuardar.Add("Banco", datos("banco")?.ToString())
            datosGuardar.Add("FechaPago", Convert.ToDateTime(datos("fechaPago")))
            datosGuardar.Add("Estado", datos("estado")?.ToString())
            datosGuardar.Add("Observaciones", datos("observaciones")?.ToString())

            ' Si es nuevo, generar folio
            If id = 0 Then
                Dim folio As String = "PAG-" & DateTime.Now.ToString("yyyyMMdd") & "-" & Guid.NewGuid().ToString("N").Substring(0, 4).ToUpper()

                datosGuardar.Add("Folio", folio)
            End If

            Dim resultado As Boolean
            Dim nuevoId As Integer = 0

            If id = 0 Then
                resultado = DynamicCrudService.Insertar(TABLA_PAGOS, datosGuardar)
                If resultado Then
                    Dim dt As DataTable = DynamicCrudService.EjecutarConsulta("SELECT LAST_INSERT_ID() AS Id")

                    If dt.Rows.Count > 0 Then nuevoId = Convert.ToInt32(dt.Rows(0)("Id"))
                End If
            Else
                resultado = DynamicCrudService.Actualizar(TABLA_PAGOS, id, datosGuardar)
                nuevoId = id
            End If

            ' Si hay aplicaciones de cuotas, guardarlas
            If resultado AndAlso datos.ContainsKey("aplicaciones") Then
                Dim aplicaciones = datos("aplicaciones")

                If aplicaciones IsNot Nothing Then
                    GuardarAplicacionesCuotas(nuevoId, aplicaciones)
                End If
            End If

            Return New With {.success = resultado, .message = If(resultado, "Guardado correctamente", "Error al guardar"), .id = nuevoId}

        Catch ex As Exception
            Logger.LogError("PagosService.GuardarPago", ex)
            Return New With {.success = False, .message = "Error: " & ex.Message}

        End Try
    End Function

    ''' <summary>
    ''' Guarda las aplicaciones de pago a cuotas
    ''' </summary>
    Private Shared Sub GuardarAplicacionesCuotas(pagoId As Integer, aplicaciones As Object)

        Try
            ' Primero eliminar aplicaciones existentes de este pago
            Dim queryDelete As String = "DELETE FROM " & TABLA_PAGOS_DETALLE & " WHERE PagoId = " & pagoId

            DynamicCrudService.EjecutarConsulta(queryDelete)

            ' Insertar nuevas aplicaciones
            If TypeOf aplicaciones Is List(Of Dictionary(Of String, Object)) Then

                For Each aplicacion As Dictionary(Of String, Object) In aplicaciones
                    Dim datosDetalle As New Dictionary(Of String, Object)
                    datosDetalle.Add("PagoId", pagoId)
                    datosDetalle.Add("CuotaId", Convert.ToInt32(aplicacion("cuotaId")))
                    datosDetalle.Add("MontoAplicado", Convert.ToDecimal(aplicacion("montoAplicado")))
                    DynamicCrudService.Insertar(TABLA_PAGOS_DETALLE, datosDetalle)

                Next

            End If

        Catch ex As Exception
            Logger.LogError("PagosService.GuardarAplicacionesCuotas", ex)
            Throw

        End Try
    End Sub

    ''' <summary>
    ''' Aplica un pago a múltiples cuotas
    ''' </summary>
    <System.Web.Services.WebMethod()>
    Public Shared Function AplicarPagoACuotas(pagoId As Integer, aplicaciones As List(Of Dictionary(Of String, Object))) As Object

        Try
            GuardarAplicacionesCuotas(pagoId, aplicaciones)

            ' Recalcular montos pagados en las cuotas

            For Each aplicacion As Dictionary(Of String, Object) In aplicaciones
                Dim cuotaId As Integer = Convert.ToInt32(aplicacion("cuotaId"))
                Dim montoAplicado As Decimal = Convert.ToDecimal(aplicacion("montoAplicado"))

                ' Obtener cuota actual
                Dim registro As DataRow = DynamicCrudService.ObtenerPorId("op_cuotas", cuotaId)

                If registro IsNot Nothing Then
                    Dim montoPagadoActual As Decimal = If(IsDBNull(registro("MontoPagado")), 0, Convert.ToDecimal(registro("MontoPagado")))
                    Dim nuevoMontoPagado As Decimal = montoPagadoActual + montoAplicado

                    ' Actualizar monto pagado
                    Dim datosUpdate As New Dictionary(Of String, Object)
                    datosUpdate.Add("MontoPagado", nuevoMontoPagado)

                    ' Actualizar estado si está completamente pagada
                    Dim montoTotal As Decimal = Convert.ToDecimal(registro("MontoTotal"))

                    If nuevoMontoPagado >= montoTotal Then
                        datosUpdate.Add("Estado", "Pagada")
                    ElseIf nuevoMontoPagado > 0 Then
                        datosUpdate.Add("Estado", "Parcial")
                    End If

                    DynamicCrudService.Actualizar("op_cuotas", cuotaId, datosUpdate)
                End If

            Next

            Return New With {.success = True, .message = "Pago aplicado correctamente a las cuotas"}

        Catch ex As Exception
            Logger.LogError("PagosService.AplicarPagoACuotas", ex)
            Return New With {.success = False, .message = "Error: " & ex.Message}

        End Try
    End Function

    ''' <summary>
    ''' Elimina un pago
    ''' </summary>
    <System.Web.Services.WebMethod()>
    Public Shared Function EliminarPago(id As Integer) As Object

        Try
            ' Primero eliminar aplicaciones
            Dim queryDelete As String = "DELETE FROM " & TABLA_PAGOS_DETALLE & " WHERE PagoId = " & id

            DynamicCrudService.EjecutarConsulta(queryDelete)

            ' Luego eliminar el pago
            Dim resultado = DynamicCrudService.Eliminar(TABLA_PAGOS, id)

            Return New With {.success = resultado, .message = If(resultado, "Eliminado", "Error")}

        Catch ex As Exception
            Logger.LogError("PagosService.EliminarPago", ex)
            Return New With {.success = False, .message = "Error: " & ex.Message}

        End Try
    End Function

    ''' <summary>
    ''' Lista todos los pagos con información relacionada
    ''' </summary>
    Public Shared Function ListarPagos(fechaDesde As DateTime, fechaHasta As DateTime) As DataTable

        Try
            Dim query As String = "SELECT p.*, " &
                "e.RazonSocial AS EntidadNombre, " &
                "u.Nombre AS UnidadNombre, " &
                "u.Codigo AS UnidadCodigo, " &
                "r.NombreCompleto AS ResidenteNombre " &
                "FROM " & TABLA_PAGOS & " p " &
                "LEFT JOIN cat_entidades e ON p.EntidadId = e.Id " &
                "LEFT JOIN cat_unidades u ON p.UnidadId = u.Id " &
                "LEFT JOIN cat_residentes r ON p.ResidenteId = r.Id " &
                "WHERE p.FechaPago >= '" & fechaDesde.ToString("yyyy-MM-dd") & "' " &
                "AND p.FechaPago <= '" & fechaHasta.ToString("yyyy-MM-dd") & "' " &
                "ORDER BY p.FechaPago DESC, p.Folio DESC"

            Return DynamicCrudService.EjecutarConsulta(query)

        Catch ex As Exception
            Logger.LogError("PagosService.ListarPagos", ex)
            Return New DataTable()

        End Try
    End Function

    ''' <summary>
    ''' Lista cuotas pendientes de una unidad para el grid
    ''' </summary>
    Public Shared Function ListarCuotasPendientesGrid(unidadId As Integer) As DataTable

        Try
            Dim query As String = "SELECT c.*, " &
                "cc.Nombre AS ConceptoNombre, " &
                "(c.MontoTotal - IFNULL(c.MontoPagado, 0)) AS Saldo " &
                "FROM " & TABLA_CUOTAS & " c " &
                "LEFT JOIN cat_conceptos_cuota cc ON c.ConceptoCuotaId = cc.Id " &
                "WHERE c.UnidadId = " & unidadId &
                " AND (c.MontoTotal - IFNULL(c.MontoPagado, 0)) > 0 " &
                "ORDER BY c.FechaVencimiento ASC"

            Return DynamicCrudService.EjecutarConsulta(query)

        Catch ex As Exception
            Logger.LogError("PagosService.ListarCuotasPendientesGrid", ex)
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
            Logger.LogError("PagosService.ListarEntidades", ex)
            Return New DataTable()

        End Try
    End Function

End Class
