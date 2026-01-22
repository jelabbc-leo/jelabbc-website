<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/Jela.Master" CodeBehind="Entidades.aspx.vb" Inherits="JelaWeb.Entidades" %>

<%@ Register Assembly="DevExpress.Web.Bootstrap.v22.2, Version=22.2.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web.Bootstrap" TagPrefix="dx" %>
<%@ Register Assembly="DevExpress.Web.v22.2" Namespace="DevExpress.Web" TagPrefix="dx" %>


<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <!-- Toolbar -->
    <div class="toolbar">
        <dx:BootstrapToolbar ID="toolbarEntidades" runat="server" ClientInstanceName="toolbarEntidades" Width="100%">
            <Items>
                <dx:BootstrapToolbarItem Text="Agregar" Name="Agregar" IconCssClass="fa fa-plus" BeginGroup="True" />
                <dx:BootstrapToolbarItem Text="Editar" Name="Editar" IconCssClass="fa fa-edit" />
                <dx:BootstrapToolbarItem Text="Eliminar" Name="Eliminar" IconCssClass="fa fa-trash" />
                <dx:BootstrapToolbarItem Text="Exportar" Name="Exportar" IconCssClass="fa fa-file-excel" />
                <dx:BootstrapToolbarItem Text="Refrescar" Name="Refrescar" IconCssClass="fa fa-sync" />
            </Items>
            <CssClasses Item="metro-button" />
            <ClientSideEvents ItemClick="onToolbarClick" />
        </dx:BootstrapToolbar>
    </div>

    <!-- Grid -->
    <dx:ASPxCallbackPanel ID="gridCallback" runat="server" ClientInstanceName="gridCallback">
        <PanelCollection>
            <dx:PanelContent>

                <dx:ASPxGridView ID="gridEntidades" runat="server" KeyFieldName="Id" Width="100%"
                    ClientInstanceName="gridEntidades" Theme="Office2010Blue" AutoGenerateColumns="True">
                    <Settings ShowHeaderFilterButton="True" />
                    <SettingsPopup>
                        <FilterControl AutoUpdatePosition="False"></FilterControl>
                    </SettingsPopup>
                    <SettingsSearchPanel Visible="True" />
                    <Columns>
                        <dx:GridViewCommandColumn ShowClearFilterButton="True" ShowInCustomizationForm="True" VisibleIndex="0">
                        </dx:GridViewCommandColumn>

                        <dx:GridViewDataColumn Caption="Sub Entidad" VisibleIndex="99">
                            <DataItemTemplate>
                                <dx:ASPxButton ID="btnVerSubEntidad" runat="server" Text="Sub entidad" AutoPostBack="False">
                                </dx:ASPxButton>
                            </DataItemTemplate>
                        </dx:GridViewDataColumn>

                    </Columns>
                </dx:ASPxGridView>
            </dx:PanelContent>
        </PanelCollection>
    </dx:ASPxCallbackPanel>


    <dx:ASPxPopupControl ID="popupForm" runat="server" AllowDragging="True" ClientInstanceName="popupForm" CloseAction="CloseButton" HeaderText="Entidad" Modal="True" PopupAction="None" PopupHorizontalAlign="WindowCenter" PopupVerticalAlign="WindowCenter" Theme="Office2010Blue" Width="800px">
        <ContentCollection>
            <dx:PopupControlContentControl runat="server">
                <asp:HiddenField ID="hfId" runat="server" />

                <div id="loadingIndicator" style="display: none; text-align: center; padding: 10px;">
                    <img src="https://i.imgur.com/llF5iyg.gif" alt="Cargando..." style="height: 24px; vertical-align: middle;" />
                    <span style="margin-left: 8px;">Procesando PDF, por favor espera...</span>
                </div>

                <dx:ASPxFormLayout ID="formLayoutEntidad" runat="server" AlignItemCaptionsInAllGroups="True" EnableTheming="True" Theme="Office2010Blue" Width="100%">
                    <SettingsAdaptivity AdaptivityMode="SingleColumnWindowLimit">
                    </SettingsAdaptivity>
                    <Items>
                        <dx:LayoutGroup Caption="Datos Entidad" ColCount="3" ColumnCount="3" Width="100%">
                            <Items>

                                <dx:LayoutItem Caption="Subir PDF" ColSpan="1">
                                    <LayoutItemNestedControlCollection>
                                        <dx:LayoutItemNestedControlContainer runat="server">
                                            <input type="file" id="pdfInput" accept="application/pdf" onchange="enviarPDF()" />
                                        </dx:LayoutItemNestedControlContainer>
                                    </LayoutItemNestedControlCollection>
                                </dx:LayoutItem>

                                <dx:LayoutItem Caption="Clave" ColSpan="1">
                                    <LayoutItemNestedControlCollection>
                                        <dx:LayoutItemNestedControlContainer runat="server">
                                            <dx:ASPxTextBox ID="txtClave" runat="server" Width="100%" Theme="Office2010Blue" ClientInstanceName="txtClave"></dx:ASPxTextBox>
                                        </dx:LayoutItemNestedControlContainer>
                                    </LayoutItemNestedControlCollection>
                                </dx:LayoutItem>

                                <dx:LayoutItem Caption="CIF" ColSpan="1">
                                    <LayoutItemNestedControlCollection>
                                        <dx:LayoutItemNestedControlContainer runat="server">
                                            <dx:ASPxTextBox ID="txtCIF" runat="server" Width="100%" ClientInstanceName="txtCIF" Theme="Office2010Blue"></dx:ASPxTextBox>
                                        </dx:LayoutItemNestedControlContainer>
                                    </LayoutItemNestedControlCollection>
                                </dx:LayoutItem>

                                <dx:LayoutItem Caption="Razón Social" ColSpan="1">
                                    <LayoutItemNestedControlCollection>
                                        <dx:LayoutItemNestedControlContainer runat="server">
                                            <dx:ASPxTextBox ID="txtRazonSocial" runat="server" Width="100%" ClientInstanceName="txtRazonSocial" Theme="Office2010Blue"></dx:ASPxTextBox>
                                        </dx:LayoutItemNestedControlContainer>
                                    </LayoutItemNestedControlCollection>
                                </dx:LayoutItem>

                                <dx:LayoutItem Caption="RFC" ColSpan="1">
                                    <LayoutItemNestedControlCollection>
                                        <dx:LayoutItemNestedControlContainer runat="server">
                                            <dx:ASPxTextBox ID="txtRFC" runat="server" Width="100%" ClientInstanceName="txtRFC" Theme="Office2010Blue"></dx:ASPxTextBox>
                                        </dx:LayoutItemNestedControlContainer>
                                    </LayoutItemNestedControlCollection>
                                </dx:LayoutItem>

                                <dx:LayoutItem Caption="Fecha Inicio" ColSpan="1">
                                    <LayoutItemNestedControlCollection>
                                        <dx:LayoutItemNestedControlContainer runat="server">
                                            <dx:ASPxTextBox ID="txtFechaInicio" runat="server" Width="100%" ClientInstanceName="txtFechaInicio" Theme="Office2010Blue"></dx:ASPxTextBox>
                                        </dx:LayoutItemNestedControlContainer>
                                    </LayoutItemNestedControlCollection>
                                </dx:LayoutItem>

                                <dx:LayoutItem Caption="CP" ColSpan="1">
                                    <LayoutItemNestedControlCollection>
                                        <dx:LayoutItemNestedControlContainer runat="server">
                                            <dx:ASPxTextBox ID="txtCP" runat="server" Width="100%" ClientInstanceName="txtCP" Theme="Office2010Blue"></dx:ASPxTextBox>
                                        </dx:LayoutItemNestedControlContainer>
                                    </LayoutItemNestedControlCollection>
                                </dx:LayoutItem>

                                <dx:LayoutItem Caption="Tipo Vialidad" ColSpan="1">
                                    <LayoutItemNestedControlCollection>
                                        <dx:LayoutItemNestedControlContainer runat="server">
                                            <dx:ASPxTextBox ID="txtTipoVial" runat="server" Width="100%" ClientInstanceName="txtTipoVial" Theme="Office2010Blue"></dx:ASPxTextBox>
                                        </dx:LayoutItemNestedControlContainer>
                                    </LayoutItemNestedControlCollection>
                                </dx:LayoutItem>

                                <dx:LayoutItem Caption="Nombre Vialidad" ColSpan="1">
                                    <LayoutItemNestedControlCollection>
                                        <dx:LayoutItemNestedControlContainer runat="server">
                                            <dx:ASPxTextBox ID="txtCalle" runat="server" Width="100%" ClientInstanceName="txtCalle" Theme="Office2010Blue"></dx:ASPxTextBox>
                                        </dx:LayoutItemNestedControlContainer>
                                    </LayoutItemNestedControlCollection>
                                </dx:LayoutItem>

                                <dx:LayoutItem Caption="No. Exterior" ColSpan="1">
                                    <LayoutItemNestedControlCollection>
                                        <dx:LayoutItemNestedControlContainer runat="server">
                                            <dx:ASPxTextBox ID="txtNoExterior" runat="server" Width="100%" ClientInstanceName="txtNoExterior" Theme="Office2010Blue"></dx:ASPxTextBox>
                                        </dx:LayoutItemNestedControlContainer>
                                    </LayoutItemNestedControlCollection>
                                </dx:LayoutItem>

                                <dx:LayoutItem Caption="No. Interior" ColSpan="1">
                                    <LayoutItemNestedControlCollection>
                                        <dx:LayoutItemNestedControlContainer runat="server">
                                            <dx:ASPxTextBox ID="txtNoInterior" runat="server" Width="100%" ClientInstanceName="txtNoInterior" Theme="Office2010Blue"></dx:ASPxTextBox>
                                        </dx:LayoutItemNestedControlContainer>
                                    </LayoutItemNestedControlCollection>
                                </dx:LayoutItem>

                                <dx:LayoutItem Caption="Colonia" ColSpan="1">
                                    <LayoutItemNestedControlCollection>
                                        <dx:LayoutItemNestedControlContainer runat="server">
                                            <dx:ASPxTextBox ID="txtColonia" runat="server" Width="100%" ClientInstanceName="txtColonia" Theme="Office2010Blue"></dx:ASPxTextBox>
                                        </dx:LayoutItemNestedControlContainer>
                                    </LayoutItemNestedControlCollection>
                                </dx:LayoutItem>

                                <dx:LayoutItem Caption="Localidad" ColSpan="1">
                                    <LayoutItemNestedControlCollection>
                                        <dx:LayoutItemNestedControlContainer runat="server">
                                            <dx:ASPxTextBox ID="txtLocalidad" runat="server" Width="100%" ClientInstanceName="txtLocalidad" Theme="Office2010Blue"></dx:ASPxTextBox>
                                        </dx:LayoutItemNestedControlContainer>
                                    </LayoutItemNestedControlCollection>
                                </dx:LayoutItem>

                                <dx:LayoutItem Caption="Municipio" ColSpan="1">
                                    <LayoutItemNestedControlCollection>
                                        <dx:LayoutItemNestedControlContainer runat="server">
                                            <dx:ASPxTextBox ID="txtMunicipio" runat="server" Width="100%" ClientInstanceName="txtMunicipio" Theme="Office2010Blue"></dx:ASPxTextBox>
                                        </dx:LayoutItemNestedControlContainer>
                                    </LayoutItemNestedControlCollection>
                                </dx:LayoutItem>

                                <dx:LayoutItem Caption="Estado" ColSpan="1">
                                    <LayoutItemNestedControlCollection>
                                        <dx:LayoutItemNestedControlContainer runat="server">
                                            <dx:ASPxTextBox ID="txtEstado" runat="server" Width="100%" ClientInstanceName="txtEstado" Theme="Office2010Blue"></dx:ASPxTextBox>
                                        </dx:LayoutItemNestedControlContainer>
                                    </LayoutItemNestedControlCollection>
                                </dx:LayoutItem>

                                <dx:LayoutItem Caption="Entre Calle" ColSpan="1">
                                    <LayoutItemNestedControlCollection>
                                        <dx:LayoutItemNestedControlContainer runat="server">
                                            <dx:ASPxTextBox ID="txtEntrecalle" runat="server" Width="100%" ClientInstanceName="txtEntrecalle" Theme="Office2010Blue"></dx:ASPxTextBox>
                                        </dx:LayoutItemNestedControlContainer>
                                    </LayoutItemNestedControlCollection>
                                </dx:LayoutItem>

                                <dx:LayoutItem Caption="Régimen Fiscal" ColSpan="1">
                                    <LayoutItemNestedControlCollection>
                                        <dx:LayoutItemNestedControlContainer runat="server">
                                            <dx:ASPxGridLookup ID="glClaveRegimen" runat="server" AutoGenerateColumns="False" KeyFieldName="Clave" ValueField="Nombre" TextFormatString="{0} - {1}" Theme="Office2010Blue" Width="100%">
                                                <GridViewProperties>
                                                    <SettingsBehavior AllowFocusedRow="True" AllowSelectSingleRowOnly="True" />
                                                    <SettingsPager Mode="ShowAllRecords" PageSize="0">
                                                    </SettingsPager>
                                                    <Settings ShowFilterRow="True" VerticalScrollBarMode="Visible" />
                                                    <SettingsPopup>
                                                        <FilterControl AutoUpdatePosition="False">
                                                        </FilterControl>
                                                    </SettingsPopup>
                                                </GridViewProperties>
                                                <Columns>
                                                    <dx:GridViewDataColumn Caption="Clave" FieldName="Clave" ShowInCustomizationForm="True" VisibleIndex="0" Width="80px">
                                                    </dx:GridViewDataColumn>
                                                    <dx:GridViewDataColumn Caption="Regimen" FieldName="Nombre" ShowInCustomizationForm="True" VisibleIndex="1" Width="250px">
                                                    </dx:GridViewDataColumn>
                                                </Columns>
                                            </dx:ASPxGridLookup>
                                        </dx:LayoutItemNestedControlContainer>
                                    </LayoutItemNestedControlCollection>
                                </dx:LayoutItem>

                                <dx:LayoutItem Caption="Metodo de Pago" ColSpan="1">
                                    <LayoutItemNestedControlCollection>
                                        <dx:LayoutItemNestedControlContainer runat="server">
                                            <dx:ASPxGridLookup ID="glMetodo" runat="server" AutoGenerateColumns="False" EnableTheming="True" ValueField="Nombre" Theme="Office2010Blue" Width="100%">
                                                <GridViewProperties>
                                                    <SettingsBehavior AllowFocusedRow="True" AllowSelectSingleRowOnly="True" />
                                                    <SettingsPopup>
                                                        <FilterControl AutoUpdatePosition="False">
                                                        </FilterControl>
                                                    </SettingsPopup>
                                                </GridViewProperties>
                                                <Columns>
                                                    <dx:GridViewDataTextColumn Caption="Clave" FieldName="Clave" ShowInCustomizationForm="True" VisibleIndex="0">
                                                    </dx:GridViewDataTextColumn>
                                                    <dx:GridViewDataTextColumn Caption="Metodo de Pago" FieldName="Nombre" ShowInCustomizationForm="True" VisibleIndex="1">
                                                    </dx:GridViewDataTextColumn>
                                                </Columns>
                                            </dx:ASPxGridLookup>
                                        </dx:LayoutItemNestedControlContainer>
                                    </LayoutItemNestedControlCollection>
                                </dx:LayoutItem>

                                <dx:LayoutItem Caption="Forma de Pago" ColSpan="1">
                                    <LayoutItemNestedControlCollection>
                                        <dx:LayoutItemNestedControlContainer runat="server">
                                            <dx:ASPxGridLookup ID="glForma" runat="server" AutoGenerateColumns="False" KeyFieldName="Clave" ValueField="Nombre" TextFormatString="{0} - {1}" Theme="Office2010Blue" Width="100%">

                                                <GridViewProperties>
                                                    <SettingsBehavior AllowFocusedRow="True" AllowSelectSingleRowOnly="True" />
                                                    <SettingsPager Mode="ShowAllRecords" PageSize="0">
                                                    </SettingsPager>
                                                    <Settings ShowFilterRow="True" VerticalScrollBarMode="Visible" />
                                                    <SettingsPopup>
                                                        <FilterControl AutoUpdatePosition="False">
                                                        </FilterControl>
                                                    </SettingsPopup>
                                                </GridViewProperties>

                                                <Columns>
                                                    <dx:GridViewDataTextColumn Caption="Clave" FieldName="Clave" ShowInCustomizationForm="True" VisibleIndex="0" Width="80px">
                                                    </dx:GridViewDataTextColumn>
                                                    <dx:GridViewDataTextColumn Caption="Forma de Pago" FieldName="Nombre" ShowInCustomizationForm="True" VisibleIndex="1" Width="200px">
                                                    </dx:GridViewDataTextColumn>
                                                </Columns>

                                            </dx:ASPxGridLookup>
                                        </dx:LayoutItemNestedControlContainer>
                                    </LayoutItemNestedControlCollection>
                                </dx:LayoutItem>

                                <dx:LayoutItem Caption="Uso CFDI" ColSpan="1">
                                    <LayoutItemNestedControlCollection>
                                        <dx:LayoutItemNestedControlContainer runat="server">
                                            <dx:ASPxGridLookup ID="glUsos" runat="server" AutoGenerateColumns="False" EnableTheming="True" ValueField="Nombre" Theme="Office2010Blue" Width="100%">
                                                <GridViewProperties>
                                                    <SettingsBehavior AllowFocusedRow="True" AllowSelectSingleRowOnly="True" />
                                                    <SettingsPager Mode="ShowAllRecords" PageSize="0">
                                                    </SettingsPager>
                                                    <Settings ShowFilterRow="True" VerticalScrollBarMode="Visible" />
                                                    <SettingsPopup>
                                                        <FilterControl AutoUpdatePosition="False">
                                                        </FilterControl>
                                                    </SettingsPopup>
                                                </GridViewProperties>
                                                <Columns>
                                                    <dx:GridViewDataTextColumn Caption="Clave" FieldName="Clave" ShowInCustomizationForm="True" VisibleIndex="0" Width="80px">
                                                    </dx:GridViewDataTextColumn>
                                                    <dx:GridViewDataTextColumn Caption="Uso" FieldName="Nombre" ShowInCustomizationForm="True" VisibleIndex="1" Width="200px">
                                                    </dx:GridViewDataTextColumn>
                                                </Columns>
                                            </dx:ASPxGridLookup>
                                        </dx:LayoutItemNestedControlContainer>
                                    </LayoutItemNestedControlCollection>
                                </dx:LayoutItem>

                            </Items>
                        </dx:LayoutGroup>

                        <dx:LayoutGroup Caption="Credenciales de Acceso">
                            <Items>
                                <dx:LayoutItem Caption="Usuario">
                                    <LayoutItemNestedControlCollection>
                                        <dx:LayoutItemNestedControlContainer runat="server">
                                            <dx:ASPxTextBox ID="ASPxTextBox1" runat="server" />
                                        </dx:LayoutItemNestedControlContainer>
                                    </LayoutItemNestedControlCollection>
                                </dx:LayoutItem>

                                <dx:LayoutItem Caption="Contraseña">
                                    <LayoutItemNestedControlCollection>
                                        <dx:LayoutItemNestedControlContainer runat="server">
                                            <dx:ASPxTextBox ID="ASPxTextBox2" runat="server" Password="True" />
                                        </dx:LayoutItemNestedControlContainer>
                                    </LayoutItemNestedControlCollection>
                                </dx:LayoutItem>

                            </Items>
                        </dx:LayoutGroup>

                    </Items>
                    <SettingsItems VerticalAlign="Middle" />
                </dx:ASPxFormLayout>
                <div class="form-actions mt-3">
                    <asp:Button ID="btnGuardar" runat="server" CssClass="metro-button" Text="Guardar" />
                </div>
            </dx:PopupControlContentControl>
        </ContentCollection>
    </dx:ASPxPopupControl>

    <!-- Modal Form -->

    <dx:ASPxPopupControl ID="popupDetalle" runat="server" ClientInstanceName="popupDetalle"
        Width="1300px" Height="500px" Modal="True" ShowOnPageLoad="False" HeaderText="Sub Entidades" AllowDragging="True" ResizingMode="Live" CloseAction="CloseButton" CloseOnEscape="True" Theme="Office2010Blue" PopupHorizontalAlign="WindowCenter" PopupVerticalAlign="WindowCenter">
        <ContentCollection>
            <dx:PopupControlContentControl runat="server">

                <dx:ASPxHiddenField ID="hfEntidad" runat="server" ClientInstanceName="hfEntidad" />

                <div class="toolbar">
                    <!-- Toolbar -->
                    <!-- BootstrapToolbar -->
                    <dx:BootstrapToolbar ID="toolbarDetalle" runat="server" ClientInstanceName="toolbarDetalle" Width="100%">
                        <Items>
                            <dx:BootstrapToolbarItem Text="Agregar" Name="Agregar" IconCssClass="fa fa-plus" />
                            <dx:BootstrapToolbarItem Text="Editar" Name="Editar" IconCssClass="fa fa-edit" />
                            <dx:BootstrapToolbarItem Text="Eliminar" Name="Eliminar" IconCssClass="fa fa-trash" />
                            <dx:BootstrapToolbarItem Text="Exportar" Name="Exportar" IconCssClass="fa fa-file-excel" />
                            <dx:BootstrapToolbarItem Text="Refrescar" Name="Refrescar" IconCssClass="fa fa-sync" />
                        </Items>
                        <CssClasses Item="metro-button" />
                        <ClientSideEvents ItemClick="function(s, e) {
        if(e.item.name === 'Agregar') {
            PopUpDatosSub.Show();
        }
    }" />

                    </dx:BootstrapToolbar>
                </div>

                <!-- Grid secundario -->
                <dx:ASPxGridView ID="gridDetalle" runat="server"
                    KeyFieldName="Id"
                    ClientInstanceName="gridDetalle"
                    OnCustomCallback="gridDetalle_CustomCallback"
                    Theme="Office2010Blue"
                    AutoGenerateColumns="True" Width="100%">
                    <SettingsPopup>
                        <FilterControl AutoUpdatePosition="False" />
                    </SettingsPopup>
                    <SettingsAdaptivity AdaptivityMode="HideDataCells" />
                    <Settings ShowFilterBar="Auto" ShowGroupPanel="True" ShowHeaderFilterButton="True" />
                    <SettingsResizing ColumnResizeMode="Control" />

                    <SettingsSearchPanel Visible="True" />

                </dx:ASPxGridView>


            </dx:PopupControlContentControl>
        </ContentCollection>
    </dx:ASPxPopupControl>

    <dx:ASPxPopupControl ID="PopUpDatosSub" runat="server"
        ClientInstanceName="PopUpDatosSub"
        HeaderText="Datos Sub entidad"
        Modal="True"
        ShowCloseButton="True"
        Width="800px" Theme="Office2010Blue" AllowDragging="True" CloseAction="CloseButton" CloseOnEscape="True" PopupHorizontalAlign="WindowCenter" PopupVerticalAlign="WindowCenter">
        <ContentCollection>
            <dx:PopupControlContentControl runat="server">

                <dx:ASPxFormLayout ID="formLayoutDatosSub" runat="server"
                    AlignItemCaptionsInAllGroups="True"
                    EnableTheming="True" Theme="Office2010Blue"
                    Width="100%">
                    <SettingsAdaptivity AdaptivityMode="SingleColumnWindowLimit" />

                    <Items>
                        <dx:LayoutGroup Caption="Datos Sub Entidad" ColCount="3" ColumnCount="3" Width="100%">
                            <Items>
                                <dx:LayoutItem Caption="IdEntidad" Visible="False">
                                    <LayoutItemNestedControlCollection>
                                        <dx:LayoutItemNestedControlContainer runat="server">
                                            <dx:ASPxHiddenField ID="hfIdEntidad" runat="server" />
                                        </dx:LayoutItemNestedControlContainer>
                                    </LayoutItemNestedControlCollection>
                                </dx:LayoutItem>


                                <dx:LayoutItem Caption="Clave">
                                    <LayoutItemNestedControlCollection>
                                        <dx:LayoutItemNestedControlContainer runat="server">
                                            <dx:ASPxTextBox ID="txtClaveSub" runat="server" Width="100%" Theme="Office2010Blue" />
                                        </dx:LayoutItemNestedControlContainer>
                                    </LayoutItemNestedControlCollection>
                                </dx:LayoutItem>

                                <dx:LayoutItem Caption="Alias">
                                    <LayoutItemNestedControlCollection>
                                        <dx:LayoutItemNestedControlContainer runat="server">
                                            <dx:ASPxTextBox ID="txtAlias" runat="server" Width="100%" Theme="Office2010Blue" />
                                        </dx:LayoutItemNestedControlContainer>
                                    </LayoutItemNestedControlCollection>
                                </dx:LayoutItem>

                                <dx:LayoutItem Caption="CIF">
                                    <LayoutItemNestedControlCollection>
                                        <dx:LayoutItemNestedControlContainer runat="server">
                                            <dx:ASPxTextBox ID="txtCifSub" runat="server" Width="100%" Theme="Office2010Blue" />
                                        </dx:LayoutItemNestedControlContainer>
                                    </LayoutItemNestedControlCollection>
                                </dx:LayoutItem>

                                <dx:LayoutItem Caption="RazonSocial">
                                    <LayoutItemNestedControlCollection>
                                        <dx:LayoutItemNestedControlContainer runat="server">
                                            <dx:ASPxTextBox ID="txtRazonSub" runat="server" Width="100%" Theme="Office2010Blue" />
                                        </dx:LayoutItemNestedControlContainer>
                                    </LayoutItemNestedControlCollection>
                                </dx:LayoutItem>

                                <dx:LayoutItem Caption="Telefonos">
                                    <LayoutItemNestedControlCollection>
                                        <dx:LayoutItemNestedControlContainer runat="server">
                                            <dx:ASPxTextBox ID="txtTelefonos" runat="server" Width="100%" Theme="Office2010Blue" />
                                        </dx:LayoutItemNestedControlContainer>
                                    </LayoutItemNestedControlCollection>
                                </dx:LayoutItem>

                                <dx:LayoutItem Caption="Whatsapp">
                                    <LayoutItemNestedControlCollection>
                                        <dx:LayoutItemNestedControlContainer runat="server">
                                            <dx:ASPxTextBox ID="txtWhatsapp" runat="server" Width="100%" Theme="Office2010Blue" />
                                        </dx:LayoutItemNestedControlContainer>
                                    </LayoutItemNestedControlCollection>
                                </dx:LayoutItem>

                                <dx:LayoutItem Caption="Mail">
                                    <LayoutItemNestedControlCollection>
                                        <dx:LayoutItemNestedControlContainer runat="server">
                                            <dx:ASPxTextBox ID="txtMail" runat="server" Width="100%" Theme="Office2010Blue" />
                                        </dx:LayoutItemNestedControlContainer>
                                    </LayoutItemNestedControlCollection>
                                </dx:LayoutItem>

                                <dx:LayoutItem Caption="CP">
                                    <LayoutItemNestedControlCollection>
                                        <dx:LayoutItemNestedControlContainer runat="server">
                                            <dx:ASPxTextBox ID="txtCPSub" runat="server" Width="100%" Theme="Office2010Blue" />
                                        </dx:LayoutItemNestedControlContainer>
                                    </LayoutItemNestedControlCollection>
                                </dx:LayoutItem>

                                <dx:LayoutItem Caption="TipoVialidad">
                                    <LayoutItemNestedControlCollection>
                                        <dx:LayoutItemNestedControlContainer runat="server">
                                            <dx:ASPxTextBox ID="txtTipoVialidad" runat="server" Width="100%" Theme="Office2010Blue" />
                                        </dx:LayoutItemNestedControlContainer>
                                    </LayoutItemNestedControlCollection>
                                </dx:LayoutItem>

                                <dx:LayoutItem Caption="NombreVialidad">
                                    <LayoutItemNestedControlCollection>
                                        <dx:LayoutItemNestedControlContainer runat="server">
                                            <dx:ASPxTextBox ID="txtNombreVialidad" runat="server" Width="100%" Theme="Office2010Blue" />
                                        </dx:LayoutItemNestedControlContainer>
                                    </LayoutItemNestedControlCollection>
                                </dx:LayoutItem>

                                <dx:LayoutItem Caption="NoExterior">
                                    <LayoutItemNestedControlCollection>
                                        <dx:LayoutItemNestedControlContainer runat="server">
                                            <dx:ASPxTextBox ID="txtNoExtSub" runat="server" Width="100%" Theme="Office2010Blue" />
                                        </dx:LayoutItemNestedControlContainer>
                                    </LayoutItemNestedControlCollection>
                                </dx:LayoutItem>

                                <dx:LayoutItem Caption="NoInterior">
                                    <LayoutItemNestedControlCollection>
                                        <dx:LayoutItemNestedControlContainer runat="server">
                                            <dx:ASPxTextBox ID="txtNoIntSub" runat="server" Width="100%" Theme="Office2010Blue" />
                                        </dx:LayoutItemNestedControlContainer>
                                    </LayoutItemNestedControlCollection>
                                </dx:LayoutItem>

                                <dx:LayoutItem Caption="Colonia">
                                    <LayoutItemNestedControlCollection>
                                        <dx:LayoutItemNestedControlContainer runat="server">
                                            <dx:ASPxTextBox ID="txtColSub" runat="server" Width="100%" Theme="Office2010Blue" />
                                        </dx:LayoutItemNestedControlContainer>
                                    </LayoutItemNestedControlCollection>
                                </dx:LayoutItem>

                                <dx:LayoutItem Caption="Localidad">
                                    <LayoutItemNestedControlCollection>
                                        <dx:LayoutItemNestedControlContainer runat="server">
                                            <dx:ASPxTextBox ID="txtLocSub" runat="server" Width="100%" Theme="Office2010Blue" />
                                        </dx:LayoutItemNestedControlContainer>
                                    </LayoutItemNestedControlCollection>
                                </dx:LayoutItem>

                                <dx:LayoutItem Caption="Municipio">
                                    <LayoutItemNestedControlCollection>
                                        <dx:LayoutItemNestedControlContainer runat="server">
                                            <dx:ASPxTextBox ID="txtMunSub" runat="server" Width="100%" Theme="Office2010Blue" />
                                        </dx:LayoutItemNestedControlContainer>
                                    </LayoutItemNestedControlCollection>
                                </dx:LayoutItem>

                                <dx:LayoutItem Caption="EntidadFederativa">
                                    <LayoutItemNestedControlCollection>
                                        <dx:LayoutItemNestedControlContainer runat="server">
                                            <dx:ASPxTextBox ID="txtEstadoSub" runat="server" Width="100%" Theme="Office2010Blue" />
                                        </dx:LayoutItemNestedControlContainer>
                                    </LayoutItemNestedControlCollection>
                                </dx:LayoutItem>

                                <dx:LayoutItem Caption="EntreCalle">
                                    <LayoutItemNestedControlCollection>
                                        <dx:LayoutItemNestedControlContainer runat="server">
                                            <dx:ASPxTextBox ID="txtEntreCalleSub" runat="server" Width="100%" Theme="Office2010Blue" />
                                        </dx:LayoutItemNestedControlContainer>
                                    </LayoutItemNestedControlCollection>
                                </dx:LayoutItem>

                                <dx:LayoutItem Caption="RFC">
                                    <LayoutItemNestedControlCollection>
                                        <dx:LayoutItemNestedControlContainer runat="server">
                                            <dx:ASPxTextBox ID="txtRFCSub" runat="server" Width="100%" Theme="Office2010Blue" />
                                        </dx:LayoutItemNestedControlContainer>
                                    </LayoutItemNestedControlCollection>
                                </dx:LayoutItem>

                                <dx:LayoutItem Caption="FechaAlta">
                                    <LayoutItemNestedControlCollection>
                                        <dx:LayoutItemNestedControlContainer runat="server">
                                            <dx:ASPxDateEdit ID="dtFechaAlta" runat="server" Width="100%" Theme="Office2010Blue" DisplayFormatString="dd/MM/yyyy" />
                                        </dx:LayoutItemNestedControlContainer>
                                    </LayoutItemNestedControlCollection>
                                </dx:LayoutItem>

                                <dx:LayoutItem Caption="FechaInicioSAT">
                                    <LayoutItemNestedControlCollection>
                                        <dx:LayoutItemNestedControlContainer runat="server">
                                            <dx:ASPxTextBox ID="txtFechaInicioSAT" runat="server" Width="100%" Theme="Office2010Blue" />
                                        </dx:LayoutItemNestedControlContainer>
                                    </LayoutItemNestedControlCollection>
                                </dx:LayoutItem>


                                <dx:LayoutItem Caption="RegimenFiscal">
                                    <LayoutItemNestedControlCollection>
                                        <dx:LayoutItemNestedControlContainer runat="server">
                                            <dx:ASPxGridLookup ID="glRegimenFiscal" runat="server" Width="100%" Theme="Office2010Blue" TextFormatString="{0}">
                                                <GridViewProperties>
                                                    <SettingsBehavior AllowFocusedRow="True" AllowSelectSingleRowOnly="True"></SettingsBehavior>

                                                    <Settings ShowFilterRow="True" ShowGroupPanel="False" />

                                                    <SettingsPopup>
                                                        <FilterControl AutoUpdatePosition="False"></FilterControl>
                                                    </SettingsPopup>
                                                </GridViewProperties>
                                            </dx:ASPxGridLookup>
                                        </dx:LayoutItemNestedControlContainer>
                                    </LayoutItemNestedControlCollection>
                                </dx:LayoutItem>

                                <dx:LayoutItem Caption="Activo">
                                    <LayoutItemNestedControlCollection>
                                        <dx:LayoutItemNestedControlContainer runat="server">
                                            <dx:ASPxCheckBox ID="chkActivo" runat="server" Text="Activo" Theme="Office2010Blue" />
                                        </dx:LayoutItemNestedControlContainer>
                                    </LayoutItemNestedControlCollection>
                                </dx:LayoutItem>
                            </Items>
                        </dx:LayoutGroup>

                        <dx:LayoutGroup Caption="Credenciales de Acceso">
                            <Items>
                                <dx:LayoutItem Caption="Usuario">
                                    <LayoutItemNestedControlCollection>
                                        <dx:LayoutItemNestedControlContainer runat="server">
                                            <dx:ASPxTextBox ID="txtUsuario" runat="server" />
                                        </dx:LayoutItemNestedControlContainer>
                                    </LayoutItemNestedControlCollection>
                                </dx:LayoutItem>

                                <dx:LayoutItem Caption="Contraseña">
                                    <LayoutItemNestedControlCollection>
                                        <dx:LayoutItemNestedControlContainer runat="server">
                                            <dx:ASPxTextBox ID="txtPassword" runat="server" Password="True" />
                                        </dx:LayoutItemNestedControlContainer>
                                    </LayoutItemNestedControlCollection>
                                </dx:LayoutItem>
                            </Items>
                        </dx:LayoutGroup>

                    </Items>
                </dx:ASPxFormLayout>

                <!-- Botón Guardar alineado a la derecha -->
                <div class="form-actions mt-3">
                    <asp:Button ID="btnGuardarSub" runat="server" CssClass="metro-button" Text="Guardar" />
                </div>

            </dx:PopupControlContentControl>
        </ContentCollection>
    </dx:ASPxPopupControl>

    <!-- Scripts de la página -->
    <script src="<%= ResolveUrl("~/Scripts/app/Catalogos/entidades.js") %>"></script>

</asp:Content>
