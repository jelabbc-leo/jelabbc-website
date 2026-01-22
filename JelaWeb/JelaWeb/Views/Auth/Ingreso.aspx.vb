Imports Newtonsoft.Json.Linq
Imports System.Web.UI.HtmlControls

Public Class Ingreso
    Inherits System.Web.UI.Page

    Private authService As New AuthService()

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' Ocultar el contenedor de error por defecto
        Dim errorContainer As HtmlGenericControl = TryCast(FindControl("errorContainer"), HtmlGenericControl)

        If errorContainer IsNot Nothing Then
            errorContainer.Style("display") = "none"
        End If

        If lblError IsNot Nothing Then
            lblError.Visible = False
        End If

        ' Limpiar sesión si viene de logout o expiración
        If Request.QueryString("expired") = "1" Then
            ShowError("Su sesión ha expirado. Por favor, ingrese nuevamente.")
            SessionHelper.ClearSession()
        ElseIf Request.QueryString("logout") = "1" Then
            ShowError("Ha cerrado sesión correctamente.")
        End If

        ' Si ya está autenticado, redirigir al inicio
        If SessionHelper.IsAuthenticated() Then
            Response.Redirect(Constants.ROUTE_INICIO, True)
        End If

        ' Agregar atributos para efectos CSS
        If Not IsPostBack Then
            txtUsername.Attributes.Add("autocomplete", "username")
            txtPassword.Attributes.Add("autocomplete", "current-password")
        End If
    End Sub

    Private Sub ShowError(message As String)
        lblError.Text = message
        lblError.Visible = True
        Dim errorContainer As HtmlGenericControl = TryCast(FindControl("errorContainer"), HtmlGenericControl)

        If errorContainer IsNot Nothing Then
            errorContainer.Style("display") = "block"
        End If
    End Sub

    Protected Sub btnLogin_Click(sender As Object, e As EventArgs) Handles btnLogin.Click

        Try
            ' DEBUG: Log de valores recibidos
            Dim usernameLength As Integer = If(txtUsername.Text Is Nothing, 0, txtUsername.Text.Length)
            Dim passwordLength As Integer = If(txtPassword.Text Is Nothing, 0, txtPassword.Text.Length)
            
            Logger.LogInfo($"DEBUG btnLogin_Click - txtUsername.Text: '{txtUsername.Text}' (Length: {usernameLength})")
            Logger.LogInfo($"DEBUG btnLogin_Click - txtPassword.Text: '{If(String.IsNullOrEmpty(txtPassword.Text), "EMPTY", "***")}' (Length: {passwordLength})")
            Logger.LogInfo($"DEBUG btnLogin_Click - IsNullOrWhiteSpace(txtUsername): {String.IsNullOrWhiteSpace(txtUsername.Text)}")
            Logger.LogInfo($"DEBUG btnLogin_Click - IsNullOrWhiteSpace(txtPassword): {String.IsNullOrWhiteSpace(txtPassword.Text)}")
            
            ' Validar entrada
            If String.IsNullOrWhiteSpace(txtUsername.Text) Or String.IsNullOrWhiteSpace(txtPassword.Text) Then
                ShowError("Por favor, ingrese usuario y contraseña.")
                Logger.LogWarning("Intento de login con campos vacíos", SecurityHelper.GetClientIP())
                Return
            End If

            ' Sanitizar entrada
            Dim username As String = SecurityHelper.SanitizeInput(txtUsername.Text)
            Dim password As String = txtPassword.Text ' No sanitizar password

            ' Validar que no contenga caracteres peligrosos
            If Not SecurityHelper.IsValidSqlInput(username) Then
                ShowError("El nombre de usuario contiene caracteres no permitidos.")
                Logger.LogWarning($"Intento de login con caracteres peligrosos: {username}", SecurityHelper.GetClientIP())
                Return
            End If

            ' Autenticar usando AuthService (API dinámica - más rápido que n8n)
            Dim result As AuthResult = authService.Autenticar(username, password)

            ' Validamos la respuesta
            If result.Success Then
                ' Validar que se obtuvo un userId válido
                If result.UserId <= 0 Then
                    Logger.LogError("Error: No se pudo obtener un userId válido del resultado del login")
                    ShowError("Error al procesar la respuesta del servidor. Por favor, intente nuevamente.")
                    Return
                End If

                ' Inicializar sesión usando SessionHelper con datos multi-entidad
                SessionHelper.InitializeSession(
                    result.UserId, 
                    result.Nombre, 
                    result.Opciones, 
                    result.TipoUsuario,
                    result.Entidades,
                    result.LicenciasDisponibles,
                    result.IdEntidadPrincipal
                )

                ' Verificar que la sesión se haya inicializado correctamente
                If Not SessionHelper.IsAuthenticated() Then
                    Logger.LogError($"Error: La sesión no se inicializó correctamente después del login. UserId: {result.UserId}, Nombre: {result.Nombre}")
                    ShowError("Error al inicializar la sesión. Por favor, intente nuevamente.")
                    Return
                End If

                ' Log de login exitoso con detalles
                Logger.LogInfo($"Login exitoso para usuario: {username} (ID: {result.UserId}, Nombre: {result.Nombre}, TipoUsuario: {result.TipoUsuario})", username)

                ' DEBUG: Log detallado para diagnóstico
                Logger.LogInfo($"DEBUG Login - TipoUsuario recibido: '{result.TipoUsuario}'")
                Logger.LogInfo($"DEBUG Login - Constante esperada: '{Constants.TIPO_USUARIO_ADMIN_CONDOMINIOS}'")
                Logger.LogInfo($"DEBUG Login - ¿Son iguales?: {result.TipoUsuario = Constants.TIPO_USUARIO_ADMIN_CONDOMINIOS}")
                Logger.LogInfo($"DEBUG Login - Comparación case-insensitive: {String.Equals(result.TipoUsuario, Constants.TIPO_USUARIO_ADMIN_CONDOMINIOS, StringComparison.OrdinalIgnoreCase)}")
                Logger.LogInfo($"DEBUG Login - Longitud TipoUsuario: {result.TipoUsuario.Length}")
                Logger.LogInfo($"DEBUG Login - Longitud Constante: {Constants.TIPO_USUARIO_ADMIN_CONDOMINIOS.Length}")

                ' Redirigir según tipo de usuario
                If result.TipoUsuario = Constants.TIPO_USUARIO_ADMIN_CONDOMINIOS Then
                    ' Administrador de Condominios: redirigir a selector de entidades
                    Logger.LogInfo($"Usuario {username} es Administrador de Condominios, redirigiendo a selector de entidades")
                    Response.Redirect(Constants.ROUTE_SELECTOR_ENTIDADES, False)
                Else
                    ' Usuario interno: redirigir directamente a inicio (ya tiene entidad asignada)
                    Logger.LogInfo($"Usuario {username} es {result.TipoUsuario}, redirigiendo a inicio")
                    Server.Transfer(Constants.ROUTE_INICIO, True)
                End If
            Else
                ' Manejar error de autenticación
                Dim errorMsg As String = result.Message

                ' Detectar si el error es de credenciales inválidas
                Dim isInvalidCredentials As Boolean = False

                If Not String.IsNullOrEmpty(errorMsg) Then
                    Dim errorMsgLower As String = errorMsg.ToLower()

                    isInvalidCredentials = errorMsgLower = "credenciales_invalidas" OrElse
                                          errorMsgLower.Contains("credenciales") OrElse
                                          errorMsgLower.Contains("inactivo")
                Else
                    isInvalidCredentials = True
                End If

                If isInvalidCredentials Then
                    ShowError("Las credenciales ingresadas no son válidas. Por favor, verifique su usuario y contraseña.")
                Else
                    ShowError(If(String.IsNullOrEmpty(errorMsg), "Error al intentar iniciar sesión.", errorMsg))
                End If

                ' Log de intento fallido
                Logger.LogWarning($"Intento de login fallido para usuario: {username} - {errorMsg}", SecurityHelper.GetClientIP())
            End If

        Catch ex As Exception
            ' Log del error
            Logger.LogError("Error durante el proceso de login", ex)
            ShowError("Ha ocurrido un error al intentar iniciar sesión. Por favor, intente nuevamente.")

        End Try
    End Sub

End Class
