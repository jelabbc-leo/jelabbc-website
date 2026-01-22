Imports Newtonsoft.Json

Partial Public Class FormularioVistaPrevia
    Inherits System.Web.UI.Page

    Private ReadOnly _formularioService As New FormularioService()

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Dim idParam = Request.QueryString("id")
            Dim camposPost = Request.Form("camposJSON")

            If Not String.IsNullOrEmpty(idParam) Then
                ' Cargar desde base de datos
                Dim formularioId As Integer

                If Integer.TryParse(idParam, formularioId) AndAlso formularioId > 0 Then
                    CargarFormulario(formularioId)
                End If
            ElseIf Not String.IsNullOrEmpty(camposPost) Then
                ' Cargar desde POST (vista previa sin guardar)
                CargarDesdePost()
            Else
                ' Sin datos - mostrar mensaje
                MostrarSinDatos()
            End If
        End If

        ' Registrar IDs de hidden fields para JavaScript
        Dim script As String = String.Format(
            "var hfFormularioIdClientID = '{0}'; var hfCamposJSONClientID = '{1}'; var hfPlataformasClientID = '{2}';",
            hfFormularioId.ClientID, hfCamposJSON.ClientID, hfPlataformas.ClientID)
        ScriptManager.RegisterStartupScript(Me, Me.GetType(), "initHiddenFields", script, True)
    End Sub

    Private Sub CargarFormulario(formularioId As Integer)

        Try
            Dim formulario = _formularioService.GetFormularioById(formularioId)

            If formulario IsNot Nothing Then
                hfFormularioId.Value = formulario.FormularioId.ToString()
                lblNombreFormulario.Text = formulario.NombreFormulario
                lblTituloMovil.Text = formulario.NombreFormulario
                lblUrlWeb.Text = LimpiarParaUrl(formulario.NombreFormulario)

                ' Plataformas
                hfPlataformas.Value = If(String.IsNullOrEmpty(formulario.Plataformas), "web,movil", formulario.Plataformas)

                ' Cargar campos
                Dim campos = _formularioService.GetCamposFormulario(formularioId)
                Dim camposJSON = ConvertirCamposAJSON(campos)

                hfCamposJSON.Value = camposJSON

                ' Inicializar vista previa
                InicializarVistaPrevia(camposJSON, hfPlataformas.Value)
            End If

        Catch ex As Exception
            Logger.LogError("FormularioVistaPrevia.CargarFormulario", ex, "")

        End Try
    End Sub

    Private Sub CargarDesdePost()

        Try
            ' Obtener datos del POST
            Dim camposJSON = Request.Form("camposJSON")
            Dim nombreParam = Request.Form("nombreFormulario")
            Dim plataformasParam = Request.Form("plataformas")

            If String.IsNullOrEmpty(camposJSON) Then
                MostrarSinDatos()
                Return
            End If

            hfCamposJSON.Value = camposJSON
            lblNombreFormulario.Text = If(String.IsNullOrEmpty(nombreParam), "Vista Previa", nombreParam)
            lblTituloMovil.Text = lblNombreFormulario.Text
            lblUrlWeb.Text = LimpiarParaUrl(lblNombreFormulario.Text)
            hfPlataformas.Value = If(String.IsNullOrEmpty(plataformasParam), "web,movil", plataformasParam)

            ' Inicializar vista previa
            InicializarVistaPrevia(camposJSON, hfPlataformas.Value)

        Catch ex As Exception
            Logger.LogError("FormularioVistaPrevia.CargarDesdePost", ex, "")
            MostrarSinDatos()

        End Try
    End Sub

    Private Sub MostrarSinDatos()
        lblNombreFormulario.Text = "Sin datos"
        lblTituloMovil.Text = "Sin datos"
        hfCamposJSON.Value = "[]"
        hfPlataformas.Value = "web,movil"

        Dim initScript = "setTimeout(function() { inicializarVistaPrevia('[]', 'web,movil'); }, 300);"

        ScriptManager.RegisterStartupScript(Me, Me.GetType(), "initPreview", initScript, True)
    End Sub

    Private Sub InicializarVistaPrevia(camposJSON As String, plataformas As String)
        ' Escapar caracteres especiales para JavaScript
        Dim camposEscaped = camposJSON.Replace("\", "\\").Replace("'", "\'").Replace(vbCr, "").Replace(vbLf, "")
        Dim initScript = String.Format("setTimeout(function() {{ inicializarVistaPrevia('{0}', '{1}'); }}, 300);",
            camposEscaped, plataformas)
        ScriptManager.RegisterStartupScript(Me, Me.GetType(), "initPreview", initScript, True)
    End Sub

    Private Function ConvertirCamposAJSON(campos As List(Of CampoFormularioDTO)) As String
        Dim camposPreview = campos.Select(Function(c) New With {
            .id = "campo_" & c.CampoId,
            .campoId = c.CampoId,
            .etiqueta = c.EtiquetaCampo,
            .nombre = c.NombreCampo,
            .tipo = c.TipoCampo,
            .seccion = If(String.IsNullOrEmpty(c.Seccion), "General", c.Seccion),
            .requerido = c.EsRequerido,
            .ancho = If(c.AnchoColumna > 0, c.AnchoColumna, 12),
            .altura = If(c.AlturaCampo.HasValue, c.AlturaCampo.Value, 0),
            .placeholder = If(c.Placeholder, ""),
            .orden = c.PosicionOrden
        }).OrderBy(Function(c) c.orden).ToList()

        Return JsonConvert.SerializeObject(camposPreview)
    End Function

    Private Function LimpiarParaUrl(nombre As String) As String
        If String.IsNullOrEmpty(nombre) Then Return "formulario"
        Return nombre.ToLower().Replace(" ", "-").Replace("á", "a").Replace("é", "e").Replace("í", "i").Replace("ó", "o").Replace("ú", "u")
    End Function

End Class
