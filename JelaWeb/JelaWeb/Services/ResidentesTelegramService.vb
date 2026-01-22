Imports System.Data
Imports System.Collections.Generic
Imports JelaWeb.Utilities

''' <summary>
''' Servicio para gestión de Residentes Telegram
''' </summary>
Public Class ResidentesTelegramService

    Private Const TABLA_RESIDENTES As String = "conf_residentes_telegram"
    Private Const TABLA_BLACKLIST As String = "conf_residentes_blacklist"

    ''' <summary>
    ''' Lista todos los residentes de Telegram
    ''' </summary>
    Public Shared Function ListarResidentes() As DataTable

        Try
            Return DynamicCrudService.ObtenerTodos(TABLA_RESIDENTES)

        Catch ex As Exception
            Logger.LogError("ResidentesTelegramService.ListarResidentes", ex)
            Return New DataTable()

        End Try
    End Function

    ''' <summary>
    ''' Lista todos los residentes en blacklist
    ''' </summary>
    Public Shared Function ListarBlacklist() As DataTable

        Try
            Return DynamicCrudService.ObtenerTodos(TABLA_BLACKLIST)

        Catch ex As Exception
            Logger.LogError("ResidentesTelegramService.ListarBlacklist", ex)
            Return New DataTable()

        End Try
    End Function

    ''' <summary>
    ''' Guarda un residente de Telegram (crea o actualiza)
    ''' </summary>
    Public Shared Function GuardarResidente(residenteId As Integer, datos As Dictionary(Of String, Object)) As Boolean

        Try
            If residenteId = 0 Then
                datos("FechaRegistro") = DateTime.Now
                datos("TicketsMesActual") = 0
                datos("IntentosFallidos") = 0

                Return DynamicCrudService.Insertar(TABLA_RESIDENTES, datos)
            Else
                Return DynamicCrudService.Actualizar(TABLA_RESIDENTES, residenteId, datos)
            End If

        Catch ex As Exception
            Logger.LogError("ResidentesTelegramService.GuardarResidente", ex)
            Return False

        End Try
    End Function

    ''' <summary>
    ''' Obtiene un residente por ChatId
    ''' </summary>
    Public Shared Function ObtenerResidentePorChatId(chatId As Long) As DataRow

        Try
            Return DynamicCrudService.ObtenerPorCampo(TABLA_RESIDENTES, "ChatId", chatId)

        Catch ex As Exception
            Logger.LogError("ResidentesTelegramService.ObtenerResidentePorChatId", ex)
            Return Nothing

        End Try
    End Function

    ''' <summary>
    ''' Envía un residente a blacklist
    ''' </summary>
    Public Shared Function EnviarABlacklist(chatId As Long, datosBlacklist As Dictionary(Of String, Object), datosResidente As Dictionary(Of String, Object)) As Boolean

        Try
            ' Insertar en blacklist
            Dim resultadoBlacklist As Boolean = DynamicCrudService.Insertar(TABLA_BLACKLIST, datosBlacklist)

            If resultadoBlacklist Then
                ' Actualizar estado del residente
                DynamicCrudService.ActualizarPorCampo(TABLA_RESIDENTES, "ChatId", chatId, datosResidente)
                Return True
            End If

            Return False

        Catch ex As Exception
            Logger.LogError("ResidentesTelegramService.EnviarABlacklist", ex)
            Return False

        End Try
    End Function

    ''' <summary>
    ''' Restaura un residente desde blacklist
    ''' </summary>
    Public Shared Function RestaurarResidente(blacklistId As Integer) As Object

        Try
            ' Obtener datos del blacklist
            Dim blacklistRow As DataRow = DynamicCrudService.ObtenerPorId(TABLA_BLACKLIST, blacklistId)

            If blacklistRow Is Nothing Then
                Return New With {.success = False, .message = "Registro no encontrado"}
            End If

            Dim chatId As Long = CLng(blacklistRow("ChatId"))

            ' Eliminar de blacklist
            DynamicCrudService.Eliminar(TABLA_BLACKLIST, blacklistId)

            ' Restaurar estado del residente
            Dim datosResidente As New Dictionary(Of String, Object)

            datosResidente("EstadoResidente") = "activo"
            datosResidente("RazonBloqueo") = DBNull.Value
            datosResidente("BloqueadoPor") = DBNull.Value
            datosResidente("FechaBloqueo") = DBNull.Value

            DynamicCrudService.ActualizarPorCampo(TABLA_RESIDENTES, "ChatId", chatId, datosResidente)

            Logger.LogInfo($"Residente restaurado: ChatId={chatId}")
            Return New With {.success = True, .message = "Residente restaurado correctamente"}

        Catch ex As Exception
            Logger.LogError("ResidentesTelegramService.RestaurarResidente", ex)
            Return New With {.success = False, .message = "Error al restaurar residente"}

        End Try
    End Function

End Class
