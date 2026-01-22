Imports System.Data
Imports System.Collections.Generic
Imports JelaWeb.Utilities
Imports JelaWeb.Infrastructure.Helpers

''' <summary>
''' Servicio para gestión de Estado de Cuenta
''' </summary>
Public Class EstadoCuentaService

    Private Const TABLA_ENTIDADES As String = "cat_entidades"
    Private Const TABLA_UNIDADES As String = "cat_unidades"
    Private Const TABLA_CUOTAS As String = "op_cuotas"
    Private Const TABLA_PAGOS As String = "op_pagos"

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
            Logger.LogError("EstadoCuentaService.ListarUnidades", ex)
            Return New With {.success = False, .message = "Error: " & ex.Message}

        End Try
    End Function

    ''' <summary>
    ''' Obtiene el estado de cuenta de una unidad
    ''' </summary>
    Public Shared Function ObtenerEstadoCuenta(unidadId As Integer, fechaDesde As DateTime, fechaHasta As DateTime) As DataTable

        Try
            ' Intentar usar vista si existe
            Dim query As String = "SELECT * FROM vw_estado_cuenta WHERE UnidadId = " & unidadId &
                                  " AND FechaMovimiento >= '" & fechaDesde.ToString("yyyy-MM-dd") & "' " &
                                  " AND FechaMovimiento <= '" & fechaHasta.ToString("yyyy-MM-dd") & "' " &
                                  " ORDER BY FechaMovimiento DESC, TipoMovimiento"

            Return DynamicCrudService.EjecutarConsulta(query)

        Catch
            ' Si la vista no existe, usar consulta manual
            Return ObtenerEstadoCuentaManual(unidadId, fechaDesde, fechaHasta)

        End Try
    End Function

    ''' <summary>
    ''' Obtiene el estado de cuenta con consulta manual (sin vista)
    ''' </summary>
    Private Shared Function ObtenerEstadoCuentaManual(unidadId As Integer, fechaDesde As Date, fechaHasta As Date) As DataTable

        Dim query As String = "SELECT " &
            "'Cuota' AS TipoMovimiento, " &
            "c.Id, " &
            "c.Folio, " &
            "c.FechaEmision AS FechaMovimiento, " &
            "cc.Nombre AS Concepto, " &
            "c.MontoTotal AS Cargo, " &
            "c.MontoPagado AS Abono, " &
            "c.Saldo, " &
            "c.Estado, " &
            "c.UnidadId, " &
            "u.Nombre AS UnidadNombre, " &
            "c.Periodo " &
            "FROM " & TABLA_CUOTAS & " c " &
            "LEFT JOIN cat_conceptos_cuota cc ON c.ConceptoCuotaId = cc.Id " &
            "LEFT JOIN " & TABLA_UNIDADES & " u ON c.UnidadId = u.Id " &
            "WHERE c.UnidadId = " & unidadId &
            " AND c.FechaEmision >= '" & fechaDesde.ToString("yyyy-MM-dd") & "' " &
            " AND c.FechaEmision <= '" & fechaHasta.ToString("yyyy-MM-dd") & "' " &
            "UNION ALL " &
            "SELECT " &
            "'Pago' AS TipoMovimiento, " &
            "p.Id, " &
            "p.Folio, " &
            "p.FechaPago AS FechaMovimiento, " &
            "CONCAT('Pago - ', p.FormaPago) AS Concepto, " &
            "0 AS Cargo, " &
            "p.Monto AS Abono, " &
            "0 AS Saldo, " &
            "p.Estado, " &
            "p.UnidadId, " &
            "u.Nombre AS UnidadNombre, " &
            "NULL AS Periodo " &
            "FROM " & TABLA_PAGOS & " p " &
            "LEFT JOIN " & TABLA_UNIDADES & " u ON p.UnidadId = u.Id " &
            "WHERE p.UnidadId = " & unidadId &
            " AND p.FechaPago >= '" & fechaDesde.ToString("yyyy-MM-dd") & "' " &
            " AND p.FechaPago <= '" & fechaHasta.ToString("yyyy-MM-dd") & "' " &
            "ORDER BY FechaMovimiento DESC"

        Return DynamicCrudService.EjecutarConsulta(query)

    End Function

    ''' <summary>
    ''' Calcula el resumen del estado de cuenta
    ''' </summary>
    Public Shared Function CalcularResumen(unidadId As Integer, fechaDesde As Date, fechaHasta As Date) As Dictionary(Of String, Object)

        Try
            Dim resultado As New Dictionary(Of String, Object)

            ' Obtener saldo anterior al período
            Dim querySaldoAnterior As String = "SELECT COALESCE(SUM(Saldo), 0) AS SaldoAnterior " &
                "FROM " & TABLA_CUOTAS & " " &
                "WHERE UnidadId = " & unidadId & " AND FechaEmision < '" & fechaDesde.ToString("yyyy-MM-dd") & "'"
            Dim dtSaldoAnterior As DataTable = DynamicCrudService.EjecutarConsulta(querySaldoAnterior)
            Dim saldoAnterior As Decimal = If(dtSaldoAnterior.Rows.Count > 0 AndAlso Not IsDBNull(dtSaldoAnterior.Rows(0)("SaldoAnterior")),
                                               Convert.ToDecimal(dtSaldoAnterior.Rows(0)("SaldoAnterior")), 0)

            ' Obtener cargos en el período
            Dim queryCargos As String = "SELECT COALESCE(SUM(MontoTotal), 0) AS TotalCargos " &
                "FROM " & TABLA_CUOTAS & " " &
                "WHERE UnidadId = " & unidadId &
                " AND FechaEmision >= '" & fechaDesde.ToString("yyyy-MM-dd") & "' " &
                " AND FechaEmision <= '" & fechaHasta.ToString("yyyy-MM-dd") & "'"
            Dim dtCargos As DataTable = DynamicCrudService.EjecutarConsulta(queryCargos)
            Dim totalCargos As Decimal = If(dtCargos.Rows.Count > 0 AndAlso Not IsDBNull(dtCargos.Rows(0)("TotalCargos")),
                                             Convert.ToDecimal(dtCargos.Rows(0)("TotalCargos")), 0)

            ' Obtener pagos en el período
            Dim queryPagos As String = "SELECT COALESCE(SUM(Monto), 0) AS TotalPagos " &
                "FROM " & TABLA_PAGOS & " " &
                "WHERE UnidadId = " & unidadId &
                " AND Estado = 'Aplicado' " &
                " AND FechaPago >= '" & fechaDesde.ToString("yyyy-MM-dd") & "' " &
                " AND FechaPago <= '" & fechaHasta.ToString("yyyy-MM-dd") & "'"
            Dim dtPagos As DataTable = DynamicCrudService.EjecutarConsulta(queryPagos)
            Dim totalPagos As Decimal = If(dtPagos.Rows.Count > 0 AndAlso Not IsDBNull(dtPagos.Rows(0)("TotalPagos")),
                                            Convert.ToDecimal(dtPagos.Rows(0)("TotalPagos")), 0)

            ' Obtener cuotas pendientes y vencidas
            Dim queryPendientes As String = "SELECT COUNT(*) AS Pendientes, " &
                "SUM(CASE WHEN FechaVencimiento < CURDATE() THEN 1 ELSE 0 END) AS Vencidas " &
                "FROM " & TABLA_CUOTAS & " " &
                "WHERE UnidadId = " & unidadId & " AND Saldo > 0"
            Dim dtPendientes As DataTable = DynamicCrudService.EjecutarConsulta(queryPendientes)
            Dim cuotasPendientes As Integer = If(dtPendientes.Rows.Count > 0 AndAlso Not IsDBNull(dtPendientes.Rows(0)("Pendientes")),
                                                  Convert.ToInt32(dtPendientes.Rows(0)("Pendientes")), 0)
            Dim cuotasVencidas As Integer = If(dtPendientes.Rows.Count > 0 AndAlso Not IsDBNull(dtPendientes.Rows(0)("Vencidas")),
                                                Convert.ToInt32(dtPendientes.Rows(0)("Vencidas")), 0)

            ' Calcular saldo actual
            Dim saldoActual As Decimal = saldoAnterior + totalCargos - totalPagos

            resultado("SaldoAnterior") = saldoAnterior
            resultado("TotalCargos") = totalCargos
            resultado("TotalPagos") = totalPagos
            resultado("SaldoActual") = saldoActual
            resultado("CuotasPendientes") = cuotasPendientes
            resultado("CuotasVencidas") = cuotasVencidas

            Return resultado

        Catch ex As Exception
            Logger.LogError("EstadoCuentaService.CalcularResumen", ex)
            Return New Dictionary(Of String, Object)

        End Try
    End Function

    ''' <summary>
    ''' Lista las entidades activas para combos
    ''' </summary>
    Public Shared Function ListarEntidades() As DataTable

        Try
            Return DynamicCrudService.ObtenerTodosConFiltro(TABLA_ENTIDADES, "Activo = 1")

        Catch ex As Exception
            Logger.LogError("EstadoCuentaService.ListarEntidades", ex)
            Return New DataTable()

        End Try
    End Function

End Class
