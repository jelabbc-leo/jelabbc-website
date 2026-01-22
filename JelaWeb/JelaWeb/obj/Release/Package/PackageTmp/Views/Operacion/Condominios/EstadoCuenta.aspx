<%@ Page Title="Estado de Cuenta" Language="vb" MasterPageFile="~/MasterPages/Jela.Master" AutoEventWireup="false" CodeBehind="EstadoCuenta.aspx.vb" Inherits="JelaWeb.EstadoCuenta" %>

<%@ Register Assembly="DevExpress.Web.v22.2, Version=22.2.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web" TagPrefix="dx" %>

<asp:Content ID="ContentHead" ContentPlaceHolderID="head" runat="server">
    <link href="<%= ResolveUrl("~/Content/CSS/estado-cuenta.css") %>" rel="stylesheet" type="text/css" />
    <script src="<%= ResolveUrl("~/Scripts/app/Operacion/estado-cuenta.js") %>" type="text/javascript"></script>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <div class="container-fluid">
        
        <!-- FILTROS: Solo fechas y unidad (según Regla 7) -->
        <dx:ASPxFormLayout ID="formFiltros" runat="server" ColCount="4" Width="100%" Theme="Office2010Blue" BackColor="#E4EFFA">
            <Items>
                <!-- Campo Entidad eliminado - El sistema usa IdEntidadActual automáticamente -->
                <dx:LayoutItem Caption="Unidad" ColSpan="2">
                    <LayoutItemNestedControlCollection>
                        <dx:LayoutItemNestedControlContainer runat="server">
                            <dx:ASPxComboBox ID="cboFiltroUnidad" runat="server" ClientInstanceName="cboFiltroUnidad"
                                Width="100%" Theme="Office2010Blue" ValueType="System.Int32">
                                <ClientSideEvents SelectedIndexChanged="onFiltroUnidadChanged" />
                            </dx:ASPxComboBox>
                        </dx:LayoutItemNestedControlContainer>
                    </LayoutItemNestedControlCollection>
                </dx:LayoutItem>
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
                            <dx:ASPxButton ID="btnConsultar" runat="server" Text="Consultar" Theme="Office2010Blue" 
                                AutoPostBack="True" OnClick="btnConsultar_Click">
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

        <!-- RESUMEN -->
        <dx:ASPxFormLayout ID="formResumen" runat="server" ColCount="6" Width="100%" Theme="Office2010Blue" BackColor="#F8F9FA">
            <Items>
                <dx:LayoutItem Caption="Saldo Anterior" ColSpan="1">
                    <LayoutItemNestedControlCollection>
                        <dx:LayoutItemNestedControlContainer runat="server">
                            <dx:ASPxLabel ID="lblSaldoAnterior" runat="server" Text="$0.00" Font-Size="16px" Font-Bold="True" ForeColor="#6c757d" />
                        </dx:LayoutItemNestedControlContainer>
                    </LayoutItemNestedControlCollection>
                </dx:LayoutItem>
                <dx:LayoutItem Caption="Cargos" ColSpan="1">
                    <LayoutItemNestedControlCollection>
                        <dx:LayoutItemNestedControlContainer runat="server">
                            <dx:ASPxLabel ID="lblCargos" runat="server" Text="$0.00" Font-Size="16px" Font-Bold="True" ForeColor="#dc3545" />
                        </dx:LayoutItemNestedControlContainer>
                    </LayoutItemNestedControlCollection>
                </dx:LayoutItem>
                <dx:LayoutItem Caption="Pagos" ColSpan="1">
                    <LayoutItemNestedControlCollection>
                        <dx:LayoutItemNestedControlContainer runat="server">
                            <dx:ASPxLabel ID="lblPagos" runat="server" Text="$0.00" Font-Size="16px" Font-Bold="True" ForeColor="#28a745" />
                        </dx:LayoutItemNestedControlContainer>
                    </LayoutItemNestedControlCollection>
                </dx:LayoutItem>
                <dx:LayoutItem Caption="Saldo Actual" ColSpan="1">
                    <LayoutItemNestedControlCollection>
                        <dx:LayoutItemNestedControlContainer runat="server">
                            <dx:ASPxLabel ID="lblSaldoActual" runat="server" Text="$0.00" Font-Size="16px" Font-Bold="True" ForeColor="#007bff" />
                        </dx:LayoutItemNestedControlContainer>
                    </LayoutItemNestedControlCollection>
                </dx:LayoutItem>
                <dx:LayoutItem Caption="Cuotas Pendientes" ColSpan="1">
                    <LayoutItemNestedControlCollection>
                        <dx:LayoutItemNestedControlContainer runat="server">
                            <dx:ASPxLabel ID="lblCuotasPendientes" runat="server" Text="0" Font-Size="16px" Font-Bold="True" ForeColor="#ffc107" />
                        </dx:LayoutItemNestedControlContainer>
                    </LayoutItemNestedControlCollection>
                </dx:LayoutItem>
                <dx:LayoutItem Caption="Cuotas Vencidas" ColSpan="1">
                    <LayoutItemNestedControlCollection>
                        <dx:LayoutItemNestedControlContainer runat="server">
                            <dx:ASPxLabel ID="lblCuotasVencidas" runat="server" Text="0" Font-Size="16px" Font-Bold="True" ForeColor="#dc3545" />
                        </dx:LayoutItemNestedControlContainer>
                    </LayoutItemNestedControlCollection>
                </dx:LayoutItem>
            </Items>
        </dx:ASPxFormLayout>

        <br />

        <!-- GRID PRINCIPAL -->
        <dx:ASPxGridView ID="gridEstadoCuenta" runat="server" ClientInstanceName="gridEstadoCuenta"
            KeyFieldName="Id" Width="100%" Theme="Office2010Blue" AutoGenerateColumns="False"
            OnDataBound="gridEstadoCuenta_DataBound" OnCustomCallback="gridEstadoCuenta_CustomCallback"
            EnableCallBacks="True">
            
            <ClientSideEvents ToolbarItemClick="onToolbarEstadoCuentaClick" />
            
            <SettingsPager Mode="ShowAllRecords" />
            <Settings ShowFilterRow="False" ShowFilterRowMenu="True" ShowGroupPanel="True" />
            <SettingsSearchPanel Visible="True" ShowApplyButton="True" ShowClearButton="True" />
            <SettingsBehavior AllowSort="True" AllowGroup="True" AllowFocusedRow="True" AllowSelectByRowClick="True" />
            <SettingsExport EnableClientSideExportAPI="True" ExcelExportMode="WYSIWYG" FileName="EstadoCuenta" />
            <SettingsDetail ShowDetailRow="True" AllowOnlyOneMasterRowExpanded="True" />
            
            <Toolbars>
                <dx:GridViewToolbar>
                    <SettingsAdaptivity Enabled="True" EnableCollapseRootItemsToIcons="True" />
                    <Items>
                        <dx:GridViewToolbarItem BeginGroup="True" Command="Refresh" />
                        <dx:GridViewToolbarItem Command="ExportToXlsx" Text="Exportar Excel" />
                        <dx:GridViewToolbarItem Command="ExportToPdf" Text="Exportar PDF" />
                    </Items>
                </dx:GridViewToolbar>
            </Toolbars>
            
            <%-- Columnas generadas dinámicamente desde el API --%>
            <Columns>
                <dx:GridViewCommandColumn ShowSelectCheckbox="False" VisibleIndex="0" Width="60px" ShowExpandButton="True" />
            </Columns>
            
            <Templates>
                <DetailRow>
                    <dx:ASPxGridView ID="gridDetalleMovimiento" runat="server" 
                        ClientInstanceName="gridDetalleMovimiento_<%# Container.VisibleIndex %>"
                        Width="100%" Theme="Office2010Blue" AutoGenerateColumns="False"
                        OnBeforePerformDataSelect="gridDetalleMovimiento_BeforePerformDataSelect">
                        <SettingsPager Mode="ShowAllRecords" />
                        <Settings ShowFilterRow="False" ShowFilterRowMenu="True" />
                        <Columns>
                            <dx:GridViewDataTextColumn FieldName="Tipo" Caption="Tipo" Width="100px" />
                            <dx:GridViewDataTextColumn FieldName="Folio" Caption="Folio" Width="120px" />
                            <dx:GridViewDataDateColumn FieldName="Fecha" Caption="Fecha" Width="120px">
                                <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy" />
                            </dx:GridViewDataDateColumn>
                            <dx:GridViewDataTextColumn FieldName="Concepto" Caption="Concepto" Width="200px" />
                            <dx:GridViewDataTextColumn FieldName="Cargo" Caption="Cargo" Width="120px">
                                <PropertiesTextEdit DisplayFormatString="C2" />
                                <CellStyle HorizontalAlign="Right" ForeColor="#dc3545" />
                            </dx:GridViewDataTextColumn>
                            <dx:GridViewDataTextColumn FieldName="Abono" Caption="Abono" Width="120px">
                                <PropertiesTextEdit DisplayFormatString="C2" />
                                <CellStyle HorizontalAlign="Right" ForeColor="#28a745" />
                            </dx:GridViewDataTextColumn>
                            <dx:GridViewDataTextColumn FieldName="Saldo" Caption="Saldo" Width="120px">
                                <PropertiesTextEdit DisplayFormatString="C2" />
                                <CellStyle HorizontalAlign="Right" Font-Bold="True" />
                            </dx:GridViewDataTextColumn>
                        </Columns>
                    </dx:ASPxGridView>
                </DetailRow>
            </Templates>
            
        </dx:ASPxGridView>
        <dx:ASPxGridViewExporter ID="gridExporter" runat="server" GridViewID="gridEstadoCuenta" />

    </div>

</asp:Content>
