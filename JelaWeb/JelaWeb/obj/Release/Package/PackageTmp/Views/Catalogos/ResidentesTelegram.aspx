<%@ Page Title="Residentes Telegram" Language="vb" MasterPageFile="~/MasterPages/Jela.Master" AutoEventWireup="false" CodeBehind="ResidentesTelegram.aspx.vb" Inherits="JelaWeb.ResidentesTelegram" %>

<%@ Register Assembly="DevExpress.Web.v22.2, Version=22.2.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web" TagPrefix="dx" %>

<asp:Content ID="ContentHead" ContentPlaceHolderID="head" runat="server">
    <link href="<%= ResolveUrl("~/Content/CSS/residentes-telegram.css") %>" rel="stylesheet" type="text/css" />
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <div class="container-fluid">
        
        <!-- TABS PRINCIPALES -->
        <dx:ASPxPageControl ID="tabsResidentes" runat="server" ClientInstanceName="tabsResidentes" 
            Width="100%" Theme="Office2010Blue" ActiveTabIndex="0">
            <ClientSideEvents ActiveTabChanged="onTabChanged" />
            <TabPages>
                
                <!-- TAB 1: RESIDENTES -->
                <dx:TabPage Text="Residentes" Name="tabResidentes">
                    <ContentCollection>
                        <dx:ContentControl runat="server">
                            
                            <dx:ASPxGridView ID="gridResidentes" runat="server" ClientInstanceName="gridResidentes"
                                KeyFieldName="Id" Width="100%" Theme="Office2010Blue" AutoGenerateColumns="False"
                                OnDataBound="gridResidentes_DataBound">
                                
                                <ClientSideEvents ToolbarItemClick="onToolbarResidentesClick" />
                                
                                <SettingsPager Mode="ShowAllRecords" />
                                <Settings ShowFilterRow="False" ShowFilterRowMenu="True" ShowGroupPanel="True" />
                                <SettingsSearchPanel Visible="True" ShowApplyButton="True" ShowClearButton="True" />
                                <SettingsBehavior AllowSort="True" AllowGroup="True" AllowFocusedRow="True" AllowSelectByRowClick="True" />
                                <SettingsExport EnableClientSideExportAPI="True" ExcelExportMode="WYSIWYG" FileName="ResidentesTelegram" />
                                <SettingsContextMenu Enabled="True">
                                    <RowMenuItemVisibility>
                                        <EditRow Visible="False" />
                                        <DeleteRow Visible="False" />
                                        <NewRow Visible="False" />
                                    </RowMenuItemVisibility>
                                </SettingsContextMenu>
                                
                                <Toolbars>
                                    <dx:GridViewToolbar>
                                        <SettingsAdaptivity Enabled="True" EnableCollapseRootItemsToIcons="True" />
                                        <Items>
                                            <dx:GridViewToolbarItem Name="btnNuevoResidente" Text="Nuevo Residente">
                                                <Image IconID="actions_add_16x16" />
                                            </dx:GridViewToolbarItem>
                                            <dx:GridViewToolbarItem Name="btnEditarResidente" Text="Editar Residente">
                                                <Image IconID="edit_edit_16x16" />
                                            </dx:GridViewToolbarItem>
                                            <dx:GridViewToolbarItem Name="btnEnviarBlacklist" Text="Enviar a Blacklist" BeginGroup="True">
                                                <Image IconID="actions_deny_16x16" />
                                            </dx:GridViewToolbarItem>
                                            <dx:GridViewToolbarItem Name="btnVerLogs" Text="Ver Logs">
                                                <Image IconID="history_history_16x16" />
                                            </dx:GridViewToolbarItem>
                                            <dx:GridViewToolbarItem BeginGroup="True" Command="Refresh" />
                                            <dx:GridViewToolbarItem Command="ExportToXlsx" Text="Exportar Excel" />
                                        </Items>
                                    </dx:GridViewToolbar>
                                </Toolbars>
                                
                                <Columns>
                                    <dx:GridViewDataTextColumn FieldName="Id" Caption="ID" Width="60px" VisibleIndex="0" />
                                    <dx:GridViewDataTextColumn FieldName="ChatId" Caption="Chat ID" Width="120px" VisibleIndex="1" />
                                    <dx:GridViewDataTextColumn FieldName="Username" Caption="Username" Width="150px" VisibleIndex="2" />
                                    <dx:GridViewDataTextColumn FieldName="FirstName" Caption="Nombre" Width="150px" VisibleIndex="3" />
                                    <dx:GridViewDataTextColumn FieldName="LastName" Caption="Apellido" Width="150px" VisibleIndex="4" />
                                    <dx:GridViewDataComboBoxColumn FieldName="EstadoResidente" Caption="Estado" Width="100px" VisibleIndex="5">
                                        <PropertiesComboBox>
                                            <Items>
                                                <dx:ListEditItem Text="Activo" Value="activo" />
                                                <dx:ListEditItem Text="Bloqueado" Value="bloqueado" />
                                                <dx:ListEditItem Text="Suspendido" Value="suspendido" />
                                            </Items>
                                        </PropertiesComboBox>
                                    </dx:GridViewDataComboBoxColumn>
                                    <dx:GridViewDataComboBoxColumn FieldName="TipoResidente" Caption="Tipo" Width="100px" VisibleIndex="6">
                                        <PropertiesComboBox>
                                            <Items>
                                                <dx:ListEditItem Text="Standard" Value="standard" />
                                                <dx:ListEditItem Text="Premium" Value="premium" />
                                                <dx:ListEditItem Text="Trial" Value="trial" />
                                            </Items>
                                        </PropertiesComboBox>
                                    </dx:GridViewDataComboBoxColumn>
                                    <dx:GridViewDataSpinEditColumn FieldName="CreditosDisponibles" Caption="Créditos" Width="80px" VisibleIndex="7" />
                                    <dx:GridViewDataSpinEditColumn FieldName="TicketsMesActual" Caption="Tickets Mes" Width="90px" VisibleIndex="8" />
                                    <dx:GridViewDataSpinEditColumn FieldName="LimiteTicketsMes" Caption="Límite" Width="70px" VisibleIndex="9" />
                                    <dx:GridViewDataDateColumn FieldName="UltimaActividad" Caption="Última Actividad" Width="140px" VisibleIndex="10">
                                        <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy HH:mm" />
                                    </dx:GridViewDataDateColumn>
                                    <dx:GridViewDataDateColumn FieldName="FechaRegistro" Caption="Fecha Registro" Width="140px" VisibleIndex="11">
                                        <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy HH:mm" />
                                    </dx:GridViewDataDateColumn>
                                    <dx:GridViewDataCheckColumn FieldName="Activo" Caption="Activo" Width="70px" VisibleIndex="12" />
                                </Columns>
                                
                            </dx:ASPxGridView>
                            
                        </dx:ContentControl>
                    </ContentCollection>
                </dx:TabPage>
                
                <!-- TAB 2: BLACKLIST -->
                <dx:TabPage Text="Blacklist" Name="tabBlacklist">
                    <ContentCollection>
                        <dx:ContentControl runat="server">
                            
                            <dx:ASPxGridView ID="gridBlacklist" runat="server" ClientInstanceName="gridBlacklist"
                                KeyFieldName="Id" Width="100%" Theme="Office2010Blue" AutoGenerateColumns="False"
                                OnDataBound="gridBlacklist_DataBound">
                                
                                <ClientSideEvents ToolbarItemClick="onToolbarBlacklistClick" />
                                
                                <SettingsPager Mode="ShowAllRecords" />
                                <Settings ShowFilterRow="False" ShowFilterRowMenu="True" ShowGroupPanel="True" />
                                <SettingsSearchPanel Visible="True" ShowApplyButton="True" ShowClearButton="True" />
                                <SettingsBehavior AllowSort="True" AllowGroup="True" AllowFocusedRow="True" />
                                <SettingsExport EnableClientSideExportAPI="True" ExcelExportMode="WYSIWYG" FileName="BlacklistTelegram" />
                                
                                <Toolbars>
                                    <dx:GridViewToolbar>
                                        <Items>
                                            <dx:GridViewToolbarItem Name="btnRestaurar" Text="Restaurar Residente">
                                                <Image IconID="actions_undo_16x16" />
                                            </dx:GridViewToolbarItem>
                                            <dx:GridViewToolbarItem Name="btnVerRazon" Text="Ver Razón">
                                                <Image IconID="actions_info_16x16" />
                                            </dx:GridViewToolbarItem>
                                            <dx:GridViewToolbarItem BeginGroup="True" Command="Refresh" />
                                            <dx:GridViewToolbarItem Command="ExportToXlsx" Text="Exportar Excel" />
                                        </Items>
                                    </dx:GridViewToolbar>
                                </Toolbars>
                                
                                <Columns>
                                    <dx:GridViewDataTextColumn FieldName="Id" Caption="ID" Width="60px" VisibleIndex="0" />
                                    <dx:GridViewDataTextColumn FieldName="ChatId" Caption="Chat ID" Width="120px" VisibleIndex="1" />
                                    <dx:GridViewDataTextColumn FieldName="Username" Caption="Username" Width="150px" VisibleIndex="2" />
                                    <dx:GridViewDataTextColumn FieldName="RazonBloqueo" Caption="Razón Bloqueo" Width="300px" VisibleIndex="3" />
                                    <dx:GridViewDataDateColumn FieldName="FechaBloqueo" Caption="Fecha Bloqueo" Width="140px" VisibleIndex="4">
                                        <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy HH:mm" />
                                    </dx:GridViewDataDateColumn>
                                    <dx:GridViewDataTextColumn FieldName="BloqueadoPor" Caption="Bloqueado Por" Width="120px" VisibleIndex="5" />
                                    <dx:GridViewDataCheckColumn FieldName="Permanente" Caption="Permanente" Width="90px" VisibleIndex="6" />
                                    <dx:GridViewDataDateColumn FieldName="FechaLevantamiento" Caption="Fecha Levantamiento" Width="140px" VisibleIndex="7">
                                        <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy" />
                                    </dx:GridViewDataDateColumn>
                                </Columns>
                                
                            </dx:ASPxGridView>
                            
                        </dx:ContentControl>
                    </ContentCollection>
                </dx:TabPage>
                
            </TabPages>
        </dx:ASPxPageControl>
        
    </div>


    <!-- POPUP: NUEVO/EDITAR RESIDENTE -->
    <dx:ASPxPopupControl ID="popupResidente" runat="server" ClientInstanceName="popupResidente"
        HeaderText="Nuevo Residente" Width="600px" Modal="True" 
        PopupHorizontalAlign="WindowCenter" PopupVerticalAlign="WindowCenter"
        CloseAction="CloseButton" AllowDragging="True" CloseOnEscape="True">
        <ContentCollection>
            <dx:PopupControlContentControl runat="server">
                <dx:ASPxFormLayout ID="formResidente" runat="server" Width="100%" ColCount="2" Theme="Office2010Blue">
                    <Items>
                        <dx:LayoutItem Caption="Chat ID" RequiredMarkDisplayMode="Required">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxSpinEdit ID="txtChatId" runat="server" ClientInstanceName="txtChatId" 
                                        Width="100%" Theme="Office2010Blue" NumberType="Integer" MinValue="0" MaxValue="9999999999999">
                                        <ValidationSettings>
                                            <RequiredField IsRequired="True" ErrorText="El Chat ID es requerido" />
                                        </ValidationSettings>
                                    </dx:ASPxSpinEdit>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        
                        <dx:LayoutItem Caption="Username">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxTextBox ID="txtUsername" runat="server" ClientInstanceName="txtUsername" 
                                        Width="100%" Theme="Office2010Blue" MaxLength="255" NullText="@usuario" />
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        
                        <dx:LayoutItem Caption="Nombre">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxTextBox ID="txtFirstName" runat="server" ClientInstanceName="txtFirstName" 
                                        Width="100%" Theme="Office2010Blue" MaxLength="255" />
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        
                        <dx:LayoutItem Caption="Apellido">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxTextBox ID="txtLastName" runat="server" ClientInstanceName="txtLastName" 
                                        Width="100%" Theme="Office2010Blue" MaxLength="255" />
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        
                        <dx:LayoutItem Caption="Estado">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxComboBox ID="cmbEstadoResidente" runat="server" ClientInstanceName="cmbEstadoResidente" 
                                        Width="100%" Theme="Office2010Blue" ValueType="System.String">
                                        <Items>
                                            <dx:ListEditItem Text="Activo" Value="activo" Selected="True" />
                                            <dx:ListEditItem Text="Bloqueado" Value="bloqueado" />
                                            <dx:ListEditItem Text="Suspendido" Value="suspendido" />
                                        </Items>
                                    </dx:ASPxComboBox>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        
                        <dx:LayoutItem Caption="Tipo">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxComboBox ID="cmbTipoResidente" runat="server" ClientInstanceName="cmbTipoResidente" 
                                        Width="100%" Theme="Office2010Blue" ValueType="System.String">
                                        <Items>
                                            <dx:ListEditItem Text="Standard" Value="standard" Selected="True" />
                                            <dx:ListEditItem Text="Premium" Value="premium" />
                                            <dx:ListEditItem Text="Trial" Value="trial" />
                                        </Items>
                                    </dx:ASPxComboBox>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        
                        <dx:LayoutItem Caption="Créditos Disponibles">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxSpinEdit ID="txtCreditos" runat="server" ClientInstanceName="txtCreditos" 
                                        Width="100%" Theme="Office2010Blue" NumberType="Integer" MinValue="0" Number="10" />
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        
                        <dx:LayoutItem Caption="Límite Tickets/Mes">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxSpinEdit ID="txtLimiteTickets" runat="server" ClientInstanceName="txtLimiteTickets" 
                                        Width="100%" Theme="Office2010Blue" NumberType="Integer" MinValue="0" Number="50" />
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        
                        <dx:LayoutItem Caption="Activo" ColSpan="2">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxCheckBox ID="chkActivo" runat="server" ClientInstanceName="chkActivo" 
                                        Text="Residente activo" Checked="True" Theme="Office2010Blue" />
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                    </Items>
                </dx:ASPxFormLayout>
                
                <div class="popup-footer">
                    <dx:ASPxButton ID="btnGuardarResidente" runat="server" Text="Guardar Residente" 
                        Theme="Office2010Blue" AutoPostBack="True" OnClick="btnGuardarResidente_Click">
                        <Image IconID="save_save_16x16" />
                    </dx:ASPxButton>
                    <dx:ASPxButton ID="btnCancelarResidente" runat="server" Text="Cancelar" 
                        Theme="Office2010Blue" AutoPostBack="False">
                        <Image IconID="actions_cancel_16x16" />
                        <ClientSideEvents Click="function(s, e) { popupResidente.Hide(); }" />
                    </dx:ASPxButton>
                </div>
            </dx:PopupControlContentControl>
        </ContentCollection>
    </dx:ASPxPopupControl>
    
    <!-- POPUP: ENVIAR A BLACKLIST -->
    <dx:ASPxPopupControl ID="popupBlacklist" runat="server" ClientInstanceName="popupBlacklist"
        HeaderText="Enviar a Blacklist" Width="500px" Modal="True" 
        PopupHorizontalAlign="WindowCenter" PopupVerticalAlign="WindowCenter"
        CloseAction="CloseButton" AllowDragging="True" CloseOnEscape="True">
        <ContentCollection>
            <dx:PopupControlContentControl runat="server">
                <dx:ASPxFormLayout ID="formBlacklist" runat="server" Width="100%" Theme="Office2010Blue">
                    <Items>
                        <dx:LayoutItem Caption="Razón del Bloqueo" RequiredMarkDisplayMode="Required">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxMemo ID="txtRazonBloqueo" runat="server" ClientInstanceName="txtRazonBloqueo" 
                                        Width="100%" Height="100px" Theme="Office2010Blue">
                                        <ValidationSettings>
                                            <RequiredField IsRequired="True" ErrorText="La razón es requerida" />
                                        </ValidationSettings>
                                    </dx:ASPxMemo>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        
                        <dx:LayoutItem Caption="Bloqueo Permanente">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxCheckBox ID="chkPermanente" runat="server" ClientInstanceName="chkPermanente" 
                                        Text="El bloqueo es permanente" Theme="Office2010Blue" />
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        
                        <dx:LayoutItem Caption="Fecha Levantamiento">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxDateEdit ID="dteFechaLevantamiento" runat="server" ClientInstanceName="dteFechaLevantamiento" 
                                        Width="100%" Theme="Office2010Blue" NullText="Solo si no es permanente" />
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        
                        <dx:LayoutItem Caption="Notas Adicionales">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxMemo ID="txtNotasBlacklist" runat="server" ClientInstanceName="txtNotasBlacklist" 
                                        Width="100%" Height="60px" Theme="Office2010Blue" />
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                    </Items>
                </dx:ASPxFormLayout>
                
                <div class="popup-footer">
                    <dx:ASPxButton ID="btnConfirmarBlacklist" runat="server" Text="Confirmar Bloqueo" 
                        Theme="Office2010Blue" AutoPostBack="True" OnClick="btnConfirmarBlacklist_Click">
                        <Image IconID="actions_deny_16x16" />
                    </dx:ASPxButton>
                    <dx:ASPxButton ID="btnCancelarBlacklist" runat="server" Text="Cancelar" 
                        Theme="Office2010Blue" AutoPostBack="False">
                        <Image IconID="actions_cancel_16x16" />
                        <ClientSideEvents Click="function(s, e) { popupBlacklist.Hide(); }" />
                    </dx:ASPxButton>
                </div>
            </dx:PopupControlContentControl>
        </ContentCollection>
    </dx:ASPxPopupControl>
    
    <!-- HIDDEN FIELDS -->
    <asp:HiddenField ID="hfResidenteId" runat="server" Value="0" />
    <asp:HiddenField ID="hfChatIdBlacklist" runat="server" Value="0" />

</asp:Content>

<asp:Content ID="ContentScripts" ContentPlaceHolderID="scripts" runat="server">
    <script src="<%= ResolveUrl("~/Scripts/app/Catalogos/residentes-telegram.js") %>" type="text/javascript"></script>
</asp:Content>
