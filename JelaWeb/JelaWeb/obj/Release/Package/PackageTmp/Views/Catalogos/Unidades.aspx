<%@ Page Title="Unidades Privativas" Language="vb" MasterPageFile="~/MasterPages/Jela.Master" AutoEventWireup="false" CodeBehind="Unidades.aspx.vb" Inherits="JelaWeb.Unidades" %>

<%@ Register Assembly="DevExpress.Web.v22.2, Version=22.2.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web" TagPrefix="dx" %>
<%@ Import Namespace="System.Configuration" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="<%= ResolveUrl("~/Content/CSS/unidades.css") %>" rel="stylesheet" type="text/css" />
    <link href="<%= ResolveUrl("~/Content/CSS/residentes.css") %>" rel="stylesheet" type="text/css" />
    <!-- Google Maps API -->
    <% 
        Dim googleMapsKey As String = ConfigurationManager.AppSettings("GoogleMapsApiKey")
        If String.IsNullOrWhiteSpace(googleMapsKey) Then
            googleMapsKey = String.Empty
        End If
    %>
    <!-- Google Maps API se cargará después del contenedor, como en frmOperacion.aspx -->
    <script src="<%= ResolveUrl("~/Scripts/app/Catalogos/unidades.js") %>" type="text/javascript"></script>
    
    <!-- CSS específico para el mapa - Simplificado como frmOperacion.aspx -->
    <style>
        #mapaUnidades {
            width: 100%;
            height: 100%;
            min-height: 300px;
            border: 1px solid #ccc;
            background-color: #e5e3df;
        }
        
        /* Asegurar que el contenedor del splitter tenga altura */
        .dxsplPane {
            overflow: hidden !important;
        }
        
        /* Estilos para el InfoWindow de Google Maps */
        .gm-style .gm-style-iw-c {
            background-color: #E4EFFA !important;
            padding: 0 !important;
            border-radius: 8px !important;
            max-width: 420px !important;
            min-width: 350px !important;
            max-height: none !important;
        }
        
        .gm-style .gm-style-iw-d {
            overflow: visible !important;
            max-height: none !important;
            height: auto !important;
        }
        
        .gm-style .gm-style-iw {
            max-height: none !important;
        }
        
        .gm-style .gm-style-iw-t::after {
            background: #E4EFFA !important;
        }
        
        /* Botón de cerrar del InfoWindow */
        .gm-style .gm-ui-hover-effect {
            top: 8px !important;
            right: 8px !important;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="container-fluid" style="height: calc(100vh - 120px); position: relative;">
        <!-- Splitter para dividir la página horizontalmente -->
        <dx:ASPxSplitter ID="splitterMain" runat="server" ClientInstanceName="splitterMain"
            Width="100%" Orientation="Vertical" FullscreenMode="False"
            Theme="Office2010Blue">
            <ClientSideEvents
                PaneResized="function(s, e) { 
                    if (mapaUnidades && typeof google !== 'undefined' && google.maps) {
                        google.maps.event.trigger(mapaUnidades, 'resize');
                    }
                }"
                Init="function(s, e) { 
                    var height = window.innerHeight - 150;
                    if (window.innerWidth <= 768) {
                        // En móviles, ajustar altura y layout
                        height = window.innerHeight - 120;
                        // Forzar paneles a ancho completo en móviles
                        var paneGrid = s.GetPaneByName('paneGrid');
                        var paneMapa = s.GetPaneByName('paneMapa');
                        if (paneGrid && paneMapa) {
                            if (window.innerWidth <= 768) {
                                paneGrid.SetSize('40%');
                                paneMapa.SetSize('60%');
                            }
                        }
                    }
                    s.SetHeight(height); 
                }" />
            <Panes>
                <%-- Panel Superior: Grid de Unidades --%>
                <dx:SplitterPane Name="paneGrid" Size="50%" MinSize="200px">
                    <ContentCollection>
                        <dx:SplitterContentControl runat="server">
                            <div style="height: 100%; display: flex; flex-direction: column;">
                                <!-- CallbackPanel para manejar callbacks correctamente (como en Entidades.aspx) -->
                                <dx:ASPxCallbackPanel ID="gridCallback" runat="server" ClientInstanceName="gridCallback"
                                    OnCallback="gridCallback_Callback">
                                    <PanelCollection>
                                        <dx:PanelContent runat="server">
                                            <!-- GRID PRINCIPAL DE UNIDADES -->
                                            <dx:ASPxGridView ID="gridUnidades" runat="server" ClientInstanceName="gridUnidades"
                                                Width="100%" KeyFieldName="Id" AutoGenerateColumns="False" Theme="Office2010Blue"
                                                OnDataBound="gridUnidades_DataBound" EnableCallBacks="True">

                                    <SettingsDataSecurity AllowReadUnlistedFieldsFromClientApi="True" />
                                    <SettingsAdaptivity AdaptivityMode="HideDataCells" AllowOnlyOneAdaptiveDetailExpanded="True" />
                                    <Settings HorizontalScrollBarMode="Auto" />
                                    <SettingsLoadingPanel Mode="ShowAsPopup" />

                                    <ClientSideEvents
                                        BeginCallback="function(s, e) { console.log('Grid BeginCallback'); }"
                                        ToolbarItemClick="function(s, e) { if (typeof window.onToolbarUnidadesClick === 'function') window.onToolbarUnidadesClick(s, e); }"
                                        RowDblClick="function(s, e) { if (typeof window.onRowDblClick === 'function') window.onRowDblClick(s, e); }"
                                        CustomButtonClick="function(s, e) { if (typeof window.onGridCustomButtonClick === 'function') window.onGridCustomButtonClick(s, e); }"
                                        FocusedRowChanged="function(s, e) { if (typeof window.onUnidadFocusedRowChanged === 'function') window.onUnidadFocusedRowChanged(s, e); }" />

                                    <SettingsPager Mode="ShowAllRecords" />
                                    <Settings ShowFilterRow="False" ShowFilterRowMenu="True" ShowGroupPanel="True" ShowHeaderFilterButton="True" />
                                    <SettingsPopup>
                                        <FilterControl AutoUpdatePosition="False"></FilterControl>
                                    </SettingsPopup>
                                    <SettingsSearchPanel Visible="True" ShowApplyButton="True" ShowClearButton="True" />
                                    <SettingsBehavior AllowSort="True" AllowGroup="True" AllowFixedGroups="True" AllowFocusedRow="True" AllowSelectByRowClick="True" />
                                    <SettingsExport EnableClientSideExportAPI="True" ExcelExportMode="WYSIWYG" FileName="Unidades" />
                                    <Settings VerticalScrollBarMode="Auto" />

                                    <Toolbars>
                                        <dx:GridViewToolbar>
                                            <SettingsAdaptivity Enabled="True" EnableCollapseRootItemsToIcons="True" />
                                            <Items>
                                                <dx:GridViewToolbarItem Name="btnNuevo" Text="Nueva Unidad">
                                                    <Image IconID="actions_add_16x16" />
                                                </dx:GridViewToolbarItem>
                                                <dx:GridViewToolbarItem Name="btnEditar" Text="Administrar">
                                                    <Image IconID="edit_edit_16x16" />
                                                </dx:GridViewToolbarItem>
                                                <dx:GridViewToolbarItem Name="btnEliminar" Text="Eliminar" BeginGroup="True">
                                                    <Image IconID="actions_cancel_16x16" />
                                                </dx:GridViewToolbarItem>
                                                <dx:GridViewToolbarItem BeginGroup="True" Command="Refresh" />
                                                <dx:GridViewToolbarItem Command="ExportToXlsx" Text="Exportar Excel" />
                                            </Items>
                                        </dx:GridViewToolbar>
                                    </Toolbars>

                                    <Columns>
                                    </Columns>
                                        </dx:ASPxGridView>
                                        </dx:PanelContent>
                                    </PanelCollection>
                                </dx:ASPxCallbackPanel>
                            </div>
                        </dx:SplitterContentControl>
                    </ContentCollection>
                </dx:SplitterPane>

                <%-- Panel Inferior: Mapa --%>
                <dx:SplitterPane Name="paneMapa" Size="50%" MinSize="200px">
                    <ContentCollection>
                        <dx:SplitterContentControl runat="server">
                            <div style="height: 100%; width: 100%; padding: 10px; position: relative; box-sizing: border-box;">
                                <!-- Contenedor del mapa de Google Maps (API tradicional) -->
                                <div id="mapaUnidades" style="width: 100%; height: 100%; min-height: 300px;"></div>
                                <div id="mapaLoading" style="position: absolute; top: 50%; left: 50%; transform: translate(-50%, -50%); z-index: 1000; background: white; padding: 10px; border-radius: 4px; box-shadow: 0 2px 8px rgba(0,0,0,0.2); pointer-events: none;">
                                    <span>Cargando mapa...</span>
                                </div>
                            </div>
                        </dx:SplitterContentControl>
                    </ContentCollection>
                </dx:SplitterPane>
            </Panes>
        </dx:ASPxSplitter>
        <dx:ASPxGridViewExporter ID="gridExporter" runat="server" GridViewID="gridUnidades" />
    </div>

    <!-- ================================================================== -->
    <!-- POPUP PRINCIPAL: ADMINISTRAR UNIDAD PRIVATIVA -->
    <!-- ================================================================== -->
    <dx:ASPxPopupControl ID="popupUnidad" runat="server" ClientInstanceName="popupUnidad"
        HeaderText="Administrar Unidad Privativa" Width="1100px" Modal="True"
        PopupHorizontalAlign="WindowCenter" PopupVerticalAlign="WindowCenter"
        CloseAction="CloseButton" AllowDragging="True" CloseOnEscape="True">
        <ClientSideEvents 
            Shown="function(s, e) {
                console.log('🔧 Popup Shown event'); 
                // Ajustar tamaño responsivo en móviles
                if (window.innerWidth <= 768) {
                    var width = window.innerWidth <= 480 ? window.innerWidth : window.innerWidth * 0.95;
                    s.SetWidth(width);
                    if (window.innerWidth <= 480) {
                        s.SetHeight(window.innerHeight);
                    }
                }
                inicializarGridsPopup(); 
            }" 
            Closing="function(s, e) { console.log('🔧 Popup Closing event', e); }"
            CloseUp="function(s, e) { console.log('🔧 Popup CloseUp event'); onPopupUnidadClosed(); }" />
        <ContentCollection>
            <dx:PopupControlContentControl runat="server">
                <dx:ASPxPageControl ID="tabsUnidad" runat="server" ClientInstanceName="tabsUnidad"
                    Width="100%" Theme="Office2010Blue" ActiveTabIndex="0">
                    <ClientSideEvents ActiveTabChanged="function(s, e) { inicializarGridsPopup(); }" />
                    <TabPages>
                        <dx:TabPage Text="Datos Generales" Name="tabDatosGenerales">
                            <ContentCollection>
                                <dx:ContentControl runat="server">
                                    <dx:ASPxFormLayout ID="formUnidad" runat="server" Width="100%" ColCount="2" Theme="Office2010Blue">
                                        <Items>
                                            <!-- Campo Entidad eliminado - El sistema usa IdEntidadActual automáticamente -->
                                            <dx:LayoutItem Caption="Sub-Entidad">
                                                <LayoutItemNestedControlCollection>
                                                    <dx:LayoutItemNestedControlContainer runat="server">
                                                        <dx:ASPxComboBox ID="cboSubEntidad" runat="server" ClientInstanceName="cboSubEntidad"
                                                            Width="100%" Theme="Office2010Blue" ValueType="System.Int32"
                                                            DropDownRows="15" DropDownHeight="400px" DropDownStyle="DropDown" />
                                                    </dx:LayoutItemNestedControlContainer>
                                                </LayoutItemNestedControlCollection>
                                            </dx:LayoutItem>
                                            <dx:LayoutItem Caption="Código" RequiredMarkDisplayMode="Required">
                                                <LayoutItemNestedControlCollection>
                                                    <dx:LayoutItemNestedControlContainer runat="server">
                                                        <dx:ASPxTextBox ID="txtCodigo" runat="server" ClientInstanceName="txtCodigo"
                                                            Width="100%" Theme="Office2010Blue" MaxLength="20">
                                                            <ValidationSettings>
                                                                <RequiredField IsRequired="True" ErrorText="El código es requerido" />
                                                            </ValidationSettings>
                                                        </dx:ASPxTextBox>
                                                    </dx:LayoutItemNestedControlContainer>
                                                </LayoutItemNestedControlCollection>
                                            </dx:LayoutItem>
                                            <dx:LayoutItem Caption="Nombre" RequiredMarkDisplayMode="Required">
                                                <LayoutItemNestedControlCollection>
                                                    <dx:LayoutItemNestedControlContainer runat="server">
                                                        <dx:ASPxTextBox ID="txtNombre" runat="server" ClientInstanceName="txtNombre"
                                                            Width="100%" Theme="Office2010Blue" MaxLength="100">
                                                            <ValidationSettings>
                                                                <RequiredField IsRequired="True" ErrorText="El nombre es requerido" />
                                                            </ValidationSettings>
                                                        </dx:ASPxTextBox>
                                                    </dx:LayoutItemNestedControlContainer>
                                                </LayoutItemNestedControlCollection>
                                            </dx:LayoutItem>
                                            <dx:LayoutGroup Caption="Ubicación en Condominio" ColSpan="2" ColCount="4">
                                                <Items>
                                                    <dx:LayoutItem Caption="Torre">
                                                        <LayoutItemNestedControlCollection>
                                                            <dx:LayoutItemNestedControlContainer runat="server">
                                                                <dx:ASPxTextBox ID="txtTorre" runat="server" ClientInstanceName="txtTorre"
                                                                    Width="100%" Theme="Office2010Blue" MaxLength="50" />
                                                            </dx:LayoutItemNestedControlContainer>
                                                        </LayoutItemNestedControlCollection>
                                                    </dx:LayoutItem>
                                                    <dx:LayoutItem Caption="Edificio">
                                                        <LayoutItemNestedControlCollection>
                                                            <dx:LayoutItemNestedControlContainer runat="server">
                                                                <dx:ASPxTextBox ID="txtEdificio" runat="server" ClientInstanceName="txtEdificio"
                                                                    Width="100%" Theme="Office2010Blue" MaxLength="50" />
                                                            </dx:LayoutItemNestedControlContainer>
                                                        </LayoutItemNestedControlCollection>
                                                    </dx:LayoutItem>
                                                    <dx:LayoutItem Caption="Piso">
                                                        <LayoutItemNestedControlCollection>
                                                            <dx:LayoutItemNestedControlContainer runat="server">
                                                                <dx:ASPxTextBox ID="txtPiso" runat="server" ClientInstanceName="txtPiso"
                                                                    Width="100%" Theme="Office2010Blue" MaxLength="10" />
                                                            </dx:LayoutItemNestedControlContainer>
                                                        </LayoutItemNestedControlCollection>
                                                    </dx:LayoutItem>
                                                    <dx:LayoutItem Caption="Número">
                                                        <LayoutItemNestedControlCollection>
                                                            <dx:LayoutItemNestedControlContainer runat="server">
                                                                <dx:ASPxTextBox ID="txtNumero" runat="server" ClientInstanceName="txtNumero"
                                                                    Width="100%" Theme="Office2010Blue" MaxLength="20" />
                                                            </dx:LayoutItemNestedControlContainer>
                                                        </LayoutItemNestedControlCollection>
                                                    </dx:LayoutItem>
                                                </Items>
                                            </dx:LayoutGroup>
                                            <dx:LayoutGroup Caption="Dirección Postal" ColSpan="2" ColCount="3">
                                                <Items>
                                                    <dx:LayoutItem Caption="Calle" ColSpan="3">
                                                        <LayoutItemNestedControlCollection>
                                                            <dx:LayoutItemNestedControlContainer runat="server">
                                                                <dx:ASPxTextBox ID="txtCalle" runat="server" ClientInstanceName="txtCalle"
                                                                    Width="100%" Theme="Office2010Blue" MaxLength="200" />
                                                            </dx:LayoutItemNestedControlContainer>
                                                        </LayoutItemNestedControlCollection>
                                                    </dx:LayoutItem>
                                                    <dx:LayoutItem Caption="Número Exterior">
                                                        <LayoutItemNestedControlCollection>
                                                            <dx:LayoutItemNestedControlContainer runat="server">
                                                                <dx:ASPxTextBox ID="txtNumeroExterior" runat="server" ClientInstanceName="txtNumeroExterior"
                                                                    Width="100%" Theme="Office2010Blue" MaxLength="20" />
                                                            </dx:LayoutItemNestedControlContainer>
                                                        </LayoutItemNestedControlCollection>
                                                    </dx:LayoutItem>
                                                    <dx:LayoutItem Caption="Número Interior">
                                                        <LayoutItemNestedControlCollection>
                                                            <dx:LayoutItemNestedControlContainer runat="server">
                                                                <dx:ASPxTextBox ID="txtNumeroInterior" runat="server" ClientInstanceName="txtNumeroInterior"
                                                                    Width="100%" Theme="Office2010Blue" MaxLength="20" />
                                                            </dx:LayoutItemNestedControlContainer>
                                                        </LayoutItemNestedControlCollection>
                                                    </dx:LayoutItem>
                                                    <dx:LayoutItem Caption="Referencia" ColSpan="3">
                                                        <LayoutItemNestedControlCollection>
                                                            <dx:LayoutItemNestedControlContainer runat="server">
                                                                <dx:ASPxTextBox ID="txtReferencia" runat="server" ClientInstanceName="txtReferencia"
                                                                    Width="100%" Theme="Office2010Blue" MaxLength="500" />
                                                            </dx:LayoutItemNestedControlContainer>
                                                        </LayoutItemNestedControlCollection>
                                                    </dx:LayoutItem>
                                                </Items>
                                            </dx:LayoutGroup>
                                            <dx:LayoutGroup Caption="Coordenadas" ColSpan="2" ColCount="2">
                                                <Items>
                                                    <dx:LayoutItem Caption="Latitud">
                                                        <LayoutItemNestedControlCollection>
                                                            <dx:LayoutItemNestedControlContainer runat="server">
                                                                <dx:ASPxSpinEdit ID="txtLatitud" runat="server" ClientInstanceName="txtLatitud"
                                                                    Width="100%" Theme="Office2010Blue" NumberType="Float" DecimalPlaces="6"
                                                                    MinValue="-90" MaxValue="90" />
                                                            </dx:LayoutItemNestedControlContainer>
                                                        </LayoutItemNestedControlCollection>
                                                    </dx:LayoutItem>
                                                    <dx:LayoutItem Caption="Longitud">
                                                        <LayoutItemNestedControlCollection>
                                                            <dx:LayoutItemNestedControlContainer runat="server">
                                                                <dx:ASPxSpinEdit ID="txtLongitud" runat="server" ClientInstanceName="txtLongitud"
                                                                    Width="100%" Theme="Office2010Blue" NumberType="Float" DecimalPlaces="6"
                                                                    MinValue="-180" MaxValue="180" />
                                                            </dx:LayoutItemNestedControlContainer>
                                                        </LayoutItemNestedControlCollection>
                                                    </dx:LayoutItem>
                                                </Items>
                                            </dx:LayoutGroup>
                                            <dx:LayoutItem Caption="Superficie (m²)">
                                                <LayoutItemNestedControlCollection>
                                                    <dx:LayoutItemNestedControlContainer runat="server">
                                                        <dx:ASPxSpinEdit ID="txtSuperficie" runat="server" ClientInstanceName="txtSuperficie"
                                                            Width="100%" Theme="Office2010Blue" NumberType="Float" DecimalPlaces="2"
                                                            MinValue="0" MaxValue="99999.99" />
                                                    </dx:LayoutItemNestedControlContainer>
                                                </LayoutItemNestedControlCollection>
                                            </dx:LayoutItem>
                                            <dx:LayoutItem Caption="Activo">
                                                <LayoutItemNestedControlCollection>
                                                    <dx:LayoutItemNestedControlContainer runat="server">
                                                        <dx:ASPxCheckBox ID="chkActivo" runat="server" ClientInstanceName="chkActivo"
                                                            Text="Unidad activa" Checked="True" Theme="Office2010Blue" />
                                                    </dx:LayoutItemNestedControlContainer>
                                                </LayoutItemNestedControlCollection>
                                            </dx:LayoutItem>
                                            <dx:LayoutItem Caption="Descripción" ColSpan="2">
                                                <LayoutItemNestedControlCollection>
                                                    <dx:LayoutItemNestedControlContainer runat="server">
                                                        <dx:ASPxMemo ID="txtDescripcion" runat="server" ClientInstanceName="txtDescripcion"
                                                            Width="100%" Theme="Office2010Blue" />
                                                    </dx:LayoutItemNestedControlContainer>
                                                </LayoutItemNestedControlCollection>
                                            </dx:LayoutItem>
                                        </Items>
                                    </dx:ASPxFormLayout>
                                </dx:ContentControl>
                            </ContentCollection>
                        </dx:TabPage>
                        <dx:TabPage Text="Residentes" Name="tabResidentes">
                            <ContentCollection>
                                <dx:ContentControl runat="server">
                                    <dx:ASPxGridView ID="gridResidentes" runat="server" ClientInstanceName="gridResidentes"
                                        Width="100%" KeyFieldName="Id" AutoGenerateColumns="False" Theme="Office2010Blue"
                                        OnCustomCallback="gridResidentes_CustomCallback" OnDataBound="gridResidentes_DataBound"
                                        EnableCallBacks="True"
                                        Settings-VerticalScrollBarMode="Auto">
                                        <ClientSideEvents ToolbarItemClick="function(s, e) { if (typeof window.onToolbarResidentesClick === 'function') window.onToolbarResidentesClick(s, e); }" />
                                        <SettingsPager Mode="ShowAllRecords" />
                                        <Settings ShowFilterRow="False" ShowFilterRowMenu="True" ShowGroupPanel="False" />
                                        <SettingsPopup>
                                            <FilterControl AutoUpdatePosition="False"></FilterControl>
                                        </SettingsPopup>
                                        <SettingsSearchPanel Visible="True" ShowApplyButton="True" ShowClearButton="True" />
                                        <SettingsBehavior AllowSort="True" AllowGroup="False" AllowFocusedRow="True" AllowSelectByRowClick="True" />
                                        <Toolbars>
                                            <dx:GridViewToolbar>
                                                <Items>
                                                    <dx:GridViewToolbarItem Name="btnNuevoResidente" Text="Agregar Residente">
                                                        <Image IconID="actions_add_16x16" />
                                                    </dx:GridViewToolbarItem>
                                                    <dx:GridViewToolbarItem Name="btnEditarResidente" Text="Editar">
                                                        <Image IconID="edit_edit_16x16" />
                                                    </dx:GridViewToolbarItem>
                                                    <dx:GridViewToolbarItem Name="btnEliminarResidente" Text="Eliminar">
                                                        <Image IconID="actions_cancel_16x16" />
                                                    </dx:GridViewToolbarItem>
                                                </Items>
                                            </dx:GridViewToolbar>
                                        </Toolbars>
                                    </dx:ASPxGridView>
                                </dx:ContentControl>
                            </ContentCollection>
                        </dx:TabPage>
                        <dx:TabPage Text="Vehículos" Name="tabVehiculos">
                            <ContentCollection>
                                <dx:ContentControl runat="server">
                                    <dx:ASPxGridView ID="gridVehiculos" runat="server" ClientInstanceName="gridVehiculos"
                                        Width="100%" KeyFieldName="Id" AutoGenerateColumns="False" Theme="Office2010Blue"
                                        OnCustomCallback="gridVehiculos_CustomCallback" OnDataBound="gridVehiculos_DataBound"
                                        EnableCallBacks="True"
                                        Settings-VerticalScrollBarMode="Auto">
                                        <ClientSideEvents ToolbarItemClick="function(s, e) { if (typeof window.onToolbarVehiculosClick === 'function') window.onToolbarVehiculosClick(s, e); }" />
                                        <SettingsPager Mode="ShowAllRecords" />
                                        <Settings ShowFilterRow="False" ShowFilterRowMenu="True" ShowGroupPanel="False" />
                                        <SettingsPopup>
                                            <FilterControl AutoUpdatePosition="False"></FilterControl>
                                        </SettingsPopup>
                                        <SettingsSearchPanel Visible="True" ShowApplyButton="True" ShowClearButton="True" />
                                        <SettingsBehavior AllowSort="True" AllowGroup="False" AllowFocusedRow="True" AllowSelectByRowClick="True" />
                                        <Toolbars>
                                            <dx:GridViewToolbar>
                                                <Items>
                                                    <dx:GridViewToolbarItem Name="btnNuevoVehiculo" Text="Agregar Vehículo">
                                                        <Image IconID="actions_add_16x16" />
                                                    </dx:GridViewToolbarItem>
                                                    <dx:GridViewToolbarItem Name="btnEditarVehiculo" Text="Editar">
                                                        <Image IconID="edit_edit_16x16" />
                                                    </dx:GridViewToolbarItem>
                                                    <dx:GridViewToolbarItem Name="btnEliminarVehiculo" Text="Eliminar">
                                                        <Image IconID="actions_cancel_16x16" />
                                                    </dx:GridViewToolbarItem>
                                                </Items>
                                            </dx:GridViewToolbar>
                                        </Toolbars>
                                    </dx:ASPxGridView>
                                </dx:ContentControl>
                            </ContentCollection>
                        </dx:TabPage>
                        <dx:TabPage Text="Tags Acceso" Name="tabTags">
                            <ContentCollection>
                                <dx:ContentControl runat="server">
                                    <dx:ASPxGridView ID="gridTags" runat="server" ClientInstanceName="gridTags"
                                        Width="100%" KeyFieldName="Id" AutoGenerateColumns="False" Theme="Office2010Blue"
                                        OnCustomCallback="gridTags_CustomCallback" OnDataBound="gridTags_DataBound"
                                        EnableCallBacks="True"
                                        Settings-VerticalScrollBarMode="Auto">
                                        <ClientSideEvents ToolbarItemClick="function(s, e) { if (typeof window.onToolbarTagsClick === 'function') window.onToolbarTagsClick(s, e); }" />
                                        <SettingsPager Mode="ShowAllRecords" />
                                        <Settings ShowFilterRow="False" ShowFilterRowMenu="True" ShowGroupPanel="False" />
                                        <SettingsPopup>
                                            <FilterControl AutoUpdatePosition="False"></FilterControl>
                                        </SettingsPopup>
                                        <SettingsSearchPanel Visible="True" ShowApplyButton="True" ShowClearButton="True" />
                                        <SettingsBehavior AllowSort="True" AllowGroup="False" AllowFocusedRow="True" AllowSelectByRowClick="True" />
                                        <Toolbars>
                                            <dx:GridViewToolbar>
                                                <Items>
                                                    <dx:GridViewToolbarItem Name="btnNuevoTag" Text="Agregar Tag">
                                                        <Image IconID="actions_add_16x16" />
                                                    </dx:GridViewToolbarItem>
                                                    <dx:GridViewToolbarItem Name="btnEditarTag" Text="Editar">
                                                        <Image IconID="edit_edit_16x16" />
                                                    </dx:GridViewToolbarItem>
                                                    <dx:GridViewToolbarItem Name="btnEliminarTag" Text="Eliminar">
                                                        <Image IconID="actions_cancel_16x16" />
                                                    </dx:GridViewToolbarItem>
                                                </Items>
                                            </dx:GridViewToolbar>
                                        </Toolbars>
                                    </dx:ASPxGridView>
                                </dx:ContentControl>
                            </ContentCollection>
                        </dx:TabPage>
                        <dx:TabPage Text="Documentos" Name="tabDocumentos">
                            <ContentCollection>
                                <dx:ContentControl runat="server">
                                    <dx:ASPxGridView ID="gridDocumentos" runat="server" ClientInstanceName="gridDocumentos"
                                        Width="100%" KeyFieldName="Id" AutoGenerateColumns="False" Theme="Office2010Blue"
                                        OnCustomCallback="gridDocumentos_CustomCallback" OnDataBound="gridDocumentos_DataBound"
                                        EnableCallBacks="True"
                                        Settings-VerticalScrollBarMode="Auto">
                                        <ClientSideEvents ToolbarItemClick="function(s, e) { if (typeof window.onToolbarDocumentosClick === 'function') window.onToolbarDocumentosClick(s, e); }" />
                                        <SettingsPager Mode="ShowAllRecords" />
                                        <Settings ShowFilterRow="False" ShowFilterRowMenu="True" ShowGroupPanel="False" />
                                        <SettingsPopup>
                                            <FilterControl AutoUpdatePosition="False"></FilterControl>
                                        </SettingsPopup>
                                        <SettingsSearchPanel Visible="True" ShowApplyButton="True" ShowClearButton="True" />
                                        <SettingsBehavior AllowSort="True" AllowGroup="False" AllowFocusedRow="True" AllowSelectByRowClick="True" />
                                        <Toolbars>
                                            <dx:GridViewToolbar>
                                                <Items>
                                                    <dx:GridViewToolbarItem Name="btnNuevoDocumento" Text="Subir Documento">
                                                        <Image IconID="actions_add_16x16" />
                                                    </dx:GridViewToolbarItem>
                                                    <dx:GridViewToolbarItem Name="btnEditarDocumento" Text="Editar Documento">
                                                        <Image IconID="edit_edit_16x16" />
                                                    </dx:GridViewToolbarItem>
                                                    <dx:GridViewToolbarItem Name="btnVerDocumento" Text="Ver/Descargar">
                                                        <Image IconID="zoom_zoom_16x16" />
                                                    </dx:GridViewToolbarItem>
                                                    <dx:GridViewToolbarItem Name="btnEliminarDocumento" Text="Eliminar">
                                                        <Image IconID="actions_cancel_16x16" />
                                                    </dx:GridViewToolbarItem>
                                                </Items>
                                            </dx:GridViewToolbar>
                                        </Toolbars>
                                    </dx:ASPxGridView>
                                </dx:ContentControl>
                            </ContentCollection>
                        </dx:TabPage>
                    </TabPages>
                </dx:ASPxPageControl>

                <div class="popup-footer" style="margin-top: 15px; text-align: right;">
                    <dx:ASPxButton ID="btnGuardar" runat="server" Text="Guardar Unidad" Theme="Office2010Blue"
                        AutoPostBack="False">
                        <Image IconID="save_save_16x16" />
                        <ClientSideEvents Click="function(s, e) { guardarUnidad(); }" />
                    </dx:ASPxButton>
                    <dx:ASPxButton ID="btnCerrar" runat="server" Text="Cerrar" Theme="Office2010Blue" AutoPostBack="False">
                        <Image IconID="actions_cancel_16x16" />
                        <ClientSideEvents Click="function(s, e) { popupUnidad.Hide(); gridUnidades.Refresh(); }" />
                    </dx:ASPxButton>
                </div>
            </dx:PopupControlContentControl>
        </ContentCollection>
    </dx:ASPxPopupControl>

    <!-- ================================================================== -->
    <!-- POPUP: EDITAR RESIDENTE -->
    <!-- ================================================================== -->
    <dx:ASPxPopupControl ID="popupResidente" runat="server" ClientInstanceName="popupResidente"
        HeaderText="Residente" Width="900px" Height="650px" Modal="True"
        PopupHorizontalAlign="WindowCenter" PopupVerticalAlign="WindowCenter"
        CloseAction="CloseButton" AllowDragging="True" CloseOnEscape="True">
        <ClientSideEvents
            Init="function(s, e) { 
                console.log('popupResidente inicializado'); 
                // Registrar globalmente para acceso seguro
                if (typeof window !== 'undefined') {
                    window.popupResidente = s;
                    if (typeof window.popupsInicializados !== 'undefined') {
                        window.popupsInicializados.popupResidente = true;
                    }
                }
            }"
            Shown="function(s, e) { 
                setTimeout(function() { 
                    if (typeof initINEScanner === 'function') { 
                        initINEScanner(); 
                    }
                    if (typeof initDocumentoFileInput === 'function') {
                    initDocumentoFileInput();
                }
            }, 100); 
        }" />
        <ContentCollection>
            <dx:PopupControlContentControl runat="server">
                <dx:ASPxFormLayout ID="formResidente" runat="server" Width="100%" ColCount="2" Theme="Office2010Blue">
                    <Items>
                        <%-- Indicador de procesamiento visible en todo el popup --%>
                        <dx:LayoutItem Caption="Estado del Procesamiento" ColSpan="2" ShowCaption="False">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <div id="ineProcessingIndicator" style="display: none; padding: 15px; background-color: #e7f3ff; border: 2px solid #007bff; border-radius: 4px; text-align: center; margin-bottom: 15px;">
                                        <i class="fa fa-spinner fa-spin" style="font-size: 24px; color: #007bff; margin-right: 10px;"></i>
                                        <span style="font-size: 14px; color: #007bff; font-weight: bold;">Procesando documento con Azure Document Intelligence... Por favor espere.</span>
                                    </div>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <%-- SUBIR ARCHIVOS INE - Movido al inicio --%>
                        <dx:LayoutItem Caption="Subir Archivos INE" ColSpan="2">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">

                                    <!-- Área para subir múltiples archivos -->
                                    <div style="margin-top: 15px;">
                                        <div id="ineDropZone" style="border: 2px dashed #ccc; padding: 20px; text-align: center; cursor: pointer; background-color: #f9f9f9; border-radius: 4px;">
                                            <i class="fa fa-cloud-upload" style="font-size: 48px; color: #999; margin-bottom: 10px;"></i>
                                            <p style="margin: 0; color: #666;">Haga clic o arrastre archivos aquí (múltiples archivos permitidos)</p>
                                            <p style="margin: 5px 0 0 0; font-size: 12px; color: #999;">Formatos: JPG, PNG, PDF (máx. 10MB cada uno)</p>
                                            <p style="margin: 5px 0 0 0; font-size: 11px; color: #007bff; font-weight: bold;">Los documentos se procesarán automáticamente al subirlos</p>
                                        </div>
                                        <input type="file" id="ineFileInput" accept="image/jpeg,image/png,image/jpg,application/pdf" multiple style="display: none;" />
                                        <div id="ineFilesList" style="margin-top: 10px; display: none;">
                                            <div id="ineFilesPreview" style="display: flex; flex-wrap: wrap; gap: 10px;"></div>
                                        </div>
                                        <div id="ineStatus" style="margin-top: 10px; padding: 8px; border-radius: 4px; display: none;"></div>
                                    </div>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <%-- Grid de archivos INE --%>
                        <dx:LayoutGroup Caption="Archivos INE y Documentos" ColSpan="2">
                            <Items>
                                <dx:LayoutItem ShowCaption="False">
                                    <LayoutItemNestedControlCollection>
                                        <dx:LayoutItemNestedControlContainer runat="server">
                                            <dx:ASPxGridView ID="gridArchivosResidente" runat="server" ClientInstanceName="gridArchivosResidente"
                                                Width="100%" KeyFieldName="Id" AutoGenerateColumns="False" Theme="Office2010Blue"
                                                OnCustomCallback="gridArchivosResidente_CustomCallback" OnDataBound="gridArchivosResidente_DataBound"
                                                EnableCallBacks="True">
                                                <ClientSideEvents ToolbarItemClick="function(s, e) { if (typeof window.onToolbarArchivosResidenteClick === 'function') window.onToolbarArchivosResidenteClick(s, e); }" />
                                                <SettingsPager Mode="ShowAllRecords" />
                                                <Settings ShowHeaderFilterButton="False" ShowFilterRow="False" ShowGroupPanel="False" />
                                                <SettingsBehavior AllowFocusedRow="True" AllowSelectByRowClick="True" AllowSort="True" />

                                                <SettingsPopup>
                                                    <FilterControl AutoUpdatePosition="False"></FilterControl>
                                                </SettingsPopup>
                                                <Toolbars>
                                                    <dx:GridViewToolbar>
                                                        <Items>
                                                            <dx:GridViewToolbarItem Name="btnVerArchivoResidente" Text="Ver Archivo">
                                                                <Image IconID="zoom_zoom_16x16" />
                                                            </dx:GridViewToolbarItem>
                                                            <dx:GridViewToolbarItem Name="btnEliminarArchivoResidente" Text="Eliminar Archivo">
                                                                <Image IconID="edit_delete_16x16" />
                                                            </dx:GridViewToolbarItem>
                                                        </Items>
                                                    </dx:GridViewToolbar>
                                                </Toolbars>
                                                <%-- Las columnas se generan dinámicamente en GenerarColumnasDinamicas --%>
                                            </dx:ASPxGridView>
                                        </dx:LayoutItemNestedControlContainer>
                                    </LayoutItemNestedControlCollection>
                                </dx:LayoutItem>
                            </Items>
                        </dx:LayoutGroup>
                        <dx:LayoutItem Caption="Tipo Residente" RequiredMarkDisplayMode="Required">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxComboBox ID="cboTipoResidente" runat="server" ClientInstanceName="cboTipoResidente"
                                        Width="100%" Theme="Office2010Blue" ValueType="System.String">
                                        <Items>
                                            <dx:ListEditItem Text="Propietario" Value="Propietario" Selected="True" />
                                            <dx:ListEditItem Text="Inquilino" Value="Inquilino" />
                                            <dx:ListEditItem Text="Familiar" Value="Familiar" />
                                            <dx:ListEditItem Text="Empleado" Value="Empleado" />
                                        </Items>
                                    </dx:ASPxComboBox>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Es Principal">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxCheckBox ID="chkResPrincipal" runat="server" ClientInstanceName="chkResPrincipal"
                                        Text="Residente principal de la unidad" Theme="Office2010Blue" />
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Nombre" RequiredMarkDisplayMode="Required">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxTextBox ID="txtResNombre" runat="server" ClientInstanceName="txtResNombre"
                                        Width="100%" Theme="Office2010Blue" MaxLength="100">
                                        <ValidationSettings>
                                            <RequiredField IsRequired="True" ErrorText="Requerido" />
                                        </ValidationSettings>
                                    </dx:ASPxTextBox>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Apellido Paterno" RequiredMarkDisplayMode="Required">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxTextBox ID="txtResApPaterno" runat="server" ClientInstanceName="txtResApPaterno"
                                        Width="100%" Theme="Office2010Blue" MaxLength="100">
                                        <ValidationSettings>
                                            <RequiredField IsRequired="True" ErrorText="Requerido" />
                                        </ValidationSettings>
                                    </dx:ASPxTextBox>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Apellido Materno" RequiredMarkDisplayMode="Required">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxTextBox ID="txtResApMaterno" runat="server" ClientInstanceName="txtResApMaterno"
                                        Width="100%" Theme="Office2010Blue" MaxLength="100">
                                        <ValidationSettings>
                                            <RequiredField IsRequired="True" ErrorText="Requerido" />
                                        </ValidationSettings>
                                    </dx:ASPxTextBox>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Email" RequiredMarkDisplayMode="Required">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxTextBox ID="txtResEmail" runat="server" ClientInstanceName="txtResEmail"
                                        Width="100%" Theme="Office2010Blue" MaxLength="200">
                                        <ValidationSettings>
                                            <RequiredField IsRequired="True" ErrorText="Requerido" />
                                            <RegularExpression ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" ErrorText="Formato de email inválido" />
                                        </ValidationSettings>
                                    </dx:ASPxTextBox>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Teléfono">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxTextBox ID="txtResTelefono" runat="server" ClientInstanceName="txtResTelefono"
                                        Width="100%" Theme="Office2010Blue" MaxLength="20" />
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Celular" RequiredMarkDisplayMode="Required">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxTextBox ID="txtResCelular" runat="server" ClientInstanceName="txtResCelular"
                                        Width="100%" Theme="Office2010Blue" MaxLength="20">
                                        <ValidationSettings>
                                            <RequiredField IsRequired="True" ErrorText="Requerido" />
                                        </ValidationSettings>
                                    </dx:ASPxTextBox>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="CURP">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxTextBox ID="txtResCURP" runat="server" ClientInstanceName="txtResCURP"
                                        Width="100%" Theme="Office2010Blue" MaxLength="18" />
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Activo">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxCheckBox ID="chkResActivo" runat="server" ClientInstanceName="chkResActivo"
                                        Text="Activo" Checked="True" Theme="Office2010Blue" />
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                    </Items>
                </dx:ASPxFormLayout>
                <div style="margin-top: 15px; text-align: right;">
                    <dx:ASPxButton ID="btnGuardarResidente" runat="server" Text="Guardar" Theme="Office2010Blue" AutoPostBack="False">
                        <Image IconID="save_save_16x16" />
                        <ClientSideEvents Click="function(s,e){ guardarResidente(); }" />
                    </dx:ASPxButton>
                    <dx:ASPxButton ID="btnCerrarResidente" runat="server" Text="Cancelar" Theme="Office2010Blue" AutoPostBack="False">
                        <ClientSideEvents Click="function(s,e){ popupResidente.Hide(); }" />
                    </dx:ASPxButton>
                </div>
            </dx:PopupControlContentControl>
        </ContentCollection>
    </dx:ASPxPopupControl>

    <!-- ================================================================== -->
    <!-- POPUP: EDITAR VEHÍCULO -->
    <!-- ================================================================== -->
    <dx:ASPxPopupControl ID="popupVehiculo" runat="server" ClientInstanceName="popupVehiculo"
        HeaderText="Vehículo" Width="900px" Modal="True"
        PopupHorizontalAlign="WindowCenter" PopupVerticalAlign="WindowCenter"
        CloseAction="CloseButton" AllowDragging="True" CloseOnEscape="True">
        <ClientSideEvents
            Init="function(s, e) { 
                console.log('popupVehiculo inicializado'); 
                // Registrar globalmente para acceso seguro
                if (typeof window !== 'undefined') {
                    window.popupVehiculo = s;
                    if (typeof window.popupsInicializados !== 'undefined') {
                        window.popupsInicializados.popupVehiculo = true;
                    }
                }
            }"
            Shown="function(s, e) { 
                setTimeout(function() { 
                    if (typeof initTarjetaScanner === 'function') { 
                        initTarjetaScanner(); 
                    }
                    if (typeof initDocumentoFileInput === 'function') {
                        initDocumentoFileInput();
                    }
                }, 100); 
            }" />
        <ContentCollection>
            <dx:PopupControlContentControl runat="server">
                <dx:ASPxFormLayout ID="formVehiculo" runat="server" Width="100%" ColCount="3" Theme="Office2010Blue">
                    <Items>
                        <%-- Indicador de procesamiento visible en todo el popup --%>
                        <dx:LayoutItem Caption="Estado del Procesamiento" ColSpan="3" ShowCaption="False">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <div id="tarjetaProcessingIndicator" style="display: none; padding: 15px; background-color: #e7f3ff; border: 2px solid #007bff; border-radius: 4px; text-align: center; margin-bottom: 15px;">
                                        <i class="fa fa-spinner fa-spin" style="font-size: 24px; color: #007bff; margin-right: 10px;"></i>
                                        <span style="font-size: 14px; color: #007bff; font-weight: bold;">Procesando tarjeta de circulación con Azure Document Intelligence... Por favor espere.</span>
                                    </div>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <%-- SUBIR ARCHIVOS TARJETA - Movido al inicio --%>
                        <dx:LayoutItem Caption="Subir Archivos de Tarjeta" ColSpan="3">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <!-- Área para subir múltiples archivos -->
                                    <div style="margin-top: 15px;">
                                        <div id="tarjetaDropZone" style="border: 2px dashed #ccc; padding: 20px; text-align: center; cursor: pointer; background-color: #f9f9f9; border-radius: 4px;">
                                            <i class="fa fa-cloud-upload" style="font-size: 48px; color: #999; margin-bottom: 10px;"></i>
                                            <p style="margin: 0; color: #666;">Haga clic o arrastre archivos aquí (múltiples archivos permitidos)</p>
                                            <p style="margin: 5px 0 0 0; font-size: 12px; color: #999;">Formatos: JPG, PNG, PDF (máx. 10MB cada uno)</p>
                                            <p style="margin: 5px 0 0 0; font-size: 11px; color: #007bff; font-weight: bold;">Los documentos se procesarán automáticamente al subirlos</p>
                                        </div>
                                        <input type="file" id="tarjetaFileInput" accept="image/jpeg,image/png,image/jpg,application/pdf" multiple style="display: none;" />
                                        <div id="tarjetaFilesList" style="margin-top: 10px; display: none;">
                                            <div id="tarjetaFilesPreview" style="display: flex; flex-wrap: wrap; gap: 10px;"></div>
                                        </div>
                                        <div id="tarjetaStatus" style="margin-top: 10px; padding: 8px; border-radius: 4px; display: none;"></div>
                                    </div>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <%-- Grid de archivos de Tarjeta --%>
                        <dx:LayoutGroup Caption="Archivos de Tarjeta de Circulación" ColSpan="3">
                            <Items>
                                <dx:LayoutItem ShowCaption="False">
                                    <LayoutItemNestedControlCollection>
                                        <dx:LayoutItemNestedControlContainer runat="server">
                                            <dx:ASPxGridView ID="gridArchivosVehiculo" runat="server" ClientInstanceName="gridArchivosVehiculo"
                                                Width="100%" KeyFieldName="Id" AutoGenerateColumns="False" Theme="Office2010Blue"
                                                OnCustomCallback="gridArchivosVehiculo_CustomCallback" OnDataBound="gridArchivosVehiculo_DataBound"
                                                EnableCallBacks="True">
                                                <ClientSideEvents ToolbarItemClick="function(s, e) { if (typeof window.onToolbarArchivosVehiculoClick === 'function') window.onToolbarArchivosVehiculoClick(s, e); }" />
                                                <SettingsPager Mode="ShowAllRecords" />
                                                <Settings ShowHeaderFilterButton="False" ShowFilterRow="False" ShowGroupPanel="False" />
                                                <SettingsBehavior AllowFocusedRow="True" AllowSelectByRowClick="True" AllowSort="True" />
                                                <SettingsPopup>
                                                    <FilterControl AutoUpdatePosition="False"></FilterControl>
                                                </SettingsPopup>
                                                <Toolbars>
                                                    <dx:GridViewToolbar>
                                                        <Items>
                                                            <dx:GridViewToolbarItem Name="btnVerArchivoVehiculo" Text="Ver Archivo">
                                                                <Image IconID="zoom_zoom_16x16" />
                                                            </dx:GridViewToolbarItem>
                                                            <dx:GridViewToolbarItem Name="btnEliminarArchivoVehiculo" Text="Eliminar Archivo">
                                                                <Image IconID="edit_delete_16x16" />
                                                            </dx:GridViewToolbarItem>
                                                        </Items>
                                                    </dx:GridViewToolbar>
                                                </Toolbars>
                                                <%-- Las columnas se generan dinámicamente en GenerarColumnasDinamicas --%>
                                            </dx:ASPxGridView>
                                        </dx:LayoutItemNestedControlContainer>
                                    </LayoutItemNestedControlCollection>
                                </dx:LayoutItem>
                            </Items>
                        </dx:LayoutGroup>
                        <dx:LayoutItem Caption="Placas" RequiredMarkDisplayMode="Required">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxTextBox ID="txtVehPlacas" runat="server" ClientInstanceName="txtVehPlacas"
                                        Width="100%" Theme="Office2010Blue" MaxLength="20">
                                        <ValidationSettings>
                                            <RequiredField IsRequired="True" ErrorText="Requerido" />
                                        </ValidationSettings>
                                    </dx:ASPxTextBox>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Tipo Vehículo">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxComboBox ID="cboVehTipo" runat="server" ClientInstanceName="cboVehTipo"
                                        Width="100%" Theme="Office2010Blue" ValueType="System.String">
                                        <Items>
                                            <dx:ListEditItem Text="Automóvil" Value="Automóvil" Selected="True" />
                                            <dx:ListEditItem Text="Camioneta" Value="Camioneta" />
                                            <dx:ListEditItem Text="Motocicleta" Value="Motocicleta" />
                                            <dx:ListEditItem Text="Bicicleta" Value="Bicicleta" />
                                            <dx:ListEditItem Text="Otro" Value="Otro" />
                                        </Items>
                                    </dx:ASPxComboBox>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Marca">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxTextBox ID="txtVehMarca" runat="server" ClientInstanceName="txtVehMarca"
                                        Width="100%" Theme="Office2010Blue" MaxLength="50" />
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Modelo">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxTextBox ID="txtVehModelo" runat="server" ClientInstanceName="txtVehModelo"
                                        Width="100%" Theme="Office2010Blue" MaxLength="50" />
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Año">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxSpinEdit ID="txtVehAnio" runat="server" ClientInstanceName="txtVehAnio"
                                        Width="100%" Theme="Office2010Blue" NumberType="Integer" MinValue="1990" MaxValue="2030" />
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Color">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxTextBox ID="txtVehColor" runat="server" ClientInstanceName="txtVehColor"
                                        Width="100%" Theme="Office2010Blue" MaxLength="30" />
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="No. Tarjetón">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxTextBox ID="txtVehTarjeton" runat="server" ClientInstanceName="txtVehTarjeton"
                                        Width="100%" Theme="Office2010Blue" MaxLength="50" />
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Residente">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxComboBox ID="cboVehResidente" runat="server" ClientInstanceName="cboVehResidente"
                                        Width="100%" Theme="Office2010Blue" ValueType="System.Int32" />
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Activo">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxCheckBox ID="chkVehActivo" runat="server" ClientInstanceName="chkVehActivo"
                                        Text="Activo" Checked="True" Theme="Office2010Blue" />
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutGroup Caption="Tarjeta de Circulación" ColSpan="3" ColCount="3">
                            <Items>
                                <dx:LayoutItem Caption="Fecha Expedición">
                                    <LayoutItemNestedControlCollection>
                                        <dx:LayoutItemNestedControlContainer runat="server">
                                            <dx:ASPxDateEdit ID="dteVehFechaExpedicion" runat="server" ClientInstanceName="dteVehFechaExpedicion"
                                                Width="100%" Theme="Office2010Blue" DisplayFormatString="dd/MM/yyyy" />
                                        </dx:LayoutItemNestedControlContainer>
                                    </LayoutItemNestedControlCollection>
                                </dx:LayoutItem>
                                <dx:LayoutItem Caption="Fecha Vigencia">
                                    <LayoutItemNestedControlCollection>
                                        <dx:LayoutItemNestedControlContainer runat="server">
                                            <dx:ASPxDateEdit ID="dteVehFechaVigencia" runat="server" ClientInstanceName="dteVehFechaVigencia"
                                                Width="100%" Theme="Office2010Blue" DisplayFormatString="dd/MM/yyyy" />
                                        </dx:LayoutItemNestedControlContainer>
                                    </LayoutItemNestedControlCollection>
                                </dx:LayoutItem>
                                <dx:LayoutItem Caption="Propietario Registrado">
                                    <LayoutItemNestedControlCollection>
                                        <dx:LayoutItemNestedControlContainer runat="server">
                                            <dx:ASPxTextBox ID="txtVehPropietarioRegistrado" runat="server" ClientInstanceName="txtVehPropietarioRegistrado"
                                                Width="100%" Theme="Office2010Blue" MaxLength="200" />
                                        </dx:LayoutItemNestedControlContainer>
                                    </LayoutItemNestedControlCollection>
                                </dx:LayoutItem>
                            </Items>
                        </dx:LayoutGroup>
                        <dx:LayoutGroup Caption="Datos Adicionales del Vehículo" ColSpan="3" ColCount="3">
                            <Items>
                                <dx:LayoutItem Caption="Número de Motor">
                                    <LayoutItemNestedControlCollection>
                                        <dx:LayoutItemNestedControlContainer runat="server">
                                            <dx:ASPxTextBox ID="txtVehNumeroMotor" runat="server" ClientInstanceName="txtVehNumeroMotor"
                                                Width="100%" Theme="Office2010Blue" MaxLength="50" />
                                        </dx:LayoutItemNestedControlContainer>
                                    </LayoutItemNestedControlCollection>
                                </dx:LayoutItem>
                                <dx:LayoutItem Caption="Número de Serie (VIN)" ColSpan="2">
                                    <LayoutItemNestedControlCollection>
                                        <dx:LayoutItemNestedControlContainer runat="server">
                                            <dx:ASPxTextBox ID="txtVehNumeroSerie" runat="server" ClientInstanceName="txtVehNumeroSerie"
                                                Width="100%" Theme="Office2010Blue" MaxLength="50" />
                                        </dx:LayoutItemNestedControlContainer>
                                    </LayoutItemNestedControlCollection>
                                </dx:LayoutItem>
                                <dx:LayoutItem Caption="Tipo de Combustible">
                                    <LayoutItemNestedControlCollection>
                                        <dx:LayoutItemNestedControlContainer runat="server">
                                            <dx:ASPxComboBox ID="cboVehTipoCombustible" runat="server" ClientInstanceName="cboVehTipoCombustible"
                                                Width="100%" Theme="Office2010Blue" ValueType="System.String">
                                                <Items>
                                                    <dx:ListEditItem Text="Gasolina" Value="Gasolina" />
                                                    <dx:ListEditItem Text="Diésel" Value="Diésel" />
                                                    <dx:ListEditItem Text="Eléctrico" Value="Eléctrico" />
                                                    <dx:ListEditItem Text="Híbrido" Value="Híbrido" />
                                                    <dx:ListEditItem Text="Gas Natural" Value="Gas Natural" />
                                                    <dx:ListEditItem Text="Otro" Value="Otro" />
                                                </Items>
                                            </dx:ASPxComboBox>
                                        </dx:LayoutItemNestedControlContainer>
                                    </LayoutItemNestedControlCollection>
                                </dx:LayoutItem>
                                <dx:LayoutItem Caption="Capacidad Pasajeros">
                                    <LayoutItemNestedControlCollection>
                                        <dx:LayoutItemNestedControlContainer runat="server">
                                            <dx:ASPxSpinEdit ID="txtVehCapacidadPasajeros" runat="server" ClientInstanceName="txtVehCapacidadPasajeros"
                                                Width="100%" Theme="Office2010Blue" NumberType="Integer" MinValue="1" MaxValue="50" />
                                        </dx:LayoutItemNestedControlContainer>
                                    </LayoutItemNestedControlCollection>
                                </dx:LayoutItem>
                                <dx:LayoutItem Caption="Uso del Vehículo">
                                    <LayoutItemNestedControlCollection>
                                        <dx:LayoutItemNestedControlContainer runat="server">
                                            <dx:ASPxComboBox ID="cboVehUso" runat="server" ClientInstanceName="cboVehUso"
                                                Width="100%" Theme="Office2010Blue" ValueType="System.String">
                                                <Items>
                                                    <dx:ListEditItem Text="Particular" Value="Particular" Selected="True" />
                                                    <dx:ListEditItem Text="Comercial" Value="Comercial" />
                                                    <dx:ListEditItem Text="Público" Value="Público" />
                                                    <dx:ListEditItem Text="Otro" Value="Otro" />
                                                </Items>
                                            </dx:ASPxComboBox>
                                        </dx:LayoutItemNestedControlContainer>
                                    </LayoutItemNestedControlCollection>
                                </dx:LayoutItem>
                            </Items>
                        </dx:LayoutGroup>
                        <dx:LayoutItem Caption="Observaciones" ColSpan="3">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxMemo ID="txtVehObservaciones" runat="server" ClientInstanceName="txtVehObservaciones"
                                        Width="100%" Height="50px" Theme="Office2010Blue" />
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                    </Items>
                </dx:ASPxFormLayout>
                <div style="margin-top: 15px; text-align: right;">
                    <dx:ASPxButton ID="btnGuardarVehiculo" runat="server" Text="Guardar" Theme="Office2010Blue" AutoPostBack="False">
                        <Image IconID="save_save_16x16" />
                        <ClientSideEvents Click="function(s,e){ guardarVehiculo(); }" />
                    </dx:ASPxButton>
                    <dx:ASPxButton ID="btnCerrarVehiculo" runat="server" Text="Cancelar" Theme="Office2010Blue" AutoPostBack="False">
                        <ClientSideEvents Click="function(s,e){ popupVehiculo.Hide(); }" />
                    </dx:ASPxButton>
                </div>
            </dx:PopupControlContentControl>
        </ContentCollection>
    </dx:ASPxPopupControl>

    <!-- ================================================================== -->
    <!-- POPUP: EDITAR TAG -->
    <!-- ================================================================== -->
    <dx:ASPxPopupControl ID="popupTag" runat="server" ClientInstanceName="popupTag"
        HeaderText="Tag de Acceso" Width="700px" Height="450px" Modal="True"
        PopupHorizontalAlign="WindowCenter" PopupVerticalAlign="WindowCenter"
        CloseAction="CloseButton" AllowDragging="True" CloseOnEscape="True">
        <ClientSideEvents
            Init="function(s, e) { 
                console.log('popupTag inicializado'); 
                if (typeof window !== 'undefined') {
                    window.popupTag = s;
                    if (typeof window.popupsInicializados !== 'undefined') {
                        window.popupsInicializados.popupTag = true;
                    }
                }
            }" />
        <ContentCollection>
            <dx:PopupControlContentControl runat="server">
                <dx:ASPxFormLayout ID="formTag" runat="server" Width="100%" ColCount="2" Theme="Office2010Blue">
                    <Items>
                        <dx:LayoutItem Caption="Código Tag" RequiredMarkDisplayMode="Required">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxTextBox ID="txtTagCodigo" runat="server" ClientInstanceName="txtTagCodigo"
                                        Width="100%" Theme="Office2010Blue" MaxLength="100">
                                        <ValidationSettings>
                                            <RequiredField IsRequired="True" ErrorText="Requerido" />
                                        </ValidationSettings>
                                    </dx:ASPxTextBox>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Tipo Tag">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxComboBox ID="cboTagTipo" runat="server" ClientInstanceName="cboTagTipo"
                                        Width="100%" Theme="Office2010Blue" ValueType="System.String">
                                        <Items>
                                            <dx:ListEditItem Text="RFID" Value="RFID" Selected="True" />
                                            <dx:ListEditItem Text="QR" Value="QR" />
                                            <dx:ListEditItem Text="Tarjeta" Value="Tarjeta" />
                                            <dx:ListEditItem Text="Control Remoto" Value="Control Remoto" />
                                        </Items>
                                    </dx:ASPxComboBox>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Descripción" ColSpan="2">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxTextBox ID="txtTagDescripcion" runat="server" ClientInstanceName="txtTagDescripcion"
                                        Width="100%" Theme="Office2010Blue" MaxLength="200" />
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Residente">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxComboBox ID="cboTagResidente" runat="server" ClientInstanceName="cboTagResidente"
                                        Width="100%" Theme="Office2010Blue" ValueType="System.Int32" />
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Fecha Asignación">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxDateEdit ID="dteTagAsignacion" runat="server" ClientInstanceName="dteTagAsignacion"
                                        Width="100%" Theme="Office2010Blue" DisplayFormatString="dd/MM/yyyy" />
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Fecha Vencimiento">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxDateEdit ID="dteTagVencimiento" runat="server" ClientInstanceName="dteTagVencimiento"
                                        Width="100%" Theme="Office2010Blue" DisplayFormatString="dd/MM/yyyy" />
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutGroup Caption="Permisos de Acceso" ColSpan="2" ColCount="3">
                            <Items>
                                <dx:LayoutItem ShowCaption="False">
                                    <LayoutItemNestedControlCollection>
                                        <dx:LayoutItemNestedControlContainer runat="server">
                                            <dx:ASPxCheckBox ID="chkTagPeatonal" runat="server" ClientInstanceName="chkTagPeatonal"
                                                Text="Acceso Peatonal" Checked="True" Theme="Office2010Blue" />
                                        </dx:LayoutItemNestedControlContainer>
                                    </LayoutItemNestedControlCollection>
                                </dx:LayoutItem>
                                <dx:LayoutItem ShowCaption="False">
                                    <LayoutItemNestedControlCollection>
                                        <dx:LayoutItemNestedControlContainer runat="server">
                                            <dx:ASPxCheckBox ID="chkTagVehicular" runat="server" ClientInstanceName="chkTagVehicular"
                                                Text="Acceso Vehicular" Theme="Office2010Blue" />
                                        </dx:LayoutItemNestedControlContainer>
                                    </LayoutItemNestedControlCollection>
                                </dx:LayoutItem>
                                <dx:LayoutItem ShowCaption="False">
                                    <LayoutItemNestedControlCollection>
                                        <dx:LayoutItemNestedControlContainer runat="server">
                                            <dx:ASPxCheckBox ID="chkTagAreas" runat="server" ClientInstanceName="chkTagAreas"
                                                Text="Áreas Comunes" Theme="Office2010Blue" />
                                        </dx:LayoutItemNestedControlContainer>
                                    </LayoutItemNestedControlCollection>
                                </dx:LayoutItem>
                            </Items>
                        </dx:LayoutGroup>
                        <dx:LayoutItem Caption="Observaciones" ColSpan="2">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxMemo ID="txtTagObservaciones" runat="server" ClientInstanceName="txtTagObservaciones"
                                        Width="100%" Height="50px" Theme="Office2010Blue" />
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Activo">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxCheckBox ID="chkTagActivo" runat="server" ClientInstanceName="chkTagActivo"
                                        Text="Activo" Checked="True" Theme="Office2010Blue" />
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                    </Items>
                </dx:ASPxFormLayout>
                <div style="margin-top: 15px; text-align: right;">
                    <dx:ASPxButton ID="btnGuardarTag" runat="server" Text="Guardar" Theme="Office2010Blue" AutoPostBack="False">
                        <Image IconID="save_save_16x16" />
                        <ClientSideEvents Click="function(s,e){ guardarTag(); }" />
                    </dx:ASPxButton>
                    <dx:ASPxButton ID="btnCerrarTag" runat="server" Text="Cancelar" Theme="Office2010Blue" AutoPostBack="False">
                        <ClientSideEvents Click="function(s,e){ popupTag.Hide(); }" />
                    </dx:ASPxButton>
                </div>
            </dx:PopupControlContentControl>
        </ContentCollection>
    </dx:ASPxPopupControl>

    <!-- ================================================================== -->
    <!-- POPUP: SUBIR DOCUMENTO -->
    <!-- ================================================================== -->
    <dx:ASPxPopupControl ID="popupDocumento" runat="server" ClientInstanceName="popupDocumento"
        HeaderText="Documento" Width="700px" Height="500px" Modal="True"
        PopupHorizontalAlign="WindowCenter" PopupVerticalAlign="WindowCenter"
        CloseAction="CloseButton" AllowDragging="True" CloseOnEscape="True">
        <ClientSideEvents
            Init="function(s, e) { 
                console.log('popupDocumento inicializado'); 
                if (typeof window !== 'undefined') {
                    window.popupDocumento = s;
                    if (typeof window.popupsInicializados !== 'undefined') {
                        window.popupsInicializados.popupDocumento = true;
                    }
                }
            }"
            Shown="function(s, e) { 
                setTimeout(function() { 
                    if (typeof initDocumentoFileInput === 'function') {
                        initDocumentoFileInput();
                    }
                }, 100); 
            }" />
        <ContentCollection>
            <dx:PopupControlContentControl runat="server">
                <dx:ASPxFormLayout ID="formDocumento" runat="server" Width="100%" ColCount="2" Theme="Office2010Blue">
                    <Items>
                        <dx:LayoutItem Caption="Tipo Documento" RequiredMarkDisplayMode="Required">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxComboBox ID="cboDocTipo" runat="server" ClientInstanceName="cboDocTipo"
                                        Width="100%" Theme="Office2010Blue" ValueType="System.String">
                                        <Items>
                                            <dx:ListEditItem Text="Escritura" Value="Escritura" Selected="True" />
                                            <dx:ListEditItem Text="Contrato Arrendamiento" Value="Contrato Arrendamiento" />
                                            <dx:ListEditItem Text="Identificación" Value="Identificación" />
                                            <dx:ListEditItem Text="Comprobante Domicilio" Value="Comprobante Domicilio" />
                                            <dx:ListEditItem Text="Reglamento Firmado" Value="Reglamento Firmado" />
                                            <dx:ListEditItem Text="Otro" Value="Otro" />
                                        </Items>
                                        <ValidationSettings>
                                            <RequiredField IsRequired="True" ErrorText="Requerido" />
                                        </ValidationSettings>
                                    </dx:ASPxComboBox>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="No. Documento">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxTextBox ID="txtDocNumero" runat="server" ClientInstanceName="txtDocNumero"
                                        Width="100%" Theme="Office2010Blue" MaxLength="100" NullText="No. escritura, folio, etc." />
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Nombre" RequiredMarkDisplayMode="Required" ColSpan="2">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxTextBox ID="txtDocNombre" runat="server" ClientInstanceName="txtDocNombre"
                                        Width="100%" Theme="Office2010Blue" MaxLength="200">
                                        <ValidationSettings>
                                            <RequiredField IsRequired="True" ErrorText="Requerido" />
                                        </ValidationSettings>
                                    </dx:ASPxTextBox>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Fecha Documento">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxDateEdit ID="dteDocFecha" runat="server" ClientInstanceName="dteDocFecha"
                                        Width="100%" Theme="Office2010Blue" DisplayFormatString="dd/MM/yyyy" />
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Notaría">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxTextBox ID="txtDocNotaria" runat="server" ClientInstanceName="txtDocNotaria"
                                        Width="100%" Theme="Office2010Blue" MaxLength="200" />
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Vigencia Inicio">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxDateEdit ID="dteDocVigenciaInicio" runat="server" ClientInstanceName="dteDocVigenciaInicio"
                                        Width="100%" Theme="Office2010Blue" DisplayFormatString="dd/MM/yyyy" />
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Vigencia Fin">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxDateEdit ID="dteDocVigenciaFin" runat="server" ClientInstanceName="dteDocVigenciaFin"
                                        Width="100%" Theme="Office2010Blue" DisplayFormatString="dd/MM/yyyy" />
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Descripción" ColSpan="2">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxMemo ID="txtDocDescripcion" runat="server" ClientInstanceName="txtDocDescripcion"
                                        Width="100%" Height="50px" Theme="Office2010Blue" />
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutGroup Caption="Archivos del Documento" ColSpan="2">
                            <Items>
                                <dx:LayoutItem ShowCaption="False">
                                    <LayoutItemNestedControlCollection>
                                        <dx:LayoutItemNestedControlContainer runat="server">
                                            <dx:ASPxGridView ID="gridArchivosDocumento" runat="server" ClientInstanceName="gridArchivosDocumento"
                                                Width="100%" KeyFieldName="Id" AutoGenerateColumns="False" Theme="Office2010Blue"
                                                OnCustomCallback="gridArchivosDocumento_CustomCallback" OnDataBound="gridArchivosDocumento_DataBound"
                                                EnableCallBacks="True">
                                                <ClientSideEvents ToolbarItemClick="function(s, e) { if (typeof window.onToolbarArchivosDocumentoClick === 'function') window.onToolbarArchivosDocumentoClick(s, e); }" />
                                                <SettingsPager Mode="ShowAllRecords" />
                                                <Settings ShowHeaderFilterButton="False" ShowFilterRow="False" ShowGroupPanel="False" />
                                                <SettingsBehavior AllowFocusedRow="True" AllowSelectByRowClick="True" AllowSort="True" />

                                                <SettingsPopup>
                                                    <FilterControl AutoUpdatePosition="False"></FilterControl>
                                                </SettingsPopup>
                                                <Toolbars>
                                                    <dx:GridViewToolbar>
                                                        <Items>
                                                            <dx:GridViewToolbarItem Name="btnVerArchivoDocumento" Text="Ver Archivo">
                                                                <Image IconID="zoom_zoom_16x16" />
                                                            </dx:GridViewToolbarItem>
                                                            <dx:GridViewToolbarItem Name="btnEliminarArchivoDocumento" Text="Eliminar Archivo">
                                                                <Image IconID="edit_delete_16x16" />
                                                            </dx:GridViewToolbarItem>
                                                        </Items>
                                                    </dx:GridViewToolbar>
                                                </Toolbars>
                                                <%-- Las columnas se generan dinámicamente en GenerarColumnasDinamicas --%>
                                            </dx:ASPxGridView>
                                        </dx:LayoutItemNestedControlContainer>
                                    </LayoutItemNestedControlCollection>
                                </dx:LayoutItem>
                            </Items>
                        </dx:LayoutGroup>
                        <dx:LayoutItem Caption="Subir Archivos del Documento" ColSpan="2">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">

                                    <!-- Área para subir múltiples archivos -->
                                    <div style="margin-top: 15px;">
                                        <div id="documentoDropZone" style="border: 2px dashed #ccc; padding: 20px; text-align: center; cursor: pointer; background-color: #f9f9f9; border-radius: 4px;">
                                            <i class="fa fa-cloud-upload" style="font-size: 48px; color: #999; margin-bottom: 10px;"></i>
                                            <p style="margin: 0; color: #666;">Haga clic o arrastre archivos aquí (múltiples archivos permitidos)</p>
                                            <p style="margin: 5px 0 0 0; font-size: 12px; color: #999;">Formatos: PDF, JPG, PNG (máx. 10MB cada uno)</p>
                                        </div>
                                        <input type="file" id="documentoFileInput" accept=".pdf,.jpg,.jpeg,.png" multiple style="display: none;" />
                                        <div id="documentoFilesList" style="margin-top: 10px; display: none;">
                                            <div id="documentoFilesPreview" style="display: flex; flex-wrap: wrap; gap: 10px;"></div>
                                        </div>
                                        <asp:HiddenField ID="hfDocRutaArchivo" runat="server" ClientIDMode="Static" />
                                        <asp:HiddenField ID="hfDocNombreArchivo" runat="server" ClientIDMode="Static" />
                                    </div>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Activo">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxCheckBox ID="chkDocActivo" runat="server" ClientInstanceName="chkDocActivo"
                                        Text="Activo" Checked="True" Theme="Office2010Blue" />
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                    </Items>
                </dx:ASPxFormLayout>
                <div style="margin-top: 15px; text-align: right;">
                    <dx:ASPxButton ID="btnGuardarDocumento" runat="server" Text="Guardar" Theme="Office2010Blue" AutoPostBack="False">
                        <Image IconID="save_save_16x16" />
                        <ClientSideEvents Click="function(s,e){ guardarDocumento(); }" />
                    </dx:ASPxButton>
                    <dx:ASPxButton ID="btnCerrarDocumento" runat="server" Text="Cancelar" Theme="Office2010Blue" AutoPostBack="False">
                        <ClientSideEvents Click="function(s,e){ popupDocumento.Hide(); }" />
                    </dx:ASPxButton>
                </div>
            </dx:PopupControlContentControl>
        </ContentCollection>
    </dx:ASPxPopupControl>

    <!-- HIDDEN FIELDS -->
    <asp:HiddenField ID="hfId" runat="server" Value="0" ClientIDMode="Static" />
    <asp:HiddenField ID="hfResidenteId" runat="server" Value="0" ClientIDMode="Static" />
    <asp:HiddenField ID="hfVehiculoId" runat="server" Value="0" ClientIDMode="Static" />
    <asp:HiddenField ID="hfTagId" runat="server" Value="0" ClientIDMode="Static" />
    <asp:HiddenField ID="hfDocumentoId" runat="server" Value="0" ClientIDMode="Static" />
    <asp:HiddenField ID="hfUnidadIdActual" runat="server" Value="0" ClientIDMode="Static" />

    <!-- Google Maps API - Colocado después del contenedor como en frmOperacion.aspx -->
    <% 
        Dim googleMapsKey As String = ConfigurationManager.AppSettings("GoogleMapsApiKey")
        If String.IsNullOrWhiteSpace(googleMapsKey) Then
            googleMapsKey = String.Empty
        End If
    %>
    <% If Not String.IsNullOrWhiteSpace(googleMapsKey) Then %>
    <% 
        Dim keyPreview As String = googleMapsKey
        If googleMapsKey.Length > 20 Then
            keyPreview = googleMapsKey.Substring(0, 20)
        End If
    %>
    <script>
        console.log('Cargando Google Maps API con key:', '<%= keyPreview %>...');
        console.log('Callback configurado: googleMapsCallback');
        
        // Contador de reintentos para evitar bucle infinito
        var googleMapsCallbackRetries = 0;
        var maxRetries = 10;
        
        // Crear función wrapper para el callback de Google Maps que verifica si la función existe
        // Esto evita errores si Google Maps se carga antes de que unidades.js termine de ejecutarse
        window.googleMapsCallback = function() {
            console.log('🔄 googleMapsCallback ejecutado (intento ' + (googleMapsCallbackRetries + 1) + '/' + maxRetries + '), buscando inicializarMapa...');
            
            // Intentar múltiples formas de obtener la función
            var initFunction = null;
            
            // Método 1: Desde scope global
            if (typeof inicializarMapa === 'function') {
                initFunction = inicializarMapa;
                console.log('✅ inicializarMapa encontrada en scope global');
            }
            // Método 2: Desde window.inicializarMapa
            else if (typeof window.inicializarMapa === 'function') {
                initFunction = window.inicializarMapa;
                console.log('✅ inicializarMapa encontrada en window.inicializarMapa');
                // Asignar también al scope global para futuros usos
                inicializarMapa = window.inicializarMapa;
            }
            
            if (initFunction && typeof initFunction === 'function') {
                try {
                    console.log('✅ Ejecutando inicializarMapa...');
                    initFunction();
                    // Resetear contador después de éxito
                    googleMapsCallbackRetries = 0;
                } catch (error) {
                    console.error('❌ Error al ejecutar inicializarMapa:', error);
                }
            } else {
                // Incrementar contador de reintentos
                googleMapsCallbackRetries++;
                
                if (googleMapsCallbackRetries < maxRetries) {
                    console.warn('⚠️ inicializarMapa no encontrada. Reintentando en 500ms... (intento ' + googleMapsCallbackRetries + '/' + maxRetries + ')');
                    // Reintentar después de un breve delay
                    setTimeout(function() {
                        window.googleMapsCallback();
                    }, 500);
                } else {
                    console.error('❌ inicializarMapa no encontrada después de ' + maxRetries + ' intentos. Abortando.');
                    var loadingDiv = document.getElementById('mapaLoading');
                    if (loadingDiv) {
                        loadingDiv.innerHTML = '<span style="color: red;">Error: No se pudo inicializar el mapa</span>';
                    }
                }
            }
        };
        
        console.log('✅ Callback wrapper creado y asignado');
    </script>
    <script async src="https://maps.googleapis.com/maps/api/js?key=<%= googleMapsKey %>&callback=googleMapsCallback"></script>
    <% Else %>
    <script>
        console.error('Google Maps API Key no está configurada en Web.config. Configure la clave "GoogleMapsApiKey" en appSettings.');
        var loadingDiv = document.getElementById('mapaLoading');
        if (loadingDiv) {
            loadingDiv.innerHTML = '<span style="color: red;">API Key no configurada</span>';
        }
    </script>
    <% End If %>

</asp:Content>
