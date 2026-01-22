<%@ Page Title="Conceptos de Cuota" Language="vb" MasterPageFile="~/MasterPages/Jela.Master" AutoEventWireup="false" CodeBehind="ConceptosCuota.aspx.vb" Inherits="JelaWeb.ConceptosCuota" %>

<%@ Register Assembly="DevExpress.Web.v22.2, Version=22.2.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web" TagPrefix="dx" %>

<asp:Content ID="ContentHead" ContentPlaceHolderID="head" runat="server">
    <link href="<%= ResolveUrl("~/Content/CSS/conceptos-cuota.css") %>" rel="stylesheet" type="text/css" />
    <script src="<%= ResolveUrl("~/Scripts/app/Catalogos/conceptos-cuota.js") %>" type="text/javascript"></script>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <div class="container-fluid">

        <!-- GRID PRINCIPAL -->
        <dx:ASPxGridView ID="gridConceptos" runat="server" ClientInstanceName="gridConceptos"
            KeyFieldName="Id" Width="100%" Theme="Office2010Blue" AutoGenerateColumns="False"
            OnDataBound="gridConceptos_DataBound">
            
            <ClientSideEvents ToolbarItemClick="onToolbarConceptosClick" RowDblClick="onRowDblClick" />
            
            <SettingsPager Mode="ShowAllRecords" />
            <Settings ShowFilterRow="False" ShowFilterRowMenu="True" ShowGroupPanel="True" />
            <SettingsSearchPanel Visible="True" ShowApplyButton="True" ShowClearButton="True" />
            <SettingsBehavior AllowSort="True" AllowGroup="True" AllowFocusedRow="True" AllowSelectByRowClick="True" />
            <SettingsExport EnableClientSideExportAPI="True" ExcelExportMode="WYSIWYG" FileName="ConceptosCuota" />
            
            <Toolbars>
                <dx:GridViewToolbar>
                    <SettingsAdaptivity Enabled="True" EnableCollapseRootItemsToIcons="True" />
                    <Items>
                        <dx:GridViewToolbarItem Name="btnNuevo" Text="Nuevo Concepto">
                            <Image IconID="actions_add_16x16" />
                        </dx:GridViewToolbarItem>
                        <dx:GridViewToolbarItem Name="btnEditar" Text="Editar Concepto">
                            <Image IconID="edit_edit_16x16" />
                        </dx:GridViewToolbarItem>
                        <dx:GridViewToolbarItem Name="btnEliminar" Text="Eliminar Concepto" BeginGroup="True">
                            <Image IconID="actions_trash_16x16" />
                        </dx:GridViewToolbarItem>
                        <dx:GridViewToolbarItem BeginGroup="True" Command="Refresh" />
                        <dx:GridViewToolbarItem Command="ExportToXlsx" Text="Exportar Excel" />
                    </Items>
                </dx:GridViewToolbar>
            </Toolbars>
            
            <Columns>
                <%-- Columnas generadas dinámicamente desde el API - Solo GridViewCommandColumn - las demás columnas se generan dinámicamente --%>
            </Columns>
            
        </dx:ASPxGridView>
        <dx:ASPxGridViewExporter ID="gridExporter" runat="server" GridViewID="gridConceptos" />

    </div>

    <!-- POPUP: NUEVO/EDITAR CONCEPTO -->
    <dx:ASPxPopupControl ID="popupConcepto" runat="server" ClientInstanceName="popupConcepto"
        HeaderText="Nuevo Concepto" Width="700px" Height="550px" Modal="True" 
        PopupHorizontalAlign="WindowCenter" PopupVerticalAlign="WindowCenter"
        CloseAction="CloseButton" AllowDragging="True" CloseOnEscape="True">
        <ContentCollection>
            <dx:PopupControlContentControl runat="server">
                <dx:ASPxPageControl ID="tabsConcepto" runat="server" ClientInstanceName="tabsConcepto" 
                    Width="100%" Theme="Office2010Blue" ActiveTabIndex="0">
                    <%-- TAB: DATOS GENERALES --%>
                    <TabPages>
                        <dx:TabPage Text="Datos Generales" Name="tabDatosGenerales">
                            <ContentCollection>
                                <dx:ContentControl runat="server">
                                    <dx:ASPxFormLayout ID="formDatosGenerales" runat="server" Width="100%" ColCount="2" Theme="Office2010Blue">
                                        <Items>
                                            <dx:LayoutItem Caption="Entidad" RequiredMarkDisplayMode="Required">
                                                <LayoutItemNestedControlCollection>
                                                    <dx:LayoutItemNestedControlContainer runat="server">
                                                        <dx:ASPxComboBox ID="cmbEntidad" runat="server" ClientInstanceName="cmbEntidad"
                                                            Width="100%" Theme="Office2010Blue" ValueType="System.Int32">
                                                            <ValidationSettings><RequiredField IsRequired="True" ErrorText="Seleccione una entidad" /></ValidationSettings>
                                                        </dx:ASPxComboBox>
                                                    </dx:LayoutItemNestedControlContainer>
                                                </LayoutItemNestedControlCollection>
                                            </dx:LayoutItem>
                                            <dx:LayoutItem Caption="Clave" RequiredMarkDisplayMode="Required">
                                                <LayoutItemNestedControlCollection>
                                                    <dx:LayoutItemNestedControlContainer runat="server">
                                                        <dx:ASPxTextBox ID="txtClave" runat="server" ClientInstanceName="txtClave"
                                                            Width="100%" Theme="Office2010Blue" MaxLength="20">
                                                            <ValidationSettings><RequiredField IsRequired="True" ErrorText="La clave es requerida" /></ValidationSettings>
                                                        </dx:ASPxTextBox>
                                                    </dx:LayoutItemNestedControlContainer>
                                                </LayoutItemNestedControlCollection>
                                            </dx:LayoutItem>
                                            <dx:LayoutItem Caption="Nombre" RequiredMarkDisplayMode="Required" ColSpan="2">
                                                <LayoutItemNestedControlCollection>
                                                    <dx:LayoutItemNestedControlContainer runat="server">
                                                        <dx:ASPxTextBox ID="txtNombre" runat="server" ClientInstanceName="txtNombre"
                                                            Width="100%" Theme="Office2010Blue" MaxLength="100">
                                                            <ValidationSettings><RequiredField IsRequired="True" ErrorText="El nombre es requerido" /></ValidationSettings>
                                                        </dx:ASPxTextBox>
                                                    </dx:LayoutItemNestedControlContainer>
                                                </LayoutItemNestedControlCollection>
                                            </dx:LayoutItem>
                                            <dx:LayoutItem Caption="Descripción" ColSpan="2">
                                                <LayoutItemNestedControlCollection>
                                                    <dx:LayoutItemNestedControlContainer runat="server">
                                                        <dx:ASPxMemo ID="txtDescripcion" runat="server" ClientInstanceName="txtDescripcion"
                                                            Width="100%" Height="60px" Theme="Office2010Blue" />
                                                    </dx:LayoutItemNestedControlContainer>
                                                </LayoutItemNestedControlCollection>
                                            </dx:LayoutItem>
                                            <dx:LayoutItem Caption="Tipo Cuota">
                                                <LayoutItemNestedControlCollection>
                                                    <dx:LayoutItemNestedControlContainer runat="server">
                                                        <dx:ASPxComboBox ID="cmbTipoCuota" runat="server" ClientInstanceName="cmbTipoCuota"
                                                            Width="100%" Theme="Office2010Blue" ValueType="System.String">
                                                            <Items>
                                                                <dx:ListEditItem Text="Ordinaria" Value="Ordinaria" Selected="True" />
                                                                <dx:ListEditItem Text="Extraordinaria" Value="Extraordinaria" />
                                                                <dx:ListEditItem Text="Servicio" Value="Servicio" />
                                                                <dx:ListEditItem Text="Multa" Value="Multa" />
                                                                <dx:ListEditItem Text="Otro" Value="Otro" />
                                                            </Items>
                                                        </dx:ASPxComboBox>
                                                    </dx:LayoutItemNestedControlContainer>
                                                </LayoutItemNestedControlCollection>
                                            </dx:LayoutItem>
                                            <dx:LayoutItem Caption="Monto Base">
                                                <LayoutItemNestedControlCollection>
                                                    <dx:LayoutItemNestedControlContainer runat="server">
                                                        <dx:ASPxSpinEdit ID="spnMontoBase" runat="server" ClientInstanceName="spnMontoBase"
                                                            Width="100%" Theme="Office2010Blue" NumberType="Float" DecimalPlaces="2"
                                                            MinValue="0" MaxValue="9999999.99" />
                                                    </dx:LayoutItemNestedControlContainer>
                                                </LayoutItemNestedControlCollection>
                                            </dx:LayoutItem>
                                            <dx:LayoutItem Caption="Cuenta Contable">
                                                <LayoutItemNestedControlCollection>
                                                    <dx:LayoutItemNestedControlContainer runat="server">
                                                        <dx:ASPxTextBox ID="txtCuentaContable" runat="server" ClientInstanceName="txtCuentaContable"
                                                            Width="100%" Theme="Office2010Blue" MaxLength="20" />
                                                    </dx:LayoutItemNestedControlContainer>
                                                </LayoutItemNestedControlCollection>
                                            </dx:LayoutItem>
                                            <dx:LayoutItem Caption="Activo">
                                                <LayoutItemNestedControlCollection>
                                                    <dx:LayoutItemNestedControlContainer runat="server">
                                                        <dx:ASPxCheckBox ID="chkActivo" runat="server" ClientInstanceName="chkActivo"
                                                            Text="Concepto activo" Checked="True" Theme="Office2010Blue" />
                                                    </dx:LayoutItemNestedControlContainer>
                                                </LayoutItemNestedControlCollection>
                                            </dx:LayoutItem>
                                        </Items>
                                    </dx:ASPxFormLayout>
                                </dx:ContentControl>
                            </ContentCollection>
                        </dx:TabPage>
                        <%-- TAB: CONFIGURACIÓN DE COBRO --%>
                        <dx:TabPage Text="Configuración de Cobro" Name="tabConfiguracion">
                            <ContentCollection>
                                <dx:ContentControl runat="server">
                                    <dx:ASPxFormLayout ID="formConfiguracion" runat="server" Width="100%" ColCount="2" Theme="Office2010Blue">
                                        <Items>
                                            <dx:LayoutItem Caption="Es Recurrente">
                                                <LayoutItemNestedControlCollection>
                                                    <dx:LayoutItemNestedControlContainer runat="server">
                                                        <dx:ASPxCheckBox ID="chkEsRecurrente" runat="server" ClientInstanceName="chkEsRecurrente"
                                                            Text="Se genera automáticamente cada periodo" Checked="True" Theme="Office2010Blue" />
                                                    </dx:LayoutItemNestedControlContainer>
                                                </LayoutItemNestedControlCollection>
                                            </dx:LayoutItem>
                                            <dx:LayoutItem Caption="Periodicidad">
                                                <LayoutItemNestedControlCollection>
                                                    <dx:LayoutItemNestedControlContainer runat="server">
                                                        <dx:ASPxComboBox ID="cmbPeriodicidad" runat="server" ClientInstanceName="cmbPeriodicidad"
                                                            Width="100%" Theme="Office2010Blue" ValueType="System.String">
                                                            <Items>
                                                                <dx:ListEditItem Text="Mensual" Value="Mensual" Selected="True" />
                                                                <dx:ListEditItem Text="Bimestral" Value="Bimestral" />
                                                                <dx:ListEditItem Text="Trimestral" Value="Trimestral" />
                                                                <dx:ListEditItem Text="Semestral" Value="Semestral" />
                                                                <dx:ListEditItem Text="Anual" Value="Anual" />
                                                                <dx:ListEditItem Text="Único" Value="Unico" />
                                                            </Items>
                                                        </dx:ASPxComboBox>
                                                    </dx:LayoutItemNestedControlContainer>
                                                </LayoutItemNestedControlCollection>
                                            </dx:LayoutItem>
                                            <dx:LayoutItem Caption="Día Vencimiento">
                                                <LayoutItemNestedControlCollection>
                                                    <dx:LayoutItemNestedControlContainer runat="server">
                                                        <dx:ASPxSpinEdit ID="spnDiaVencimiento" runat="server" ClientInstanceName="spnDiaVencimiento"
                                                            Width="100%" Theme="Office2010Blue" NumberType="Integer"
                                                            MinValue="1" MaxValue="31" Number="10" />
                                                    </dx:LayoutItemNestedControlContainer>
                                                </LayoutItemNestedControlCollection>
                                            </dx:LayoutItem>
                                            <dx:LayoutItem Caption="Días de Gracia">
                                                <LayoutItemNestedControlCollection>
                                                    <dx:LayoutItemNestedControlContainer runat="server">
                                                        <dx:ASPxSpinEdit ID="spnDiasGracia" runat="server" ClientInstanceName="spnDiasGracia"
                                                            Width="100%" Theme="Office2010Blue" NumberType="Integer"
                                                            MinValue="0" MaxValue="30" Number="5" />
                                                    </dx:LayoutItemNestedControlContainer>
                                                </LayoutItemNestedControlCollection>
                                            </dx:LayoutItem>
                                            <dx:LayoutGroup Caption="Recargos" ColSpan="2" ColCount="2">
                                                <Items>
                                                    <dx:LayoutItem Caption="Aplica Recargo">
                                                        <LayoutItemNestedControlCollection>
                                                            <dx:LayoutItemNestedControlContainer runat="server">
                                                                <dx:ASPxCheckBox ID="chkAplicaRecargo" runat="server" ClientInstanceName="chkAplicaRecargo"
                                                                    Text="Aplica recargo por mora" Checked="True" Theme="Office2010Blue" />
                                                            </dx:LayoutItemNestedControlContainer>
                                                        </LayoutItemNestedControlCollection>
                                                    </dx:LayoutItem>
                                                    <dx:LayoutItem Caption="% Recargo">
                                                        <LayoutItemNestedControlCollection>
                                                            <dx:LayoutItemNestedControlContainer runat="server">
                                                                <dx:ASPxSpinEdit ID="spnPorcentajeRecargo" runat="server" ClientInstanceName="spnPorcentajeRecargo"
                                                                    Width="100%" Theme="Office2010Blue" NumberType="Float" DecimalPlaces="2"
                                                                    MinValue="0" MaxValue="100" Number="5" />
                                                            </dx:LayoutItemNestedControlContainer>
                                                        </LayoutItemNestedControlCollection>
                                                    </dx:LayoutItem>
                                                </Items>
                                            </dx:LayoutGroup>
                                            <dx:LayoutGroup Caption="Descuento Pronto Pago" ColSpan="2" ColCount="3">
                                                <Items>
                                                    <dx:LayoutItem Caption="Aplica Descuento">
                                                        <LayoutItemNestedControlCollection>
                                                            <dx:LayoutItemNestedControlContainer runat="server">
                                                                <dx:ASPxCheckBox ID="chkAplicaDescuento" runat="server" ClientInstanceName="chkAplicaDescuento"
                                                                    Text="Aplica descuento" Theme="Office2010Blue" />
                                                            </dx:LayoutItemNestedControlContainer>
                                                        </LayoutItemNestedControlCollection>
                                                    </dx:LayoutItem>
                                                    <dx:LayoutItem Caption="% Descuento">
                                                        <LayoutItemNestedControlCollection>
                                                            <dx:LayoutItemNestedControlContainer runat="server">
                                                                <dx:ASPxSpinEdit ID="spnPorcentajeDescuento" runat="server" ClientInstanceName="spnPorcentajeDescuento"
                                                                    Width="100%" Theme="Office2010Blue" NumberType="Float" DecimalPlaces="2"
                                                                    MinValue="0" MaxValue="100" Number="0" />
                                                            </dx:LayoutItemNestedControlContainer>
                                                        </LayoutItemNestedControlCollection>
                                                    </dx:LayoutItem>
                                                    <dx:LayoutItem Caption="Día Límite">
                                                        <LayoutItemNestedControlCollection>
                                                            <dx:LayoutItemNestedControlContainer runat="server">
                                                                <dx:ASPxSpinEdit ID="spnDiaLimiteDescuento" runat="server" ClientInstanceName="spnDiaLimiteDescuento"
                                                                    Width="100%" Theme="Office2010Blue" NumberType="Integer"
                                                                    MinValue="1" MaxValue="31" Number="5" />
                                                            </dx:LayoutItemNestedControlContainer>
                                                        </LayoutItemNestedControlCollection>
                                                    </dx:LayoutItem>
                                                </Items>
                                            </dx:LayoutGroup>
                                        </Items>
                                    </dx:ASPxFormLayout>
                                </dx:ContentControl>
                            </ContentCollection>
                        </dx:TabPage>
                        
                    </TabPages>
                </dx:ASPxPageControl>
                
                <div class="popup-footer" style="margin-top: 15px; text-align: right;">
                    <dx:ASPxButton ID="btnGuardar" runat="server" Text="Guardar" Theme="Office2010Blue" 
                        AutoPostBack="True" OnClick="btnGuardar_Click">
                        <Image IconID="save_save_16x16" />
                    </dx:ASPxButton>
                    <dx:ASPxButton ID="btnCancelar" runat="server" Text="Cancelar" Theme="Office2010Blue" AutoPostBack="False">
                        <Image IconID="actions_cancel_16x16" />
                        <ClientSideEvents Click="function(s, e) { popupConcepto.Hide(); }" />
                    </dx:ASPxButton>
                </div>
            </dx:PopupControlContentControl>
        </ContentCollection>
    </dx:ASPxPopupControl>
    
    <!-- HIDDEN FIELDS -->
    <asp:HiddenField ID="hfConceptoId" runat="server" Value="0" />

</asp:Content>
