<%@ Page Title="Tickets de Atención al Cliente" Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/Jela.Master" CodeBehind="Tickets.aspx.vb" Inherits="JelaWeb.Tickets" %>

<%@ Register Assembly="DevExpress.Web.v22.2, Version=22.2.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web" TagPrefix="dx" %>

<asp:Content ID="ContentHead" ContentPlaceHolderID="head" runat="server">
    <link href="<%= ResolveUrl("~/Content/CSS/tickets.css") %>" rel="stylesheet" type="text/css" />
    <script src="<%= ResolveUrl("~/Scripts/app/Operacion/Tickets/tickets.js") %>" type="text/javascript"></script>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <div class="container-fluid">

        <!-- FILTROS SUPERIORES - SOLO RANGO DE FECHAS -->
        <dx:ASPxFormLayout ID="formFiltrosTickets" runat="server" ColCount="3" Width="100%" Theme="Office2010Blue" AlignItemCaptionsInAllGroups="True" BackColor="#E4EFFA">
            <Items>
                <dx:LayoutItem Caption="Fecha Desde:">
                    <LayoutItemNestedControlCollection>
                        <dx:LayoutItemNestedControlContainer runat="server">
                            <dx:ASPxDateEdit ID="dteFechaDesde" runat="server" ClientInstanceName="dteFechaDesde" 
                                Width="100%" Theme="Office2010Blue" 
                                NullText="Seleccione fecha inicial">
                            </dx:ASPxDateEdit>
                        </dx:LayoutItemNestedControlContainer>
                    </LayoutItemNestedControlCollection>
                    <CaptionSettings VerticalAlign="Middle" />
                </dx:LayoutItem>

                <dx:LayoutItem Caption="Fecha Hasta:">
                    <LayoutItemNestedControlCollection>
                        <dx:LayoutItemNestedControlContainer runat="server">
                            <dx:ASPxDateEdit ID="dteFechaHasta" runat="server" ClientInstanceName="dteFechaHasta" 
                                Width="100%" Theme="Office2010Blue" 
                                NullText="Seleccione fecha final">
                            </dx:ASPxDateEdit>
                        </dx:LayoutItemNestedControlContainer>
                    </LayoutItemNestedControlCollection>
                    <CaptionSettings VerticalAlign="Middle" />
                </dx:LayoutItem>

                <dx:LayoutItem ShowCaption="False">
                    <LayoutItemNestedControlCollection>
                        <dx:LayoutItemNestedControlContainer runat="server">
                            <dx:ASPxButton ID="btnFiltrar" runat="server" Text="Aplicar Filtros" Theme="Office2010Blue" AutoPostBack="True" OnClick="btnFiltrar_Click">
                                <Image IconID="filter_filter_16x16"></Image>
                            </dx:ASPxButton>
                            <dx:ASPxButton ID="btnLimpiar" runat="server" Text="Limpiar" Theme="Office2010Blue" AutoPostBack="True" OnClick="btnLimpiar_Click" Style="margin-left: 5px;">
                                <Image IconID="actions_clear_16x16"></Image>
                            </dx:ASPxButton>
                        </dx:LayoutItemNestedControlContainer>
                    </LayoutItemNestedControlCollection>
                    <CaptionSettings VerticalAlign="Middle" />
                </dx:LayoutItem>
            </Items>
        </dx:ASPxFormLayout>

        <br />

        <!-- GRID CON TOOLBAR INTEGRADO -->
        <dx:ASPxGridView ID="gridTickets" runat="server" ClientInstanceName="gridTickets" 
            KeyFieldName="Id" Width="100%" Theme="Office2010Blue" 
            AutoGenerateColumns="False">
            <ClientSideEvents ToolbarItemClick="onToolbarTicketsClick" 
                RowClick="OnGridRowClick" 
                RowDblClick="OnGridRowDblClick" />
            <Settings ShowHeaderFilterButton="True" ShowFilterRow="False" ShowFilterRowMenu="False" 
                ShowGroupPanel="True" VerticalScrollableHeight="500" VerticalScrollBarMode="Visible" />
            <SettingsResizing ColumnResizeMode="NextColumn" />
            <SettingsPopup>
                <FilterControl AutoUpdatePosition="False"></FilterControl>
            </SettingsPopup>
            <SettingsSearchPanel Visible="True" />
            <SettingsBehavior AllowFocusedRow="True" AllowSelectByRowClick="True" AllowDragDrop="True" 
                EnableRowHotTrack="True" />
            <SettingsDataSecurity AllowDelete="False" AllowEdit="False" AllowInsert="False" />
            <SettingsPager Mode="ShowAllRecords">
            </SettingsPager>
            <GroupSummary>
                <dx:ASPxSummaryItem SummaryType="Count" FieldName="Id" DisplayFormat="Registros: {0}" />
            </GroupSummary>
            <SettingsExport FileName="Tickets">
            </SettingsExport>
            <Toolbars>
                <dx:GridViewToolbar>
                    <SettingsAdaptivity Enabled="true" EnableCollapseRootItemsToIcons="true" />
                    <Items>
                        <dx:GridViewToolbarItem Name="btnNuevoTicket" Text="Nuevo Ticket" BeginGroup="True" GroupName="grp1">
                            <Image IconID="mail_newmail_16x16"></Image>
                        </dx:GridViewToolbarItem>
                        <dx:GridViewToolbarItem Name="btnVerTicket" Text="Ver Ticket" GroupName="grp1">
                            <Image IconID="find_find_16x16"></Image>
                        </dx:GridViewToolbarItem>
                        <dx:GridViewToolbarItem Name="btnResolverConIA" Text="Resolver con IA" GroupName="grp1">
                            <Image IconID="spellcheck_spellcheck_16x16"></Image>
                        </dx:GridViewToolbarItem>
                        <dx:GridViewToolbarItem BeginGroup="True" GroupName="grp2" />
                        <dx:GridViewToolbarItem Command="Refresh" GroupName="grp2" />
                        <dx:GridViewToolbarItem Command="ExportToXlsx" GroupName="grp2" />
                    </Items>
                </dx:GridViewToolbar>
            </Toolbars>
            <Columns>
                <dx:GridViewDataTextColumn FieldName="Id" Caption="ID" Width="70px" ReadOnly="True" VisibleIndex="0">
                    <Settings AllowAutoFilter="True" AllowHeaderFilter="True" AllowGroup="True" />
                </dx:GridViewDataTextColumn>
                <dx:GridViewDataTextColumn FieldName="AsuntoCorto" Caption="Asunto" Width="250px" ReadOnly="True" VisibleIndex="1">
                    <Settings AllowAutoFilter="True" AllowHeaderFilter="True" AllowGroup="True" />
                </dx:GridViewDataTextColumn>
                <dx:GridViewDataTextColumn FieldName="NombreCompleto" Caption="Cliente" Width="200px" ReadOnly="True" VisibleIndex="2">
                    <Settings AllowAutoFilter="True" AllowHeaderFilter="True" AllowGroup="True" />
                </dx:GridViewDataTextColumn>
                <dx:GridViewDataComboBoxColumn FieldName="Canal" Caption="Canal" Width="120px" ReadOnly="True" VisibleIndex="3">
                    <PropertiesComboBox>
                        <Items>
                            <dx:ListEditItem Text="Email" Value="Email" />
                            <dx:ListEditItem Text="Chat" Value="Chat" />
                            <dx:ListEditItem Text="Telefono" Value="Telefono" />
                            <dx:ListEditItem Text="RedesSociales" Value="RedesSociales" />
                            <dx:ListEditItem Text="App" Value="App" />
                        </Items>
                    </PropertiesComboBox>
                    <Settings AllowAutoFilter="True" AllowHeaderFilter="True" AllowGroup="True" />
                </dx:GridViewDataComboBoxColumn>
                <dx:GridViewDataComboBoxColumn FieldName="Estado" Caption="Estado" Width="120px" ReadOnly="True" VisibleIndex="4">
                    <PropertiesComboBox>
                        <Items>
                            <dx:ListEditItem Text="Abierto" Value="Abierto" />
                            <dx:ListEditItem Text="EnProceso" Value="EnProceso" />
                            <dx:ListEditItem Text="Resuelto" Value="Resuelto" />
                            <dx:ListEditItem Text="Cerrado" Value="Cerrado" />
                            <dx:ListEditItem Text="Cancelado" Value="Cancelado" />
                        </Items>
                    </PropertiesComboBox>
                    <Settings AllowAutoFilter="True" AllowHeaderFilter="True" AllowGroup="True" />
                </dx:GridViewDataComboBoxColumn>
                <dx:GridViewDataComboBoxColumn FieldName="PrioridadAsignada" Caption="Prioridad" Width="100px" ReadOnly="True" VisibleIndex="5">
                    <PropertiesComboBox>
                        <Items>
                            <dx:ListEditItem Text="Baja" Value="Baja" />
                            <dx:ListEditItem Text="Media" Value="Media" />
                            <dx:ListEditItem Text="Alta" Value="Alta" />
                            <dx:ListEditItem Text="Crítica" Value="Crítica" />
                        </Items>
                    </PropertiesComboBox>
                    <Settings AllowAutoFilter="True" AllowHeaderFilter="True" AllowGroup="True" />
                </dx:GridViewDataComboBoxColumn>
                <dx:GridViewDataTextColumn FieldName="SentimientoDetectado" Caption="Sentimiento" Width="120px" ReadOnly="True" VisibleIndex="6">
                    <Settings AllowAutoFilter="True" AllowHeaderFilter="True" AllowGroup="True" />
                </dx:GridViewDataTextColumn>
                <dx:GridViewDataTextColumn FieldName="CategoriaAsignada" Caption="Categoría" Width="150px" ReadOnly="True" VisibleIndex="7">
                    <Settings AllowAutoFilter="True" AllowHeaderFilter="True" AllowGroup="True" />
                </dx:GridViewDataTextColumn>
                <dx:GridViewDataTextColumn FieldName="EmailCliente" Caption="Email" Width="200px" ReadOnly="True" VisibleIndex="8">
                    <Settings AllowAutoFilter="True" AllowHeaderFilter="True" AllowGroup="True" />
                </dx:GridViewDataTextColumn>
                <dx:GridViewDataDateColumn FieldName="FechaCreacion" Caption="Fecha Creación" Width="150px" ReadOnly="True" VisibleIndex="9">
                    <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy HH:mm" />
                    <Settings AllowAutoFilter="True" AllowHeaderFilter="True" AllowGroup="True" />
                </dx:GridViewDataDateColumn>
            </Columns>
        </dx:ASPxGridView>
        <dx:ASPxGridViewExporter ID="gridExporter" runat="server" GridViewID="gridTickets"></dx:ASPxGridViewExporter>

        <!-- POPUP MODAL PARA CAPTURA Y DETALLE -->
        <dx:ASPxPopupControl ID="popupTicket" runat="server" ClientInstanceName="popupTicket"
            HeaderText="Nuevo Ticket" Width="1100px" Height="750px"
            Modal="True" PopupHorizontalAlign="WindowCenter" PopupVerticalAlign="WindowCenter"
            CloseAction="CloseButton" AllowDragging="True" CloseOnEscape="True">
            <ContentCollection>
                <dx:PopupControlContentControl runat="server">
                    <dx:ASPxPageControl ID="tabsTicket" runat="server" ClientInstanceName="tabsTicket" Width="100%" Height="650px" Theme="Office2010Blue">
                        <TabPages>
                            <dx:TabPage Text="Cliente" Name="tabCliente">
                                <ContentCollection>
                                    <dx:ContentControl runat="server">
                                        <dx:ASPxFormLayout ID="formCliente" runat="server" Width="100%" ColCount="2" Theme="Office2010Blue" AlignItemCaptionsInAllGroups="True">
                                            <Items>
                                                <dx:LayoutItem Caption="Canal" RequiredMarkDisplayMode="Required">
                                                    <LayoutItemNestedControlCollection>
                                                        <dx:LayoutItemNestedControlContainer runat="server">
                                                            <dx:ASPxComboBox ID="cmbCanal" runat="server" ClientInstanceName="cmbCanal" 
                                                                Width="100%" Theme="Office2010Blue" ValueType="System.String" 
                                                                NullText="Seleccione un canal">
                                                                <Items>
                                                                    <dx:ListEditItem Text="Email" Value="Email" />
                                                                    <dx:ListEditItem Text="Chat" Value="Chat" />
                                                                    <dx:ListEditItem Text="Telefono" Value="Telefono" />
                                                                    <dx:ListEditItem Text="RedesSociales" Value="RedesSociales" />
                                                                    <dx:ListEditItem Text="App" Value="App" />
                                                                </Items>
                                                                <ValidationSettings>
                                                                    <RequiredField IsRequired="True" ErrorText="El canal es requerido" />
                                                                </ValidationSettings>
                                                            </dx:ASPxComboBox>
                                                        </dx:LayoutItemNestedControlContainer>
                                                    </LayoutItemNestedControlCollection>
                                                </dx:LayoutItem>

                                                <dx:LayoutItem Caption="Estado">
                                                    <LayoutItemNestedControlCollection>
                                                        <dx:LayoutItemNestedControlContainer runat="server">
                                                            <dx:ASPxComboBox ID="cmbEstado" runat="server" ClientInstanceName="cmbEstado" 
                                                                Width="100%" Theme="Office2010Blue" ValueType="System.String">
                                                                <Items>
                                                                    <dx:ListEditItem Text="Abierto" Value="Abierto" />
                                                                    <dx:ListEditItem Text="EnProceso" Value="EnProceso" />
                                                                    <dx:ListEditItem Text="Resuelto" Value="Resuelto" />
                                                                    <dx:ListEditItem Text="Cerrado" Value="Cerrado" />
                                                                    <dx:ListEditItem Text="Cancelado" Value="Cancelado" />
                                                                </Items>
                                                            </dx:ASPxComboBox>
                                                        </dx:LayoutItemNestedControlContainer>
                                                    </LayoutItemNestedControlCollection>
                                                </dx:LayoutItem>

                                                <dx:LayoutItem Caption="Nombre Completo" RequiredMarkDisplayMode="Required">
                                                    <LayoutItemNestedControlCollection>
                                                        <dx:LayoutItemNestedControlContainer runat="server">
                                                            <dx:ASPxTextBox ID="txtNombreCompleto" runat="server" ClientInstanceName="txtNombreCompleto" 
                                                                Width="100%" Theme="Office2010Blue" MaxLength="200">
                                                                <ValidationSettings>
                                                                    <RequiredField IsRequired="True" ErrorText="El nombre es requerido" />
                                                                </ValidationSettings>
                                                            </dx:ASPxTextBox>
                                                        </dx:LayoutItemNestedControlContainer>
                                                    </LayoutItemNestedControlCollection>
                                                </dx:LayoutItem>

                                                <dx:LayoutItem Caption="Email">
                                                    <LayoutItemNestedControlCollection>
                                                        <dx:LayoutItemNestedControlContainer runat="server">
                                                            <dx:ASPxTextBox ID="txtEmailCliente" runat="server" ClientInstanceName="txtEmailCliente" 
                                                                Width="100%" Theme="Office2010Blue" MaxLength="255">
                                                            </dx:ASPxTextBox>
                                                        </dx:LayoutItemNestedControlContainer>
                                                    </LayoutItemNestedControlCollection>
                                                </dx:LayoutItem>

                                                <dx:LayoutItem Caption="Teléfono">
                                                    <LayoutItemNestedControlCollection>
                                                        <dx:LayoutItemNestedControlContainer runat="server">
                                                            <dx:ASPxTextBox ID="txtTelefonoCliente" runat="server" ClientInstanceName="txtTelefonoCliente" 
                                                                Width="100%" Theme="Office2010Blue" MaxLength="50">
                                                            </dx:ASPxTextBox>
                                                        </dx:LayoutItemNestedControlContainer>
                                                    </LayoutItemNestedControlCollection>
                                                </dx:LayoutItem>

                                                <dx:LayoutItem Caption="Agente Asignado">
                                                    <LayoutItemNestedControlCollection>
                                                        <dx:LayoutItemNestedControlContainer runat="server">
                                                            <dx:ASPxComboBox ID="cmbAgenteAsignado" runat="server" ClientInstanceName="cmbAgenteAsignado" 
                                                                Width="100%" Theme="Office2010Blue" ValueType="System.Int32" 
                                                                NullText="Sin asignar">
                                                            </dx:ASPxComboBox>
                                                        </dx:LayoutItemNestedControlContainer>
                                                    </LayoutItemNestedControlCollection>
                                                </dx:LayoutItem>

                                                <dx:LayoutItem Caption="Mensaje Original" ColSpan="2" RequiredMarkDisplayMode="Required">
                                                    <LayoutItemNestedControlCollection>
                                                        <dx:LayoutItemNestedControlContainer runat="server">
                                                            <dx:ASPxMemo ID="txtMensajeOriginal" runat="server" ClientInstanceName="txtMensajeOriginal" 
                                                                Width="100%" Height="300px" Theme="Office2010Blue">
                                                                <ValidationSettings>
                                                                    <RequiredField IsRequired="True" ErrorText="El mensaje es requerido" />
                                                                </ValidationSettings>
                                                            </dx:ASPxMemo>
                                                        </dx:LayoutItemNestedControlContainer>
                                                    </LayoutItemNestedControlCollection>
                                                </dx:LayoutItem>
                                            </Items>
                                        </dx:ASPxFormLayout>
                                    </dx:ContentControl>
                                </ContentCollection>
                            </dx:TabPage>

                            <dx:TabPage Text="Conversación" Name="tabConversacion">
                                <ContentCollection>
                                    <dx:ContentControl runat="server">
                                        <dx:ASPxGridView ID="gridConversacion" runat="server" ClientInstanceName="gridConversacion"
                                            Width="100%" KeyFieldName="Id" AutoGenerateColumns="False" Theme="Office2010Blue"
                                            OnCustomCallback="gridConversacion_CustomCallback">
                                            <Settings ShowStatusBar="Hidden" VerticalScrollBarMode="Visible" />
                                            <SettingsBehavior AllowFocusedRow="True" />
                                            <SettingsPager Mode="ShowAllRecords" />

<SettingsPopup>
<FilterControl AutoUpdatePosition="False"></FilterControl>
</SettingsPopup>
                                            <Columns>
                                                <dx:GridViewDataTextColumn FieldName="Id" Caption="ID" Width="50px" ReadOnly="True" VisibleIndex="0">
                                                </dx:GridViewDataTextColumn>
                                                <dx:GridViewDataTextColumn FieldName="TipoMensaje" Caption="Tipo" Width="100px" ReadOnly="True" VisibleIndex="1">
                                                </dx:GridViewDataTextColumn>
                                                <dx:GridViewDataTextColumn FieldName="Mensaje" Caption="Mensaje" Width="100%" ReadOnly="True" VisibleIndex="2">
                                                </dx:GridViewDataTextColumn>
                                                <dx:GridViewDataTextColumn FieldName="NombreUsuarioEnvio" Caption="Enviado Por" Width="150px" ReadOnly="True" VisibleIndex="3">
                                                </dx:GridViewDataTextColumn>
                                                <dx:GridViewDataDateColumn FieldName="FechaEnvio" Caption="Fecha" Width="150px" ReadOnly="True" VisibleIndex="4">
                                                    <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy HH:mm" />
                                                </dx:GridViewDataDateColumn>
                                            </Columns>
                                        </dx:ASPxGridView>
                                        <br />
                                        <dx:ASPxFormLayout ID="formNuevoMensaje" runat="server" Width="100%" Theme="Office2010Blue">
                                            <Items>
                                                <dx:LayoutItem Caption="Nuevo Mensaje">
                                                    <LayoutItemNestedControlCollection>
                                                        <dx:LayoutItemNestedControlContainer runat="server">
                                                            <dx:ASPxMemo ID="txtNuevoMensaje" runat="server" ClientInstanceName="txtNuevoMensaje" 
                                                                Width="100%" Height="100px" Theme="Office2010Blue">
                                                            </dx:ASPxMemo>
                                                        </dx:LayoutItemNestedControlContainer>
                                                    </LayoutItemNestedControlCollection>
                                                </dx:LayoutItem>
                                                <dx:LayoutItem ShowCaption="False">
                                                    <LayoutItemNestedControlCollection>
                                                        <dx:LayoutItemNestedControlContainer runat="server">
                                                            <dx:ASPxButton ID="btnEnviarMensaje" runat="server" Text="Enviar Mensaje" Theme="Office2010Blue"
                                                                AutoPostBack="True" OnClick="btnEnviarMensaje_Click">
                                                                <Image IconID="mail_send_16x16"></Image>
                                                            </dx:ASPxButton>
                                                        </dx:LayoutItemNestedControlContainer>
                                                    </LayoutItemNestedControlCollection>
                                                </dx:LayoutItem>
                                            </Items>
                                        </dx:ASPxFormLayout>
                                    </dx:ContentControl>
                                </ContentCollection>
                            </dx:TabPage>

                            <dx:TabPage Text="Resumen IA" Name="tabResumenIA">
                                <ContentCollection>
                                    <dx:ContentControl runat="server">
                                        <dx:ASPxFormLayout ID="formResumenIA" runat="server" Width="100%" ColCount="2" Theme="Office2010Blue" AlignItemCaptionsInAllGroups="True">
                                            <Items>
                                                <dx:LayoutItem Caption="Asunto Corto" ColSpan="2">
                                                    <LayoutItemNestedControlCollection>
                                                        <dx:LayoutItemNestedControlContainer runat="server">
                                                            <dx:ASPxTextBox ID="txtAsuntoCorto" runat="server" ClientInstanceName="txtAsuntoCorto" 
                                                                Width="100%" Theme="Office2010Blue" MaxLength="200" ReadOnly="True">
                                                            </dx:ASPxTextBox>
                                                        </dx:LayoutItemNestedControlContainer>
                                                    </LayoutItemNestedControlCollection>
                                                </dx:LayoutItem>

                                                <dx:LayoutItem Caption="Resumen IA" ColSpan="2">
                                                    <LayoutItemNestedControlCollection>
                                                        <dx:LayoutItemNestedControlContainer runat="server">
                                                            <dx:ASPxMemo ID="txtResumenIA" runat="server" ClientInstanceName="txtResumenIA" 
                                                                Width="100%" Height="100px" Theme="Office2010Blue" ReadOnly="True">
                                                            </dx:ASPxMemo>
                                                        </dx:LayoutItemNestedControlContainer>
                                                    </LayoutItemNestedControlCollection>
                                                </dx:LayoutItem>

                                                <dx:LayoutItem Caption="Categoría Asignada">
                                                    <LayoutItemNestedControlCollection>
                                                        <dx:LayoutItemNestedControlContainer runat="server">
                                                            <dx:ASPxTextBox ID="txtCategoriaAsignada" runat="server" ClientInstanceName="txtCategoriaAsignada" 
                                                                Width="100%" Theme="Office2010Blue" ReadOnly="True">
                                                            </dx:ASPxTextBox>
                                                        </dx:LayoutItemNestedControlContainer>
                                                    </LayoutItemNestedControlCollection>
                                                </dx:LayoutItem>

                                                <dx:LayoutItem Caption="Subcategoría Asignada">
                                                    <LayoutItemNestedControlCollection>
                                                        <dx:LayoutItemNestedControlContainer runat="server">
                                                            <dx:ASPxTextBox ID="txtSubcategoriaAsignada" runat="server" ClientInstanceName="txtSubcategoriaAsignada" 
                                                                Width="100%" Theme="Office2010Blue" ReadOnly="True">
                                                            </dx:ASPxTextBox>
                                                        </dx:LayoutItemNestedControlContainer>
                                                    </LayoutItemNestedControlCollection>
                                                </dx:LayoutItem>

                                                <dx:LayoutItem Caption="Sentimiento Detectado">
                                                    <LayoutItemNestedControlCollection>
                                                        <dx:LayoutItemNestedControlContainer runat="server">
                                                            <dx:ASPxTextBox ID="txtSentimientoDetectado" runat="server" ClientInstanceName="txtSentimientoDetectado" 
                                                                Width="100%" Theme="Office2010Blue" ReadOnly="True">
                                                            </dx:ASPxTextBox>
                                                        </dx:LayoutItemNestedControlContainer>
                                                    </LayoutItemNestedControlCollection>
                                                </dx:LayoutItem>

                                                <dx:LayoutItem Caption="Prioridad Asignada">
                                                    <LayoutItemNestedControlCollection>
                                                        <dx:LayoutItemNestedControlContainer runat="server">
                                                            <dx:ASPxTextBox ID="txtPrioridadAsignada" runat="server" ClientInstanceName="txtPrioridadAsignada" 
                                                                Width="100%" Theme="Office2010Blue" ReadOnly="True">
                                                            </dx:ASPxTextBox>
                                                        </dx:LayoutItemNestedControlContainer>
                                                    </LayoutItemNestedControlCollection>
                                                </dx:LayoutItem>

                                                <dx:LayoutItem Caption="Urgencia Asignada">
                                                    <LayoutItemNestedControlCollection>
                                                        <dx:LayoutItemNestedControlContainer runat="server">
                                                            <dx:ASPxTextBox ID="txtUrgenciaAsignada" runat="server" ClientInstanceName="txtUrgenciaAsignada" 
                                                                Width="100%" Theme="Office2010Blue" ReadOnly="True">
                                                            </dx:ASPxTextBox>
                                                        </dx:LayoutItemNestedControlContainer>
                                                    </LayoutItemNestedControlCollection>
                                                </dx:LayoutItem>

                                                <dx:LayoutItem Caption="Puede Resolver IA">
                                                    <LayoutItemNestedControlCollection>
                                                        <dx:LayoutItemNestedControlContainer runat="server">
                                                            <dx:ASPxCheckBox ID="chkPuedeResolverIA" runat="server" ClientInstanceName="chkPuedeResolverIA" 
                                                                Theme="Office2010Blue" ReadOnly="True">
                                                            </dx:ASPxCheckBox>
                                                        </dx:LayoutItemNestedControlContainer>
                                                    </LayoutItemNestedControlCollection>
                                                </dx:LayoutItem>

                                                <dx:LayoutItem Caption="Respuesta IA" ColSpan="2">
                                                    <LayoutItemNestedControlCollection>
                                                        <dx:LayoutItemNestedControlContainer runat="server">
                                                            <dx:ASPxMemo ID="txtRespuestaIA" runat="server" ClientInstanceName="txtRespuestaIA" 
                                                                Width="100%" Height="150px" Theme="Office2010Blue" ReadOnly="True">
                                                            </dx:ASPxMemo>
                                                        </dx:LayoutItemNestedControlContainer>
                                                    </LayoutItemNestedControlCollection>
                                                </dx:LayoutItem>
                                            </Items>
                                        </dx:ASPxFormLayout>
                                    </dx:ContentControl>
                                </ContentCollection>
                            </dx:TabPage>

                            <dx:TabPage Text="Resolución" Name="tabResolucion">
                                <ContentCollection>
                                    <dx:ContentControl runat="server">
                                        <dx:ASPxFormLayout ID="formResolucion" runat="server" Width="100%" ColCount="2" Theme="Office2010Blue" AlignItemCaptionsInAllGroups="True">
                                            <Items>
                                                <dx:LayoutItem Caption="Fecha Resolución">
                                                    <LayoutItemNestedControlCollection>
                                                        <dx:LayoutItemNestedControlContainer runat="server">
                                                            <dx:ASPxDateEdit ID="dtFechaResolucion" runat="server" ClientInstanceName="dtFechaResolucion" 
                                                                Width="100%" Theme="Office2010Blue">
                                                            </dx:ASPxDateEdit>
                                                        </dx:LayoutItemNestedControlContainer>
                                                    </LayoutItemNestedControlCollection>
                                                </dx:LayoutItem>

                                                <dx:LayoutItem Caption="Tiempo Resolución (minutos)">
                                                    <LayoutItemNestedControlCollection>
                                                        <dx:LayoutItemNestedControlContainer runat="server">
                                                            <dx:ASPxSpinEdit ID="txtTiempoResolucion" runat="server" ClientInstanceName="txtTiempoResolucion" 
                                                                Width="100%" Theme="Office2010Blue" ReadOnly="True" MinValue="0">
                                                            </dx:ASPxSpinEdit>
                                                        </dx:LayoutItemNestedControlContainer>
                                                    </LayoutItemNestedControlCollection>
                                                </dx:LayoutItem>

                                                <dx:LayoutItem Caption="Satisfacción Cliente">
                                                    <LayoutItemNestedControlCollection>
                                                        <dx:LayoutItemNestedControlContainer runat="server">
                                                            <dx:ASPxSpinEdit ID="txtSatisfaccionCliente" runat="server" ClientInstanceName="txtSatisfaccionCliente" 
                                                                Width="100%" Theme="Office2010Blue" MinValue="1" MaxValue="5">
                                                            </dx:ASPxSpinEdit>
                                                        </dx:LayoutItemNestedControlContainer>
                                                    </LayoutItemNestedControlCollection>
                                                </dx:LayoutItem>

                                                <dx:LayoutItem Caption="Comentario Satisfacción" ColSpan="2">
                                                    <LayoutItemNestedControlCollection>
                                                        <dx:LayoutItemNestedControlContainer runat="server">
                                                            <dx:ASPxMemo ID="txtComentarioSatisfaccion" runat="server" ClientInstanceName="txtComentarioSatisfaccion" 
                                                                Width="100%" Height="100px" Theme="Office2010Blue">
                                                            </dx:ASPxMemo>
                                                        </dx:LayoutItemNestedControlContainer>
                                                    </LayoutItemNestedControlCollection>
                                                </dx:LayoutItem>
                                            </Items>
                                        </dx:ASPxFormLayout>
                                    </dx:ContentControl>
                                </ContentCollection>
                            </dx:TabPage>
                        </TabPages>
                    </dx:ASPxPageControl>

                    <br />
                    <div class="form-actions">
                        <dx:ASPxButton ID="btnGuardar" runat="server" Text="Guardar" Theme="Office2010Blue"
                            AutoPostBack="True" OnClick="btnGuardar_Click">
                            <Image IconID="actions_save_16x16"></Image>
                        </dx:ASPxButton>
                        <dx:ASPxButton ID="btnCancelar" runat="server" Text="Cancelar" Theme="Office2010Blue"
                            AutoPostBack="False">
                            <ClientSideEvents Click="function(s, e) { popupTicket.Hide(); }" />
                            <Image IconID="actions_cancel_16x16"></Image>
                        </dx:ASPxButton>
                    </div>
                    <dx:ASPxLabel ID="lblMensaje" runat="server" ClientInstanceName="lblMensaje"
                        CssClass="error-message" Visible="False">
                    </dx:ASPxLabel>
                </dx:PopupControlContentControl>
            </ContentCollection>
        </dx:ASPxPopupControl>

        <dx:ASPxHiddenField ID="hfIdTicket" runat="server" ClientInstanceName="hfIdTicket" />
    </div>
</asp:Content>
