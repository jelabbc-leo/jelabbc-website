<%@ Page Title="Visitantes" Language="vb" MasterPageFile="~/MasterPages/Jela.Master" AutoEventWireup="false" CodeBehind="Visitantes.aspx.vb" Inherits="JelaWeb.Visitantes" %>

<%@ Register Assembly="DevExpress.Web.v22.2, Version=22.2.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web" TagPrefix="dx" %>

<asp:Content ID="ContentHead" ContentPlaceHolderID="head" runat="server">
    <link href="<%= ResolveUrl("~/Content/CSS/visitantes.css") %>" rel="stylesheet" type="text/css" />
    <script src="<%= ResolveUrl("~/Scripts/app/Operacion/visitantes.js") %>?v=20260121" type="text/javascript"></script>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <div class="container-fluid">
        
        <!-- FILTROS: Solo fechas (según Regla 7) -->
        <dx:ASPxFormLayout ID="formFiltros" runat="server" ColCount="4" Width="100%" Theme="Office2010Blue" BackColor="#E4EFFA">
            <Items>
                <dx:LayoutItem Caption="Fecha Desde" ColSpan="2">
                    <LayoutItemNestedControlCollection>
                        <dx:LayoutItemNestedControlContainer runat="server">
                            <dx:ASPxDateEdit ID="dtFechaDesde" runat="server" ClientInstanceName="dtFechaDesde"
                                Width="100%" Theme="Office2010Blue" DisplayFormatString="dd/MM/yyyy">
                            </dx:ASPxDateEdit>
                        </dx:LayoutItemNestedControlContainer>
                    </LayoutItemNestedControlCollection>
                </dx:LayoutItem>
                <dx:LayoutItem Caption="Fecha Hasta" ColSpan="2">
                    <LayoutItemNestedControlCollection>
                        <dx:LayoutItemNestedControlContainer runat="server">
                            <dx:ASPxDateEdit ID="dtFechaHasta" runat="server" ClientInstanceName="dtFechaHasta"
                                Width="100%" Theme="Office2010Blue" DisplayFormatString="dd/MM/yyyy">
                            </dx:ASPxDateEdit>
                        </dx:LayoutItemNestedControlContainer>
                    </LayoutItemNestedControlCollection>
                </dx:LayoutItem>
                <dx:LayoutItem ShowCaption="False" ColSpan="4">
                    <LayoutItemNestedControlCollection>
                        <dx:LayoutItemNestedControlContainer runat="server">
                            <dx:ASPxButton ID="btnFiltrar" runat="server" Text="Filtrar" Theme="Office2010Blue" 
                                AutoPostBack="True" OnClick="btnFiltrar_Click">
                                <Image IconID="zoom_zoom_16x16" />
                            </dx:ASPxButton>
                            <dx:ASPxButton ID="btnLimpiar" runat="server" Text="Limpiar" Theme="Office2010Blue" 
                                AutoPostBack="True" OnClick="btnLimpiar_Click">
                                <Image IconID="actions_cancel_16x16" />
                            </dx:ASPxButton>
                        </dx:LayoutItemNestedControlContainer>
                    </LayoutItemNestedControlCollection>
                </dx:LayoutItem>
            </Items>
        </dx:ASPxFormLayout>

        <br />

        <!-- GRID PRINCIPAL -->
        <dx:ASPxGridView ID="gridVisitantes" runat="server" ClientInstanceName="gridVisitantes"
            KeyFieldName="Id" Width="100%" Theme="Office2010Blue" AutoGenerateColumns="False"
            OnDataBound="gridVisitantes_DataBound" OnCustomCallback="gridVisitantes_CustomCallback"
            EnableCallBacks="True">
            
            <ClientSideEvents ToolbarItemClick="onToolbarVisitantesClick" RowDblClick="onRowDblClick" />
            
            <SettingsPager Mode="ShowAllRecords" />
            <Settings ShowFilterRow="False" ShowFilterRowMenu="True" ShowGroupPanel="True" />
            <SettingsSearchPanel Visible="True" ShowApplyButton="True" ShowClearButton="True" />
            <SettingsBehavior AllowSort="True" AllowGroup="True" AllowFocusedRow="True" AllowSelectByRowClick="True" />
            <SettingsExport EnableClientSideExportAPI="True" ExcelExportMode="WYSIWYG" FileName="Visitantes" />
            
            <Toolbars>
                <dx:GridViewToolbar>
                    <SettingsAdaptivity Enabled="True" EnableCollapseRootItemsToIcons="True" />
                    <Items>
                        <dx:GridViewToolbarItem Name="btnNuevoVisitante" Text="Registrar Visitante">
                            <Image IconID="actions_add_16x16" />
                        </dx:GridViewToolbarItem>
                        <dx:GridViewToolbarItem Name="btnEditarVisitante" Text="Editar Visitante">
                            <Image IconID="edit_edit_16x16" />
                        </dx:GridViewToolbarItem>
                        <dx:GridViewToolbarItem Name="btnRegistrarSalida" Text="Registrar Salida">
                            <Image IconID="actions_logoff_16x16" />
                        </dx:GridViewToolbarItem>
                        <dx:GridViewToolbarItem Name="btnEliminarVisitante" Text="Eliminar Visitante" BeginGroup="True">
                            <Image IconID="actions_trash_16x16" />
                        </dx:GridViewToolbarItem>
                        <dx:GridViewToolbarItem BeginGroup="True" Command="Refresh" />
                        <dx:GridViewToolbarItem Command="ExportToXlsx" Text="Exportar Excel" />
                    </Items>
                </dx:GridViewToolbar>
            </Toolbars>
            
            <%-- Columnas generadas dinámicamente desde el API --%>
            <Columns>
                <dx:GridViewCommandColumn ShowSelectCheckbox="False" VisibleIndex="0" Width="60px" />
            </Columns>
            
        </dx:ASPxGridView>
        <dx:ASPxGridViewExporter ID="gridExporter" runat="server" GridViewID="gridVisitantes" />
    </div>

    <!-- POPUP: VISITANTE -->
    <dx:ASPxPopupControl ID="popupVisitante" runat="server" ClientInstanceName="popupVisitante"
        HeaderText="Registrar Visitante" Width="900px" Modal="True" 
        PopupHorizontalAlign="WindowCenter" PopupVerticalAlign="WindowCenter"
        CloseAction="CloseButton" AllowDragging="True" CloseOnEscape="True">
        <ContentCollection>
            <dx:PopupControlContentControl runat="server">
                <dx:ASPxFormLayout ID="formVisitante" runat="server" Width="100%" ColCount="2" Theme="Office2010Blue">
                    <Items>
                        <dx:LayoutItem Caption="Entidad" RequiredMarkDisplayMode="Required">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxComboBox ID="cboEntidad" runat="server" ClientInstanceName="cboEntidad"
                                        Width="100%" Theme="Office2010Blue" ValueType="System.Int32">
                                        <ClientSideEvents SelectedIndexChanged="onEntidadChanged" />
                                        <ValidationSettings><RequiredField IsRequired="True" ErrorText="Requerido" /></ValidationSettings>
                                    </dx:ASPxComboBox>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Unidad" RequiredMarkDisplayMode="Required">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxComboBox ID="cboUnidad" runat="server" ClientInstanceName="cboUnidad"
                                        Width="100%" Theme="Office2010Blue" ValueType="System.Int32">
                                        <ClientSideEvents SelectedIndexChanged="onUnidadChanged" />
                                        <ValidationSettings><RequiredField IsRequired="True" ErrorText="Requerido" /></ValidationSettings>
                                    </dx:ASPxComboBox>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Nombre Visitante" RequiredMarkDisplayMode="Required" ColSpan="2">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxTextBox ID="txtNombreVisitante" runat="server" ClientInstanceName="txtNombreVisitante"
                                        Width="100%" Theme="Office2010Blue" MaxLength="200">
                                        <ValidationSettings><RequiredField IsRequired="True" ErrorText="Requerido" /></ValidationSettings>
                                    </dx:ASPxTextBox>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Tipo Identificación">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxComboBox ID="cboTipoIdentificacion" runat="server" ClientInstanceName="cboTipoIdentificacion"
                                        Width="100%" Theme="Office2010Blue" ValueType="System.String">
                                        <Items>
                                            <dx:ListEditItem Text="INE" Value="INE" Selected="True" />
                                            <dx:ListEditItem Text="Pasaporte" Value="Pasaporte" />
                                            <dx:ListEditItem Text="Licencia" Value="Licencia" />
                                            <dx:ListEditItem Text="Otro" Value="Otro" />
                                        </Items>
                                    </dx:ASPxComboBox>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Número Identificación">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxTextBox ID="txtNumeroIdentificacion" runat="server" ClientInstanceName="txtNumeroIdentificacion"
                                        Width="100%" Theme="Office2010Blue" MaxLength="50" />
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Tipo Visita">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxComboBox ID="cboTipoVisita" runat="server" ClientInstanceName="cboTipoVisita"
                                        Width="100%" Theme="Office2010Blue" ValueType="System.String">
                                        <Items>
                                            <dx:ListEditItem Text="Personal" Value="Personal" Selected="True" />
                                            <dx:ListEditItem Text="Proveedor" Value="Proveedor" />
                                            <dx:ListEditItem Text="Delivery" Value="Delivery" />
                                            <dx:ListEditItem Text="Emergencia" Value="Emergencia" />
                                            <dx:ListEditItem Text="Trabajador" Value="Trabajador" />
                                            <dx:ListEditItem Text="Otro" Value="Otro" />
                                        </Items>
                                    </dx:ASPxComboBox>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Motivo Visita" ColSpan="2">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxTextBox ID="txtMotivoVisita" runat="server" ClientInstanceName="txtMotivoVisita"
                                        Width="100%" Theme="Office2010Blue" MaxLength="200" />
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutGroup Caption="Vehículo (si aplica)" ColSpan="2" ColCount="3">
                            <Items>
                                <dx:LayoutItem Caption="Placas">
                                    <LayoutItemNestedControlCollection>
                                        <dx:LayoutItemNestedControlContainer runat="server">
                                            <dx:ASPxTextBox ID="txtVehiculo" runat="server" ClientInstanceName="txtVehiculo"
                                                Width="100%" Theme="Office2010Blue" MaxLength="50" />
                                        </dx:LayoutItemNestedControlContainer>
                                    </LayoutItemNestedControlCollection>
                                </dx:LayoutItem>
                                <dx:LayoutItem Caption="Color">
                                    <LayoutItemNestedControlCollection>
                                        <dx:LayoutItemNestedControlContainer runat="server">
                                            <dx:ASPxTextBox ID="txtColorVehiculo" runat="server" ClientInstanceName="txtColorVehiculo"
                                                Width="100%" Theme="Office2010Blue" MaxLength="30" />
                                        </dx:LayoutItemNestedControlContainer>
                                    </LayoutItemNestedControlCollection>
                                </dx:LayoutItem>
                                <dx:LayoutItem Caption="Marca">
                                    <LayoutItemNestedControlCollection>
                                        <dx:LayoutItemNestedControlContainer runat="server">
                                            <dx:ASPxTextBox ID="txtMarcaVehiculo" runat="server" ClientInstanceName="txtMarcaVehiculo"
                                                Width="100%" Theme="Office2010Blue" MaxLength="50" />
                                        </dx:LayoutItemNestedControlContainer>
                                    </LayoutItemNestedControlCollection>
                                </dx:LayoutItem>
                            </Items>
                        </dx:LayoutGroup>
                        <dx:LayoutItem Caption="Fecha/Hora Entrada" RequiredMarkDisplayMode="Required">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxDateEdit ID="dteFechaEntrada" runat="server" ClientInstanceName="dteFechaEntrada"
                                        Width="100%" Theme="Office2010Blue" DisplayFormatString="dd/MM/yyyy HH:mm"
                                        EditFormat="DateTime" UseMaskBehavior="True">
                                        <TimeSectionProperties>
                                            <TimeEditProperties>
                                                <ValidationSettings><RequiredField IsRequired="True" ErrorText="Requerido" /></ValidationSettings>
                                            </TimeEditProperties>
                                        </TimeSectionProperties>
                                        <ValidationSettings><RequiredField IsRequired="True" ErrorText="Requerido" /></ValidationSettings>
                                    </dx:ASPxDateEdit>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Estado">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxComboBox ID="cboEstado" runat="server" ClientInstanceName="cboEstado"
                                        Width="100%" Theme="Office2010Blue" ValueType="System.String">
                                        <Items>
                                            <dx:ListEditItem Text="EnCondominio" Value="EnCondominio" Selected="True" />
                                            <dx:ListEditItem Text="Salida" Value="Salida" />
                                            <dx:ListEditItem Text="Rechazado" Value="Rechazado" />
                                        </Items>
                                    </dx:ASPxComboBox>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Residente (Autoriza)" ColSpan="2">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxComboBox ID="cboResidente" runat="server" ClientInstanceName="cboResidente"
                                        Width="100%" Theme="Office2010Blue" ValueType="System.Int32" />
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Observaciones" ColSpan="2">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxMemo ID="txtObservaciones" runat="server" ClientInstanceName="txtObservaciones"
                                        Width="100%" Height="80px" Theme="Office2010Blue" />
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                    </Items>
                </dx:ASPxFormLayout>
                <div style="margin-top:15px; text-align:right;">
                    <dx:ASPxButton ID="btnGuardar" runat="server" Text="Guardar" Theme="Office2010Blue" AutoPostBack="False">
                        <Image IconID="save_save_16x16" />
                        <ClientSideEvents Click="function(s,e){ guardarVisitante(); }" />
                    </dx:ASPxButton>
                    <dx:ASPxButton ID="btnCerrar" runat="server" Text="Cancelar" Theme="Office2010Blue" AutoPostBack="False">
                        <ClientSideEvents Click="function(s,e){ popupVisitante.Hide(); }" />
                    </dx:ASPxButton>
                </div>
            </dx:PopupControlContentControl>
        </ContentCollection>
    </dx:ASPxPopupControl>

    <asp:HiddenField ID="hfId" runat="server" Value="0" ClientIDMode="Static" />

</asp:Content>
