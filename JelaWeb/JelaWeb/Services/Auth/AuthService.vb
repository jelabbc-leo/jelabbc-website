Imports System.Net.Http
Imports System.Security.Cryptography
Imports System.Text
Imports System.Threading.Tasks
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

''' <summary>
''' Servicio de autenticación usando la nueva API JWT
''' </summary>
Public Class AuthService
    Private ReadOnly apiConsumer As ApiConsumer
    Private ReadOnly baseUrl As String
    Private ReadOnly apiAuthUrl As String

    Public Sub New()
        apiConsumer = New ApiConsumer()
        baseUrl = ConfigurationManager.AppSettings("ApiBaseUrl")
        ' Extraer URL base para autenticación
        If Not String.IsNullOrEmpty(baseUrl) Then
            Dim idx = baseUrl.IndexOf("/api/")
            If idx > 0 Then
                apiAuthUrl = baseUrl.Substring(0, idx) & "/api/auth/login"
            End If
        End If
    End Sub

    ''' <summary>
    ''' Autentica un usuario usando el endpoint JWT de la API
    ''' </summary>
    ''' <param name="username">Nombre de usuario</param>
    ''' <param name="password">Contraseña en texto plano</param>
    ''' <returns>Resultado de autenticación con datos del usuario</returns>
    Public Function Autenticar(username As String, password As String) As AuthResult

        Try
            ' Validar parámetros
            If String.IsNullOrWhiteSpace(username) OrElse String.IsNullOrWhiteSpace(password) Then
                Return New AuthResult With {
                    .Success = False,
                    .Message = "Usuario y contraseña son requeridos"
                }
            End If

            ' Autenticar via API JWT
            Dim loginRequest = New With {
                .username = username,
                .password = password
            }

            Dim json = JsonConvert.SerializeObject(loginRequest)
            Dim content = New StringContent(json, Encoding.UTF8, "application/json")

            Dim taskRespuesta = Task.Run(Async Function()
                                             Return Await HttpClientHelper.Client.PostAsync(apiAuthUrl, content).ConfigureAwait(False)
                                         End Function)
            Dim respuesta = taskRespuesta.Result

            Dim taskContenido = Task.Run(Async Function()
                                             Return Await respuesta.Content.ReadAsStringAsync().ConfigureAwait(False)
                                         End Function)
            Dim contenido = taskContenido.Result

            ' Parsear respuesta
            Dim authResponse = JsonConvert.DeserializeObject(Of JwtAuthResponse)(contenido)

            If authResponse Is Nothing OrElse Not authResponse.Success Then
                Dim mensaje = If(authResponse?.Message, "credenciales_invalidas")
                Return New AuthResult With {
                    .Success = False,
                    .Message = mensaje
                }
            End If

            ' Guardar tokens JWT para uso posterior
            If Not String.IsNullOrEmpty(authResponse.Token) Then
                JwtTokenService.Instance.SetToken(
                    authResponse.Token,
                    authResponse.RefreshToken,
                    If(authResponse.ExpiresAt.HasValue, authResponse.ExpiresAt.Value, DateTime.UtcNow.AddHours(1))
                )
            End If

            ' Obtener datos del usuario de la respuesta JWT
            Dim userId As Integer = If(authResponse.User?.Id, 0)
            Dim nombre As String = If(authResponse.User?.Nombre, username)
            Dim email As String = If(authResponse.User?.Email, "")
            Dim idEntidad As Integer = If(authResponse.User?.EntidadId, 1)
            Dim entidadNombre As String = If(authResponse.User?.EntidadNombre, "")
            
            ' NUEVO: Obtener datos multi-entidad
            Dim tipoUsuario As String = If(authResponse.User?.TipoUsuario, "Residente")
            Dim entidades As JArray = ConvertirEntidades(authResponse.User?.Entidades)
            Dim idEntidadPrincipal As Integer? = authResponse.User?.IdEntidadPrincipal
            Dim licenciasDisponibles As Integer = If(authResponse.User?.LicenciasDisponibles, 0)

            ' Obtener opciones del menú para el usuario
            Dim opciones As JArray = ObtenerOpcionesMenu(userId)

            Return New AuthResult With {
                .Success = True,
                .UserId = userId,
                .Nombre = nombre,
                .Email = email,
                .IdEntidad = idEntidad,
                .EntidadNombre = entidadNombre,
                .TipoUsuario = tipoUsuario,
                .Entidades = entidades,
                .IdEntidadPrincipal = idEntidadPrincipal,
                .LicenciasDisponibles = licenciasDisponibles,
                .Opciones = opciones,
                .Message = "Login exitoso"
            }

        Catch ex As Exception
            Logger.LogError("Error en AuthService.Autenticar", ex)
            Return New AuthResult With {
                .Success = False,
                .Message = "Error al autenticar: " & ex.Message
            }

        End Try
    End Function

    ''' <summary>
    ''' Obtiene las opciones del menú para un usuario basado en sus roles/permisos
    ''' </summary>
    Private Function ObtenerOpcionesMenu(userId As Integer) As JArray

        Try
            ' Consultar opciones del menú basadas en los permisos del usuario
            ' Tablas: conf_opciones, conf_rolopciones, conf_usuarioroles
            Dim query As String = $"SELECT DISTINCT o.Id, o.Nombre, o.Url, o.Icono, o.RibbonTab, o.RibbonGroup, " &
                                  $"o.OrdenTab, o.OrdenGrupo, o.OrdenOpcion " &
                                  $"FROM conf_opciones o " &
                                  $"INNER JOIN conf_rolopciones ro ON o.Id = ro.OpcionId " &
                                  $"INNER JOIN conf_usuarioroles ur ON ro.RolId = ur.RolId " &
                                  $"WHERE ur.UsuarioId = {userId} " &
                                  $"AND o.Activo = 1 " &
                                  $"ORDER BY o.OrdenTab, o.OrdenGrupo, o.OrdenOpcion"

            Logger.LogInfo($"AuthService.ObtenerOpcionesMenu - UserId: {userId}, Query: {query}")

            Dim url As String = baseUrl & System.Web.HttpUtility.UrlEncode(query)

            Logger.LogInfo($"AuthService.ObtenerOpcionesMenu - URL: {url}")

            Dim datos = apiConsumer.ObtenerDatos(url)

            Logger.LogInfo($"AuthService.ObtenerOpcionesMenu - Datos retornados: {If(datos IsNot Nothing, datos.Count.ToString(), "Nothing")}")

            Dim opciones As New JArray()

            If datos IsNot Nothing AndAlso datos.Count > 0 Then

                For Each opcion In datos
                    Dim campos = opcion.Campos
                    Dim item As New JObject()

                    ' Convertir valores a tipos compatibles con JToken
                    If campos.ContainsKey("Id") AndAlso campos("Id").Valor IsNot Nothing Then
                        item("id") = Convert.ToInt32(campos("Id").Valor)
                    Else
                        item("id") = 0
                    End If

                    item("Nombre") = If(campos.ContainsKey("Nombre") AndAlso campos("Nombre").Valor IsNot Nothing, 
                                       campos("Nombre").Valor.ToString(), "")
                    item("Url") = If(campos.ContainsKey("Url") AndAlso campos("Url").Valor IsNot Nothing, 
                                    campos("Url").Valor.ToString(), "")
                    item("Icono") = If(campos.ContainsKey("Icono") AndAlso campos("Icono").Valor IsNot Nothing, 
                                      campos("Icono").Valor.ToString(), "")
                    item("RibbonTab") = If(campos.ContainsKey("RibbonTab") AndAlso campos("RibbonTab").Valor IsNot Nothing, 
                                          campos("RibbonTab").Valor.ToString(), "")
                    item("RibbonGroup") = If(campos.ContainsKey("RibbonGroup") AndAlso campos("RibbonGroup").Valor IsNot Nothing, 
                                            campos("RibbonGroup").Valor.ToString(), "")
                    opciones.Add(item)

                Next

            End If

            Logger.LogInfo($"AuthService.ObtenerOpcionesMenu - Opciones construidas: {opciones.Count}")

            Return opciones

        Catch ex As Exception
            Logger.LogError("Error al obtener opciones del menú", ex)
            Return New JArray()

        End Try
    End Function

    ''' <summary>
    ''' Hashea la contraseña usando SHA256
    ''' </summary>
    Private Function HashPassword(password As String) As String
        Using sha256 As SHA256 = SHA256.Create()
            Dim bytes As Byte() = Encoding.UTF8.GetBytes(password)
            Dim hash As Byte() = sha256.ComputeHash(bytes)
            Dim builder As New StringBuilder()

            For Each b As Byte In hash
                builder.Append(b.ToString("x2"))

            Next

            Return builder.ToString()
        End Using
    End Function
    
    ''' <summary>
    ''' Convierte la lista de entidades del API a JArray
    ''' </summary>
    Private Function ConvertirEntidades(entidades As Object) As JArray
        Try
            If entidades Is Nothing Then
                Return New JArray()
            End If
            
            ' Si ya es JArray, retornarlo
            If TypeOf entidades Is JArray Then
                Return DirectCast(entidades, JArray)
            End If
            
            ' Si es una lista, convertirla
            Dim json = JsonConvert.SerializeObject(entidades)
            Return JArray.Parse(json)
            
        Catch ex As Exception
            Logger.LogError("Error al convertir entidades", ex)
            Return New JArray()
        End Try
    End Function
    
    ''' <summary>
    ''' Consume una licencia del usuario para crear una nueva entidad
    ''' </summary>
    ''' <param name="userId">ID del usuario</param>
    ''' <returns>Número de licencias restantes</returns>
    Public Function ConsumirLicencia(userId As Integer) As Integer
        Try
            ' Construir URL del endpoint
            Dim apiBaseUrl As String = ConfigurationManager.AppSettings("ApiBaseUrl")
            Dim idx = apiBaseUrl.IndexOf("/api/")
            Dim baseUrlSinQuery As String = If(idx > 0, apiBaseUrl.Substring(0, idx), apiBaseUrl.TrimEnd("/"c))
            Dim url As String = $"{baseUrlSinQuery}/api/usuarios/{userId}/consumir-licencia"
            
            Logger.LogInfo($"AuthService.ConsumirLicencia - URL: {url}")
            
            ' Hacer POST al endpoint
            Dim taskRespuesta = Task.Run(Async Function()
                                             Return Await HttpClientHelper.Client.PostAsync(url, Nothing).ConfigureAwait(False)
                                         End Function)
            Dim respuesta = taskRespuesta.Result
            
            ' Leer respuesta
            Dim taskContenido = Task.Run(Async Function()
                                             Return Await respuesta.Content.ReadAsStringAsync().ConfigureAwait(False)
                                         End Function)
            Dim contenido = taskContenido.Result
            
            If Not respuesta.IsSuccessStatusCode Then
                Logger.LogError($"Error al consumir licencia: {respuesta.StatusCode} - {contenido}")
                Throw New ApplicationException($"Error al consumir licencia: {contenido}")
            End If
            
            ' Parsear respuesta
            Dim resultado = JObject.Parse(contenido)
            Dim licenciasRestantes As Integer = If(resultado("licenciasRestantes") IsNot Nothing, 
                                                   resultado("licenciasRestantes").ToObject(Of Integer)(), 
                                                   0)
            
            Logger.LogInfo($"Licencia consumida exitosamente. Restantes: {licenciasRestantes}")
            
            Return licenciasRestantes
            
        Catch ex As Exception
            Logger.LogError("Error en AuthService.ConsumirLicencia", ex)
            Throw New ApplicationException("Error al consumir licencia: " & ex.Message, ex)
        End Try
    End Function
End Class

''' <summary>
''' Resultado de autenticación
''' </summary>
Public Class AuthResult
    Public Property Success As Boolean
    Public Property UserId As Integer
    Public Property Nombre As String
    Public Property Email As String
    Public Property IdEntidad As Integer
    Public Property EntidadNombre As String
    Public Property Opciones As JArray
    Public Property Message As String
    
    ' NUEVO: Propiedades multi-entidad
    Public Property TipoUsuario As String
    Public Property Entidades As JArray
    Public Property IdEntidadPrincipal As Integer?
    Public Property LicenciasDisponibles As Integer
End Class
