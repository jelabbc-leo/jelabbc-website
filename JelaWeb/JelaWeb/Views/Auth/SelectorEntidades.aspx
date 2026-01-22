<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="SelectorEntidades.aspx.vb" Inherits="JelaWeb.SelectorEntidades" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Selector de Entidades - JELA</title>
    <link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.0/css/all.min.css" rel="stylesheet" />
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.1.3/dist/css/bootstrap.min.css" rel="stylesheet" />
    <link href="~/Content/Styles/selector-entidades.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
        <!-- Botones flotantes en la esquina superior derecha -->
        <div class="floating-buttons">
            <a id="btnAgregarEntidad" 
               runat="server" 
               href="#"
               class="btn-floating btn-floating-add"
               onserverclick="btnAgregarEntidad_Click"
               title="Agregar Nuevo Condominio"></a>
            <a id="btnCerrarSesion" 
               runat="server" 
               href="#"
               class="btn-floating btn-floating-logout"
               onserverclick="btnCerrarSesion_Click"
               title="Cerrar Sesión"></a>
        </div>

        <div class="selector-wrapper">
            <div class="selector-container">
                <!-- Header -->
                <div class="selector-header">
                    <img src="<%= ResolveUrl("/Content/Images/LogoJelaBBC.png") %>" class="logo" alt="JELA Logo" />
                    <h2>Seleccione un Condominio</h2>
                    <p class="user-name">Bienvenido, <asp:Label ID="lblNombreUsuario" runat="server" /></p>
                </div>

                <!-- Mensaje de alerta -->
                <asp:Panel ID="pnlMensaje" runat="server" Visible="false" CssClass="alert alert-info alert-dismissible fade show">
                    <asp:Label ID="lblMensaje" runat="server" />
                    <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
                </asp:Panel>

                <!-- Lista de entidades -->
                <div class="entidades-grid">
                    <asp:Repeater ID="rptEntidades" runat="server" OnItemCommand="rptEntidades_ItemCommand">
                        <ItemTemplate>
                            <asp:LinkButton 
                                ID="btnCardEntidad" 
                                runat="server" 
                                CommandName="Seleccionar" 
                                CommandArgument='<%# Eval("Id") %>'
                                CssClass="entidad-card">
                                <div class="entidad-logo">
                                    <i class="fas fa-building"></i>
                                </div>
                                <div class="entidad-info">
                                    <h3><%# Eval("Nombre") %></h3>
                                    <p class="entidad-direccion"><%# Eval("Direccion") %></p>
                                    <%# If(CBool(Eval("EsPrincipal")), "<span class='badge bg-primary'>Principal</span>", "") %>
                                </div>
                                <div class="entidad-arrow">
                                    <i class="fas fa-arrow-right"></i>
                                </div>
                            </asp:LinkButton>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>

                <!-- Footer con licencias -->
                <div class="selector-footer">
                    <div class="licencias-info">
                        <asp:Label ID="lblLicencias" runat="server" CssClass="text-muted" />
                    </div>
                </div>

                <div class="copyright">
                    <p>&copy; <%= DateTime.Now.Year %> JELA. Todos los derechos reservados.</p>
                </div>
            </div>
        </div>
    </form>
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.1.3/dist/js/bootstrap.bundle.min.js"></script>
</body>
</html>
