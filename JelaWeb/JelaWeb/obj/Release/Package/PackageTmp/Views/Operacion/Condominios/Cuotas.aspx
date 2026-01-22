<%@ Page Title="Cuotas" Language="vb" MasterPageFile="~/MasterPages/Jela.Master" AutoEventWireup="false" CodeBehind="Cuotas.aspx.vb" Inherits="JelaWeb.Cuotas" %>

<%@ Register Assembly="DevExpress.Web.v22.2, Version=22.2.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web" TagPrefix="dx" %>

<asp:Content ID="ContentHead" ContentPlaceHolderID="head" runat="server">
    <link href="<%= ResolveUrl("~/Content/CSS/cuotas.css") %>" rel="stylesheet" type="text/css" />
    <script src="<%= ResolveUrl("~/Scripts/app/Operacion/cuotas.js") %>" type="text/javascript"></script>
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
        <dx:ASPxGridView ID="gridCuotas" runat="server" ClientInstanceName="gridCuotas"
            KeyFieldName="Id" Width="100%" Theme="Office2010Blue" AutoGenerateColumns="False"
            OnDataBound="gridCuotas_DataBound" OnCustomCallback="gridCuotas_CustomCallback"
            EnableCallBacks="True">
            
            <ClientSideEvents ToolbarItemClick="onToolbarCuotasClick" RowDblClick="onRowDblClick" />
            
            <SettingsPager Mode="ShowAllRecords" />
            <Settings ShowFilterRow="False" ShowFilterRowMenu="True" ShowGroupPanel="True" />
            <SettingsSearchPanel Visible="True" ShowApplyButton="True" ShowClearButton="True" />
            <SettingsBehavior AllowSort="True" AllowGroup="True" AllowFocusedRow="True" AllowSelectByRowClick="True" />
            <SettingsExport EnableClientSideExportAPI="True" ExcelExportMode="WYSIWYG" FileName="Cuotas" />
            
            <Toolbars>
                <dx:GridViewToolbar>
                    <SettingsAdaptivity Enabled="True" EnableCollapseRootItemsToIcons="True" />
                    <Items>
                        <dx:GridViewToolbarItem Name="btnNuevaCuota" Text="Nueva Cuota">
                            <Image IconID="actions_add_16x16" />
                        </dx:GridViewToolbarItem>
                        <dx:GridViewToolbarItem Name="btnEditarCuota" Text="Editar Cuota">
                            <Image IconID="edit_edit_16x16" />
                        </dx:GridViewToolbarItem>
                        <dx:GridViewToolbarItem Name="btnGenerarMasiva" Text="Generar Cuotas Masivas">
                            <Image IconID="content_copy_16x16" />
                        </dx:GridViewToolbarItem>
                        <dx:GridViewToolbarItem Name="btnAplicarRecargos" Text="Aplicar Recargos por Mora">
                            <Image IconID="money_money_16x16" />
                        </dx:GridViewToolbarItem>
                        <dx:GridViewToolbarItem Name="btnEliminarCuota" Text="Eliminar Cuota" BeginGroup="True">
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
        <dx:ASPxGridViewExporter ID="gridExporter" runat="server" GridViewID="gridCuotas" />

    </div>

    <!-- POPUP: NUEVA/EDITAR CUOTA -->
    <dx:ASPxPopupControl ID="popupCuota" runat="server" ClientInstanceName="popupCuota"
        HeaderText="Nueva Cuota" Width="800px" Modal="True" 
        PopupHorizontalAlign="WindowCenter" PopupVerticalAlign="WindowCenter"
        CloseAction="CloseButton" AllowDragging="True" CloseOnEscape="True">
        <ContentCollection>
            <dx:PopupControlContentControl runat="server">
                <dx:ASPxFormLayout ID="formCuota" runat="server" Width="100%" ColCount="2" Theme="Office2010Blue">
                    <Items>
                        <!-- Campo Entidad eliminado - El sistema usa IdEntidadActual automáticamente -->
                        <dx:LayoutItem Caption="Unidad" RequiredMarkDisplayMode="Required">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxComboBox ID="cboCuotaUnidad" runat="server" ClientInstanceName="cboCuotaUnidad"
                                        Width="100%" Theme="Office2010Blue" ValueType="System.Int32">
                                        <ClientSideEvents SelectedIndexChanged="onCuotaUnidadChanged" />
                                        <ValidationSettings><RequiredField IsRequired="True" ErrorText="Seleccione una unidad" /></ValidationSettings>
                                    </dx:ASPxComboBox>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Concepto de Cuota" RequiredMarkDisplayMode="Required">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxComboBox ID="cboConceptoCuota" runat="server" ClientInstanceName="cboConceptoCuota"
                                        Width="100%" Theme="Office2010Blue" ValueType="System.Int32">
                                        <ValidationSettings><RequiredField IsRequired="True" ErrorText="Seleccione un concepto" /></ValidationSettings>
                                    </dx:ASPxComboBox>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Residente">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxComboBox ID="cboCuotaResidente" runat="server" ClientInstanceName="cboCuotaResidente"
                                        Width="100%" Theme="Office2010Blue" ValueType="System.Int32" />
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Período" RequiredMarkDisplayMode="Required">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxTextBox ID="txtPeriodo" runat="server" ClientInstanceName="txtPeriodo"
                                        Width="100%" Theme="Office2010Blue" MaxLength="7" NullText="YYYY-MM">
                                        <ValidationSettings><RequiredField IsRequired="True" ErrorText="Ingrese el período (YYYY-MM)" /></ValidationSettings>
                                    </dx:ASPxTextBox>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Estado">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxComboBox ID="cboEstado" runat="server" ClientInstanceName="cboEstado"
                                        Width="100%" Theme="Office2010Blue" ValueType="System.String">
                                        <Items>
                                            <dx:ListEditItem Text="Pendiente" Value="Pendiente" Selected="True" />
                                            <dx:ListEditItem Text="Parcial" Value="Parcial" />
                                            <dx:ListEditItem Text="Pagada" Value="Pagada" />
                                            <dx:ListEditItem Text="Vencida" Value="Vencida" />
                                            <dx:ListEditItem Text="Cancelada" Value="Cancelada" />
                                        </Items>
                                    </dx:ASPxComboBox>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Monto" RequiredMarkDisplayMode="Required">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxSpinEdit ID="txtMonto" runat="server" ClientInstanceName="txtMonto"
                                        Width="100%" Theme="Office2010Blue" NumberType="Float" DecimalPlaces="2"
                                        MinValue="0" DisplayFormatString="C2">
                                        <ValidationSettings><RequiredField IsRequired="True" ErrorText="Ingrese el monto" /></ValidationSettings>
                                    </dx:ASPxSpinEdit>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Descuento">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxSpinEdit ID="txtDescuento" runat="server" ClientInstanceName="txtDescuento"
                                        Width="100%" Theme="Office2010Blue" NumberType="Float" DecimalPlaces="2"
                                        MinValue="0" DisplayFormatString="C2" />
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Recargo">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxSpinEdit ID="txtRecargo" runat="server" ClientInstanceName="txtRecargo"
                                        Width="100%" Theme="Office2010Blue" NumberType="Float" DecimalPlaces="2"
                                        MinValue="0" DisplayFormatString="C2" />
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Fecha Emisión" RequiredMarkDisplayMode="Required">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxDateEdit ID="dtFechaEmision" runat="server" ClientInstanceName="dtFechaEmision"
                                        Width="100%" Theme="Office2010Blue" DisplayFormatString="dd/MM/yyyy">
                                        <ValidationSettings><RequiredField IsRequired="True" ErrorText="Seleccione la fecha de emisión" /></ValidationSettings>
                                    </dx:ASPxDateEdit>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Fecha Vencimiento" RequiredMarkDisplayMode="Required">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxDateEdit ID="dtFechaVencimiento" runat="server" ClientInstanceName="dtFechaVencimiento"
                                        Width="100%" Theme="Office2010Blue" DisplayFormatString="dd/MM/yyyy">
                                        <ValidationSettings><RequiredField IsRequired="True" ErrorText="Seleccione la fecha de vencimiento" /></ValidationSettings>
                                    </dx:ASPxDateEdit>
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
                    <dx:ASPxButton ID="btnGuardarCuota" runat="server" Text="Guardar Cuota" Theme="Office2010Blue" AutoPostBack="False">
                        <Image IconID="save_save_16x16" />
                        <ClientSideEvents Click="function(s,e){ guardarCuota(); }" />
                    </dx:ASPxButton>
                    <dx:ASPxButton ID="btnCerrarCuota" runat="server" Text="Cancelar" Theme="Office2010Blue" AutoPostBack="False">
                        <ClientSideEvents Click="function(s,e){ popupCuota.Hide(); }" />
                    </dx:ASPxButton>
                </div>
            </dx:PopupControlContentControl>
        </ContentCollection>
    </dx:ASPxPopupControl>

    <!-- POPUP: GENERAR CUOTAS MASIVAS -->
    <dx:ASPxPopupControl ID="popupGenerarMasiva" runat="server" ClientInstanceName="popupGenerarMasiva"
        HeaderText="Generar Cuotas Masivas" Width="600px" Modal="True" 
        PopupHorizontalAlign="WindowCenter" PopupVerticalAlign="WindowCenter"
        CloseAction="CloseButton" AllowDragging="True" CloseOnEscape="True">
        <ContentCollection>
            <dx:PopupControlContentControl runat="server">
                <dx:ASPxFormLayout ID="formGenerarMasiva" runat="server" Width="100%" ColCount="2" Theme="Office2010Blue">
                    <Items>
                        <!-- Campo Entidad eliminado - El sistema usa IdEntidadActual automáticamente -->
                        <dx:LayoutItem Caption="Concepto de Cuota" RequiredMarkDisplayMode="Required" ColSpan="2">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxComboBox ID="cboGenConcepto" runat="server" ClientInstanceName="cboGenConcepto"
                                        Width="100%" Theme="Office2010Blue" ValueType="System.Int32">
                                        <ValidationSettings><RequiredField IsRequired="True" ErrorText="Seleccione un concepto" /></ValidationSettings>
                                    </dx:ASPxComboBox>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Período" RequiredMarkDisplayMode="Required">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxTextBox ID="txtGenPeriodo" runat="server" ClientInstanceName="txtGenPeriodo"
                                        Width="100%" Theme="Office2010Blue" MaxLength="7" NullText="YYYY-MM">
                                        <ValidationSettings><RequiredField IsRequired="True" ErrorText="Ingrese el período (YYYY-MM)" /></ValidationSettings>
                                    </dx:ASPxTextBox>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Fecha Vencimiento" RequiredMarkDisplayMode="Required">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxDateEdit ID="dtGenVencimiento" runat="server" ClientInstanceName="dtGenVencimiento"
                                        Width="100%" Theme="Office2010Blue" DisplayFormatString="dd/MM/yyyy">
                                        <ValidationSettings><RequiredField IsRequired="True" ErrorText="Seleccione la fecha de vencimiento" /></ValidationSettings>
                                    </dx:ASPxDateEdit>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Monto Base" RequiredMarkDisplayMode="Required" ColSpan="2">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxSpinEdit ID="txtGenMonto" runat="server" ClientInstanceName="txtGenMonto"
                                        Width="100%" Theme="Office2010Blue" NumberType="Float" DecimalPlaces="2"
                                        MinValue="0" DisplayFormatString="C2">
                                        <ValidationSettings><RequiredField IsRequired="True" ErrorText="Ingrese el monto base" /></ValidationSettings>
                                    </dx:ASPxSpinEdit>
                                    <small class="text-muted">Este monto se aplicará a todas las unidades. Las cuotas individuales pueden ajustarse después.</small>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                    </Items>
                </dx:ASPxFormLayout>
                <div style="margin-top:15px; text-align:right;">
                    <dx:ASPxButton ID="btnGenerarMasiva" runat="server" Text="Generar Cuotas" Theme="Office2010Blue" AutoPostBack="False">
                        <Image IconID="content_copy_16x16" />
                        <ClientSideEvents Click="function(s,e){ generarCuotasMasivas(); }" />
                    </dx:ASPxButton>
                    <dx:ASPxButton ID="btnCerrarGen" runat="server" Text="Cancelar" Theme="Office2010Blue" AutoPostBack="False">
                        <ClientSideEvents Click="function(s,e){ popupGenerarMasiva.Hide(); }" />
                    </dx:ASPxButton>
                </div>
            </dx:PopupControlContentControl>
        </ContentCollection>
    </dx:ASPxPopupControl>

    <asp:HiddenField ID="hfCuotaId" runat="server" ClientIDMode="Static" Value="0" />

</asp:Content>
