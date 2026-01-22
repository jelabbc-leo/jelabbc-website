<%@ Page Title="Entidades" Language="vb" MasterPageFile="~/MasterPages/Jela.Master" AutoEventWireup="false" CodeBehind="Entidades.aspx.vb" Inherits="JelaWeb.Entidades" %>

<%@ Register Assembly="DevExpress.Web.Bootstrap.v22.2, Version=22.2.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web.Bootstrap" TagPrefix="dx" %>
<%@ Register Assembly="DevExpress.Web.v22.2" Namespace="DevExpress.Web" TagPrefix="dx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="<%= ResolveUrl("~/Content/CSS/modules/entidades.css") %>" rel="stylesheet" type="text/css" />
</asp:Content>
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
                    ClientInstanceName="gridEntidades" Theme="Office2010Blue" AutoGenerateColumns="False">
                    
                    <SettingsPager Mode="ShowAllRecords" />
                    <Settings ShowFilterRow="False" ShowFilterRowMenu="True" ShowGroupPanel="True" />
                    <SettingsBehavior AllowSort="True" AllowGroup="True" AllowFixedGroups="True" />
                    <SettingsSearchPanel Visible="True" ShowApplyButton="True" ShowClearButton="True" />
                    
                    <SettingsCommandButton>
                        <NewButton Text="Nueva Entidad">
                            <Image IconID="actions_add_16x16" />
                        </NewButton>
                        <EditButton Text="Editar Entidad">
                            <Image IconID="edit_edit_16x16" />
                        </EditButton>
                        <DeleteButton Text="Eliminar Entidad">
                            <Image IconID="edit_delete_16x16" />
                        </DeleteButton>
                        <UpdateButton Text="Guardar">
                            <Image IconID="save_save_16x16" />
                        </UpdateButton>
                        <CancelButton Text="Cancelar">
                            <Image IconID="actions_cancel_16x16" />
                        </CancelButton>
                    </SettingsCommandButton>
                    
                    <Toolbars>
                        <dx:GridViewToolbar>
                            <Items>
                                <dx:GridViewToolbarItem Command="New" />
                                <dx:GridViewToolbarItem Command="Edit" />
                                <dx:GridViewToolbarItem Command="Delete" />
                                <dx:GridViewToolbarItem BeginGroup="true" Command="Refresh" />
                                <dx:GridViewToolbarItem Command="ExportToPdf" />
                                <dx:GridViewToolbarItem Command="ExportToXlsx" />
                            </Items>
                        </dx:GridViewToolbar>
                    </Toolbars>
                    
                    <SettingsPopup>
                        <FilterControl AutoUpdatePosition="False"></FilterControl>
                    </SettingsPopup>
                    
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


    <!-- ================================================================== -->
    <!-- POPUP: ADMINISTRAR ENTIDAD -->
    <!-- ================================================================== -->
    <dx:ASPxPopupControl ID="popupEntidad" runat="server" ClientInstanceName="popupEntidad"
        HeaderText="Administrar Entidad" Width="1000px" Modal="True"
        PopupHorizontalAlign="WindowCenter" PopupVerticalAlign="WindowCenter"
        CloseAction="CloseButton" AllowDragging="True" CloseOnEscape="True">
        
        <ClientSideEvents 
            Shown="function(s, e) { console.log('🔧 Popup Entidad mostrado'); }"
            Closing="function(s, e) { console.log('🔧 Popup Entidad cerrándose'); }"
            CloseUp="function(s, e) { console.log('🔧 Popup Entidad cerrado'); }" />
        
        <ContentCollection>
            <dx:PopupControlContentControl runat="server">
                <dx:ASPxFormLayout ID="formEntidad" runat="server" Width="100%" ColCount="2" Theme="Office2010Blue">
                    <Items>
                        <dx:LayoutItem Caption="Clave" RequiredMarkDisplayMode="Required">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                    <dx:ASPxTextBox ID="txtClave" runat="server" ClientInstanceName="txtClave" Width="100%" MaxLength="10">
                                        <ValidationSettings RequiredField-IsRequired="true" RequiredField-ErrorText="La clave es obligatoria" />
                                    </dx:ASPxTextBox>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        
                        <dx:LayoutItem Caption="Alias">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                    <dx:ASPxTextBox ID="txtAlias" runat="server" ClientInstanceName="txtAlias" Width="100%" MaxLength="50" />
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        
                        <dx:LayoutItem Caption="Razón Social" RequiredMarkDisplayMode="Required" ColSpan="2">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                    <dx:ASPxTextBox ID="txtRazonSocial" runat="server" ClientInstanceName="txtRazonSocial" Width="100%" MaxLength="200">
                                        <ValidationSettings RequiredField-IsRequired="true" RequiredField-ErrorText="La razón social es obligatoria" />
                                    </dx:ASPxTextBox>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        
                        <dx:LayoutItem Caption="RFC">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                    <dx:ASPxTextBox ID="txtRFC" runat="server" ClientInstanceName="txtRFC" Width="100%" MaxLength="13" />
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        
                        <dx:LayoutItem Caption="Tipo Condominio">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                    <dx:ASPxComboBox ID="cmbTipoCondominio" runat="server" ClientInstanceName="cmbTipoCondominio" Width="100%">
                                        <Items>
                                            <dx:ListEditItem Text="Vertical" Value="Vertical" />
                                            <dx:ListEditItem Text="Horizontal" Value="Horizontal" />
                                        </Items>
                                    </dx:ASPxComboBox>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        
                        <dx:LayoutItem Caption="Número de Unidades">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                    <dx:ASPxSpinEdit ID="spnNumeroUnidades" runat="server" ClientInstanceName="spnNumeroUnidades" Width="100%" MinValue="0" MaxValue="10000" />
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        
                        <dx:LayoutItem Caption="Dirección" ColSpan="2">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                    <dx:ASPxMemo ID="memoDireccion" runat="server" ClientInstanceName="memoDireccion" Width="100%" Rows="3" />
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        
                        <dx:LayoutItem Caption="Ciudad">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                    <dx:ASPxTextBox ID="txtCiudad" runat="server" ClientInstanceName="txtCiudad" Width="100%" MaxLength="100" />
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        
                        <dx:LayoutItem Caption="Estado">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                    <dx:ASPxTextBox ID="txtEstado" runat="server" ClientInstanceName="txtEstado" Width="100%" MaxLength="50" />
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        
                        <dx:LayoutItem Caption="Código Postal">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                    <dx:ASPxTextBox ID="txtCodigoPostal" runat="server" ClientInstanceName="txtCodigoPostal" Width="100%" MaxLength="10" />
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        
                        <dx:LayoutItem Caption="Teléfono">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                    <dx:ASPxTextBox ID="txtTelefono" runat="server" ClientInstanceName="txtTelefono" Width="100%" MaxLength="20" />
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        
                        <dx:LayoutItem Caption="Email">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                    <dx:ASPxTextBox ID="txtEmail" runat="server" ClientInstanceName="txtEmail" Width="100%" MaxLength="100" />
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        
                        <dx:LayoutItem Caption="Sitio Web">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                    <dx:ASPxTextBox ID="txtSitioWeb" runat="server" ClientInstanceName="txtSitioWeb" Width="100%" MaxLength="200" />
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        
                        <dx:LayoutItem Caption="Activo">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                    <dx:ASPxCheckBox ID="chkActivo" runat="server" ClientInstanceName="chkActivo" Checked="True" />
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        
                        <dx:LayoutItem Caption="Es Condominio">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer>
                                    <dx:ASPxCheckBox ID="chkEsCondominio" runat="server" ClientInstanceName="chkEsCondominio" Checked="True" />
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                    </Items>
                </dx:ASPxFormLayout>
                
                <div class="entidades-popup-footer">
                    <dx:ASPxButton ID="btnGuardarEntidad" runat="server" Text="Guardar" Theme="Office2010Blue"
                        AutoPostBack="False">
                        <Image IconID="save_save_16x16" />
                        <ClientSideEvents Click="function(s, e) { EntidadesModule.guardar(); }" />
                    </dx:ASPxButton>
                    <dx:ASPxButton ID="btnCerrarEntidad" runat="server" Text="Cerrar" Theme="Office2010Blue" AutoPostBack="False">
                        <Image IconID="actions_cancel_16x16" />
                        <ClientSideEvents Click="function(s, e) { popupEntidad.Hide(); }" />
                    </dx:ASPxButton>
                </div>
            </dx:PopupControlContentControl>
        </ContentCollection>
    </dx:ASPxPopupControl>

    <!-- Hidden Fields -->
    <asp:HiddenField ID="hfEntidadId" runat="server" Value="0" ClientIDMode="Static" />

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="scripts" runat="server">
    <script src="<%= ResolveUrl("~/Scripts/app/Catalogos/entidades.js") %>?v=20260121" type="text/javascript"></script>
</asp:Content>
