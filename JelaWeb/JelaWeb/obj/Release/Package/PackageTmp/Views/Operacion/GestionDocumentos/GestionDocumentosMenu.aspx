<%@ Page Title="Gestión de Documentos" Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/Jela.Master" CodeBehind="GestionDocumentosMenu.aspx.vb" Inherits="JelaWeb.GestionDocumentosMenu" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="documentos-container">
        <div class="documentos-header">
            <h1><i class="fas fa-folder-open"></i> Gestión de Documentos</h1>
            <p class="documentos-subtitle">Seleccione el módulo de gestión de documentos que desea administrar</p>
        </div>

        <div class="documentos-cards-grid">
            <!-- Tarjeta Condominios -->
            <div class="documentos-card" onclick="navigateToDocumentos('Condominios')">
                <div class="documentos-card-icon">
                    <i class="fas fa-building"></i>
                </div>
                <div class="documentos-card-content">
                    <h3 class="documentos-card-title">Condominios</h3>
                    <p class="documentos-card-description">Gestión y administración de documentos para condominios</p>
                </div>
                <div class="documentos-card-arrow">
                    <i class="fas fa-chevron-right"></i>
                </div>
            </div>

            <!-- Tarjeta Servicios Municipales -->
            <div class="documentos-card" onclick="navigateToDocumentos('ServiciosMunicipales')">
                <div class="documentos-card-icon">
                    <i class="fas fa-city"></i>
                </div>
                <div class="documentos-card-content">
                    <h3 class="documentos-card-title">Servicios Municipales</h3>
                    <p class="documentos-card-description">Administración de documentos para servicios municipales</p>
                </div>
                <div class="documentos-card-arrow">
                    <i class="fas fa-chevron-right"></i>
                </div>
            </div>

            <!-- Tarjeta Agro -->
            <div class="documentos-card" onclick="navigateToDocumentos('Agro')">
                <div class="documentos-card-icon">
                    <i class="fas fa-tractor"></i>
                </div>
                <div class="documentos-card-content">
                    <h3 class="documentos-card-title">Agro</h3>
                    <p class="documentos-card-description">Gestión de documentos para el sector agrícola</p>
                </div>
                <div class="documentos-card-arrow">
                    <i class="fas fa-chevron-right"></i>
                </div>
            </div>
        </div>
    </div>

    <script src="<%= ResolveUrl("~/Scripts/app/Operacion/gestiondocumentosmenu.js") %>"></script>
</asp:Content>

