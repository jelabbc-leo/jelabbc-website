<%@ Page Title="Reservaciones" Language="vb" MasterPageFile="~/MasterPages/Jela.Master" AutoEventWireup="false" CodeBehind="Reservaciones.aspx.vb" Inherits="JelaWeb.Reservaciones" %>

<%@ Register Assembly="DevExpress.Web.v22.2, Version=22.2.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web" TagPrefix="dx" %>

<asp:Content ID="ContentHead" ContentPlaceHolderID="head" runat="server">
    <link href="<%= ResolveUrl("~/Content/CSS/reservaciones.css") %>" rel="stylesheet" type="text/css" />
    <script src="<%= ResolveUrl("~/Scripts/app/Operacion/reservaciones.js") %>" type="text/javascript"></script>
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
        <dx:ASPxGridView ID="gridReservaciones" runat="server" ClientInstanceName="gridReservaciones"
            KeyFieldName="Id" Width="100%" Theme="Office2010Blue" AutoGenerateColumns="False"
            OnDataBound="gridReservaciones_DataBound" OnCustomCallback="gridReservaciones_CustomCallback"
            EnableCallBacks="True">
            
            <ClientSideEvents ToolbarItemClick="onToolbarReservacionesClick" RowDblClick="onRowDblClick" />
            
            <SettingsPager Mode="ShowAllRecords" />
            <Settings ShowFilterRow="False" ShowFilterRowMenu="True" ShowGroupPanel="True" />
            <SettingsSearchPanel Visible="True" ShowApplyButton="True" ShowClearButton="True" />
            <SettingsBehavior AllowSort="True" AllowGroup="True" AllowFocusedRow="True" AllowSelectByRowClick="True" />
            <SettingsExport EnableClientSideExportAPI="True" ExcelExportMode="WYSIWYG" FileName="Reservaciones" />
            
            <Toolbars>
                <dx:GridViewToolbar>
                    <SettingsAdaptivity Enabled="True" EnableCollapseRootItemsToIcons="True" />
                    <Items>
                        <dx:GridViewToolbarItem Name="btnNuevaReservacion" Text="Nueva Reservación">
                            <Image IconID="actions_add_16x16" />
                        </dx:GridViewToolbarItem>
                        <dx:GridViewToolbarItem Name="btnEditarReservacion" Text="Editar Reservación">
                            <Image IconID="edit_edit_16x16" />
                        </dx:GridViewToolbarItem>
                        <dx:GridViewToolbarItem Name="btnEliminarReservacion" Text="Eliminar Reservación" BeginGroup="True">
                            <Image IconID="actions_trash_16x16" />
                        </dx:GridViewToolbarItem>
                        <dx:GridViewToolbarItem BeginGroup="True" Command="Refresh" />
                        <dx:GridViewToolbarItem Command="ExportToXlsx" Text="Exportar Excel" />
                        <dx:GridViewToolbarItem Command="ExportToPdf" Text="Exportar PDF" />
                    </Items>
                </dx:GridViewToolbar>
            </Toolbars>
            
            <Columns>
                <dx:GridViewCommandColumn ShowSelectCheckbox="False" VisibleIndex="0" Width="60px" />
            </Columns>
            
        </dx:ASPxGridView>
        <dx:ASPxGridViewExporter ID="gridExporter" runat="server" GridViewID="gridReservaciones" />
    </div>

    <!-- ================================================================== -->
    <!-- POPUP: RESERVACIÓN -->
    <!-- ================================================================== -->
    <dx:ASPxPopupControl ID="popupReservacion" runat="server" ClientInstanceName="popupReservacion"
        HeaderText="Nueva Reservación" Width="900px" Modal="True" 
        PopupHorizontalAlign="WindowCenter" PopupVerticalAlign="WindowCenter"
        CloseAction="CloseButton" AllowDragging="True" CloseOnEscape="True">
        <ContentCollection>
            <dx:PopupControlContentControl runat="server">
                <dx:ASPxFormLayout ID="formReservacion" runat="server" Width="100%" ColCount="2" Theme="Office2010Blue">
                    <Items>
                        <!-- Campo Entidad eliminado - El sistema usa IdEntidadActual automáticamente -->
                        <dx:LayoutItem Caption="Área Común" RequiredMarkDisplayMode="Required">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxComboBox ID="cboAreaComun" runat="server" ClientInstanceName="cboAreaComun"
                                        Width="100%" Theme="Office2010Blue" ValueType="System.Int32">
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
                        <dx:LayoutItem Caption="Residente">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxComboBox ID="cboResidente" runat="server" ClientInstanceName="cboResidente"
                                        Width="100%" Theme="Office2010Blue" ValueType="System.Int32" />
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Fecha Reservación" RequiredMarkDisplayMode="Required">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxDateEdit ID="dteFechaReservacion" runat="server" ClientInstanceName="dteFechaReservacion"
                                        Width="100%" Theme="Office2010Blue" DisplayFormatString="dd/MM/yyyy">
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
                                            <dx:ListEditItem Text="Pendiente" Value="Pendiente" Selected="True" />
                                            <dx:ListEditItem Text="Confirmada" Value="Confirmada" />
                                            <dx:ListEditItem Text="EnUso" Value="EnUso" />
                                            <dx:ListEditItem Text="Completada" Value="Completada" />
                                            <dx:ListEditItem Text="Cancelada" Value="Cancelada" />
                                        </Items>
                                    </dx:ASPxComboBox>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Hora Inicio" RequiredMarkDisplayMode="Required">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxTimeEdit ID="teHoraInicio" runat="server" ClientInstanceName="teHoraInicio"
                                        Width="100%" Theme="Office2010Blue" DisplayFormatString="HH:mm" EditFormat="Time" TimeSectionProperties-Visible="True">
                                        <ValidationSettings><RequiredField IsRequired="True" ErrorText="Requerido" /></ValidationSettings>
                                    </dx:ASPxTimeEdit>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Hora Fin" RequiredMarkDisplayMode="Required">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxTimeEdit ID="teHoraFin" runat="server" ClientInstanceName="teHoraFin"
                                        Width="100%" Theme="Office2010Blue" DisplayFormatString="HH:mm" EditFormat="Time" TimeSectionProperties-Visible="True">
                                        <ValidationSettings><RequiredField IsRequired="True" ErrorText="Requerido" /></ValidationSettings>
                                    </dx:ASPxTimeEdit>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Número de Invitados">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxSpinEdit ID="spNumeroInvitados" runat="server" ClientInstanceName="spNumeroInvitados"
                                        Width="100%" Theme="Office2010Blue" NumberType="Integer" MinValue="0" MaxValue="999" />
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Costo Total">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxSpinEdit ID="spCostoTotal" runat="server" ClientInstanceName="spCostoTotal"
                                        Width="100%" Theme="Office2010Blue" NumberType="Float" DecimalPlaces="2" 
                                        MinValue="0" MaxValue="999999.99" DisplayFormatString="c2" />
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Depósito Pagado">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxSpinEdit ID="spDepositoPagado" runat="server" ClientInstanceName="spDepositoPagado"
                                        Width="100%" Theme="Office2010Blue" NumberType="Float" DecimalPlaces="2" 
                                        MinValue="0" MaxValue="999999.99" DisplayFormatString="c2" />
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Depósito Devuelto">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxSpinEdit ID="spDepositoDevuelto" runat="server" ClientInstanceName="spDepositoDevuelto"
                                        Width="100%" Theme="Office2010Blue" NumberType="Float" DecimalPlaces="2" 
                                        MinValue="0" MaxValue="999999.99" DisplayFormatString="c2" />
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Motivo" ColSpan="2">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxTextBox ID="txtMotivo" runat="server" ClientInstanceName="txtMotivo"
                                        Width="100%" Theme="Office2010Blue" MaxLength="200" />
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
                        <ClientSideEvents Click="function(s,e){ guardarReservacion(); }" />
                    </dx:ASPxButton>
                    <dx:ASPxButton ID="btnCerrar" runat="server" Text="Cancelar" Theme="Office2010Blue" AutoPostBack="False">
                        <ClientSideEvents Click="function(s,e){ popupReservacion.Hide(); }" />
                    </dx:ASPxButton>
                </div>
            </dx:PopupControlContentControl>
        </ContentCollection>
    </dx:ASPxPopupControl>

    <!-- HIDDEN FIELDS -->
    <asp:HiddenField ID="hfId" runat="server" Value="0" ClientIDMode="Static" />

</asp:Content>
