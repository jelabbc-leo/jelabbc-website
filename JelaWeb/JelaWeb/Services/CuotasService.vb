Imports System.Data
Imports System.Collections.Generic
Imports JelaWeb.Utilities
Imports JelaWeb.Infrastructure.Helpers

''' <summary>
''' Servicio para gestión de Cuotas
''' </summary>
Public Class CuotasService

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
            Logger.LogError("CuotasService.ListarUnidades", ex)
            Return New With {.success = False, .message = "Error: " & ex.Message}

        End Try
    End Function

    ''' <summary>
    ''' Lista los conceptos de cuota de una entidad
    ''' </summary>
    <System.Web.Services.WebMethod()>
    Public Shared Function ListarConceptosCuota(entidadId As Integer) As Object

        Try
            Dim query As String = "SELECT Id, Nombre " &
                                  "FROM cat_conceptos_cuota " &
                                  "WHERE EntidadId = " & entidadId & " AND Activo = 1 " &
                                  "ORDER BY Nombre"
            Dim dt As DataTable = DynamicCrudService.EjecutarConsulta(query)

            Return New With {.success = True, .data = DataTableHelper.ConvertDataTableToList(dt)}

        Catch ex As Exception
            Logger.LogError("CuotasService.ListarConceptosCuota", ex)
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
            Logger.LogError("CuotasService.ListarResidentes", ex)
            Return New With {.success = False, .message = "Error: " & ex.Message}

        End Try
    End Function

    ''' <summary>
    ''' Obtiene una cuota por ID
    ''' </summary>
    <System.Web.Services.WebMethod()>
    Public Shared Function ObtenerCuota(id As Integer) As Object

        Try
            Dim registro As DataRow = DynamicCrudService.ObtenerPorId(TABLA_CUOTAS, id)

            If registro Is Nothing Then Return New With {.success = False, .message = "No encontrado"}
            Dim datos As New Dictionary(Of String, Object)

            For Each col As DataColumn In registro.Table.Columns
                datos(col.ColumnName) = If(IsDBNull(registro(col)), Nothing, registro(col))

            Next

            Return New With {.success = True, .data = datos}

        Catch ex As Exception
            Logger.LogError("CuotasService.ObtenerCuota", ex)
            Return New With {.success = False, .message = "Error: " & ex.Message}

        End Try
    End Function

    ''' <summary>
    ''' Guarda una cuota (crea o actualiza)
    ''' </summary>
    <System.Web.Services.WebMethod()>
    Public Shared Function GuardarCuota(datos As Dictionary(Of String, Object)) As Object

        Try
            Dim id As Integer = Convert.ToInt32(datos("id"))
            Dim datosGuardar As New Dictionary(Of String, Object)
            datosGuardar.Add("EntidadId", Convert.ToInt32(datos("entidadId")))
            datosGuardar.Add("UnidadId", Convert.ToInt32(datos("unidadId")))
            datosGuardar.Add("ConceptoCuotaId", Convert.ToInt32(datos("conceptoCuotaId")))
            datosGuardar.Add("ResidenteId", If(datos("residenteId") IsNot Nothing AndAlso Convert.ToInt32(datos("residenteId")) > 0, Convert.ToInt32(datos("residenteId")), DBNull.Value))
            datosGuardar.Add("Periodo", datos("periodo")?.ToString())
            datosGuardar.Add("Monto", Convert.ToDecimal(datos("monto")))
            datosGuardar.Add("Descuento", If(datos("descuento") IsNot Nothing, Convert.ToDecimal(datos("descuento")), 0))
            datosGuardar.Add("Recargo", If(datos("recargo") IsNot Nothing, Convert.ToDecimal(datos("recargo")), 0))
            datosGuardar.Add("FechaEmision", Convert.ToDateTime(datos("fechaEmision")))
            datosGuardar.Add("FechaVencimiento", Convert.ToDateTime(datos("fechaVencimiento")))
            datosGuardar.Add("Estado", datos("estado")?.ToString())
            datosGuardar.Add("Observaciones", datos("observaciones")?.ToString())

            ' Si es nuevo, generar folio
            If id = 0 Then
                Dim folio As String = "CUO-" & DateTime.Now.ToString("yyyyMM") & "-" & Guid.NewGuid().ToString("N").Substring(0, 4).ToUpper()

                datosGuardar.Add("Folio", folio)
            End If

            Dim resultado As Boolean

            If id = 0 Then
                resultado = DynamicCrudService.Insertar(TABLA_CUOTAS, datosGuardar)
            Else
                resultado = DynamicCrudService.Actualizar(TABLA_CUOTAS, id, datosGuardar)
            End If
            Return New With {.success = resultado, .message = If(resultado, "Guardado correctamente", "Error al guardar")}

        Catch ex As Exception
            Logger.LogError("CuotasService.GuardarCuota", ex)
            Return New With {.success = False, .message = "Error: " & ex.Message}

        End Try
    End Function

    ''' <summary>
    ''' Elimina una cuota
    ''' </summary>
    <System.Web.Services.WebMethod()>
    Public Shared Function EliminarCuota(id As Integer) As Object

        Try
            Dim resultado = DynamicCrudService.Eliminar(TABLA_CUOTAS, id)

            Return New With {.success = resultado, .message = If(resultado, "Eliminado", "Error")}

        Catch ex As Exception
            Logger.LogError("CuotasService.EliminarCuota", ex)
            Return New With {.success = False, .message = "Error: " & ex.Message}

        End Try
    End Function

    ''' <summary>
    ''' Genera cuotas masivas para todas las unidades de una entidad
    ''' </summary>
    <System.Web.Services.WebMethod()>
    Public Shared Function GenerarCuotasMasivas(datos As Dictionary(Of String, Object)) As Object

        Try
            Dim entidadId As Integer = Convert.ToInt32(datos("entidadId"))
            Dim conceptoCuotaId As Integer = Convert.ToInt32(datos("conceptoCuotaId"))
            Dim periodo As String = datos("periodo")?.ToString()
            Dim fechaVencimiento As Date = Convert.ToDateTime(datos("fechaVencimiento"))
            Dim montoBase As Decimal = Convert.ToDecimal(datos("montoBase"))

            ' Obtener todas las unidades activas de la entidad
            Dim queryUnidades As String = "SELECT Id FROM cat_unidades WHERE EntidadId = " & entidadId & " AND Activo = 1"
            Dim dtUnidades As DataTable = DynamicCrudService.EjecutarConsulta(queryUnidades)
            Dim cuotasGeneradas As Integer = 0
            Dim cuotasOmitidas As Integer = 0

            For Each row As DataRow In dtUnidades.Rows
                Dim unidadId As Integer = Convert.ToInt32(row("Id"))

                ' Verificar si ya existe una cuota para esta unidad, concepto y período
                Dim queryExiste As String = "SELECT COUNT(*) AS Existe FROM " & TABLA_CUOTAS &
                                            " WHERE UnidadId = " & unidadId &
                                            " AND ConceptoCuotaId = " & conceptoCuotaId &
                                            " AND Periodo = '" & periodo & "'"
                Dim dtExiste As DataTable = DynamicCrudService.EjecutarConsulta(queryExiste)

                If dtExiste.Rows.Count > 0 AndAlso Convert.ToInt32(dtExiste.Rows(0)("Existe")) > 0 Then
                    cuotasOmitidas += 1
                    Continue For
                End If

                ' Generar folio único
                Dim folio As String = "CUO-" & periodo.Replace("-", "") & "-" & Guid.NewGuid().ToString("N").Substring(0, 4).ToUpper()

                ' Insertar cuota
                Dim datosCuota As New Dictionary(Of String, Object)
                datosCuota.Add("Folio", folio)
                datosCuota.Add("EntidadId", entidadId)
                datosCuota.Add("UnidadId", unidadId)
                datosCuota.Add("ConceptoCuotaId", conceptoCuotaId)
                datosCuota.Add("Periodo", periodo)
                datosCuota.Add("Monto", montoBase)
                datosCuota.Add("Descuento", 0)
                datosCuota.Add("Recargo", 0)
                datosCuota.Add("FechaEmision", DateTime.Now.Date)
                datosCuota.Add("FechaVencimiento", fechaVencimiento)
                datosCuota.Add("Estado", "Pendiente")

                If DynamicCrudService.Insertar(TABLA_CUOTAS, datosCuota) Then
                    cuotasGeneradas += 1
                End If

            Next

            Return New With {
                .success = True,
                .message = $"Generadas {cuotasGeneradas} cuotas. Omitidas {cuotasOmitidas} (ya existían).",
                .cuotasGeneradas = cuotasGeneradas,
                .cuotasOmitidas = cuotasOmitidas
            }

        Catch ex As Exception
            Logger.LogError("CuotasService.GenerarCuotasMasivas", ex)
            Return New With {.success = False, .message = "Error: " & ex.Message}

        End Try
    End Function

    ''' <summary>
    ''' Aplica recargos por mora a las cuotas vencidas
    ''' </summary>
    <System.Web.Services.WebMethod()>
    Public Shared Function AplicarRecargosMora() As Object

        Try
            ' Este método puede llamar al stored procedure si existe
            ' Por ahora, implementación básica
            Dim query As String = "SELECT * FROM op_cuotas WHERE Estado IN ('Pendiente', 'Vencida') AND FechaVencimiento < CURDATE()"
            Dim dt As DataTable = DynamicCrudService.EjecutarConsulta(query)
            Dim recargosAplicados As Integer = 0

            For Each row As DataRow In dt.Rows
                Dim cuotaId As Integer = Convert.ToInt32(row("Id"))
                Dim diasVencidos As Integer = DateDiff(DateInterval.Day, Convert.ToDateTime(row("FechaVencimiento")), DateTime.Now)

                ' Obtener concepto de cuota para saber el porcentaje de recargo
                Dim conceptoId As Integer = Convert.ToInt32(row("ConceptoCuotaId"))
                Dim queryConcepto As String = "SELECT PorcentajeRecargo FROM cat_conceptos_cuota WHERE Id = " & conceptoId
                Dim dtConcepto As DataTable = DynamicCrudService.EjecutarConsulta(queryConcepto)
                Dim porcentajeRecargo As Decimal = 0

                If dtConcepto.Rows.Count > 0 AndAlso Not IsDBNull(dtConcepto.Rows(0)("PorcentajeRecargo")) Then
                    porcentajeRecargo = Convert.ToDecimal(dtConcepto.Rows(0)("PorcentajeRecargo"))
                End If

                If porcentajeRecargo > 0 AndAlso diasVencidos > 0 Then
                    Dim monto As Decimal = Convert.ToDecimal(row("Monto"))
                    Dim recargo As Decimal = monto * (porcentajeRecargo / 100) * (diasVencidos / 30) ' Recargo mensual
                    Dim datosUpdate As New Dictionary(Of String, Object)
                    datosUpdate.Add("Recargo", recargo)
                    datosUpdate.Add("Estado", "Vencida")

                    If DynamicCrudService.Actualizar(TABLA_CUOTAS, cuotaId, datosUpdate) Then
                        recargosAplicados += 1
                    End If
                End If

            Next

            Return New With {
                .success = True,
                .message = $"Recargos aplicados a {recargosAplicados} cuotas vencidas.",
                .recargosAplicados = recargosAplicados
            }

        Catch ex As Exception
            Logger.LogError("CuotasService.AplicarRecargosMora", ex)
            Return New With {.success = False, .message = "Error: " & ex.Message}

        End Try
    End Function

    ''' <summary>
    ''' Lista todas las cuotas con información relacionada
    ''' </summary>
    Public Shared Function ListarCuotas(fechaDesde As DateTime, fechaHasta As DateTime) As DataTable

        Try
            Dim query As String = "SELECT c.*, " &
                "e.RazonSocial AS EntidadNombre, " &
                "u.Nombre AS UnidadNombre, " &
                "u.Codigo AS UnidadCodigo, " &
                "cc.Nombre AS ConceptoNombre, " &
                "r.NombreCompleto AS ResidenteNombre " &
                "FROM " & TABLA_CUOTAS & " c " &
                "LEFT JOIN cat_entidades e ON c.EntidadId = e.Id " &
                "LEFT JOIN cat_unidades u ON c.UnidadId = u.Id " &
                "LEFT JOIN cat_conceptos_cuota cc ON c.ConceptoCuotaId = cc.Id " &
                "LEFT JOIN cat_residentes r ON c.ResidenteId = r.Id " &
                "WHERE c.FechaEmision >= '" & fechaDesde.ToString("yyyy-MM-dd") & "' " &
                "AND c.FechaEmision <= '" & fechaHasta.ToString("yyyy-MM-dd") & "' " &
                "ORDER BY c.FechaEmision DESC, c.Folio DESC"

            Return DynamicCrudService.EjecutarConsulta(query)

        Catch ex As Exception
            Logger.LogError("CuotasService.ListarCuotas", ex)
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
            Logger.LogError("CuotasService.ListarEntidades", ex)
            Return New DataTable()

        End Try
    End Function

End Class
