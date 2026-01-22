Imports System.Web
Imports System.Web.UI

Public Class ErrorPage
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' Obtener código de error del query string (por defecto 500)
        Dim errorCode As String = Request.QueryString("code")

        If String.IsNullOrEmpty(errorCode) Then
            errorCode = "500"
        End If

        ' Obtener mensajes personalizados del query string
        Dim customMessage As String = Request.QueryString("msg")
        Dim customDescription As String = Request.QueryString("desc")

        ' Configurar la página según el código de error
        ConfigureErrorPage(errorCode, customMessage, customDescription)

        ' Establecer código de estado HTTP

        Try
            Dim statusCode As Integer = Convert.ToInt32(errorCode)

            Response.StatusCode = statusCode
            Response.StatusDescription = GetStatusDescription(statusCode)

        Catch
            Response.StatusCode = 500
            Response.StatusDescription = "Internal Server Error"

        End Try
    End Sub

    Private Sub ConfigureErrorPage(code As String, customMessage As String, customDescription As String)
        Dim errorInfo As ErrorInfo = GetErrorInfo(code)

        ' Aplicar estilos CSS dinámicos según el tipo de error
        Dim style As String = $"

        <style>
            :root {{
                --error-bg-gradient: {errorInfo.BackgroundGradient};
                --error-icon-color: {errorInfo.IconColor};
                --error-code-color: {errorInfo.CodeColor};
                --error-btn-color: {errorInfo.ButtonColor};
                --error-btn-hover: {errorInfo.ButtonHover};
            }}
        </style>"
        Page.ClientScript.RegisterStartupScript(Me.GetType(), "ErrorStyles", style, False)

        ' Configurar icono
        errorIcon.Attributes("class") = $"error-icon {errorInfo.IconClass}"

        ' Configurar código de error
        errorCode.InnerHtml = code

        ' Configurar mensaje
        If Not String.IsNullOrEmpty(customMessage) Then
            errorMessage.InnerHtml = HttpUtility.HtmlEncode(customMessage)
        Else
            errorMessage.InnerHtml = HttpUtility.HtmlEncode(errorInfo.Message)
        End If

        ' Configurar descripción
        If Not String.IsNullOrEmpty(customDescription) Then
            errorDescription.InnerHtml = HttpUtility.HtmlEncode(customDescription)
        Else
            errorDescription.InnerHtml = HttpUtility.HtmlEncode(errorInfo.Description)
        End If

        ' Configurar botón de acción
        btnAction.HRef = errorInfo.ActionUrl
        btnIcon.Attributes("class") = errorInfo.ButtonIcon
        btnText.InnerHtml = errorInfo.ButtonText
    End Sub

    Private Function GetErrorInfo(code As String) As ErrorInfo
        Select Case code

            Case "403"
                Return New ErrorInfo With {
                    .BackgroundGradient = "linear-gradient(135deg, #f093fb 0%, #f5576c 100%)",
                    .IconClass = "fas fa-lock",
                    .IconColor = "#f5576c",
                    .CodeColor = "#f5576c",
                    .ButtonColor = "#f5576c",
                    .ButtonHover = "#e0455a",
                    .Message = "Acceso no autorizado",
                    .Description = "No tiene permisos para acceder a esta página. Por favor, inicie sesión para continuar.",
                    .ActionUrl = ResolveUrl("~/Views/Auth/Ingreso.aspx"),
                    .ButtonIcon = "fas fa-sign-in-alt",
                    .ButtonText = "Ir al inicio de sesión"
                }

            Case "404"
                Return New ErrorInfo With {
                    .BackgroundGradient = "linear-gradient(135deg, #667eea 0%, #764ba2 100%)",
                    .IconClass = "fas fa-exclamation-triangle",
                    .IconColor = "#f39c12",
                    .CodeColor = "#667eea",
                    .ButtonColor = "#667eea",
                    .ButtonHover = "#5568d3",
                    .Message = "Página no encontrada",
                    .Description = "Lo sentimos, la página que está buscando no existe o ha sido movida.",
                    .ActionUrl = ResolveUrl("~/Views/Inicio.aspx"),
                    .ButtonIcon = "fas fa-home",
                    .ButtonText = "Volver al inicio"
                }

            Case "500"
                Return New ErrorInfo With {
                    .BackgroundGradient = "linear-gradient(135deg, #f093fb 0%, #f5576c 100%)",
                    .IconClass = "fas fa-server",
                    .IconColor = "#e74c3c",
                    .CodeColor = "#f5576c",
                    .ButtonColor = "#f5576c",
                    .ButtonHover = "#e0455a",
                    .Message = "Error del servidor",
                    .Description = "Ha ocurrido un error interno en el servidor. Nuestro equipo ha sido notificado y está trabajando en solucionarlo.",
                    .ActionUrl = ResolveUrl("~/Views/Inicio.aspx"),
                    .ButtonIcon = "fas fa-home",
                    .ButtonText = "Volver al inicio"
                }

            Case Else
                Return New ErrorInfo With {
                    .BackgroundGradient = "linear-gradient(135deg, #fa709a 0%, #fee140 100%)",
                    .IconClass = "fas fa-exclamation-circle",
                    .IconColor = "#fa709a",
                    .CodeColor = "#fa709a",
                    .ButtonColor = "#fa709a",
                    .ButtonHover = "#e85a8a",
                    .Message = "Ha ocurrido un error",
                    .Description = "Por favor, intente nuevamente más tarde. Si el problema persiste, contacte al administrador del sistema.",
                    .ActionUrl = ResolveUrl("~/Views/Inicio.aspx"),
                    .ButtonIcon = "fas fa-home",
                    .ButtonText = "Volver al inicio"
                }

        End Select
    End Function

    Private Function GetStatusDescription(statusCode As Integer) As String
        Select Case statusCode

            Case 403
                Return "Forbidden"

            Case 404
                Return "Not Found"

            Case 500
                Return "Internal Server Error"

            Case Else
                Return "Error"

        End Select
    End Function

    Private Class ErrorInfo
        Public Property BackgroundGradient As String
        Public Property IconClass As String
        Public Property IconColor As String
        Public Property CodeColor As String
        Public Property ButtonColor As String
        Public Property ButtonHover As String
        Public Property Message As String
        Public Property Description As String
        Public Property ActionUrl As String
        Public Property ButtonIcon As String
        Public Property ButtonText As String
    End Class

End Class
