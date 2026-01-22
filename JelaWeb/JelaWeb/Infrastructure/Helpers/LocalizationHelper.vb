Imports System.Globalization
Imports System.Threading
Imports System.Web

''' <summary>
''' Helper para gestión de internacionalización y localización
''' </summary>
Public NotInheritable Class LocalizationHelper
    Private Sub New()
        ' Clase estática, no instanciable
    End Sub

    ''' <summary>
    ''' Obtiene la cultura actual desde la sesión o navegador
    ''' </summary>
    Public Shared Function GetCurrentCulture() As CultureInfo
        Dim session = HttpContext.Current.Session

        If session IsNot Nothing AndAlso session("Culture") IsNot Nothing Then

            Try
                Return New CultureInfo(session("Culture").ToString())

            Catch
                ' Si falla, usar español como predeterminado

            End Try
        End If

        ' Predeterminado: español de México
        Return New CultureInfo("es-MX")
    End Function

    ''' <summary>
    ''' Establece la cultura en la sesión y el hilo actual
    ''' </summary>
    Public Shared Sub SetCulture(cultureName As String)
        Dim session = HttpContext.Current.Session

        If session IsNot Nothing Then
            session("Culture") = cultureName
        End If

        Dim culture = New CultureInfo(cultureName)
        Dim uiCulture = GetUiCulture(culture)

        Thread.CurrentThread.CurrentCulture = culture
        Thread.CurrentThread.CurrentUICulture = uiCulture
    End Sub

    ''' <summary>
    ''' Obtiene un string localizado desde los recursos
    ''' </summary>
    Public Shared Function GetString(key As String) As String

        Try
            Dim culture = GetCurrentCulture()

            ' Usar GetGlobalResourceObject para archivos en App_GlobalResources
            Dim value = System.Web.HttpContext.GetGlobalResourceObject("Resources", key, culture)

            If value IsNot Nothing Then
                Return value.ToString()
            End If

            ' Si no hay valor, retornar la clave
            Return key

        Catch ex As Exception
            Logger.LogError($"Error al obtener recurso '{key}'", ex)
            Return key

        End Try
    End Function

    ''' <summary>
    ''' Obtiene un string localizado con formato
    ''' </summary>
    Public Shared Function GetString(key As String, ParamArray args As Object()) As String

        Try
            Dim template = GetString(key)

            Return String.Format(template, args)

        Catch ex As Exception
            Logger.LogError($"Error al formatear recurso '{key}'", ex)
            Return key

        End Try
    End Function

    ''' <summary>
    ''' Aplica la cultura actual al hilo
    ''' </summary>
    Public Shared Sub ApplyCulture()
        Dim culture = GetCurrentCulture()
        Dim uiCulture = GetUiCulture(culture)

        Thread.CurrentThread.CurrentCulture = culture
        Thread.CurrentThread.CurrentUICulture = uiCulture
    End Sub

    Private Shared Function GetUiCulture(culture As CultureInfo) As CultureInfo
        If culture Is Nothing Then
            Return New CultureInfo("es")
        End If

        If culture.Name.StartsWith("es", StringComparison.OrdinalIgnoreCase) Then
            Return New CultureInfo("es")
        End If

        Return culture
    End Function

End Class
