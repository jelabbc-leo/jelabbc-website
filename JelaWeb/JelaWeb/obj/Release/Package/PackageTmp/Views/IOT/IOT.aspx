<%@ Page Title="Gestión IOT" Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/Jela.Master" CodeBehind="IOT.aspx.vb" Inherits="JelaWeb.IOT" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="iot-container">
        <div class="iot-header">
            <h1><i class="fas fa-network-wired"></i> Gestión IOT</h1>
            <p class="iot-subtitle">Seleccione el módulo de IOT que desea gestionar</p>
        </div>

        <div class="iot-cards-grid">
            <!-- Tarjeta IOT Home -->
            <div class="iot-card" onclick="navigateToIOT('Home')">
                <div class="iot-card-icon">
                    <i class="fas fa-home"></i>
                </div>
                <div class="iot-card-content">
                    <h3 class="iot-card-title">IOT Home</h3>
                    <p class="iot-card-description">Gestión de dispositivos IOT para hogares inteligentes</p>
                </div>
                <div class="iot-card-arrow">
                    <i class="fas fa-chevron-right"></i>
                </div>
            </div>

            <!-- Tarjeta IOT Condominio -->
            <div class="iot-card" onclick="navigateToIOT('Condominio')">
                <div class="iot-card-icon">
                    <i class="fas fa-building"></i>
                </div>
                <div class="iot-card-content">
                    <h3 class="iot-card-title">IOT Condominio</h3>
                    <p class="iot-card-description">Administración de sistemas IOT para condominios</p>
                </div>
                <div class="iot-card-arrow">
                    <i class="fas fa-chevron-right"></i>
                </div>
            </div>

            <!-- Tarjeta IOT Invernadero -->
            <div class="iot-card" onclick="navigateToIOT('Invernadero')">
                <div class="iot-card-icon">
                    <i class="fas fa-seedling"></i>
                </div>
                <div class="iot-card-content">
                    <h3 class="iot-card-title">IOT Invernadero</h3>
                    <p class="iot-card-description">Control y monitoreo de sistemas IOT para invernaderos</p>
                </div>
                <div class="iot-card-arrow">
                    <i class="fas fa-chevron-right"></i>
                </div>
            </div>
        </div>
    </div>

    <script src="<%= ResolveUrl("~/Scripts/app/IOT/iot.js") %>"></script>
</asp:Content>

