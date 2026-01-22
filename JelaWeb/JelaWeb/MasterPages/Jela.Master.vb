Imports System.Windows.Controls.Ribbon
Imports DevExpress.Web
Imports Newtonsoft.Json.Linq
Imports RibbonTab = DevExpress.Web.RibbonTab
Imports System.Text
Imports System.Web.UI.HtmlControls

Public Class Jela
    Inherits System.Web.UI.MasterPage

    ''' <summary>
    ''' Propiedad pública para exponer el IdEntidad actual al markup
    ''' </summary>
    Public ReadOnly Property IdEntidadActual As Integer
        Get
            Dim idEntidad = SessionHelper.GetIdEntidadActual()
            Return If(idEntidad.HasValue, idEntidad.Value, 1)
        End Get
    End Property

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load

        Try
            ' Aplicar localización
            LocalizationHelper.ApplyCulture()

            ' Configurar selector de idioma
            LoadLanguageSelector()

            If Not IsPostBack Then
                ' Obtener opciones usando SessionHelper
                Dim opciones As JArray = SessionHelper.GetOpciones()

                If opciones IsNot Nothing Then
                    ConstruirRibbon(opciones)
                Else
                    ' Si no hay opciones en sesión, redirigir al login
                    Logger.LogWarning($"Usuario sin opciones en sesión: {SessionHelper.GetNombre()}")
                    SessionHelper.ClearSession()
                    Response.Redirect(Constants.ROUTE_LOGIN, True)
                End If
                
                ' Cargar dropdown de entidades si es administrador con múltiples entidades
                CargarDropdownEntidades()
            End If

        Catch ex As Exception
            Logger.LogError("Error en Page_Load del MasterPage", ex, SessionHelper.GetNombre())
            ' Si hay un error crítico, redirigir al login
            SessionHelper.ClearSession()
            Response.Redirect(Constants.ROUTE_LOGIN & "?error=1", True)

        End Try
    End Sub

    ''' <summary>
    ''' Configura el selector de idioma en la barra de estado
    ''' </summary>
    Private Sub LoadLanguageSelector()

        Try
            ' Solo configurar en la carga inicial, no en PostBack
            ' para evitar resetear el valor antes de procesar el cambio
            If Not IsPostBack Then
                Dim currentCulture = LocalizationHelper.GetCurrentCulture()

                If cmbLanguage IsNot Nothing Then
                    ' Buscar el item que coincide con la cultura actual

                    For i As Integer = 0 To cmbLanguage.Items.Count - 1
                        Dim item = cmbLanguage.Items(i)

                        If item.Value IsNot Nothing AndAlso 
                           item.Value.ToString().Equals(currentCulture.Name, StringComparison.OrdinalIgnoreCase) Then
                            cmbLanguage.SelectedIndex = i
                            Exit For
                        End If

                    Next

                    ' Si no se encontró, usar español como predeterminado
                    If cmbLanguage.SelectedIndex < 0 Then
                        cmbLanguage.SelectedIndex = 0
                    End If
                End If
            End If

        Catch ex As Exception
            Logger.LogError("Error al configurar selector de idioma", ex)

        End Try
    End Sub

    ''' <summary>
    ''' Maneja el cambio de idioma desde la barra de estado
    ''' </summary>
    Protected Sub cmbLanguage_SelectedIndexChanged(sender As Object, e As EventArgs)

        Try
            ' Obtener el valor seleccionado del combo
            Dim selectedValue As String = String.Empty

            If cmbLanguage.SelectedItem IsNot Nothing AndAlso cmbLanguage.SelectedItem.Value IsNot Nothing Then
                selectedValue = cmbLanguage.SelectedItem.Value.ToString()
            ElseIf cmbLanguage.Value IsNot Nothing Then
                selectedValue = cmbLanguage.Value.ToString()
            End If

            If Not String.IsNullOrEmpty(selectedValue) Then
                Logger.LogInfo($"Cambiando idioma a: {selectedValue}", SessionHelper.GetNombre())
                LocalizationHelper.SetCulture(selectedValue)

                ' Recargar la página para aplicar los cambios
                Response.Redirect(Request.RawUrl, False)
                HttpContext.Current.ApplicationInstance.CompleteRequest()
            End If

        Catch ex As Threading.ThreadAbortException

            ' Ignorar - es normal cuando se hace Response.Redirect

        Catch ex As Exception
            Logger.LogError("Error al cambiar idioma", ex, SessionHelper.GetNombre())

        End Try
    End Sub

    ''' <summary>
    ''' Pobla la información de la barra de estado
    ''' </summary>
    Private Sub PoblarBarraEstado()

        Try
            ' Buscar los controles usando FindControl si no están disponibles directamente
            Dim usuarioLabel As Label = lblUsuario
            Dim versionLabel As Label = lblVersion
            Dim apiLabel As Label = lblApi
            Dim paginaLabel As Label = lblPaginaActual

            ' Si algún control es Nothing, intentar encontrarlo con FindControl
            If usuarioLabel Is Nothing Then
                usuarioLabel = TryCast(FindControl("lblUsuario"), Label)
            End If
            If versionLabel Is Nothing Then
                versionLabel = TryCast(FindControl("lblVersion"), Label)
            End If
            If apiLabel Is Nothing Then
                apiLabel = TryCast(FindControl("lblApi"), Label)
            End If
            If paginaLabel Is Nothing Then
                paginaLabel = TryCast(FindControl("lblPaginaActual"), Label)
            End If

            ' Si aún no se encuentran, buscar en statusBar
            If statusBar IsNot Nothing Then
                If usuarioLabel Is Nothing Then
                    usuarioLabel = TryCast(statusBar.FindControl("lblUsuario"), Label)
                End If
                If versionLabel Is Nothing Then
                    versionLabel = TryCast(statusBar.FindControl("lblVersion"), Label)
                End If
                If apiLabel Is Nothing Then
                    apiLabel = TryCast(statusBar.FindControl("lblApi"), Label)
                End If
                If paginaLabel Is Nothing Then
                    paginaLabel = TryCast(statusBar.FindControl("lblPaginaActual"), Label)
                End If
            End If

            ' Verificar que los controles estén inicializados
            If usuarioLabel Is Nothing OrElse versionLabel Is Nothing OrElse apiLabel Is Nothing Then
                Logger.LogWarning("Controles de la barra de estado no inicializados correctamente")
                Return
            End If

            ' Página actual - obtener del título de la página
            If paginaLabel IsNot Nothing Then
                Dim pageTitle As String = Page.Title

                If Not String.IsNullOrEmpty(pageTitle) Then
                    paginaLabel.Text = pageTitle
                Else
                    paginaLabel.Text = "Inicio"
                End If
            End If

            ' Usuario firmado
            Dim nombreUsuario As String = SessionHelper.GetNombre()

            If Not String.IsNullOrEmpty(nombreUsuario) Then
                usuarioLabel.Text = nombreUsuario
            Else
                usuarioLabel.Text = "No autenticado"
            End If

            ' Versión del sistema
            versionLabel.Text = Constants.SYSTEM_VERSION

            ' API/Base de datos conectada
            Dim apiUrl As String = ConfigurationManager.AppSettings("ApiBaseUrl")

            If Not String.IsNullOrEmpty(apiUrl) Then
                ' Extraer el dominio de la URL de la API

                Try
                    Dim uri As New Uri(apiUrl)
                    Dim hostName As String = uri.Host

                    ' Mostrar solo el nombre del host sin el subdominio completo si es muy largo
                    If hostName.Length > 30 Then
                        hostName = hostName.Substring(0, 27) & "..."
                    End If
                    apiLabel.Text = hostName

                Catch
                    ' Si no se puede parsear la URL, mostrar una versión truncada
                    If apiUrl.Length > 30 Then
                        apiLabel.Text = apiUrl.Substring(0, 27) & "..."
                    Else
                        apiLabel.Text = apiUrl
                    End If

                End Try
            Else
                apiLabel.Text = "No configurada"
            End If

        Catch ex As Exception
            Logger.LogError("Error al poblar la barra de estado", ex)

        End Try
    End Sub

    Private Sub ConstruirRibbon(opciones As JArray)
        ' Agrupar por RibbonTab
        Dim tabs = opciones.GroupBy(Function(o) o("RibbonTab").ToString())

        For Each tabGroup In tabs
            Dim ribbonTab As New RibbonTab(tabGroup.Key)

            ribbonControl.Tabs.Add(ribbonTab)

            ' Agrupar por RibbonGroup dentro del tab
            Dim groups = tabGroup.GroupBy(Function(o) o("RibbonGroup").ToString())

            For Each group In groups
                Dim ribbonGroup As New DevExpress.Web.RibbonGroup(group.Key)

                ribbonTab.Groups.Add(ribbonGroup)

                For Each opcion In group
                    Dim btn As New DevExpress.Web.RibbonButtonItem()

                    btn.Text = opcion("Nombre").ToString()
                    btn.LargeImage.Url = opcion("Icono").ToString()
                    Dim url As String = opcion("Url").ToString()

                    btn.Size = RibbonItemSize.Large

                    ' Usar NavigateUrl normal para navegación directa
                    btn.NavigateUrl = url

                    ribbonGroup.Items.Add(btn)

                Next

            Next

        Next

    End Sub

    ''' <summary>
    ''' Maneja el click del botón de cerrar sesión
    ''' </summary>
    Protected Sub btnCerrarSesion_Click(sender As Object, e As EventArgs)

        Try
            ' Log del logout
            Dim usuario As String = SessionHelper.GetNombre()

            Logger.LogInfo($"Usuario cerrando sesión: {usuario}", usuario)

            ' Limpiar sesión
            SessionHelper.ClearSession()

            ' Redirigir al login
            Response.Redirect(Constants.ROUTE_LOGIN & "?logout=1", True)

        Catch ex As Exception
            Logger.LogError("Error al cerrar sesión", ex, SessionHelper.GetNombre())
            ' Asegurar que se limpie la sesión incluso si hay error

            Try
                SessionHelper.ClearSession()

            Catch
                ' Ignorar errores al limpiar sesión

            End Try
            Response.Redirect(Constants.ROUTE_LOGIN & "?logout=1", True)

        End Try
    End Sub

    ''' <summary>
    ''' Pre-render de la página
    ''' </summary>
    Protected Overrides Sub OnPreRender(e As EventArgs)
        MyBase.OnPreRender(e)

        ' COMENTADO: Causa error cuando hay bloques <% %> en el markup del MasterPage
        ' El error es: "La colección de controles no puede modificarse porque el control contiene bloques de código"
        ' Los scripts y estilos deben agregarse directamente en el markup del .master
        ' LoadScriptsAndStyles()

        ' Poblar la barra de estado (los controles ya están inicializados en OnPreRender)
        PoblarBarraEstado()

        ' Configurar el nombre de usuario en el menú popup
        If lblUsuarioMenu IsNot Nothing Then
            Dim nombreUsuario As String = SessionHelper.GetNombre()

            If Not String.IsNullOrEmpty(nombreUsuario) Then
                lblUsuarioMenu.Text = nombreUsuario
            Else
                Dim userId = SessionHelper.GetUserId()

                If userId.HasValue Then
                    lblUsuarioMenu.Text = $"user{userId.Value}"
                Else
                    lblUsuarioMenu.Text = "Usuario"
                End If
            End If
        End If
    End Sub
    
    ''' <summary>
    ''' Carga el dropdown de entidades para administradores con múltiples entidades
    ''' </summary>
    Private Sub CargarDropdownEntidades()
        Try
            ' Verificar si es administrador de condominios
            If SessionHelper.IsAdministradorCondominios() Then
                ' Obtener entidades de la sesión
                Dim entidades = SessionHelper.GetEntidades()
                
                ' Mostrar dropdown si tiene al menos una entidad
                If entidades IsNot Nothing AndAlso entidades.Count > 0 Then
                    ' Mostrar panel y separador
                    pnlSelectorEntidades.Visible = True
                    pnlSeparadorEntidades.Visible = True
                    
                    ' Limpiar items existentes
                    ddlEntidades.Items.Clear()
                    
                    ' Agregar entidades al dropdown
                    For Each entidad As JObject In entidades
                        Dim item As New ListEditItem()
                        item.Text = entidad("Nombre").ToString()
                        item.Value = entidad("Id").ToObject(Of Integer)()
                        ddlEntidades.Items.Add(item)
                    Next
                    
                    ' Seleccionar la entidad actual
                    Dim idEntidadActual = SessionHelper.GetIdEntidadActual()
                    If idEntidadActual.HasValue Then
                        ddlEntidades.Value = idEntidadActual.Value
                    End If
                    
                    Logger.LogInfo($"Dropdown de entidades cargado con {entidades.Count} entidades para administrador")
                Else
                    ' Ocultar si no tiene entidades
                    pnlSelectorEntidades.Visible = False
                    pnlSeparadorEntidades.Visible = False
                End If
            Else
                ' Ocultar panel para usuarios que no son administradores
                pnlSelectorEntidades.Visible = False
                pnlSeparadorEntidades.Visible = False
            End If
            
        Catch ex As Exception
            Logger.LogError("Error al cargar dropdown de entidades", ex)
            pnlSelectorEntidades.Visible = False
            pnlSeparadorEntidades.Visible = False
        End Try
    End Sub
    
    ''' <summary>
    ''' Maneja el cambio de entidad seleccionada en el dropdown
    ''' </summary>
    Protected Sub ddlEntidades_SelectedIndexChanged(sender As Object, e As EventArgs)
        Try
            ' Obtener la entidad seleccionada
            Dim idEntidad As Integer = Convert.ToInt32(ddlEntidades.Value)
            Dim nombreEntidad As String = ddlEntidades.SelectedItem.Text
            
            ' Actualizar la sesión con la nueva entidad
            SessionHelper.SetEntidadActual(idEntidad, nombreEntidad)
            
            ' Log de auditoría
            Logger.LogInfo($"Usuario {SessionHelper.GetNombre()} cambió a entidad: {nombreEntidad} (ID: {idEntidad})")
            
            ' Recargar la página actual para aplicar el cambio
            Response.Redirect(Request.RawUrl, False)
            HttpContext.Current.ApplicationInstance.CompleteRequest()
            
        Catch ex As Threading.ThreadAbortException
            ' Ignorar - es normal cuando se hace Response.Redirect
            
        Catch ex As Exception
            Logger.LogError("Error al cambiar entidad", ex, SessionHelper.GetNombre())
            ' Mostrar mensaje de error al usuario
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "ErrorCambioEntidad",
                "showToast('error', 'Error al cambiar de entidad. Por favor, intente nuevamente.');", True)
        End Try
    End Sub

    ''' <summary>
    ''' Carga los scripts y estilos necesarios para el Master Page
    ''' </summary>
    Private Sub LoadScriptsAndStyles()

        Try
            ' Cargar CSS
            AddStyleSheet("siteCssLink", "~/Content/Styles/site.css")
            AddStyleSheet("entidadesCssLink", "~/Content/CSS/entidades.css")

            ' Cargar Toastr CSS desde CDN (URL absoluta, no usar ResolveUrl)
            AddStyleSheetExternal("toastrCssLink", "https://cdnjs.cloudflare.com/ajax/libs/toastr.js/latest/toastr.min.css")

            ' Nota: El stub de toastr y showToast ahora se cargan directamente en el archivo .master
            ' antes del ContentPlaceHolder para asegurar que estén disponibles desde el inicio

            ' Cargar Toastr JS desde CDN (al final del body, después de otros scripts)
            Dim toastrScript As HtmlGenericControl = TryCast(Me.FindControl("toastrJsScript"), HtmlGenericControl)

            If toastrScript Is Nothing Then
                toastrScript = New HtmlGenericControl("script")
                toastrScript.ID = "toastrJsScript"
                toastrScript.Attributes("type") = "text/javascript"
                toastrScript.Attributes("src") = "https://cdnjs.cloudflare.com/ajax/libs/toastr.js/latest/toastr.min.js"
                ' Inicializar toastr cuando el script se haya cargado completamente
                toastrScript.Attributes("onload") = "if (typeof toastr !== 'undefined' && toastr) { " &
                    "toastr.options = { closeButton: true, progressBar: true, positionClass: 'toast-top-right', timeOut: '5000' }; " &
                    "console.log('Toastr cargado correctamente'); " &
                    "}"
                Me.Controls.Add(toastrScript)
            End If

        Catch ex As Exception
            Logger.LogError("Error al cargar scripts y estilos en Master Page: " & ex.Message, ex, "")

        End Try
    End Sub

    ''' <summary>
    ''' Agrega una hoja de estilos al head
    ''' </summary>
    Private Sub AddStyleSheet(id As String, href As String)

        Try
            ' Verificar si el CSS ya fue agregado
            Dim cssLink As HtmlLink = TryCast(Page.Header.FindControl(id), HtmlLink)

            If cssLink Is Nothing Then
                ' Crear el link tag para el CSS
                cssLink = New HtmlLink()
                cssLink.ID = id
                cssLink.Href = ResolveUrl(href)
                cssLink.Attributes("rel") = "stylesheet"
                cssLink.Attributes("type") = "text/css"

                ' Agregar al head
                Page.Header.Controls.Add(cssLink)
            End If

        Catch ex As Exception
            Logger.LogError("Error al agregar hoja de estilos: " & id & " - " & href, ex, "")

        End Try
    End Sub

    ''' <summary>
    ''' Agrega un script al head o al final del body
    ''' </summary>
    Private Sub AddScript(id As String, src As String, addToHead As Boolean)
        Dim scriptControl As HtmlGenericControl = Nothing

        If addToHead Then
            scriptControl = TryCast(Page.Header.FindControl(id), HtmlGenericControl)
        Else
            ' Buscar en el form
            scriptControl = TryCast(Me.FindControl(id), HtmlGenericControl)
        End If

        If scriptControl Is Nothing Then
            scriptControl = New HtmlGenericControl("script")
            scriptControl.ID = id
            scriptControl.Attributes("type") = "text/javascript"
            scriptControl.Attributes("src") = ResolveUrl(src)

            If addToHead Then
                Page.Header.Controls.Add(scriptControl)
            Else
                ' Agregar al final del form (antes del cierre)
                Me.Controls.Add(scriptControl)
            End If
        End If
    End Sub

    ''' <summary>
    ''' Agrega una hoja de estilos externa (CDN) al head
    ''' </summary>
    Private Sub AddStyleSheetExternal(id As String, href As String)

        Try
            ' Verificar si el CSS ya fue agregado
            Dim cssLink As HtmlLink = TryCast(Page.Header.FindControl(id), HtmlLink)

            If cssLink Is Nothing Then
                ' Crear el link tag para el CSS
                cssLink = New HtmlLink()
                cssLink.ID = id
                cssLink.Href = href ' URL absoluta, no usar ResolveUrl
                cssLink.Attributes("rel") = "stylesheet"
                cssLink.Attributes("type") = "text/css"

                ' Agregar al head
                Page.Header.Controls.Add(cssLink)
            End If

        Catch ex As Exception
            Logger.LogError("Error al agregar hoja de estilos externa: " & id & " - " & href, ex, "")

        End Try
    End Sub

    ''' <summary>
    ''' Agrega un script externo (CDN) al head o al final del body
    ''' </summary>
    Private Sub AddScriptExternal(id As String, src As String, addToHead As Boolean)
        Dim scriptControl As HtmlGenericControl = Nothing

        If addToHead Then
            scriptControl = TryCast(Page.Header.FindControl(id), HtmlGenericControl)
        Else
            ' Buscar en el form
            scriptControl = TryCast(Me.FindControl(id), HtmlGenericControl)
        End If

        If scriptControl Is Nothing Then
            scriptControl = New HtmlGenericControl("script")
            scriptControl.ID = id
            scriptControl.Attributes("type") = "text/javascript"
            scriptControl.Attributes("src") = src ' URL absoluta, no usar ResolveUrl

            If addToHead Then
                Page.Header.Controls.Add(scriptControl)
            Else
                ' Agregar al final del form (antes del cierre)
                Me.Controls.Add(scriptControl)
            End If
        End If
    End Sub

End Class
