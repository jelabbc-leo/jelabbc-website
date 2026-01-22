<%@ Page Title="Pagos" Language="vb" MasterPageFile="~/MasterPages/Jela.Master" AutoEventWireup="false" CodeBehind="Pagos.aspx.vb" Inherits="JelaWeb.Pagos" %>

<%@ Register Assembly="DevExpress.Web.v22.2, Version=22.2.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web" TagPrefix="dx" %>

<asp:Content ID="ContentHead" ContentPlaceHolderID="head" runat="server">
    <link href="<%= ResolveUrl("~/Content/CSS/pagos.css") %>" rel="stylesheet" type="text/css" />
    <script src="<%= ResolveUrl("~/Scripts/app/Operacion/pagos.js") %>" type="text/javascript"></script>
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
        <dx:ASPxGridView ID="gridPagos" runat="server" ClientInstanceName="gridPagos"
            KeyFieldName="Id" Width="100%" Theme="Office2010Blue" AutoGenerateColumns="False"
            OnDataBound="gridPagos_DataBound" OnCustomCallback="gridPagos_CustomCallback"
            EnableCallBacks="True">
            
            <ClientSideEvents ToolbarItemClick="onToolbarPagosClick" RowDblClick="onRowDblClick" />
            
            <SettingsPager Mode="ShowAllRecords" />
            <Settings ShowFilterRow="False" ShowFilterRowMenu="True" ShowGroupPanel="True" />
            <SettingsSearchPanel Visible="True" ShowApplyButton="True" ShowClearButton="True" />
            <SettingsBehavior AllowSort="True" AllowGroup="True" AllowFocusedRow="True" AllowSelectByRowClick="True" />
            <SettingsExport EnableClientSideExportAPI="True" ExcelExportMode="WYSIWYG" FileName="Pagos" />
            
            <Toolbars>
                <dx:GridViewToolbar>
                    <SettingsAdaptivity Enabled="True" EnableCollapseRootItemsToIcons="True" />
                    <Items>
                        <dx:GridViewToolbarItem Name="btnNuevoPago" Text="Nuevo Pago">
                            <Image IconID="actions_add_16x16" />
                        </dx:GridViewToolbarItem>
                        <dx:GridViewToolbarItem Name="btnEditarPago" Text="Editar Pago">
                            <Image IconID="edit_edit_16x16" />
                        </dx:GridViewToolbarItem>
                        <dx:GridViewToolbarItem Name="btnAplicarPago" Text="Aplicar a Cuotas">
                            <Image IconID="check_mark_16x16" />
                        </dx:GridViewToolbarItem>
                        <dx:GridViewToolbarItem Name="btnEliminarPago" Text="Eliminar Pago" BeginGroup="True">
                            <Image IconID="actions_trash_16x16" />
                        </dx:GridViewToolbarItem>
                        <dx:GridViewToolbarItem BeginGroup="True" Command="Refresh" />
                        <dx:GridViewToolbarItem Command="ExportToXlsx" Text="Exportar Excel" />
                        <dx:GridViewToolbarItem Command="ExportToPdf" Text="Exportar PDF" />
                    </Items>
                </dx:GridViewToolbar>
            </Toolbars>
            
            <%-- Columnas generadas dinámicamente desde el API - Solo GridViewCommandColumn - las demás columnas se generan dinámicamente --%>
            <Columns>
                <dx:GridViewCommandColumn ShowSelectCheckbox="False" VisibleIndex="0" Width="60px" />
            </Columns>
            
        </dx:ASPxGridView>
        <dx:ASPxGridViewExporter ID="gridExporter" runat="server" GridViewID="gridPagos" />

    </div>

    <!-- POPUP: NUEVO/EDITAR PAGO -->
    <dx:ASPxPopupControl ID="popupPago" runat="server" ClientInstanceName="popupPago"
        HeaderText="Nuevo Pago" Width="900px" Modal="True" 
        PopupHorizontalAlign="WindowCenter" PopupVerticalAlign="WindowCenter"
        CloseAction="CloseButton" AllowDragging="True" CloseOnEscape="True">
        <ContentCollection>
            <dx:PopupControlContentControl runat="server">
                <dx:ASPxPageControl ID="tabsPago" runat="server" ClientInstanceName="tabsPago" 
                    Width="100%" Theme="Office2010Blue" ActiveTabIndex="0">
                    <%-- TAB: DATOS DEL PAGO --%>
                    <TabPages>
                        <dx:TabPage Text="Datos del Pago" Name="tabDatosPago">
                            <ContentCollection>
                                <dx:ContentControl runat="server">
                                    <dx:ASPxFormLayout ID="formPago" runat="server" Width="100%" ColCount="2" Theme="Office2010Blue">
                                        <Items>
                                            <!-- Campo Entidad eliminado - El sistema usa IdEntidadActual automáticamente -->
                                            <dx:LayoutItem Caption="Unidad" RequiredMarkDisplayMode="Required">
                                                <LayoutItemNestedControlCollection>
                                                    <dx:LayoutItemNestedControlContainer runat="server">
                                                        <dx:ASPxComboBox ID="cboPagoUnidad" runat="server" ClientInstanceName="cboPagoUnidad"
                                                            Width="100%" Theme="Office2010Blue" ValueType="System.Int32">
                                                            <ClientSideEvents SelectedIndexChanged="onPagoUnidadChanged" />
                                                            <ValidationSettings><RequiredField IsRequired="True" ErrorText="Seleccione una unidad" /></ValidationSettings>
                                                        </dx:ASPxComboBox>
                                                    </dx:LayoutItemNestedControlContainer>
                                                </LayoutItemNestedControlCollection>
                                            </dx:LayoutItem>
                                            <dx:LayoutItem Caption="Residente">
                                                <LayoutItemNestedControlCollection>
                                                    <dx:LayoutItemNestedControlContainer runat="server">
                                                        <dx:ASPxComboBox ID="cboPagoResidente" runat="server" ClientInstanceName="cboPagoResidente"
                                                            Width="100%" Theme="Office2010Blue" ValueType="System.Int32" />
                                                    </dx:LayoutItemNestedControlContainer>
                                                </LayoutItemNestedControlCollection>
                                            </dx:LayoutItem>
                                            <dx:LayoutItem Caption="Fecha Pago" RequiredMarkDisplayMode="Required">
                                                <LayoutItemNestedControlCollection>
                                                    <dx:LayoutItemNestedControlContainer runat="server">
                                                        <dx:ASPxDateEdit ID="dtFechaPago" runat="server" ClientInstanceName="dtFechaPago"
                                                            Width="100%" Theme="Office2010Blue" DisplayFormatString="dd/MM/yyyy">
                                                            <ValidationSettings><RequiredField IsRequired="True" ErrorText="Seleccione la fecha de pago" /></ValidationSettings>
                                                        </dx:ASPxDateEdit>
                                                    </dx:LayoutItemNestedControlContainer>
                                                </LayoutItemNestedControlCollection>
                                            </dx:LayoutItem>
                                            <dx:LayoutItem Caption="Monto" RequiredMarkDisplayMode="Required">
                                                <LayoutItemNestedControlCollection>
                                                    <dx:LayoutItemNestedControlContainer runat="server">
                                                        <dx:ASPxSpinEdit ID="txtPagoMonto" runat="server" ClientInstanceName="txtPagoMonto"
                                                            Width="100%" Theme="Office2010Blue" NumberType="Float" DecimalPlaces="2"
                                                            MinValue="0.01" DisplayFormatString="C2">
                                                            <ValidationSettings><RequiredField IsRequired="True" ErrorText="Ingrese el monto" /></ValidationSettings>
                                                            <ClientSideEvents ValueChanged="onPagoMontoChanged" />
                                                        </dx:ASPxSpinEdit>
                                                    </dx:LayoutItemNestedControlContainer>
                                                </LayoutItemNestedControlCollection>
                                            </dx:LayoutItem>
                                            <dx:LayoutItem Caption="Forma de Pago" RequiredMarkDisplayMode="Required">
                                                <LayoutItemNestedControlCollection>
                                                    <dx:LayoutItemNestedControlContainer runat="server">
                                                        <dx:ASPxComboBox ID="cboFormaPago" runat="server" ClientInstanceName="cboFormaPago"
                                                            Width="100%" Theme="Office2010Blue" ValueType="System.String">
                                                            <Items>
                                                                <dx:ListEditItem Text="Efectivo" Value="Efectivo" Selected="True" />
                                                                <dx:ListEditItem Text="Transferencia" Value="Transferencia" />
                                                                <dx:ListEditItem Text="Tarjeta" Value="Tarjeta" />
                                                                <dx:ListEditItem Text="Cheque" Value="Cheque" />
                                                                <dx:ListEditItem Text="Depósito" Value="Deposito" />
                                                                <dx:ListEditItem Text="SPEI" Value="SPEI" />
                                                            </Items>
                                                            <ValidationSettings><RequiredField IsRequired="True" ErrorText="Seleccione la forma de pago" /></ValidationSettings>
                                                        </dx:ASPxComboBox>
                                                    </dx:LayoutItemNestedControlContainer>
                                                </LayoutItemNestedControlCollection>
                                            </dx:LayoutItem>
                                            <dx:LayoutItem Caption="Referencia">
                                                <LayoutItemNestedControlCollection>
                                                    <dx:LayoutItemNestedControlContainer runat="server">
                                                        <dx:ASPxTextBox ID="txtReferencia" runat="server" ClientInstanceName="txtReferencia"
                                                            Width="100%" Theme="Office2010Blue" MaxLength="100" NullText="Número de referencia bancaria" />
                                                    </dx:LayoutItemNestedControlContainer>
                                                </LayoutItemNestedControlCollection>
                                            </dx:LayoutItem>
                                            <dx:LayoutItem Caption="Banco">
                                                <LayoutItemNestedControlCollection>
                                                    <dx:LayoutItemNestedControlContainer runat="server">
                                                        <dx:ASPxTextBox ID="txtBanco" runat="server" ClientInstanceName="txtBanco"
                                                            Width="100%" Theme="Office2010Blue" MaxLength="100" />
                                                    </dx:LayoutItemNestedControlContainer>
                                                </LayoutItemNestedControlCollection>
                                            </dx:LayoutItem>
                                            <dx:LayoutItem Caption="Estado">
                                                <LayoutItemNestedControlCollection>
                                                    <dx:LayoutItemNestedControlContainer runat="server">
                                                        <dx:ASPxComboBox ID="cboPagoEstado" runat="server" ClientInstanceName="cboPagoEstado"
                                                            Width="100%" Theme="Office2010Blue" ValueType="System.String">
                                                            <Items>
                                                                <dx:ListEditItem Text="Aplicado" Value="Aplicado" Selected="True" />
                                                                <dx:ListEditItem Text="Pendiente" Value="Pendiente" />
                                                                <dx:ListEditItem Text="Rechazado" Value="Rechazado" />
                                                                <dx:ListEditItem Text="Cancelado" Value="Cancelado" />
                                                            </Items>
                                                        </dx:ASPxComboBox>
                                                    </dx:LayoutItemNestedControlContainer>
                                                </LayoutItemNestedControlCollection>
                                            </dx:LayoutItem>
                                            <dx:LayoutItem Caption="Observaciones" ColSpan="2">
                                                <LayoutItemNestedControlCollection>
                                                    <dx:LayoutItemNestedControlContainer runat="server">
                                                        <dx:ASPxMemo ID="txtPagoObservaciones" runat="server" ClientInstanceName="txtPagoObservaciones"
                                                            Width="100%" Height="80px" Theme="Office2010Blue" />
                                                    </dx:LayoutItemNestedControlContainer>
                                                </LayoutItemNestedControlCollection>
                                            </dx:LayoutItem>
                                        </Items>
                                    </dx:ASPxFormLayout>
                                </dx:ContentControl>
                            </ContentCollection>
                        </dx:TabPage>
                        <%-- TAB: APLICACIÓN A CUOTAS --%>
                        <dx:TabPage Text="Aplicar a Cuotas" Name="tabAplicarCuotas">
                            <ContentCollection>
                                <dx:ContentControl runat="server">
                                    <div style="padding: 10px;">
                                        <p><strong>Monto del pago:</strong> <span id="lblMontoPago" style="font-size: 18px; font-weight: bold; color: #007bff;">$0.00</span></p>
                                        <p><strong>Monto aplicado:</strong> <span id="lblMontoAplicado" style="font-size: 18px; font-weight: bold; color: #28a745;">$0.00</span></p>
                                        <p><strong>Saldo pendiente:</strong> <span id="lblSaldoPendiente" style="font-size: 18px; font-weight: bold; color: #dc3545;">$0.00</span></p>
                                        <hr />
                                    </div>
                                    <dx:ASPxGridView ID="gridCuotasPendientes" runat="server" ClientInstanceName="gridCuotasPendientes"
                                        Width="100%" KeyFieldName="Id" Theme="Office2010Blue" AutoGenerateColumns="False"
                                        OnCustomCallback="gridCuotasPendientes_CustomCallback"
                                        EnableCallBacks="True">
                                        <ClientSideEvents SelectionChanged="onCuotasPendientesSelectionChanged" />
                                        <SettingsPager Mode="ShowAllRecords" />
                                        <Settings ShowFilterRow="False" ShowFilterRowMenu="True" ShowGroupPanel="False" />
                                        <SettingsBehavior AllowSort="True" AllowGroup="False" AllowFocusedRow="True" AllowSelectByRowClick="True" />
                                        <SettingsSelection Mode="Multiple" />
                                        <Columns>
                                            <dx:GridViewCommandColumn ShowSelectCheckbox="True" VisibleIndex="0" Width="50px" />
                                            <dx:GridViewDataTextColumn FieldName="Folio" Caption="Folio" Width="120px" VisibleIndex="1" />
                                            <dx:GridViewDataTextColumn FieldName="ConceptoNombre" Caption="Concepto" Width="200px" VisibleIndex="2" />
                                            <dx:GridViewDataTextColumn FieldName="Periodo" Caption="Período" Width="100px" VisibleIndex="3" />
                                            <dx:GridViewDataDateColumn FieldName="FechaVencimiento" Caption="Vencimiento" Width="120px" VisibleIndex="4">
                                                <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy" />
                                            </dx:GridViewDataDateColumn>
                                            <dx:GridViewDataTextColumn FieldName="MontoTotal" Caption="Total" Width="100px" VisibleIndex="5">
                                                <PropertiesTextEdit DisplayFormatString="C2" />
                                                <CellStyle HorizontalAlign="Right" />
                                            </dx:GridViewDataTextColumn>
                                            <dx:GridViewDataTextColumn FieldName="MontoPagado" Caption="Pagado" Width="100px" VisibleIndex="6">
                                                <PropertiesTextEdit DisplayFormatString="C2" />
                                                <CellStyle HorizontalAlign="Right" />
                                            </dx:GridViewDataTextColumn>
                                            <dx:GridViewDataTextColumn FieldName="Saldo" Caption="Saldo" Width="100px" VisibleIndex="7">
                                                <PropertiesTextEdit DisplayFormatString="C2" />
                                                <CellStyle HorizontalAlign="Right" />
                                                <DataItemTemplate>
                                                    <span style="font-weight: bold; color: <%# If(Convert.ToDecimal(Eval("Saldo")) > 0, "#dc3545", "#28a745") %>;">
                                                        <%# String.Format("{0:C2}", Eval("Saldo")) %>
                                                    </span>
                                                </DataItemTemplate>
                                            </dx:GridViewDataTextColumn>
                                        </Columns>
                                    </dx:ASPxGridView>
                                    <div style="margin-top: 15px; text-align: right;">
                                        <dx:ASPxButton ID="btnAplicarSeleccionadas" runat="server" Text="Aplicar a Cuotas Seleccionadas" Theme="Office2010Blue" AutoPostBack="False">
                                            <Image IconID="check_mark_16x16" />
                                            <ClientSideEvents Click="function(s,e){ aplicarPagoACuotas(); }" />
                                        </dx:ASPxButton>
                                    </div>
                                </dx:ContentControl>
                            </ContentCollection>
                        </dx:TabPage>
                        
                    </TabPages>
                </dx:ASPxPageControl>
                <div style="margin-top:15px; text-align:right;">
                    <dx:ASPxButton ID="btnGuardarPago" runat="server" Text="Guardar Pago" Theme="Office2010Blue" AutoPostBack="False">
                        <Image IconID="save_save_16x16" />
                        <ClientSideEvents Click="function(s,e){ guardarPago(); }" />
                    </dx:ASPxButton>
                    <dx:ASPxButton ID="btnCerrarPago" runat="server" Text="Cancelar" Theme="Office2010Blue" AutoPostBack="False">
                        <ClientSideEvents Click="function(s,e){ popupPago.Hide(); }" />
                    </dx:ASPxButton>
                </div>
            </dx:PopupControlContentControl>
        </ContentCollection>
    </dx:ASPxPopupControl>

    <asp:HiddenField ID="hfPagoId" runat="server" ClientIDMode="Static" Value="0" />

</asp:Content>
