Imports System.IO
Imports System.Web
Imports System

''' <summary>
''' Clase para logging de eventos y errores de la aplicación
''' </summary>
Public NotInheritable Class Logger
    Private Sub New()
        ' Clase estática, no instanciable
    End Sub

    Private Shared ReadOnly MaxLogFileSize As Long = Constants.LOG_MAX_FILE_SIZE_MB * 1024 * 1024

    ''' <summary>
    ''' Obtiene el directorio de logs de forma segura
    ''' </summary>
    Private Shared Function GetLogDirectory() As String

        Try
            If HttpContext.Current IsNot Nothing AndAlso HttpContext.Current.Server IsNot Nothing Then
                Return HttpContext.Current.Server.MapPath("~/App_Data/Logs")
            Else
                ' Fallback: usar ruta relativa si no hay HttpContext
                Return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "Logs")
            End If

        Catch
            ' Último recurso: usar ruta absoluta
            Return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "Logs")

        End Try
    End Function

    ''' <summary>
    ''' Escribe un mensaje de información en el log
    ''' </summary>
    Public Shared Sub LogInfo(mensaje As String, Optional usuario As String = "")
        WriteLog("INFO", mensaje, usuario)
    End Sub

    ''' <summary>
    ''' Escribe un mensaje de advertencia en el log
    ''' </summary>
    Public Shared Sub LogWarning(mensaje As String, Optional usuario As String = "")
        WriteLog("WARNING", mensaje, usuario)
    End Sub

    ''' <summary>
    ''' Escribe un error en el log
    ''' </summary>
    Public Shared Sub LogError(mensaje As String, ex As Exception, Optional usuario As String = "")
        Dim errorDetails As String = mensaje

        If ex IsNot Nothing Then
            errorDetails &= vbCrLf & "Exception: " & ex.GetType().Name & vbCrLf &
                           "Message: " & ex.Message & vbCrLf &
                           "Stack Trace: " & If(ex.StackTrace IsNot Nothing, ex.StackTrace, "N/A")

            If ex.InnerException IsNot Nothing Then
                errorDetails &= vbCrLf & "Inner Exception: " & ex.InnerException.Message
            End If
        Else
            errorDetails &= vbCrLf & "Exception: (ex fue Nothing)"
        End If

        WriteLog("ERROR", errorDetails, usuario)
    End Sub

    ''' <summary>
    ''' Escribe un error simple en el log
    ''' </summary>
    Public Shared Sub LogError(mensaje As String, Optional usuario As String = "")
        WriteLog("ERROR", mensaje, usuario)
    End Sub

    ''' <summary>
    ''' Escribe un mensaje de depuración en el log (solo en modo debug)
    ''' </summary>
    Public Shared Sub LogDebug(mensaje As String, Optional usuario As String = "")
#If DEBUG Then
        WriteLog("DEBUG", mensaje, usuario)
#End If
    End Sub

    ''' <summary>
    ''' Método privado para escribir en el archivo de log
    ''' </summary>
    Private Shared Sub WriteLog(level As String, mensaje As String, usuario As String)

        Try
            Dim logDirectory As String = GetLogDirectory()

            ' Crear directorio si no existe
            If Not Directory.Exists(logDirectory) Then
                Directory.CreateDirectory(logDirectory)
            End If

            Dim logFileName As String = Path.Combine(logDirectory, $"JelaWeb_{DateTime.Now:yyyyMMdd}.log")
            Dim logEntry As String = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{level}]"

            If Not String.IsNullOrEmpty(usuario) Then
                logEntry &= $" [Usuario: {usuario}]"
            End If

            logEntry &= $" {mensaje}" & vbCrLf

            ' Rotar archivo si es muy grande
            If File.Exists(logFileName) AndAlso New FileInfo(logFileName).Length > MaxLogFileSize Then
                Dim backupFileName As String = Path.Combine(logDirectory, $"JelaWeb_{DateTime.Now:yyyyMMdd}_{DateTime.Now:HHmmss}.log")

                File.Move(logFileName, backupFileName)
            End If

            ' Escribir en el archivo
            File.AppendAllText(logFileName, logEntry)

        Catch ex As Exception
            ' Si falla el logging, intentar escribir en el Event Log de Windows

            Try
                System.Diagnostics.EventLog.WriteEntry("Application", 
                    $"Error al escribir log: {ex.Message} | Mensaje original: {mensaje}", 
                    System.Diagnostics.EventLogEntryType.Error)

            Catch
                ' Si también falla, no hacer nada para evitar loops infinitos

            End Try
        End Try
    End Sub

    ''' <summary>
    ''' Limpia archivos de log antiguos (más de 30 días)
    ''' </summary>
    Public Shared Sub CleanOldLogs()

        Try
            Dim logDirectory As String = GetLogDirectory()

            If Not Directory.Exists(logDirectory) Then Return

            Dim cutoffDate As DateTime = DateTime.Now.AddDays(-Constants.LOG_RETENTION_DAYS)
            Dim logFiles = Directory.GetFiles(logDirectory, "JelaWeb_*.log")

            For Each logFile In logFiles
                Dim fileInfo As New FileInfo(logFile)

                If fileInfo.CreationTime < cutoffDate Then
                    File.Delete(logFile)
                End If

            Next

        Catch ex As Exception
            ' Logear el error pero no lanzar excepción
            System.Diagnostics.EventLog.WriteEntry("Application", 
                $"Error al limpiar logs antiguos: {ex.Message}", 
                System.Diagnostics.EventLogEntryType.Warning)

        End Try
    End Sub

End Class
