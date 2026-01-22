Imports System.Web.UI.HtmlControls

Public Class GestionDocumentosMenu
    Inherits BasePage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' Agregar el CSS de Gestión de Documentos al head de la página
        If Not IsPostBack Then
            AddDocumentosStyles()
        End If
    End Sub

    ''' <summary>
    ''' Agrega el CSS de Gestión de Documentos al head de la página
    ''' </summary>
    Private Sub AddDocumentosStyles()
        ' Verificar si el CSS ya fue agregado
        Dim cssLink As HtmlLink = TryCast(Page.Header.FindControl("documentosCssLink"), HtmlLink)

        If cssLink Is Nothing Then
            ' Crear el link tag para el CSS
            cssLink = New HtmlLink()
            cssLink.ID = "documentosCssLink"
            cssLink.Href = ResolveUrl("~/Content/CSS/gestiondocumentosmenu.css")
            cssLink.Attributes("rel") = "stylesheet"
            cssLink.Attributes("type") = "text/css"

            ' Agregar al head
            Page.Header.Controls.Add(cssLink)
        End If
    End Sub

End Class
