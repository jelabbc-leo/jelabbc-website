Imports System.Web
Imports System.Web.SessionState
Imports Newtonsoft.Json
Imports JelaWeb

''' <summary>
''' Handler para obtener información del usuario autenticado
''' Usado por el chat widget para pre-llenar nombre y email
''' </summary>
Public Class UserInfoHandler
    Implements IHttpHandler, IRequiresSessionState
    
    Public Sub ProcessRequest(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest
        Try
            ' Configurar CORS si es necesario
            context.Response.AddHeader("Access-Control-Allow-Origin", "*")
            context.Response.AddHeader("Access-Control-Allow-Methods", "GET, POST, OPTIONS")
            context.Response.AddHeader("Access-Control-Allow-Headers", "Content-Type")
            
            ' Verificar autenticación
            If Not SessionHelper.IsAuthenticated() Then
                context.Response.StatusCode = 401
                context.Response.ContentType = "application/json"
                context.Response.Write(JsonConvert.SerializeObject(New With {
                    .Success = False,
                    .Message = "Usuario no autenticado"
                }))
                Return
            End If
            
            ' Obtener datos del usuario
            Dim userId = SessionHelper.GetUserId()
            Dim nombre = SessionHelper.GetNombre()
            Dim idEntidad = SessionHelper.GetIdEntidadActual()
            
            ' DEBUG: Log para diagnosticar problema de IdEntidad
            Logger.LogInfo($"UserInfoHandler - UserId: {userId}, Nombre: {nombre}, IdEntidadActual: {If(idEntidad.HasValue, idEntidad.Value.ToString(), "NULL")}")
            
            ' Obtener email del usuario desde la sesión o base de datos
            Dim email As String = ""
            Try
                ' Intentar obtener email de la sesión primero
                If context.Session("Email") IsNot Nothing Then
                    email = context.Session("Email").ToString()
                Else
                    ' Si no está en sesión, intentar obtener de la base de datos usando DynamicCrudService
                    Dim row = DynamicCrudService.ObtenerPorId("cat_usuarios", userId)
                    
                    If row IsNot Nothing AndAlso Not row.IsNull("Email") Then
                        email = row("Email").ToString()
                    End If
                End If
            Catch ex As Exception
                ' Si no se puede obtener el email, usar uno basado en el nombre de usuario
                Logger.LogWarning("No se pudo obtener email del usuario, usando email genérico")
                email = $"usuario{userId}@jelaweb.com"
            End Try
            
            ' Retornar información del usuario
            ' TEMPORAL: Valor fijo para pruebas - siempre devolver 1
            Dim idEntidadFinal As Integer = 1  ' ← HARDCODED para pruebas
            
            Dim userInfo = New With {
                .Success = True,
                .UserId = userId,
                .Nombre = nombre,
                .Email = email,
                .IdEntidad = idEntidadFinal,
                .IsAuthenticated = True
            }
            
            context.Response.ContentType = "application/json"
            context.Response.Write(JsonConvert.SerializeObject(userInfo))
            
        Catch ex As Exception
            Logger.LogError("Error en UserInfoHandler", ex)
            
            context.Response.StatusCode = 500
            context.Response.ContentType = "application/json"
            context.Response.Write(JsonConvert.SerializeObject(New With {
                .Success = False,
                .Message = "Error al obtener información del usuario",
                .Error = ex.Message
            }))
        End Try
    End Sub
    
    Public ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property
    
End Class
