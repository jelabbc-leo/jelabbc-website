Imports DevExpress.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls

''' <summary>
''' Servicio para renderizar formularios dinámicos con controles DevExpress
''' </summary>
Public Class FormularioRenderService

    ''' <summary>
    ''' Renderiza un formulario completo en un contenedor usando DevExpress ASPxFormLayout
    ''' </summary>
    Public Sub RenderizarFormulario(container As Control, formulario As FormularioCompletoDTO)
        ' Crear FormLayout principal
        Dim formLayout As New ASPxFormLayout()

        formLayout.ID = $"formLayout_{formulario.Formulario.FormularioId}"
        formLayout.Width = Unit.Percentage(100)
        formLayout.ColCount = 12

        ' Agrupar campos por sección
        Dim secciones = formulario.Campos.GroupBy(Function(c) c.Seccion).OrderBy(Function(g) g.Key)

        For Each seccion In secciones
            ' Crear grupo para la sección
            Dim grupo As New LayoutGroup()

            grupo.Caption = If(String.IsNullOrEmpty(seccion.Key), "General", seccion.Key)
            grupo.ColCount = 12

            For Each campo In seccion.OrderBy(Function(c) c.PosicionOrden)
                Dim layoutItem As New LayoutItem()

                layoutItem.Caption = campo.EtiquetaCampo
                layoutItem.ColSpan = campo.AnchoColumna

                ' Mostrar asterisco para campos requeridos agregando al caption
                If campo.EsRequerido Then
                    layoutItem.Caption = campo.EtiquetaCampo & " *"
                End If

                ' Crear control según tipo
                Dim control = CrearControlPorTipo(campo)

                If control IsNot Nothing Then
                    layoutItem.Controls.Add(control)
                End If

                grupo.Items.Add(layoutItem)

            Next

            formLayout.Items.Add(grupo)

        Next

        container.Controls.Add(formLayout)
    End Sub

    Private Function CrearControlPorTipo(campo As CampoFormularioDTO) As Control
        Select Case campo.TipoCampo.ToLower()

            Case "texto"
                Return CrearASPxTextBox(campo)

            Case "numero"
                Return CrearASPxSpinEdit(campo, 0)

            Case "decimal"
                Return CrearASPxSpinEdit(campo, 2)

            Case "fecha"
                Return CrearASPxDateEdit(campo)

            Case "fecha_hora"
                Return CrearASPxDateEdit(campo, True)

            Case "hora"
                Return CrearASPxTimeEdit(campo)

            Case "dropdown"
                Return CrearASPxComboBox(campo)

            Case "checkbox"
                Return CrearASPxCheckBox(campo)

            Case "radio"
                Return CrearASPxRadioButtonList(campo)

            Case "textarea"
                Return CrearASPxMemo(campo)

            Case "foto", "archivo"
                Return CrearASPxUploadControl(campo)

            Case "firma"
                Return CrearControlFirma(campo)

            Case Else
                Return CrearASPxTextBox(campo)

        End Select
    End Function

    Private Function CrearASPxTextBox(campo As CampoFormularioDTO) As ASPxTextBox
        Dim txt As New ASPxTextBox()

        txt.ID = $"txt_{campo.NombreCampo}"
        txt.ClientInstanceName = $"txt_{campo.NombreCampo}"
        txt.Width = Unit.Percentage(100)
        txt.NullText = campo.Placeholder

        If campo.EsRequerido Then
            txt.ValidationSettings.RequiredField.IsRequired = True
            txt.ValidationSettings.RequiredField.ErrorText = $"{campo.EtiquetaCampo} es requerido"
        End If

        If campo.LongitudMaxima.HasValue Then
            txt.MaxLength = campo.LongitudMaxima.Value
        End If

        Return txt
    End Function

    Private Function CrearASPxSpinEdit(campo As CampoFormularioDTO, decimales As Integer) As ASPxSpinEdit
        Dim spin As New ASPxSpinEdit()

        spin.ID = $"spin_{campo.NombreCampo}"
        spin.ClientInstanceName = $"spin_{campo.NombreCampo}"
        spin.Width = Unit.Percentage(100)
        spin.DecimalPlaces = decimales
        spin.NumberType = If(decimales > 0, SpinEditNumberType.Float, SpinEditNumberType.Integer)

        If campo.EsRequerido Then
            spin.ValidationSettings.RequiredField.IsRequired = True
        End If

        Return spin
    End Function

    Private Function CrearASPxDateEdit(campo As CampoFormularioDTO, Optional incluirHora As Boolean = False) As ASPxDateEdit
        Dim dt As New ASPxDateEdit()

        dt.ID = $"dt_{campo.NombreCampo}"
        dt.ClientInstanceName = $"dt_{campo.NombreCampo}"
        dt.Width = Unit.Percentage(100)
        dt.DisplayFormatString = If(incluirHora, "dd/MM/yyyy HH:mm", "dd/MM/yyyy")
        dt.EditFormatString = dt.DisplayFormatString

        If incluirHora Then
            dt.TimeSectionProperties.Visible = True
        End If

        If campo.EsRequerido Then
            dt.ValidationSettings.RequiredField.IsRequired = True
        End If

        Return dt
    End Function

    Private Function CrearASPxTimeEdit(campo As CampoFormularioDTO) As ASPxTimeEdit
        Dim tm As New ASPxTimeEdit()

        tm.ID = $"tm_{campo.NombreCampo}"
        tm.ClientInstanceName = $"tm_{campo.NombreCampo}"
        tm.Width = Unit.Percentage(100)
        tm.DisplayFormatString = "HH:mm"

        Return tm
    End Function

    Private Function CrearASPxComboBox(campo As CampoFormularioDTO) As ASPxComboBox
        Dim cmb As New ASPxComboBox()

        cmb.ID = $"cmb_{campo.NombreCampo}"
        cmb.ClientInstanceName = $"cmb_{campo.NombreCampo}"
        cmb.Width = Unit.Percentage(100)
        cmb.ValueType = GetType(String)
        cmb.TextField = "EtiquetaOpcion"
        cmb.ValueField = "ValorOpcion"

        ' Agregar opciones
        If campo.Opciones IsNot Nothing Then

            For Each opcion In campo.Opciones
                cmb.Items.Add(opcion.EtiquetaOpcion, opcion.ValorOpcion)

            Next

        End If

        If campo.EsRequerido Then
            cmb.ValidationSettings.RequiredField.IsRequired = True
        End If

        Return cmb
    End Function

    Private Function CrearASPxCheckBox(campo As CampoFormularioDTO) As ASPxCheckBox
        Dim chk As New ASPxCheckBox()

        chk.ID = $"chk_{campo.NombreCampo}"
        chk.ClientInstanceName = $"chk_{campo.NombreCampo}"
        chk.Text = campo.EtiquetaCampo
        Return chk
    End Function

    Private Function CrearASPxRadioButtonList(campo As CampoFormularioDTO) As ASPxRadioButtonList
        Dim rbl As New ASPxRadioButtonList()

        rbl.ID = $"rbl_{campo.NombreCampo}"
        rbl.ClientInstanceName = $"rbl_{campo.NombreCampo}"
        rbl.RepeatDirection = RepeatDirection.Horizontal
        rbl.ValueType = GetType(String)

        If campo.Opciones IsNot Nothing Then

            For Each opcion In campo.Opciones
                rbl.Items.Add(opcion.EtiquetaOpcion, opcion.ValorOpcion)

            Next

        End If

        Return rbl
    End Function

    Private Function CrearASPxMemo(campo As CampoFormularioDTO) As ASPxMemo
        Dim memo As New ASPxMemo()

        memo.ID = $"memo_{campo.NombreCampo}"
        memo.ClientInstanceName = $"memo_{campo.NombreCampo}"
        memo.Width = Unit.Percentage(100)
        memo.Height = Unit.Pixel(100)
        memo.NullText = campo.Placeholder
        Return memo
    End Function

    Private Function CrearASPxUploadControl(campo As CampoFormularioDTO) As ASPxUploadControl
        Dim upload As New ASPxUploadControl()

        upload.ID = $"upload_{campo.NombreCampo}"
        upload.ClientInstanceName = $"upload_{campo.NombreCampo}"
        upload.Width = Unit.Percentage(100)
        upload.UploadMode = UploadControlUploadMode.Auto
        upload.ShowProgressPanel = True
        upload.ValidationSettings.AllowedFileExtensions = {".jpg", ".jpeg", ".png", ".pdf"}
        upload.ValidationSettings.MaxFileSize = 5 * 1024 * 1024 ' 5MB
        Return upload
    End Function

    Private Function CrearControlFirma(campo As CampoFormularioDTO) As Panel
        Dim panel As New Panel()

        panel.ID = $"pnlFirma_{campo.NombreCampo}"
        panel.CssClass = "firma-container"

        ' Canvas para firma (se maneja con JavaScript)
        Dim literal As New LiteralControl()

        literal.Text = $"<canvas id='canvas_{campo.NombreCampo}' class='firma-canvas' width='400' height='150'></canvas>" &
                      $"<input type='hidden' id='hf_{campo.NombreCampo}' name='hf_{campo.NombreCampo}' />" &
                      $"<button type='button' class='btn btn-sm btn-secondary mt-2' onclick='FormulariosDinamicosModule.limpiarFirma(""{campo.NombreCampo}"")'>Limpiar Firma</button>"
        panel.Controls.Add(literal)

        Return panel
    End Function
End Class
