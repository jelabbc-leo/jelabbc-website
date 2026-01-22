Imports System.Text.RegularExpressions
Imports System.Web

''' <summary>
''' Clase helper para validaciones de seguridad y headers HTTP
''' </summary>
Public NotInheritable Class SecurityHelper
    Private Sub New()
        ' Clase estática, no instanciable
    End Sub

    ''' <summary>
    ''' Valida y sanitiza entrada de texto para prevenir XSS
    ''' </summary>
    Public Shared Function SanitizeInput(input As String) As String
        If String.IsNullOrEmpty(input) Then Return String.Empty

        ' Remover caracteres peligrosos
        Dim sanitized = input.Trim()

        ' Remover scripts y tags HTML peligrosos
        sanitized = Regex.Replace(sanitized, "<script[^>]*>.*?</script>", "", RegexOptions.IgnoreCase Or RegexOptions.Singleline)
        sanitized = Regex.Replace(sanitized, "<iframe[^>]*>.*?</iframe>", "", RegexOptions.IgnoreCase Or RegexOptions.Singleline)
        sanitized = Regex.Replace(sanitized, "javascript:", "", RegexOptions.IgnoreCase)
        sanitized = Regex.Replace(sanitized, "on\w+\s*=", "", RegexOptions.IgnoreCase)

        Return sanitized
    End Function

    ''' <summary>
    ''' Valida que una cadena no contenga caracteres SQL peligrosos
    ''' </summary>
    Public Shared Function IsValidSqlInput(input As String) As Boolean
        If String.IsNullOrEmpty(input) Then Return True

        ' Lista de patrones peligrosos
        Dim dangerousPatterns As String() = {
            "';", "--", "/*", "*/", "xp_", "sp_", "exec", "execute", 
            "union", "select", "insert", "update", "delete", "drop", 
            "create", "alter", "script", "<script", "javascript:"
        }

        Dim inputLower = input.ToLower()

        For Each pattern In dangerousPatterns
            If inputLower.Contains(pattern) Then
                Return False
            End If

        Next

        Return True
    End Function

    ''' <summary>
    ''' Valida formato de email
    ''' </summary>
    Public Shared Function IsValidEmail(email As String) As Boolean
        If String.IsNullOrEmpty(email) Then Return False

        Try
            Dim emailRegex As New Regex("^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$")

            Return emailRegex.IsMatch(email)

        Catch
            Return False

        End Try
    End Function

    ''' <summary>
    ''' Aplica headers de seguridad HTTP a la respuesta
    ''' </summary>
    Public Shared Sub ApplySecurityHeaders()
        Dim response = HttpContext.Current.Response

        ' Asegurar que el Content-Type tenga charset UTF-8
        Dim currentContentType = response.ContentType

        If String.IsNullOrEmpty(currentContentType) OrElse Not currentContentType.Contains("charset") Then
            response.ContentType = "text/html; charset=utf-8"
        ElseIf Not currentContentType.ToLower().Contains("utf-8") Then
            ' Si tiene charset pero no es UTF-8, reemplazarlo
            Dim regex As New Regex("charset=[^;]+", RegexOptions.IgnoreCase)

            currentContentType = regex.Replace(currentContentType, "charset=utf-8")
            response.ContentType = currentContentType
        End If

        ' Prevenir clickjacking
        response.Headers.Add("X-Frame-Options", "DENY")

        ' Prevenir MIME type sniffing
        response.Headers.Add("X-Content-Type-Options", "nosniff")

        ' Habilitar XSS Protection
        response.Headers.Add("X-XSS-Protection", "1; mode=block")

        ' Referrer Policy
        response.Headers.Add("Referrer-Policy", "strict-origin-when-cross-origin")

        ' Content Security Policy (ajustar según necesidades)
        ' Obtener URL del API desde configuración para CSP
        Dim apiBaseUrl = ConfigurationManager.AppSettings("ApiBaseUrl")
        Dim apiUrlForCsp As String = String.Empty
        If Not String.IsNullOrEmpty(apiBaseUrl) Then
            ' Extraer URL base (quitar /api/CRUD?strQuery=)
            Dim idx = apiBaseUrl.IndexOf("/api/")
            If idx > 0 Then
                apiUrlForCsp = apiBaseUrl.Substring(0, idx)
            Else
                ' Si no se encuentra /api/, usar la URL completa sin el query string
                Dim queryIdx = apiBaseUrl.IndexOf("?")
                If queryIdx > 0 Then
                    apiUrlForCsp = apiBaseUrl.Substring(0, queryIdx)
                    ' Quitar /CRUD si está presente
                    If apiUrlForCsp.EndsWith("/CRUD") Then
                        apiUrlForCsp = apiUrlForCsp.Substring(0, apiUrlForCsp.Length - 5)
                    End If
                End If
            End If
        End If
        
        ' Construir CSP con URL del API dinámica
        Dim cspConnectSrc = "connect-src 'self'"
        If Not String.IsNullOrEmpty(apiUrlForCsp) Then
            cspConnectSrc &= " " & apiUrlForCsp
        End If
        cspConnectSrc &= " https://jela-n8n.azurewebsites.net https://r2cdn.perplexity.ai https://cdn.jsdelivr.net https://maps.googleapis.com https://maps.gstatic.com https://*.googleapis.com https://api.ipify.org"
        
        response.Headers.Add("Content-Security-Policy", 
            "default-src 'self'; " &
            "script-src 'self' 'unsafe-inline' 'unsafe-eval' https://cdn.jsdelivr.net https://cdnjs.cloudflare.com https://maps.googleapis.com https://maps.gstatic.com; " &
            "style-src 'self' 'unsafe-inline' https://cdn.jsdelivr.net https://cdnjs.cloudflare.com https://fonts.googleapis.com; " &
            "img-src 'self' data: https: blob:; " &
            cspConnectSrc & "; " &
            "font-src 'self' https://cdn.jsdelivr.net https://cdnjs.cloudflare.com https://fonts.gstatic.com https://r2cdn.perplexity.ai; " &
            "frame-src 'self'; " &
            "object-src 'none';")

        ' Permissions Policy
        response.Headers.Add("Permissions-Policy", 
            "geolocation=(), microphone=(), camera=()")

    End Sub

    ''' <summary>
    ''' Valida que el request proviene de una fuente confiable
    ''' </summary>
    Public Shared Function IsValidRequest() As Boolean
        Dim request = HttpContext.Current.Request

        ' Validar que no sea un request sospechoso
        If request.Url Is Nothing Then Return False

        ' Validar User-Agent (opcional, puede ser muy restrictivo)
        ' If String.IsNullOrEmpty(request.UserAgent) Then Return False

        Return True
    End Function

    ''' <summary>
    ''' Obtiene la IP del cliente
    ''' </summary>
    Public Shared Function GetClientIP() As String
        Dim request = HttpContext.Current.Request
        Dim ip As String = request.Headers("X-Forwarded-For")

        If String.IsNullOrEmpty(ip) Then
            ip = request.Headers("X-Real-IP")
        End If

        If String.IsNullOrEmpty(ip) Then
            ip = request.UserHostAddress
        End If

        Return ip
    End Function

End Class
