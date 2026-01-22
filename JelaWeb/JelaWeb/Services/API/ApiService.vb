Public Class ApiService
    Private ReadOnly api As ApiConsumer
    Private ReadOnly baseUrl As String

    Public Sub New(apiBaseUrl As String)
        api = New ApiConsumer()
        baseUrl = apiBaseUrl
    End Sub

    Public Function ListarEntidades() As DataTable

        Try
            Dim query As String
            Dim userId = SessionHelper.GetUserId()
            Dim tipoUsuario = SessionHelper.GetTipoUsuario()
            
            ' Si es Administrador de Condominios, filtrar por entidades asignadas
            If tipoUsuario = Constants.TIPO_USUARIO_ADMIN_CONDOMINIOS AndAlso userId.HasValue Then
                query = "SELECT DISTINCT e.Id,e.Clave,e.RazonSocial,e.CP,e.NombreVialidad,e.NoExterior,e.NoInterior,e.Colonia,e.Localidad,e.EntidadFederativa,e.RFC,e.FechaAlta " &
                        "FROM cat_entidades e " &
                        "INNER JOIN conf_usuario_entidades ue ON e.Id = ue.IdEntidad " &
                        "WHERE ue.IdUsuario = " & QueryBuilder.EscapeSqlInteger(userId.Value) & " AND ue.Activo = 1"
                
                Logger.LogInfo($"ListarEntidades - Filtrando para Administrador UserId={userId.Value}")
            Else
                ' Para otros tipos de usuario, mostrar todas las entidades (comportamiento original)
                query = "SELECT e.Id,e.Clave,e.RazonSocial,e.CP,e.NombreVialidad,e.NoExterior,e.NoInterior,e.Colonia,e.Localidad,e.EntidadFederativa,e.RFC,e.FechaAlta FROM cat_entidades e"
                
                Logger.LogInfo($"ListarEntidades - Sin filtro para TipoUsuario={tipoUsuario}")
            End If
            
            Dim url As String = baseUrl & query
            Dim datos = api.ObtenerDatos(url)

            Return api.ConvertirADatatable(datos)

        Catch ex As Exception
            Logger.LogError("ApiService.ListarEntidades", ex)
            Throw New ApplicationException("Error al obtener los registros desde el API.", ex)

        End Try
    End Function

    Public Function ListarRegimen() As DataTable

        Try
            ' Intentar obtener del caché primero
            Dim cached = CacheHelper.GetValue(Of DataTable)(Constants.CACHE_REGIMEN)

            If cached IsNot Nothing Then
                Return cached
            End If

            Dim query As String = "SELECT * FROM cat_regimen_fiscal"
            Dim url As String = baseUrl & query
            Dim datos = api.ObtenerDatos(url)
            Dim result = api.ConvertirADatatable(datos)

            ' Guardar en caché por 60 minutos (datos que cambian poco)
            CacheHelper.SetValue(Constants.CACHE_REGIMEN, result, TimeSpan.FromMinutes(Constants.CACHE_LONG_MINUTES))

            Return result

        Catch ex As Exception
            Throw New ApplicationException("Error al obtener los registros desde el API.", ex)

        End Try
    End Function

    Public Function ListarForma() As DataTable

        Try
            ' Intentar obtener del caché primero
            Dim cached = CacheHelper.GetValue(Of DataTable)(Constants.CACHE_FORMA_PAGO)

            If cached IsNot Nothing Then
                Return cached
            End If

            Dim query As String = "SELECT * FROM cat_forma_de_pago"
            Dim url As String = baseUrl & query
            Dim datos = api.ObtenerDatos(url)
            Dim result = api.ConvertirADatatable(datos)

            ' Guardar en caché por 60 minutos
            CacheHelper.SetValue(Constants.CACHE_FORMA_PAGO, result, TimeSpan.FromMinutes(Constants.CACHE_LONG_MINUTES))

            Return result

        Catch ex As Exception
            Throw New ApplicationException("Error al obtener los registros desde el API.", ex)

        End Try
    End Function

    Public Function ListarMetodo() As DataTable

        Try
            ' Intentar obtener del caché primero
            Dim cached = CacheHelper.GetValue(Of DataTable)(Constants.CACHE_METODO_PAGO)

            If cached IsNot Nothing Then
                Return cached
            End If

            Dim query As String = "SELECT * FROM cat_metodo_de_pago"
            Dim url As String = baseUrl & query
            Dim datos = api.ObtenerDatos(url)
            Dim result = api.ConvertirADatatable(datos)

            ' Guardar en caché por 60 minutos
            CacheHelper.SetValue(Constants.CACHE_METODO_PAGO, result, TimeSpan.FromMinutes(Constants.CACHE_LONG_MINUTES))

            Return result

        Catch ex As Exception
            Throw New ApplicationException("Error al obtener los registros desde el API.", ex)

        End Try
    End Function

    Public Function ListarUsos() As DataTable

        Try
            ' Intentar obtener del caché primero
            Dim cached = CacheHelper.GetValue(Of DataTable)(Constants.CACHE_USOS_CFDI)

            If cached IsNot Nothing Then
                Return cached
            End If

            Dim query As String = "SELECT * FROM cat_usos_cfdi"
            Dim url As String = baseUrl & query
            Dim datos = api.ObtenerDatos(url)
            Dim result = api.ConvertirADatatable(datos)

            ' Guardar en caché por 60 minutos
            CacheHelper.SetValue(Constants.CACHE_USOS_CFDI, result, TimeSpan.FromMinutes(Constants.CACHE_LONG_MINUTES))

            Return result

        Catch ex As Exception
            Throw New ApplicationException("Error al obtener los registros desde el API.", ex)

        End Try
    End Function

    Public Function ListarSubEntidades(IdEntidad As Integer) As DataTable

        Try
            ' Validar y escapar el parámetro
            If IdEntidad <= 0 Then
                Throw New ArgumentException("IdEntidad debe ser mayor a cero")
            End If

            Dim query As String = "SELECT

  Id,
  Clave,
  CIF,
  RazonSocial AS Nombre,
  Telefonos as Tel,
  Mail,
  CP,
  NombreVialidad AS Calle,
  NoExterior AS Exterior,
  NoInterior AS Interior,
  Colonia AS Col,
  Municipio,
  EntidadFederativa AS Estado,
  RFC,
  Activo AS Act 
FROM cat_sub_entidades 
WHERE IdEntidad=" & QueryBuilder.EscapeSqlInteger(IdEntidad)

            Dim url As String = baseUrl & System.Web.HttpUtility.UrlEncode(query)
            Dim datos = api.ObtenerDatos(url)

            Return api.ConvertirADatatable(datos)

        Catch ex As Exception
            Throw New ApplicationException("Error al obtener los registros desde el API.", ex)

        End Try
    End Function

    Public Function ListarTiposDocs() As DataTable

        Try
            ' Intentar obtener del caché primero
            Dim cached = CacheHelper.GetValue(Of DataTable)(Constants.CACHE_TIPOS_DOC)

            If cached IsNot Nothing Then
                Return cached
            End If

            Dim query As String = "SELECT * FROM cat_tipo_documentos"
            Dim url As String = baseUrl & query
            Dim datos = api.ObtenerDatos(url)
            Dim result = api.ConvertirADatatable(datos)

            ' Guardar en caché por 60 minutos
            CacheHelper.SetValue(Constants.CACHE_TIPOS_DOC, result, TimeSpan.FromMinutes(Constants.CACHE_LONG_MINUTES))

            Return result

        Catch ex As Exception
            Throw New ApplicationException("Error al obtener los registros desde el API.", ex)

        End Try
    End Function

    Public Function ObtenerDocumentos(IdTipo As Integer, dt1 As Date, dt2 As Date) As DataTable

        Try
            ' Validar parámetros
            If IdTipo <= 0 Then
                Throw New ArgumentException("IdTipo debe ser mayor a cero")
            End If

            If dt2 < dt1 Then
                Throw New ArgumentException("La fecha final no puede ser menor que la fecha inicial")
            End If

            ' Construir query de forma segura
            Dim query As String = "SELECT op_documentos.Id AS Id, op_documentos.NoDocumento AS NODocumento, " &
                "op_documentos.FechaCreacion AS FechaCreacion, op_documentos.FechaAsignacion AS FechaAsignacion, " &
                "op_documentos.FechaUltimoMovimiento AS FechaUltimoMovimiento, op_documentos.FechaCierre AS FechaCierre, " &
                "op_documentos.NoPartidas AS NoPartidas, op_documentos.Referencia AS Referencia, " &
                "cat_sub_entidades.RazonSocial AS SubEntAsignada, cat_tipo_documentos.Nombre AS TipoDocumento, " &
                "op_documentos.Activo AS Activo " &
                "FROM op_documentos " &
                "INNER JOIN cat_entidades ON op_documentos.IdEntidad = cat_entidades.Id " &
                "INNER JOIN cat_sub_entidades ON op_documentos.IdSubEntidad = cat_sub_entidades.Id " &
                "INNER JOIN cat_tipo_documentos ON op_documentos.IdTipoDocumento = cat_tipo_documentos.Id " &
                "WHERE op_documentos.IdTipoDocumento = " & QueryBuilder.EscapeSqlInteger(IdTipo) &
                " AND op_documentos.FechaCreacion BETWEEN " & QueryBuilder.EscapeSqlDate(New Date(dt1.Year, dt1.Month, dt1.Day, 0, 0, 0)) &
                " AND " & QueryBuilder.EscapeSqlDate(New Date(dt2.Year, dt2.Month, dt2.Day, 23, 59, 59)) & ";"

            Dim url As String = baseUrl & System.Web.HttpUtility.UrlEncode(query)
            Dim datos = api.ObtenerDatos(url)

            Return api.ConvertirADatatable(datos)

        Catch ex As Exception
            Throw New ApplicationException("Error al obtener los documentos desde el API.", ex)

        End Try
    End Function

    Public Function BuscarColporClave(clave As String) As ColoniasDTO

        Try
            ' Validar y sanitizar entrada
            If String.IsNullOrEmpty(clave) Then
                Throw New ArgumentException("La clave no puede estar vacía")
            End If

            ' Validar que no contenga caracteres peligrosos
            Dim validation = InputValidator.ValidateSqlInput(clave)

            If Not validation.IsValid Then
                Throw New ArgumentException($"Clave inválida: {validation.Message}")
            End If

            ' Construir query de forma segura
            Dim query As String = "SELECT * FROM cat_colonias WHERE Clave = " & QueryBuilder.EscapeSqlString(clave)
            Dim url As String = baseUrl & System.Web.HttpUtility.UrlEncode(query)
            Dim datos = api.ObtenerDatos(url)

            If datos IsNot Nothing AndAlso datos.Count > 0 Then
                Dim campos = datos(0).Campos
                Dim dto As New ColoniasDTO

                dto.Id = Convert.ToInt32(campos("Id").Valor)
                dto.Clave = If(campos.ContainsKey("Clave") AndAlso campos("Clave") IsNot Nothing AndAlso campos("Clave").Valor IsNot Nothing, campos("Clave").Valor.ToString(), "")
                dto.UnidadTerritorial = If(campos.ContainsKey("UnidadTerritorial") AndAlso campos("UnidadTerritorial") IsNot Nothing AndAlso campos("UnidadTerritorial").Valor IsNot Nothing, campos("UnidadTerritorial").Valor.ToString(), "")
                dto.CP = If(campos.ContainsKey("CP") AndAlso campos("CP") IsNot Nothing AndAlso campos("CP").Valor IsNot Nothing, campos("CP").Valor.ToString(), "")
                dto.Latitud = Convert.ToDouble(campos("Latitud").Valor)
                dto.Longitud = Convert.ToDouble(campos("Longitud").Valor)

                Return dto

            Else
                Return Nothing
            End If

        Catch ex As Exception
            Throw New ApplicationException("Error al buscar colonia por clave.", ex)

        End Try
    End Function

    Public Function GetConceptoByClave(clave As String) As ConceptosDTO

        Try
            ' Validar y sanitizar entrada
            If String.IsNullOrEmpty(clave) Then
                Throw New ArgumentException("La clave no puede estar vacía")
            End If

            ' Validar que no contenga caracteres peligrosos
            Dim validation = InputValidator.ValidateSqlInput(clave)

            If Not validation.IsValid Then
                Throw New ArgumentException($"Clave inválida: {validation.Message}")
            End If

            ' Construir query de forma segura
            Dim query As String = "SELECT * FROM cat_conceptos WHERE Clave = " & QueryBuilder.EscapeSqlString(clave)
            Dim url As String = baseUrl & System.Web.HttpUtility.UrlEncode(query)
            Dim datos = api.ObtenerDatos(url)

            If datos IsNot Nothing AndAlso datos.Count > 0 Then
                Dim campos = datos(0).Campos
                Dim dto As New ConceptosDTO

                dto.Id = Convert.ToInt32(campos("Id").Valor)
                dto.Clave = If(campos.ContainsKey("Clave") AndAlso campos("Clave") IsNot Nothing AndAlso campos("Clave").Valor IsNot Nothing, campos("Clave").Valor.ToString(), "")
                dto.Concepto = If(campos.ContainsKey("Descripcion") AndAlso campos("Descripcion") IsNot Nothing AndAlso campos("Descripcion").Valor IsNot Nothing, campos("Descripcion").Valor.ToString(), "")
                dto.Costo = Convert.ToDouble(campos("CostoUnitario").Valor)

                Return dto

            Else
                Return Nothing
            End If

        Catch ex As Exception
            Throw New ApplicationException("Error al obtener concepto por clave.", ex)

        End Try
    End Function

    Public Function ListarConceptos() As DataTable

        Dim query As String = "SELECT

  c.Id AS Id,
  c.Clave AS Clave,
  c.ClaveAlterna AS ClaveAlterna,
  c.Descripcion AS Descripcion,
  c.Existencia AS Existencia,
  c.CostoUnitario AS CostoUnitario,
  c.FactorConversion AS Factor,
  c.Minimo AS Minimo,
  c.Maximo AS Maximo,
  um.Nombre AS UMNombre,
  f.Nombre AS NombreFamilia,
  c.Inventariable AS Inventariable,
  c.Activo AS Activo 
FROM cat_conceptos c
  INNER JOIN cat_unidades_medidas um
    ON c.IdUnidadMedida = um.Id
  INNER JOIN cat_conceptos_familias f 
    ON c.IdFamilia = f.Id"

        Dim url As String = baseUrl & query
        Dim datos = api.ObtenerDatos(url)

        Return api.ConvertirADatatable(datos)

    End Function

    Public Function ListarConceptosCombo() As DataTable

        Try
            ' Intentar obtener del caché primero
            Dim cached = CacheHelper.GetValue(Of DataTable)(Constants.CACHE_CONCEPTOS_COMBO)

            If cached IsNot Nothing Then
                Return cached
            End If

            Dim query As String = "SELECT Clave, Descripcion FROM cat_conceptos WHERE Activo = 1 ORDER BY Descripcion"
            Dim url As String = baseUrl & query
            Dim datos = api.ObtenerDatos(url)
            Dim result = api.ConvertirADatatable(datos)

            ' Guardar en caché por 30 minutos (datos que pueden cambiar más frecuentemente)
            CacheHelper.SetValue(Constants.CACHE_CONCEPTOS_COMBO, result, TimeSpan.FromMinutes(Constants.CACHE_DEFAULT_MINUTES))

            Return result

        Catch ex As Exception
            Throw New ApplicationException("Error al obtener los registros desde el API.", ex)

        End Try
    End Function

    Public Function ListarColoniasCombo(IdEntidad As Integer) As DataTable

        Try
            ' Validar y escapar el parámetro
            If IdEntidad <= 0 Then
                Throw New ArgumentException("IdEntidad debe ser mayor a cero")
            End If

            ' Construir query para obtener colonias de la entidad
            ' Nota: La tabla cat_colonias no tiene columna Activo, se elimina esa condición
            ' Si existe relación con IdEntidad, se debería incluir un JOIN o WHERE adicional
            Dim query As String = "SELECT Clave, UnidadTerritorial AS Descripcion FROM cat_colonias ORDER BY UnidadTerritorial"
            Dim url As String = baseUrl & System.Web.HttpUtility.UrlEncode(query)
            Dim datos = api.ObtenerDatos(url)

            Return api.ConvertirADatatable(datos)

        Catch ex As Exception
            Throw New ApplicationException("Error al obtener las colonias desde el API.", ex)

        End Try
    End Function

    Public Function BuscarColporDescripcion(descripcion As String) As ColoniasDTO

        Try
            ' Validar y sanitizar entrada
            If String.IsNullOrEmpty(descripcion) Then
                Throw New ArgumentException("La descripción no puede estar vacía")
            End If

            ' Validar que no contenga caracteres peligrosos
            Dim validation = InputValidator.ValidateSqlInput(descripcion)

            If Not validation.IsValid Then
                Throw New ArgumentException($"Descripción inválida: {validation.Message}")
            End If

            ' Construir query de forma segura
            Dim query As String = "SELECT * FROM cat_colonias WHERE UnidadTerritorial = " & QueryBuilder.EscapeSqlString(descripcion)
            Dim url As String = baseUrl & System.Web.HttpUtility.UrlEncode(query)
            Dim datos = api.ObtenerDatos(url)

            If datos IsNot Nothing AndAlso datos.Count > 0 Then
                Dim campos = datos(0).Campos
                Dim dto As New ColoniasDTO

                dto.Id = Convert.ToInt32(campos("Id").Valor)
                dto.Clave = If(campos.ContainsKey("Clave") AndAlso campos("Clave") IsNot Nothing AndAlso campos("Clave").Valor IsNot Nothing, campos("Clave").Valor.ToString(), "")
                dto.UnidadTerritorial = If(campos.ContainsKey("UnidadTerritorial") AndAlso campos("UnidadTerritorial") IsNot Nothing AndAlso campos("UnidadTerritorial").Valor IsNot Nothing, campos("UnidadTerritorial").Valor.ToString(), "")
                dto.CP = If(campos.ContainsKey("CP") AndAlso campos("CP") IsNot Nothing AndAlso campos("CP").Valor IsNot Nothing, campos("CP").Valor.ToString(), "")
                dto.Latitud = Convert.ToDouble(campos("Latitud").Valor)
                dto.Longitud = Convert.ToDouble(campos("Longitud").Valor)

                Return dto

            Else
                Return Nothing
            End If

        Catch ex As Exception
            Throw New ApplicationException("Error al buscar colonia por descripción.", ex)

        End Try
    End Function

    Public Function BuscarConceptoPorDescripcion(descripcion As String) As ConceptosDTO

        Try
            ' Validar y sanitizar entrada
            If String.IsNullOrEmpty(descripcion) Then
                Throw New ArgumentException("La descripción no puede estar vacía")
            End If

            ' Validar que no contenga caracteres peligrosos
            Dim validation = InputValidator.ValidateSqlInput(descripcion)

            If Not validation.IsValid Then
                Throw New ArgumentException($"Descripción inválida: {validation.Message}")
            End If

            ' Construir query de forma segura
            Dim query As String = "SELECT * FROM cat_conceptos WHERE Descripcion = " & QueryBuilder.EscapeSqlString(descripcion)
            Dim url As String = baseUrl & System.Web.HttpUtility.UrlEncode(query)
            Dim datos = api.ObtenerDatos(url)

            If datos IsNot Nothing AndAlso datos.Count > 0 Then
                Dim campos = datos(0).Campos
                Dim dto As New ConceptosDTO

                dto.Id = Convert.ToInt32(campos("Id").Valor)
                dto.Clave = If(campos.ContainsKey("Clave") AndAlso campos("Clave") IsNot Nothing AndAlso campos("Clave").Valor IsNot Nothing, campos("Clave").Valor.ToString(), "")
                dto.Concepto = If(campos.ContainsKey("Descripcion") AndAlso campos("Descripcion") IsNot Nothing AndAlso campos("Descripcion").Valor IsNot Nothing, campos("Descripcion").Valor.ToString(), "")
                dto.Costo = Convert.ToDouble(campos("CostoUnitario").Valor)

                Return dto

            Else
                Return Nothing
            End If

        Catch ex As Exception
            Throw New ApplicationException("Error al buscar concepto por descripción.", ex)

        End Try
    End Function

    Public Function ObtenerDocumentoPorId(idDocumento As Integer) As DataTable

        Try
            If idDocumento <= 0 Then
                Throw New ArgumentException("IdDocumento debe ser mayor a cero")
            End If

            Dim query As String = "SELECT * FROM op_documentos WHERE Id = " & QueryBuilder.EscapeSqlInteger(idDocumento)
            Dim url As String = baseUrl & System.Web.HttpUtility.UrlEncode(query)
            Dim datos = api.ObtenerDatos(url)

            Return api.ConvertirADatatable(datos)

        Catch ex As Exception
            Throw New ApplicationException("Error al obtener el documento desde el API.", ex)

        End Try
    End Function

    Public Function ObtenerColoniasPorDocumento(idDocumento As Integer) As DataTable

        Try
            If idDocumento <= 0 Then
                Throw New ArgumentException("IdDocumento debe ser mayor a cero")
            End If

            Dim query As String = "SELECT * FROM op_documentos_colonias WHERE IdDocumento = " & QueryBuilder.EscapeSqlInteger(idDocumento)
            Dim url As String = baseUrl & System.Web.HttpUtility.UrlEncode(query)
            Dim datos = api.ObtenerDatos(url)

            Return api.ConvertirADatatable(datos)

        Catch ex As Exception
            Throw New ApplicationException("Error al obtener las colonias del documento desde el API.", ex)

        End Try
    End Function

    Public Function ObtenerConceptosPorDocumento(idDocumento As Integer) As DataTable

        Try
            If idDocumento <= 0 Then
                Throw New ArgumentException("IdDocumento debe ser mayor a cero")
            End If

            Dim query As String = "SELECT * FROM op_documentos_detalle WHERE IdDocumento = " & QueryBuilder.EscapeSqlInteger(idDocumento)
            Dim url As String = baseUrl & System.Web.HttpUtility.UrlEncode(query)
            Dim datos = api.ObtenerDatos(url)

            Return api.ConvertirADatatable(datos)

        Catch ex As Exception
            Throw New ApplicationException("Error al obtener los conceptos del documento desde el API.", ex)

        End Try
    End Function

    ''' <summary>
    ''' Lista todas las unidades privativas con información de entidades y estatus financiero
    ''' </summary>
    Public Function ListarUnidades() As DataTable

        Try
            ' Solo seleccionar las columnas necesarias para el grid
            Dim query As String = "SELECT " &
                "u.Id, " &
                "u.Codigo, " &
                "u.Nombre, " &
                "e.RazonSocial AS Entidad, " &
                "se.RazonSocial AS SubEntidad, " &
                "u.Calle, " &
                "u.NumeroExterior AS NoExterior, " &
                "u.NumeroInterior AS NoInterior, " &
                "u.numero_residentes AS Residentes, " &
                "u.fecha_creacion AS FechaCreacion, " &
                "u.Latitud, " &
                "u.Longitud, " &
                "COALESCE(SUM(CASE WHEN c.Estado IN ('Pendiente', 'Parcial') THEN (c.MontoTotal - COALESCE(c.MontoPagado, 0)) ELSE 0 END), 0) AS SaldoPendiente, " &
                "COALESCE(SUM(CASE WHEN c.Estado IN ('Pendiente', 'Parcial') AND c.FechaVencimiento < CURDATE() THEN (c.MontoTotal - COALESCE(c.MontoPagado, 0)) ELSE 0 END), 0) AS SaldoVencido, " &
                "CASE " &
                "  WHEN COALESCE(SUM(CASE WHEN c.Estado IN ('Pendiente', 'Parcial') THEN (c.MontoTotal - COALESCE(c.MontoPagado, 0)) ELSE 0 END), 0) = 0 THEN 'Al Día' " &
                "  WHEN COALESCE(SUM(CASE WHEN c.Estado IN ('Pendiente', 'Parcial') AND c.FechaVencimiento < CURDATE() THEN (c.MontoTotal - COALESCE(c.MontoPagado, 0)) ELSE 0 END), 0) > 0 THEN 'Moroso' " &
                "  ELSE 'Con Adeudo' " &
                "END AS EstatusFinanciero " &
                "FROM cat_unidades u " &
                "LEFT JOIN cat_entidades e ON u.entidad_id = e.Id " &
                "LEFT JOIN cat_sub_entidades se ON u.SubEntidadId = se.Id " &
                "LEFT JOIN op_cuotas c ON c.UnidadId = u.Id " &
                "WHERE (u.Activo = 1 OR u.Activo IS NULL) " &
                "GROUP BY u.Id, u.Codigo, u.Nombre, u.Calle, u.NumeroExterior, u.NumeroInterior, " &
                "u.numero_residentes, u.fecha_creacion, u.Latitud, u.Longitud, e.RazonSocial, se.RazonSocial " &
                "ORDER BY u.Nombre"

            Dim url As String = baseUrl & System.Web.HttpUtility.UrlEncode(query)
            Dim datos = api.ObtenerDatos(url)

            Return api.ConvertirADatatable(datos)

        Catch ex As Exception
            ' Si falla la consulta completa (por ejemplo, si op_cuotas no existe), intentar consulta simple

            Try
                Dim querySimple As String = "SELECT " &
                    "u.Id, " &
                    "u.Codigo, " &
                    "u.Nombre, " &
                    "e.RazonSocial AS Entidad, " &
                    "se.RazonSocial AS SubEntidad, " &
                    "u.Calle, " &
                    "u.NumeroExterior AS NoExterior, " &
                    "u.NumeroInterior AS NoInterior, " &
                    "u.numero_residentes AS Residentes, " &
                    "u.fecha_creacion AS FechaCreacion, " &
                    "u.Latitud, " &
                    "u.Longitud, " &
                    "CAST(0 AS DECIMAL(10,2)) AS SaldoPendiente, " &
                    "CAST(0 AS DECIMAL(10,2)) AS SaldoVencido, " &
                    "'Al Día' AS EstatusFinanciero " &
                    "FROM cat_unidades u " &
                    "LEFT JOIN cat_entidades e ON u.entidad_id = e.Id " &
                    "LEFT JOIN cat_sub_entidades se ON u.SubEntidadId = se.Id " &
                    "WHERE (u.Activo = 1 OR u.Activo IS NULL) " &
                    "ORDER BY u.Nombre"

                Dim urlSimple As String = baseUrl & System.Web.HttpUtility.UrlEncode(querySimple)
                Dim datosSimple = api.ObtenerDatos(urlSimple)

                Return api.ConvertirADatatable(datosSimple)

            Catch exSimple As Exception

                Throw New ApplicationException("Error al obtener las unidades desde el API.", exSimple)

            End Try
        End Try
    End Function

    ''' <summary>
    ''' Lista todas las entidades activas (para combos)
    ''' Incluye registros donde Activo = 1 o Activo IS NULL (por compatibilidad)
    ''' Solo incluye registros con RazonSocial válido (no NULL y no vacío)
    ''' </summary>
    Public Function ListarEntidadesActivas() As DataTable

        Try
            ' Incluir registros donde Activo = 1 o Activo IS NULL, y que tengan RazonSocial válido
            Dim query As String = "SELECT Id, RazonSocial FROM cat_entidades WHERE (Activo = 1 OR Activo IS NULL) AND RazonSocial IS NOT NULL AND TRIM(RazonSocial) != '' ORDER BY RazonSocial"
            Dim url As String = baseUrl & System.Web.HttpUtility.UrlEncode(query)
            Dim datos = api.ObtenerDatos(url)

            Return api.ConvertirADatatable(datos)

        Catch ex As Exception
            Throw New ApplicationException("Error al obtener las entidades desde el API.", ex)

        End Try
    End Function

    ''' <summary>
    ''' Lista subentidades activas por entidad (para combos)
    ''' Incluye registros donde Activo = 1 o Activo IS NULL (por compatibilidad)
    ''' Solo incluye registros con RazonSocial válido (no NULL y no vacío)
    ''' </summary>
    Public Function ListarSubEntidadesActivasPorEntidad(IdEntidad As Integer) As DataTable

        Try
            If IdEntidad <= 0 Then
                Throw New ArgumentException("IdEntidad debe ser mayor a cero")
            End If

            ' Incluir registros donde Activo = 1 o Activo IS NULL, y que tengan RazonSocial válido
            ' Nota: Usar IdEntidad (no entidad_id) según la estructura de la tabla
            Dim query As String = "SELECT Id, RazonSocial FROM cat_sub_entidades WHERE IdEntidad = " & QueryBuilder.EscapeSqlInteger(IdEntidad) & " AND (Activo = 1 OR Activo IS NULL) AND RazonSocial IS NOT NULL AND TRIM(RazonSocial) != '' ORDER BY RazonSocial"
            Dim url As String = baseUrl & System.Web.HttpUtility.UrlEncode(query)
            Dim datos = api.ObtenerDatos(url)

            Return api.ConvertirADatatable(datos)

        Catch ex As Exception
            Throw New ApplicationException("Error al obtener las sub-entidades desde el API.", ex)

        End Try
    End Function

End Class
