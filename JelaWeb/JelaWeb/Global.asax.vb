Imports System.Web.SessionState
Imports DevExpress.Web

Public Class Global_asax
    Inherits System.Web.HttpApplication

    ' Variable de aplicación para detectar reinicios
    Private Shared ApplicationStartTime As DateTime = DateTime.MinValue
    Private Const RESTART_GRACE_PERIOD_SECONDS As Integer = 10

    Sub Application_Start(ByVal sender As Object, ByVal e As EventArgs)

        Try
            ' Registrar el tiempo de inicio de la aplicación
            ApplicationStartTime = DateTime.Now

            ' Configuración de DevExpress
            AddHandler DevExpress.Web.ASPxWebControl.CallbackError, AddressOf Application_Error
            DevExpress.Security.Resources.AccessSettings.DataResources.SetRules(
                DevExpress.Security.Resources.DirectoryAccessRule.Allow(Server.MapPath("~/Content")),
                DevExpress.Security.Resources.UrlAccessRule.Allow()
            )

            ' Limpiar logs antiguos al iniciar la aplicación (con manejo de errores)

            Try
                Logger.CleanOldLogs()

            Catch ex As Exception
                ' Si falla la limpieza de logs, continuar sin bloquear el inicio
                System.Diagnostics.EventLog.WriteEntry("Application", 
                    $"Error al limpiar logs en Application_Start: {ex.Message}", 
                    System.Diagnostics.EventLogEntryType.Warning)

            End Try
            ' Log de inicio de aplicación (con manejo de errores)

            Try
                Logger.LogInfo("Aplicación JELA iniciada correctamente")

            Catch ex As Exception
                ' Si falla el logging, continuar sin bloquear el inicio
                System.Diagnostics.EventLog.WriteEntry("Application", 
                    $"Error al escribir log en Application_Start: {ex.Message}", 
                    System.Diagnostics.EventLogEntryType.Warning)

            End Try
        Catch ex As Exception
            ' Error crítico en Application_Start - registrar en Event Log
            System.Diagnostics.EventLog.WriteEntry("Application", 
                $"Error crítico en Application_Start: {ex.Message} | StackTrace: {ex.StackTrace}", 
                System.Diagnostics.EventLogEntryType.Error)
            ' Re-lanzar para que Azure muestre el error
            Throw

        End Try
    End Sub

    ''' <summary>
    ''' Verifica si la aplicación acaba de reiniciarse (útil para detectar reinicios de IIS Express en desarrollo)
    ''' </summary>
    Public Shared Function IsWithinRestartGracePeriod() As Boolean
        If ApplicationStartTime = DateTime.MinValue Then Return False
        Dim secondsSinceStart = (DateTime.Now - ApplicationStartTime).TotalSeconds
        Return secondsSinceStart <= RESTART_GRACE_PERIOD_SECONDS
    End Function

    Sub Session_Start(ByVal sender As Object, ByVal e As EventArgs)
        ' Registrar inicio de sesión (con manejo de errores)

        Try
            Dim sessionId As String = "Desconocido"
            Dim clientIp As String = "Desconocido"

            Try
                sessionId = Session.SessionID

            Catch
                ' Si la sesión no está disponible aún, usar valor por defecto

            End Try
            Try
                clientIp = SecurityHelper.GetClientIP()

            Catch
                ' Si SecurityHelper falla, continuar sin IP

            End Try
            Logger.LogInfo($"Nueva sesión iniciada - SessionID: {sessionId}", clientIp)

        Catch ex As Exception
            ' Si falla el logging, no bloquear el inicio de sesión
            System.Diagnostics.EventLog.WriteEntry("Application", 
                $"Error en Session_Start: {ex.Message}", 
                System.Diagnostics.EventLogEntryType.Warning)

        End Try
    End Sub

    Sub Application_BeginRequest(ByVal sender As Object, ByVal e As EventArgs)
        ' Aplicar headers de seguridad en cada request (con manejo de errores)

        Try
            SecurityHelper.ApplySecurityHeaders()

        Catch ex As Exception
            ' Si falla, continuar sin bloquear el request
            System.Diagnostics.EventLog.WriteEntry("Application", 
                $"Error al aplicar headers de seguridad: {ex.Message}", 
                System.Diagnostics.EventLogEntryType.Warning)

        End Try
        ' Validar request (con manejo de errores)

        Try
            If Not SecurityHelper.IsValidRequest() Then

                Try
                    Logger.LogWarning($"Request sospechoso detectado desde: {SecurityHelper.GetClientIP()} - {Request.Url}")

                Catch
                    ' Si falla el logging, continuar

                End Try
            End If

        Catch
            ' Si falla la validación, continuar sin bloquear

        End Try
    End Sub

    Sub Application_AcquireRequestState(ByVal sender As Object, ByVal e As EventArgs)
        ' En este punto, la sesión ya está disponible
        ' Validar autenticación solo para páginas .aspx (no para archivos estáticos, handlers, etc.)
        ' Esto previene acceso directo por URL sin pasar por login
        Dim absolutePath = Request.Path

        ' Verificar si es una página ASPX
        If absolutePath IsNot Nothing AndAlso absolutePath.ToLower().EndsWith(".aspx") Then
            ' Verificar si es una ruta pública ANTES de validar autenticación
            ' Esto evita redirigir a la página de login cuando se accede directamente a Ingreso.aspx
            If AuthHelper.IsPublicRoute(absolutePath) Then
                ' Es una ruta pública, permitir acceso sin validar autenticación
                Return
            End If

            ' Si NO es una ruta pública, validar autenticación
            ' Ahora la sesión ya está disponible, así que podemos verificar correctamente
            If Not AuthHelper.ValidateAndRedirect() Then
                ' La redirección ya se hizo en ValidateAndRedirect
                ' Solo necesitamos terminar el request aquí
                Return
            End If
        End If
    End Sub

    Sub Application_AuthenticateRequest(ByVal sender As Object, ByVal e As EventArgs)
        ' Aquí se puede agregar lógica de autenticación personalizada si es necesario
    End Sub

    Sub Application_Error(ByVal sender As Object, ByVal e As EventArgs)
        ' Obtener el último error
        Dim lastError As Exception = Server.GetLastError()

        ' Validar que lastError no sea Nothing antes de acceder a sus propiedades
        If lastError Is Nothing Then

            Try
                Logger.LogWarning("Application_Error: Server.GetLastError() retornó Nothing")

            Catch
                ' Si falla el logging, continuar

            End Try
            Server.ClearError()
            Return
        End If

        Dim baseException As Exception = lastError.GetBaseException()

        ' Log del error (con manejo de errores para evitar loops)
        Dim errorMessage As String = $"Error no controlado en la aplicación"
        Dim usuario As String = "Anónimo"

        Try
            usuario = SessionHelper.GetNombre()
            If String.IsNullOrEmpty(usuario) Then
                usuario = "Anónimo"
            End If

        Catch
            ' Si falla obtener el usuario, usar valor por defecto

        End Try
        Try
            Logger.LogError(errorMessage, lastError, usuario)

        Catch
            ' Si falla el logging, intentar Event Log

            Try
                System.Diagnostics.EventLog.WriteEntry("Application", 
                    $"Error crítico: {errorMessage} | {lastError.Message}", 
                    System.Diagnostics.EventLogEntryType.Error)

            Catch
                ' Si también falla, continuar sin bloquear

            End Try
        End Try
        ' Registrar en Application Insights si está configurado (con manejo de errores)

        Try
            ApplicationInsightsHelper.TrackException(lastError)

        Catch
            ' Si Application Insights falla, continuar sin bloquear

        End Try
        ' Limpiar el error del servidor
        Server.ClearError()

        ' Verificar si es un callback de DevExpress - no se puede hacer redirect en callbacks
        Dim isCallback As Boolean = False
        Try
            ' Detectar callback por parámetros de Request
            isCallback = Request.Params("__CALLBACKID") IsNot Nothing OrElse
                         Request.Params("__CALLBACKPARAM") IsNot Nothing OrElse
                         Request.Params("__ASYNCPOST") = "true" OrElse
                         Request.Headers("X-Requested-With") = "XMLHttpRequest"
        Catch
            ' Si falla, asumir que no es callback
        End Try

        ' Si es un callback, no podemos hacer redirect - establecer mensaje de error
        If isCallback Then
            Try
                ASPxWebControl.SetCallbackErrorMessage(baseException.Message)
            Catch
                ' Si falla, simplemente terminar sin redirect
            End Try
            Return
        End If

        ' Redirigir a página de error apropiada (solo para requests normales)
        Dim statusCode As Integer = 500

        If TypeOf lastError Is HttpException Then
            Dim httpEx As HttpException = DirectCast(lastError, HttpException)

            statusCode = httpEx.GetHttpCode()

            Select Case statusCode

                Case 404
                    Response.Redirect(Constants.GetErrorUrl("404"), False)
                    Return

                Case 500
                    Response.Redirect(Constants.GetErrorUrl("500"), False)
                    Return

            End Select
        End If

        ' Error general
        Response.Redirect(Constants.GetErrorUrl("500", "Ha ocurrido un error inesperado", baseException.Message), False)
    End Sub

    Sub Session_End(ByVal sender As Object, ByVal e As EventArgs)
        ' Registrar fin de sesión (con manejo de errores)

        Try
            Dim userId As String = "Desconocido"
            Dim sessionId As String = "Desconocido"

            Try
                If Session("UserId") IsNot Nothing Then
                    userId = Session("UserId").ToString()
                End If

            Catch
                ' Si falla, usar valor por defecto

            End Try
            Try
                sessionId = Session.SessionID

            Catch
                ' Si falla, usar valor por defecto

            End Try
            Logger.LogInfo($"Sesión finalizada - UserId: {userId} - SessionID: {sessionId}")

        Catch
            ' Si falla el logging, no hacer nada

        End Try
    End Sub

    Sub Application_End(ByVal sender As Object, ByVal e As EventArgs)
        ' Log de cierre de aplicación (con manejo de errores)

        Try
            Logger.LogInfo("Aplicación JELA finalizada")

        Catch
            ' Si falla el logging, continuar

        End Try
        ' Liberar recursos de HttpClient (con manejo de errores)

        Try
            HttpClientHelper.Dispose()

        Catch
            ' Si falla, continuar

        End Try
    End Sub

End Class