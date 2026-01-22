<%@ Page Title="Vista Previa de Formulario" Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/Jela.Master" CodeBehind="FormularioVistaPrevia.aspx.vb" Inherits="JelaWeb.FormularioVistaPrevia" %>

<%@ Register Assembly="DevExpress.Web.v22.2, Version=22.2.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web" TagPrefix="dx" %>

<asp:Content ID="ContentHead" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <link href="/Content/CSS/formulario-vistaprevia.css" rel="stylesheet" type="text/css" />

    <!-- Hidden Fields -->
    <asp:HiddenField ID="hfFormularioId" runat="server" Value="0" />
    <asp:HiddenField ID="hfCamposJSON" runat="server" Value="[]" />
    <asp:HiddenField ID="hfPlataformas" runat="server" Value="web,movil" />

    <!-- HEADER -->
    <div class="preview-header">
        <div class="header-left">
            <dx:ASPxButton ID="btnVolver" runat="server" Text="Volver al Diseñador" Theme="Office2010Blue" AutoPostBack="False">
                <Image IconID="arrows_prev_16x16"></Image>
                <ClientSideEvents Click="function(s,e) { history.back(); }" />
            </dx:ASPxButton>
            <span class="header-title">
                <i class="fas fa-eye"></i>
                Vista Previa: <asp:Label ID="lblNombreFormulario" runat="server" Text="Formulario" />
            </span>
        </div>
        <div class="header-center">
            <!-- Selector de plataforma -->
            <div class="platform-selector">
                <button type="button" id="btnWeb" class="platform-btn active" onclick="cambiarPlataforma('web')" title="Vista Web">
                    <i class="fas fa-desktop"></i> Web
                </button>
                <button type="button" id="btnMovil" class="platform-btn" onclick="cambiarPlataforma('movil')" title="Vista Móvil">
                    <i class="fas fa-mobile-alt"></i> Móvil
                </button>
            </div>
        </div>
        <div class="header-right">
            <dx:ASPxButton ID="btnEditar" runat="server" Text="Editar Formulario" Theme="Office2010Blue" AutoPostBack="False">
                <Image IconID="edit_edit_16x16"></Image>
                <ClientSideEvents Click="function(s,e) { 
                    var id = document.getElementById(hfFormularioIdClientID).value;
                    window.location.href = 'FormularioDisenador.aspx?id=' + id;
                }" />
            </dx:ASPxButton>
        </div>
    </div>

    <!-- CONTENEDOR PRINCIPAL -->
    <div class="preview-container">
        
        <!-- VISTA WEB -->
        <div id="webPreview" class="preview-web">
            <div class="web-frame">
                <div class="web-browser-bar">
                    <div class="browser-dots">
                        <span class="dot red"></span>
                        <span class="dot yellow"></span>
                        <span class="dot green"></span>
                    </div>
                    <div class="browser-url">
                        <i class="fas fa-lock"></i>
                        <span>https://app.jela.com/formularios/</span>
                        <asp:Label ID="lblUrlWeb" runat="server" Text="formulario" CssClass="url-name" />
                    </div>
                </div>
                <div class="web-content" id="webFormContent">
                    <!-- Aquí se renderiza el formulario web -->
                </div>
            </div>
        </div>

        <!-- VISTA MÓVIL -->
        <div id="movilPreview" class="preview-movil" style="display: none;">
            <div class="phone-frame">
                <div class="phone-notch"></div>
                <div class="phone-speaker"></div>
                <div class="phone-screen">
                    <div class="phone-status-bar">
                        <span class="time">9:41</span>
                        <div class="status-icons">
                            <i class="fas fa-signal"></i>
                            <i class="fas fa-wifi"></i>
                            <i class="fas fa-battery-full"></i>
                        </div>
                    </div>
                    <div class="phone-header">
                        <i class="fas fa-arrow-left"></i>
                        <span class="phone-title"><asp:Label ID="lblTituloMovil" runat="server" Text="Formulario" /></span>
                        <i class="fas fa-ellipsis-v"></i>
                    </div>
                    <div class="phone-content" id="movilFormContent">
                        <!-- Aquí se renderiza el formulario móvil -->
                    </div>
                    <div class="phone-home-indicator"></div>
                </div>
                <div class="phone-button"></div>
            </div>
        </div>

    </div>

    <script src="/Scripts/app/Catalogos/formulario-vistaprevia.js" type="text/javascript"></script>
</asp:Content>
