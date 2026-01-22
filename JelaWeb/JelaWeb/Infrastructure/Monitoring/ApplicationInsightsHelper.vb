Imports System.Configuration
Imports System.Web

''' <summary>
''' Clase helper para integración básica con Azure Application Insights
''' Nota: Requiere el paquete NuGet Microsoft.ApplicationInsights.Web
''' </summary>
Public NotInheritable Class ApplicationInsightsHelper
    Private Sub New()
        ' Clase estática, no instanciable
    End Sub

    ''' <summary>
    ''' Verifica si Application Insights está configurado
    ''' </summary>
    Public Shared Function IsConfigured() As Boolean
        Dim instrumentationKey = ConfigurationManager.AppSettings("ApplicationInsights:InstrumentationKey")

        Return Not String.IsNullOrEmpty(instrumentationKey) AndAlso instrumentationKey <> "YOUR_INSTRUMENTATION_KEY_HERE"
    End Function

    ''' <summary>
    ''' Registra un evento personalizado
    ''' Nota: Requiere Microsoft.ApplicationInsights.Web instalado
    ''' </summary>
    Public Shared Sub TrackEvent(eventName As String, Optional properties As Dictionary(Of String, String) = Nothing)
        If Not IsConfigured() Then Return

        Try
            ' Este código requiere el paquete NuGet Microsoft.ApplicationInsights.Web
            ' Descomentar cuando se instale el paquete:
            '
            ' Dim telemetry = New Microsoft.ApplicationInsights.TelemetryClient()
            ' telemetry.TrackEvent(eventName, properties)
            ' telemetry.Flush()
            '
            Logger.LogInfo($"Application Insights Event: {eventName}")

        Catch ex As Exception
            ' Si Application Insights no está disponible, solo loguear
            Logger.LogWarning($"No se pudo registrar evento en Application Insights: {ex.Message}")

        End Try
    End Sub

    ''' <summary>
    ''' Registra una excepción
    ''' </summary>
    Public Shared Sub TrackException(ex As Exception, Optional properties As Dictionary(Of String, String) = Nothing)
        If Not IsConfigured() Then Return

        Try
            ' Este código requiere el paquete NuGet Microsoft.ApplicationInsights.Web
            ' Descomentar cuando se instale el paquete:
            '
            ' Dim telemetry = New Microsoft.ApplicationInsights.TelemetryClient()
            ' telemetry.TrackException(ex, properties)
            ' telemetry.Flush()
            '
            Logger.LogError("Application Insights Exception", ex)

        Catch
            ' Si Application Insights no está disponible, solo loguear
            Logger.LogError("Error al registrar excepción en Application Insights", ex)

        End Try
    End Sub

    ''' <summary>
    ''' Registra una métrica personalizada
    ''' </summary>
    Public Shared Sub TrackMetric(metricName As String, value As Double, Optional properties As Dictionary(Of String, String) = Nothing)
        If Not IsConfigured() Then Return

        Try
            ' Este código requiere el paquete NuGet Microsoft.ApplicationInsights.Web
            ' Descomentar cuando se instale el paquete:
            '
            ' Dim telemetry = New Microsoft.ApplicationInsights.TelemetryClient()
            ' telemetry.TrackMetric(metricName, value, properties)
            ' telemetry.Flush()
            '
            Logger.LogInfo($"Application Insights Metric: {metricName} = {value}")

        Catch ex As Exception
            Logger.LogWarning($"No se pudo registrar métrica en Application Insights: {ex.Message}")

        End Try
    End Sub

End Class
