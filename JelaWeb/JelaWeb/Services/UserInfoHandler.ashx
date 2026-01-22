<%@ WebHandler Language="VB" Class="UserInfoHandler" %>

Imports System.Web
Imports System.Web.SessionState
Imports Newtonsoft.Json
Imports JelaWeb

Public Class UserInfoHandler
    Implements IHttpHandler, IRequiresSessionState
    
    Public Sub ProcessRequest(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest
        Try
            context.Response.AddHeader("Access-Control-Allow-Origin", "*")
            context.Response.AddHeader("Access-Control-Allow-Methods", "GET, POST, OPTIONS")
            context.Response.AddHeader("Access-Control-Allow-Headers", "Content-Type")
            
            If Not SessionHelper.IsAuthenticated() Then
                context.Response.StatusCode = 401
                context.Response.ContentType = "application/json"
                context.Response.Write(JsonConvert.SerializeObject(New With {
                    .Success = False,
                    .Message = "Usuario no autenticado"
                }))
                Return
            End If
            
            Dim userId = SessionHelper.GetUserId()
            Dim nombre = SessionHelper.GetNombre()
            Dim idEntidadNullable = SessionHelper.GetIdEntidadActual()
            Dim idEntidad As Integer = If(idEntidadNullable.HasValue, idEntidadNullable.Value, 1)
            
            Dim email As String = ""
            Try
                If context.Session("Email") IsNot Nothing Then
                    email = context.Session("Email").ToString()
                End If
            Catch ex As Exception
                Logger.LogWarning("Error al obtener email del usuario desde sesion")
            End Try
            
            If String.IsNullOrWhiteSpace(email) Then
                email = String.Format("usuario{0}@jelaweb.com", userId)
                Logger.LogWarning(String.Format("Email no encontrado para usuario {0}, usando email por defecto", userId))
            End If
            
            Dim userInfo = New With {
                .Success = True,
                .UserId = userId,
                .Nombre = nombre,
                .Email = email,
                .IdEntidad = idEntidad,
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
                .Message = "Error al obtener informacion del usuario",
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
