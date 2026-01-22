Imports System.Web

''' <summary>
''' Clase helper para forzar HTTPS en producción
''' </summary>
Public NotInheritable Class HttpsHelper
    Private Sub New()
        ' Clase estática, no instanciable
    End Sub

    ''' <summary>
    ''' Verifica si la request es HTTPS
    ''' </summary>
    Public Shared Function IsHttps() As Boolean
        Dim request = HttpContext.Current.Request

        Return request.IsSecureConnection OrElse
               request.Headers("X-Forwarded-Proto") = "https" OrElse
               request.Url.Scheme = "https"
    End Function

    ''' <summary>
    ''' Redirige a HTTPS si la request no es segura
    ''' </summary>
    Public Shared Sub RedirectToHttps()
        Dim request = HttpContext.Current.Request
        Dim response = HttpContext.Current.Response

        If Not IsHttps() Then
            Dim httpsUrl As String = request.Url.ToString().Replace("http://", "https://")

            response.RedirectPermanent(httpsUrl, True)
        End If
    End Sub

    ''' <summary>
    ''' Fuerza HTTPS solo en producción
    ''' </summary>
    Public Shared Sub EnforceHttpsInProduction()
#If Not DEBUG Then
        ' Solo en producción (Release)
        If Not IsHttps() Then
            RedirectToHttps()
        End If
#End If
    End Sub

End Class
