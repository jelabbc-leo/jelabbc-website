Imports DevExpress.Web
Imports System.Data
Imports System.Collections.Generic
Imports System.Configuration
Imports System.Text
Imports System.Web
Imports JelaWeb.Business.Operacion
Imports JelaWeb.Utilities

Public Class Tickets
    Inherits BasePage

    Private apiConsumer As Global.JelaWeb.ApiConsumer
    Private apiConsumerCRUD As Global.JelaWeb.ApiConsumerCRUD
    Private apiBaseUrl As String
    Private apiPostUrl As String
    Private ticketsBusiness As TicketsBusiness

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        apiBaseUrl = ConfigurationManager.AppSettings("ApiBaseUrl")
        apiPostUrl = ConfigurationManager.AppSettings("APIPost")
        apiConsumer = New Global.JelaWeb.ApiConsumer()
        apiConsumerCRUD = New Global.JelaWeb.ApiConsumerCRUD()
        ticketsBusiness = New TicketsBusiness()

        ' Manejar acciones desde JavaScript
        Dim eventTarget As String = Request("__EVENTTARGET")
        Dim eventArgument As String = Request("__EVENTARGUMENT")

        If Not String.IsNullOrWhiteSpace(eventTarget) Then
            Select Case eventTarget

                Case "NUEVO_TICKET"
                    MostrarPopupNuevo()

                Case "VER_TICKET"
                    If Not String.IsNullOrWhiteSpace(eventArgument) Then
                        Dim idTicket As Integer = Convert.ToInt32(eventArgument)

                        CargarDatosTicket(idTicket)
                        popupTicket.ShowOnPageLoad = True
                    End If

                Case "RESOLVER_IA"
                    If Not String.IsNullOrWhiteSpace(eventArgument) Then
                        Dim idTicket As Integer = Convert.ToInt32(eventArgument)

                        ResolverTicketConIA(idTicket)
                        CargarDatosTicket(idTicket)
                        popupTicket.ShowOnPageLoad = True
                    End If

                Case "CargarTicket"
                    If Not String.IsNullOrWhiteSpace(eventArgument) Then
                        Dim idTicket As Integer = Convert.ToInt32(eventArgument)

                        CargarDatosTicket(idTicket)
                        popupTicket.ShowOnPageLoad = True
                    End If

                Case "ResolverIA"
                    If Not String.IsNullOrWhiteSpace(eventArgument) Then
                        Dim idTicket As Integer = Convert.ToInt32(eventArgument)

                        ResolverTicketConIA(idTicket)
                        CargarDatosTicket(idTicket)
                        popupTicket.ShowOnPageLoad = True
                    End If

            End Select
        End If

        If Not IsPostBack Then
            ' Cargar combos
            CargarCombos()

            ' Establecer fechas por defecto (últimos 90 días para asegurar que se muestren todos los registros)
            If dteFechaDesde.Value Is Nothing Then
                dteFechaDesde.Value = DateTime.Now.AddDays(-90)
            End If
            If dteFechaHasta.Value Is Nothing Then
                dteFechaHasta.Value = DateTime.Now
            End If

            ' Cargar tickets con las fechas por defecto
            CargarTickets()
        Else
            ' En postback, restaurar DataSource desde Session para que filtros y agrupaciones funcionen
            Dim dt As DataTable = TryCast(Session("dtTickets"), DataTable)

            If dt IsNot Nothing AndAlso gridTickets.DataSource Is Nothing Then
                gridTickets.DataSource = dt
                gridTickets.DataBind()
            End If
        End If
    End Sub

    Private Sub CargarCombos()

        Try
            ' Cargar agentes (usuarios activos)
            Dim queryAgentes As String = "SELECT Id, Nombre FROM conf_usuarios WHERE Activo = 1 ORDER BY Nombre"
            Dim urlAgentes As String = apiBaseUrl & System.Web.HttpUtility.UrlEncode(queryAgentes)
            Dim datosAgentes = apiConsumer.ObtenerDatos(urlAgentes)
            Dim dtAgentes = apiConsumer.ConvertirADatatable(datosAgentes)

            cmbAgenteAsignado.Items.Clear()
            cmbAgenteAsignado.Items.Add("Sin asignar", Nothing)
            cmbAgenteAsignado.DataSource = dtAgentes
            cmbAgenteAsignado.ValueField = "Id"
            cmbAgenteAsignado.TextField = "Nombre"
            cmbAgenteAsignado.DataBind()

        Catch ex As Exception
            Logger.LogError("Error al cargar combos: " & ex.Message, ex, "")

        End Try
    End Sub

    Protected Sub btnFiltrar_Click(sender As Object, e As EventArgs)

        Try
            CargarTickets()

        Catch ex As Exception
            Logger.LogError("Error al filtrar tickets: " & ex.Message, ex, "")
            MostrarMensajeError("Error al cargar los datos: " & ex.Message)

        End Try
    End Sub

    Protected Sub btnLimpiar_Click(sender As Object, e As EventArgs)
        dteFechaDesde.Value = Nothing
        dteFechaHasta.Value = Nothing
        Session("dtTickets") = Nothing
        gridTickets.DataSource = Nothing
        gridTickets.DataBind()
    End Sub

    Private Sub CargarTickets()

        Try
            Dim query As New StringBuilder()

            query.Append("SELECT t.Id, t.AsuntoCorto, t.NombreCompleto, t.Canal, t.Estado, ")
            query.Append("t.PrioridadAsignada, t.SentimientoDetectado, t.CategoriaAsignada, ")
            query.Append("t.EmailCliente, t.FechaCreacion ")
            query.Append("FROM op_tickets_v2 t WHERE 1=1 ")

            ' Filtro SOLO por fechas usando BETWEEN
            If dteFechaDesde.Value IsNot Nothing AndAlso dteFechaHasta.Value IsNot Nothing Then
                ' Ambas fechas están establecidas - usar BETWEEN
                Dim fechaDesde As DateTime = Convert.ToDateTime(dteFechaDesde.Value)

                fechaDesde = fechaDesde.Date ' Inicio del día (00:00:00)

                Dim fechaHasta As DateTime = Convert.ToDateTime(dteFechaHasta.Value)

                fechaHasta = fechaHasta.Date.AddDays(1).AddSeconds(-1) ' Fin del día (23:59:59)

                query.Append("AND t.FechaCreacion BETWEEN " & QueryBuilder.EscapeSqlDate(fechaDesde) &
                            " AND " & QueryBuilder.EscapeSqlDate(fechaHasta) & " ")
            ElseIf dteFechaDesde.Value IsNot Nothing Then
                ' Solo fecha desde está establecida
                Dim fechaDesde As DateTime = Convert.ToDateTime(dteFechaDesde.Value)

                fechaDesde = fechaDesde.Date
                query.Append("AND t.FechaCreacion >= " & QueryBuilder.EscapeSqlDate(fechaDesde) & " ")
            ElseIf dteFechaHasta.Value IsNot Nothing Then
                ' Solo fecha hasta está establecida
                Dim fechaHasta As DateTime = Convert.ToDateTime(dteFechaHasta.Value)

                fechaHasta = fechaHasta.Date.AddDays(1).AddSeconds(-1)
                query.Append("AND t.FechaCreacion <= " & QueryBuilder.EscapeSqlDate(fechaHasta) & " ")
            End If

            query.Append(" ORDER BY t.FechaCreacion DESC")

            Dim url As String = apiBaseUrl & System.Web.HttpUtility.UrlEncode(query.ToString())
            Dim datos = apiConsumer.ObtenerDatos(url)
            Dim dt As DataTable = apiConsumer.ConvertirADatatable(datos)

            ' Guardar DataTable en Session para que los filtros y agrupaciones funcionen del lado del cliente
            Session("dtTickets") = dt

            gridTickets.DataSource = dt
            gridTickets.DataBind()

        Catch ex As Exception
            Logger.LogError("Error al cargar tickets: " & ex.Message, ex, "")
            Throw

        End Try
    End Sub

    ''' <summary>
    ''' Evento DataBound para aplicar FuncionesGridWeb
    ''' </summary>
    Protected Sub gridTickets_DataBound(sender As Object, e As EventArgs) Handles gridTickets.DataBound

        Try
            ' Obtener DataTable de Session si está disponible, sino del DataSource
            Dim tabla As DataTable = TryCast(Session("dtTickets"), DataTable)

            If tabla Is Nothing Then
                tabla = TryCast(gridTickets.DataSource, DataTable)
            End If

            If tabla IsNot Nothing Then
                FuncionesGridWeb.SUMColumn(gridTickets, tabla)
            End If

        Catch ex As Exception
            Logger.LogError("Error al aplicar FuncionesGridWeb en gridTickets: " & ex.Message, ex, "")

        End Try
    End Sub

    Private Sub MostrarPopupNuevo()

        Try
            ' Limpiar formulario
            txtAsuntoCorto.Text = String.Empty
            txtResumenIA.Text = String.Empty
            txtCategoriaAsignada.Text = String.Empty
            txtSubcategoriaAsignada.Text = String.Empty
            txtSentimientoDetectado.Text = String.Empty
            txtPrioridadAsignada.Text = String.Empty
            txtUrgenciaAsignada.Text = String.Empty
            ' El checkbox siempre será True cuando se cree un ticket
            chkPuedeResolverIA.Checked = True
            txtRespuestaIA.Text = String.Empty

            cmbCanal.Value = Nothing
            cmbEstado.Value = "Abierto"
            txtNombreCompleto.Text = String.Empty
            txtEmailCliente.Text = String.Empty
            txtTelefonoCliente.Text = String.Empty
            cmbAgenteAsignado.Value = Nothing

            txtMensajeOriginal.Text = String.Empty

            dtFechaResolucion.Value = Nothing
            txtTiempoResolucion.Value = Nothing
            txtSatisfaccionCliente.Value = Nothing
            txtComentarioSatisfaccion.Text = String.Empty

            txtNuevoMensaje.Text = String.Empty

            ' Limpiar ID del ticket
            hfIdTicket("IdTicket") = "0"

            ' Configurar popup
            popupTicket.HeaderText = "Nuevo Ticket"
            popupTicket.ShowOnPageLoad = True

        Catch ex As Exception
            Logger.LogError("Error al mostrar popup nuevo ticket: " & ex.Message, ex, "")
            Throw

        End Try
    End Sub

    Private Sub CargarDatosTicket(idTicket As Integer)

        Try
            hfIdTicket("IdTicket") = idTicket.ToString()

            Dim query As String = "SELECT * FROM op_tickets_v2 WHERE Id = " & idTicket
            Dim url As String = apiBaseUrl & System.Web.HttpUtility.UrlEncode(query)
            Dim datos = apiConsumer.ObtenerDatos(url)

            If datos IsNot Nothing AndAlso datos.Count > 0 Then
                Dim ticket = datos(0)

                ' Tab Resumen IA
                txtAsuntoCorto.Text = If(ticket("AsuntoCorto") IsNot Nothing, ticket("AsuntoCorto").ToString(), String.Empty)
                txtResumenIA.Text = If(ticket("ResumenIA") IsNot Nothing, ticket("ResumenIA").ToString(), String.Empty)
                txtCategoriaAsignada.Text = If(ticket("CategoriaAsignada") IsNot Nothing, ticket("CategoriaAsignada").ToString(), String.Empty)
                txtSubcategoriaAsignada.Text = If(ticket("SubcategoriaAsignada") IsNot Nothing, ticket("SubcategoriaAsignada").ToString(), String.Empty)
                txtSentimientoDetectado.Text = If(ticket("SentimientoDetectado") IsNot Nothing, ticket("SentimientoDetectado").ToString(), String.Empty)
                txtPrioridadAsignada.Text = If(ticket("PrioridadAsignada") IsNot Nothing, ticket("PrioridadAsignada").ToString(), String.Empty)
                txtUrgenciaAsignada.Text = If(ticket("UrgenciaAsignada") IsNot Nothing, ticket("UrgenciaAsignada").ToString(), String.Empty)
                ' SIEMPRE mostrar el checkbox en True (ya que siempre se procesa con IA)
                chkPuedeResolverIA.Checked = True
                txtRespuestaIA.Text = If(ticket("RespuestaIA") IsNot Nothing, ticket("RespuestaIA").ToString(), String.Empty)

                ' Tab Cliente
                cmbCanal.Value = If(ticket("Canal") IsNot Nothing, ticket("Canal").ToString(), Nothing)
                cmbEstado.Value = If(ticket("Estado") IsNot Nothing, ticket("Estado").ToString(), "Abierto")
                txtNombreCompleto.Text = If(ticket("NombreCompleto") IsNot Nothing, ticket("NombreCompleto").ToString(), String.Empty)
                txtEmailCliente.Text = If(ticket("EmailCliente") IsNot Nothing, ticket("EmailCliente").ToString(), String.Empty)
                txtTelefonoCliente.Text = If(ticket("TelefonoCliente") IsNot Nothing, ticket("TelefonoCliente").ToString(), String.Empty)

                If ticket("IdAgenteAsignado") IsNot Nothing Then
                    cmbAgenteAsignado.Value = Convert.ToInt32(ticket("IdAgenteAsignado"))
                Else
                    cmbAgenteAsignado.Value = Nothing
                End If

                ' Tab Mensaje Original
                txtMensajeOriginal.Text = If(ticket("MensajeOriginal") IsNot Nothing, ticket("MensajeOriginal").ToString(), String.Empty)

                ' Tab Resolución
                If ticket("FechaResolucion") IsNot Nothing Then
                    dtFechaResolucion.Value = Convert.ToDateTime(ticket("FechaResolucion"))
                End If

                If ticket("TiempoResolucionMinutos") IsNot Nothing Then
                    txtTiempoResolucion.Value = Convert.ToInt32(ticket("TiempoResolucionMinutos"))
                End If

                If ticket("SatisfaccionCliente") IsNot Nothing Then
                    txtSatisfaccionCliente.Value = Convert.ToInt32(ticket("SatisfaccionCliente"))
                End If

                txtComentarioSatisfaccion.Text = If(ticket("ComentarioSatisfaccion") IsNot Nothing, ticket("ComentarioSatisfaccion").ToString(), String.Empty)

                ' Cargar conversación
                CargarConversacion(idTicket)
            End If

        Catch ex As Exception
            Logger.LogError("Error al cargar datos del ticket: " & ex.Message, ex, "")
            Throw

        End Try
    End Sub

    Private Sub CargarConversacion(idTicket As Integer)

        Try
            Dim query As String = "SELECT * FROM op_ticket_conversacion WHERE IdTicket = " & idTicket & " ORDER BY FechaEnvio ASC"
            Dim url As String = apiBaseUrl & System.Web.HttpUtility.UrlEncode(query)
            Dim datos = apiConsumer.ObtenerDatos(url)
            Dim dt As DataTable = apiConsumer.ConvertirADatatable(datos)

            gridConversacion.DataSource = dt
            gridConversacion.DataBind()

        Catch ex As Exception
            Logger.LogError("Error al cargar conversación: " & ex.Message, ex, "")

        End Try
    End Sub

    Protected Sub gridConversacion_CustomCallback(sender As Object, e As ASPxGridViewCustomCallbackEventArgs)
        ' Callback para actualizar conversación si es necesario
    End Sub

    Protected Sub btnGuardar_Click(sender As Object, e As EventArgs)

        Try
            ' Validar campos requeridos
            If String.IsNullOrWhiteSpace(txtMensajeOriginal.Text) Then
                MostrarMensajeError("El mensaje original es requerido")
                Return
            End If

            If cmbCanal.Value Is Nothing Then
                MostrarMensajeError("Debe seleccionar un canal")
                Return
            End If

            If String.IsNullOrWhiteSpace(txtNombreCompleto.Text) Then
                MostrarMensajeError("El nombre completo es requerido")
                Return
            End If

            Dim idTicketValue As String = If(hfIdTicket("IdTicket") IsNot Nothing, hfIdTicket("IdTicket").ToString(), String.Empty)
            Dim esEdicion As Boolean = Not String.IsNullOrWhiteSpace(idTicketValue) AndAlso idTicketValue <> "0"

            If esEdicion Then
                ' Actualizar ticket existente
                ActualizarTicket()
            Else
                ' Crear nuevo ticket con procesamiento IA
                CrearNuevoTicket()
            End If

        Catch ex As Exception
            Logger.LogError("Error al guardar ticket: " & ex.Message, ex, "")
            MostrarMensajeError("Error al guardar: " & ex.Message)

        End Try
    End Sub

    Private Sub CrearNuevoTicket()

        Try
            ' Procesar ticket con IA
            Dim resultadoIA = ticketsBusiness.ProcesarTicketConIA(txtMensajeOriginal.Text)

            ' Crear DTO para el ticket
            Dim dto As New DynamicDto()

            dto("AsuntoCorto") = If(resultadoIA.ContainsKey("AsuntoCorto") AndAlso resultadoIA("AsuntoCorto") IsNot Nothing, resultadoIA("AsuntoCorto").ToString(), "Sin asunto")
            dto("MensajeOriginal") = txtMensajeOriginal.Text
            dto("ResumenIA") = If(resultadoIA.ContainsKey("Resumen") AndAlso resultadoIA("Resumen") IsNot Nothing, resultadoIA("Resumen").ToString(), String.Empty)
            dto("Canal") = If(cmbCanal.Value IsNot Nothing, cmbCanal.Value.ToString(), String.Empty)
            dto("NombreCompleto") = txtNombreCompleto.Text
            dto("EmailCliente") = If(Not String.IsNullOrWhiteSpace(txtEmailCliente.Text), txtEmailCliente.Text, Nothing)
            dto("TelefonoCliente") = If(Not String.IsNullOrWhiteSpace(txtTelefonoCliente.Text), txtTelefonoCliente.Text, Nothing)
            dto("CategoriaAsignada") = If(resultadoIA.ContainsKey("Categoria") AndAlso resultadoIA("Categoria") IsNot Nothing, resultadoIA("Categoria").ToString(), Nothing)
            dto("SubcategoriaAsignada") = If(resultadoIA.ContainsKey("Subcategoria") AndAlso resultadoIA("Subcategoria") IsNot Nothing, resultadoIA("Subcategoria").ToString(), Nothing)
            dto("SentimientoDetectado") = If(resultadoIA.ContainsKey("Sentimiento") AndAlso resultadoIA("Sentimiento") IsNot Nothing, resultadoIA("Sentimiento").ToString(), "Neutral")
            dto("PrioridadAsignada") = If(resultadoIA.ContainsKey("Prioridad") AndAlso resultadoIA("Prioridad") IsNot Nothing, resultadoIA("Prioridad").ToString(), "Media")
            dto("UrgenciaAsignada") = If(resultadoIA.ContainsKey("Urgencia") AndAlso resultadoIA("Urgencia") IsNot Nothing, resultadoIA("Urgencia").ToString(), "Media")
            ' SIEMPRE establecer PuedeResolverIA en True para que siempre se procese con IA
            dto("PuedeResolverIA") = True
            dto("Estado") = "Abierto"
            dto("IdUsuarioCreacion") = If(SessionHelper.GetUserId().HasValue, SessionHelper.GetUserId().Value, 1)
            dto("FechaCreacion") = DateTime.Now

            ' SIEMPRE generar respuesta automática con IA
            Dim respuestaIA = ticketsBusiness.ResolverTicketConIA(0, txtMensajeOriginal.Text)

            dto("RespuestaIA") = If(respuestaIA.ContainsKey("Respuesta") AndAlso respuestaIA("Respuesta") IsNot Nothing, respuestaIA("Respuesta").ToString(), String.Empty)
            dto("Estado") = "Resuelto"
            dto("FechaResolucion") = DateTime.Now

            ' Asignar agente si se seleccionó
            If cmbAgenteAsignado.Value IsNot Nothing Then
                dto("IdAgenteAsignado") = Convert.ToInt32(cmbAgenteAsignado.Value)
                dto("FechaAsignacionAgente") = DateTime.Now
                dto("Estado") = "EnProceso"
            End If

            ' Guardar ticket
            Dim urlPost As String = apiPostUrl & "op_tickets_v2"
            Dim nuevoId As Integer = apiConsumerCRUD.EnviarPostId(urlPost, dto)

            ' Registrar acción de creación
            RegistrarAccion(nuevoId, "Creacion", "Ticket creado", Nothing, Nothing, False)

            ' Agregar mensaje inicial a la conversación
            AgregarMensajeConversacion(nuevoId, "Cliente", txtMensajeOriginal.Text, False, Nothing, txtNombreCompleto.Text)

            ' SIEMPRE agregar respuesta de la IA a la conversación (ya que siempre se procesa con IA)
            Dim respuestaIAText As String = If(dto("RespuestaIA") IsNot Nothing, dto("RespuestaIA").ToString(), String.Empty)

            If Not String.IsNullOrWhiteSpace(respuestaIAText) Then
                AgregarMensajeConversacion(nuevoId, "IA", respuestaIAText, True, Nothing, "Asistente IA")
                RegistrarAccion(nuevoId, "Resolucion", "Ticket resuelto automáticamente por IA", Nothing, "Resuelto", True)
            End If

            ' Cargar los datos del ticket recién creado para mostrar la respuesta de la IA
            CargarDatosTicket(nuevoId)

            ' Cambiar el título del popup a "Ver Ticket"
            popupTicket.HeaderText = "Ticket #" & nuevoId.ToString()

            ' Mantener el popup abierto para que el usuario vea la respuesta de la IA
            popupTicket.ShowOnPageLoad = True

            MostrarMensajeExito("Ticket creado correctamente. La respuesta de la IA está disponible en la pestaña 'Resumen IA'.")
            CargarTickets()

        Catch ex As Exception
            Logger.LogError("Error al crear ticket: " & ex.Message, ex, "")
            Throw

        End Try
    End Sub

    Private Sub ActualizarTicket()

        Try
            Dim idTicket As Integer = Convert.ToInt32(hfIdTicket("IdTicket"))
            Dim dto As New DynamicDto()

            dto("Id") = idTicket
            dto("Canal") = If(cmbCanal.Value IsNot Nothing, cmbCanal.Value.ToString(), String.Empty)
            dto("Estado") = If(cmbEstado.Value IsNot Nothing, cmbEstado.Value.ToString(), String.Empty)
            dto("NombreCompleto") = txtNombreCompleto.Text
            dto("EmailCliente") = If(Not String.IsNullOrWhiteSpace(txtEmailCliente.Text), txtEmailCliente.Text, Nothing)
            dto("TelefonoCliente") = If(Not String.IsNullOrWhiteSpace(txtTelefonoCliente.Text), txtTelefonoCliente.Text, Nothing)

            If cmbAgenteAsignado.Value IsNot Nothing Then
                dto("IdAgenteAsignado") = Convert.ToInt32(cmbAgenteAsignado.Value)
                If dto("FechaAsignacionAgente") Is Nothing Then
                    dto("FechaAsignacionAgente") = DateTime.Now
                End If
            Else
                dto("IdAgenteAsignado") = Nothing
                dto("FechaAsignacionAgente") = Nothing
            End If

            If dtFechaResolucion.Value IsNot Nothing Then
                dto("FechaResolucion") = dtFechaResolucion.Value
                If txtTiempoResolucion.Value Is Nothing Then
                    ' Calcular tiempo de resolución
                    Dim queryTicket As String = "SELECT FechaCreacion FROM op_tickets_v2 WHERE Id = " & idTicket
                    Dim urlTicket As String = apiBaseUrl & System.Web.HttpUtility.UrlEncode(queryTicket)
                    Dim datosTicket = apiConsumer.ObtenerDatos(urlTicket)

                    If datosTicket IsNot Nothing AndAlso datosTicket.Count > 0 Then
                        Dim fechaCreacion As DateTime = Convert.ToDateTime(datosTicket(0)("FechaCreacion"))
                        Dim tiempoResolucion As TimeSpan = Convert.ToDateTime(dtFechaResolucion.Value) - fechaCreacion

                        dto("TiempoResolucionMinutos") = CInt(tiempoResolucion.TotalMinutes)
                    End If
                End If
            End If

            If txtSatisfaccionCliente.Value IsNot Nothing Then
                dto("SatisfaccionCliente") = Convert.ToInt32(txtSatisfaccionCliente.Value)
            End If

            dto("ComentarioSatisfaccion") = If(Not String.IsNullOrWhiteSpace(txtComentarioSatisfaccion.Text), txtComentarioSatisfaccion.Text, Nothing)

            Dim urlPut As String = apiPostUrl & "op_tickets_v2"

            apiConsumerCRUD.EnviarPut(urlPut, dto)

            ' Registrar acción de actualización
            RegistrarAccion(idTicket, "Actualizacion", "Ticket actualizado", Nothing, Nothing, False)

            MostrarMensajeExito("Ticket actualizado correctamente")
            popupTicket.ShowOnPageLoad = False
            CargarTickets()

        Catch ex As Exception
            Logger.LogError("Error al actualizar ticket: " & ex.Message, ex, "")
            Throw

        End Try
    End Sub

    Private Sub ResolverTicketConIA(idTicket As Integer)

        Try
            ' Obtener mensaje original del ticket
            Dim query As String = "SELECT MensajeOriginal FROM op_tickets_v2 WHERE Id = " & idTicket
            Dim url As String = apiBaseUrl & System.Web.HttpUtility.UrlEncode(query)
            Dim datos = apiConsumer.ObtenerDatos(url)

            If datos IsNot Nothing AndAlso datos.Count > 0 Then
                Dim mensajeOriginal As String = datos(0)("MensajeOriginal").ToString()

                ' Resolver con IA
                Dim resultadoIA = ticketsBusiness.ResolverTicketConIA(idTicket, mensajeOriginal)

                If resultadoIA.ContainsKey("Respuesta") Then
                    Dim respuestaIA As String = resultadoIA("Respuesta").ToString()

                    ' Actualizar ticket
                    Dim dto As New DynamicDto()

                    dto("Id") = idTicket
                    dto("RespuestaIA") = respuestaIA
                    dto("Estado") = "Resuelto"
                    dto("FechaResolucion") = DateTime.Now

                    ' Calcular tiempo de resolución
                    Dim queryTicket As String = "SELECT FechaCreacion FROM op_tickets_v2 WHERE Id = " & idTicket
                    Dim urlTicket As String = apiBaseUrl & System.Web.HttpUtility.UrlEncode(queryTicket)
                    Dim datosTicket = apiConsumer.ObtenerDatos(urlTicket)

                    If datosTicket IsNot Nothing AndAlso datosTicket.Count > 0 Then
                        Dim fechaCreacion As DateTime = Convert.ToDateTime(datosTicket(0)("FechaCreacion"))
                        Dim tiempoResolucion As TimeSpan = DateTime.Now - fechaCreacion

                        dto("TiempoResolucionMinutos") = CInt(tiempoResolucion.TotalMinutes)
                    End If

                    Dim urlPut As String = apiPostUrl & "op_tickets_v2"

                    apiConsumerCRUD.EnviarPut(urlPut, dto)

                    ' Agregar respuesta a la conversación
                    AgregarMensajeConversacion(idTicket, "IA", respuestaIA, True, Nothing, "Asistente IA")

                    ' Registrar acción
                    RegistrarAccion(idTicket, "Resolucion", "Ticket resuelto automáticamente por IA", Nothing, "Resuelto", True)

                    MostrarMensajeExito("Ticket resuelto correctamente por IA")
                    CargarDatosTicket(idTicket)
                Else
                    MostrarMensajeError("No se pudo generar una respuesta automática")
                End If
            End If

        Catch ex As Exception
            Logger.LogError("Error al resolver ticket con IA: " & ex.Message, ex, "")
            MostrarMensajeError("Error al resolver: " & ex.Message)

        End Try
    End Sub

    Protected Sub btnEnviarMensaje_Click(sender As Object, e As EventArgs)

        Try
            Dim idTicketValue As String = If(hfIdTicket("IdTicket") IsNot Nothing, hfIdTicket("IdTicket").ToString(), String.Empty)

            If String.IsNullOrWhiteSpace(idTicketValue) OrElse idTicketValue = "0" Then
                MostrarMensajeError("No hay un ticket seleccionado")
                Return
            End If

            If String.IsNullOrWhiteSpace(txtNuevoMensaje.Text) Then
                MostrarMensajeError("Debe escribir un mensaje")
                Return
            End If

            Dim idTicket As Integer = Convert.ToInt32(idTicketValue)
            Dim idUsuario As Integer? = SessionHelper.GetUserId()
            Dim nombreUsuario As String = SessionHelper.GetNombre()

            AgregarMensajeConversacion(idTicket, "Agente", txtNuevoMensaje.Text, False, idUsuario, nombreUsuario)

            ' Limpiar campo
            txtNuevoMensaje.Text = String.Empty

            ' Recargar conversación
            CargarConversacion(idTicket)

            MostrarMensajeExito("Mensaje enviado correctamente")

        Catch ex As Exception
            Logger.LogError("Error al enviar mensaje: " & ex.Message, ex, "")
            MostrarMensajeError("Error al enviar mensaje: " & ex.Message)

        End Try
    End Sub

    Private Sub AgregarMensajeConversacion(idTicket As Integer, tipoMensaje As String, mensaje As String, esRespuestaIA As Boolean, idUsuario As Integer?, nombreUsuario As String)

        Try
            Dim dto As New DynamicDto()

            dto("IdTicket") = idTicket
            dto("TipoMensaje") = tipoMensaje
            dto("Mensaje") = mensaje
            dto("EsRespuestaIA") = esRespuestaIA
            dto("IdUsuarioEnvio") = If(idUsuario.HasValue, idUsuario.Value, Nothing)
            dto("NombreUsuarioEnvio") = If(Not String.IsNullOrWhiteSpace(nombreUsuario), nombreUsuario, "Sistema")
            dto("FechaEnvio") = DateTime.Now
            dto("Leido") = False

            Dim urlPost As String = apiPostUrl & "op_ticket_conversacion"

            apiConsumerCRUD.EnviarPost(urlPost, dto)

        Catch ex As Exception
            Logger.LogError("Error al agregar mensaje a conversación: " & ex.Message, ex, "")
            Throw

        End Try
    End Sub

    Private Sub RegistrarAccion(idTicket As Integer, tipoAccion As String, descripcion As String, valorAnterior As String, valorNuevo As String, esAccionIA As Boolean)

        Try
            Dim dto As New DynamicDto()

            dto("IdTicket") = idTicket
            dto("TipoAccion") = tipoAccion
            dto("Descripcion") = descripcion
            dto("ValorAnterior") = valorAnterior
            dto("ValorNuevo") = valorNuevo
            dto("EsAccionIA") = esAccionIA
            dto("IdUsuarioAccion") = If(SessionHelper.GetUserId().HasValue, SessionHelper.GetUserId().Value, Nothing)
            dto("FechaAccion") = DateTime.Now

            Dim urlPost As String = apiPostUrl & "op_ticket_acciones"

            apiConsumerCRUD.EnviarPost(urlPost, dto)

        Catch ex As Exception
            Logger.LogError("Error al registrar acción: " & ex.Message, ex, "")
            ' No lanzar excepción, solo registrar el error

        End Try
    End Sub

    Private Sub MostrarMensajeExito(mensaje As String)
        lblMensaje.Text = mensaje
        lblMensaje.ForeColor = System.Drawing.Color.Green
        lblMensaje.Visible = True
        ClientScript.RegisterStartupScript(Me.GetType(), "MensajeExito", "alert('" & mensaje.Replace("'", "\'") & "');", True)
    End Sub

    Private Sub MostrarMensajeError(mensaje As String)
        lblMensaje.Text = mensaje
        lblMensaje.ForeColor = System.Drawing.Color.Red
        lblMensaje.Visible = True
        ClientScript.RegisterStartupScript(Me.GetType(), "MensajeError", "alert('" & mensaje.Replace("'", "\'") & "');", True)
    End Sub

End Class
