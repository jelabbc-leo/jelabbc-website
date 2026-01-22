Imports Newtonsoft.Json.Linq

Public Class SelectorEntidades
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            ' Verificar autenticación
            If Not SessionHelper.IsAuthenticated() Then
                Response.Redirect(Constants.ROUTE_LOGIN, True)
                Return
            End If

            ' Verificar que sea administrador de condominios
            If Not SessionHelper.IsAdministradorCondominios() Then
                ' Si no es administrador, redirigir a inicio (ya debe tener entidad asignada)
                Response.Redirect(Constants.ROUTE_INICIO, True)
                Return
            End If

            If Not IsPostBack Then
                ' Cargar nombre del usuario
                lblNombreUsuario.Text = SessionHelper.GetNombre()

                ' Cargar entidades disponibles
                CargarEntidades()

                ' Mostrar licencias disponibles
                MostrarLicenciasDisponibles()

                ' Si viene de Entidades.aspx después de crear una nueva entidad
                If Request.QueryString("nueva") = "1" Then
                    MostrarMensaje("Condominio creado exitosamente.", "success")
                    ' Recargar entidades para mostrar la nueva
                    CargarEntidades()
                End If
            End If

        Catch ex As Exception
            Logger.LogError("SelectorEntidades.Page_Load", ex)
            MostrarMensaje("Error al cargar la página: " & ex.Message, "danger")
        End Try
    End Sub

    ''' <summary>
    ''' Carga las entidades asignadas al usuario desde la sesión
    ''' </summary>
    Private Sub CargarEntidades()
        Try
            Dim entidades = SessionHelper.GetEntidades()

            If entidades Is Nothing OrElse entidades.Count = 0 Then
                MostrarMensaje("No tiene entidades asignadas. Contacte al administrador.", "warning")
                rptEntidades.Visible = False
                Return
            End If

            ' Convertir JArray a List para el Repeater
            Dim listaEntidades As New List(Of Object)

            For Each entidad As JObject In entidades
                listaEntidades.Add(New With {
                    .Id = entidad("Id").ToObject(Of Integer)(),
                    .Nombre = entidad("Nombre").ToString(),
                    .Direccion = If(entidad("Direccion") IsNot Nothing, entidad("Direccion").ToString(), ""),
                    .Logotipo = If(entidad("Logotipo") IsNot Nothing, entidad("Logotipo").ToString(), ""),
                    .EsPrincipal = If(entidad("EsPrincipal") IsNot Nothing, entidad("EsPrincipal").ToObject(Of Boolean)(), False)
                })
            Next

            rptEntidades.DataSource = listaEntidades
            rptEntidades.DataBind()

        Catch ex As Exception
            Logger.LogError("SelectorEntidades.CargarEntidades", ex)
            MostrarMensaje("Error al cargar entidades: " & ex.Message, "danger")
        End Try
    End Sub

    ''' <summary>
    ''' Muestra el número de licencias disponibles y habilita/deshabilita el botón
    ''' </summary>
    Private Sub MostrarLicenciasDisponibles()
        Try
            Dim licencias = SessionHelper.GetLicenciasDisponibles()

            lblLicencias.Text = $"Licencias disponibles: {licencias}"

            ' Habilitar/deshabilitar botón según licencias
            ' NO modificar las clases CSS - mantener btn-floating btn-floating-add
            If licencias = 0 Then
                btnAgregarEntidad.Attributes("title") = "No tiene licencias disponibles. Contacte al administrador del sistema."
                btnAgregarEntidad.Attributes("class") = "btn-floating btn-floating-add disabled"
                btnAgregarEntidad.Attributes("style") = "opacity: 0.5; cursor: not-allowed;"
            Else
                btnAgregarEntidad.Attributes("title") = "Agregar un nuevo condominio"
                btnAgregarEntidad.Attributes("class") = "btn-floating btn-floating-add"
                btnAgregarEntidad.Attributes("style") = ""
            End If

        Catch ex As Exception
            Logger.LogError("SelectorEntidades.MostrarLicenciasDisponibles", ex)
        End Try
    End Sub

    ''' <summary>
    ''' Maneja el evento de selección de entidad
    ''' </summary>
    Protected Sub rptEntidades_ItemCommand(source As Object, e As RepeaterCommandEventArgs)
        Try
            If e.CommandName = "Seleccionar" Then
                Dim idEntidad As Integer = Convert.ToInt32(e.CommandArgument)

                ' Buscar el nombre de la entidad seleccionada
                Dim entidades = SessionHelper.GetEntidades()
                Dim nombreEntidad As String = ""

                For Each entidad As JObject In entidades
                    If entidad("Id").ToObject(Of Integer)() = idEntidad Then
                        nombreEntidad = entidad("Nombre").ToString()
                        Exit For
                    End If
                Next

                ' Establecer la entidad actual en la sesión
                SessionHelper.SetEntidadActual(idEntidad, nombreEntidad)

                ' Log de auditoría
                Logger.LogInfo($"Usuario {SessionHelper.GetNombre()} seleccionó entidad: {nombreEntidad} (ID: {idEntidad})")

                ' Redirigir a la página de inicio
                Response.Redirect(Constants.ROUTE_INICIO, False)
            End If

        Catch ex As Exception
            Logger.LogError("SelectorEntidades.rptEntidades_ItemCommand", ex)
            MostrarMensaje("Error al seleccionar entidad: " & ex.Message, "danger")
        End Try
    End Sub

    ''' <summary>
    ''' Maneja el clic en el botón de agregar nueva entidad
    ''' </summary>
    Protected Sub btnAgregarEntidad_Click(sender As Object, e As EventArgs)
        Try
            ' Validar que tenga licencias disponibles
            If Not SessionHelper.TieneLicenciasDisponibles() Then
                MostrarMensaje("No tiene licencias disponibles. Contacte al administrador del sistema.", "warning")
                Return
            End If

            ' Redirigir a la página de catálogo de entidades con parámetros
            Response.Redirect("~/Views/Catalogos/Entidades.aspx?modo=nuevo&origen=selector", False)

        Catch ex As Exception
            Logger.LogError("SelectorEntidades.btnAgregarEntidad_Click", ex)
            MostrarMensaje("Error al intentar agregar entidad: " & ex.Message, "danger")
        End Try
    End Sub

    ''' <summary>
    ''' Maneja el clic en el botón de cerrar sesión
    ''' </summary>
    Protected Sub btnCerrarSesion_Click(sender As Object, e As EventArgs)
        Try
            ' Limpiar sesión
            SessionHelper.ClearSession()

            ' Redirigir al login
            Response.Redirect(Constants.ROUTE_LOGIN, False)

        Catch ex As Exception
            Logger.LogError("SelectorEntidades.btnCerrarSesion_Click", ex)
            Response.Redirect(Constants.ROUTE_LOGIN, False)
        End Try
    End Sub

    ''' <summary>
    ''' Muestra un mensaje al usuario
    ''' </summary>
    Private Sub MostrarMensaje(mensaje As String, tipo As String)
        pnlMensaje.Visible = True
        pnlMensaje.CssClass = $"alert alert-{tipo} alert-dismissible fade show"
        lblMensaje.Text = mensaje
    End Sub

End Class
