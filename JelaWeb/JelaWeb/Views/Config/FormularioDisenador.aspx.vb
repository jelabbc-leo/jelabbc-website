Imports DevExpress.Web
Imports Newtonsoft.Json

Public Class FormularioDisenador
    Inherits System.Web.UI.Page

    Private ReadOnly _formularioService As New FormularioService()
    Private ReadOnly _documentIntelligenceService As New DocumentIntelligenceFormulariosService()

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Dim idParam = Request.QueryString("id")

            If Not String.IsNullOrEmpty(idParam) Then
                Dim formularioId As Integer

                If Integer.TryParse(idParam, formularioId) AndAlso formularioId > 0 Then
                    CargarFormulario(formularioId)
                End If
            Else
                lblTitulo.Text = "Nuevo Formulario"
            End If
        End If

        ' Registrar IDs de hidden fields para JavaScript
        Dim script As String = String.Format(
            "var hfFormularioIdClientID = '{0}'; var hfCamposJSONClientID = '{1}';",
            hfFormularioId.ClientID, hfCamposJSON.ClientID)
        ScriptManager.RegisterStartupScript(Me, Me.GetType(), "initHiddenFields", script, True)
    End Sub

    Private Sub CargarFormulario(formularioId As Integer)

        Try
            Dim formulario = _formularioService.GetFormularioById(formularioId)

            If formulario IsNot Nothing Then
                hfFormularioId.Value = formulario.FormularioId.ToString()
                lblTitulo.Text = "Editar: " & formulario.NombreFormulario
                txtNombre.Text = formulario.NombreFormulario
                cmbEstado.Value = formulario.Estado

                ' Cargar plataformas
                If Not String.IsNullOrEmpty(formulario.Plataformas) Then
                    Dim plats = formulario.Plataformas.Split(","c)

                    For Each item As ListEditItem In chkPlataformas.Items
                        If item.Value IsNot Nothing Then
                            item.Selected = plats.Contains(item.Value.ToString())
                        End If

                    Next

                End If

                ' Cargar campos para el diseñador
                Dim campos = _formularioService.GetCamposFormulario(formularioId)
                Dim camposJSON = ConvertirCamposAJSON(campos)

                hfCamposJSON.Value = camposJSON

                ' Inicializar diseñador con campos
                Dim initScript = String.Format("setTimeout(function() {{ cargarCamposDesdeServidor('{0}'); }}, 300);",
                    camposJSON.Replace("'", "\'").Replace(vbCr, "").Replace(vbLf, ""))
                ScriptManager.RegisterStartupScript(Me, Me.GetType(), "loadCampos", initScript, True)
            End If

        Catch ex As Exception
            Logger.LogError("FormularioDisenador.CargarFormulario", ex.Message)

        End Try
    End Sub

    Private Function ConvertirCamposAJSON(campos As List(Of CampoFormularioDTO)) As String
        Dim camposDesigner = campos.Select(Function(c, index) New With {
            .id = "campo_" & (index + 1),
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
        }).ToList()

        Return JsonConvert.SerializeObject(camposDesigner)
    End Function

    Protected Sub btnGuardar_Click(sender As Object, e As EventArgs)

        Try
            Dim formulario As New FormularioDTO()

            formulario.NombreFormulario = txtNombre.Text.Trim()
            formulario.Estado = If(cmbEstado.Value IsNot Nothing, cmbEstado.Value.ToString(), "borrador")

            ' Obtener plataformas
            Dim plataformas As New List(Of String)()

            For Each item As ListEditItem In chkPlataformas.Items
                If item.Selected AndAlso item.Value IsNot Nothing Then
                    plataformas.Add(item.Value.ToString())
                End If

            Next

            formulario.Plataformas = String.Join(",", plataformas)

            Dim formularioId As Integer = 0
            Dim formularioIdStr = hfFormularioId.Value

            If String.IsNullOrEmpty(formularioIdStr) OrElse formularioIdStr = "0" Then
                formularioId = _formularioService.CreateFormulario(formulario)
                hfFormularioId.Value = formularioId.ToString()
            Else
                formularioId = Integer.Parse(formularioIdStr)
                formulario.FormularioId = formularioId
                _formularioService.UpdateFormulario(formulario)
            End If

            ' Guardar campos
            GuardarCamposDesdeJSON(formularioId)

            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "success",
                "alert('Formulario guardado exitosamente'); window.location.href='FormulariosDinamicos.aspx';", True)

        Catch ex As Exception
            Logger.LogError("FormularioDisenador.btnGuardar_Click", ex.Message)
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "error",
                $"alert('Error al guardar: {ex.Message}');", True)

        End Try
    End Sub

    Private Sub GuardarCamposDesdeJSON(formularioId As Integer)

        Try
            Dim jsonCampos = hfCamposJSON.Value

            If String.IsNullOrEmpty(jsonCampos) OrElse jsonCampos = "[]" Then Return

            Dim camposDesigner = JsonConvert.DeserializeObject(Of List(Of CampoDesignerDTO))(jsonCampos)

            If camposDesigner Is Nothing OrElse camposDesigner.Count = 0 Then Return

            ' Obtener campos existentes
            Dim camposExistentes = _formularioService.GetCamposFormulario(formularioId)
            Dim idsEnJSON = camposDesigner.Where(Function(c) c.campoId > 0).Select(Function(c) c.campoId).ToList()

            ' Eliminar campos que ya no están

            For Each campoExistente In camposExistentes
                If Not idsEnJSON.Contains(campoExistente.CampoId) Then
                    _formularioService.DeleteCampo(campoExistente.CampoId)
                End If

            Next

            ' Crear o actualizar campos

            For Each campoDesigner In camposDesigner
                Dim campo As New CampoFormularioDTO()

                campo.FormularioId = formularioId
                campo.EtiquetaCampo = campoDesigner.etiqueta
                campo.NombreCampo = If(String.IsNullOrEmpty(campoDesigner.nombre),
                    _documentIntelligenceService.LimpiarNombreCampo(campoDesigner.etiqueta),
                    campoDesigner.nombre)
                campo.TipoCampo = campoDesigner.tipo
                campo.Seccion = If(String.IsNullOrEmpty(campoDesigner.seccion), "General", campoDesigner.seccion)
                campo.PosicionOrden = campoDesigner.orden
                campo.AnchoColumna = If(campoDesigner.ancho > 0, campoDesigner.ancho, 12)
                campo.AlturaCampo = If(campoDesigner.altura > 0, campoDesigner.altura, Nothing)
                campo.EsRequerido = campoDesigner.requerido
                campo.Placeholder = campoDesigner.placeholder
                campo.EsVisible = True

                If campoDesigner.campoId > 0 Then
                    campo.CampoId = campoDesigner.campoId
                    _formularioService.UpdateCampo(campo)
                Else
                    _formularioService.CreateCampo(campo)
                End If

            Next

        Catch ex As Exception
            Logger.LogError("FormularioDisenador.GuardarCamposDesdeJSON", ex.Message)
            Throw

        End Try
    End Sub

    Private Class CampoDesignerDTO
        Public Property id As String
        Public Property campoId As Integer
        Public Property etiqueta As String
        Public Property nombre As String
        Public Property tipo As String
        Public Property seccion As String
        Public Property requerido As Boolean
        Public Property ancho As Integer
        Public Property altura As Integer
        Public Property placeholder As String
        Public Property orden As Integer
    End Class
End Class