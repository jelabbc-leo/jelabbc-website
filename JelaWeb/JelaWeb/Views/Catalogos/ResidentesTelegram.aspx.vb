Imports DevExpress.Web
Imports System.Data
Imports JelaWeb.Utilities
Imports JelaWeb.Services

Public Class ResidentesTelegram
    Inherits BasePage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load

        Try
            If Not IsPostBack Then
                CargarResidentes()
                CargarBlacklist()
            End If

        Catch ex As Exception
            Logger.LogError("ResidentesTelegram.Page_Load", ex)
            MostrarMensaje("Error al cargar la página", "error")

        End Try
    End Sub

    Private Sub CargarResidentes()

        Try
            ' Obtener datos del servicio
            Dim dt As DataTable = ResidentesTelegramService.ListarResidentes()

            gridResidentes.DataSource = dt
            gridResidentes.DataBind()

        Catch ex As Exception
            Logger.LogError("ResidentesTelegram.CargarResidentes", ex)
            MostrarMensaje("Error al cargar residentes", "error")

        End Try
    End Sub

    Private Sub CargarBlacklist()

        Try
            ' Obtener datos del servicio
            Dim dt As DataTable = ResidentesTelegramService.ListarBlacklist()

            gridBlacklist.DataSource = dt
            gridBlacklist.DataBind()

        Catch ex As Exception
            Logger.LogError("ResidentesTelegram.CargarBlacklist", ex)
            MostrarMensaje("Error al cargar blacklist", "error")

        End Try
    End Sub

    Protected Sub gridResidentes_DataBound(sender As Object, e As EventArgs) Handles gridResidentes.DataBound

        Try
            Dim tabla As DataTable = TryCast(gridResidentes.DataSource, DataTable)

            If tabla IsNot Nothing Then
                FuncionesGridWeb.SUMColumn(gridResidentes, tabla)
            End If

        Catch ex As Exception
            Logger.LogError("ResidentesTelegram.gridResidentes_DataBound", ex)

        End Try
    End Sub

    Protected Sub gridBlacklist_DataBound(sender As Object, e As EventArgs) Handles gridBlacklist.DataBound

        Try
            Dim tabla As DataTable = TryCast(gridBlacklist.DataSource, DataTable)

            If tabla IsNot Nothing Then
                FuncionesGridWeb.SUMColumn(gridBlacklist, tabla)
            End If

        Catch ex As Exception
            Logger.LogError("ResidentesTelegram.gridBlacklist_DataBound", ex)

        End Try
    End Sub

    Protected Sub btnGuardarResidente_Click(sender As Object, e As EventArgs)

        Try
            Dim residenteId As Integer = Integer.Parse(hfResidenteId.Value)
            Dim datos As New Dictionary(Of String, Object)

            datos("ChatId") = CLng(txtChatId.Number)
            datos("Username") = txtUsername.Text.Trim()
            datos("FirstName") = txtFirstName.Text.Trim()
            datos("LastName") = txtLastName.Text.Trim()
            datos("EstadoResidente") = cmbEstadoResidente.Value?.ToString()
            datos("TipoResidente") = cmbTipoResidente.Value?.ToString()
            datos("CreditosDisponibles") = CInt(txtCreditos.Number)
            datos("LimiteTicketsMes") = CInt(txtLimiteTickets.Number)
            datos("Activo") = chkActivo.Checked

            ' Guardar a través del servicio
            Dim resultado As Boolean = ResidentesTelegramService.GuardarResidente(residenteId, datos)

            If residenteId = 0 Then
                Logger.LogInfo($"Residente creado: ChatId={datos("ChatId")}")
            Else
                Logger.LogInfo($"Residente actualizado: Id={residenteId}")
            End If

            If resultado Then
                MostrarMensaje("Residente guardado correctamente", "success")
                CargarResidentes()
                ScriptManager.RegisterStartupScript(Me, Me.GetType(), "hidePopup", "popupResidente.Hide();", True)
            Else
                MostrarMensaje("Error al guardar el residente", "error")
            End If

        Catch ex As Exception
            Logger.LogError("ResidentesTelegram.btnGuardarResidente_Click", ex)
            MostrarMensaje("Error al guardar el residente", "error")

        End Try
    End Sub

    Protected Sub btnConfirmarBlacklist_Click(sender As Object, e As EventArgs)

        Try
            Dim chatId As Long = Long.Parse(hfChatIdBlacklist.Value)

            If chatId = 0 Then
                MostrarMensaje("Seleccione un residente", "warning")
                Return
            End If

            ' Obtener datos del residente desde el servicio
            Dim residente As DataRow = ResidentesTelegramService.ObtenerResidentePorChatId(chatId)

            If residente Is Nothing Then
                MostrarMensaje("Residente no encontrado", "error")
                Return
            End If

            ' Preparar datos para blacklist
            Dim datosBlacklist As New Dictionary(Of String, Object)

            datosBlacklist("ChatId") = chatId
            datosBlacklist("Username") = residente("Username")?.ToString()
            datosBlacklist("RazonBloqueo") = txtRazonBloqueo.Text.Trim()
            datosBlacklist("FechaBloqueo") = DateTime.Now
            datosBlacklist("BloqueadoPor") = SessionHelper.GetCurrentUser()?.Username
            datosBlacklist("Permanente") = chkPermanente.Checked
            datosBlacklist("NotasAdicionales") = txtNotasBlacklist.Text.Trim()

            If Not chkPermanente.Checked AndAlso dteFechaLevantamiento.Date <> DateTime.MinValue Then
                datosBlacklist("FechaLevantamiento") = dteFechaLevantamiento.Date
            End If

            ' Preparar datos para actualizar residente
            Dim datosResidente As New Dictionary(Of String, Object)

            datosResidente("EstadoResidente") = "bloqueado"
            datosResidente("RazonBloqueo") = txtRazonBloqueo.Text.Trim()
            datosResidente("BloqueadoPor") = SessionHelper.GetCurrentUser()?.Username
            datosResidente("FechaBloqueo") = DateTime.Now

            ' Enviar a blacklist a través del servicio
            Dim resultadoBlacklist As Boolean = ResidentesTelegramService.EnviarABlacklist(chatId, datosBlacklist, datosResidente)

            If resultadoBlacklist Then
                Logger.LogInfo($"Residente enviado a blacklist: ChatId={chatId}")
                MostrarMensaje("Residente enviado a blacklist correctamente", "success")
                CargarResidentes()
                CargarBlacklist()
                ScriptManager.RegisterStartupScript(Me, Me.GetType(), "hidePopup", "popupBlacklist.Hide();", True)
            Else
                MostrarMensaje("Error al enviar a blacklist", "error")
            End If

        Catch ex As Exception
            Logger.LogError("ResidentesTelegram.btnConfirmarBlacklist_Click", ex)
            MostrarMensaje("Error al enviar a blacklist", "error")

        End Try
    End Sub

    ' Método para restaurar residente desde blacklist (llamado desde JavaScript)
    <System.Web.Services.WebMethod()>
    Public Shared Function RestaurarResidente(blacklistId As Integer) As Object
        Return ResidentesTelegramService.RestaurarResidente(blacklistId)
    End Function

    Private Sub MostrarMensaje(mensaje As String, tipo As String)
        ScriptManager.RegisterStartupScript(Me, Me.GetType(), "showAlert", $"toastr.{tipo}('{mensaje}');", True)
    End Sub

End Class
