Imports System.Data
Imports System.Collections.Generic
Imports System.Configuration
Imports JelaWeb.Utilities
Imports JelaWeb.Infrastructure.Helpers
Imports JelaWeb.Services.API

''' <summary>
''' Servicio para gestión de Unidades
''' </summary>
Public Class UnidadesService

    Private Const TABLA_UNIDADES As String = "cat_unidades"
    Private Const TABLA_RESIDENTES As String = "cat_residentes"
    Private Const TABLA_VEHICULOS As String = "cat_vehiculos_unidad"
    Private Const TABLA_TAGS As String = "cat_tags_unidad"
    Private Const TABLA_DOCUMENTOS As String = "cat_documentos_unidad"
    Private Const TABLA_ARCHIVOS_RESIDENTE As String = "cat_residente_archivos"
    Private Const TABLA_ARCHIVOS_VEHICULO As String = "cat_vehiculo_archivos"
    Private Const TABLA_ARCHIVOS_DOCUMENTO As String = "cat_documento_unidad_archivos"

    ''' <summary>
    ''' Obtiene una unidad por ID
    ''' </summary>
    <System.Web.Services.WebMethod()>
    Public Shared Function ObtenerUnidad(id As Integer) As Object

        Try
            ' Consulta con JOIN para obtener el propietario principal
            Dim query As String = "SELECT u.*, " &
                                  "CONCAT(r.Nombre, ' ', r.ApellidoPaterno, ' ', IFNULL(r.ApellidoMaterno, '')) AS PropietarioPrincipal " &
                                  "FROM cat_unidades u " &
                                  "LEFT JOIN cat_residentes r ON u.Id = r.UnidadId AND r.EsPrincipal = 1 AND r.Activo = 1 " &
                                  "WHERE u.Id = " & id

            Dim dt As DataTable = DynamicCrudService.EjecutarConsulta(query)

            If dt Is Nothing OrElse dt.Rows.Count = 0 Then
                Return New With {.success = False, .message = "Unidad no encontrada"}
            End If

            Dim registro As DataRow = dt.Rows(0)
            Dim datos As New Dictionary(Of String, Object)

            For Each col As DataColumn In registro.Table.Columns
                datos(col.ColumnName) = If(IsDBNull(registro(col)), Nothing, registro(col))

            Next

            Return New With {.success = True, .data = datos}

        Catch ex As Exception
            Logger.LogError("UnidadesService.ObtenerUnidad", ex)
            Return New With {.success = False, .message = "Error al obtener unidad"}

        End Try
    End Function

    ''' <summary>
    ''' Elimina una unidad
    ''' </summary>
    <System.Web.Services.WebMethod()>
    Public Shared Function EliminarUnidad(id As Integer) As Object

        Try
            Dim resultado As Boolean = DynamicCrudService.Eliminar(TABLA_UNIDADES, id)

            If resultado Then
                Logger.LogInfo($"Unidad eliminada: Id={id}")
                Return New With {.success = True, .message = "Unidad eliminada correctamente"}
            Else
                Return New With {.success = False, .message = "Error al eliminar unidad"}
            End If

        Catch ex As Exception
            Logger.LogError("UnidadesService.EliminarUnidad", ex)
            Return New With {.success = False, .message = "Error al eliminar unidad"}

        End Try
    End Function

    ''' <summary>
    ''' Obtiene las entidades activas
    ''' </summary>
    <System.Web.Services.WebMethod()>
    Public Shared Function ObtenerEntidades() As Object

        Try
            Dim apiUrl As String = ConfigurationManager.AppSettings("ApiBaseUrl")
            Dim servicio As New ApiService(apiUrl)
            Dim dt As DataTable = servicio.ListarEntidadesActivas()
            Dim lista As New List(Of Object)

            Logger.LogInfo($"UnidadesService.ObtenerEntidades: Se encontraron {If(dt IsNot Nothing, dt.Rows.Count, 0)} entidades")

            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then

                For Each row As DataRow In dt.Rows
                    lista.Add(New With {
                        .Id = Convert.ToInt32(row("Id")),
                        .RazonSocial = If(IsDBNull(row("RazonSocial")), "", row("RazonSocial").ToString())
                    })

                Next

            End If

            Return New With {.success = True, .data = lista}

        Catch ex As Exception
            Logger.LogError("UnidadesService.ObtenerEntidades", ex)
            Return New With {.success = False, .message = "Error al obtener entidades: " & ex.Message}

        End Try
    End Function

    ''' <summary>
    ''' Obtiene las sub-entidades de una entidad
    ''' </summary>
    <System.Web.Services.WebMethod()>
    Public Shared Function ObtenerSubEntidadesPorEntidad(entidadId As Integer) As Object

        Try
            Dim apiUrl As String = ConfigurationManager.AppSettings("ApiBaseUrl")
            Dim servicio As New ApiService(apiUrl)
            Dim dt As DataTable = servicio.ListarSubEntidadesActivasPorEntidad(entidadId)
            Dim lista As New List(Of Object)

            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then

                For Each row As DataRow In dt.Rows
                    lista.Add(New With {
                        .Id = Convert.ToInt32(row("Id")),
                        .RazonSocial = If(IsDBNull(row("RazonSocial")), "", row("RazonSocial").ToString())
                    })

                Next

            End If

            Return New With {.success = True, .data = lista}

        Catch ex As Exception
            Logger.LogError("UnidadesService.ObtenerSubEntidadesPorEntidad", ex)
            Return New With {.success = False, .message = "Error al obtener sub-entidades: " & ex.Message}

        End Try
    End Function

    ''' <summary>
    ''' Guarda una unidad (crea o actualiza)
    ''' </summary>
    <System.Web.Services.WebMethod()>
    Public Shared Function GuardarUnidad(datos As Dictionary(Of String, Object)) As Object

        Try
            Dim id As Integer = Convert.ToInt32(datos("id"))
            Dim datosGuardar As New Dictionary(Of String, Object)
            datosGuardar.Add("entidad_id", Convert.ToInt32(datos("entidadId")))
            datosGuardar.Add("SubEntidadId", If(datos("subEntidadId") IsNot Nothing AndAlso Convert.ToInt32(datos("subEntidadId")) > 0, Convert.ToInt32(datos("subEntidadId")), DBNull.Value))
            datosGuardar.Add("codigo", datos("codigo")?.ToString())
            datosGuardar.Add("nombre", datos("nombre")?.ToString())
            datosGuardar.Add("torre", datos("torre")?.ToString())
            datosGuardar.Add("edificio", datos("edificio")?.ToString())
            datosGuardar.Add("piso", datos("piso")?.ToString())
            datosGuardar.Add("numero", datos("numero")?.ToString())
            datosGuardar.Add("Calle", datos("calle")?.ToString())
            datosGuardar.Add("NumeroExterior", datos("numeroExterior")?.ToString())
            datosGuardar.Add("NumeroInterior", datos("numeroInterior")?.ToString())
            datosGuardar.Add("Referencia", datos("referencia")?.ToString())
            datosGuardar.Add("Latitud", If(datos("latitud") IsNot Nothing AndAlso Not String.IsNullOrEmpty(datos("latitud").ToString()), Convert.ToDecimal(datos("latitud")), DBNull.Value))
            datosGuardar.Add("Longitud", If(datos("longitud") IsNot Nothing AndAlso Not String.IsNullOrEmpty(datos("longitud").ToString()), Convert.ToDecimal(datos("longitud")), DBNull.Value))
            datosGuardar.Add("superficie", If(datos("superficie") IsNot Nothing, Convert.ToDecimal(datos("superficie")), DBNull.Value))
            datosGuardar.Add("activo", If(Convert.ToBoolean(datos("activo")), 1, 0))

            Dim resultado As Boolean
            Dim nuevoId As Integer = 0

            If id = 0 Then
                nuevoId = DynamicCrudService.InsertarConId(TABLA_UNIDADES, datosGuardar)
                resultado = nuevoId > 0
            Else
                resultado = DynamicCrudService.Actualizar(TABLA_UNIDADES, id, datosGuardar)
                nuevoId = id
            End If

            Return New With {.success = resultado, .message = If(resultado, "Guardado correctamente", "Error al guardar"), .id = nuevoId}

        Catch ex As Exception
            Logger.LogError("UnidadesService.GuardarUnidad", ex)
            Return New With {.success = False, .message = "Error: " & ex.Message}

        End Try
    End Function

    ''' <summary>
    ''' Obtiene los residentes de una unidad
    ''' </summary>
    <System.Web.Services.WebMethod()>
    Public Shared Function ObtenerResidentesUnidad(unidadId As Integer) As Object

        Try
            Dim query As String = "SELECT " &
                                  "IFNULL(e.RazonSocial, '') AS NombreEntidad, " &
                                  "IFNULL(se.RazonSocial, '') AS NombreSubEntidad, " &
                                  "CONCAT(IFNULL(u.Codigo, ''), ' - ', IFNULL(u.Nombre, '')) AS NombreUnidad, " &
                                  "CONCAT(IFNULL(r.Nombre, ''), ' ', IFNULL(r.ApellidoPaterno, ''), ' ', IFNULL(r.ApellidoMaterno,'')) AS NombreCompleto, " &
                                  "IFNULL(r.TipoResidente, '') AS TipoResidente, " &
                                  "CAST(IFNULL(r.EsPrincipal, 0) AS UNSIGNED) AS EsPrincipal, " &
                                  "IFNULL(r.Email, '') AS Email, " &
                                  "IFNULL(r.TelefonoCelular, '') AS Celular, " &
                                  "IFNULL(r.CURP, '') AS CURP " &
                                  "FROM cat_residentes r " &
                                  "INNER JOIN cat_unidades u ON r.UnidadId = u.Id " &
                                  "INNER JOIN cat_entidades e ON u.entidad_id = e.Id " &
                                  "LEFT JOIN cat_sub_entidades se ON u.SubEntidadId = se.Id " &
                                  "WHERE r.UnidadId = " & unidadId & " " &
                                  "ORDER BY r.EsPrincipal DESC, r.Nombre"
            Dim dt As DataTable = DynamicCrudService.EjecutarConsulta(query)

            ' Convertir columna EsPrincipal a Boolean si existe
            If dt IsNot Nothing AndAlso dt.Columns.Contains("EsPrincipal") Then
                Dim colEsPrincipal = dt.Columns("EsPrincipal")

                If colEsPrincipal.DataType IsNot GetType(Boolean) Then
                    ' Crear nueva columna Boolean
                    Dim nuevaCol As New DataColumn("EsPrincipal_Bool", GetType(Boolean))

                    dt.Columns.Add(nuevaCol)

                    ' Copiar valores convertidos

                    For Each row As DataRow In dt.Rows
                        If Not IsDBNull(row("EsPrincipal")) Then
                            Dim valor = row("EsPrincipal")

                            row("EsPrincipal_Bool") = (Convert.ToInt32(valor) <> 0)
                        Else
                            row("EsPrincipal_Bool") = False
                        End If

                    Next

                    ' Remover columna original y renombrar la nueva
                    dt.Columns.Remove("EsPrincipal")
                    nuevaCol.ColumnName = "EsPrincipal"
                End If
            End If

            Return New With {.success = True, .data = DataTableHelper.ConvertDataTableToList(dt)}

        Catch ex As Exception
            Logger.LogError("UnidadesService.ObtenerResidentesUnidad", ex)
            Return New With {.success = False, .message = "Error al obtener residentes"}

        End Try
    End Function

    ''' <summary>
    ''' Obtiene un residente por ID
    ''' </summary>
    <System.Web.Services.WebMethod()>
    Public Shared Function ObtenerResidente(id As Integer) As Object

        Try
            Dim registro As DataRow = DynamicCrudService.ObtenerPorId(TABLA_RESIDENTES, id)

            If registro Is Nothing Then Return New With {.success = False, .message = "No encontrado"}
            Dim datos As New Dictionary(Of String, Object)

            For Each col As DataColumn In registro.Table.Columns
                datos(col.ColumnName) = If(IsDBNull(registro(col)), Nothing, registro(col))

            Next

            Return New With {.success = True, .data = datos}

        Catch ex As Exception
            Return New With {.success = False, .message = "Error: " & ex.Message}

        End Try
    End Function

    ''' <summary>
    ''' Guarda un residente (crea o actualiza)
    ''' </summary>
    <System.Web.Services.WebMethod()>
    Public Shared Function GuardarResidente(datos As Dictionary(Of String, Object)) As Object

        Try
            Dim id As Integer = Convert.ToInt32(datos("id"))

            ' Validar campos requeridos
            Dim nombre As String = If(datos.ContainsKey("nombre"), datos("nombre")?.ToString(), "")
            Dim apellidoPaterno As String = If(datos.ContainsKey("apellidoPaterno"), datos("apellidoPaterno")?.ToString(), "")
            Dim apellidoMaterno As String = If(datos.ContainsKey("apellidoMaterno"), datos("apellidoMaterno")?.ToString(), "")
            Dim email As String = If(datos.ContainsKey("email"), datos("email")?.ToString(), "")
            Dim celular As String = If(datos.ContainsKey("celular"), datos("celular")?.ToString(), "")

            If String.IsNullOrWhiteSpace(nombre) Then
                Return New With {.success = False, .message = "El nombre es requerido"}
            End If

            If String.IsNullOrWhiteSpace(apellidoPaterno) Then
                Return New With {.success = False, .message = "El apellido paterno es requerido"}
            End If

            If String.IsNullOrWhiteSpace(apellidoMaterno) Then
                Return New With {.success = False, .message = "El apellido materno es requerido"}
            End If

            If String.IsNullOrWhiteSpace(email) Then
                Return New With {.success = False, .message = "El email es requerido"}
            End If

            If String.IsNullOrWhiteSpace(celular) Then
                Return New With {.success = False, .message = "El celular es requerido"}
            End If

            ' IMPORTANTE: Los nombres de campos deben coincidir EXACTAMENTE con los nombres de columnas en MySQL
            ' Basado en las consultas SELECT, las columnas usan mayúscula inicial
            Dim datosGuardar As New Dictionary(Of String, Object)
            datosGuardar.Add("UnidadId", Convert.ToInt32(datos("unidadId")))
            datosGuardar.Add("TipoResidente", If(datos.ContainsKey("tipoResidente") AndAlso datos("tipoResidente") IsNot Nothing, datos("tipoResidente").ToString(), "Propietario"))
            datosGuardar.Add("EsPrincipal", If(datos.ContainsKey("esPrincipal") AndAlso Convert.ToBoolean(datos("esPrincipal")), 1, 0))
            datosGuardar.Add("Nombre", nombre)
            datosGuardar.Add("ApellidoPaterno", apellidoPaterno)
            datosGuardar.Add("ApellidoMaterno", apellidoMaterno)
            datosGuardar.Add("Email", email)
            datosGuardar.Add("Telefono", If(datos.ContainsKey("telefono") AndAlso datos("telefono") IsNot Nothing, datos("telefono").ToString(), ""))
            datosGuardar.Add("TelefonoCelular", celular)
            datosGuardar.Add("CURP", If(datos.ContainsKey("curp") AndAlso datos("curp") IsNot Nothing, datos("curp").ToString(), ""))
            datosGuardar.Add("Activo", If(datos.ContainsKey("activo") AndAlso Convert.ToBoolean(datos("activo")), 1, 0))

            Logger.LogInfo($"UnidadesService.GuardarResidente: Intentando guardar residente. ID={id}, UnidadId={datosGuardar("UnidadId")}, Nombre={nombre}")

            Dim resultado As Boolean
            Dim nuevoId As Integer = id

            If id = 0 Then
                nuevoId = DynamicCrudService.InsertarConId(TABLA_RESIDENTES, datosGuardar)
                resultado = nuevoId > 0
                Logger.LogInfo($"UnidadesService.GuardarResidente: Inserción completada. NuevoId={nuevoId}, Resultado={resultado}")
            Else
                resultado = DynamicCrudService.Actualizar(TABLA_RESIDENTES, id, datosGuardar)
                nuevoId = id
                Logger.LogInfo($"UnidadesService.GuardarResidente: Actualización completada. Id={id}, Resultado={resultado}")
            End If

            If Not resultado Then
                Logger.LogError("UnidadesService.GuardarResidente: El servicio dinámico retornó False")
            End If

            ' Los archivos se guardan por separado desde JavaScript
            Return New With {
                .success = resultado,
                .message = If(resultado, "Guardado correctamente", "Error al guardar"),
                .data = New With {.id = nuevoId}
            }

        Catch ex As Exception
            Logger.LogError("UnidadesService.GuardarResidente", ex)
            Return New With {.success = False, .message = "Error: " & ex.Message}

        End Try
    End Function

    ''' <summary>
    ''' Elimina un residente
    ''' </summary>
    <System.Web.Services.WebMethod()>
    Public Shared Function EliminarResidente(id As Integer) As Object

        Try
            Dim resultado = DynamicCrudService.Eliminar(TABLA_RESIDENTES, id)

            Return New With {.success = resultado, .message = If(resultado, "Eliminado", "Error")}

        Catch ex As Exception
            Return New With {.success = False, .message = "Error: " & ex.Message}

        End Try
    End Function

    ''' <summary>
    ''' Obtiene los vehículos de una unidad
    ''' </summary>
    <System.Web.Services.WebMethod()>
    Public Shared Function ObtenerVehiculosUnidad(unidadId As Integer) As Object

        Try
            Dim query As String = "SELECT v.*, CONCAT(r.Nombre, ' ', r.ApellidoPaterno) AS ResidenteNombre " &
                                  "FROM " & TABLA_VEHICULOS & " v LEFT JOIN cat_residentes r ON v.ResidenteId = r.Id " &
                                  "WHERE v.UnidadId = " & unidadId & " ORDER BY v.Placas"
            Dim dt As DataTable = DynamicCrudService.EjecutarConsulta(query)

            Return New With {.success = True, .data = DataTableHelper.ConvertDataTableToList(dt)}

        Catch ex As Exception
            Return New With {.success = False, .message = "Error: " & ex.Message}

        End Try
    End Function

    ''' <summary>
    ''' Obtiene un vehículo por ID
    ''' </summary>
    <System.Web.Services.WebMethod()>
    Public Shared Function ObtenerVehiculo(id As Integer) As Object

        Try
            Dim registro As DataRow = DynamicCrudService.ObtenerPorId(TABLA_VEHICULOS, id)

            If registro Is Nothing Then Return New With {.success = False, .message = "No encontrado"}
            Dim datos As New Dictionary(Of String, Object)

            For Each col As DataColumn In registro.Table.Columns
                datos(col.ColumnName) = If(IsDBNull(registro(col)), Nothing, registro(col))

            Next

            Return New With {.success = True, .data = datos}

        Catch ex As Exception
            Return New With {.success = False, .message = "Error: " & ex.Message}

        End Try
    End Function

    ''' <summary>
    ''' Guarda un vehículo (crea o actualiza)
    ''' </summary>
    <System.Web.Services.WebMethod()>
    Public Shared Function GuardarVehiculo(datos As Dictionary(Of String, Object)) As Object

        Try
            Dim id As Integer = Convert.ToInt32(datos("id"))
            Dim datosGuardar As New Dictionary(Of String, Object)
            datosGuardar.Add("UnidadId", Convert.ToInt32(datos("unidadId")))
            datosGuardar.Add("ResidenteId", If(datos("residenteId") IsNot Nothing AndAlso Convert.ToInt32(datos("residenteId")) > 0, Convert.ToInt32(datos("residenteId")), DBNull.Value))
            datosGuardar.Add("Placas", datos("placas")?.ToString())
            datosGuardar.Add("TipoVehiculo", datos("tipoVehiculo")?.ToString())
            datosGuardar.Add("Marca", datos("marca")?.ToString())
            datosGuardar.Add("Modelo", datos("modelo")?.ToString())
            datosGuardar.Add("Anio", If(datos("anio") IsNot Nothing, Convert.ToInt32(datos("anio")), DBNull.Value))
            datosGuardar.Add("Color", datos("color")?.ToString())
            datosGuardar.Add("NumeroMotor", datos("numeroMotor")?.ToString())
            datosGuardar.Add("NumeroSerie", datos("numeroSerie")?.ToString())
            datosGuardar.Add("TipoCombustible", datos("tipoCombustible")?.ToString())
            datosGuardar.Add("CapacidadPasajeros", If(datos("capacidadPasajeros") IsNot Nothing AndAlso Not String.IsNullOrEmpty(datos("capacidadPasajeros").ToString()), Convert.ToInt32(datos("capacidadPasajeros")), DBNull.Value))
            datosGuardar.Add("UsoVehiculo", datos("usoVehiculo")?.ToString())
            datosGuardar.Add("NumeroTarjeton", datos("numeroTarjeton")?.ToString())
            datosGuardar.Add("FechaExpedicionTarjeta", If(datos("fechaExpedicionTarjeta") IsNot Nothing, Convert.ToDateTime(datos("fechaExpedicionTarjeta")), DBNull.Value))
            datosGuardar.Add("FechaVigenciaTarjeta", If(datos("fechaVigenciaTarjeta") IsNot Nothing, Convert.ToDateTime(datos("fechaVigenciaTarjeta")), DBNull.Value))
            datosGuardar.Add("PropietarioRegistrado", datos("propietarioRegistrado")?.ToString())
            datosGuardar.Add("Observaciones", datos("observaciones")?.ToString())
            datosGuardar.Add("Activo", If(Convert.ToBoolean(datos("activo")), 1, 0))

            Dim resultado As Boolean
            Dim nuevoId As Integer = id

            If id = 0 Then
                nuevoId = DynamicCrudService.InsertarConId(TABLA_VEHICULOS, datosGuardar)
                resultado = nuevoId > 0
            Else
                resultado = DynamicCrudService.Actualizar(TABLA_VEHICULOS, id, datosGuardar)
                nuevoId = id
            End If

            Return New With {
                .success = resultado,
                .message = If(resultado, "Guardado correctamente", "Error al guardar"),
                .data = New With {.id = nuevoId}
            }

        Catch ex As Exception
            Return New With {.success = False, .message = "Error: " & ex.Message}

        End Try
    End Function

    ''' <summary>
    ''' Elimina un vehículo
    ''' </summary>
    <System.Web.Services.WebMethod()>
    Public Shared Function EliminarVehiculo(id As Integer) As Object

        Try
            Dim resultado = DynamicCrudService.Eliminar(TABLA_VEHICULOS, id)

            Return New With {.success = resultado, .message = If(resultado, "Eliminado", "Error")}

        Catch ex As Exception
            Return New With {.success = False, .message = "Error: " & ex.Message}

        End Try
    End Function

    ''' <summary>
    ''' Obtiene los tags de una unidad
    ''' </summary>
    <System.Web.Services.WebMethod()>
    Public Shared Function ObtenerTagsUnidad(unidadId As Integer) As Object

        Try
            Dim query As String = "SELECT t.*, CONCAT(r.Nombre, ' ', r.ApellidoPaterno) AS ResidenteNombre " &
                                  "FROM " & TABLA_TAGS & " t LEFT JOIN cat_residentes r ON t.ResidenteId = r.Id " &
                                  "WHERE t.UnidadId = " & unidadId & " ORDER BY t.CodigoTag"
            Dim dt As DataTable = DynamicCrudService.EjecutarConsulta(query)

            Return New With {.success = True, .data = DataTableHelper.ConvertDataTableToList(dt)}

        Catch ex As Exception
            Return New With {.success = False, .message = "Error: " & ex.Message}

        End Try
    End Function

    ''' <summary>
    ''' Obtiene un tag por ID
    ''' </summary>
    <System.Web.Services.WebMethod()>
    Public Shared Function ObtenerTag(id As Integer) As Object

        Try
            Dim registro As DataRow = DynamicCrudService.ObtenerPorId(TABLA_TAGS, id)

            If registro Is Nothing Then Return New With {.success = False, .message = "No encontrado"}
            Dim datos As New Dictionary(Of String, Object)

            For Each col As DataColumn In registro.Table.Columns
                datos(col.ColumnName) = If(IsDBNull(registro(col)), Nothing, registro(col))

            Next

            Return New With {.success = True, .data = datos}

        Catch ex As Exception
            Return New With {.success = False, .message = "Error: " & ex.Message}

        End Try
    End Function

    ''' <summary>
    ''' Guarda un tag (crea o actualiza)
    ''' </summary>
    <System.Web.Services.WebMethod()>
    Public Shared Function GuardarTag(datos As Dictionary(Of String, Object)) As Object

        Try
            Dim id As Integer = Convert.ToInt32(datos("id"))
            Dim datosGuardar As New Dictionary(Of String, Object)
            datosGuardar.Add("UnidadId", Convert.ToInt32(datos("unidadId")))
            datosGuardar.Add("ResidenteId", If(datos("residenteId") IsNot Nothing AndAlso Convert.ToInt32(datos("residenteId")) > 0, Convert.ToInt32(datos("residenteId")), DBNull.Value))
            datosGuardar.Add("CodigoTag", datos("codigoTag")?.ToString())
            datosGuardar.Add("TipoTag", datos("tipoTag")?.ToString())
            datosGuardar.Add("Descripcion", datos("descripcion")?.ToString())
            datosGuardar.Add("FechaAsignacion", If(datos("fechaAsignacion") IsNot Nothing, Convert.ToDateTime(datos("fechaAsignacion")), DBNull.Value))
            datosGuardar.Add("FechaVencimiento", If(datos("fechaVencimiento") IsNot Nothing, Convert.ToDateTime(datos("fechaVencimiento")), DBNull.Value))
            datosGuardar.Add("PermiteAccesoPeatonal", If(Convert.ToBoolean(datos("permiteAccesoPeatonal")), 1, 0))
            datosGuardar.Add("PermiteAccesoVehicular", If(Convert.ToBoolean(datos("permiteAccesoVehicular")), 1, 0))
            datosGuardar.Add("PermiteAccesoAreas", If(Convert.ToBoolean(datos("permiteAccesoAreas")), 1, 0))
            datosGuardar.Add("Observaciones", datos("observaciones")?.ToString())
            datosGuardar.Add("Activo", If(Convert.ToBoolean(datos("activo")), 1, 0))
            Dim resultado As Boolean
            Dim nuevoId As Integer = id

            If id = 0 Then
                nuevoId = DynamicCrudService.InsertarConId(TABLA_TAGS, datosGuardar)
                resultado = nuevoId > 0
            Else
                resultado = DynamicCrudService.Actualizar(TABLA_TAGS, id, datosGuardar)
                nuevoId = id
            End If
            Return New With {
                .success = resultado,
                .message = If(resultado, "Guardado correctamente", "Error al guardar"),
                .data = New With {.id = nuevoId}
            }

        Catch ex As Exception
            Return New With {.success = False, .message = "Error: " & ex.Message}

        End Try
    End Function

    ''' <summary>
    ''' Elimina un tag
    ''' </summary>
    <System.Web.Services.WebMethod()>
    Public Shared Function EliminarTag(id As Integer) As Object

        Try
            Dim resultado = DynamicCrudService.Eliminar(TABLA_TAGS, id)

            Return New With {.success = resultado, .message = If(resultado, "Eliminado", "Error")}

        Catch ex As Exception
            Return New With {.success = False, .message = "Error: " & ex.Message}

        End Try
    End Function

    ''' <summary>
    ''' Obtiene los documentos de una unidad
    ''' </summary>
    <System.Web.Services.WebMethod()>
    Public Shared Function ObtenerDocumentosUnidad(unidadId As Integer) As Object

        Try
            Dim dt As DataTable = DynamicCrudService.ObtenerTodosConFiltro(TABLA_DOCUMENTOS, "UnidadId = " & unidadId)

            Return New With {.success = True, .data = DataTableHelper.ConvertDataTableToList(dt)}

        Catch ex As Exception
            Return New With {.success = False, .message = "Error: " & ex.Message}

        End Try
    End Function

    ''' <summary>
    ''' Obtiene un documento por ID
    ''' </summary>
    <System.Web.Services.WebMethod()>
    Public Shared Function ObtenerDocumento(id As Integer) As Object

        Try
            Dim registro As DataRow = DynamicCrudService.ObtenerPorId(TABLA_DOCUMENTOS, id)

            If registro Is Nothing Then Return New With {.success = False, .message = "No encontrado"}
            Dim datos As New Dictionary(Of String, Object)

            For Each col As DataColumn In registro.Table.Columns
                datos(col.ColumnName) = If(IsDBNull(registro(col)), Nothing, registro(col))

            Next

            Return New With {.success = True, .data = datos}

        Catch ex As Exception
            Return New With {.success = False, .message = "Error: " & ex.Message}

        End Try
    End Function

    ''' <summary>
    ''' Guarda un documento (crea o actualiza)
    ''' </summary>
    <System.Web.Services.WebMethod()>
    Public Shared Function GuardarDocumento(datos As Dictionary(Of String, Object)) As Object

        Try
            Dim id As Integer = Convert.ToInt32(datos("id"))
            Dim datosGuardar As New Dictionary(Of String, Object)
            datosGuardar.Add("UnidadId", Convert.ToInt32(datos("unidadId")))
            datosGuardar.Add("TipoDocumento", datos("tipoDocumento")?.ToString())
            datosGuardar.Add("Nombre", datos("nombre")?.ToString())
            datosGuardar.Add("Descripcion", datos("descripcion")?.ToString())
            datosGuardar.Add("NumeroDocumento", datos("numeroDocumento")?.ToString())
            datosGuardar.Add("Notaria", datos("notaria")?.ToString())
            datosGuardar.Add("FechaDocumento", If(datos("fechaDocumento") IsNot Nothing, Convert.ToDateTime(datos("fechaDocumento")), DBNull.Value))
            datosGuardar.Add("FechaVigenciaInicio", If(datos("fechaVigenciaInicio") IsNot Nothing, Convert.ToDateTime(datos("fechaVigenciaInicio")), DBNull.Value))
            datosGuardar.Add("FechaVigenciaFin", If(datos("fechaVigenciaFin") IsNot Nothing, Convert.ToDateTime(datos("fechaVigenciaFin")), DBNull.Value))
            datosGuardar.Add("NombreArchivo", datos("nombreArchivo")?.ToString())
            datosGuardar.Add("RutaArchivo", datos("rutaArchivo")?.ToString())
            datosGuardar.Add("Activo", If(Convert.ToBoolean(datos("activo")), 1, 0))

            Dim resultado As Boolean
            Dim nuevoId As Integer = id

            If id = 0 Then
                nuevoId = DynamicCrudService.InsertarConId(TABLA_DOCUMENTOS, datosGuardar)
                resultado = nuevoId > 0
            Else
                resultado = DynamicCrudService.Actualizar(TABLA_DOCUMENTOS, id, datosGuardar)
                nuevoId = id
            End If

            Return New With {
                .success = resultado,
                .message = If(resultado, "Guardado correctamente", "Error al guardar"),
                .data = New With {.id = nuevoId}
            }

        Catch ex As Exception
            Return New With {.success = False, .message = "Error: " & ex.Message}

        End Try
    End Function

    ''' <summary>
    ''' Elimina un documento
    ''' </summary>
    <System.Web.Services.WebMethod()>
    Public Shared Function EliminarDocumento(id As Integer) As Object

        Try
            Dim resultado = DynamicCrudService.Eliminar(TABLA_DOCUMENTOS, id)

            Return New With {.success = resultado, .message = If(resultado, "Eliminado", "Error")}

        Catch ex As Exception
            Return New With {.success = False, .message = "Error: " & ex.Message}

        End Try
    End Function

    ''' <summary>
    ''' Obtiene los archivos de un residente
    ''' </summary>
    <System.Web.Services.WebMethod()>
    Public Shared Function ObtenerArchivosResidente(residenteId As Integer) As Object

        Try
            Dim query As String = "SELECT * FROM " & TABLA_ARCHIVOS_RESIDENTE & " WHERE ResidenteId = " & residenteId & " AND Activo = 1 ORDER BY FechaCreacion DESC"
            Dim dt As DataTable = DynamicCrudService.EjecutarConsulta(query)

            Return New With {.success = True, .data = DataTableHelper.ConvertDataTableToList(dt)}

        Catch ex As Exception
            Return New With {.success = False, .message = "Error: " & ex.Message}

        End Try
    End Function

    ''' <summary>
    ''' Guarda un archivo de residente
    ''' </summary>
    <System.Web.Services.WebMethod()>
    Public Shared Function GuardarArchivoResidente(datos As Dictionary(Of String, Object)) As Object

        Try
            Dim datosGuardar As New Dictionary(Of String, Object)
            datosGuardar.Add("ResidenteId", Convert.ToInt32(datos("residenteId")))
            datosGuardar.Add("TipoArchivo", datos("tipoArchivo")?.ToString())
            datosGuardar.Add("NombreArchivo", datos("nombreArchivo")?.ToString())
            datosGuardar.Add("ArchivoBase64", datos("archivoBase64")?.ToString())
            datosGuardar.Add("TipoMime", datos("tipoMime")?.ToString())
            datosGuardar.Add("TamanioBytes", If(datos.ContainsKey("tamanioBytes") AndAlso datos("tamanioBytes") IsNot Nothing, Convert.ToInt64(datos("tamanioBytes")), DBNull.Value))
            datosGuardar.Add("Activo", 1)

            Dim resultado = DynamicCrudService.Insertar(TABLA_ARCHIVOS_RESIDENTE, datosGuardar)

            Return New With {.success = resultado, .message = If(resultado, "Archivo guardado correctamente", "Error al guardar")}

        Catch ex As Exception
            Return New With {.success = False, .message = "Error: " & ex.Message}

        End Try
    End Function

    ''' <summary>
    ''' Elimina un archivo de residente
    ''' </summary>
    <System.Web.Services.WebMethod()>
    Public Shared Function EliminarArchivoResidente(id As Integer) As Object

        Try
            Dim datosActualizar As New Dictionary(Of String, Object)
            datosActualizar.Add("Activo", 0)
            Dim resultado = DynamicCrudService.Actualizar(TABLA_ARCHIVOS_RESIDENTE, id, datosActualizar)

            Return New With {.success = resultado, .message = If(resultado, "Archivo eliminado", "Error")}

        Catch ex As Exception
            Return New With {.success = False, .message = "Error: " & ex.Message}

        End Try
    End Function

    ''' <summary>
    ''' Obtiene los archivos de un vehículo
    ''' </summary>
    <System.Web.Services.WebMethod()>
    Public Shared Function ObtenerArchivosVehiculo(vehiculoId As Integer) As Object

        Try
            Dim query As String = "SELECT * FROM " & TABLA_ARCHIVOS_VEHICULO & " WHERE VehiculoId = " & vehiculoId & " AND Activo = 1 ORDER BY FechaCreacion DESC"
            Dim dt As DataTable = DynamicCrudService.EjecutarConsulta(query)

            Return New With {.success = True, .data = DataTableHelper.ConvertDataTableToList(dt)}

        Catch ex As Exception
            Return New With {.success = False, .message = "Error: " & ex.Message}

        End Try
    End Function

    ''' <summary>
    ''' Guarda un archivo de vehículo
    ''' </summary>
    <System.Web.Services.WebMethod()>
    Public Shared Function GuardarArchivoVehiculo(datos As Dictionary(Of String, Object)) As Object

        Try
            Dim datosGuardar As New Dictionary(Of String, Object)
            datosGuardar.Add("VehiculoId", Convert.ToInt32(datos("vehiculoId")))
            datosGuardar.Add("TipoArchivo", datos("tipoArchivo")?.ToString())
            datosGuardar.Add("NombreArchivo", datos("nombreArchivo")?.ToString())
            datosGuardar.Add("ArchivoBase64", datos("archivoBase64")?.ToString())
            datosGuardar.Add("TipoMime", datos("tipoMime")?.ToString())
            datosGuardar.Add("TamanioBytes", If(datos.ContainsKey("tamanioBytes") AndAlso datos("tamanioBytes") IsNot Nothing, Convert.ToInt64(datos("tamanioBytes")), DBNull.Value))
            datosGuardar.Add("Activo", 1)

            Dim resultado = DynamicCrudService.Insertar(TABLA_ARCHIVOS_VEHICULO, datosGuardar)

            Return New With {.success = resultado, .message = If(resultado, "Archivo guardado correctamente", "Error al guardar")}

        Catch ex As Exception
            Return New With {.success = False, .message = "Error: " & ex.Message}

        End Try
    End Function

    ''' <summary>
    ''' Elimina un archivo de vehículo
    ''' </summary>
    <System.Web.Services.WebMethod()>
    Public Shared Function EliminarArchivoVehiculo(id As Integer) As Object

        Try
            Dim datosActualizar As New Dictionary(Of String, Object)
            datosActualizar.Add("Activo", 0)
            Dim resultado = DynamicCrudService.Actualizar(TABLA_ARCHIVOS_VEHICULO, id, datosActualizar)

            Return New With {.success = resultado, .message = If(resultado, "Archivo eliminado", "Error")}

        Catch ex As Exception
            Return New With {.success = False, .message = "Error: " & ex.Message}

        End Try
    End Function

    ''' <summary>
    ''' Obtiene los archivos de un documento
    ''' </summary>
    <System.Web.Services.WebMethod()>
    Public Shared Function ObtenerArchivosDocumento(documentoId As Integer) As Object

        Try
            Dim query As String = "SELECT * FROM " & TABLA_ARCHIVOS_DOCUMENTO & " WHERE DocumentoId = " & documentoId & " AND Activo = 1 ORDER BY FechaCreacion DESC"
            Dim dt As DataTable = DynamicCrudService.EjecutarConsulta(query)

            Return New With {.success = True, .data = DataTableHelper.ConvertDataTableToList(dt)}

        Catch ex As Exception
            Return New With {.success = False, .message = "Error: " & ex.Message}

        End Try
    End Function

    ''' <summary>
    ''' Guarda un archivo de documento
    ''' </summary>
    <System.Web.Services.WebMethod()>
    Public Shared Function GuardarArchivoDocumento(datos As Dictionary(Of String, Object)) As Object

        Try
            Dim datosGuardar As New Dictionary(Of String, Object)
            datosGuardar.Add("DocumentoId", Convert.ToInt32(datos("documentoId")))
            datosGuardar.Add("NombreArchivo", datos("nombreArchivo")?.ToString())
            datosGuardar.Add("ArchivoBase64", datos("archivoBase64")?.ToString())
            datosGuardar.Add("TipoMime", datos("tipoMime")?.ToString())
            datosGuardar.Add("TamanioBytes", If(datos.ContainsKey("tamanioBytes") AndAlso datos("tamanioBytes") IsNot Nothing, Convert.ToInt64(datos("tamanioBytes")), DBNull.Value))
            datosGuardar.Add("Activo", 1)

            Dim resultado = DynamicCrudService.Insertar(TABLA_ARCHIVOS_DOCUMENTO, datosGuardar)

            Return New With {.success = resultado, .message = If(resultado, "Archivo guardado correctamente", "Error al guardar")}

        Catch ex As Exception
            Return New With {.success = False, .message = "Error: " & ex.Message}

        End Try
    End Function

    ''' <summary>
    ''' Elimina un archivo de documento
    ''' </summary>
    <System.Web.Services.WebMethod()>
    Public Shared Function EliminarArchivoDocumento(id As Integer) As Object

        Try
            Dim datosActualizar As New Dictionary(Of String, Object)
            datosActualizar.Add("Activo", 0)
            Dim resultado = DynamicCrudService.Actualizar(TABLA_ARCHIVOS_DOCUMENTO, id, datosActualizar)

            Return New With {.success = resultado, .message = If(resultado, "Archivo eliminado", "Error")}

        Catch ex As Exception
            Return New With {.success = False, .message = "Error: " & ex.Message}

        End Try
    End Function

    ''' <summary>
    ''' Procesa el CustomCallback del grid de residentes
    ''' </summary>
    Public Shared Function ProcesarCustomCallbackResidentes(parameters As String, session As System.Web.SessionState.HttpSessionState, unidadIdActual As String) As DataTable
        Try
            Dim partes = parameters.Split("|"c)

            If partes.Length >= 2 AndAlso partes(0) = "cargar" Then
                Dim unidadId As Integer = Integer.Parse(partes(1))

                ' Consulta simplificada - CAST boolean para asegurar tipo correcto
                ' IMPORTANTE: r.Id debe estar primero para que GetRowKey() funcione correctamente
                Dim query As String = "SELECT " &
                                      "r.Id, " &
                                      "IFNULL(e.RazonSocial, '') AS NombreEntidad, " &
                                      "IFNULL(se.RazonSocial, '') AS NombreSubEntidad, " &
                                      "CONCAT(IFNULL(u.Codigo, ''), ' - ', IFNULL(u.Nombre, '')) AS NombreUnidad, " &
                                      "CONCAT(IFNULL(r.Nombre, ''), ' ', IFNULL(r.ApellidoPaterno, ''), ' ', IFNULL(r.ApellidoMaterno,'')) AS NombreCompleto, " &
                                      "IFNULL(r.TipoResidente, '') AS TipoResidente, " &
                                      "CAST(IFNULL(r.EsPrincipal, 0) AS UNSIGNED) AS EsPrincipal, " &
                                      "IFNULL(r.Email, '') AS Email, " &
                                      "IFNULL(r.TelefonoCelular, '') AS Celular, " &
                                      "IFNULL(r.CURP, '') AS CURP " &
                                      "FROM cat_residentes r " &
                                      "INNER JOIN cat_unidades u ON r.UnidadId = u.Id " &
                                      "INNER JOIN cat_entidades e ON u.entidad_id = e.Id " &
                                      "LEFT JOIN cat_sub_entidades se ON u.SubEntidadId = se.Id " &
                                      "WHERE r.UnidadId = " & unidadId & " " &
                                      "ORDER BY r.EsPrincipal DESC, r.Nombre"

                Logger.LogInfo($"UnidadesService.ProcesarCustomCallbackResidentes - Query: {query}")

                Dim dt As DataTable = DynamicCrudService.EjecutarConsulta(query)

                ' Convertir columna EsPrincipal a Boolean si existe
                If dt IsNot Nothing AndAlso dt.Columns.Contains("EsPrincipal") Then
                    Dim colEsPrincipal = dt.Columns("EsPrincipal")

                    If colEsPrincipal.DataType IsNot GetType(Boolean) Then
                        ' Crear nueva columna Boolean
                        Dim nuevaCol As New DataColumn("EsPrincipal_Bool", GetType(Boolean))

                        dt.Columns.Add(nuevaCol)

                        ' Copiar valores convertidos

                        For Each row As DataRow In dt.Rows
                            If Not IsDBNull(row("EsPrincipal")) Then
                                Dim valor = row("EsPrincipal")

                                row("EsPrincipal_Bool") = (Convert.ToInt32(valor) <> 0)
                            Else
                                row("EsPrincipal_Bool") = False
                            End If

                        Next

                        ' Remover columna original y renombrar la nueva
                        dt.Columns.Remove("EsPrincipal")
                        nuevaCol.ColumnName = "EsPrincipal"
                    End If
                End If

                Logger.LogInfo($"UnidadesService.ProcesarCustomCallbackResidentes - Rows: {If(dt IsNot Nothing, dt.Rows.Count, 0)}")

                Return dt
            ElseIf partes.Length >= 2 AndAlso partes(0) = "agregarUnbound" Then
                ' Agregar fila unbound (sin guardar en BD)
                ' Formato: agregarUnbound|unidadId|datosJson
                Dim unidadId As Integer = 0
                Dim datosJson As String = ""

                If partes.Length >= 3 Then
                    ' Si viene el unidadId como parámetro
                    Integer.TryParse(partes(1), unidadId)
                    datosJson = partes(2)
                Else
                    ' Si no viene unidadId, usar datosJson directamente
                    datosJson = partes(1)
                    ' Intentar obtener desde Session
                    If session("currentUnidadId") IsNot Nothing Then
                        Integer.TryParse(session("currentUnidadId").ToString(), unidadId)
                    End If
                    ' Intentar obtener desde el campo oculto si está disponible
                    If unidadId = 0 AndAlso Not String.IsNullOrEmpty(unidadIdActual) Then
                        Integer.TryParse(unidadIdActual, unidadId)
                    End If
                End If

                Dim dt As DataTable = TryCast(session("dtResidentes"), DataTable)

                ' Si no existe el DataTable, crearlo con la estructura básica
                If dt Is Nothing Then

                    If unidadId > 0 Then
                        ' Cargar desde BD para obtener estructura completa
                        Dim query As String = "SELECT " &
                                              "IFNULL(e.RazonSocial, '') AS NombreEntidad, " &
                                              "IFNULL(se.RazonSocial, '') AS NombreSubEntidad, " &
                                              "CONCAT(IFNULL(u.Codigo, ''), ' - ', IFNULL(u.Nombre, '')) AS NombreUnidad, " &
                                              "CONCAT(IFNULL(r.Nombre, ''), ' ', IFNULL(r.ApellidoPaterno, ''), ' ', IFNULL(r.ApellidoMaterno,'')) AS NombreCompleto, " &
                                              "IFNULL(r.TipoResidente, '') AS TipoResidente, " &
                                              "CAST(IFNULL(r.EsPrincipal, 0) AS UNSIGNED) AS EsPrincipal, " &
                                              "IFNULL(r.Email, '') AS Email, " &
                                              "IFNULL(r.TelefonoCelular, '') AS Celular, " &
                                              "IFNULL(r.CURP, '') AS CURP " &
                                              "FROM cat_residentes r " &
                                              "INNER JOIN cat_unidades u ON r.UnidadId = u.Id " &
                                              "INNER JOIN cat_entidades e ON u.entidad_id = e.Id " &
                                              "LEFT JOIN cat_sub_entidades se ON u.SubEntidadId = se.Id " &
                                              "WHERE r.UnidadId = " & unidadId & " " &
                                              "ORDER BY r.EsPrincipal DESC, r.Nombre"
                        dt = DynamicCrudService.EjecutarConsulta(query)

                        ' Convertir columna EsPrincipal a Boolean si existe
                        If dt IsNot Nothing AndAlso dt.Columns.Contains("EsPrincipal") Then
                            Dim colEsPrincipal = dt.Columns("EsPrincipal")

                            If colEsPrincipal.DataType IsNot GetType(Boolean) Then
                                ' Crear nueva columna Boolean
                                Dim nuevaCol As New DataColumn("EsPrincipal_Bool", GetType(Boolean))

                                dt.Columns.Add(nuevaCol)

                                ' Copiar valores convertidos

                                For Each row As DataRow In dt.Rows
                                    If Not IsDBNull(row("EsPrincipal")) Then
                                        Dim valor = row("EsPrincipal")

                                        row("EsPrincipal_Bool") = (Convert.ToInt32(valor) <> 0)
                                    Else
                                        row("EsPrincipal_Bool") = False
                                    End If

                                Next

                                ' Remover columna original y renombrar la nueva
                                dt.Columns.Remove("EsPrincipal")
                                nuevaCol.ColumnName = "EsPrincipal"
                            End If
                        End If
                    Else
                        ' Crear estructura mínima si no hay unidad
                        dt = New DataTable()
                        dt.Columns.Add("NombreEntidad", GetType(String))
                        dt.Columns.Add("NombreSubEntidad", GetType(String))
                        dt.Columns.Add("NombreUnidad", GetType(String))
                        dt.Columns.Add("NombreCompleto", GetType(String))
                        dt.Columns.Add("TipoResidente", GetType(String))
                        dt.Columns.Add("EsPrincipal", GetType(Boolean))
                        dt.Columns.Add("Email", GetType(String))
                        dt.Columns.Add("Celular", GetType(String))
                        dt.Columns.Add("CURP", GetType(String))
                    End If
                End If

                ' Parsear JSON y agregar fila
                Dim datos As Dictionary(Of String, Object) = Newtonsoft.Json.JsonConvert.DeserializeObject(Of Dictionary(Of String, Object))(datosJson)
                Dim nuevaFila = dt.NewRow()

                ' Generar ID temporal negativo para identificar filas unbound
                Dim nuevoId = -(dt.Rows.Count + 1)

                nuevaFila("Id") = nuevoId

                nuevaFila("Nombre") = If(datos.ContainsKey("Nombre") AndAlso datos("Nombre") IsNot Nothing, datos("Nombre").ToString(), "")
                nuevaFila("ApellidoPaterno") = If(datos.ContainsKey("ApellidoPaterno") AndAlso datos("ApellidoPaterno") IsNot Nothing, datos("ApellidoPaterno").ToString(), "")
                nuevaFila("ApellidoMaterno") = If(datos.ContainsKey("ApellidoMaterno") AndAlso datos("ApellidoMaterno") IsNot Nothing, datos("ApellidoMaterno").ToString(), "")

                Dim nombreCompleto = Trim(nuevaFila("Nombre").ToString() & " " & nuevaFila("ApellidoPaterno").ToString() & " " & nuevaFila("ApellidoMaterno").ToString())

                nuevaFila("NombreCompleto") = nombreCompleto

                If dt.Columns.Contains("TipoResidente") Then
                    Dim tipoResidente = If(datos.ContainsKey("TipoResidente") AndAlso datos("TipoResidente") IsNot Nothing, datos("TipoResidente").ToString(), "")

                    nuevaFila("TipoResidente") = If(String.IsNullOrEmpty(tipoResidente), "Propietario", tipoResidente)
                End If

                If dt.Columns.Contains("EsPrincipal") Then
                    Dim esPrincipalValor As Boolean = False

                    If datos.ContainsKey("EsPrincipal") AndAlso datos("EsPrincipal") IsNot Nothing Then
                        Boolean.TryParse(datos("EsPrincipal").ToString(), esPrincipalValor)
                    End If
                    nuevaFila("EsPrincipal") = esPrincipalValor
                End If

                If dt.Columns.Contains("Email") Then
                    nuevaFila("Email") = If(datos.ContainsKey("Email") AndAlso datos("Email") IsNot Nothing, datos("Email").ToString(), "")
                End If

                If dt.Columns.Contains("Telefono") Then
                    nuevaFila("Telefono") = If(datos.ContainsKey("Telefono") AndAlso datos("Telefono") IsNot Nothing, datos("Telefono").ToString(), "")
                End If

                If dt.Columns.Contains("Celular") Then
                    nuevaFila("Celular") = If(datos.ContainsKey("Celular") AndAlso datos("Celular") IsNot Nothing, datos("Celular").ToString(), "")
                End If

                If dt.Columns.Contains("CURP") Then
                    nuevaFila("CURP") = If(datos.ContainsKey("CURP") AndAlso datos("CURP") IsNot Nothing, datos("CURP").ToString(), "")
                End If

                If dt.Columns.Contains("Activo") Then
                    Dim activoValor As Boolean = True

                    If datos.ContainsKey("Activo") AndAlso datos("Activo") IsNot Nothing Then
                        Boolean.TryParse(datos("Activo").ToString(), activoValor)
                    End If
                    nuevaFila("Activo") = activoValor
                End If

                dt.Rows.Add(nuevaFila)

                Return dt
            End If

            Return Nothing
        Catch ex As Exception
            Logger.LogError("UnidadesService.ProcesarCustomCallbackResidentes", ex)
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Procesa el CustomCallback del grid de vehículos
    ''' </summary>
    Public Shared Function ProcesarCustomCallbackVehiculos(parameters As String, session As System.Web.SessionState.HttpSessionState, unidadIdActual As String) As DataTable
        Try
            Dim partes = parameters.Split("|"c)

            If partes.Length >= 2 AndAlso partes(0) = "cargar" Then
                Dim unidadId As Integer = Integer.Parse(partes(1))

                ' IMPORTANTE: v.Id debe estar primero y usar alias "Id" para que GetRowKey() funcione correctamente
                Dim query As String = "SELECT " &
                                      "v.Id, " &
                                      "CONCAT(IFNULL(u.Codigo, ''), ' - ', IFNULL(u.Nombre, '')) AS NombreUnidad, " &
                                      "CONCAT(IFNULL(r.Nombre, ''), ' ', IFNULL(r.ApellidoPaterno, ''), ' ', IFNULL(r.ApellidoMaterno, '')) AS NombreResidente, " &
                                      "IFNULL(v.Placas, '') AS Placas, " &
                                      "IFNULL(v.TipoVehiculo, '') AS TipoVehiculo, " &
                                      "IFNULL(v.Marca, '') AS Marca, " &
                                      "IFNULL(v.Modelo, '') AS Modelo, " &
                                      "IFNULL(v.Anio, 0) AS Anio, " &
                                      "IFNULL(v.Color, '') AS Color, " &
                                      "IFNULL(v.NumeroTarjeton, '') AS NumeroTarjeton, " &
                                      "IFNULL(v.FechaVigenciaTarjeta, NULL) AS FechaVigencia, " &
                                      "CAST(IFNULL(v.Activo, 0) AS UNSIGNED) AS Activo " &
                                      "FROM cat_vehiculos_unidad v " &
                                      "INNER JOIN cat_unidades u ON v.UnidadId = u.Id " &
                                      "LEFT JOIN cat_residentes r ON v.ResidenteId = r.Id " &
                                      "WHERE v.UnidadId = " & unidadId & " ORDER BY v.Placas"
                Dim dt As DataTable = DynamicCrudService.EjecutarConsulta(query)

                ' Convertir columna Activo a Boolean si existe
                If dt IsNot Nothing AndAlso dt.Columns.Contains("Activo") Then
                    Dim colActivo = dt.Columns("Activo")

                    If colActivo.DataType IsNot GetType(Boolean) Then
                        ' Crear nueva columna Boolean
                        Dim nuevaCol As New DataColumn("Activo_Bool", GetType(Boolean))

                        dt.Columns.Add(nuevaCol)

                        ' Copiar valores convertidos

                        For Each row As DataRow In dt.Rows
                            If Not IsDBNull(row("Activo")) Then
                                Dim valor = row("Activo")

                                row("Activo_Bool") = (Convert.ToInt32(valor) <> 0)
                            Else
                                row("Activo_Bool") = False
                            End If

                        Next

                        ' Remover columna original y renombrar la nueva
                        dt.Columns.Remove("Activo")
                        nuevaCol.ColumnName = "Activo"
                    End If
                End If

                Return dt
            ElseIf partes.Length >= 2 AndAlso partes(0) = "agregarUnbound" Then
                ' Agregar fila unbound (sin guardar en BD)
                ' Formato: agregarUnbound|unidadId|datosJson
                Dim unidadId As Integer = 0
                Dim datosJson As String = ""

                If partes.Length >= 3 Then
                    ' Si viene el unidadId como parámetro
                    Integer.TryParse(partes(1), unidadId)
                    datosJson = partes(2)
                Else
                    ' Si no viene unidadId, usar datosJson directamente
                    datosJson = partes(1)
                    ' Intentar obtener desde Session
                    If session("currentUnidadId") IsNot Nothing Then
                        Integer.TryParse(session("currentUnidadId").ToString(), unidadId)
                    End If
                    ' Intentar obtener desde el campo oculto si está disponible
                    If unidadId = 0 AndAlso Not String.IsNullOrEmpty(unidadIdActual) Then
                        Integer.TryParse(unidadIdActual, unidadId)
                    End If
                End If

                Dim dt As DataTable = TryCast(session("dtVehiculos"), DataTable)

                ' Si no existe el DataTable, crearlo
                If dt Is Nothing Then
                    If unidadId > 0 Then
                        ' Cargar desde BD para obtener estructura completa
                        ' IMPORTANTE: v.Id debe estar primero y usar alias "Id" para que GetRowKey() funcione correctamente
                        Dim query As String = "SELECT " &
                                              "v.Id, " &
                                              "CONCAT(IFNULL(u.Codigo, ''), ' - ', IFNULL(u.Nombre, '')) AS NombreUnidad, " &
                                              "CONCAT(IFNULL(r.Nombre, ''), ' ', IFNULL(r.ApellidoPaterno, ''), ' ', IFNULL(r.ApellidoMaterno, '')) AS NombreResidente, " &
                                              "IFNULL(v.Placas, '') AS Placas, " &
                                              "IFNULL(v.TipoVehiculo, '') AS TipoVehiculo, " &
                                              "IFNULL(v.Marca, '') AS Marca, " &
                                              "IFNULL(v.Modelo, '') AS Modelo, " &
                                              "IFNULL(v.Anio, 0) AS Anio, " &
                                              "IFNULL(v.Color, '') AS Color, " &
                                              "IFNULL(v.NumeroTarjeton, '') AS NumeroTarjeton, " &
                                              "IFNULL(v.FechaVigenciaTarjeta, NULL) AS FechaVigencia, " &
                                              "CAST(IFNULL(v.Activo, 0) AS UNSIGNED) AS Activo " &
                                              "FROM cat_vehiculos_unidad v " &
                                              "INNER JOIN cat_unidades u ON v.UnidadId = u.Id " &
                                              "LEFT JOIN cat_residentes r ON v.ResidenteId = r.Id " &
                                              "WHERE v.UnidadId = " & unidadId & " ORDER BY v.Placas"
                        dt = DynamicCrudService.EjecutarConsulta(query)
                    Else
                        ' Crear estructura mínima con las columnas solicitadas
                        ' IMPORTANTE: La columna debe llamarse "Id" para que GetRowKey() funcione correctamente
                        dt = New DataTable()
                        dt.Columns.Add("Id", GetType(Integer))
                        dt.Columns.Add("NombreUnidad", GetType(String))
                        dt.Columns.Add("NombreResidente", GetType(String))
                        dt.Columns.Add("Placas", GetType(String))
                        dt.Columns.Add("TipoVehiculo", GetType(String))
                        dt.Columns.Add("Marca", GetType(String))
                        dt.Columns.Add("Modelo", GetType(String))
                        dt.Columns.Add("Anio", GetType(Integer))
                        dt.Columns.Add("Color", GetType(String))
                        dt.Columns.Add("NumeroTarjeton", GetType(String))
                        dt.Columns.Add("FechaVigencia", GetType(DateTime))
                        dt.Columns.Add("Activo", GetType(Boolean))
                    End If
                End If

                ' Parsear JSON y agregar fila
                Dim datos As Dictionary(Of String, Object) = Newtonsoft.Json.JsonConvert.DeserializeObject(Of Dictionary(Of String, Object))(datosJson)
                Dim nuevaFila = dt.NewRow()

                ' Generar ID temporal negativo para identificar filas unbound
                Dim nuevoId = -(dt.Rows.Count + 1)

                ' IMPORTANTE: La columna debe llamarse "Id" para que GetRowKey() funcione correctamente
                If dt.Columns.Contains("Id") Then
                    nuevaFila("Id") = nuevoId
                End If

                If dt.Columns.Contains("NombreUnidad") Then
                    nuevaFila("NombreUnidad") = ""
                End If

                If dt.Columns.Contains("Placas") Then
                    nuevaFila("Placas") = If(datos.ContainsKey("Placas") AndAlso datos("Placas") IsNot Nothing, datos("Placas").ToString(), "")
                End If

                If dt.Columns.Contains("TipoVehiculo") Then
                    Dim tipoVehiculo = If(datos.ContainsKey("TipoVehiculo") AndAlso datos("TipoVehiculo") IsNot Nothing, datos("TipoVehiculo").ToString(), "")

                    nuevaFila("TipoVehiculo") = If(String.IsNullOrEmpty(tipoVehiculo), "Automóvil", tipoVehiculo)
                End If

                If dt.Columns.Contains("Marca") Then
                    nuevaFila("Marca") = If(datos.ContainsKey("Marca") AndAlso datos("Marca") IsNot Nothing, datos("Marca").ToString(), "")
                End If

                If dt.Columns.Contains("Modelo") Then
                    nuevaFila("Modelo") = If(datos.ContainsKey("Modelo") AndAlso datos("Modelo") IsNot Nothing, datos("Modelo").ToString(), "")
                End If

                If dt.Columns.Contains("Anio") Then
                    Dim anioValor As Object = DBNull.Value

                    If datos.ContainsKey("Anio") AndAlso datos("Anio") IsNot Nothing Then

                        Try
                            anioValor = CInt(datos("Anio"))

                        Catch
                            anioValor = DBNull.Value

                        End Try
                    End If
                    nuevaFila("Anio") = anioValor
                End If

                If dt.Columns.Contains("Color") Then
                    nuevaFila("Color") = If(datos.ContainsKey("Color") AndAlso datos("Color") IsNot Nothing, datos("Color").ToString(), "")
                End If

                If dt.Columns.Contains("NombreResidente") Then
                    nuevaFila("NombreResidente") = ""
                ElseIf dt.Columns.Contains("ResidenteNombre") Then
                    nuevaFila("ResidenteNombre") = ""
                End If

                If dt.Columns.Contains("NumeroTarjeton") Then
                    nuevaFila("NumeroTarjeton") = If(datos.ContainsKey("NumeroTarjeton") AndAlso datos("NumeroTarjeton") IsNot Nothing, datos("NumeroTarjeton").ToString(), "")
                End If

                If dt.Columns.Contains("FechaVigencia") Then
                    Dim fechaVigenciaValor As Object = DBNull.Value

                    If datos.ContainsKey("FechaVigenciaTarjeta") AndAlso datos("FechaVigenciaTarjeta") IsNot Nothing Then

                        Try
                            fechaVigenciaValor = Convert.ToDateTime(datos("FechaVigenciaTarjeta"))

                        Catch
                            fechaVigenciaValor = DBNull.Value

                        End Try
                    End If
                    nuevaFila("FechaVigencia") = fechaVigenciaValor
                End If

                If dt.Columns.Contains("Activo") Then
                    Dim activoValor As Boolean = True

                    If datos.ContainsKey("Activo") AndAlso datos("Activo") IsNot Nothing Then
                        Boolean.TryParse(datos("Activo").ToString(), activoValor)
                    End If
                    nuevaFila("Activo") = activoValor
                End If

                dt.Rows.Add(nuevaFila)

                Return dt
            End If

            Return Nothing
        Catch ex As Exception
            Logger.LogError("UnidadesService.ProcesarCustomCallbackVehiculos", ex)
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Procesa el CustomCallback del grid de tags
    ''' </summary>
    Public Shared Function ProcesarCustomCallbackTags(parameters As String, session As System.Web.SessionState.HttpSessionState, unidadIdActual As String) As DataTable
        Try
            Dim partes = parameters.Split("|"c)

            If partes.Length >= 2 AndAlso partes(0) = "cargar" Then
                Dim unidadId As Integer = Integer.Parse(partes(1))
                Dim query As String = "SELECT t.*, CONCAT(r.Nombre, ' ', r.ApellidoPaterno) AS ResidenteNombre " &
                                      "FROM cat_tags_unidad t LEFT JOIN cat_residentes r ON t.ResidenteId = r.Id " &
                                      "WHERE t.UnidadId = " & unidadId & " ORDER BY t.CodigoTag"
                Dim dt As DataTable = DynamicCrudService.EjecutarConsulta(query)

                Return dt
            ElseIf partes.Length >= 2 AndAlso partes(0) = "agregarUnbound" Then
                ' Agregar fila unbound (sin guardar en BD)
                Dim unidadId As Integer = 0
                Dim datosJson As String = ""

                If partes.Length >= 3 Then
                    Integer.TryParse(partes(1), unidadId)
                    datosJson = partes(2)
                Else
                    datosJson = partes(1)
                    If session("currentUnidadId") IsNot Nothing Then
                        Integer.TryParse(session("currentUnidadId").ToString(), unidadId)
                    End If
                    If unidadId = 0 AndAlso Not String.IsNullOrEmpty(unidadIdActual) Then
                        Integer.TryParse(unidadIdActual, unidadId)
                    End If
                End If

                Dim dt As DataTable = TryCast(session("dtTags"), DataTable)

                If dt Is Nothing Then
                    If unidadId > 0 Then
                        Dim query As String = "SELECT t.*, CONCAT(r.Nombre, ' ', r.ApellidoPaterno) AS ResidenteNombre " &
                                              "FROM cat_tags_unidad t LEFT JOIN cat_residentes r ON t.ResidenteId = r.Id " &
                                              "WHERE t.UnidadId = " & unidadId & " ORDER BY t.CodigoTag"
                        dt = DynamicCrudService.EjecutarConsulta(query)
                    Else
                        dt = New DataTable()
                        dt.Columns.Add("Id", GetType(Integer))
                        dt.Columns.Add("CodigoTag", GetType(String))
                        dt.Columns.Add("TipoTag", GetType(String))
                        dt.Columns.Add("Descripcion", GetType(String))
                        dt.Columns.Add("ResidenteNombre", GetType(String))
                        dt.Columns.Add("Activo", GetType(Boolean))
                    End If
                End If

                Dim datos As Dictionary(Of String, Object) = Newtonsoft.Json.JsonConvert.DeserializeObject(Of Dictionary(Of String, Object))(datosJson)
                Dim nuevaFila = dt.NewRow()
                Dim nuevoId = -(dt.Rows.Count + 1)

                nuevaFila("Id") = nuevoId

                If dt.Columns.Contains("CodigoTag") Then
                    nuevaFila("CodigoTag") = If(datos.ContainsKey("CodigoTag") AndAlso datos("CodigoTag") IsNot Nothing, datos("CodigoTag").ToString(), "")
                End If

                If dt.Columns.Contains("TipoTag") Then
                    nuevaFila("TipoTag") = If(datos.ContainsKey("TipoTag") AndAlso datos("TipoTag") IsNot Nothing, datos("TipoTag").ToString(), "")
                End If

                If dt.Columns.Contains("Descripcion") Then
                    nuevaFila("Descripcion") = If(datos.ContainsKey("Descripcion") AndAlso datos("Descripcion") IsNot Nothing, datos("Descripcion").ToString(), "")
                End If

                If dt.Columns.Contains("ResidenteNombre") Then
                    nuevaFila("ResidenteNombre") = If(datos.ContainsKey("ResidenteNombre") AndAlso datos("ResidenteNombre") IsNot Nothing, datos("ResidenteNombre").ToString(), "")
                End If

                If dt.Columns.Contains("Activo") Then
                    Dim activoValor As Boolean = True

                    If datos.ContainsKey("Activo") AndAlso datos("Activo") IsNot Nothing Then
                        Boolean.TryParse(datos("Activo").ToString(), activoValor)
                    End If
                    nuevaFila("Activo") = activoValor
                End If

                dt.Rows.Add(nuevaFila)

                Return dt
            End If

            Return Nothing
        Catch ex As Exception
            Logger.LogError("UnidadesService.ProcesarCustomCallbackTags", ex)
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Procesa el CustomCallback del grid de documentos
    ''' </summary>
    Public Shared Function ProcesarCustomCallbackDocumentos(parameters As String, session As System.Web.SessionState.HttpSessionState, unidadIdActual As String) As DataTable
        Try
            Dim partes = parameters.Split("|"c)

            If partes.Length >= 2 AndAlso partes(0) = "cargar" Then
                Dim unidadId As Integer = Integer.Parse(partes(1))
                Dim dt As DataTable = DynamicCrudService.ObtenerTodosConFiltro("cat_documentos_unidad", "UnidadId = " & unidadId)

                Return dt
            ElseIf partes.Length >= 2 AndAlso partes(0) = "agregarUnbound" Then
                ' Agregar fila unbound (sin guardar en BD)
                Dim unidadId As Integer = 0
                Dim datosJson As String = ""

                If partes.Length >= 3 Then
                    Integer.TryParse(partes(1), unidadId)
                    datosJson = partes(2)
                Else
                    datosJson = partes(1)
                    If session("currentUnidadId") IsNot Nothing Then
                        Integer.TryParse(session("currentUnidadId").ToString(), unidadId)
                    End If
                    If unidadId = 0 AndAlso Not String.IsNullOrEmpty(unidadIdActual) Then
                        Integer.TryParse(unidadIdActual, unidadId)
                    End If
                End If

                Dim dt As DataTable = TryCast(session("dtDocumentos"), DataTable)

                If dt Is Nothing Then
                    If unidadId > 0 Then
                        dt = DynamicCrudService.ObtenerTodosConFiltro("cat_documentos_unidad", "UnidadId = " & unidadId)
                    Else
                        dt = New DataTable()
                        dt.Columns.Add("Id", GetType(Integer))
                        dt.Columns.Add("TipoDocumento", GetType(String))
                        dt.Columns.Add("NumeroDocumento", GetType(String))
                        dt.Columns.Add("Nombre", GetType(String))
                        dt.Columns.Add("FechaDocumento", GetType(DateTime))
                        dt.Columns.Add("Activo", GetType(Boolean))
                    End If
                End If

                Dim datos As Dictionary(Of String, Object) = Newtonsoft.Json.JsonConvert.DeserializeObject(Of Dictionary(Of String, Object))(datosJson)
                Dim nuevaFila = dt.NewRow()
                Dim nuevoId = -(dt.Rows.Count + 1)

                nuevaFila("Id") = nuevoId

                If dt.Columns.Contains("TipoDocumento") Then
                    nuevaFila("TipoDocumento") = If(datos.ContainsKey("TipoDocumento") AndAlso datos("TipoDocumento") IsNot Nothing, datos("TipoDocumento").ToString(), "")
                End If

                If dt.Columns.Contains("NumeroDocumento") Then
                    nuevaFila("NumeroDocumento") = If(datos.ContainsKey("NumeroDocumento") AndAlso datos("NumeroDocumento") IsNot Nothing, datos("NumeroDocumento").ToString(), "")
                End If

                If dt.Columns.Contains("Nombre") Then
                    nuevaFila("Nombre") = If(datos.ContainsKey("Nombre") AndAlso datos("Nombre") IsNot Nothing, datos("Nombre").ToString(), "")
                End If

                If dt.Columns.Contains("FechaDocumento") Then
                    Dim fechaValor As Object = DBNull.Value

                    If datos.ContainsKey("FechaDocumento") AndAlso datos("FechaDocumento") IsNot Nothing Then

                        Try
                            fechaValor = DateTime.Parse(datos("FechaDocumento").ToString())

                        Catch
                            fechaValor = DBNull.Value

                        End Try
                    End If
                    nuevaFila("FechaDocumento") = fechaValor
                End If

                If dt.Columns.Contains("Activo") Then
                    Dim activoValor As Boolean = True

                    If datos.ContainsKey("Activo") AndAlso datos("Activo") IsNot Nothing Then
                        Boolean.TryParse(datos("Activo").ToString(), activoValor)
                    End If
                    nuevaFila("Activo") = activoValor
                End If

                dt.Rows.Add(nuevaFila)

                Return dt
            End If

            Return Nothing
        Catch ex As Exception
            Logger.LogError("UnidadesService.ProcesarCustomCallbackDocumentos", ex)
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Procesa el CustomCallback del grid de archivos de residente
    ''' </summary>
    Public Shared Function ProcesarCustomCallbackArchivosResidente(parameters As String, session As System.Web.SessionState.HttpSessionState) As DataTable
        Try
            Dim partes = parameters.Split("|"c)

            If partes.Length >= 2 AndAlso partes(0) = "cargar" Then
                Dim residenteId As Integer = Integer.Parse(partes(1))
                Dim dt As DataTable = New DataTable()

                ' Si hay residenteId válido, cargar desde BD
                If residenteId > 0 Then
                    Dim query As String = "SELECT * FROM cat_residente_archivos WHERE ResidenteId = " & residenteId & " AND Activo = 1 ORDER BY FechaCreacion DESC"

                    dt = DynamicCrudService.EjecutarConsulta(query)

                    ' Agregar archivos unbound si existen (solo si hay residenteId válido)
                    If session("archivosResidenteUnbound") IsNot Nothing Then
                        Dim archivosUnbound As List(Of Dictionary(Of String, Object)) = TryCast(session("archivosResidenteUnbound"), List(Of Dictionary(Of String, Object)))

                        If archivosUnbound IsNot Nothing AndAlso archivosUnbound.Count > 0 Then

                            For Each archivoUnbound In archivosUnbound
                                Dim nuevaFila As DataRow = dt.NewRow()

                                nuevaFila("Id") = archivoUnbound("Id")
                                nuevaFila("ResidenteId") = residenteId
                                nuevaFila("TipoArchivo") = If(archivoUnbound.ContainsKey("TipoArchivo"), archivoUnbound("TipoArchivo").ToString(), "INE")
                                nuevaFila("NombreArchivo") = If(archivoUnbound.ContainsKey("NombreArchivo"), archivoUnbound("NombreArchivo").ToString(), "")
                                nuevaFila("TamanioBytes") = If(archivoUnbound.ContainsKey("TamanioBytes"), archivoUnbound("TamanioBytes"), 0)
                                nuevaFila("TipoMime") = If(archivoUnbound.ContainsKey("TipoMime"), archivoUnbound("TipoMime").ToString(), "")
                                nuevaFila("FechaCreacion") = If(archivoUnbound.ContainsKey("FechaCreacion"), DateTime.Parse(archivoUnbound("FechaCreacion").ToString()), DateTime.Now)
                                nuevaFila("Activo") = If(archivoUnbound.ContainsKey("Activo"), archivoUnbound("Activo"), 1)
                                dt.Rows.InsertAt(nuevaFila, 0)

                            Next

                        End If
                    End If
                Else
                    ' Si no hay residenteId (0), limpiar Session y crear tabla vacía
                    session("archivosResidenteUnbound") = Nothing
                    session("dtArchivosResidente") = Nothing

                    dt.Columns.Add("Id", GetType(Integer))
                    dt.Columns.Add("ResidenteId", GetType(Integer))
                    dt.Columns.Add("TipoArchivo", GetType(String))
                    dt.Columns.Add("NombreArchivo", GetType(String))
                    dt.Columns.Add("TamanioBytes", GetType(Long))
                    dt.Columns.Add("TipoMime", GetType(String))
                    dt.Columns.Add("FechaCreacion", GetType(DateTime))
                    dt.Columns.Add("Activo", GetType(Integer))
                End If

                Return dt
            ElseIf partes.Length >= 2 AndAlso partes(0) = "agregarUnbound" Then
                ' Agregar archivo unbound al grid
                Dim datosJson As String = partes(1)
                Dim datos As Dictionary(Of String, Object) = Newtonsoft.Json.JsonConvert.DeserializeObject(Of Dictionary(Of String, Object))(datosJson)

                ' Obtener o crear lista de archivos unbound en Session
                Dim archivosUnbound As List(Of Dictionary(Of String, Object))

                If session("archivosResidenteUnbound") Is Nothing Then
                    archivosUnbound = New List(Of Dictionary(Of String, Object))
                    session("archivosResidenteUnbound") = archivosUnbound
                Else
                    archivosUnbound = TryCast(session("archivosResidenteUnbound"), List(Of Dictionary(Of String, Object)))
                    If archivosUnbound Is Nothing Then
                        archivosUnbound = New List(Of Dictionary(Of String, Object))
                        session("archivosResidenteUnbound") = archivosUnbound
                    End If
                End If

                ' Agregar el nuevo archivo a la lista
                archivosUnbound.Add(datos)

                ' Obtener DataTable actual o crear uno nuevo
                Dim dtUnbound As DataTable = TryCast(session("dtArchivosResidente"), DataTable)

                If dtUnbound Is Nothing Then
                    dtUnbound = New DataTable()
                    dtUnbound.Columns.Add("Id", GetType(Integer))
                    dtUnbound.Columns.Add("ResidenteId", GetType(Integer))
                    dtUnbound.Columns.Add("TipoArchivo", GetType(String))
                    dtUnbound.Columns.Add("NombreArchivo", GetType(String))
                    dtUnbound.Columns.Add("TamanioBytes", GetType(Long))
                    dtUnbound.Columns.Add("TipoMime", GetType(String))
                    dtUnbound.Columns.Add("FechaCreacion", GetType(DateTime))
                    dtUnbound.Columns.Add("Activo", GetType(Integer))
                End If

                ' Agregar nueva fila al DataTable
                Dim nuevaFila As DataRow = dtUnbound.NewRow()

                nuevaFila("Id") = If(datos.ContainsKey("Id") AndAlso datos("Id") IsNot Nothing, Integer.Parse(datos("Id").ToString()), 0)
                nuevaFila("ResidenteId") = If(datos.ContainsKey("ResidenteId") AndAlso datos("ResidenteId") IsNot Nothing, Integer.Parse(datos("ResidenteId").ToString()), 0)
                nuevaFila("TipoArchivo") = If(datos.ContainsKey("TipoArchivo") AndAlso datos("TipoArchivo") IsNot Nothing, datos("TipoArchivo").ToString(), "INE")
                nuevaFila("NombreArchivo") = If(datos.ContainsKey("NombreArchivo") AndAlso datos("NombreArchivo") IsNot Nothing, datos("NombreArchivo").ToString(), "")
                nuevaFila("TamanioBytes") = If(datos.ContainsKey("TamanioBytes") AndAlso datos("TamanioBytes") IsNot Nothing, Long.Parse(datos("TamanioBytes").ToString()), 0)
                nuevaFila("TipoMime") = If(datos.ContainsKey("TipoMime") AndAlso datos("TipoMime") IsNot Nothing, datos("TipoMime").ToString(), "")
                nuevaFila("FechaCreacion") = If(datos.ContainsKey("FechaCreacion") AndAlso datos("FechaCreacion") IsNot Nothing, DateTime.Parse(datos("FechaCreacion").ToString()), DateTime.Now)
                nuevaFila("Activo") = If(datos.ContainsKey("Activo") AndAlso datos("Activo") IsNot Nothing, Integer.Parse(datos("Activo").ToString()), 1)
                dtUnbound.Rows.InsertAt(nuevaFila, 0)

                Return dtUnbound
            End If

            Return Nothing
        Catch ex As Exception
            Logger.LogError("UnidadesService.ProcesarCustomCallbackArchivosResidente", ex)
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Procesa el CustomCallback del grid de archivos de vehículo
    ''' </summary>
    Public Shared Function ProcesarCustomCallbackArchivosVehiculo(parameters As String, session As System.Web.SessionState.HttpSessionState) As DataTable
        Try
            Dim partes = parameters.Split("|"c)

            If partes.Length >= 2 AndAlso partes(0) = "cargar" Then
                Dim vehiculoId As Integer = Integer.Parse(partes(1))
                Dim dt As DataTable = New DataTable()

                ' Si hay vehiculoId válido, cargar desde BD
                If vehiculoId > 0 Then
                    Dim query As String = "SELECT * FROM cat_vehiculo_archivos WHERE VehiculoId = " & vehiculoId & " AND Activo = 1 ORDER BY FechaCreacion DESC"

                    dt = DynamicCrudService.EjecutarConsulta(query)

                    ' Agregar archivos unbound si existen (solo si hay vehiculoId válido)
                    If session("archivosVehiculoUnbound") IsNot Nothing Then
                        Dim archivosUnbound As List(Of Dictionary(Of String, Object)) = TryCast(session("archivosVehiculoUnbound"), List(Of Dictionary(Of String, Object)))

                        If archivosUnbound IsNot Nothing AndAlso archivosUnbound.Count > 0 Then

                            For Each archivoUnbound In archivosUnbound
                                Dim nuevaFila As DataRow = dt.NewRow()

                                nuevaFila("Id") = archivoUnbound("Id")
                                nuevaFila("VehiculoId") = vehiculoId
                                nuevaFila("TipoArchivo") = If(archivoUnbound.ContainsKey("TipoArchivo"), archivoUnbound("TipoArchivo").ToString(), "TarjetaCirculacion")
                                nuevaFila("NombreArchivo") = If(archivoUnbound.ContainsKey("NombreArchivo"), archivoUnbound("NombreArchivo").ToString(), "")
                                nuevaFila("TamanioBytes") = If(archivoUnbound.ContainsKey("TamanioBytes"), archivoUnbound("TamanioBytes"), 0)
                                nuevaFila("TipoMime") = If(archivoUnbound.ContainsKey("TipoMime"), archivoUnbound("TipoMime").ToString(), "")
                                nuevaFila("FechaCreacion") = If(archivoUnbound.ContainsKey("FechaCreacion"), DateTime.Parse(archivoUnbound("FechaCreacion").ToString()), DateTime.Now)
                                nuevaFila("Activo") = If(archivoUnbound.ContainsKey("Activo"), archivoUnbound("Activo"), 1)
                                dt.Rows.InsertAt(nuevaFila, 0)

                            Next

                        End If
                    End If
                Else
                    ' Si no hay vehiculoId (0), limpiar Session y crear tabla vacía
                    session("archivosVehiculoUnbound") = Nothing
                    session("dtArchivosVehiculo") = Nothing

                    dt.Columns.Add("Id", GetType(Integer))
                    dt.Columns.Add("VehiculoId", GetType(Integer))
                    dt.Columns.Add("TipoArchivo", GetType(String))
                    dt.Columns.Add("NombreArchivo", GetType(String))
                    dt.Columns.Add("TamanioBytes", GetType(Long))
                    dt.Columns.Add("TipoMime", GetType(String))
                    dt.Columns.Add("FechaCreacion", GetType(DateTime))
                    dt.Columns.Add("Activo", GetType(Integer))
                End If

                Return dt
            ElseIf partes.Length >= 2 AndAlso partes(0) = "agregarUnbound" Then
                ' Agregar archivo unbound al grid
                Dim datosJson As String = partes(1)
                Dim datos As Dictionary(Of String, Object) = Newtonsoft.Json.JsonConvert.DeserializeObject(Of Dictionary(Of String, Object))(datosJson)

                ' Obtener o crear lista de archivos unbound en Session
                Dim archivosUnbound As List(Of Dictionary(Of String, Object))

                If session("archivosVehiculoUnbound") Is Nothing Then
                    archivosUnbound = New List(Of Dictionary(Of String, Object))
                    session("archivosVehiculoUnbound") = archivosUnbound
                Else
                    archivosUnbound = TryCast(session("archivosVehiculoUnbound"), List(Of Dictionary(Of String, Object)))
                    If archivosUnbound Is Nothing Then
                        archivosUnbound = New List(Of Dictionary(Of String, Object))
                        session("archivosVehiculoUnbound") = archivosUnbound
                    End If
                End If

                ' Agregar el nuevo archivo a la lista
                archivosUnbound.Add(datos)

                ' Obtener DataTable actual o crear uno nuevo
                Dim dtUnbound As DataTable = TryCast(session("dtArchivosVehiculo"), DataTable)

                If dtUnbound Is Nothing Then
                    dtUnbound = New DataTable()
                    dtUnbound.Columns.Add("Id", GetType(Integer))
                    dtUnbound.Columns.Add("VehiculoId", GetType(Integer))
                    dtUnbound.Columns.Add("TipoArchivo", GetType(String))
                    dtUnbound.Columns.Add("NombreArchivo", GetType(String))
                    dtUnbound.Columns.Add("TamanioBytes", GetType(Long))
                    dtUnbound.Columns.Add("TipoMime", GetType(String))
                    dtUnbound.Columns.Add("FechaCreacion", GetType(DateTime))
                    dtUnbound.Columns.Add("Activo", GetType(Integer))
                End If

                ' Agregar nueva fila al DataTable
                Dim nuevaFila As DataRow = dtUnbound.NewRow()

                nuevaFila("Id") = If(datos.ContainsKey("Id") AndAlso datos("Id") IsNot Nothing, Integer.Parse(datos("Id").ToString()), 0)
                nuevaFila("VehiculoId") = If(datos.ContainsKey("VehiculoId") AndAlso datos("VehiculoId") IsNot Nothing, Integer.Parse(datos("VehiculoId").ToString()), 0)
                nuevaFila("TipoArchivo") = If(datos.ContainsKey("TipoArchivo") AndAlso datos("TipoArchivo") IsNot Nothing, datos("TipoArchivo").ToString(), "TarjetaCirculacion")
                nuevaFila("NombreArchivo") = If(datos.ContainsKey("NombreArchivo") AndAlso datos("NombreArchivo") IsNot Nothing, datos("NombreArchivo").ToString(), "")
                nuevaFila("TamanioBytes") = If(datos.ContainsKey("TamanioBytes") AndAlso datos("TamanioBytes") IsNot Nothing, Long.Parse(datos("TamanioBytes").ToString()), 0)
                nuevaFila("TipoMime") = If(datos.ContainsKey("TipoMime") AndAlso datos("TipoMime") IsNot Nothing, datos("TipoMime").ToString(), "")
                nuevaFila("FechaCreacion") = If(datos.ContainsKey("FechaCreacion") AndAlso datos("FechaCreacion") IsNot Nothing, DateTime.Parse(datos("FechaCreacion").ToString()), DateTime.Now)
                nuevaFila("Activo") = If(datos.ContainsKey("Activo") AndAlso datos("Activo") IsNot Nothing, Integer.Parse(datos("Activo").ToString()), 1)
                dtUnbound.Rows.InsertAt(nuevaFila, 0)

                Return dtUnbound
            End If

            Return Nothing
        Catch ex As Exception
            Logger.LogError("UnidadesService.ProcesarCustomCallbackArchivosVehiculo", ex)
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Procesa el CustomCallback del grid de archivos de documento
    ''' </summary>
    Public Shared Function ProcesarCustomCallbackArchivosDocumento(parameters As String, session As System.Web.SessionState.HttpSessionState) As DataTable
        Try
            Dim partes = parameters.Split("|"c)

            If partes.Length >= 2 AndAlso partes(0) = "cargar" Then
                Dim documentoId As Integer = Integer.Parse(partes(1))
                Dim dt As DataTable = New DataTable()

                ' Si hay documentoId válido, cargar desde BD
                If documentoId > 0 Then
                    Dim query As String = "SELECT * FROM cat_documento_unidad_archivos WHERE DocumentoId = " & documentoId & " AND Activo = 1 ORDER BY FechaCreacion DESC"

                    dt = DynamicCrudService.EjecutarConsulta(query)
                Else
                    ' Si no hay documentoId (0), limpiar Session y crear tabla vacía
                    session("dtArchivosDocumento") = Nothing

                    dt.Columns.Add("Id", GetType(Integer))
                    dt.Columns.Add("DocumentoId", GetType(Integer))
                    dt.Columns.Add("TipoArchivo", GetType(String))
                    dt.Columns.Add("NombreArchivo", GetType(String))
                    dt.Columns.Add("TamanioBytes", GetType(Long))
                    dt.Columns.Add("TipoMime", GetType(String))
                    dt.Columns.Add("FechaCreacion", GetType(DateTime))
                    dt.Columns.Add("Activo", GetType(Integer))
                End If

                Return dt
            End If

            Return Nothing
        Catch ex As Exception
            Logger.LogError("UnidadesService.ProcesarCustomCallbackArchivosDocumento", ex)
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Obtiene una unidad como DataRow para cargar en el formulario de edición
    ''' </summary>
    Public Shared Function ObtenerUnidadDataRow(id As Integer) As DataRow

        Try
            Return DynamicCrudService.ObtenerPorId(TABLA_UNIDADES, id)

        Catch ex As Exception
            Logger.LogError("UnidadesService.ObtenerUnidadDataRow", ex)
            Return Nothing

        End Try
    End Function

    ''' <summary>
    ''' Guarda una unidad (crea o actualiza) desde el code-behind
    ''' </summary>
    Public Shared Function GuardarUnidadDesdeCodeBehind(id As Integer, datos As Dictionary(Of String, Object), userId As Integer?) As Boolean

        Try
            If id = 0 Then
                If userId.HasValue Then
                    datos("CreadoPor") = userId.Value
                End If

                Return DynamicCrudService.Insertar(TABLA_UNIDADES, datos)
            Else
                If userId.HasValue Then
                    datos("ModificadoPor") = userId.Value
                End If

                Return DynamicCrudService.Actualizar(TABLA_UNIDADES, id, datos)
            End If

        Catch ex As Exception
            Logger.LogError("UnidadesService.GuardarUnidadDesdeCodeBehind", ex)
            Return False

        End Try
    End Function

    ''' <summary>
    ''' Elimina una unidad desde el code-behind
    ''' </summary>
    Public Shared Function EliminarUnidadDesdeCodeBehind(id As Integer) As Boolean

        Try
            Return DynamicCrudService.Eliminar(TABLA_UNIDADES, id)

        Catch ex As Exception
            Logger.LogError("UnidadesService.EliminarUnidadDesdeCodeBehind", ex)
            Return False

        End Try
    End Function

End Class
